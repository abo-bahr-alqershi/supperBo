using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.AuditLog;

/// <summary>
/// استعلام لجلب آخر سجلات النشاطات للمدراء
/// Query to get latest activity logs for admins
/// </summary>
public class GetAdminActivityLogsQuery : IRequest<PaginatedResult<AuditLogDto>>
{
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