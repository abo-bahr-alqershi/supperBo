using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لتحديث بيانات الحجز
/// Command to update booking information
/// </summary>
public class UpdateBookingCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// تاريخ الدخول المحدث
    /// Updated check-in date
    /// </summary>
    public DateTime? CheckIn { get; set; }

    /// <summary>
    /// تاريخ الخروج المحدث
    /// Updated check-out date
    /// </summary>
    public DateTime? CheckOut { get; set; }

    /// <summary>
    /// عدد الضيوف المحدث
    /// Updated number of guests
    /// </summary>
    public int? GuestsCount { get; set; }
} 