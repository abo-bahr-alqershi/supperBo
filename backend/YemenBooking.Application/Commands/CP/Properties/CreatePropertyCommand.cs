using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Properties;

/// <summary>
/// أمر لإنشاء كيان جديد
/// Command to create a new property
/// </summary>
public class CreatePropertyCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// اسم الكيان
    /// Property name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// العنوان الكامل للكيان
    /// Full address of the property
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// نوع الكيان
    /// Property type
    /// </summary>
    public Guid PropertyTypeId { get; set; }

    /// <summary>
    /// معرف المالك
    /// Owner ID
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// وصف الكيان
    /// Property description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// خط العرض للموقع الجغرافي
    /// Latitude for geographic location
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// خط الطول للموقع الجغرافي
    /// Longitude for geographic location
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// المدينة
    /// City
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// تقييم النجوم
    /// Star rating
    /// </summary>
    public int StarRating { get; set; }

    /// <summary>
    /// صور الكيان المحدثة
    /// Updated property images
    /// </summary>
    public List<string> Images { get; set; } = new List<string>();
} 