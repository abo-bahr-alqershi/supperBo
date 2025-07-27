using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Dashboard;

/// <summary>
/// استعلام للحصول على المراجعات المعلقة للموافقة
/// Query to get pending reviews awaiting approval
/// </summary>
public class GetPendingReviewsQuery : IRequest<ResultDto<IEnumerable<ReviewDto>>>
{
} 