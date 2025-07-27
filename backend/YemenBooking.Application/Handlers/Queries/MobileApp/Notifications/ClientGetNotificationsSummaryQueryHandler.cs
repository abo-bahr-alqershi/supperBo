using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Notifications;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Notifications;

/// <summary>
/// معالج استعلام ملخص الإشعارات للعميل
/// Handler for client get notifications summary query
/// </summary>
public class ClientGetNotificationsSummaryQueryHandler : IRequestHandler<ClientGetNotificationsSummaryQuery, ResultDto<ClientNotificationsSummaryDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ClientGetNotificationsSummaryQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام ملخص الإشعارات
    /// Constructor for client get notifications summary query handler
    /// </summary>
    /// <param name="notificationRepository">مستودع الإشعارات</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="logger">مسجل الأحداث</param>
    public ClientGetNotificationsSummaryQueryHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ILogger<ClientGetNotificationsSummaryQueryHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام ملخص الإشعارات للعميل
    /// Handle client get notifications summary query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>ملخص الإشعارات</returns>
    public async Task<ResultDto<ClientNotificationsSummaryDto>> Handle(ClientGetNotificationsSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام ملخص الإشعارات للعميل. معرف المستخدم: {UserId}", request.UserId);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                return ResultDto<ClientNotificationsSummaryDto>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // الحصول على إحصائيات الإشعارات
            var totalCount = await _notificationRepository.GetUserNotificationsCountAsync(request.UserId, cancellationToken);
            var unreadCount = await _notificationRepository.GetUserUnreadNotificationsCountAsync(request.UserId, cancellationToken);
            var readCount = totalCount - unreadCount;

            // الحصول على عدد الإشعارات حسب النوع
            var countByType = await _notificationRepository.GetNotificationsCountByTypeAsync(request.UserId, cancellationToken);

            // الحصول على عدد الإشعارات حسب الأولوية
            var countByPriority = await _notificationRepository.GetNotificationsCountByPriorityAsync(request.UserId, cancellationToken);

            // الحصول على آخر إشعار
            var lastNotification = await _notificationRepository.GetLastUserNotificationAsync(request.UserId, cancellationToken);
            ClientNotificationDto? lastNotificationDto = null;

            if (lastNotification != null)
            {
                lastNotificationDto = new ClientNotificationDto
                {
                    Id = lastNotification.Id,
                    Title = lastNotification.Title ?? string.Empty,
                    Content = lastNotification.Content ?? string.Empty,
                    Type = lastNotification.Type ?? string.Empty,
                    IsRead = lastNotification.IsRead,
                    CreatedAt = lastNotification.CreatedAt,
                    ReadAt = lastNotification.ReadAt,
                    IconUrl = lastNotification.IconUrl,
                    ImageUrl = lastNotification.ImageUrl,
                    AdditionalData = lastNotification.AdditionalData,
                    ActionUrl = lastNotification.ActionUrl,
                    Priority = lastNotification.Priority ?? "Normal",
                    CanDismiss = lastNotification.CanDismiss
                };
            }

            // الحصول على الإشعارات عالية الأولوية غير المقروءة
            var highPriorityUnreadNotifications = await _notificationRepository.GetHighPriorityUnreadNotificationsAsync(
                request.UserId, 5, cancellationToken); // أحدث 5 إشعارات عالية الأولوية

            var highPriorityUnreadDtos = highPriorityUnreadNotifications?.Select(n => new ClientNotificationDto
            {
                Id = n.Id,
                Title = n.Title ?? string.Empty,
                Content = n.Content ?? string.Empty,
                Type = n.Type ?? string.Empty,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt,
                IconUrl = n.IconUrl,
                ImageUrl = n.ImageUrl,
                AdditionalData = n.AdditionalData,
                ActionUrl = n.ActionUrl,
                Priority = n.Priority ?? "Normal",
                CanDismiss = n.CanDismiss
            }).ToList() ?? new List<ClientNotificationDto>();

            // إنشاء DTO للاستجابة
            var summaryDto = new ClientNotificationsSummaryDto
            {
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                ReadCount = readCount,
                CountByType = countByType ?? new Dictionary<string, int>(),
                CountByPriority = countByPriority ?? new Dictionary<string, int>(),
                LastNotification = lastNotificationDto,
                HighPriorityUnread = highPriorityUnreadDtos
            };

            _logger.LogInformation("تم الحصول على ملخص الإشعارات بنجاح. المستخدم: {UserId}, إجمالي: {Total}, غير مقروء: {Unread}", 
                request.UserId, totalCount, unreadCount);

            return ResultDto<ClientNotificationsSummaryDto>.Ok(
                summaryDto, 
                "تم الحصول على ملخص الإشعارات بنجاح"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على ملخص الإشعارات. معرف المستخدم: {UserId}", request.UserId);
            return ResultDto<ClientNotificationsSummaryDto>.Failed(
                $"حدث خطأ أثناء الحصول على ملخص الإشعارات: {ex.Message}", 
                "GET_NOTIFICATIONS_SUMMARY_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<ClientNotificationsSummaryDto> ValidateRequest(ClientGetNotificationsSummaryQuery request)
    {
        if (request.UserId == Guid.Empty)
        {
            _logger.LogWarning("معرف المستخدم مطلوب");
            return ResultDto<ClientNotificationsSummaryDto>.Failed("معرف المستخدم مطلوب", "USER_ID_REQUIRED");
        }

        return ResultDto<ClientNotificationsSummaryDto>.Ok(null, "البيانات صحيحة");
    }
}
