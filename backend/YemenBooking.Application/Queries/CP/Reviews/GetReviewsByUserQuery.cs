using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reviews;

/// <summary>
/// استعلام للحصول على تقييمات المستخدم
/// Query to get reviews by user
/// </summary>
public class GetReviewsByUserQuery : IRequest<PaginatedResult<ReviewDto>>
{
    /// <summary>
    /// معرف المستخدم
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

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

    /// <summary>
    /// فلترة بالتقييمات المعلقة للموافقة (اختياري)
    /// Filter by pending approval (optional)
    /// </summary>
    public bool? IsPendingApproval { get; set; }

    /// <summary>
    /// فلترة بالتقييمات التي لديها رد (اختياري)
    /// Filter by reviews with response (optional)
    /// </summary>
    public bool? HasResponse { get; set; }

    /// <summary>
    /// فلترة بتاريخ التقييم بعد (اختياري)
    /// Filter by reviews after date (optional)
    /// </summary>
    public DateTime? ReviewedAfter { get; set; }

    /// <summary>
    /// خيارات الترتيب: cleanliness_rating, service_rating, review_date (اختياري)
    /// Sort options: cleanliness_rating, service_rating, review_date (optional)
    /// </summary>
    public string? SortBy { get; set; }
} 