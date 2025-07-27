namespace YemenBooking.Application.Commands.UnitFieldValues;

using System;
using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// تحديث متعدد لقيم حقول الوحدة
/// Bulk update unit field values
/// </summary>
public class BulkUpdateUnitFieldValuesCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الوحدة
    /// UnitId
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// قائمة قيم الحقول
    /// FieldValues
    /// </summary>
    public List<FieldValueDto> FieldValues { get; set; }
} 