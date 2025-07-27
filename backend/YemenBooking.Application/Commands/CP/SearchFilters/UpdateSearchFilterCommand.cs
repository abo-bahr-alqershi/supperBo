namespace YemenBooking.Application.Commands.SearchFilters;

using System;
using MediatR;
using System.Collections.Generic;
using Unit = MediatR.Unit;
using YemenBooking.Application.DTOs;

/// <summary>
/// تحديث فلتر بحث
/// Update search filter
/// </summary>
public class UpdateSearchFilterCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الفلتر
    /// FilterId
    /// </summary>
    public Guid FilterId { get; set; }

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
    public bool IsActive { get; set; }

    /// <summary>
    /// ترتيب الفلتر
    /// SortOrder
    /// </summary>
    public int SortOrder { get; set; }
} 