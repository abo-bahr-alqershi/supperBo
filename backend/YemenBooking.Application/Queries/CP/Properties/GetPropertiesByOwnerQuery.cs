using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Properties;

/// <summary>
/// استعلام للحصول على كيانات المالك
/// Query to get properties by owner
/// </summary>
public class GetPropertiesByOwnerQuery : IRequest<PaginatedResult<PropertyDto>>
{
    /// <summary>
    /// معرف المالك
    /// Owner ID
    /// </summary>
    public Guid OwnerId { get; set; }

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