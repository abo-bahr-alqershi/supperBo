using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Application.Queries.Units;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض أنواع الوحدات للعميل
    /// Controller for client to get unit types
    /// </summary>
    public class UnitTypesController : BaseClientController
    {
        public UnitTypesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب نوع وحدة بواسطة المعرف
        /// Get unit type by ID
        /// </summary>
        [HttpGet("{unitTypeId}")]
        public async Task<IActionResult> GetUnitTypeById(Guid unitTypeId)
        {
            var query = new GetUnitTypeByIdQuery { UnitTypeId = unitTypeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب أنواع الوحدات حسب نوع الكيان
        /// Get unit types by property type
        /// </summary>
        [HttpGet("property-type/{propertyTypeId}")]
        public async Task<IActionResult> GetUnitTypesByPropertyType(Guid propertyTypeId)
        {
            var query = new GetUnitTypesByPropertyTypeQuery { PropertyTypeId = propertyTypeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 