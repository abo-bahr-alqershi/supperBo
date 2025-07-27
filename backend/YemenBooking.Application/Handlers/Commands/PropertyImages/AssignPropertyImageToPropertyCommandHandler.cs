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
/// معالج أمر ربط صورة بكيان
/// Assign property image to property command handler
/// 
/// يقوم بربط صورة موجودة بكيان معين ويشمل:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الصورة والكيان
/// - التحقق من صلاحيات المستخدم
/// - التحقق من قواعد العمل
/// - تحديث ربط الصورة بالكيان
/// - إنشاء حدث ربط الصورة
/// - إرسال الإشعارات اللازمة
/// 
/// Assigns an existing image to a specific property and includes:
/// - Input data validation
/// - Image and property existence validation
/// - User authorization validation
/// - Business rules validation
/// - Updating image assignment to property
/// - Creating image assignment event
/// - Sending necessary notifications
/// </summary>
public class AssignPropertyImageToPropertyCommandHandler : IRequestHandler<AssignPropertyImageToPropertyCommand, ResultDto<bool>>
{
    #region Dependencies
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AssignPropertyImageToPropertyCommandHandler> _logger;
    #endregion

    #region Constructor
    /// <summary>
    /// المنشئ - حقن التبعيات
    /// Constructor - Dependency injection
    /// </summary>
    public AssignPropertyImageToPropertyCommandHandler(
        IPropertyImageRepository propertyImageRepository,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        INotificationService notificationService,
        ILogger<AssignPropertyImageToPropertyCommandHandler> logger)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
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
    public async Task<ResultDto<bool>> Handle(AssignPropertyImageToPropertyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة ربط صورة بكيان - ImageId: {ImageId}, PropertyId: {PropertyId}", 
                request.ImageId, request.PropertyId);

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

            // التحقق من وجود الكيان
            // Check property existence
            var targetProperty = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (targetProperty == null)
            {
                return ResultDto<bool>.Failed(["الكيان غير موجود"]);
            }

            // التحقق من الصلاحيات
            // Check permissions
            var authResult = await ValidatePermissionsAsync(existingImage, targetProperty, cancellationToken);
            if (!authResult.IsSuccess)
            {
                return authResult;
            }

            // التحقق من قواعد العمل
            // Validate business rules
            var businessRulesResult = await ValidateBusinessRulesAsync(existingImage, targetProperty, cancellationToken);
            if (!businessRulesResult.IsSuccess)
            {
                return businessRulesResult;
            }

            // حفظ البيانات القديمة للمراجعة
            // Save old data for audit
            var oldPropertyId = existingImage.PropertyId;
            var oldUnitId = existingImage.UnitId;

            // ربط الصورة بالكيان
            // Assign image to property
            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // تحديث بيانات الصورة
                // Update image data
                existingImage.PropertyId = request.PropertyId;
                existingImage.UnitId = null; // إلغاء ربط الوحدة إن وجدت
                existingImage.UpdatedAt = DateTime.UtcNow;
                existingImage.UpdatedBy = _currentUserService.UserId;

                // إذا كان طلب جعل الصورة رئيسية، تحديث الصور الأخرى
                // If request to make image main, update other images
                if (request.SetAsMain)
                {
                    await UpdateMainImageStatusAsync(existingImage, cancellationToken);
                    existingImage.IsMainImage = true;
                    existingImage.IsMain = true;
                }

                // حفظ التحديثات
                // Save updates
                var updatedImage = await _propertyImageRepository.UpdatePropertyImageAsync(existingImage, cancellationToken);

                // تسجيل العملية للمراجعة
                // Log operation for audit
                await _auditService.LogActivityAsync(
                    "PropertyImageAssignedToProperty",
                    $"تم ربط صورة بكيان - ImageId: {updatedImage.Id}, PropertyId: {request.PropertyId}",
                    "PropertyImage",
                    updatedImage.Id.ToString(),
                    new { 
                        OldPropertyId = oldPropertyId,
                        OldUnitId = oldUnitId
                    },
                    new { 
                        ImageId = updatedImage.Id,
                        NewPropertyId = updatedImage.PropertyId,
                        NewUnitId = updatedImage.UnitId,
                        IsMain = updatedImage.IsMainImage
                    },
                    cancellationToken);

                // إنشاء وإرسال الحدث
                // Create and publish event
                await PublishImageAssignedEventAsync(updatedImage, targetProperty, oldPropertyId, oldUnitId, cancellationToken);

