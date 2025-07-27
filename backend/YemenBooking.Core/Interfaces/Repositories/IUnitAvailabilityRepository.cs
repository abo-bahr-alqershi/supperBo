using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Interfaces.Repositories;

/// <summary>
/// واجهة مستودع توفر الوحدات
/// Unit availability repository interface
/// </summary>
public interface IUnitAvailabilityRepository : IRepository<UnitAvailability>
{
    /// <summary>
    /// تحديث توفر الوحدة
    /// Update unit availability
    /// </summary>
    Task<bool> UpdateAvailabilityAsync(
        Guid unitId,
        DateTime fromDate,
        DateTime toDate,
        bool isAvailable,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على توفر الوحدة
    /// Get unit availability
    /// </summary>
    Task<IDictionary<DateTime, bool>> GetUnitAvailabilityAsync(
        Guid unitId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من توفر الوحدة
    /// Check unit availability
    /// </summary>
    Task<bool> IsUnitAvailableAsync(
        Guid unitId,
        DateTime checkIn,
        DateTime checkOut,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حجز فترة للوحدة
    /// Block unit period
    /// </summary>
    Task<bool> BlockUnitPeriodAsync(
        Guid unitId,
        DateTime fromDate,
        DateTime toDate,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// إلغاء حجز فترة للوحدة
    /// Unblock unit period
    /// </summary>
    Task<bool> UnblockUnitPeriodAsync(
        Guid unitId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من تداخل الفترات
    /// Check for date range overlap
    /// </summary>
    Task<bool> HasOverlapAsync(
        Guid unitId,
        DateTime fromDate,
        DateTime toDate,
        Guid? excludeAvailabilityId = null,
        CancellationToken cancellationToken = default);
}
