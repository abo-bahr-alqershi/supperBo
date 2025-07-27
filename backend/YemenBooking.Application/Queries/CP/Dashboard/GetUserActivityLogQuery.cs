using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Dashboard;

/// <summary>
/// استعلام للحصول على سجلات نشاط المستخدم
/// Query to get user activity logs
/// </summary>
public class GetUserActivityLogQuery : IRequest<ResultDto<IEnumerable<AuditLogDto>>>
{
    /// <summary>
    /// معرف المستخدم
    /// User identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// تاريخ البداية (اختياري)
    /// Start date (optional)
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// تاريخ النهاية (اختياري)
    /// End date (optional)
    /// </summary>
    public DateTime? To { get; set; }
} 