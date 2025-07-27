using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Properties;

/// <summary>
/// استعلام للحصول على الكيانات القريبة من موقع معين
/// Query to get properties near a specific location
/// </summary>
public class GetPropertiesNearLocationQuery : IRequest<PaginatedResult<PropertyDto>>
{
    /// <summary>
    /// خط العرض
    /// Latitude
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// خط الطول
    /// Longitude
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// المسافة بالكيلومترات (اختياري)
    /// Distance in kilometers (optional)
    /// </summary>
    public double Distance { get; set; } = 10.0;

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