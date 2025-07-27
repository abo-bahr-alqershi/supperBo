using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Reports;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Reports
{
    /// <summary>
    /// معالج استعلام الحصول على بلاغ حسب المعرف
    /// Handles GetReportByIdQuery and returns the requested report
    /// </summary>
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ResultDto<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetReportByIdQueryHandler> _logger;

        public GetReportByIdQueryHandler(
            IReportRepository reportRepository,
            ICurrentUserService currentUserService,
            ILogger<GetReportByIdQueryHandler> logger)
        {
            _reportRepository = reportRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<ReportDto>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب البلاغ بالمعرف: {Id}", request.Id);

            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("معرف البلاغ غير صالح");
                return ResultDto<ReportDto>.Failure("معرف البلاغ غير صالح");
            }

            var report = await _reportRepository.GetReportByIdAsync(request.Id, cancellationToken);
            if (report == null)
            {
                _logger.LogWarning("البلاغ غير موجود: {Id}", request.Id);
                return ResultDto<ReportDto>.Failure("البلاغ غير موجود");
            }

            var currentUserId = _currentUserService.UserId;
            var role = _currentUserService.Role;
            if (role != "Admin" && currentUserId != report.ReporterUserId && currentUserId != report.ReportedUserId)
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحيات للوصول إلى هذا البلاغ");
                return ResultDto<ReportDto>.Failure("ليس لديك صلاحية الوصول إلى هذا البلاغ");
            }

            var dto = new ReportDto
            {
                Id = report.Id,
                ReporterUserId = report.ReporterUserId,
                ReporterUserName = report.ReporterUser.Name,
                ReportedUserId = report.ReportedUserId,
                ReportedUserName = report.ReportedUser != null ? report.ReportedUser.Name : null,
                ReportedPropertyId = report.ReportedPropertyId,
                ReportedPropertyName = report.ReportedProperty != null ? report.ReportedProperty.Name : null,
                Reason = report.Reason,
                Description = report.Description,
                CreatedAt = report.CreatedAt
            };

            return ResultDto<ReportDto>.Ok(dto);
        }
    }
} 