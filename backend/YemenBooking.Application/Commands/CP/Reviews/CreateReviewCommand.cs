using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Reviews;

/// <summary>
/// أمر لإنشاء تقييم جديد
/// Command to create a new review
/// </summary>
public class CreateReviewCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الحجز
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// تقييم النظافة
    /// Cleanliness
    /// </summary>
    public int Cleanliness { get; set; }

    /// <summary>
    /// تقييم الخدمة
    /// Service
    /// </summary>
    public int Service { get; set; }

    /// <summary>
    /// تقييم الموقع
    /// Location
    /// </summary>
    public int Location { get; set; }

    /// <summary>
    /// تقييم القيمة
    /// Value
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// تعليق المستخدم
    /// User comment
    /// </summary>
    public string Comment { get; set; } = string.Empty;
} 