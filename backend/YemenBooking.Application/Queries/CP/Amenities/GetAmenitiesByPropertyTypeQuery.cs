using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using System.Collections.Generic;

namespace YemenBooking.Application.Queries.Amenities;

/// <summary>
/// استعلام للحصول على مرافق نوع الكيان
/// Query to get amenities by property type
/// </summary>
public class GetAmenitiesByPropertyTypeQuery : IRequest<ResultDto<IEnumerable<AmenityDto>>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// Property type ID
    /// </summary>
    public Guid PropertyTypeId { get; set; }
}