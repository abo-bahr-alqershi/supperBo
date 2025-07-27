using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Dashboard;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.Queries.PropertyImages;
using YemenBooking.Application.Queries.Units;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بتحديث توفر الوحدات للمدراء
    /// Controller for bulk updating unit availability by admins
    /// </summary>
    public class UnitsController : BaseAdminController
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
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var img in result.Items.Select(i => i.Images).SelectMany(i => i))
            {
                // Ensure absolute Url for the image
                if (!img.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    img.Url = baseUrl + (img.Url.StartsWith("/") ? img.Url : "/" + img.Url);
                {
                    // Ensure absolute Url for the main image
                    if (!img.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        img.Url = baseUrl + (img.Url.StartsWith("/") ? img.Url : "/" + img.Url);
                }
            }
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
        /// جلب الوحدات حسب الكيان
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
        [HttpGet("{unitId}/availability")]
        public async Task<IActionResult> GetUnitAvailability(Guid unitId)
        {
            var query = new GetUnitAvailabilityQuery { UnitId = unitId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات الوحدة للتحرير
        /// Get unit for edit
        /// </summary>
        [HttpGet("{unitId}/for-edit")]
        public async Task<IActionResult> GetUnitForEdit(Guid unitId)
        {
            var query = new GetUnitForEditQuery { UnitId = unitId };
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
        /// جلب صور الوحدة
        /// Get unit images
        /// </summary>
        [HttpGet("{unitId}/images")]
        public async Task<IActionResult> GetUnitImages(Guid unitId)
        {
            var query = new GetUnitImagesQuery { UnitId = unitId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 