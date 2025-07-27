using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.AuditLog;

/// <summary>
/// استعلام لجلب آخر سجلات النشاطات للمالكين وموظفي الكيان
/// Query to get latest activity logs for property owners and staff
/// </summary>
public class GetPropertyActivityLogsQuery : IRequest<PaginatedResult<AuditLogDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property identifier
    /// </summary>
    public Guid? PropertyId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
} 