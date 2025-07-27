using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.PropertyTypes;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بعرض أنواع الكيانات لأصحاب الكيانات
    /// Controller for property owners to get property types
    /// </summary>
    public class PropertyTypesController : BasePropertyController
    {
        public PropertyTypesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب جميع أنواع الكيانات للعميل
        /// Get all property types for client use
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllPropertyTypes([FromQuery] GetAllPropertyTypesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب نوع كيان بواسطة المعرف
        /// Get a property type by its ID
        /// </summary>
        [HttpGet("{propertyTypeId}")]
        public async Task<IActionResult> GetPropertyTypeById(Guid propertyTypeId)
        {
            var query = new GetPropertyTypeByIdQuery { PropertyTypeId = propertyTypeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 