namespace YemenBooking.Application.Queries.SearchFilters;

using System;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب فلتر بحث حسب المعرف
/// Get search filter by id
/// </summary>
public class GetSearchFilterByIdQuery : IRequest<SearchFilterDto>
{
    /// <summary>
    /// معرف الفلتر
    /// FilterId
    /// </summary>
    public Guid FilterId { get; set; }
} 