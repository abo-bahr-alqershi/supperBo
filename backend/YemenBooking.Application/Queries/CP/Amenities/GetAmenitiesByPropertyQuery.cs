using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using System.Collections.Generic;

namespace YemenBooking.Application.Queries.Amenities;

/// <summary>
/// استعلام للحصول على مرافق الكيان
/// Query to get amenities by property
/// </summary>
public class GetAmenitiesByPropertyQuery : IRequest<ResultDto<IEnumerable<AmenityDto>>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
} 