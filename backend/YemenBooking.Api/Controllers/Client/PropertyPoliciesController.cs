using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.Policies;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض سياسات الكيانات للعميل
    /// Controller for client to get property policies
    /// </summary>
    public class PropertyPoliciesController : BaseClientController
    {
        public PropertyPoliciesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب سياسات الكيانات
        /// Get property policies with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPropertyPolicies([FromQuery] GetPropertyPoliciesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب سياسة بواسطة المعرف
        /// Get a policy by its ID
        /// </summary>
        [HttpGet("{policyId}")]
        public async Task<IActionResult> GetPolicyById(Guid policyId)
        {
            var query = new GetPolicyByIdQuery { PolicyId = policyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب السياسات حسب النوع
        /// Get policies by type
        /// </summary>
        [HttpGet("type/{policyType}")]
        public async Task<IActionResult> GetPoliciesByType(string policyType)
        {
            var query = new GetPoliciesByTypeQuery { PolicyType = policyType };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 