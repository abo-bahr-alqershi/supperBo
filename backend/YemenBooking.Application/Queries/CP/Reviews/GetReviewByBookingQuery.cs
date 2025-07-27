using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reviews;

/// <summary>
/// استعلام للحصول على تقييم الحجز
/// Query to get review by booking
/// </summary>
public class GetReviewByBookingQuery : IRequest<ResultDto<ReviewDto>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }
} 