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
/// معالج أمر إكمال الحجز
/// Complete booking command handler
/// </summary>
public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, ResultDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CompleteBookingCommandHandler> _logger;

    public CompleteBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        INotificationService notificationService,
        ILogger<CompleteBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _validationService = validationService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر إكمال الحجز {BookingId}", request.BookingId);

            // التحقق من وجود الحجز
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                return ResultDto<bool>.Failed("الحجز غير موجود");
            }

            // التحقق من الصلاحيات
            var authorizationValidation = await ValidateAuthorizationAsync(booking, cancellationToken);
            if (!authorizationValidation.IsSuccess)
            {
                return authorizationValidation;
            }

            // التحقق من حالة الكيان
            var stateValidation = ValidateBookingState(booking);
            if (!stateValidation.IsSuccess)
            {
                return stateValidation;
            }

            // التحقق من قواعد العمل
            var businessRulesValidation = ValidateBusinessRules(booking);
            if (!businessRulesValidation.IsSuccess)
            {
                return businessRulesValidation;
            }

            // إكمال الحجز
            booking.Status = BookingStatus.Completed;
            booking.ActualCheckOutDate = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            // حفظ التغييرات
            await _unitOfWork.Repository<Booking>().UpdateAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogAsync(
                "CompleteBooking",
                booking.Id.ToString(),
                $"تم إكمال الحجز في {booking.ActualCheckOutDate}",
                _currentUserService.UserId,
                cancellationToken);

            // إرسال حدث إكمال الحجز
            // await _eventPublisher.PublishEventAsync(new BookingCompletedEvent
            // {
            //     BookingId = booking.Id,
            //     CompletedBookingId = booking.Id,
            //     UserId = booking.UserId,
            //     UnitId = booking.UnitId,
            //     PropertyId = booking.Unit.PropertyId,
            //     CompletedAt = booking.ActualCheckOutDate.Value,
            //     CompletedBy = _currentUserService.UserId,
            //     PlannedCheckInDate = booking.CheckIn.Date,
            //     PlannedCheckOutDate = booking.CheckOut.Date,
            //     ActualCheckInDate = booking.ActualCheckInDate.Value,
            //     ActualCheckOutDate = booking.ActualCheckOutDate.Value,
            //     FinalAmount = booking.FinalAmount,
            //     CustomerRating = booking.CustomerRating,
            //     CustomerFeedback = booking.CompletionNotes,
            //     CompletionNotes = booking.CompletionNotes,
            //     ActualNights = (booking.CheckOut.Date - booking.CheckIn.Date).Days + 1,
            //     OccurredAt = DateTime.UtcNow,
            //     OccurredOn = DateTime.UtcNow,
            //     EventId = Guid.NewGuid(),
            //     EventType = nameof(BookingCompletedEvent),
            //     Version = 1,
            //     CorrelationId = null
            // }, cancellationToken);

            // إرسال إشعار للضيف
            await _notificationService.SendAsync(new NotificationRequest
            {
                UserId = booking.UserId,
                Type = NotificationType.BookingCompleted,
                Title = "تم إكمال الحجز / Booking Completed",
                Message = $"تم إكمال حجزك بنجاح للحجز رقم {booking.Id} / Your booking {booking.Id} has been completed successfully",
                Data = new { BookingId = booking.Id }
            }, cancellationToken);

            // إرسال إشعار لمالك الكيان
            var unitEnt = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(booking.UnitId, cancellationToken);
            if (unitEnt != null)
            {
                var propertyEnt = await _unitOfWork.Repository<Property>().GetByIdAsync(unitEnt.PropertyId, cancellationToken);
                if (propertyEnt != null && propertyEnt.OwnerId != Guid.Empty)
                {
                    await _notificationService.SendAsync(new NotificationRequest
                    {
                        UserId = propertyEnt.OwnerId,
                        Type = NotificationType.BookingCompleted,
                        Title = "تم إكمال حجز / Booking Completed",
                        Message = $"تم إكمال حجز في وحدتك للحجز رقم {booking.Id} / A booking in your unit has been completed for booking {booking.Id}",
                        Data = new { BookingId = booking.Id, UnitId = unitEnt.Id }
                    }, cancellationToken);
                }
            }

            _logger.LogInformation("تم إكمال الحجز {BookingId} بنجاح", booking.Id);

            return ResultDto<bool>.Succeeded(true, "تم إكمال الحجز بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إكمال الحجز {BookingId}", request.BookingId);
            return ResultDto<bool>.Failed("حدث خطأ أثناء إكمال الحجز");
        }
    }

    /// <summary>
    /// التحقق من الصلاحيات
    /// Authorization validation
    /// </summary>
    private async Task<ResultDto<bool>> ValidateAuthorizationAsync(Booking booking, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var errors = new List<string>();

        // التحقق من أن المستخدم مسؤول
        if (_currentUserService.Role == "Admin")
        {
            return ResultDto<bool>.Succeeded(true);
        }

        // التحقق من كون المستخدم مالك الكيان
        var unit = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(booking.UnitId, cancellationToken);
        var property = await _unitOfWork.Repository<Property>().GetByIdAsync(unit.PropertyId, cancellationToken);
        
        if (property.OwnerId == currentUserId)
        {
            return ResultDto<bool>.Succeeded(true);
        }

        // التحقق من أن المستخدم هو موظف في الكيان
        if (_currentUserService.Role != "Admin" && !_currentUserService.IsStaffInProperty(booking.Unit.PropertyId))
        {
            errors.Add("لست موظفًا في هذا الكيان / You are not a staff member of this property");
        }

        if (errors.Any())
        {
            return ResultDto<bool>.Failed(string.Join("\n", errors));
        }

        return ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من حالة الحجز
    /// Booking state validation
    /// </summary>
    private ResultDto<bool> ValidateBookingState(Booking booking)
    {
        // يمكن إكمال الحجوزات المسجلة الوصول فقط
        if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.CheckedIn)
        {
            _logger.LogWarning("Booking {BookingId} is not in confirmed or checked-in status.", booking.Id);
            return ResultDto<bool>.Failure("Booking is not in confirmed or checked-in status.");
        }

        return ResultDto<bool>.Succeeded(true);
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Business rules validation
    /// </summary>
    private ResultDto<bool> ValidateBusinessRules(Booking booking)
    {
        // التحقق من أن تاريخ اليوم هو تاريخ المغادرة أو بعده
        if (DateTime.Today < booking.CheckOut.Date)
        {
            return ResultDto<bool>.Failed($"لا يمكن إكمال الحجز قبل تاريخ المغادرة المحدد ({booking.CheckOut:yyyy-MM-dd})");
        }

        // التحقق من عدم إكمال الحجز مسبقاً
        if (booking.ActualCheckOutDate.HasValue)
        {
            return ResultDto<bool>.Failed($"تم إكمال الحجز مسبقاً في {booking.ActualCheckOutDate:yyyy-MM-dd HH:mm}");
        }

        // التحقق من تسجيل الوصول
        if (!booking.ActualCheckInDate.HasValue)
        {
            return ResultDto<bool>.Failed("يجب تسجيل الوصول قبل إكمال الحجز");
        }

        return ResultDto<bool>.Succeeded(true);
    }
}

/// <summary>
/// حدث إكمال الحجز
/// Booking completed event
/// </summary>
public class BookingCompletedEvent : IBookingCompletedEvent
{
    public Guid BookingId { get; set; }
    public Guid CompletedBookingId { get; set; }
    public Guid? UserId { get; set; }
    public Guid UnitId { get; set; }
    public Guid PropertyId { get; set; }
    public DateTime CompletedAt { get; set; }
    public Guid? CompletedBy { get; set; }
    public DateTime PlannedCheckInDate { get; set; }
    public DateTime PlannedCheckOutDate { get; set; }
    public DateTime ActualCheckInDate { get; set; }
    public DateTime ActualCheckOutDate { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal? CustomerRating { get; set; }
    public string CustomerFeedback { get; set; }
    public string CompletionNotes { get; set; }
    public int ActualNights { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(BookingCompletedEvent);
    public int Version { get; set; } = 1;
    public string CorrelationId { get; set; }
}
