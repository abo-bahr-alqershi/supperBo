using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Properties;

/// <summary>
/// استعلام للحصول على الكيانات حسب النوع
/// Query to get properties by type
/// </summary>
public class GetPropertiesByTypeQuery : IRequest<PaginatedResult<PropertyDto>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// Property type ID
    /// </summary>
    public Guid PropertyTypeId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
} 