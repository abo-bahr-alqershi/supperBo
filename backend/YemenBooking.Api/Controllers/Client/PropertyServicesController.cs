using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.Services;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض خدمات الكيان للعميل
    /// Controller for client to get property services
    /// </summary>
    public class PropertyServicesController : BaseClientController
    {
        public PropertyServicesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب خدمات كيان معين
        /// Get services for a specific property
        /// </summary>
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetPropertyServices(Guid propertyId)
        {
            var query = new GetPropertyServicesQuery { PropertyId = propertyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب خدمة الكيان بحسب المعرف
        /// Get property service by id
        /// </summary>
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetServiceById(Guid serviceId)
        {
            var query = new GetServiceByIdQuery { ServiceId = serviceId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الخدمات حسب النوع
        /// Get services by type
        /// </summary>
        [HttpGet("type/{serviceType}")]
        public async Task<IActionResult> GetServicesByType(string serviceType)
        {
            var query = new GetServicesByTypeQuery { ServiceType = serviceType };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 