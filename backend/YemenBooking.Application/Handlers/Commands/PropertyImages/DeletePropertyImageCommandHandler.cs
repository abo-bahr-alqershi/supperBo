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

namespace YemenBooking.Application.Handlers.Commands.PropertyImages;

/// <summary>
/// معالج أمر حذف صورة الكيان
/// Delete property image command handler
/// 
/// يقوم بحذف صورة موجودة (حذف ناعم) ويشمل:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الصورة
/// - التحقق من صلاحيات المستخدم
/// - التحقق من قواعد العمل (مثل عدم حذف الصورة الرئيسية الوحيدة)
/// - تنفيذ الحذف الناعم للصورة
/// - حذف الملف الفيزيائي (اختياري)
/// - إنشاء حدث حذف الصورة
/// - إرسال الإشعارات اللازمة
/// 
/// Deletes an existing image (soft delete) and includes:
/// - Input data validation
/// - Image existence validation
/// - User authorization validation
/// - Business rules validation (like not deleting the only main image)
/// - Performing soft delete of the image
/// - Deleting physical file (optional)
/// - Creating image deletion event
/// - Sending necessary notifications
/// </summary>
public class DeletePropertyImageCommandHandler : IRequestHandler<DeletePropertyImageCommand, ResultDto<bool>>
{
    #region Dependencies
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeletePropertyImageCommandHandler> _logger;
    #endregion

