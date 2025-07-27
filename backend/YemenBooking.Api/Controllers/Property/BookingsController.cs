using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.Queries.Analytics;
using YemenBooking.Application.Queries.Bookings;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Application.Queries.ReportsAnalytics;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بعمليات الحجز الخاصة بأصحاب الكيانات
    /// Controller for booking operations by property owners
    /// </summary>
    public class BookingsController : BasePropertyController
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
        /// إلغاء حجز
        /// Cancel a booking
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل الوصول للحجز (Check-in)
        /// Check-in for a booking
        /// </summary>
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل المغادرة للحجز (Check-out)
        /// Check-out for a booking
        /// </summary>
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إكمال الحجز (Complete)
        /// Complete a booking
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> CompleteBooking([FromBody] CompleteBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تأكيد الحجز
        /// Confirm a booking
        /// </summary>
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingCommand command)
        {
            var result = await _mediator.Send(command);
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

        /// <summary>
        /// تحديث بيانات الحجز
        /// Update booking information
        /// </summary>
        [HttpPut("{bookingId}/update")]
        public async Task<IActionResult> UpdateBooking(Guid bookingId, [FromBody] UpdateBookingCommand command)
        {
            command.BookingId = bookingId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب حجوزات الكيان مع الفلاتر والفرز والصفحات
        /// Get bookings for a property with filters, sorting, and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBookingsByProperty([FromQuery] GetBookingsByPropertyQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات حجز بواسطة المعرف
        /// Get booking details by ID
        /// </summary>
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(Guid bookingId)
        {
            var query = new GetBookingByIdQuery { BookingId = bookingId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الحجوزات حسب الحالة
        /// Get bookings by status with pagination
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetBookingsByStatus([FromQuery] GetBookingsByStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الحجوزات حسب الوحدة
        /// Get bookings by unit
        /// </summary>
        [HttpGet("unit/{unitId}")]
        public async Task<IActionResult> GetBookingsByUnit(Guid unitId, [FromQuery] GetBookingsByUnitQuery query)
        {
            query.UnitId = unitId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الحجوزات حسب المستخدم
        /// Get bookings by user with filters and pagination
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUser(Guid userId, [FromQuery] GetBookingsByUserQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب خدمات الحجز
        /// Get services for a booking
        /// </summary>
        [HttpGet("{bookingId}/services")]
        public async Task<IActionResult> GetBookingServices(Guid bookingId)
        {
            var query = new GetBookingServicesQuery { BookingId = bookingId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام تقرير الحجوزات
        /// Get booking report
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetBookingReport([FromQuery] GetBookingReportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام اتجاهات الحجوزات كسلسلة زمنية
        /// Get booking trends as time series
        /// </summary>
        [HttpGet("trends")]
        public async Task<IActionResult> GetBookingTrends([FromQuery] GetBookingTrendsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام تحليل نافذة الحجز لكيان معين
        /// Get booking window analysis for a specific property
        /// </summary>
        [HttpGet("window-analysis/{propertyId}")]
        public async Task<IActionResult> GetBookingWindowAnalysis(Guid propertyId)
        {
            var query = new GetBookingWindowAnalysisQuery(propertyId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام الحجوزات في نطاق زمني
        /// Get bookings by date range
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetBookingsByDateRange([FromQuery] GetBookingsByDateRangeQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 