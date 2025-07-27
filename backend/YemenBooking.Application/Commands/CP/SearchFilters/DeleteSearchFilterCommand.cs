namespace YemenBooking.Application.Commands.SearchFilters;

using System;
using MediatR;
using YemenBooking.Application.DTOs;
using Unit = MediatR.Unit;

/// <summary>
/// حذف فلتر بحث
/// Delete search filter
/// </summary>
public class DeleteSearchFilterCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الفلتر
    /// FilterId
    /// </summary>
    public Guid FilterId { get; set; }
} 