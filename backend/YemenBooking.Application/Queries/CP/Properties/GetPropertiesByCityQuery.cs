using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Properties;

/// <summary>
/// استعلام للحصول على الكيانات حسب المدينة
/// Query to get properties by city
/// </summary>
public class GetPropertiesByCityQuery : IRequest<PaginatedResult<PropertyDto>>
{
    /// <summary>
    /// اسم المدينة
    /// City name
    /// </summary>
    public string CityName { get; set; } = string.Empty;

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