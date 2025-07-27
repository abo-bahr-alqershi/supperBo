namespace YemenBooking.Application.Commands.UnitFieldValues;

using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// تحديث جماعي لقيم حقول الوحدات
/// Bulk update unit field values
/// </summary>
public class BulkUpdateUnitFieldValueCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// قائمة قيم الحقول لتحديثها
    /// List of field values to update
    /// </summary>
    public List<FieldValueDto> FieldValues { get; set; } = new List<FieldValueDto>();
} 