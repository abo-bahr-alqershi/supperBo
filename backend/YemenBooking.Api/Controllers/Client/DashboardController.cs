using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.Dashboard;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم ببيانات لوحة تحكم العميل
    /// Controller for customer dashboard operations
    /// </summary>
    public class DashboardController : BaseClientController
    {
        public DashboardController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// استعلام عن بيانات لوحة التحكم للعميل
        /// Get customer dashboard data for the given customer
        /// </summary>
        [HttpGet("dashboard/{customerId}")]
        [Authorize(Roles = "Client")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCustomerDashboard(Guid customerId)
        {
            var query = new GetCustomerDashboardQuery(customerId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 