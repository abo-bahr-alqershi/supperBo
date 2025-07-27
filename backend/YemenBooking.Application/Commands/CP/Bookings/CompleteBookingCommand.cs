using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لإكمال الحجز (checkout)
/// Command to complete a booking (checkout)
/// </summary>
public class CompleteBookingCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
} 