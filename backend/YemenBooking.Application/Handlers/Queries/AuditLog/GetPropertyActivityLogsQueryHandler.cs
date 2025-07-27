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
    /// معالج استعلام لجلب آخر سجلات النشاطات للمالكين وموظفي الكيان
    /// Handler for GetPropertyActivityLogsQuery
    /// </summary>
    public class GetPropertyActivityLogsQueryHandler : IRequestHandler<GetPropertyActivityLogsQuery, PaginatedResult<AuditLogDto>>
    {
        private readonly IAuditService _auditService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyActivityLogsQueryHandler> _logger;

        public GetPropertyActivityLogsQueryHandler(
            IAuditService auditService,
            IPropertyRepository propertyRepository,
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyActivityLogsQueryHandler> logger)
        {
            _auditService = auditService;
            _propertyRepository = propertyRepository;
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<AuditLogDto>> Handle(GetPropertyActivityLogsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب سجلات النشاطات للمالكين وموظفي الكيان: {PropertyId}", request.PropertyId);

            if (!request.PropertyId.HasValue || request.PropertyId == Guid.Empty)
                throw new BusinessRuleException("BadRequest", "معرف الكيان غير صالح");

            var propertyId = request.PropertyId.Value;

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new BusinessRuleException("Unauthorized", "يجب تسجيل الدخول لعرض سجلات النشاطات");

            if (!await _currentUserService.IsInRoleAsync("Admin") &&
                !(_currentUserService.PropertyId.HasValue && _currentUserService.PropertyId.Value == propertyId) &&
                !_currentUserService.IsStaffInProperty(propertyId))
            {
                throw new BusinessRuleException("Forbidden", "ليس لديك صلاحية لعرض سجلات النشاطات لهذا الكيان");
            }

            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
                throw new BusinessRuleException("NotFound", "الكيان غير موجود");

            var ownerId = property.OwnerId;
            var staffList = await _staffRepository.GetStaffByPropertyAsync(propertyId, cancellationToken);
            var userIds = staffList.Select(s => s.UserId).ToList();
            userIds.Add(ownerId);

            var logs = await _auditService.GetAuditTrailAsync(
                entityType: null,
                entityId: null,
                performedBy: null,
                cancellationToken: cancellationToken);

            var filteredLogs = logs
                .Where(log => log.PerformedBy.HasValue && userIds.Contains(log.PerformedBy.Value))
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