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
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.Interfaces;

namespace YemenBooking.Application.Handlers.Queries.Notifications
{
    /// <summary>
    /// معالج استعلام الحصول على إشعارات المستخدم
    /// Handles GetUserNotificationsQuery and returns paginated user notifications
    /// </summary>
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, PaginatedResult<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUserNotificationsQueryHandler> _logger;

        public GetUserNotificationsQueryHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            ILogger<GetUserNotificationsQueryHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب إشعارات المستخدم: {UserId}, Page={Page}, Size={Size}", request.UserId, request.PageNumber, request.PageSize);

            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new BusinessRuleException("InvalidPagination", "رقم الصفحة وحجم الصفحة يجب أن يكونا أكبر من صفر");

            if (request.UserId == Guid.Empty)
            {
                _logger.LogWarning("معرف المستخدم غير صالح");
                return PaginatedResult<NotificationDto>.Create(new List<NotificationDto>(), request.PageNumber, request.PageSize, 0);
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return PaginatedResult<NotificationDto>.Create(new List<NotificationDto>(), request.PageNumber, request.PageSize, 0);
            }

            if (currentUser.Id != request.UserId && _currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية للوصول إلى هذه الإشعارات");
                return PaginatedResult<NotificationDto>.Create(new List<NotificationDto>(), request.PageNumber, request.PageSize, 0);
            }

            Expression<Func<Notification, bool>> predicate = n => n.RecipientId == request.UserId;
            if (request.IsRead.HasValue)
                predicate = n => n.RecipientId == request.UserId && n.IsRead == request.IsRead.Value;

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