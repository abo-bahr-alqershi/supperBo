namespace YemenBooking.Application.Queries.UnitFieldValues;

using System;
using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب قيم الحقول للوحدة مجمعة حسب المجموعات
/// Get unit field values grouped by field groups
/// </summary>
public class GetUnitFieldValuesGroupedQuery : IRequest<List<FieldGroupWithValuesDto>>
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