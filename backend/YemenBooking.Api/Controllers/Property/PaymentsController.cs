using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Payments;
using YemenBooking.Application.Queries.Payments;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بعمليات الدفع لأصحاب الكيانات
    /// Controller for payment operations by property owners
    /// </summary>
    public class PaymentsController : BasePropertyController
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
        /// استرجاع الدفع
        /// Refund a payment
        /// </summary>
        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء الدفع
        /// Void a payment
        /// </summary>
        [HttpPost("void")]
        public async Task<IActionResult> VoidPayment([FromBody] VoidPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة الدفع
        /// Update payment status
        /// </summary>
        [HttpPut("{paymentId}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid paymentId, [FromBody] UpdatePaymentStatusCommand command)
        {
            command.PaymentId = paymentId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على دفعة بواسطة المعرف
        /// Get payment details by ID
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
        /// Get payments by booking
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
        /// Get payments by status
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetPaymentsByStatus([FromQuery] GetPaymentsByStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على المدفوعات حسب المستخدم
        /// Get payments by user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUser(Guid userId, [FromQuery] GetPaymentsByUserQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على المدفوعات حسب طريقة الدفع
        /// Get payments by method
        /// </summary>
        [HttpGet("method/{paymentMethod}")]
        public async Task<IActionResult> GetPaymentsByMethod(string paymentMethod, [FromQuery] GetPaymentsByMethodQuery query)
        {
            query.PaymentMethod = paymentMethod;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 