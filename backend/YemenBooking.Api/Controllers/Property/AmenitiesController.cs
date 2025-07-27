using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Amenities;
using YemenBooking.Application.Queries.Amenities;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بإدارة المرافق لأصحاب الكيانات
    /// Controller for managing amenities by property owners
    /// </summary>
    public class AmenitiesController : BasePropertyController
    {
        public AmenitiesController(IMediator mediator) : base(mediator) { }

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
        /// جلب المرافق لكيان معين
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
        /// جلب المرافق حسب نوع الكيان
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
        [HttpPost("{amenityId}/assign/{propertyId}")]
        public async Task<IActionResult> AssignAmenityToProperty(Guid amenityId, Guid propertyId, [FromBody] AssignAmenityToPropertyCommand command)
        {
            command.AmenityId = amenityId;
            command.PropertyId = propertyId;
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