using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.PropertyTypes;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Application.Queries.Units;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بأنواع الوحدات للمدراء
    /// Controller for unit type management by admins
    /// </summary>
    public class UnitTypesController : BaseAdminController
    {
        public UnitTypesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء نوع وحدة جديد
        /// Create a new unit type
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUnitType([FromBody] CreateUnitTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث نوع وحدة
        /// Update an existing unit type
        /// </summary>
        [HttpPut("{unitTypeId}")]
        public async Task<IActionResult> UpdateUnitType(Guid unitTypeId, [FromBody] UpdateUnitTypeCommand command)
        {
            command.UnitTypeId = unitTypeId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف نوع وحدة
        /// Delete a unit type
        /// </summary>
        [HttpDelete("{unitTypeId}")]
        public async Task<IActionResult> DeleteUnitType(Guid unitTypeId)
        {
            var command = new DeleteUnitTypeCommand { UnitTypeId = unitTypeId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

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