using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Services
{
    /// <summary>
    /// أمر لإزالة خدمة من الحجز
    /// Command to remove a service from a booking
    /// </summary>
    public class RemoveServiceFromBookingCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الحجز
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// معرف الخدمة
        /// </summary>
        public Guid ServiceId { get; set; }
    }
} 