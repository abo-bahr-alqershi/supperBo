namespace YemenBooking.Application.Queries.UnitTypeFields;

using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب الحقول القابلة للبحث
/// Get searchable dynamic fields
/// </summary>
public class GetSearchableFieldsQuery : IRequest<List<UnitTypeFieldDto>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// PropertyTypeId
    /// </summary>
    public string PropertyTypeId { get; set; }
} 