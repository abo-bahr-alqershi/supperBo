using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.PropertyImages;
using YemenBooking.Application.Queries.PropertyImages;
using YemenBooking.Application.Queries.Dashboard;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بصور الكيانات للمالكين
    /// Controller for managing property images by property owners
    /// </summary>
    public class PropertyImagesController : BasePropertyController
    {
        public PropertyImagesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء صورة كيان جديدة
        /// Create a new property image
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePropertyImage([FromBody] CreatePropertyImageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات صورة الكيان
        /// Update an existing property image
        /// </summary>
        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdatePropertyImage(Guid imageId, [FromBody] UpdatePropertyImageCommand command)
        {
            command.ImageId = imageId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف صورة كيان
        /// Delete a property image
        /// </summary>
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeletePropertyImage(Guid imageId)
        {
            var command = new DeletePropertyImageCommand { ImageId = imageId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إسناد الصورة للكيان
        /// Assign an image to a property
        /// </summary>
        [HttpPost("{imageId}/assign/property/{propertyId}")]
        public async Task<IActionResult> AssignPropertyImageToProperty(Guid imageId, Guid propertyId, [FromBody] AssignPropertyImageToPropertyCommand command)
        {
            command.ImageId = imageId;
            command.PropertyId = propertyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إسناد الصورة للوحدة
        /// Assign an image to a unit
        /// </summary>
        [HttpPost("{imageId}/assign/unit/{unitId}")]
        public async Task<IActionResult> AssignPropertyImageToUnit(Guid imageId, Guid unitId, [FromBody] AssignPropertyImageToUnitCommand command)
        {
            command.ImageId = imageId;
            command.UnitId = unitId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

                /// <summary>
        /// تعيين صور متعددة لكيانات
        /// Bulk assign images to properties
        /// </summary>
        [HttpPost("bulk-assign/property")]
        public async Task<IActionResult> BulkAssignImagesToProperties([FromBody] BulkAssignImageToPropertyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تعيين صور متعددة لوحدات
        /// Bulk assign images to units
        /// </summary>
        [HttpPost("bulk-assign/unit")]
        public async Task<IActionResult> BulkAssignImagesToUnits([FromBody] BulkAssignImageToUnitCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// جلب جميع صور الكيان
        /// Get all property images
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPropertyImages([FromQuery] GetPropertyImagesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب إحصائيات صور الكيان
        /// Get property image statistics for a specific property
        /// </summary>
        [HttpGet("{propertyId}/stats")]
        public async Task<IActionResult> GetPropertyImageStats(Guid propertyId)
        {
            var query = new GetPropertyImageStatsQuery { PropertyId = propertyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

                /// <summary>
        /// إعادة ترتيب صور الكيان
        /// Reorder property images display order
        /// </summary>
        [HttpPut("order")]
        public async Task<IActionResult> ReorderPropertyImages([FromBody] ReorderPropertyImagesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
} 