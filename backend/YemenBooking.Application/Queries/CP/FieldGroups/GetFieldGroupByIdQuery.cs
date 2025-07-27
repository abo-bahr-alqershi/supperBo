namespace YemenBooking.Application.Queries.FieldGroups;

using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب مجموعة حقول حسب المعرف
/// Get field group by id
/// </summary>
public class GetFieldGroupByIdQuery : IRequest<FieldGroupDto>
{
    /// <summary>
    /// معرف المجموعة
    /// GroupId
    /// </summary>
    public string GroupId { get; set; }
} 