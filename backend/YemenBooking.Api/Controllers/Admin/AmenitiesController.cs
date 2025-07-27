using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Amenities;
using YemenBooking.Application.Queries.Amenities;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بإدارة المرافق للمدراء
    /// Controller for managing amenities by admins
    /// </summary>
    public class AmenitiesController : BaseAdminController
    {
        public AmenitiesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء مرفق جديد
        /// Create a new amenity
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAmenity([FromBody] CreateAmenityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث مرفق موجود
        /// Update an existing amenity
        /// </summary>
        [HttpPut("{amenityId}")]
        public async Task<IActionResult> UpdateAmenity(Guid amenityId, [FromBody] UpdateAmenityCommand command)
        {
            command.AmenityId = amenityId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف مرفق
        /// Delete an amenity
        /// </summary>
        [HttpDelete("{amenityId}")]
        public async Task<IActionResult> DeleteAmenity(Guid amenityId)
        {
            var command = new DeleteAmenityCommand { AmenityId = amenityId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب جميع المرافق مع الصفحات
        /// Get all amenities with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAmenities([FromQuery] GetAllAmenitiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب مرافق بناءً على معرف الكيان
        /// Get amenities by property ID
        /// </summary>
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetAmenitiesByProperty(Guid propertyId, [FromQuery] GetAmenitiesByPropertyQuery query)
        {
            query.PropertyId = propertyId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب مرافق بناءً على نوع الكيان
        /// Get amenities by property type ID
        /// </summary>
        [HttpGet("type/{propertyTypeId}")]
        public async Task<IActionResult> GetAmenitiesByPropertyType(Guid propertyTypeId, [FromQuery] GetAmenitiesByPropertyTypeQuery query)
        {
            query.PropertyTypeId = propertyTypeId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// إسناد مرفق لكيان
        /// Assign an amenity to a property
        /// </summary>
        [HttpPost("{amenityId}/assign/property/{propertyId}")]
        public async Task<IActionResult> AssignAmenityToProperty(Guid amenityId, Guid propertyId, [FromBody] AssignAmenityToPropertyCommand command)
        {
            command.AmenityId = amenityId;
            command.PropertyId = propertyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تخصيص مرفق لنوع الكيان
        /// Assign an amenity to a property type
        /// </summary>
        [HttpPost("{amenityId}/assign/property-type/{propertyTypeId}")]
        public async Task<IActionResult> AssignAmenityToPropertyType(Guid amenityId, Guid propertyTypeId, [FromBody] AssignAmenityToPropertyTypeCommand command)
        {
            command.AmenityId = amenityId;
            command.PropertyTypeId = propertyTypeId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة وتكلفة المرفق لكيان
        /// Update amenity availability and extra cost for a property
        /// </summary>
        [HttpPut("{amenityId}/update/property/{propertyId}")]
        public async Task<IActionResult> UpdatePropertyAmenity(Guid amenityId, Guid propertyId, [FromBody] UpdatePropertyAmenityCommand command)
        {
            command.AmenityId = amenityId;
            command.PropertyId = propertyId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 