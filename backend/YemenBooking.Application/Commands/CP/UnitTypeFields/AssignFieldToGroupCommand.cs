namespace YemenBooking.Application.Commands.UnitTypeFields;

using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// تخصيص حقل لمجموعة
/// Assign field to group
/// </summary>
public class AssignFieldToGroupCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الحقل
    /// FieldId
    /// </summary>
    public string FieldId { get; set; }

    /// <summary>
    /// معرف المجموعة
    /// GroupId
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// ترتيب الحقل داخل المجموعة
    /// SortOrder
    /// </summary>
    public int SortOrder { get; set; }
} 