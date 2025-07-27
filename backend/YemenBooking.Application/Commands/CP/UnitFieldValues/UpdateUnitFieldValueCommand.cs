using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitFieldValues;

/// <summary>
/// أمر تحديث قيمة حقل الوحدة
/// Update unit field value command
/// </summary>
public class UpdateUnitFieldValueCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف قيمة الحقل
    /// Field value ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// معرف قيمة الحقل (alias for Id)
    /// Field value ID (alias for Id)
    /// </summary>
    public Guid ValueId 
    { 
        get => Id;
        set => Id = value;
    }

    /// <summary>
    /// القيمة الجديدة للحقل
    /// New field value
    /// </summary>
    public string Value { get; set; } = string.Empty;
} 