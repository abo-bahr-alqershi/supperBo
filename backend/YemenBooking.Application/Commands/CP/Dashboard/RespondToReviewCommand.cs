using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Dashboard
{
    /// <summary>
    /// الأمر للرد على مراجعة من قبل المالك
    /// Command to respond to a review by owner
    /// </summary>
    public class RespondToReviewCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف المراجعة
        /// Review identifier
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// نص الرد
        /// Response text
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// معرف المالك
        /// Owner identifier
        /// </summary>
        public Guid OwnerId { get; set; }

        public RespondToReviewCommand(Guid reviewId, string responseText, Guid ownerId)
        {
            ReviewId = reviewId;
            ResponseText = responseText;
            OwnerId = ownerId;
        }
    }
} 