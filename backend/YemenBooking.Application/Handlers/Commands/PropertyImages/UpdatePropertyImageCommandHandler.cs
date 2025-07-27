using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.PropertyImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Notifications;
using CoreUnit = YemenBooking.Core.Entities.Unit;
using YemenBooking.Application.DTOs;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.PropertyImages;

/// <summary>
/// معالج أمر تحديث صورة الكيان
/// Property image update command handler
/// 
/// يعالج تحديث صورة كيان موجودة ويتضمن:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الصورة
/// - التحقق من صلاحيات المستخدم
/// - تحديث بيانات الصورة
/// - معالجة الصورة الجديدة (إذا لزم الأمر)
/// - إنشاء حدث تحديث الصورة
/// - إرسال الإشعارات اللازمة
/// 
/// Updates an existing property image and includes:
/// - Input data validation
/// - Image existence validation
/// - User authorization validation
/// - Image data update
/// - New image processing (if required)
/// - Creating image updated event
/// - Sending necessary notifications
/// </summary>
public class UpdatePropertyImageCommandHandler : IRequestHandler<UpdatePropertyImageCommand, ResultDto<bool>>
{
    #region Dependencies
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UpdatePropertyImageCommandHandler> _logger;
    #endregion

    #region Constructor
    /// <summary>
    /// المنشئ - حقن التبعيات
    /// Constructor - Dependency injection
    /// </summary>
    public UpdatePropertyImageCommandHandler(
        IPropertyImageRepository propertyImageRepository,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        IImageProcessingService imageProcessingService,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        INotificationService notificationService,
        ILogger<UpdatePropertyImageCommandHandler> logger)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
        _imageProcessingService = imageProcessingService;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _validationService = validationService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _notificationService = notificationService;
        _logger = logger;
    }
    #endregion

    #region Main Handler

    /// <summary>
    /// معالج الأمر الرئيسي
    /// Main command handler
    /// </summary>
    public async Task<ResultDto<bool>> Handle(UpdatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة تحديث صورة الكيان - ImageId: {ImageId}", request.ImageId);

            // التحقق من صحة البيانات المدخلة
            // Validate input data
            var validationResult = await ValidateRequestAsync(request, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود الصورة
            // Check image existence
            var existingImage = await _propertyImageRepository.GetPropertyImageByIdAsync(request.ImageId, cancellationToken);
            if (existingImage == null)
            {
                return ResultDto<bool>.Failed(["الصورة غير موجودة"]);
            }

            // التحقق من الصلاحيات
            // Check permissions
            var authResult = await ValidatePermissionsAsync(existingImage, cancellationToken);
            if (!authResult.IsSuccess)
            {
                return authResult;
            }

            // التحقق من قواعد العمل
            // Validate business rules
            var businessRulesResult = await ValidateBusinessRulesAsync(existingImage, request, cancellationToken);
            if (!businessRulesResult.IsSuccess)
            {
                return businessRulesResult;
            }

            // حفظ البيانات القديمة للمراجعة
            // Save old data for audit
            var oldImageData = new {
                existingImage.Url,
                existingImage.Caption,
                existingImage.AltText,
                existingImage.Category,
                existingImage.IsMainImage
            };

            // تحديث بيانات الصورة
            // Update image data
            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // معالجة الصورة الجديدة إذا تم تغيير الرابط
                // Process new image if URL changed
                ProcessedImageResult? processedImage = null;
                if (!string.IsNullOrEmpty(request.Url) && request.Url != existingImage.Url)
                {
                    var imageProcessResult = await ProcessImageIfNeededAsync(request.Url, cancellationToken);
                    if (imageProcessResult.IsSuccess)
                    {
                        processedImage = imageProcessResult.Data;
                    }
                }

                // تحديث خصائص الصورة
                // Update image properties
                UpdateImageProperties(existingImage, request, processedImage);

                // إذا كانت صورة رئيسية، تحديث الصور الأخرى
                // If main image, update other images
                if (request.IsMain.HasValue && request.IsMain.Value)
                {
                    await UpdateMainImageStatusAsync(existingImage, cancellationToken);
                }

                // حفظ التحديثات
                // Save updates
                var updatedImage = await _propertyImageRepository.UpdatePropertyImageAsync(existingImage, cancellationToken);

                // تسجيل العملية للمراجعة
                // Log operation for audit
                await _auditService.LogActivityAsync(
                    "PropertyImageUpdated",
                    $"تم تحديث صورة الكيان - ID: {updatedImage.Id}",
                    "PropertyImage",
                    updatedImage.Id.ToString(),
                    oldImageData,
                    new { 
                        ImageId = updatedImage.Id, 
                        NewUrl = updatedImage.Url,
                        NewCaption = updatedImage.Caption,
                        NewIsMain = updatedImage.IsMainImage
                    },
                    cancellationToken);

                // إنشاء وإرسال الحدث
                // Create and publish event
                await PublishImageUpdatedEventAsync(existingImage, updatedImage, oldImageData, cancellationToken);

                // إرسال الإشعارات
                // Send notifications
                await SendNotificationsAsync(updatedImage, cancellationToken);

                _logger.LogInformation("تم تحديث صورة الكيان بنجاح - ImageId: {ImageId}", updatedImage.Id);
                return ResultDto<bool>.Ok(true);
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تحديث صورة الكيان - ImageId: {ImageId}", request.ImageId);
            return ResultDto<bool>.Failure("فشل في تحديث صورة الكيان");
        }
    }

    #endregion

    #region Validation Methods

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private async Task<ResultDto<bool>> ValidateRequestAsync(UpdatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الصورة
        // Validate image ID
        if (request.ImageId == Guid.Empty)
        {
            errors.Add("معرف الصورة مطلوب");
        }

        // التحقق من صحة رابط الصورة الجديد إن وجد
        // Validate new image URL if provided
        if (!string.IsNullOrEmpty(request.Url) && !Uri.IsWellFormedUriString(request.Url, UriKind.RelativeOrAbsolute))
        {
            errors.Add("رابط الصورة الجديد غير صحيح");
        }

        // التحقق من صحة فئة الصورة إن وجدت
        // Validate image category if provided
        if (request.Category.HasValue && !Enum.IsDefined(typeof(ImageCategory), request.Category.Value))
        {
            errors.Add("فئة الصورة غير صحيحة");
        }

        return await Task.FromResult(errors.Any() 
            ? ResultDto<bool>.Failed(errors)
            : ResultDto<bool>.Ok(true));
    }

    /// <summary>
    /// التحقق من صلاحيات المستخدم
    /// Validate user permissions
    /// </summary>
    private async Task<ResultDto<bool>> ValidatePermissionsAsync(PropertyImage image, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return ResultDto<bool>.Failed(["المستخدم غير مسجل الدخول"]);
        }

        // التحقق من الصلاحيات حسب الكيان أو الوحدة
        // Check permissions based on property or unit
        if (image.PropertyId.HasValue)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(image.PropertyId.Value, cancellationToken);
            if (property == null || (property.OwnerId != userId && !_currentUserService.IsStaffInProperty(image.PropertyId.Value)))
            {
                return ResultDto<bool>.Failed(["ليس لديك صلاحية لتعديل هذه الصورة"]);
            }
        }
        else if (image.UnitId.HasValue)
        {
            var unit = await _unitRepository.GetByIdAsync(image.UnitId.Value, cancellationToken);
            if (unit == null)
            {
                return ResultDto<bool>.Failed(["الوحدة غير موجودة"]);
            }

            var property = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
            if (property == null || (property.OwnerId != userId && !_currentUserService.IsStaffInProperty(unit.PropertyId)))
            {
                return ResultDto<bool>.Failed(["ليس لديك صلاحية لتعديل هذه الصورة"]);
            }
        }

        return ResultDto<bool>.Ok(true);
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(PropertyImage existingImage, UpdatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من حالة الصورة
        // Check image status
        if (existingImage.Status == ImageStatus.Rejected)
        {
            errors.Add("لا يمكن تحديث صورة مرفوضة");
        }

        // إذا كان هناك رابط جديد، التحقق من عدم تكراره
        // If new URL, check for duplicates
        if (!string.IsNullOrEmpty(request.Url) && request.Url != existingImage.Url)
        {
            // يمكن إضافة تحقق من تكرار الرابط هنا إذا لزم الأمر
            // Can add duplicate URL check here if needed
        }

        return await Task.FromResult(errors.Any() 
            ? ResultDto<bool>.Failed(errors)
            : ResultDto<bool>.Ok(true));
    }

    #endregion

    #region Image Processing

    /// <summary>
    /// معالجة الصورة الجديدة إذا لزم الأمر
    /// Process new image if needed
    /// </summary>
    private async Task<ResultDto<ProcessedImageResult>> ProcessImageIfNeededAsync(string newUrl, CancellationToken cancellationToken)
    {
        try
        {
            // إذا كان الرابط محلي، معالجة الصورة
            // If URL is local, process the image
            if (!newUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var processResult = new ProcessedImageResult
                {
                    ProcessedUrl = newUrl,
                    ThumbnailUrl = newUrl,
                    FileSize = 0,
                    Dimensions = "0x0",
                    MimeType = "image/jpeg"
                };

                return await Task.FromResult(ResultDto<ProcessedImageResult>.Ok(processResult));
            }

            // للروابط الخارجية، إرجاع البيانات كما هي
            // For external URLs, return data as is
            var result = new ProcessedImageResult
            {
                ProcessedUrl = newUrl,
                ThumbnailUrl = newUrl,
                FileSize = 0,
                Dimensions = "0x0",
                MimeType = "image/jpeg"
            };

            return await Task.FromResult(ResultDto<ProcessedImageResult>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء معالجة الصورة الجديدة: {Url}", newUrl);
            return ResultDto<ProcessedImageResult>.Failure("فشل في معالجة الصورة الجديدة");
        }
    }

    /// <summary>
    /// تحديث خصائص الصورة
    /// Update image properties
    /// </summary>
    private void UpdateImageProperties(PropertyImage existingImage, UpdatePropertyImageCommand request, ProcessedImageResult? processedImage)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService.UserId;

        // تحديث الرابط إذا تم توفير رابط جديد
        // Update URL if new URL provided
        if (!string.IsNullOrEmpty(request.Url))
        {
            existingImage.Url = processedImage?.ProcessedUrl ?? request.Url;
        }

        // تحديث التعليق إذا تم توفيره
        // Update caption if provided
        if (!string.IsNullOrEmpty(request.Caption))
        {
            existingImage.Caption = request.Caption;
            existingImage.Name = request.Caption; // تحديث الاسم أيضاً
        }

        // تحديث النص البديل إذا تم توفيره
        // Update alt text if provided
        if (!string.IsNullOrEmpty(request.AltText))
        {
            existingImage.AltText = request.AltText;
        }

        // تحديث الفئة إذا تم توفيرها
        // Update category if provided
        if (request.Category.HasValue)
        {
            existingImage.Category = request.Category.Value;
        }

        // تحديث حالة الصورة الرئيسية إذا تم توفيرها
        // Update main image status if provided
        if (request.IsMain.HasValue)
        {
            existingImage.IsMainImage = request.IsMain.Value;
            existingImage.IsMain = request.IsMain.Value;
        }

        // تحديث معلومات التعديل
        // Update modification info
        existingImage.UpdatedAt = now;
        existingImage.UpdatedBy = userId;

        // تحديث معلومات الصورة المعالجة إن وجدت
        // Update processed image info if available
        if (processedImage != null)
        {
            existingImage.Type = processedImage.MimeType;
            existingImage.SizeBytes = processedImage.FileSize;
            existingImage.Sizes = JsonSerializer.Serialize(new { full = processedImage.ProcessedUrl, thumbnail = processedImage.ThumbnailUrl });
        }
    }

    /// <summary>
    /// تحديث حالة الصورة الرئيسية للصور الأخرى
    /// Update main image status for other images
    /// </summary>
    private async Task UpdateMainImageStatusAsync(PropertyImage newMainImage, CancellationToken cancellationToken)
    {
        if (!newMainImage.IsMainImage) return;

        if (newMainImage.PropertyId.HasValue)
        {
            var otherImages = await _propertyImageRepository.GetImagesByPropertyAsync(newMainImage.PropertyId.Value, cancellationToken);
            foreach (var image in otherImages.Where(i => i.Id != newMainImage.Id && i.IsMainImage))
            {
                image.IsMainImage = false;
                image.IsMain = false;
                image.UpdatedAt = DateTime.UtcNow;
                image.UpdatedBy = _currentUserService.UserId;
                await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);
            }
        }

        if (newMainImage.UnitId.HasValue)
        {
            var otherImages = await _propertyImageRepository.GetImagesByUnitAsync(newMainImage.UnitId.Value, cancellationToken);
            foreach (var image in otherImages.Where(i => i.Id != newMainImage.Id && i.IsMainImage))
            {
                image.IsMainImage = false;
                image.IsMain = false;
                image.UpdatedAt = DateTime.UtcNow;
                image.UpdatedBy = _currentUserService.UserId;
                await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);
            }
        }
    }

    /// <summary>
    /// إنشاء وإرسال حدث تحديث الصورة
    /// Create and publish image updated event
    /// </summary>
    private async Task PublishImageUpdatedEventAsync(PropertyImage oldImage, PropertyImage newImage, object oldData, CancellationToken cancellationToken)
    {
        var eventData = new PropertyImageUpdatedEvent
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            EventType = "PropertyImageUpdated",
            Version = 1,
            UserId = _currentUserService.UserId,
            CorrelationId = Guid.NewGuid().ToString(),
            ImageId = newImage.Id,
            PropertyId = newImage.PropertyId,
            UnitId = newImage.UnitId,
            UpdatedFields = GetUpdatedFields(oldImage, newImage),
            OldUrl = oldImage.Url,
            NewUrl = newImage.Url,
            OldCaption = oldImage.Caption,
            NewCaption = newImage.Caption,
            OldAltText = oldImage.AltText,
            NewAltText = newImage.AltText,
            OldCategory = oldImage.Category,
            NewCategory = newImage.Category,
            OldIsMain = oldImage.IsMainImage,
            NewIsMain = newImage.IsMainImage,
            UpdatedAt = newImage.UpdatedAt
        };

        await _eventPublisher.PublishAsync(eventData, cancellationToken);
    }

    /// <summary>
    /// الحصول على قائمة الحقول المحدثة
    /// Get list of updated fields
    /// </summary>
    private string[] GetUpdatedFields(PropertyImage oldImage, PropertyImage newImage)
    {
        var updatedFields = new List<string>();

        if (oldImage.Url != newImage.Url)
            updatedFields.Add("Url");

        if (oldImage.Caption != newImage.Caption)
            updatedFields.Add("Caption");

        if (oldImage.AltText != newImage.AltText)
            updatedFields.Add("AltText");

        if (oldImage.Category != newImage.Category)
            updatedFields.Add("Category");

        if (oldImage.IsMainImage != newImage.IsMainImage)
            updatedFields.Add("IsMain");

        return updatedFields.ToArray();
    }

    /// <summary>
    /// إرسال الإشعارات اللازمة
    /// Send necessary notifications
    /// </summary>
    private async Task SendNotificationsAsync(PropertyImage image, CancellationToken cancellationToken)
    {
        try
        {
            // إشعار مالك الكيان
            // Notify property owner
            if (image.PropertyId.HasValue)
            {
                var property = await _propertyRepository.GetPropertyByIdAsync(image.PropertyId.Value, cancellationToken);
                if (property != null && property.OwnerId != _currentUserService.UserId)
                {
                    var notificationRequest = new NotificationRequest
                    {
                        UserId = property.OwnerId,
                        Type = NotificationType.BookingUpdated, // Use available type as placeholder
                        Title = "تم تحديث صورة الكيان",
                        Message = $"تم تحديث صورة في كيانك: {property.Name}",
                        Data = new { ImageId = image.Id, PropertyId = property.Id }
                    };

                    await _notificationService.SendAsync(notificationRequest, cancellationToken);
                }
            }
            // إشعار مالك الوحدة (إذا كانت مختلفة عن مالك الكيان)
            // Notify unit owner (if different from property owner)
            else if (image.UnitId.HasValue)
            {
                var unit = await _unitRepository.GetByIdAsync(image.UnitId.Value, cancellationToken);
                if (unit != null)
                {
                    var property = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
                    if (property != null && property.OwnerId != _currentUserService.UserId)
                    {
                        var notificationRequest = new NotificationRequest
                        {
                            UserId = property.OwnerId,
                            Type = NotificationType.BookingUpdated, // Use available type as placeholder
                            Title = "تم تحديث صورة الوحدة",
                            Message = $"تم تحديث صورة في وحدة: {unit.Name}",
                            Data = new { ImageId = image.Id, UnitId = unit.Id }
                        };

                        await _notificationService.SendAsync(notificationRequest, cancellationToken);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "فشل في إرسال الإشعارات لتحديث الصورة - ImageId: {ImageId}", image.Id);
            // لا نريد أن يفشل التحديث بسبب فشل الإشعارات
            // We don't want update to fail because of notification failures
        }
    }

    #endregion
}

/// <summary>
/// حدث تحديث صورة الكيان
/// Property image updated event
/// </summary>
public class PropertyImageUpdatedEvent : IPropertyImageUpdatedEvent
{
    /// <summary>
    /// معرف الحدث
    /// Event ID
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// تاريخ الحدث
    /// Event occurrence date
    /// </summary>
    public DateTime OccurredOn { get; set; }

    /// <summary>
    /// نوع الحدث
    /// Event type
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// إصدار الحدث
    /// Event version
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// معرف ربط الأحداث
    /// Correlation ID
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// معرف المستخدم الذي قام بالتحديث
    /// ID of the user who performed the update
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// معرف الصورة المحدثة
    /// ID of the updated image
    /// </summary>
    public Guid ImageId { get; set; }

    /// <summary>
    /// معرف الكيان (إن وجد)
    /// Property ID (if any)
    /// </summary>
    public Guid? PropertyId { get; set; }

    /// <summary>
    /// معرف الوحدة (إن وجد)
    /// Unit ID (if any)
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// الحقول المحدثة
    /// Updated fields
    /// </summary>
    public string[] UpdatedFields { get; set; } = [];

    /// <summary>
    /// الرابط القديم للصورة
    /// Old image URL
    /// </summary>
    public string OldUrl { get; set; } = string.Empty;

    /// <summary>
    /// الرابط الجديد للصورة
    /// New image URL
    /// </summary>
    public string? NewUrl { get; set; }

    /// <summary>
    /// التعليق القديم
    /// Old caption
    /// </summary>
    public string OldCaption { get; set; } = string.Empty;

    /// <summary>
    /// التعليق الجديد
    /// New caption
    /// </summary>
    public string? NewCaption { get; set; }

    /// <summary>
    /// النص البديل القديم
    /// Old alt text
    /// </summary>
    public string OldAltText { get; set; } = string.Empty;

    /// <summary>
    /// النص البديل الجديد
    /// New alt text
    /// </summary>
    public string? NewAltText { get; set; }

    /// <summary>
    /// الفئة القديمة
    /// Old category
    /// </summary>
    public ImageCategory OldCategory { get; set; }

    /// <summary>
    /// الفئة الجديدة
    /// New category
    /// </summary>
    public ImageCategory? NewCategory { get; set; }

    /// <summary>
    /// حالة الصورة الرئيسية القديمة
    /// Old main image status
    /// </summary>
    public bool OldIsMain { get; set; }

    /// <summary>
    /// حالة الصورة الرئيسية الجديدة
    /// New main image status
    /// </summary>
    public bool? NewIsMain { get; set; }

    /// <summary>
    /// تاريخ التحديث
    /// Update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

