using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.MobileApp.UnitTypes;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر أنواع الوحدات للعملاء
    /// Client Unit Types Controller
    /// </summary>
    public class UnitTypesController : BaseClientController
    {
        public UnitTypesController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// الحصول على جميع أنواع الوحدات
        /// Get all unit types
        /// </summary>
        /// <returns>قائمة أنواع الوحدات</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<UnitTypesResponse>>> GetUnitTypes()
        {
            var query = new GetUnitTypesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}