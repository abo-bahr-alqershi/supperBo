using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.MobileApp.Units;
using YemenBooking.Application.Queries.MobileApp.Units;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر إدارة الوحدات للعملاء
    /// Client Units Management Controller
    /// </summary>
    public class UnitsController : BaseClientController
    {
        public UnitsController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// التحقق من توفر وحدة للعميل
        /// Check unit availability for client
        /// </summary>
        /// <param name="command">بيانات التحقق</param>
        /// <returns>حالة التوفر</returns>
        [HttpPost("check-availability")]
        public async Task<ActionResult<ResultDto<ClientCheckUnitAvailabilityResponse>>> CheckUnitAvailability([FromBody] ClientCheckUnitAvailabilityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حجز وحدة للعميل
        /// Reserve unit for client
        /// </summary>
        /// <param name="command">بيانات الحجز</param>
        /// <returns>نتيجة الحجز</returns>
        [HttpPost("reserve")]
        public async Task<ActionResult<ResultDto<ClientReserveUnitResponse>>> ReserveUnit([FromBody] ClientReserveUnitCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الوحدات المتاحة
        /// Get available units
        /// </summary>
        /// <param name="query">معايير البحث</param>
        /// <returns>قائمة الوحدات المتاحة</returns>
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<AvailableUnitsResponse>>> GetAvailableUnits([FromQuery] GetAvailableUnitsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تفاصيل وحدة محددة
        /// Get specific unit details
        /// </summary>
        /// <param name="id">معرف الوحدة</param>
        /// <returns>تفاصيل الوحدة</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<UnitDetailsResponse>>> GetUnitDetails(Guid id)
        {
            var query = new GetUnitDetailsQuery { UnitId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}