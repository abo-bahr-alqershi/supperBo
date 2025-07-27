using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Units
{
    /// <summary>
    /// استعلام لحساب سعر الوحدة لفترة محددة
    /// Query to calculate the unit price for a specified period
    /// </summary>
    public class GetUnitPriceQuery : IRequest<ResultDto<decimal>>
    {
        /// <summary>
        /// معرف الوحدة
        /// Unit ID
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// تاريخ البدء (تاريخ الوصول)
        /// Check-in date
        /// </summary>
        public DateTime CheckIn { get; set; }

        /// <summary>
        /// تاريخ الانتهاء (تاريخ المغادرة)
        /// Check-out date
        /// </summary>
        public DateTime CheckOut { get; set; }

        /// <summary>
        /// عدد الضيوف
        /// Number of guests
        /// </summary>
        public int? GuestCount { get; set; }
    }
} 