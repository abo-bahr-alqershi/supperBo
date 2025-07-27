using System;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على تقدم ولاء المستخدم
/// Query to get user loyalty progress
/// </summary>
public class GetUserLoyaltyProgressQuery : IRequest<ResultDto<LoyaltyProgressDto>>
{
    /// <summary>
    /// معرف المستخدم
    /// User identifier
    /// </summary>
    public Guid UserId { get; set; }

    public GetUserLoyaltyProgressQuery(Guid userId)
    {
        UserId = userId;
    }
} 