using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.MobileApp.Notifications;

/// <summary>
/// استعلام جلب إشعارات المستخدم للعميل
/// Query to get user notifications for client
/// </summary>
public class ClientGetUserNotificationsQuery : IRequest<ResultDto<PaginatedResult<ClientNotificationDto>>>
{
    /// <summary>
    /// معرف المستخدم
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// فلتر حسب الحالة (قراءة، غير مقروء)
    /// Filter by status (read, unread)
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// فلتر حسب النوع
    /// Filter by type
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// فقط الإشعارات غير المقروءة
    /// Only unread notifications
    /// </summary>
    public bool? UnreadOnly { get; set; }

    /// <summary>
    /// تاريخ البداية للفلترة
    /// Start date for filtering
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// تاريخ النهاية للفلترة
    /// End date for filtering
    /// </summary>
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// بيانات الإشعار للعميل
/// Client notification data
/// </summary>
public class ClientNotificationDto
{
    /// <summary>
    /// معرف الإشعار
    /// Notification ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// العنوان
    /// Title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// المحتوى
    /// Content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// نوع الإشعار
    /// Notification type
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// هل مقروء
    /// Is read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// تاريخ الإنشاء
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// تاريخ القراءة
    /// Read date
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// أيقونة الإشعار
    /// Notification icon
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// صورة الإشعار
    /// Notification image
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// بيانات إضافية (JSON)
    /// Additional data (JSON)
    /// </summary>
    public string? AdditionalData { get; set; }

    /// <summary>
    /// رابط الإجراء
    /// Action URL
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// أولوية الإشعار
    /// Notification priority
    /// </summary>
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// هل يمكن تجاهله
    /// Can be dismissed
    /// </summary>
    public bool CanDismiss { get; set; } = true;
}