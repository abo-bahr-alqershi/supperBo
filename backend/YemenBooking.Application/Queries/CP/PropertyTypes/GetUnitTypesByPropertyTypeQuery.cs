using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.PropertyTypes
{
    /// <summary>
    /// استعلام للحصول على أنواع الوحدات لنوع كيان معين
    /// Query to get unit types by property type
    /// </summary>
    public class GetUnitTypesByPropertyTypeQuery : IRequest<PaginatedResult<UnitTypeDto>>
    {
        /// <summary>
        /// معرف نوع الكيان
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// رقم الصفحة
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
} 