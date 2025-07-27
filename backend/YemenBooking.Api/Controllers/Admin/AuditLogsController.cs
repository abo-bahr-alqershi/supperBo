using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.AuditLog;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بسجلات نشاط وتدقيق النظام للمدراء
    /// Controller for admin activity and audit logs
    /// </summary>
    public class AuditLogsController : BaseAdminController
    {
        public AuditLogsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب سجلات نشاطات المدراء (مرقمة)
        /// Get paginated admin activity logs
        /// </summary>
        [HttpGet("activity-logs")]
        public async Task<IActionResult> GetAdminActivityLogs([FromQuery] GetAdminActivityLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب سجلات التدقيق مع الفلاتر
        /// Get audit logs with optional filters (user, date range, search term, operation type)
        /// </summary>
        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

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