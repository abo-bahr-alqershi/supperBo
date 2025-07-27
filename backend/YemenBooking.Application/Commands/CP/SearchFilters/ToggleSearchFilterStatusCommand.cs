namespace YemenBooking.Application.Commands.SearchFilters;

using System;
using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// تغيير حالة التفعيل لفلتر بحث
/// Toggle search filter status
/// </summary>
public class ToggleSearchFilterStatusCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الفلتر
    /// FilterId
    /// </summary>
    public Guid FilterId { get; set; }

    /// <summary>
    /// حالة التفعيل الجديدة
    /// IsActive
    /// </summary>
    public bool IsActive { get; set; }
} 