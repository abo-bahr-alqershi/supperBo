using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reports;

/// <summary>
/// استعلام للحصول على بلاغ حسب المعرف
/// Query to get a report by identifier
/// </summary>
public class GetReportByIdQuery : IRequest<ResultDto<ReportDto>>
{
    /// <summary>
    /// معرف البلاغ
    /// Report identifier
    /// </summary>
    public Guid Id { get; set; }
} 