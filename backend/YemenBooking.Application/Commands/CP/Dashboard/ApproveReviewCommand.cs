using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Dashboard
{
    /// <summary>
    /// الأمر للموافقة على مراجعة
    /// Command to approve a review
    /// </summary>
    public class ApproveReviewCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف المراجعة
        /// Review identifier
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// معرف المسؤول
        /// Admin identifier
        /// </summary>
        public Guid AdminId { get; set; }

        public ApproveReviewCommand(Guid reviewId, Guid adminId)
        {
            ReviewId = reviewId;
            AdminId = adminId;
        }
    }
} 