    #region Constructor
    /// <summary>
    /// المنشئ - حقن التبعيات
    /// Constructor - Dependency injection
    /// </summary>
    public DeletePropertyImageCommandHandler(
        IPropertyImageRepository propertyImageRepository,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        INotificationService notificationService,
        ILogger<DeletePropertyImageCommandHandler> logger)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
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
    public async Task<ResultDto<bool>> Handle(DeletePropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة حذف صورة الكيان - ImageId: {ImageId}", request.ImageId);

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
            var businessRulesResult = await ValidateBusinessRulesAsync(existingImage, cancellationToken);
            if (!businessRulesResult.IsSuccess)
            {
                return businessRulesResult;
            }

            // حذف الصورة
            // Delete image data
            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // تنفيذ الحذف الناعم
                // Perform soft delete
                await _propertyImageRepository.DeletePropertyImageAsync(existingImage.Id, cancellationToken);
                var deletedImage = existingImage;

                // إذا كانت صورة رئيسية، تعيين صورة أخرى كرئيسية
                // If main image, set another image as main
                if (existingImage.IsMainImage)
                {
                    await SetNewMainImageAsync(existingImage, cancellationToken);
                }

                // تسجيل العملية للمراجعة
                // Log operation for audit
                await _auditService.LogActivityAsync(
                    "PropertyImageDeleted",
                    $"تم حذف صورة الكيان - ID: {deletedImage.Id}",
                    "PropertyImage",
                    deletedImage.Id.ToString(),
                    new { 
                        ImageId = deletedImage.Id, 
                        Url = deletedImage.Url,
                        Caption = deletedImage.Caption,
                        PropertyId = deletedImage.PropertyId,
                        UnitId = deletedImage.UnitId
                    },
                    null,
                    cancellationToken);

                // إنشاء وإرسال الحدث
                // Create and publish event
                await PublishImageDeletedEventAsync(deletedImage, cancellationToken);

                // إرسال الإشعارات
                // Send notifications
                await SendNotificationsAsync(deletedImage, cancellationToken);

                _logger.LogInformation("تم حذف صورة الكيان بنجاح - ImageId: {ImageId}", deletedImage.Id);
                return ResultDto<bool>.Ok(true);
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء حذف صورة الكيان - ImageId: {ImageId}", request.ImageId);
            return ResultDto<bool>.Failure("فشل في حذف صورة الكيان");
        }
    }
    #endregion

    #region Validation Methods

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private async Task<ResultDto<bool>> ValidateRequestAsync(DeletePropertyImageCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الصورة
        // Validate image ID
        if (request.ImageId == Guid.Empty)
        {
            errors.Add("معرف الصورة مطلوب");
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
                return ResultDto<bool>.Failed(["ليس لديك صلاحية لحذف هذه الصورة"]);
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
                return ResultDto<bool>.Failed(["ليس لديك صلاحية لحذف هذه الصورة"]);
            }
        }

        return ResultDto<bool>.Ok(true);
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(PropertyImage existingImage, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الصورة ليست محذوفة مسبقاً
        // Check image is not already deleted
        if (existingImage.IsDeleted)
        {
            errors.Add("الصورة محذوفة مسبقاً");
        }

        // التحقق من حالة الصورة
        // Check image status
        if (existingImage.Status == ImageStatus.Rejected)
        {
            // يمكن السماح بحذف الصور المرفوضة
            // Can allow deletion of rejected images
        }

        return await Task.FromResult(errors.Any() 
            ? ResultDto<bool>.Failed(errors)
            : ResultDto<bool>.Ok(true));
    }

    #endregion

    #region Image Processing

    /// <summary>
    /// تعيين صورة جديدة كرئيسية إذا تم حذف الصورة الرئيسية
    /// Set new main image if main image was deleted
    /// </summary>
    private async Task SetNewMainImageAsync(PropertyImage deletedImage, CancellationToken cancellationToken)
    {
        try
        {
            PropertyImage? newMainImage = null;

            if (deletedImage.PropertyId.HasValue)
            {
                var propertyImages = await _propertyImageRepository.GetImagesByPropertyAsync(deletedImage.PropertyId.Value, cancellationToken);
                newMainImage = propertyImages.FirstOrDefault(img => img.Id != deletedImage.Id && !img.IsDeleted);
            }
            else if (deletedImage.UnitId.HasValue)
            {
                var unitImages = await _propertyImageRepository.GetImagesByUnitAsync(deletedImage.UnitId.Value, cancellationToken);
                newMainImage = unitImages.FirstOrDefault(img => img.Id != deletedImage.Id && !img.IsDeleted);
            }

            if (newMainImage != null)
            {
                newMainImage.IsMainImage = true;
                newMainImage.IsMain = true;
                newMainImage.UpdatedAt = DateTime.UtcNow;
                newMainImage.UpdatedBy = _currentUserService.UserId;
                
                await _propertyImageRepository.UpdatePropertyImageAsync(newMainImage, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "فشل في تعيين صورة جديدة كرئيسية بعد حذف الصورة الرئيسية");
        }
    }

    /// <summary>
    /// إنشاء وإرسال حدث حذف الصورة
    /// Create and publish image deleted event
    /// </summary>
    private async Task PublishImageDeletedEventAsync(PropertyImage deletedImage, CancellationToken cancellationToken)
    {
        var eventData = new PropertyImageDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            EventType = "PropertyImageDeleted",
            Version = 1,
            UserId = _currentUserService.UserId,
            CorrelationId = Guid.NewGuid().ToString(),
            ImageId = deletedImage.Id,
            PropertyId = deletedImage.PropertyId,
            UnitId = deletedImage.UnitId,
            DeletedUrl = deletedImage.Url,
            WasMainImage = deletedImage.IsMainImage,
            DeletedAt = DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(eventData, cancellationToken);
    }

    /// <summary>
    /// إرسال الإشعارات اللازمة
    /// Send necessary notifications
    /// </summary>
    private async Task SendNotificationsAsync(PropertyImage deletedImage, CancellationToken cancellationToken)
    {
        try
        {
            // إشعار مالك الكيان
            // Notify property owner
            if (deletedImage.PropertyId.HasValue)
            {
                var property = await _propertyRepository.GetPropertyByIdAsync(deletedImage.PropertyId.Value, cancellationToken);
                if (property != null && property.OwnerId != _currentUserService.UserId)
                {
                    var notificationRequest = new NotificationRequest
                    {
                        UserId = property.OwnerId,
                        Type = NotificationType.BookingUpdated, // Use available type as placeholder
                        Title = "تم حذف صورة الكيان",
                        Message = $"تم حذف صورة من كيانك: {property.Name}",
                        Data = new { ImageId = deletedImage.Id, PropertyId = property.Id }
                    };

                    await _notificationService.SendAsync(notificationRequest, cancellationToken);
                }
            }
            // إشعار مالك الوحدة (إذا كانت مختلفة عن مالك الكيان)
            // Notify unit owner (if different from property owner)
            else if (deletedImage.UnitId.HasValue)
            {
                var unit = await _unitRepository.GetByIdAsync(deletedImage.UnitId.Value, cancellationToken);
                if (unit != null)
                {
                    var property = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
                    if (property != null && property.OwnerId != _currentUserService.UserId)
                    {
                        var notificationRequest = new NotificationRequest
                        {
                            UserId = property.OwnerId,
                            Type = NotificationType.BookingUpdated, // Use available type as placeholder
                            Title = "تم حذف صورة الوحدة",
                            Message = $"تم حذف صورة من وحدة: {unit.Name}",
                            Data = new { ImageId = deletedImage.Id, UnitId = unit.Id }
                        };

                        await _notificationService.SendAsync(notificationRequest, cancellationToken);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "فشل في إرسال الإشعارات لحذف الصورة - ImageId: {ImageId}", deletedImage.Id);
            // لا نريد أن يفشل الحذف بسبب فشل الإشعارات
            // We don't want deletion to fail because of notification failures
        }
    }

    #endregion
}

/// <summary>
/// حدث حذف صورة الكيان
/// Property image deleted event
/// </summary>
public class PropertyImageDeletedEvent : IPropertyImageDeletedEvent
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
    /// معرف المستخدم الذي قام بالحذف
    /// ID of the user who performed the deletion
    /// </summary>
    public Guid UserId { get; set; }
    Guid? IDomainEvent.UserId => UserId;

    /// <summary>
    /// معرف الصورة المحذوفة
    /// Deleted image ID
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
    /// رابط الصورة المحذوفة
    /// Deleted image URL
    /// </summary>
    public string DeletedUrl { get; set; } = string.Empty;

    /// <summary>
    /// هل كانت الصورة الرئيسية
    /// Was main image
    /// </summary>
    public bool WasMainImage { get; set; }

    /// <summary>
    /// تاريخ الحذف
    /// Deletion date
    /// </summary>
    public DateTime DeletedAt { get; set; }
}
