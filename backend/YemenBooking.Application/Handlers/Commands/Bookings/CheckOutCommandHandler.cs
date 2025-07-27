using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Commands.Bookings;

/// <summary>
/// معالج أمر إتمام تسجيل الخروج للغرفة
/// Handler for CheckOutCommand
/// </summary>
public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, ResultDto<bool>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CheckOutCommandHandler> _logger;

    public CheckOutCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<CheckOutCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء إجراء تسجيل خروج للحجز {BookingId}", request.BookingId);

        // التحقق من وجود الحجز
        var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException("Booking", request.BookingId.ToString());

        // التحقق من الصلاحيات
        if (_currentUserService.Role != "Admin" && _currentUserService.UserId != booking.UserId)
            throw new ForbiddenException("غير مصرح لك بإتمام تسجيل الخروج لهذا الحجز");

        // التحقق من الحالة
        if (booking.Status != BookingStatus.Confirmed)
            throw new BusinessRuleException("InvalidStatus", "لا يمكن القيام بعملية تسجيل الخروج للحجز الحالي");

        // تنفيذ التحديث ضمن معاملة
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            booking.Status = BookingStatus.Completed;
            booking.ActualCheckOutDate = DateTime.UtcNow;
            booking.UpdatedBy = _currentUserService.UserId;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateBookingAsync(booking, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogActivityAsync(
                "Booking",
                booking.Id.ToString(),
                "CheckOut",
                $"تم تسجيل خروج الحجز {booking.Id}",
                null,
                booking,
                cancellationToken);

            // نشر الحدث
            // await _eventPublisher.PublishEventAsync(new BookingCheckedOutEvent
            // {
            //     BookingId = booking.Id,
            //     UserId = booking.UserId,
            //     CheckOutDate = booking.ActualCheckOutDate.Value,
            //     CompletedBy = _currentUserService.UserId,
            //     CompletedAt = DateTime.UtcNow
            // }, cancellationToken);

            _logger.LogInformation("تم تسجيل خروج الحجز بنجاح: {BookingId}", booking.Id);
        });

        return ResultDto<bool>.Ok(true);
    }
}

/// <summary>
/// حدث إتمام تسجيل الخروج للحجز
/// Booking checked-out event
/// </summary>
public class BookingCheckedOutEvent
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CheckOutDate { get; set; }
    public Guid CompletedBy { get; set; }
    public DateTime CompletedAt { get; set; }
} 