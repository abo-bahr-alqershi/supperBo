namespace YemenBooking.Application.Commands.UnitFieldValues;

using System;
using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// حذف قيمة حقل للوحدة
/// Delete unit field value
/// </summary>
public class DeleteUnitFieldValueCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف القيمة
    /// ValueId
    /// </summary>
    public Guid ValueId { get; set; }
} 