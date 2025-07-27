using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Dashboard;
using YemenBooking.Application.Commands.Reviews;
using YemenBooking.Application.Queries.Reviews;
using YemenBooking.Application.Queries.Dashboard;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بالتقييمات للمدراء
    /// Controller for managing reviews by admins
    /// </summary>
    public class ReviewsController : BaseAdminController
    {
        public ReviewsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب جميع التقييمات مع دعم التصفية
        /// Get all reviews with filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllReviews([FromQuery] GetAllReviewsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

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

        /// <summary>
        /// استعلام للحصول على المراجعات المعلقة للموافقة
        /// Query to get pending reviews awaiting approval
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingReviews([FromQuery] GetPendingReviewsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 