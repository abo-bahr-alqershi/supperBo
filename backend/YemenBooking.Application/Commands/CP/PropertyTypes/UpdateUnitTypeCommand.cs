using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyTypes;

/// <summary>
/// أمر لتحديث نوع الوحدة
/// Command to update a unit type
/// </summary>
public class UpdateUnitTypeCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type identifier
    /// </summary>
    public Guid UnitTypeId { get; set; }

    /// <summary>
    /// الاسم الجديد لنوع الوحدة
    /// New unit type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// الحد الأقصى للسعة الجديدة
    /// New maximum capacity
    /// </summary>
    public int MaxCapacity { get; set; }
} 