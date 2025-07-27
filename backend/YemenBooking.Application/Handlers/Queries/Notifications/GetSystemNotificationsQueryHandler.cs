using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Notifications;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Notifications
{
    /// <summary>
    /// معالج استعلام الحصول على إشعارات النظام
    /// Handles GetSystemNotificationsQuery and returns paginated system notifications
    /// </summary>
    public class GetSystemNotificationsQueryHandler : IRequestHandler<GetSystemNotificationsQuery, PaginatedResult<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetSystemNotificationsQueryHandler> _logger;

        public GetSystemNotificationsQueryHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            ILogger<GetSystemNotificationsQueryHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<NotificationDto>> Handle(GetSystemNotificationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب إشعارات النظام: الصفحة={Page}, الحجم={Size}", request.PageNumber, request.PageSize);

            if (_currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى إشعارات النظام");
                throw new ForbiddenException("ليس لديك صلاحية الوصول إلى إشعارات النظام");
            }

            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new BusinessRuleException("InvalidPagination", "رقم الصفحة وحجم الصفحة يجب أن يكونا أكبر من صفر");

            Expression<Func<Notification, bool>> predicate = n =>
                (string.IsNullOrEmpty(request.NotificationType) || n.Type == request.NotificationType)
                && (!request.RecipientId.HasValue || n.RecipientId == request.RecipientId.Value)
                && (string.IsNullOrEmpty(request.Status) || n.Status == request.Status)
                && (!request.SentAfter.HasValue || n.CreatedAt >= request.SentAfter.Value)
                && (!request.SentBefore.HasValue || n.CreatedAt <= request.SentBefore.Value);

            var (entities, totalCount) = await _notificationRepository.GetPagedAsync(
                request.PageNumber, request.PageSize,
                predicate, n => n.CreatedAt, false, cancellationToken);

            var items = entities.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.GetTitle("ar"),
                Message = n.GetMessage("ar"),
                Priority = n.Priority,
                Status = n.Status,
                RecipientId = n.RecipientId,
                RecipientName = string.Empty,
                SenderId = n.SenderId,
                SenderName = string.Empty,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt
            }).ToList();

            return PaginatedResult<NotificationDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
} 