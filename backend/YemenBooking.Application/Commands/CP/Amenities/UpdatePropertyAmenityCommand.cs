using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Amenities
{
    /// <summary>
    /// أمر لتحديث حالة المرفق في الكيان
    /// Command to update amenity availability and cost in a property
    /// </summary>
    public class UpdatePropertyAmenityCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// معرف المرفق
        /// </summary>
        public Guid AmenityId { get; set; }

        /// <summary>
        /// حالة التوفر
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// التكلفة الإضافية
        /// </summary>
        public MoneyDto ExtraCost { get; set; }
    }
} 