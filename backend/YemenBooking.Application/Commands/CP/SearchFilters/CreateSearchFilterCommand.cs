namespace YemenBooking.Application.Commands.SearchFilters;

using System;
using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// إنشاء فلتر بحث
/// Create search filter
/// </summary>
public class CreateSearchFilterCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الحقل
    /// FieldId
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// نوع الفلتر
    /// FilterType
    /// </summary>
    public string FilterType { get; set; }

    /// <summary>
    /// الاسم المعروض
    /// DisplayName
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// خيارات الفلتر (JSON)
    /// FilterOptions
    /// </summary>
    public Dictionary<string, object> FilterOptions { get; set; }

    /// <summary>
    /// حالة التفعيل
    /// IsActive
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ترتيب الفلتر
    /// SortOrder
    /// </summary>
    public int SortOrder { get; set; }
} 