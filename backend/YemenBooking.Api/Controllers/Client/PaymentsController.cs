using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Payments;
using YemenBooking.Application.Queries.Payments;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعمليات الدفع للعميل
    /// Controller for payment operations by client
    /// </summary>
    public class PaymentsController : BaseClientController
    {
        public PaymentsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// معالجة الدفع
        /// Process a payment
        /// </summary>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على بيانات دفع بواسطة المعرف
        /// Get payment details by payment ID
        /// </summary>
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(Guid paymentId)
        {
            var query = new GetPaymentByIdQuery { PaymentId = paymentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على المدفوعات حسب الحجز
        /// Get payments by booking with pagination
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetPaymentsByBooking(Guid bookingId, [FromQuery] GetPaymentsByBookingQuery query)
        {
            query.BookingId = bookingId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على المدفوعات حسب الحالة
        /// Get payments by status with pagination
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetPaymentsByStatus([FromQuery] GetPaymentsByStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على مدفوعات المستخدم
        /// Get payments by user with date range and pagination
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUser(Guid userId, [FromQuery] GetPaymentsByUserQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 