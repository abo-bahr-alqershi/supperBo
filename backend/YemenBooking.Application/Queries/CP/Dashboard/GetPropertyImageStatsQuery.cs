using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Dashboard;

/// <summary>
/// استعلام للحصول على إحصائيات صور الكيان
/// Query to get property image statistics
/// </summary>
public class GetPropertyImageStatsQuery : IRequest<ResultDto<PropertyImageStatsDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property identifier
    /// </summary>
    public Guid PropertyId { get; set; }
} 