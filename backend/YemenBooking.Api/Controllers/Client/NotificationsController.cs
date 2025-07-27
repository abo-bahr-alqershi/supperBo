using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.Notifications;
using System;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بإشعارات النظام للعملاء
    /// Controller for system notifications by clients
    /// </summary>
    public class NotificationsController : BaseClientController
    {
        public NotificationsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب إشعارات النظام
        /// Get system notifications
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSystemNotifications([FromQuery] GetSystemNotificationsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب إشعارات المستخدم
        /// Get user notifications
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] GetUserNotificationsQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 