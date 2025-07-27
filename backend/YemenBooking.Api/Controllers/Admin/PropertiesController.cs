using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Properties;
using YemenBooking.Application.Queries.Properties;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بالكيانات للمدراء
    /// Controller for managing properties by admins
    /// </summary>
    public class PropertiesController : BaseAdminController
    {
        public PropertiesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء كيان جديد
        /// Create a new property
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات كيان
        /// Update an existing property
        /// </summary>
        [HttpPut("{propertyId}")]
        public async Task<IActionResult> UpdateProperty(Guid propertyId, [FromBody] UpdatePropertyCommand command)
        {
            command.PropertyId = propertyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف كيان
        /// Delete a property
        /// </summary>
        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> DeleteProperty(Guid propertyId)
        {
            var command = new DeletePropertyCommand { PropertyId = propertyId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الموافقة على الكيان
        /// Approve a property
        /// </summary>
        [HttpPost("{propertyId}/approve")]
        public async Task<IActionResult> ApproveProperty(Guid propertyId, [FromBody] ApprovePropertyCommand command)
        {
            command.PropertyId = propertyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// رفض الكيان
        /// Reject a property
        /// </summary>
        [HttpPost("{propertyId}/reject")]
        public async Task<IActionResult> RejectProperty(Guid propertyId, [FromBody] RejectPropertyCommand command)
        {
            command.PropertyId = propertyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب جميع الكيانات مع الفلاتر، الفرز، والتصفّح
        /// Get all properties with filters, sorting, and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProperties([FromQuery] GetAllPropertiesQuery query)
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
            /// جلب الكيانات في انتظار الموافقة
            /// Get pending properties
            /// </summary>
            [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProperties([FromQuery] GetPendingPropertiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات كيان بواسطة المعرف
        /// Get property data by ID
        /// </summary>
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetPropertyById(Guid propertyId)
        {
            var query = new GetPropertyByIdQuery { PropertyId = propertyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب تفاصيل الكيان مع الحقول الديناميكية
        /// Get property details including dynamic fields
        /// </summary>
        [HttpGet("{propertyId}/details")]
        public async Task<IActionResult> GetPropertyDetails(Guid propertyId, [FromQuery] bool includeUnits = true, [FromQuery] bool includeDynamicFields = true)
        {
            var query = new GetPropertyDetailsQuery { PropertyId = propertyId, IncludeUnits = includeUnits, IncludeDynamicFields = includeDynamicFields };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات الكيان للتحرير
        /// Get property data for edit form
        /// </summary>
        [HttpGet("{propertyId}/for-edit")]
        public async Task<IActionResult> GetPropertyForEdit(Guid propertyId, [FromQuery] Guid ownerId)
        {
            var query = new GetPropertyForEditQuery { PropertyId = propertyId, OwnerId = ownerId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام للحصول على مرافق الكيان
        /// Query to get property amenities
        /// </summary>
        [HttpGet("{propertyId}/amenities")]
        public async Task<IActionResult> GetPropertyAmenities(Guid propertyId, [FromQuery] GetPropertyAmenitiesQuery query)
        {
            query.PropertyId = propertyId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الكيانات حسب المدينة
        /// Get properties by city
        /// </summary>
        [HttpGet("by-city")]
        public async Task<IActionResult> GetPropertiesByCity([FromQuery] GetPropertiesByCityQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على كيانات المالك
        /// Get properties by owner
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetPropertiesByOwner(Guid ownerId, [FromQuery] GetPropertiesByOwnerQuery query)
        {
            query.OwnerId = ownerId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الكيانات حسب النوع
        /// Get properties by type
        /// </summary>
        [HttpGet("type/{propertyTypeId}")]
        public async Task<IActionResult> GetPropertiesByType(Guid propertyTypeId, [FromQuery] GetPropertiesByTypeQuery query)
        {
            query.PropertyTypeId = propertyTypeId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 