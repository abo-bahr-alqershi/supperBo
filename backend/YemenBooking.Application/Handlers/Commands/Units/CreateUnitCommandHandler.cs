using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Application.Exceptions;
using System.Linq;
using YemenBooking.Core.Enums;
using System.IO;

namespace YemenBooking.Application.Handlers.Commands.Units
{
    /// <summary>
    /// معالج أمر إنشاء وحدة جديدة في الكيان
    /// </summary>
    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, ResultDto<Guid>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUnitTypeRepository _unitTypeRepository;
        private readonly IUnitFieldValueRepository _valueRepository;
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IIndexingService _indexingService;
        private readonly ILogger<CreateUnitCommandHandler> _logger;

        public CreateUnitCommandHandler(
            IUnitRepository unitRepository,
            IPropertyRepository propertyRepository,
            IUnitTypeRepository unitTypeRepository,
            IUnitFieldValueRepository valueRepository,
            IUnitOfWork unitOfWork,
            IUnitTypeFieldRepository fieldRepository,
            IFileStorageService fileStorageService,
            IPropertyImageRepository propertyImageRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IIndexingService indexingService,
            ILogger<CreateUnitCommandHandler> logger)
        {
            _unitRepository = unitRepository;
            _propertyRepository = propertyRepository;
            _fieldRepository = fieldRepository;
            _unitTypeRepository = unitTypeRepository;
            _valueRepository = valueRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _propertyImageRepository = propertyImageRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء وحدة في الكيان: PropertyId={PropertyId}, Name={Name}", request.PropertyId, request.Name);

            // التحقق من المدخلات
            if (request.PropertyId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف الكيان مطلوب");
            if (request.UnitTypeId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف نوع الوحدة مطلوب");
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failed("اسم الوحدة مطلوب");
            if (request.BasePrice == null || request.BasePrice.Amount <= 0)
                return ResultDto<Guid>.Failed("السعر الأساسي يجب أن يكون أكبر من صفر");

            // التحقق من وجود الكيان والنوع
            var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<Guid>.Failed("الكيان غير موجود");
            var unitType = await _unitTypeRepository.GetUnitTypeByIdAsync(request.UnitTypeId, cancellationToken);
            if (unitType == null)
                return ResultDto<Guid>.Failed("نوع الوحدة غير موجود");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء وحدة في هذا الكيان");

            // التحقق من التكرار
            bool exists = await _unitRepository.ExistsAsync(u => u.PropertyId == request.PropertyId && u.Name.Trim() == request.Name.Trim(), cancellationToken);
            if (exists)
                return ResultDto<Guid>.Failed("يوجد وحدة بنفس الاسم في هذا الكيان");
            // التحقق من صحة قيم الحقول الديناميكية حسب التعريفات
            var fieldDefs = await _fieldRepository.GetFieldsByUnitTypeIdAsync(request.UnitTypeId, cancellationToken);
            foreach (var def in fieldDefs)
            {
                var dto = request.FieldValues.FirstOrDefault(f => f.FieldId == def.Id);
                if (def.IsRequired && (dto == null || string.IsNullOrWhiteSpace(dto.FieldValue)))
                    return ResultDto<Guid>.Failed($"الحقل {def.DisplayName} مطلوب.");
                if (dto != null && (def.FieldTypeId == "number" || def.FieldTypeId == "currency" || def.FieldTypeId == "percentage" || def.FieldTypeId == "range"))
                {
                    if (!decimal.TryParse(dto.FieldValue, out _))
                        return ResultDto<Guid>.Failed($"قيمة الحقل {def.DisplayName} يجب أن تكون رقمًا.");
                }
            }

            // إنشاء الوحدة مع القيم الديناميكية في معاملة واحدة
            Guid createdId = Guid.Empty;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // إنشاء الكيان
                var unit = new YemenBooking.Core.Entities.Unit
                {
                    PropertyId = request.PropertyId,
                    UnitTypeId = request.UnitTypeId,
                    Name = request.Name.Trim(),
                    BasePrice = new Money(request.BasePrice.Amount, request.BasePrice.Currency),
                    MaxCapacity = unitType.MaxCapacity,
                    CustomFeatures = request.CustomFeatures.Trim(),
                    PricingMethod = request.PricingMethod,
                    IsAvailable = true,
                    CreatedBy = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                var created = await _unitRepository.CreateUnitAsync(unit, cancellationToken);
                createdId = created.Id;

                // إنشاء قيم الحقول الديناميكية
                foreach (var dto in request.FieldValues)
                {
                    if (dto.FieldId == Guid.Empty)
                        throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");

                    var newValue = new UnitFieldValue
                    {
                        UnitId = created.Id,
                        UnitTypeFieldId = dto.FieldId,
                        FieldValue = dto.FieldValue,
                        CreatedBy = _currentUserService.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _valueRepository.CreateUnitFieldValueAsync(newValue, cancellationToken);
                }

                // تسجيل التدقيق
                await _auditService.LogBusinessOperationAsync(
                    "CreateUnitWithFields",
                    $"تم إنشاء وحدة جديدة {created.Id} باسم {created.Name} مع قيم الحقول الديناميكية",
                    created.Id,
                    "Unit",
                    _currentUserService.UserId,
                    null,
                    cancellationToken);

                _logger.LogInformation("اكتمل إنشاء الوحدة بنجاح: UnitId={UnitId}", created.Id);
            });

            // نقل الصور المؤقتة إلى المسار الرسمي بعد إنشاء الوحدة
            _logger.LogInformation("نقل الصور المؤقتة المحددة في الكوماند للوحدة: {UnitId}", createdId);
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
                    var tempFolder = string.Join("/", folderSegments);                                                   // e.g. "temp"
                    var sourceRelativePath = $"{tempFolder}/{fileName}";                                                  // e.g. "temp/file.png"
                    var persistentFolder = ImageType.Management.ToString();                                                   // e.g. "Management"
                    var destFolderPath = $"{persistentFolder}/{request.PropertyId}/{createdId}";                           // e.g. "Management/{propertyId}/{unitId}"
                    var destRelativePath = $"{destFolderPath}/{fileName}";
                    // العثور على السجل المؤقت المطابق للمسار في DB
                    var img = tempImages.FirstOrDefault(i => i.Url == relativePath);
                    if (img == null) continue;
                    // نقل الملف وإنشاء المجلد الوجهة إذا لزم الأمر
                    await _fileStorageService.MoveFileAsync(sourceRelativePath, destRelativePath, cancellationToken);
                    var newUrl = await _fileStorageService.GetFileUrlAsync(destRelativePath, null, cancellationToken);
                    img.PropertyId = request.PropertyId;
                    img.UnitId = createdId;
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

            // فهرسة الوحدة الجديدة مع الحقول الديناميكية
            try
            {
                // جلب الوحدة مع البيانات المرتبطة للفهرسة
                var unitForIndexing = await _unitRepository.GetByIdWithRelatedDataAsync(createdId, cancellationToken);
                if (unitForIndexing != null)
                {
                    await _indexingService.IndexUnitAsync(unitForIndexing);
                    _logger.LogInformation("تم فهرسة الوحدة الجديدة {UnitId} بنجاح", createdId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في فهرسة الوحدة {UnitId}، سيتم المحاولة لاحقاً", createdId);
                // لا نفشل العملية إذا فشلت الفهرسة
            }

            return ResultDto<Guid>.Succeeded(createdId, "تم إنشاء الوحدة بنجاح مع قيم الحقول الديناميكية");
        }
    }
} 