                // إرسال الإشعارات
                // Send notifications
                await SendNotificationsAsync(updatedImage, targetProperty, oldPropertyId, oldUnitId, cancellationToken);

                _logger.LogInformation("تم ربط الصورة بالكيان بنجاح - ImageId: {ImageId}, PropertyId: {PropertyId}", 
                    updatedImage.Id, request.PropertyId);
                return ResultDto<bool>.Ok(true);
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء ربط الصورة بالكيان - ImageId: {ImageId}, PropertyId: {PropertyId}", 
                request.ImageId, request.PropertyId);
            return ResultDto<bool>.Failure("فشل في ربط الصورة بالكيان");
        }
    }
    #endregion

    #region Validation Methods

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private async Task<ResultDto<bool>> ValidateRequestAsync(AssignPropertyImageToPropertyCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الصورة
        // Validate image ID
        if (request.ImageId == Guid.Empty)
        {
            errors.Add("معرف الصورة مطلوب");
        }

        // التحقق من معرف الكيان
        // Validate property ID
        if (request.PropertyId == Guid.Empty)
        {
            errors.Add("معرف الكيان مطلوب");
        }

        return await Task.FromResult(errors.Any() 
            ? ResultDto<bool>.Failed(errors)
            : ResultDto<bool>.Ok(true));
    }

    /// <summary>
    /// التحقق من صلاحيات المستخدم
    /// Validate user permissions
    /// </summary>
    private async Task<ResultDto<bool>> ValidatePermissionsAsync(PropertyImage image, Property targetProperty, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return ResultDto<bool>.Failed(["المستخدم غير مسجل الدخول"]);
        }

        // التحقق من صلاحية إدارة الصورة الحالية
        // Check permission to manage current image
        var canManageImage = false;
        if (image.PropertyId.HasValue)
        {
            var currentProperty = await _propertyRepository.GetPropertyByIdAsync(image.PropertyId.Value, cancellationToken);
            canManageImage = currentProperty != null && 
                (currentProperty.OwnerId == userId || _currentUserService.IsStaffInProperty(image.PropertyId.Value));
        }
        else if (image.UnitId.HasValue)
        {
            var unit = await _unitRepository.GetByIdAsync(image.UnitId.Value, cancellationToken);
            if (unit != null)
            {
                var currentProperty = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
                canManageImage = currentProperty != null && 
                    (currentProperty.OwnerId == userId || _currentUserService.IsStaffInProperty(unit.PropertyId));
            }
        }

        if (!canManageImage)
        {
            return ResultDto<bool>.Failed(["ليس لديك صلاحية لإدارة هذه الصورة"]);
        }

        // التحقق من صلاحية إدارة الكيان المستهدف
        // Check permission to manage target property
        var canManageTargetProperty = targetProperty.OwnerId == userId || 
            _currentUserService.IsStaffInProperty(targetProperty.Id);

        if (!canManageTargetProperty)
        {
            return ResultDto<bool>.Failed(["ليس لديك صلاحية لإدارة الكيان المستهدف"]);
        }

        return ResultDto<bool>.Ok(true);
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(PropertyImage existingImage, Property targetProperty, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الصورة ليست مربوطة بالكيان المستهدف مسبقاً
        // Check image is not already assigned to target property
        if (existingImage.PropertyId == targetProperty.Id)
        {
            errors.Add("الصورة مربوطة بالكيان المستهدف مسبقاً");
        }

        // التحقق من حالة الصورة
        // Check image status
        if (existingImage.Status == ImageStatus.Rejected)
        {
            errors.Add("لا يمكن ربط صورة مرفوضة");
        }

        // التحقق من حالة الكيان
        // Check property status
        if (!targetProperty.IsActive)
        {
            errors.Add("لا يمكن ربط صورة بكيان غير نشط");
        }

        return await Task.FromResult(errors.Any() 
            ? ResultDto<bool>.Failed(errors)
            : ResultDto<bool>.Ok(true));
    }

    #endregion

    #region Image Processing

    /// <summary>
    /// تحديث حالة الصورة الرئيسية للصور الأخرى
    /// Update main image status for other images
    /// </summary>
    private async Task UpdateMainImageStatusAsync(PropertyImage newMainImage, CancellationToken cancellationToken)
    {
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
    }

    /// <summary>
    /// إنشاء وإرسال حدث ربط الصورة
    /// Create and publish image assigned event
    /// </summary>
    private async Task PublishImageAssignedEventAsync(PropertyImage assignedImage, Property targetProperty, Guid? oldPropertyId, Guid? oldUnitId, CancellationToken cancellationToken)
    {
        var eventData = new PropertyImageAssignedToPropertyEvent
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            EventType = "PropertyImageAssignedToProperty",
            Version = 1,
            UserId = _currentUserService.UserId,
            CorrelationId = Guid.NewGuid().ToString(),
            ImageId = assignedImage.Id,
            NewPropertyId = assignedImage.PropertyId!.Value,
            OldPropertyId = oldPropertyId,
            OldUnitId = oldUnitId,
            IsMainImage = assignedImage.IsMainImage,
            AssignedAt = DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(eventData, cancellationToken);
    }

    /// <summary>
    /// إرسال الإشعارات اللازمة
    /// Send necessary notifications
    /// </summary>
    private async Task SendNotificationsAsync(PropertyImage assignedImage, Property targetProperty, Guid? oldPropertyId, Guid? oldUnitId, CancellationToken cancellationToken)
    {
        try
        {
            // إشعار مالك الكيان المستهدف
            // Notify target property owner
            if (targetProperty.OwnerId != _currentUserService.UserId)
            {
                var notificationRequest = new NotificationRequest
                {
                    UserId = targetProperty.OwnerId,
                    Type = NotificationType.BookingUpdated, // Use available type as placeholder
                    Title = "تم إضافة صورة للكيان",
                    Message = $"تم إضافة صورة جديدة لكيانك: {targetProperty.Name}",
                    Data = new { ImageId = assignedImage.Id, PropertyId = targetProperty.Id }
                };

                await _notificationService.SendAsync(notificationRequest, cancellationToken);
            }

            // إشعار مالك الكيان السابق (إذا كان مختلفاً)
            // Notify previous property owner (if different)
            if (oldPropertyId.HasValue && oldPropertyId != targetProperty.Id)
            {
                var oldProperty = await _propertyRepository.GetPropertyByIdAsync(oldPropertyId.Value, cancellationToken);
                if (oldProperty != null && oldProperty.OwnerId != _currentUserService.UserId && oldProperty.OwnerId != targetProperty.OwnerId)
                {
                    var notificationRequest = new NotificationRequest
                    {
                        UserId = oldProperty.OwnerId,
                        Type = NotificationType.BookingUpdated, // Use available type as placeholder
                        Title = "تم نقل صورة من الكيان",
                        Message = $"تم نقل صورة من كيانك: {oldProperty.Name}",
                        Data = new { ImageId = assignedImage.Id, PropertyId = oldProperty.Id }
                    };

                    await _notificationService.SendAsync(notificationRequest, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "فشل في إرسال الإشعارات لربط الصورة - ImageId: {ImageId}", assignedImage.Id);
            // لا نريد أن يفشل الربط بسبب فشل الإشعارات
            // We don't want assignment to fail because of notification failures
        }
    }

    #endregion
}

/// <summary>
/// حدث ربط صورة الكيان بكيان
/// Property image assigned to property event
/// </summary>
public class PropertyImageAssignedToPropertyEvent : IPropertyImageAssignedToPropertyEvent
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
    /// معرف المستخدم الذي قام بالربط
    /// ID of the user who performed the assignment
    /// </summary>
    public Guid? UserId { get; set; }

    public Guid PropertyId  { get; set; }

    /// <summary>
    /// معرف الصورة المربوطة
    /// Assigned image ID
    /// </summary>
    public Guid ImageId { get; set; }

    /// <summary>
    /// معرف الكيان الجديد
    /// New property ID
    /// </summary>
    public Guid NewPropertyId { get; set; }

    /// <summary>
    /// معرف الكيان السابق (إن وجد)
    /// Old property ID (if any)
    /// </summary>
    public Guid? OldPropertyId { get; set; }

    /// <summary>
    /// معرف الوحدة السابقة (إن وجدت)
    /// Old unit ID (if any)
    /// </summary>
    public Guid? OldUnitId { get; set; }

    /// <summary>
    /// هل الصورة رئيسية
    /// Is main image
    /// </summary>
    public bool IsMainImage { get; set; }

    /// <summary>
    /// تاريخ الربط
    /// Assignment date
    /// </summary>
    public DateTime AssignedAt { get; set; }

}
