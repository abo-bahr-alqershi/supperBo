namespace YemenBooking.Core.Interfaces.Services;

/// <summary>
/// واجهة خدمة التسعير
/// Pricing service interface
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// حساب السعر
    /// Calculate price
    /// </summary>
    Task<decimal> CalculatePriceAsync(
        Guid unitId,
        DateTime checkIn,
        DateTime checkOut,
        int guestCount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// إعادة حساب السعر
    /// Recalculate price
    /// </summary>
    Task<decimal> RecalculatePriceAsync(
        Guid bookingId,
        DateTime? newCheckIn = null,
        DateTime? newCheckOut = null,
        int? newGuestCount = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حساب السعر الأساسي
    /// Calculate base price
    /// </summary>
    Task<decimal> CalculateBasePriceAsync(
        Guid unitId,
        int nights,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حساب الرسوم الإضافية
    /// Calculate additional fees
    /// </summary>
    Task<decimal> CalculateAdditionalFeesAsync(
        Guid unitId,
        int guestCount,
        IEnumerable<Guid>? serviceIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حساب الخصومات
    /// Calculate discounts
    /// </summary>
    Task<decimal> CalculateDiscountsAsync(
        Guid unitId,
        DateTime checkIn,
        DateTime checkOut,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حساب الضرائب
    /// Calculate taxes
    /// </summary>
    Task<decimal> CalculateTaxesAsync(
        decimal baseAmount,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على تفاصيل التسعير
    /// Get pricing breakdown
    /// </summary>
    Task<object> GetPricingBreakdownAsync(
        Guid unitId,
        DateTime checkIn,
        DateTime checkOut,
        int guestCount,
        CancellationToken cancellationToken = default);
}
