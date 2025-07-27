using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.PropertyImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Notifications;
using CoreUnit = YemenBooking.Core.Entities.Unit;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.PropertyImages;

/// <summary>
/// معالج أمر إنشاء صورة كيان جديدة
/// Create property image command handler
/// 
/// يقوم بإنشاء صورة جديدة لكيان أو وحدة ويشمل:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الكيان أو الوحدة
/// - التحقق من صلاحيات المستخدم
/// - معالجة الصورة (إذا لزم الأمر)
/// - حفظ الصورة في قاعدة البيانات
/// - إنشاء حدث إنشاء الصورة
/// - إرسال الإشعارات اللازمة
/// 
/// Creates a new property image and includes:
/// - Input data validation
/// - Property or unit existence validation
/// - User authorization validation
/// - Image processing (if required)
/// - Saving image to database
/// - Creating image creation event
/// - Sending necessary notifications
/// </summary>
public class CreatePropertyImageCommandHandler : IRequestHandler<CreatePropertyImageCommand, ResultDto<Guid>>
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
    private readonly ILogger<CreatePropertyImageCommandHandler> _logger;
    #endregion

    #region Constructor
    /// <summary>
    /// المنشئ - حقن التبعيات
    /// Constructor - Dependency injection
    /// </summary>
    public CreatePropertyImageCommandHandler(
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
        ILogger<CreatePropertyImageCommandHandler> logger)
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

    #region Handle Method
    /// <summary>
    /// معالج الأمر الرئيسي
    /// Main command handler
    /// </summary>
    public async Task<ResultDto<Guid>> Handle(CreatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // تسجيل بداية المعالجة
            // Log processing start
            _logger.LogInformation("بدء معالجة أمر إنشاء صورة كيان جديدة - PropertyId: {PropertyId}, UnitId: {UnitId}", 
                request.PropertyId, request.UnitId);

            // التحقق من صحة البيانات
            // Validate input data
            var validationResult = await ValidateRequestAsync(request, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من صحة البيانات: {Errors}", string.Join(", ", validationResult.Errors));
                return ResultDto<Guid>.Failed(validationResult.Errors.Select(e => e.ToString()));
            }

            // التحقق من الصلاحيات
            // Check permissions
            var authorizationResult = await CheckUserAuthorizationAsync(request, cancellationToken);
            if (!authorizationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من صلاحيات المستخدم: {UserId}", _currentUserService.UserId);
                return ResultDto<Guid>.Failed(authorizationResult.Errors.Select(e => e.ToString()));
            }

            // التحقق من وجود الكيان أو الوحدة
            // Verify property or unit exists
            var existenceResult = await VerifyPropertyOrUnitExistsAsync(request, cancellationToken);
            if (!existenceResult.IsSuccess)
            {
                _logger.LogWarning("الكيان أو الوحدة غير موجودة - PropertyId: {PropertyId}, UnitId: {UnitId}", 
                    request.PropertyId, request.UnitId);
                return ResultDto<Guid>.Failed(existenceResult.Errors.Select(e => e.ToString()));
            }

            // معالجة الصورة إذا لزم الأمر
            // Process image if needed
            var processedImageResult = await ProcessImageIfNeededAsync(request, cancellationToken);
            if (!processedImageResult.IsSuccess)
            {
                _logger.LogError("فشل في معالجة الصورة: {Error}", processedImageResult.Message);
                return ResultDto<Guid>.Failed(processedImageResult.Errors.Select(e => e.ToString()));
            }

            // إنشاء كائن الصورة الجديد
            // Create new image entity
            var propertyImage = CreatePropertyImageEntity(request, processedImageResult.Data);

            // حفظ الصورة في قاعدة البيانات
            // Save image to database
            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var createdImage = await _propertyImageRepository.CreatePropertyImageAsync(propertyImage, cancellationToken);

                // إذا كانت صورة رئيسية، تحديث الصور الأخرى
                // If main image, update other images
                if (request.IsMain)
                {
                    await UpdateMainImageStatusAsync(createdImage, cancellationToken);
                }

                // تسجيل العملية للمراجعة
                // Log operation for audit
                await _auditService.LogActivityAsync(
                    "PropertyImageCreated",
                    $"تم إنشاء صورة كيان جديدة - ID: {createdImage.Id}",
                    "PropertyImage",
                    createdImage.Id.ToString(),
                    null,
                    new { ImageId = createdImage.Id, Url = createdImage.Url },
                    cancellationToken);

                // إنشاء وإرسال الحدث
                // Create and publish event
                await PublishImageCreatedEventAsync(createdImage, cancellationToken);

                // إرسال الإشعارات
                // Send notifications
                await SendNotificationsAsync(createdImage, cancellationToken);

                _logger.LogInformation("تم إنشاء صورة الكيان بنجاح - ImageId: {ImageId}", createdImage.Id);

                return createdImage.Id;
            }, cancellationToken);

            return ResultDto<Guid>.Ok(result, "تم إنشاء صورة الكيان بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ غير متوقع أثناء معالجة أمر إنشاء صورة الكيان");
            return ResultDto<Guid>.Failure("حدث خطأ غير متوقع أثناء إنشاء صورة الكيان");
        }
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private async Task<ResultDto<bool>> ValidateRequestAsync(CreatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // يجب تحديد كيان أو وحدة واحدة على الأقل
        // Must specify either property or unit
        if (!request.PropertyId.HasValue && !request.UnitId.HasValue)
        {
            errors.Add("يجب تحديد معرف الكيان أو معرف الوحدة");
        }

        // لا يمكن تحديد كيان ووحدة في نفس الوقت
        // Cannot specify both property and unit
        if (request.PropertyId.HasValue && request.UnitId.HasValue)
        {
            errors.Add("لا يمكن تعيين الصورة لكيان ووحدة في نفس الوقت");
        }

        // التحقق من صحة رابط الصورة
        // Validate image URL
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            errors.Add("رابط الصورة مطلوب");
        }
        else if (!Uri.IsWellFormedUriString(request.Url, UriKind.RelativeOrAbsolute))
        {
            errors.Add("رابط الصورة غير صحيح");
        }

        // التحقق من صحة فئة الصورة
        // Validate image category
        if (!Enum.IsDefined(typeof(ImageCategory), request.Category))
        {
            errors.Add("فئة الصورة غير صحيحة");
        }

        // استخدام خدمة التحقق للتحقق من قواعد العمل الإضافية
        // Use validation service for additional business rules
        var businessValidationResult = await _validationService.ValidateAsync(request, cancellationToken);
        if (!businessValidationResult.IsValid)
        {
            errors.AddRange(businessValidationResult.Errors.Select(e => e.Message));
        }

        return errors.Any() 
            ? ResultDto<bool>.Failed(errors)
            : ResultDto<bool>.Ok(true);
    }

    /// <summary>
    /// التحقق من صلاحيات المستخدم
    /// Check user authorization
    /// </summary>
    private async Task<ResultDto<bool>> CheckUserAuthorizationAsync(CreatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        // Check current user permissions
        if (_currentUserService.UserId == Guid.Empty)
        {
            return ResultDto<bool>.Failure("المستخدم غير مسجل الدخول");
        }

        // التحقق من صلاحية إنشاء صور الكيانات
        // Check property image creation permission
        var hasPermission = _currentUserService.Permissions.Contains("PropertyImages.Create");
        if (!hasPermission)
        {
            return ResultDto<bool>.Failure("المستخدم لا يملك صلاحية إنشاء صور الكيانات");
        }

        // إذا كان الكيان محدد، التحقق من ملكية الكيان أو الصلاحية لإدارته
        // If property is specified, check property ownership or management permission
        if (request.PropertyId.HasValue)
        {
            // Simple check - admin or property owner
            var isAdmin = _currentUserService.UserRoles.Contains("Admin");
            if (!isAdmin && _currentUserService.PropertyId != request.PropertyId)
            {
                return ResultDto<bool>.Failure("المستخدم لا يملك صلاحية إدارة هذا الكيان");
            }
        }

        return await Task.FromResult(ResultDto<bool>.Ok(true));
    }

    /// <summary>
    /// التحقق من وجود الكيان أو الوحدة
    /// Verify property or unit exists
    /// </summary>
    private async Task<ResultDto<bool>> VerifyPropertyOrUnitExistsAsync(CreatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        if (request.PropertyId.HasValue)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId.Value, cancellationToken);
            if (property == null)
            {
                return ResultDto<bool>.Failure("الكيان المحدد غير موجود");
            }

            // التحقق من أن الكيان ليس محذوف
            // Check property is not deleted
            if (property.IsDeleted)
            {
                return ResultDto<bool>.Failure("لا يمكن إضافة صور لكيان محذوف");
            }
        }

        if (request.UnitId.HasValue)
        {
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId.Value, cancellationToken);
            if (unit == null)
            {
                return ResultDto<bool>.Failure("الوحدة المحددة غير موجودة");
            }

            // التحقق من أن الوحدة ليست محذوفة
            // Check unit is not deleted
            if (unit.IsDeleted)
            {
                return ResultDto<bool>.Failure("لا يمكن إضافة صور لوحدة محذوفة");
            }
        }

        return ResultDto<bool>.Ok(true);
    }

    /// <summary>
    /// معالجة الصورة إذا لزم الأمر
    /// Process image if needed
    /// </summary>
    private async Task<ResultDto<ProcessedImageResult>> ProcessImageIfNeededAsync(CreatePropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // إذا كان الرابط محلي، معالجة الصورة
            // If URL is local, process the image
            if (!request.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // هنا يمكن إضافة منطق معالجة الصورة
                // Here we can add image processing logic
                // مثل تغيير الحجم، ضغط، إضافة علامة مائية، إلخ
                // Like resizing, compression, watermarking, etc.
                
                var processResult = new ProcessedImageResult
                {
                    ProcessedUrl = request.Url,
                    ThumbnailUrl = request.Url, // يمكن إنشاء صورة مصغرة
                    FileSize = 0, // يمكن حساب حجم الملف
                    Dimensions = "0x0", // يمكن الحصول على أبعاد الصورة
                    MimeType = "image/jpeg" // يمكن تحديد نوع الملف
                };

                return await Task.FromResult(ResultDto<ProcessedImageResult>.Ok(processResult));
            }

            // للروابط الخارجية، إرجاع البيانات كما هي
            // For external URLs, return data as is
            var result = new ProcessedImageResult
            {
                ProcessedUrl = request.Url,
                ThumbnailUrl = request.Url,
                FileSize = 0,
                Dimensions = "0x0",
                MimeType = "image/jpeg"
            };

            return await Task.FromResult(ResultDto<ProcessedImageResult>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء معالجة الصورة: {Url}", request.Url);
            return ResultDto<ProcessedImageResult>.Failure("فشل في معالجة الصورة");
        }
    }

    /// <summary>
    /// إنشاء كائن الصورة الجديد
    /// Create new image entity
    /// </summary>
    private PropertyImage CreatePropertyImageEntity(CreatePropertyImageCommand request, ProcessedImageResult? processedImage)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService.UserId;

        return new PropertyImage
        {
            Id = Guid.NewGuid(),
            PropertyId = request.PropertyId,
            UnitId = request.UnitId,
            Name = !string.IsNullOrEmpty(request.Caption) ? request.Caption : "صورة كيان",
            Url = processedImage?.ProcessedUrl ?? request.Url,
            Caption = request.Caption ?? string.Empty,
            AltText = request.AltText ?? string.Empty,
            Category = request.Category,
            IsMain = request.IsMain,
            IsMainImage = request.IsMain,
            SortOrder = 0, // يمكن تحديد ترتيب العرض
            DisplayOrder = 0,
            Status = ImageStatus.Approved, // أو Pending للمراجعة
            Type = processedImage?.MimeType ?? "image/jpeg",
            SizeBytes = processedImage?.FileSize ?? 0,
            Tags = "[]", // يمكن إضافة وسوم
            Sizes = JsonSerializer.Serialize(new { full = processedImage?.ProcessedUrl ?? request.Url, thumbnail = processedImage?.ThumbnailUrl ?? request.Url }),
            Views = 0,
            Downloads = 0,
            UploadedAt = now,
            CreatedAt = now,
            CreatedBy = userId,
            UpdatedAt = now,
            UpdatedBy = userId,
            IsDeleted = false
        };
    }

    /// <summary>
    /// تحديث حالة الصورة الرئيسية للصور الأخرى
    /// Update main image status for other images
    /// </summary>
    private async Task UpdateMainImageStatusAsync(PropertyImage newImage, CancellationToken cancellationToken)
    {
        if (!newImage.IsMainImage) return;

        if (newImage.PropertyId.HasValue)
        {
            var otherImages = await _propertyImageRepository.GetImagesByPropertyAsync(newImage.PropertyId.Value, cancellationToken);
            foreach (var image in otherImages.Where(i => i.Id != newImage.Id && i.IsMainImage))
            {
                image.IsMainImage = false;
                image.UpdatedAt = DateTime.UtcNow;
                image.UpdatedBy = _currentUserService.UserId;
                await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);
            }
        }

        if (newImage.UnitId.HasValue)
        {
            var otherImages = await _propertyImageRepository.GetImagesByUnitAsync(newImage.UnitId.Value, cancellationToken);
            foreach (var image in otherImages.Where(i => i.Id != newImage.Id && i.IsMainImage))
            {
                image.IsMainImage = false;
                image.UpdatedAt = DateTime.UtcNow;
                image.UpdatedBy = _currentUserService.UserId;
                await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);
            }
        }
    }

    /// <summary>
    /// إنشاء وإرسال حدث إنشاء الصورة
    /// Create and publish image created event
    /// </summary>
    private async Task PublishImageCreatedEventAsync(PropertyImage image, CancellationToken cancellationToken)
    {
        var eventData = new PropertyImageCreatedEvent
        {
            ImageId = image.Id,
            PropertyId = image.PropertyId,
            UnitId = image.UnitId,
            Url = image.Url,
            Caption = image.Caption,
            Category = image.Category,
            IsMain = image.IsMainImage,
            UserId = _currentUserService.UserId,
            OccurredAt = DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(eventData, cancellationToken);
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
                    // Use NotificationRequest to send via service
                    var notificationRequest = new NotificationRequest
                    {
                        UserId = property.OwnerId,
                        Type = NotificationType.BookingCreated, // Use available type as placeholder
                        Title = "تم إضافة صورة جديدة",
                        Message = $"تم إضافة صورة جديدة للكيان: {property.Name}",
                        Data = new { ImageId = image.Id, PropertyId = property.Id }
                    };

                    await _notificationService.SendAsync(notificationRequest, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "فشل في إرسال الإشعارات لصورة الكيان: {ImageId}", image.Id);
            // لا نريد أن يفشل الأمر بسبب فشل الإشعارات
            // We don't want the command to fail because of notification failure
        }
    }

    #endregion
}

