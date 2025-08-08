using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.MobileApp.Properties;
using YemenBooking.Application.Queries.MobileApp.Properties;
using YemenBooking.Application.Queries.Policies;
using YemenBooking.Application.DTOs;
using System.Collections.Generic;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر إدارة العقارات للعملاء
    /// Client Properties Management Controller
    /// </summary>
    public class PropertiesController : BaseClientController
    {
        public PropertiesController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// إضافة عقار لقائمة الرغبات للعميل
        /// Add property to client's wishlist
        /// </summary>
        /// <param name="command">بيانات إضافة للرغبات</param>
        /// <returns>نتيجة الإضافة</returns>
        [HttpPost("wishlist")]
        public async Task<ActionResult<ResultDto<bool>>> AddToWishlist([FromBody] ClientAddPropertyToWishlistCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث عداد المشاهدات للعقار
        /// Update property view count
        /// </summary>
        /// <param name="command">بيانات العقار</param>
        /// <returns>نتيجة التحديث</returns>
        [HttpPost("view-count")]
        public async Task<ActionResult<ResultDto<bool>>> UpdateViewCount([FromBody] ClientUpdatePropertyViewCountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// البحث في العقارات
        /// Search properties
        /// </summary>
        /// <param name="query">معايير البحث</param>
        /// <returns>نتائج البحث</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<SearchPropertiesResponse>>> SearchProperties([FromQuery] SearchPropertiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تفاصيل عقار محدد
        /// Get specific property details
        /// </summary>
        /// <param name="id">معرف العقار</param>
        /// <param name="userId">معرف المستخدم (اختياري)</param>
        /// <returns>تفاصيل العقار</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<YemenBooking.Application.DTOs.Properties.PropertyDetailsDto>>> GetPropertyDetails(Guid id, [FromQuery] Guid? userId = null)
        {
            var query = new GetPropertyDetailsQuery { PropertyId = id, UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على سياسات العقار
        /// Get property policies
        /// </summary>
        [HttpGet("{id}/policies")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<IEnumerable<PolicyDto>>>> GetPropertyPolicies(Guid id)
        {
            var query = new GetPropertyPoliciesQuery { PropertyId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على العقارات القريبة
        /// Get nearby properties
        /// </summary>
        /// <param name="query">معايير الموقع</param>
        /// <returns>قائمة العقارات القريبة</returns>
        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<List<YemenBooking.Application.DTOs.Properties.NearbyPropertyDto>>>> GetNearbyProperties([FromQuery] GetNearbyPropertiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// التحقق من توفر العقار
        /// Check property availability
        /// </summary>
        /// <param name="query">معايير التحقق</param>
        /// <returns>حالة التوفر</returns>
        [HttpGet("availability")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<PropertyAvailabilityResponse>>> CheckAvailability([FromQuery] CheckPropertyAvailabilityQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
