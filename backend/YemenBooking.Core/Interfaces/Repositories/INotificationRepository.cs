using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Interfaces.Repositories;

/// <summary>
/// واجهة مستودع الإشعارات
/// Notification repository interface
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    /// <summary>
    /// إنشاء إشعار جديد
    /// Create new notification
    /// </summary>
    Task<bool> CreateNotificationAsync(
        Guid userId,
        string title,
        string message,
        string type = "INFO",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على إشعارات المستخدم
    /// Get user notifications
    /// </summary>
    Task<IEnumerable<object>> GetUserNotificationsAsync(
        Guid userId,
        bool? isRead = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على إشعارات النظام
    /// Get system notifications
    /// </summary>
    Task<IEnumerable<object>> GetSystemNotificationsAsync(
        string? notificationType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تحديث حالة قراءة الإشعار
    /// Update notification read status
    /// </summary>
    Task<bool> MarkNotificationAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تحديث حالة قراءة جميع إشعارات المستخدم
    /// Mark all user notifications as read
    /// </summary>
    Task<bool> MarkAllUserNotificationsAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف الإشعار
    /// Delete notification
    /// </summary>
    Task<bool> DeleteNotificationAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عدد الإشعارات غير المقروءة
    /// Get unread notifications count
    /// </summary>
    Task<int> GetUnreadNotificationsCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عدد إشعارات المستخدم
    /// Get user notifications count
    /// </summary>
    Task<int> GetUserNotificationsCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عدد الإشعارات غير المقروءة للمستخدم
    /// Get user unread notifications count
    /// </summary>
    Task<int> GetUserUnreadNotificationsCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عدد الإشعارات حسب النوع
    /// Get notifications count by type
    /// </summary>
    Task<int> GetNotificationsCountByTypeAsync(
        Guid userId,
        string notificationType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عدد الإشعارات حسب الأولوية
    /// Get notifications count by priority
    /// </summary>
    Task<int> GetNotificationsCountByPriorityAsync(
        Guid userId,
        string priority,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على آخر إشعار للمستخدم
    /// Get last user notification
    /// </summary>
    Task<Notification?> GetLastUserNotificationAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الإشعارات عالية الأولوية غير المقروءة
    /// Get high priority unread notifications
    /// </summary>
    Task<IEnumerable<Notification>> GetHighPriorityUnreadNotificationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
