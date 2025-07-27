using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.PropertyTypes;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض أنواع الكيانات للعميل
    /// Controller for client to get property types
    /// </summary>
    public class PropertyTypesController : BaseClientController
    {
        public PropertyTypesController(IMediator mediator) : base(mediator) { }

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