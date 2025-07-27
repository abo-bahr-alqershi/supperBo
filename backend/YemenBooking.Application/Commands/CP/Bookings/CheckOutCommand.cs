using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر لتسجيل المغادرة (Check-out) لحجز معين
/// Command to mark a booking as checked out
/// </summary>
public class CheckOutCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking identifier
    /// </summary>
    public Guid BookingId { get; set; }
} 