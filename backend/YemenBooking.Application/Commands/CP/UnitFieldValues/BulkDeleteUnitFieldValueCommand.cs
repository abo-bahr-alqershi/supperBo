namespace YemenBooking.Application.Commands.UnitFieldValues;

using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// حذف جماعي لقيم حقول الوحدات
/// Bulk delete unit field values
/// </summary>
public class BulkDeleteUnitFieldValueCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// قائمة معرفات قيم الحقول لحذفها
    /// List of field value identifiers to delete
    /// </summary>
    public List<Guid> ValueIds { get; set; } = new List<Guid>();
} 