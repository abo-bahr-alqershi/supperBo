using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.AuditLog;

/// <summary>
/// استعلام لجلب آخر سجلات النشاطات للعميل الحالي
/// Query to get latest activity logs for the current customer
/// </summary>
public class GetCustomerActivityLogsQuery : IRequest<PaginatedResult<AuditLogDto>>
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