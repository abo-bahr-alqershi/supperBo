using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Reports;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم ببلاغات المستخدم للعميل
    /// Controller for client report operations
    /// </summary>
    public class ReportsController : BaseClientController
    {
        public ReportsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء بلاغ جديد
        /// Create a new report
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 