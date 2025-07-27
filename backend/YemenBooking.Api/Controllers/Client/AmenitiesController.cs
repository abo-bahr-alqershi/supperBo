using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.Amenities;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض المرافق للعملاء
    /// Controller for clients to view property amenities
    /// </summary>
    public class AmenitiesController : BaseClientController
    {
        public AmenitiesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب مرافق بناءً على معرف الكيان
        /// Get amenities by property
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
        /// Get amenities by property type
        /// </summary>
        [HttpGet("type/{propertyTypeId}")]
        public async Task<IActionResult> GetAmenitiesByPropertyType(Guid propertyTypeId, [FromQuery] GetAmenitiesByPropertyTypeQuery query)
        {
            query.PropertyTypeId = propertyTypeId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 