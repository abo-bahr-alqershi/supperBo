using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitFieldValues;

/// <summary>
/// أمر إنشاء قيمة حقل الوحدة
/// Create unit field value command
/// </summary>
public class CreateUnitFieldValueCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// معرف حقل نوع الوحدة
    /// Unit type field ID
    /// </summary>
    public Guid UnitTypeFieldId { get; set; }

    /// <summary>
    /// قيمة الحقل
    /// Field value
    /// </summary>
    public string Value { get; set; } = string.Empty;
} 