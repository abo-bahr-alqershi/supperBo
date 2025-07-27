using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Reviews;

/// <summary>
/// أمر لتحديث التقييم
/// Command to update a review
/// </summary>
public class UpdateReviewCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف التقييم
    /// Review ID
    /// </summary>
    public Guid ReviewId { get; set; }

    /// <summary>
    /// تقييم النظافة المحدث
    /// Updated cleanliness
    /// </summary>
    public int? Cleanliness { get; set; }

    /// <summary>
    /// تقييم الخدمة المحدث
    /// Updated service
    /// </summary>
    public int? Service { get; set; }

    /// <summary>
    /// تقييم الموقع المحدث
    /// Updated location
    /// </summary>
    public int? Location { get; set; }

    /// <summary>
    /// تقييم القيمة المحدث
    /// Updated value
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// تعليق المستخدم المحدث
    /// Updated user comment
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Updated Property ID (if changing association)
    /// </summary>
    public Guid? PropertyId { get; set; }
}