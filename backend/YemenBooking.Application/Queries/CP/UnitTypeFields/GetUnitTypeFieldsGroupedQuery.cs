namespace YemenBooking.Application.Queries.UnitTypeFields;

using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب الحقول الديناميكية مجمعة حسب المجموعات
/// Get dynamic fields grouped by field groups
/// </summary>
public class GetUnitTypeFieldsGroupedQuery : IRequest<List<FieldGroupWithFieldsDto>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// PropertyTypeId
    /// </summary>
    public string PropertyTypeId { get; set; }

    /// <summary>
    /// عام فقط (اختياري)
    /// IsPublic
    /// </summary>
    public bool? IsPublic { get; set; }
} 