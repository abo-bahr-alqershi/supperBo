using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.PropertyImages;

/// <summary>
/// استعلام للحصول على جميع الصور الخاصة بكيان محدد
/// Query to get all images for a specific property
/// </summary>
public class GetPropertyImagesQuery : IRequest<ResultDto<PaginatedResult<PropertyImageDto>>>
{
    /// <summary>
    /// معرف الكيان (اختياري)
    /// Property identifier (optional)
    /// </summary>
    public Guid? PropertyId { get; set; }

    /// <summary>
    /// معرف الوحدة (اختياري)
    /// Unit identifier (optional)
    /// </summary>
    public Guid? UnitId { get; set; }

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