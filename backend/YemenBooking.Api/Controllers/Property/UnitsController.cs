using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Application.Commands.Dashboard;
using YemenBooking.Application.Queries.PropertyImages;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بإدارة الوحدات لأصحاب الكيانات
    /// Controller for unit management by property owners
    /// </summary>
    public class UnitsController : BasePropertyController
    {
        public UnitsController(IMediator mediator) : base(mediator) { }


        /// <summary>
        /// جلب جميع الوحدات مع الصفحات والفلاتر
        /// Get all units with pagination and filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUnits([FromQuery] SearchUnitsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات وحدة بواسطة المعرف
        /// Get unit details by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(Guid id)
        {
            var query = new GetUnitByIdQuery { UnitId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الوحدات الخاصة بكيان معين
        /// Get units by property ID
        /// </summary>
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetUnitsByProperty(Guid propertyId, [FromQuery] GetUnitsByPropertyQuery query)
        {
            query.PropertyId = propertyId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء وحدة جديدة
        /// Create a new unit
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromBody] CreateUnitCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء وحدة جديدة مع قيم الحقول الديناميكية
        /// Create a new unit along with dynamic field values
        /// </summary>
        [HttpPost("with-field-values")]
        public async Task<IActionResult> CreateUnitWithFieldValues([FromBody] CreateUnitWithFieldValuesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات وحدة
        /// Update an existing unit
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UpdateUnitCommand command)
        {
            command.UnitId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف وحدة
        /// Delete a unit
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(Guid id)
        {
            var command = new DeleteUnitCommand { UnitId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة توفر الوحدة
        /// Update unit availability status
        /// </summary>
        [HttpPost("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(Guid id, [FromBody] UpdateUnitAvailabilityCommand command)
        {
            command.UnitId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث متعدد لتوفر الوحدات في نطاق زمني
        /// Bulk update unit availability within a date range
        /// </summary>
        [HttpPost("bulk-availability")]
        public async Task<IActionResult> BulkUpdateUnitAvailability([FromBody] BulkUpdateUnitAvailabilityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

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
        /// جلب الوحدات حسب نوع الوحدة
        /// Get units by type
        /// </summary>
        [HttpGet("type/{unitTypeId}")]
        public async Task<IActionResult> GetUnitsByType(Guid unitTypeId, [FromQuery] GetUnitsByTypeQuery query)
        {
            query.UnitTypeId = unitTypeId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب توفر وحدة
        /// Get unit availability
        /// </summary>
        [HttpGet("{id}/availability")]
        public async Task<IActionResult> GetUnitAvailability(Guid id)
        {
            var query = new GetUnitAvailabilityQuery { UnitId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات الوحدة للتحرير
        /// Get unit for edit
        /// </summary>
        [HttpGet("{id}/for-edit")]
        public async Task<IActionResult> GetUnitForEdit(Guid id)
        {
            var query = new GetUnitForEditQuery { UnitId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب صور الوحدة
        /// Get unit images
        /// </summary>
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetUnitImages(Guid id)
        {
            var query = new GetUnitImagesQuery { UnitId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 