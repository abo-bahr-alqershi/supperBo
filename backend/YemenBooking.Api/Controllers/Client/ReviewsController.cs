using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Reviews;
using YemenBooking.Application.Queries.Reviews;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بتقييمات المستخدم للعميل
    /// Controller for client review operations
    /// </summary>
    public class ReviewsController : BaseClientController
    {
        public ReviewsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء تقييم جديد
        /// Create a new review
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تقييمات الكيان
        /// Get reviews for a property
        /// </summary>
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetReviewsByProperty(Guid propertyId, [FromQuery] GetReviewsByPropertyQuery query)
        {
            query.PropertyId = propertyId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تقييمات المستخدم
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
        /// تحديث التقييم
        /// Update a review
        /// </summary>
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewCommand command)
        {
            command.ReviewId = reviewId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 