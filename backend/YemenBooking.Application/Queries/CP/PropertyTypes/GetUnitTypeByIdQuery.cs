using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.PropertyTypes
{
    /// <summary>
    /// استعلام للحصول على نوع وحدة محدد
    /// Query to get a specific unit type by ID
    /// </summary>
    public class GetUnitTypeByIdQuery : IRequest<ResultDto<UnitTypeDto>>
    {
        /// <summary>
        /// معرف نوع الوحدة
        /// </summary>
        public Guid UnitTypeId { get; set; }
    }
} 