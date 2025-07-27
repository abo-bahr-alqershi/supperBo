using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Properties;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Notifications;
using YemenBooking.Core.Enums;
using System.IO;
using System.Linq;

namespace YemenBooking.Application.Handlers.Commands.Properties
{
    /// <summary>
    /// معالج أمر إنشاء كيان جديد
    /// </summary>
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, ResultDto<Guid>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IIndexingService _indexingService;
        private readonly ILogger<CreatePropertyCommandHandler> _logger;

        public CreatePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IRoleRepository roleRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            IAuditService auditService,
            IFileStorageService fileStorageService,
            IPropertyImageRepository propertyImageRepository,
            IIndexingService indexingService,
            ILogger<CreatePropertyCommandHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _auditService = auditService;
            _fileStorageService = fileStorageService;
            _propertyImageRepository = propertyImageRepository;
            _indexingService = indexingService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            // Prevent duplicate property ownership
            var existingProperties = await _propertyRepository.GetPropertiesByOwnerAsync(request.OwnerId, cancellationToken);
            if (existingProperties.Any())
                return ResultDto<Guid>.Failed("المستخدم مالك كيان بالفعل");
            _logger.LogInformation("بدء إنشاء كيان جديد: Name={Name}, OwnerId={OwnerId}", request.Name, request.OwnerId);

