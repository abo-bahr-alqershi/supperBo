using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام سجلات نشاط المستخدم
    /// Query handler for user activity logs
    /// </summary>
    public class GetUserActivityLogQueryHandler : IRequestHandler<GetUserActivityLogQuery, ResultDto<IEnumerable<AuditLogDto>>>
    {
        #region Dependencies
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUserActivityLogQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetUserActivityLogQueryHandler(
            IAuditService auditService,
            ICurrentUserService currentUserService,
            ILogger<GetUserActivityLogQueryHandler> logger)
        {
            _auditService = auditService;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<IEnumerable<AuditLogDto>>> Handle(GetUserActivityLogQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب سجلات نشاط المستخدم: {UserId}", request.UserId);

            // 1. Validate input
            if (request.UserId == Guid.Empty)
            {
                _logger.LogWarning("معرف المستخدم غير صالح");
                return ResultDto<IEnumerable<AuditLogDto>>.Failure("معرف المستخدم غير صالح");
            }

            // 2. Authorization: only self or admin
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == Guid.Empty)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<IEnumerable<AuditLogDto>>.Failure("يجب تسجيل الدخول لعرض سجلات النشاط");
            }
            if (currentUserId != request.UserId && _currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم الصلاحيات اللازمة");
                return ResultDto<IEnumerable<AuditLogDto>>.Failure("ليس لديك صلاحية لعرض سجلات نشاط المستخدم");
            }

            // 3. Validate date range
            if (request.From.HasValue && request.To.HasValue && request.From > request.To)
            {
                _logger.LogWarning("تاريخ البداية أكبر من تاريخ النهاية");
                return ResultDto<IEnumerable<AuditLogDto>>.Failure("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");
            }

            // 4. Get audit logs
            var logs = await _auditService.GetAuditTrailAsync(
                entityType: null,
                entityId: null,
                performedBy: request.UserId,
                fromDate: request.From,
                toDate: request.To,
                cancellationToken: cancellationToken);

            // 5. Map to DTOs
            var logDtos = logs
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    TableName = a.EntityType,
                    Action = a.Action.ToString(),
                    RecordId = a.EntityId.GetValueOrDefault(),
                    UserId = a.PerformedBy.GetValueOrDefault(),
                    Changes = a.Notes ?? string.Empty,
                    Timestamp = a.CreatedAt
                })
                .ToList();

            _logger.LogInformation("تم جلب {Count} سجل تدقيق", logDtos.Count);
            return ResultDto<IEnumerable<AuditLogDto>>.Ok(logDtos);
        }
        #endregion
    }
} 