using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.PropertyTypes;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Application.Queries.Units;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بأنواع الوحدات لأصحاب الكيانات
    /// Controller for unit type management by property owners
    /// </summary>
    public class UnitTypesController : BasePropertyController
    {
        public UnitTypesController(IMediator mediator) : base(mediator) { }


        [HttpGet("{unitTypeId}")]
        public async Task<IActionResult> GetUnitTypeById(Guid unitTypeId)
        {
            var query = new GetUnitTypeByIdQuery { UnitTypeId = unitTypeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("property-type/{propertyTypeId}")]
        public async Task<IActionResult> GetUnitTypesByPropertyType(Guid propertyTypeId)
        {
            var query = new GetUnitTypesByPropertyTypeQuery { PropertyTypeId = propertyTypeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        /// <summary>
        /// جلب جميع أنواع الوحدات مع الصفحات
        /// Get all unit types with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUnitTypes([FromQuery] GetAllUnitTypesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
} 