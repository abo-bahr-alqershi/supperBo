using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Queries.CP.Properties;

/// <summary>
/// استعلام للحصول على مرافق الكيان
/// Query to get property amenities
/// </summary>
public class GetPropertyAmenitiesQuery : IRequest<ResultDto<IEnumerable<AmenityDto>>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
} 