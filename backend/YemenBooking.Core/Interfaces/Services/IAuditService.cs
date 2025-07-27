using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace YemenBooking.Core.Interfaces.Services;

/// <summary>
/// خدمة المراجعة والتدقيق
/// Audit service interface
/// </summary>
public interface IAuditService
{

    /// <summary>
    /// Alias for LogAuditAsync to support LogAsync usage in handlers.
    /// </summary>
    Task<bool> LogAsync(string entityType,
        string entityId,
        string notes,
        Guid performedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Log activity with old and new values support
    /// </summary>
    Task<bool> LogActivityAsync(string entityType,
        string entityId,
        string action,
        string notes,
        object? oldValues,
        object? newValues,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تسجيل عملية تدقيق
    /// Log audit entry
    /// </summary>
    Task<bool> LogAuditAsync(
        string entityType,
        Guid entityId,
        AuditAction action,
        string? oldValues = null,
        string? newValues = null,
        Guid? performedBy = null,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تسجيل تغيير في الكيان
    /// Log entity change
    /// </summary>
    Task<bool> LogEntityChangeAsync<T>(
        T entity,
        AuditAction action,
        T? previousState = default,
        Guid? performedBy = null,
        string? notes = null,
        CancellationToken cancellationToken = default) where T : BaseEntity;

    /// <summary>
    /// تسجيل محاولة دخول
    /// Log login attempt
    /// </summary>
    Task<bool> LogLoginAttemptAsync(
        string username,
        bool isSuccessful,
        string ipAddress,
        string userAgent,
        string? failureReason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تسجيل عملية تجارية
    /// Log business operation
    /// </summary>
    Task<bool> LogBusinessOperationAsync(
        string operationType,
        string operationDescription,
        Guid? entityId = null,
        string? entityType = null,
        Guid? performedBy = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على سجلات التدقيق
    /// Get audit trail
    /// </summary>
    Task<IEnumerable<AuditLog>> GetAuditTrailAsync(
        string? entityType = null,
        Guid? entityId = null,
        Guid? performedBy = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على سجل تدقيق محدد
    /// Get specific audit log
    /// </summary>
    Task<AuditLog?> GetAuditLogAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث في سجلات التدقيق
    /// Search audit logs
    /// </summary>
    Task<IEnumerable<AuditLog>> SearchAuditLogsAsync(
        string searchTerm,
        AuditAction? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على إحصائيات التدقيق
    /// Get audit statistics
    /// </summary>
    Task<AuditStatistics> GetAuditStatisticsAsync(
        DateTime fromDate,
        DateTime toDate,
        string? entityType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// أرشفة سجلات التدقيق القديمة
    /// Archive old audit logs
    /// </summary>
    Task<int> ArchiveOldLogsAsync(
        DateTime olderThan,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تصدير سجلات التدقيق
    /// Export audit logs
    /// </summary>
    Task<byte[]> ExportAuditLogsAsync(
        DateTime fromDate,
        DateTime toDate,
        string format = "CSV",
        string? entityType = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// إجراء التدقيق
/// Audit action
/// </summary>
public enum AuditAction
{
    /// <summary>
    /// إنشاء
    /// Create
    /// </summary>
    [Display(Name = "إنشاء")]
    CREATE,
    
    /// <summary>
    /// تحديث
    /// Update
    /// </summary>
    [Display(Name = "تحديث")]
    UPDATE,
    
    /// <summary>
    /// حذف
    /// Delete
    /// </summary>
    [Display(Name = "حذف")]
    DELETE,
    
    /// <summary>
    /// حذف ناعم
    /// Soft delete
    /// </summary>
    [Display(Name = "حذف ناعم")]
    SOFT_DELETE,
    
    /// <summary>
    /// عرض
    /// View
    /// </summary>
    [Display(Name = "عرض")]
    VIEW,
    
    /// <summary>
    /// تسجيل دخول
    /// Login
    /// </summary>
    LOGIN,
    
    /// <summary>
    /// تسجيل خروج
    /// Logout
    /// </summary>
    LOGOUT,
    
    /// <summary>
    /// تغيير كلمة المرور
    /// Password change
    /// </summary>
    PASSWORD_CHANGE,
    
    /// <summary>
    /// إعادة تعيين كلمة المرور
    /// Password reset
    /// </summary>
    PASSWORD_RESET,
    
    /// <summary>
    /// تفعيل
    /// Activate
    /// </summary>
    ACTIVATE,
    
    /// <summary>
    /// إلغاء التفعيل
    /// Deactivate
    /// </summary>
    DEACTIVATE,
    
    /// <summary>
    /// موافقة
    /// Approve
    /// </summary>
    APPROVE,
    
    /// <summary>
    /// رفض
    /// Reject
    /// </summary>
    REJECT,
    
    /// <summary>
    /// تعليق
    /// Suspend
    /// </summary>
    SUSPEND,
    
    /// <summary>
    /// استيراد
    /// Import
    /// </summary>
    IMPORT,
    
    /// <summary>
    /// تصدير
    /// Export
    /// </summary>
    EXPORT
}

/// <summary>
/// إحصائيات التدقيق
/// Audit statistics
/// </summary>
public class AuditStatistics
{
    public int TotalOperations { get; set; }
    public Dictionary<AuditAction, int> OperationsByAction { get; set; } = new();
    public Dictionary<string, int> OperationsByEntityType { get; set; } = new();
    public Dictionary<string, int> OperationsByUser { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int UniqueUsers { get; set; }
    public int FailedOperations { get; set; }
}
