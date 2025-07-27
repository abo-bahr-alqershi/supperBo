using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.PropertyImages;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Application.Queries.Units;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض الوحدات للعميل
    /// Controller for unit availability and details for clients
    /// </summary>
    public class UnitsController : BaseClientController
    {
        public UnitsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// الحصول على الوحدات المتاحة
        /// Get available units for a property
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableUnits([FromQuery] GetAvailableUnitsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على بيانات الوحدة
        /// Get unit by ID
        /// </summary>
        [HttpGet("{unitId}")]
        public async Task<IActionResult> GetUnitById(Guid unitId)
        {
            var query = new GetUnitByIdQuery { UnitId = unitId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تفاصيل الوحدة
        /// Get unit details including dynamic fields
        /// </summary>
        [HttpGet("{unitId}/details")]
        public async Task<IActionResult> GetUnitDetails(Guid unitId, [FromQuery] bool includeDynamicFields = true)
        {
            var query = new GetUnitDetailsQuery { UnitId = unitId, IncludeDynamicFields = includeDynamicFields };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على صور الوحدة
        /// Get unit images
        /// </summary>
        [HttpGet("{unitId}/images")]
        public async Task<IActionResult> GetUnitImages(Guid unitId)
        {
            var query = new GetUnitImagesQuery { UnitId = unitId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على سعر الوحدة
        /// Get unit price
        /// </summary>
        [HttpGet("price")]
        public async Task<IActionResult> GetUnitPrice([FromQuery] GetUnitPriceQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على أنواع الوحدات لنوع الكيان معين
        /// Get unit types by property type
        /// </summary>
        [HttpGet("types")]
        public async Task<IActionResult> GetUnitTypesByPropertyType([FromQuery] GetUnitTypesByPropertyTypeQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الوحدات حسب الكيان
        /// Get units by property
        /// </summary>
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetUnitsByProperty(Guid propertyId, [FromQuery] GetUnitsByPropertyQuery query)
        {
            query.PropertyId = propertyId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب توفر وحدة
        /// Get unit availability
        /// </summary>
        [HttpGet("{unitId}/availability")]
        public async Task<IActionResult> GetUnitAvailability(Guid unitId)
        {
            var query = new GetUnitAvailabilityQuery { UnitId = unitId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الوحدات حسب نوع الوحدة
        /// Get units by type
        /// </summary>
        [HttpGet("types/{unitTypeId}")]
        public async Task<IActionResult> GetUnitsByType(Guid unitTypeId, [FromQuery] GetUnitsByTypeQuery query)
        {
            query.UnitTypeId = unitTypeId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 