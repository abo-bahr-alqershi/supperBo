namespace YemenBooking.Application.Queries.Units;

using System;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب بيانات الوحدة للتحرير
/// Get unit data for edit form
/// </summary>
public class GetUnitForEditQuery : IRequest<ResultDto<UnitEditDto>>
{
    /// <summary>
    /// معرف الوحدة
    /// UnitId
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// معرف المالك
    /// OwnerId
    /// </summary>
    public Guid OwnerId { get; set; }
} 