            // التحقق من صحة المدخلات
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failed("اسم الكيان مطلوب");
            if (string.IsNullOrWhiteSpace(request.Address))
                return ResultDto<Guid>.Failed("عنوان الكيان مطلوب");
            if (request.OwnerId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف المالك مطلوب");
            if (request.PropertyTypeId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف نوع الكيان مطلوب");
            if (string.IsNullOrWhiteSpace(request.City))
                return ResultDto<Guid>.Failed("اسم المدينة مطلوب");
            if (request.StarRating < 1 || request.StarRating > 5)
                return ResultDto<Guid>.Failed("تقييم النجوم يجب أن يكون بين 1 و 5");
            if (request.Latitude < -90 || request.Latitude > 90)
                return ResultDto<Guid>.Failed("خط العرض يجب أن يكون بين -90 و 90");
            if (request.Longitude < -180 || request.Longitude > 180)
                return ResultDto<Guid>.Failed("خط الطول يجب أن يكون بين -180 و 180");

            // التحقق من وجود المالك ونوع الكيان
            var owner = await _propertyRepository.GetOwnerByIdAsync(request.OwnerId, cancellationToken);
            if (owner == null)
                return ResultDto<Guid>.Failed("المالك غير موجود");
            var propertyType = await _propertyRepository.GetPropertyTypeByIdAsync(request.PropertyTypeId, cancellationToken);
            if (propertyType == null)
                return ResultDto<Guid>.Failed("نوع الكيان غير موجود");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            if (_currentUserService.Role != "Admin" && request.OwnerId != _currentUserService.UserId)
                return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء كيان جديد");

            // إنشاء الكيان بحالة انتظار الموافقة
            var property = new Property
            {
                OwnerId = request.OwnerId,
                TypeId = request.PropertyTypeId,
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                City = request.City,
                Latitude = (decimal)request.Latitude,
                Longitude = (decimal)request.Longitude,
                StarRating = request.StarRating,
                IsApproved = false,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _propertyRepository.CreatePropertyAsync(property, cancellationToken);

            // Assign 'Owner' role to the user
            var allRoles = await _roleRepository.GetAllRolesAsync(cancellationToken);
            var ownerRole = allRoles.FirstOrDefault(r => r.Name.Equals("Owner", StringComparison.OrdinalIgnoreCase));
            if (ownerRole != null)
                await _roleRepository.AssignRoleToUserAsync(request.OwnerId, ownerRole.Id, cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreateProperty",
                $"تم إنشاء الكيان جديد {created.Id} باسم {created.Name}",
                created.Id,
                "Property",
                _currentUserService.UserId,
                null,
                cancellationToken);

            // إرسال إشعار للمراجعة إلى المالك
            await _notificationService.SendAsync(new NotificationRequest
            {
                UserId = request.OwnerId,
                Type = NotificationType.BookingCreated,
                Title = "تم إنشاء الكيان وينتظر الموافقة",
                Message = $"تم إنشاء الكيان '{created.Name}' ويحتاج إلى موافقة الإدارة"
            }, cancellationToken);

            _logger.LogInformation("اكتمل إنشاء الكيان: PropertyId={PropertyId}", created.Id);

            // نقل الصور المؤقتة المحددة في الكوماند إلى المسار الرسمي بعد إنشاء الكيان
            if (request.Images != null && request.Images.Any())
            {
                // جمع المسارات النسبية للصور من الطلب (بما في ذلك الشريط المائل الأمامي)
                var relativePaths = request.Images.Select(imagePath =>
                {
                    // Get absolute path (including leading slash) then unescape
                    var absolutePath = Uri.TryCreate(imagePath, UriKind.Absolute, out var uriRes)
                        ? uriRes.AbsolutePath
                        : (imagePath.StartsWith("/") ? imagePath : "/" + imagePath);
                    return Uri.UnescapeDataString(absolutePath);
                }).ToList();
                // جلب سجلات الصور المؤقتة حسب المسار فقط
                var tempImages = await _propertyImageRepository.GetImagesByPathAsync(relativePaths, cancellationToken);
                foreach (var imagePath in request.Images)
                {
                    // استخراج المسار النسبي كاملاً وإلغاء ترميز الـ URL
                    string absolutePath = Uri.TryCreate(imagePath, UriKind.Absolute, out var uriRes)
                        ? uriRes.AbsolutePath
                        : (imagePath.StartsWith("/") ? imagePath : "/" + imagePath);
                    string relativePath = Uri.UnescapeDataString(absolutePath);
                    var segments = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    // نحتاج على الأقل: ["uploads", "folder", "filename"]
                    if (segments.Length < 3) continue;
                    // استخراج المسار الفرعي دون بادئة "uploads"
                    var folderSegments = segments.Skip(1).Take(segments.Length - 2);
                    var fileName = segments[^1];
                    // استخدم مجلد "Management" للصور الدائمة
                    var tempFolder = string.Join("/", folderSegments);                                        // e.g. "temp"
                    var sourceRelativePath = $"{tempFolder}/{fileName}";                                       // e.g. "temp/file.png"
                    var persistentFolder = ImageType.Management.ToString();                                    // e.g. "Management"
                    var destFolderPath = $"{persistentFolder}/{created.Id}";                                  // e.g. "Management/{propertyId}"
                    var destRelativePath = $"{destFolderPath}/{fileName}";
                    // العثور على السجل المؤقت المطابق للمسار في DB
                    var img = tempImages.FirstOrDefault(i => i.Url == relativePath);
                    if (img == null) continue;
                    // نقل الملف وإنشاء المجلد الوجهة إذا لزم الأمر
                    await _fileStorageService.MoveFileAsync(sourceRelativePath, destRelativePath, cancellationToken);
                    var newUrl = await _fileStorageService.GetFileUrlAsync(destRelativePath, null, cancellationToken);
                    img.PropertyId = created.Id;
                    img.Url = newUrl;
                    img.Sizes = newUrl;
                    await _propertyImageRepository.UpdatePropertyImageAsync(img, cancellationToken);
                    // Move thumbnail files if exist
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    var ext = Path.GetExtension(fileName);
                    var thumbnailSuffixes = new[] { "_thumb", "_thumb48", "_thumb64" };
                    foreach (var suffix in thumbnailSuffixes)
                    {
                        var thumbSource = $"{tempFolder}/{nameWithoutExt}{suffix}{ext}";
                        var thumbDest = $"{destFolderPath}/{nameWithoutExt}{suffix}{ext}";
                        await _fileStorageService.MoveFileAsync(thumbSource, thumbDest, cancellationToken);
                    }
                }
            }

            // فهرسة العقار الجديد
            try
            {
                await _indexingService.IndexPropertyAsync(created);
                _logger.LogInformation("تم فهرسة العقار الجديد {PropertyId} بنجاح", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في فهرسة العقار {PropertyId}، سيتم المحاولة لاحقاً", created.Id);
                // لا نفشل العملية إذا فشلت الفهرسة
            }

            return ResultDto<Guid>.Succeeded(created.Id, "تم إنشاء الكيان بنجاح وينتظر الموافقة");
        }
    }
} 