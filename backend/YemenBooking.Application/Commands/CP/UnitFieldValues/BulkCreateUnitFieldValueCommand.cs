namespace YemenBooking.Application.Commands.UnitFieldValues;

using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// إنشاء جماعي لقيم حقول الوحدات
/// Bulk create unit field values
/// </summary>
public class BulkCreateUnitFieldValueCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// قائمة قيم الحقول لإنشائها
    /// List of field values to create
    /// </summary>
    public List<FieldValueDto> FieldValues { get; set; } = new List<FieldValueDto>();
} 