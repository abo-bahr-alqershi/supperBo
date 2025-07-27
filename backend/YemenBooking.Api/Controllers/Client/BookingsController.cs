using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.Queries.Bookings;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعمليات الحجز للعميل
    /// Controller for client booking operations
    /// </summary>
    public class BookingsController : BaseClientController
    {
        public BookingsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء حجز جديد
        /// Create a new booking
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات الحجز
        /// Update booking information
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء الحجز
        /// Cancel a booking
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الحجوزات الخاصة بالمستخدم
        /// Get bookings by user with filters and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBookingsByUser([FromQuery] GetBookingsByUserQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على حجز بواسطة المعرف
        /// Get a booking by ID
        /// </summary>
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(Guid bookingId)
        {
            var query = new GetBookingByIdQuery { BookingId = bookingId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الحجوزات حسب الحالة
        /// Get bookings by status with pagination
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetBookingsByStatus([FromQuery] GetBookingsByStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على خدمات الحجز
        /// Get services added to a booking
        /// </summary>
        [HttpGet("{bookingId}/services")]
        public async Task<IActionResult> GetBookingServices(Guid bookingId)
        {
            var query = new GetBookingServicesQuery { BookingId = bookingId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// إضافة خدمة للحجز
        /// Add service to a booking
        /// </summary>
        [HttpPost("add-service-to-booking")]
        public async Task<IActionResult> AddServiceToBooking([FromBody] AddServiceToBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إزالة خدمة من الحجز
        /// Remove service from a booking
        /// </summary>
        [HttpPost("remove-service-from-booking")]
        public async Task<IActionResult> RemoveServiceFromBooking([FromBody] RemoveServiceFromBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 