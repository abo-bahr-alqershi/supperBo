namespace YemenBooking.Application.Queries.UnitFieldValues;

using System;
using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب قيم الحقول لوحدة معينة
/// Get unit field values
/// </summary>
public class GetUnitFieldValuesQuery : IRequest<List<UnitFieldValueDto>>
{
    /// <summary>
    /// معرف الوحدة
    /// UnitId
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// عام فقط (اختياري)
    /// IsPublic
    /// </summary>
    public bool? IsPublic { get; set; }
} 