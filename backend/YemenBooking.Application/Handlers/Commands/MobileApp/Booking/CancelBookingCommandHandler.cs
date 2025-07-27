using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Bookings.Commands;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Booking;

/// <summary>
/// معالج أمر إلغاء الحجز للعميل عبر تطبيق الجوال
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, CancelBookingResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<CancelBookingCommandHandler> _logger;

    public CancelBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<CancelBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CancelBookingResponse> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء إلغاء الحجز {BookingId} من قبل المستخدم {UserId}", request.BookingId, request.UserId);

        var bookingRepo = _unitOfWork.Repository<Core.Entities.Booking>();
        var booking = await bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            return new CancelBookingResponse { Success = false, Message = "الحجز غير موجود" };
        }

        if (booking.UserId != request.UserId)
        {
            return new CancelBookingResponse { Success = false, Message = "غير مصرح بإلغاء هذا الحجز" };
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = request.CancellationReason;
        booking.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditService.LogBusinessOperationAsync(
            "CancelBooking",
            $"تم إلغاء الحجز {booking.Id}",
            booking.Id,
            "Booking",
            request.UserId,
            null,
            cancellationToken);

        return new CancelBookingResponse
        {
            Success = true,
            Message = "تم إلغاء الحجز بنجاح",
            RefundAmount = 0 // TODO: حساب المبلغ المسترد بناءً على سياسة الإلغاء
        };
    }
}
