using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Dashboard;
using YemenBooking.Application.Commands.Reviews;
using YemenBooking.Application.Queries.Reviews;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بالتقييمات لأصحاب الكيانات
    /// Controller for property owner review operations
    /// </summary>
    public class ReviewsController : BasePropertyController
    {
        public ReviewsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// الموافقة على تقييم
        /// Approve a review
        /// </summary>
        [HttpPost("{reviewId}/approve")]
        public async Task<IActionResult> ApproveReview(Guid reviewId, [FromBody] ApproveReviewCommand command)
        {
            command.ReviewId = reviewId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف تقييم
        /// Delete a review
        /// </summary>
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            var command = new DeleteReviewCommand { ReviewId = reviewId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الرد على تقييم
        /// Respond to a review
        /// </summary>
        [HttpPost("{reviewId}/respond")]
        public async Task<IActionResult> RespondToReview(Guid reviewId, [FromBody] RespondToReviewCommand command)
        {
            command.ReviewId = reviewId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تقييم الحجز
        /// Get review by booking
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetReviewByBooking(Guid bookingId)
        {
            var query = new GetReviewByBookingQuery { BookingId = bookingId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تقييمات لكيان معين
        /// Get reviews by property
        /// </summary>
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetReviewsByProperty(Guid propertyId, [FromQuery] GetReviewsByPropertyQuery query)
        {
            query.PropertyId = propertyId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تقييمات مستخدم
        /// Get reviews by user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewsByUser(Guid userId, [FromQuery] GetReviewsByUserQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 