using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.MobileApp.Notifications;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Notifications;

/// <summary>
/// معالج استعلام الحصول على إشعارات المستخدم للعميل
/// Handler for client get user notifications query
/// </summary>
public class ClientGetUserNotificationsQueryHandler : IRequestHandler<ClientGetUserNotificationsQuery, ResultDto<PaginatedResult<ClientNotificationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientGetUserNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// معالجة استعلام الحصول على إشعارات المستخدم
    /// Handle get user notifications query
    /// </summary>
    /// <param name="request">الطلب</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة مُقسمة من الإشعارات</returns>
    public async Task<ResultDto<PaginatedResult<ClientNotificationDto>>> Handle(ClientGetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notificationRepo = _unitOfWork.Repository<Core.Entities.Notification>();
            var allNotifications = await notificationRepo.GetAllAsync();
            
            var query = allNotifications.Where(n => n.RecipientId == request.UserId);

            // تطبيق الفلاتر
            // Apply filters
            if (!string.IsNullOrEmpty(request.Type))
            {
                query = query.Where(n => n.Type == request.Type);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt <= request.ToDate.Value);
            }

            // الترتيب
            // Ordering
            query = query.OrderByDescending(n => n.CreatedAt);

            // التحويل إلى DTO
            // Convert to DTO
            var notificationDtos = query.Select(n => new ClientNotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            }).ToList();

            // التقسيم
            // Pagination
            var totalCount = notificationDtos.Count;
            var items = notificationDtos
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return ResultDto<PaginatedResult<ClientNotificationDto>>.SuccessResult(new PaginatedResult<ClientNotificationDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }
        catch (Exception)
        {
            // في حالة الخطأ، إرجاع نتيجة فارغة
            // In case of error, return empty result
            return ResultDto<PaginatedResult<ClientNotificationDto>>.Failure("حدث خطأ أثناء جلب الإشعارات");
        }
    }
}