/// <summary>
/// حدث إنشاء صورة الكيان
/// Property image created event
/// </summary>
public class PropertyImageCreatedEvent : IPropertyImageCreatedEvent
{
    /// <summary>
    /// معرف الحدث
    /// Event identifier
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// تاريخ حدوث الحدث
    /// Event occurred date
    /// </summary>
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// نوع الحدث
    /// Event type
    /// </summary>
    public string EventType { get; set; } = "PropertyImageCreated";

    /// <summary>
    /// إصدار الحدث
    /// Event version
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// معرف المستخدم
    /// User identifier
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// معرف الربط للتتبع
    /// Correlation identifier for tracing
    /// </summary>
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// معرف الصورة الجديدة
    /// New image ID
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
    /// رابط أو مسار الصورة
    /// Image URL or path
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// تعليق توضيحي للصورة
    /// Image caption
    /// </summary>
    public string Caption { get; set; } = string.Empty;

    /// <summary>
    /// النص البديل للصورة
    /// Alternative text for image
    /// </summary>
    public string AltText { get; set; } = string.Empty;

    /// <summary>
    /// فئة الصورة
    /// Image category
    /// </summary>
    public ImageCategory Category { get; set; }

    /// <summary>
    /// هل هي الصورة الرئيسية
    /// Is main image
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// تاريخ إنشاء الصورة
    /// Image creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// تاريخ الحدث (for compatibility)
    /// Event timestamp (for compatibility)
    /// </summary>
    public DateTime OccurredAt { get; set; }
}
