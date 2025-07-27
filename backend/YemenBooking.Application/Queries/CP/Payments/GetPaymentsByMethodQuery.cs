using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Queries.Payments;

/// <summary>
/// استعلام للحصول على المدفوعات حسب طريقة الدفع
/// Query to get payments by method
/// </summary>
public class GetPaymentsByMethodQuery : IRequest<PaginatedResult<PaymentDto>>
{
    /// <summary>
    /// طريقة الدفع
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ البداية (اختياري)
    /// Start date (optional)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية (اختياري)
    /// End date (optional)
    /// </summary>
    public DateTime? EndDate { get; set; }

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