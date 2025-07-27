namespace YemenBooking.Application.Queries.UnitFieldValues;

using System;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب قيمة حقل للوحدة حسب المعرف
/// Get unit field value by id
/// </summary>
public class GetUnitFieldValueByIdQuery : IRequest<UnitFieldValueDto>
{
    /// <summary>
    /// معرف القيمة
    /// ValueId
    /// </summary>
    public Guid ValueId { get; set; }
} 