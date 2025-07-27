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
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.AuditLog
{
    /// <summary>
    /// معالج استعلام لجلب آخر سجلات النشاطات للمدراء
    /// Handler for GetAdminActivityLogsQuery
    /// </summary>
    public class GetAdminActivityLogsQueryHandler : IRequestHandler<GetAdminActivityLogsQuery, PaginatedResult<AuditLogDto>>
    {
        private readonly IAuditService _auditService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAdminActivityLogsQueryHandler> _logger;

        public GetAdminActivityLogsQueryHandler(
            IAuditService auditService,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            ICurrentUserService currentUserService,
            ILogger<GetAdminActivityLogsQueryHandler> logger)
        {
            _auditService = auditService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<AuditLogDto>> Handle(GetAdminActivityLogsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب سجلات النشاطات للمدراء");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new BusinessRuleException("Unauthorized", "يجب تسجيل الدخول لعرض سجلات النشاطات");

            if (!await _currentUserService.IsInRoleAsync("Admin"))
                throw new BusinessRuleException("Forbidden", "ليس لديك صلاحية لعرض سجلات النشاطات");

            var roles = await _roleRepository.GetAllRolesAsync(cancellationToken);
            var adminRole = roles.FirstOrDefault(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));
            if (adminRole == null)
                throw new BusinessRuleException("NotFound", "دور المسؤول غير موجود");

            var userRoles = await _userRoleRepository.GetUsersInRoleAsync(adminRole.Id, cancellationToken);
            var adminUserIds = userRoles.Select(ur => ur.UserId).ToList();
            if (!adminUserIds.Any())
                return PaginatedResult<AuditLogDto>.Create(Enumerable.Empty<AuditLogDto>(), request.PageNumber, request.PageSize, 0);

            var logs = await _auditService.GetAuditTrailAsync(
                entityType: null,
                entityId: null,
                performedBy: null,
                cancellationToken: cancellationToken);

            var filteredLogs = logs
                .Where(log => log.PerformedBy.HasValue && adminUserIds.Contains(log.PerformedBy.Value))
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