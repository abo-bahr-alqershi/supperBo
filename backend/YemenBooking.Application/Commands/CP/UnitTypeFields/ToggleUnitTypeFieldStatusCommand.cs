namespace YemenBooking.Application.Commands.UnitTypeFields;

using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// تغيير حالة التفعيل لحقل ديناميكي
/// Toggle active status for dynamic field
/// </summary>
public class ToggleUnitTypeFieldStatusCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحقل
    /// FieldId
    /// </summary>
    public string FieldId { get; set; }

    /// <summary>
    /// الحالة الجديدة
    /// IsActive
    /// </summary>
    public bool IsActive { get; set; }
} 