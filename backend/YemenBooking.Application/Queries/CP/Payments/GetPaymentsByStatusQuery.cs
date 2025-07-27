using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Payments;

/// <summary>
/// استعلام للحصول على المدفوعات حسب الحالة
/// Query to get payments by status
/// </summary>
public class GetPaymentsByStatusQuery : IRequest<PaginatedResult<PaymentDto>>
{
    /// <summary>
    /// حالة الدفع
    /// Payment status
    /// </summary>
    public string Status { get; set; } = string.Empty;

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