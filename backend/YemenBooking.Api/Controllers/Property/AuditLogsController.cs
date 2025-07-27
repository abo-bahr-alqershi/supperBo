using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.AuditLog;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بسجلات نشاط العملاء ومالكي الكيانات
    /// Controller for customer and property activity logs
    /// </summary>
    public class AuditLogsController : BasePropertyController
    {
        public AuditLogsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب سجلات نشاطات العملاء
        /// Get latest activity logs for customers
        /// </summary>
        [HttpGet("customer-activity-logs")]
        public async Task<IActionResult> GetCustomerActivityLogs([FromQuery] GetCustomerActivityLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب سجلات نشاطات المالكين والموظفين
        /// Get latest activity logs for property owners and staff
        /// </summary>
        [HttpGet("property-activity-logs")]
        public async Task<IActionResult> GetPropertyActivityLogs([FromQuery] GetPropertyActivityLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 