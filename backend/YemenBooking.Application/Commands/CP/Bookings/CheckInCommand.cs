using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لتسجيل الوصول (Check-in) لحجز معين
/// Command to mark a booking as checked in
/// </summary>
public class CheckInCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking identifier
    /// </summary>
    public Guid BookingId { get; set; }
} 