using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System.Linq;
using YemenBooking.Core.Notifications;

namespace YemenBooking.Application.Handlers.Commands.Bookings;

/// <summary>
/// معالج أمر تسجيل الوصول
/// Check-in command handler
/// </summary>
public class CheckInCommandHandler : IRequestHandler<CheckInCommand, ResultDto<bool>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CheckInCommandHandler> _logger;

    public CheckInCommandHandler(
        IBookingRepository bookingRepository,
        IUnitRepository unitRepository,
        IPropertyRepository propertyRepository,
        IValidationService validationService,
        IAuditService auditService,
        INotificationService notificationService,
        IEventPublisher eventPublisher,
        ICurrentUserService currentUserService,
        ILogger<CheckInCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _unitRepository = unitRepository;
        _propertyRepository = propertyRepository;
        _validationService = validationService;
        _auditService = auditService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر تسجيل الوصول للحجز {BookingId} / Starting check-in processing for booking: {BookingId}", request.BookingId);

            // الخطوة 1: التحقق من صحة البيانات المدخلة
            // Step 1: Input data validation
            var inputValidationResult = await ValidateInputAsync(request, cancellationToken);
            if (!inputValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من صحة البيانات المدخلة: {Errors} / Input validation failed: {Errors}", string.Join(", ", inputValidationResult.Errors));
                return ResultDto<bool>.Failed(inputValidationResult.Errors);
            }

            // الخطوة 2: التحقق من وجود الحجز
            // Step 2: Booking existence validation
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("Booking not found for ID: {BookingId}", request.BookingId);
                return ResultDto<bool>.Failure("Booking not found.");
            }

            // الخطوة 3: التحقق من صلاحيات المستخدم
            // Step 3: User authorization validation
            var authorizationResult = await ValidateAuthorizationAsync(booking, cancellationToken);
            if (!authorizationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من الصلاحيات للمستخدم: {UserId} / Authorization failed for user: {UserId}", _currentUserService.UserId);
                return ResultDto<bool>.Failed(authorizationResult.Errors);
            }

            // الخطوة 4: التحقق من قواعد الأعمال
            // Step 4: Business rules validation
            var businessRulesResult = await ValidateBusinessRulesAsync(booking, cancellationToken);
            if (!businessRulesResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من قواعد الأعمال: {Errors} / Business rules validation failed: {Errors}", string.Join(", ", businessRulesResult.Errors));
                return ResultDto<bool>.Failed(businessRulesResult.Errors);
            }

            // الخطوة 5: التحقق من حالة الحجز
            // Step 5: Booking state validation
            var stateValidationResult = await ValidateBookingStateAsync(booking, cancellationToken);
            if (!stateValidationResult.IsSuccess)
            {
                _logger.LogWarning("فشل التحقق من حالة الحجز: {Errors} / Booking state validation failed: {Errors}", string.Join(", ", stateValidationResult.Errors));
                return ResultDto<bool>.Failed(stateValidationResult.Errors);
            }

            // الخطوة 6: معالجة تسجيل الوصول
            // Step 6: Process check-in
            var checkInResult = await ProcessCheckInAsync(booking, cancellationToken);
            if (!checkInResult.IsSuccess)
            {
                _logger.LogError("فشل في معالجة تسجيل الوصول: {Errors} / Check-in processing failed: {Errors}", string.Join(", ", checkInResult.Errors));
                return ResultDto<bool>.Failed(checkInResult.Errors);
            }

            // الخطوة 7: تسجيل العملية ونشر الأحداث
            // Step 7: Audit logging and event publishing
            await LogAuditAndPublishEventsAsync(booking, cancellationToken);

            _logger.LogInformation("تم تسجيل الوصول للحجز بنجاح: {BookingId} / Check-in completed successfully for booking: {BookingId}", booking.Id);
            return ResultDto<bool>.Succeeded(true, "تم تسجيل الوصول بنجاح / Check-in completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة أمر تسجيل الوصول للحجز: {BookingId} / Error processing check-in command for booking: {BookingId}", request.BookingId);
            return ResultDto<bool>.Failed("حدث خطأ أثناء تسجيل الوصول / An error occurred while processing check-in");
        }
    }

    private async Task<ResultDto<bool>> ValidateInputAsync(CheckInCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من معرف الحجز
        // Validate booking ID
        if (request.BookingId == Guid.Empty)
        {
            errors.Add("معرف الحجز مطلوب / Booking ID is required");
        }

        // التحقق من صحة البيانات باستخدام خدمة التحقق
        // Validate data using validation service
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.Errors.Select(e => e.Message));
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من الصلاحيات
    /// Authorization validation
    /// </summary>
    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(Booking booking, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        // التحقق من أن المستخدم مسؤول
        if (_currentUserService.Role == "Admin")
        {
            return ResultDto<bool>.Succeeded(true);
        }

        // التحقق من كون المستخدم مالك الكيان
        if (_currentUserService.Role == "Owner")
        {
            var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
            if (unit != null)
            {
                var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
                if (property != null && property.OwnerId == currentUserId)
                    return ResultDto<bool>.Succeeded(true);
            }
        }

        // التحقق من أن الضيف يحجز بنفسه
        if (booking.UserId == currentUserId)
        {
            return ResultDto<bool>.Succeeded(true);
        }

        return ResultDto<bool>.Failed("ليس لديك صلاحية لتسجيل الوصول لهذا الحجز / You don't have permission to check-in this booking");
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Business rules validation
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBusinessRulesAsync(Booking booking, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الحجز لم يتم تسجيل الوصول له مسبقًا
        // Check that booking has not been checked-in already
        if (booking.Status == BookingStatus.CheckedIn || booking.Status == BookingStatus.Completed)
        {
            errors.Add("تم تسجيل الوصول لهذا الحجز مسبقًا / Check-in has already been completed for this booking");
        }

        // التحقق من أن تاريخ الوصول قد حان
        // Check that check-in date has arrived
        if (booking.CheckIn > DateTime.UtcNow)
        {
            errors.Add("لا يمكن تسجيل الوصول قبل تاريخ الوصول المحدد / Cannot check-in before the scheduled check-in date");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من حالة الحجز
    /// Booking state validation
    /// </summary>
    private async Task<ResultDto<bool>> ValidateBookingStateAsync(Booking booking, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // التحقق من أن الحجز في حالة تسمح بتسجيل الوصول
        // Check that booking is in a state that allows check-in
        if (booking.Status != BookingStatus.Confirmed)
        {
            _logger.LogWarning("Booking {BookingId} is not confirmed. Current status: {Status}", booking.Id, booking.Status);
            return ResultDto<bool>.Failed($"Booking is not confirmed. Current status: {booking.Status}");
        }

        return errors.Any() ? ResultDto<bool>.Failed(errors) : ResultDto<bool>.Succeeded(true);
    }

    private async Task<ResultDto<bool>> ProcessCheckInAsync(Booking booking, CancellationToken cancellationToken)
    {
        try
        {
            // تحديث حالة الحجز إلى مسجل الوصول
            // Update booking status to checked-in
            booking.Status = BookingStatus.CheckedIn;
            booking.UpdatedAt = DateTime.UtcNow;

            // حفظ التغييرات في قاعدة البيانات
            // Save changes to database
            await _bookingRepository.UpdateAsync(booking, cancellationToken);

            return ResultDto<bool>.Succeeded(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة تسجيل الوصول للحجز {BookingId} / Error processing check-in for booking {BookingId}", booking.Id);
            return ResultDto<bool>.Failed("حدث خطأ أثناء تسجيل الوصول / An error occurred while processing check-in");
        }
    }

    private async Task LogAuditAndPublishEventsAsync(Booking booking, CancellationToken cancellationToken)
    {
        // تسجيل العملية
        // Audit logging
        await _auditService.LogAsync(
            "CheckInBooking",
            booking.Id.ToString(),
            $"تم تسجيل الوصول للحجز / Check-in completed for booking",
            _currentUserService.UserId,
            cancellationToken);

        // نشر حدث تسجيل الوصول
        // Publish check-in event
        await _eventPublisher.PublishAsync(new BookingCheckedInEvent
        {
            BookingId = booking.Id,
            UnitId = booking.UnitId,
            UserId = _currentUserService.UserId,
            CheckedInAt = DateTime.UtcNow,
            CheckedInBy = _currentUserService.UserId,
            CorrelationId = _currentUserService.CorrelationId
        }, cancellationToken);

        // إرسال إشعار للضيف
        // Send notification to guest
        await _notificationService.SendAsync(new NotificationRequest
        {
            UserId = booking.UserId,
            Type = NotificationType.CheckInCompleted,
            Title = "تم تسجيل الوصول / Check-in Completed",
            Message = $"تم تسجيل وصولك بنجاح / Your check-in has been completed successfully",
            Data = new { BookingId = booking.Id }
        }, cancellationToken);

        // إرسال إشعار لمالك الكيان
        // Send notification to property owner
        var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
        if (unit != null)
        {
            var property = await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken);
            if (property != null && property.OwnerId != Guid.Empty)
            {
                await _notificationService.SendAsync(new NotificationRequest
                {
                    UserId = property.OwnerId,
                    Type = NotificationType.CheckInCompleted,
                    Title = "تم تسجيل وصول ضيف / Guest Check-in Completed",
                    Message = $"تم تسجيل وصول ضيف في وحدتك / A guest has checked in to your unit",
                    Data = new { BookingId = booking.Id, UnitId = unit.Id }
                }, cancellationToken);
            }
        }
    }
}

/// <summary>
/// حدث تسجيل الوصول
/// Booking checked-in event
/// </summary>
public class BookingCheckedInEvent : IBookingCheckedInEvent
{
    public Guid BookingId { get; set; }
    public Guid UnitId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CheckedInAt { get; set; }
    public Guid CheckedInBy { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(BookingCheckedInEvent);
    public int Version { get; set; } = 1;
    public string CorrelationId { get; set; }
}
