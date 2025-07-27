using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Payments;

/// <summary>
/// استعلام للحصول على بيانات الدفع بواسطة المعرف
/// Query to get payment details by ID
/// </summary>
public class GetPaymentByIdQuery : IRequest<ResultDto<PaymentDetailsDto>>
{
    /// <summary>
    /// معرف الدفع
    /// Payment ID
    /// </summary>
    public Guid PaymentId { get; set; }
}