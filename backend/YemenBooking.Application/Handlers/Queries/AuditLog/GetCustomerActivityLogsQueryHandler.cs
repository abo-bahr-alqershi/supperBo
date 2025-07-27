using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.AuditLog;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.AuditLog
{
    /// <summary>
    /// معالج استعلام لجلب آخر سجلات النشاطات للعميل الحالي
    /// Handler for GetCustomerActivityLogsQuery
    /// </summary>
    public class GetCustomerActivityLogsQueryHandler : IRequestHandler<GetCustomerActivityLogsQuery, PaginatedResult<AuditLogDto>>
    {
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerActivityLogsQueryHandler> _logger;

        public GetCustomerActivityLogsQueryHandler(
            IAuditService auditService,
            ICurrentUserService currentUserService,
            ILogger<GetCustomerActivityLogsQueryHandler> logger)
        {
            _auditService = auditService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<AuditLogDto>> Handle(GetCustomerActivityLogsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب سجلات النشاطات للعميل الحالي");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new BusinessRuleException("Unauthorized", "يجب تسجيل الدخول لعرض سجلات النشاطات");

            var userId = _currentUserService.UserId;

            var logs = await _auditService.GetAuditTrailAsync(
                entityType: null,
                entityId: null,
                performedBy: userId,
                cancellationToken: cancellationToken);

            var filteredLogs = logs
                .OrderByDescending(log => log.CreatedAt);

            var totalCount = filteredLogs.Count();
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var items = filteredLogs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(log => new AuditLogDto
                {
                    Id = log.Id,
                    TableName = log.EntityType,
                    Action = log.Action.ToString(),
                    RecordId = log.EntityId ?? Guid.Empty,
                    UserId = log.PerformedBy ?? Guid.Empty,
                    Changes = log.Notes ?? string.Empty,
                    Timestamp = log.CreatedAt
                });

            return new PaginatedResult<AuditLogDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 