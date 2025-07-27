namespace YemenBooking.Application.Commands.FieldGroups;

using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// إعادة ترتيب مجموعات الحقول
/// Reorder field groups
/// </summary>
public class ReorderFieldGroupsCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// PropertyTypeId
    /// </summary>
    public string PropertyTypeId { get; set; }

    /// <summary>
    /// طلبات ترتيب المجموعات
    /// GroupOrders
    /// </summary>
    public List<GroupOrderDto> GroupOrders { get; set; }
} 