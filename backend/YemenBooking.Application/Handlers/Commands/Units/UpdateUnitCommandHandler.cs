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
using System.Linq;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Enums;
using System.IO;

namespace YemenBooking.Application.Handlers.Commands.Units
{
    /// <summary>
    /// معالج أمر تحديث بيانات الوحدة
    /// </summary>
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, ResultDto<bool>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUnitFieldValueRepository _valueRepository;
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateUnitCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPropertyImageRepository _propertyImageRepository;

        public UpdateUnitCommandHandler(
            IUnitRepository unitRepository,
            IPropertyRepository propertyRepository,
            IUnitFieldValueRepository valueRepository,
            IUnitOfWork unitOfWork,
            IUnitTypeFieldRepository fieldRepository,
            IFileStorageService fileStorageService,
            IPropertyImageRepository propertyImageRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateUnitCommandHandler> logger)
        {
            _unitRepository = unitRepository;
            _propertyRepository = propertyRepository;
            _fieldRepository = fieldRepository;
            _valueRepository = valueRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _propertyImageRepository = propertyImageRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث بيانات الوحدة: UnitId={UnitId}", request.UnitId);

            // التحقق من المدخلات
            if (request.UnitId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الوحدة مطلوب");
            if (request.Name != null && string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<bool>.Failed("اسم الوحدة المطلوب غير صالح");
            if (request.BasePrice != null && request.BasePrice.Amount <= 0)
                return ResultDto<bool>.Failed("السعر الأساسي يجب أن يكون أكبر من صفر");

            // التحقق من الوجود
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
            if (unit == null)
                return ResultDto<bool>.Failed("الوحدة غير موجودة");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            var property = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالوحدة غير موجود");
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث بيانات هذه الوحدة");

            // التحقق من التكرار عند تغيير الاسم
            if (!string.IsNullOrWhiteSpace(request.Name) && !string.Equals(unit.Name, request.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                bool duplicate = await _unitRepository.ExistsAsync(u => u.PropertyId == unit.PropertyId && u.Name.Trim() == request.Name.Trim() && u.Id != request.UnitId, cancellationToken);
                if (duplicate)
                    return ResultDto<bool>.Failed("يوجد وحدة أخرى بنفس الاسم في هذا الكيان");
                unit.Name = request.Name.Trim();
            }

            // التحقق من صحة قيم الحقول الديناميكية حسب التعريفات
            var fieldDefs = await _fieldRepository.GetFieldsByUnitTypeIdAsync(unit.UnitTypeId, cancellationToken);
            foreach (var def in fieldDefs)
            {
                var dto = request.FieldValues.FirstOrDefault(f => f.FieldId == def.Id);
                if (def.IsRequired && (dto == null || string.IsNullOrWhiteSpace(dto.FieldValue)))
                    return ResultDto<bool>.Failed($"الحقل {def.DisplayName} مطلوب.");
                if (dto != null && (def.FieldTypeId == "number" || def.FieldTypeId == "currency" || def.FieldTypeId == "percentage" || def.FieldTypeId == "range"))
                {
                    if (!decimal.TryParse(dto.FieldValue, out _))
                        return ResultDto<bool>.Failed($"قيمة الحقل {def.DisplayName} يجب أن تكون رقمًا.");
                }
            }
            // تحديث الوحدة وقيم الحقول الديناميكية في معاملة واحدة
            bool success = false;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // تطبيق التحديثات الممكنة على الوحدة
                if (request.BasePrice != null)
                    unit.BasePrice = new Money(request.BasePrice.Amount, request.BasePrice.Currency);
                if (!string.IsNullOrWhiteSpace(request.CustomFeatures))
                    unit.CustomFeatures = request.CustomFeatures.Trim();
                if (request.PricingMethod.HasValue)
                    unit.PricingMethod = request.PricingMethod.Value;

                unit.UpdatedBy = _currentUserService.UserId;
                unit.UpdatedAt = DateTime.UtcNow;

                await _unitRepository.UpdateUnitAsync(unit, cancellationToken);

                // جلب القيم الحالية للحقل
                var existingValues = (await _valueRepository.GetValuesByUnitIdAsync(request.UnitId, cancellationToken))
                    .ToDictionary(v => v.UnitTypeFieldId);
                var incomingIds = request.FieldValues.Select(f => f.FieldId).ToHashSet();

                // تحديث أو إنشاء قيم الحقول الديناميكية
                foreach (var dto in request.FieldValues)
                {
                    if (existingValues.TryGetValue(dto.FieldId, out var entity))
                    {
                        entity.FieldValue = dto.FieldValue;
                        entity.UpdatedBy = _currentUserService.UserId;
                        entity.UpdatedAt = DateTime.UtcNow;
                        await _valueRepository.UpdateUnitFieldValueAsync(entity, cancellationToken);
                    }
                    else
                    {
                        if (dto.FieldId == Guid.Empty)
                            throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");
                        var newValue = new UnitFieldValue
                        {
                            UnitId = unit.Id,
                            UnitTypeFieldId = dto.FieldId,
                            FieldValue = dto.FieldValue,
                            CreatedBy = _currentUserService.UserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _valueRepository.CreateUnitFieldValueAsync(newValue, cancellationToken);
                    }
                }

                // حذف القيم التي أزيلت
                foreach (var kv in existingValues)
                {
                    if (!incomingIds.Contains(kv.Key))
                        await _valueRepository.DeleteUnitFieldValueAsync(kv.Value.Id, cancellationToken);
                }

                // تسجيل التدقيق
                await _auditService.LogBusinessOperationAsync(
                    "UpdateUnitWithFields",
                    $"تم تحديث بيانات الوحدة {request.UnitId} مع قيم الحقول الديناميكية",
                    request.UnitId,
                    "Unit",
                    _currentUserService.UserId,
                    null,
                    cancellationToken);

                _logger.LogInformation("اكتمل تحديث بيانات الوحدة بنجاح: UnitId={UnitId}", request.UnitId);
                success = true;
            });
            // نقل الصور المؤقتة المحددة في الكوماند للوحدة
            _logger.LogInformation("نقل الصور المؤقتة المحددة في الكوماند للوحدة: {UnitId}", request.UnitId);
                        // حذف الصور التي أزيلت من الكيان
            var existingUnitImages = (await _propertyImageRepository.GetImagesByUnitAsync(request.UnitId, cancellationToken)).ToList();
            var unitImagesToDelete = existingUnitImages.Where(img => request.Images == null || !request.Images.Contains(img.Url)).ToList();
            foreach (var img in unitImagesToDelete)
            {
                // حذف الملف من التخزين
                var uri = new Uri(img.Url);
                var filePath = uri.AbsolutePath.TrimStart('/');
                await _fileStorageService.DeleteFileAsync(filePath, cancellationToken);
                // حذف السجل من قاعدة البيانات
                await _propertyImageRepository.DeletePropertyImageAsync(img.Id, cancellationToken);
            }
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
                    var destFolderPath = $"{persistentFolder}/{unit.PropertyId}/{request.UnitId}";                            // e.g. "Management/{propertyId}/{unitId}"
                    var destRelativePath = $"{destFolderPath}/{fileName}";
                    // العثور على السجل المؤقت المطابق للمسار في DB
                    var img = tempImages.FirstOrDefault(i => i.Url == relativePath);
                    if (img == null) continue;
                    // نقل الملف وإنشاء المجلد الوجهة إذا لزم الأمر
                    await _fileStorageService.MoveFileAsync(sourceRelativePath, destRelativePath, cancellationToken);
                    var newUrl = await _fileStorageService.GetFileUrlAsync(destRelativePath, null, cancellationToken);
                    img.PropertyId = unit.PropertyId;
                    img.UnitId = request.UnitId;
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
            return ResultDto<bool>.Succeeded(success, "تم تحديث بيانات الوحدة بنجاح مع قيم الحقول الديناميكية");
        }
    }
} 