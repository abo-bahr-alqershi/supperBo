using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.SearchFilters
{
    /// <summary>
    /// استعلام للحصول على الكيانات المميزة
    /// Query to get featured properties
    /// </summary>
    public class GetFeaturedPropertiesQuery : IRequest<PaginatedResult<PropertyDto>>
    {
        /// <summary>
        /// عدد النتائج
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// الموقع (اختياري)
        /// </summary>
        public string? Location { get; set; }
    }
} 