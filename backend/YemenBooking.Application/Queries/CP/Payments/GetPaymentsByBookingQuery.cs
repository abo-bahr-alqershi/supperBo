using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Payments;

/// <summary>
/// استعلام للحصول على مدفوعات الحجز
/// Query to get payments by booking
/// </summary>
public class GetPaymentsByBookingQuery : IRequest<PaginatedResult<PaymentDto>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
} 