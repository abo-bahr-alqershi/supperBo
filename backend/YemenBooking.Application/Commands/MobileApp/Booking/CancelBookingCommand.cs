using MediatR;

namespace YemenBooking.Application.Features.Bookings.Commands;

/// <summary>
/// أمر إلغاء الحجز
/// Command to cancel booking
/// </summary>
public class CancelBookingCommand : IRequest<CancelBookingResponse>
{
    /// <summary>
    /// معرف الحجز
    /// </summary>
    public Guid BookingId { get; set; }
    
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// سبب الإلغاء
    /// </summary>
    public string CancellationReason { get; set; } = string.Empty;
}

/// <summary>
/// استجابة إلغاء الحجز
/// </summary>
public class CancelBookingResponse
{
    /// <summary>
    /// نجاح الإلغاء
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// رسالة النتيجة
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// مبلغ الاسترداد إن وجد
    /// </summary>
    public decimal? RefundAmount { get; set; }
}