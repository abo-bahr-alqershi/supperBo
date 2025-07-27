using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.PropertyImages;

/// <summary>
/// استعلام للحصول على جميع الصور الخاصة بوحدة محددة
/// Query to get all images for a specific unit
/// </summary>
public class GetUnitImagesQuery : IRequest<ResultDto<IEnumerable<PropertyImageDto>>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    public Guid UnitId { get; set; }
} 