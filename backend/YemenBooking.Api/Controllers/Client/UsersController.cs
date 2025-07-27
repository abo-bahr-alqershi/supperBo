using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.Queries.Analytics;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Application.Queries.Notifications;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم ببيانات المستخدم للعميل
    /// Controller for client user operations
    /// </summary>
    public class UsersController : BaseClientController
    {
        public UsersController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// تسجيل عميل جديد
        /// Register a new client user
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterClient([FromBody] RegisterClientCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث قائمة المفضلة للمستخدم
        /// Update user favorites JSON
        /// </summary>
        [HttpPut("favorites")]
        public async Task<IActionResult> UpdateFavorites([FromBody] UpdateUserFavoritesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على تقدم الولاء للمستخدم
        /// Get user loyalty progress
        /// </summary>
        [HttpGet("loyalty/{userId}")]
        public async Task<IActionResult> GetLoyaltyProgress(Guid userId)
        {
            var query = new GetUserLoyaltyProgressQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات المستخدم
        /// Update an existing user
        /// </summary>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserCommand command)
        {
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// استعلام سجلات نشاط المستخدم
        /// Get user activity logs
        /// </summary>
        [HttpGet("{userId}/activity-log")]
        public async Task<IActionResult> GetUserActivityLog(Guid userId, [FromQuery] GetUserActivityLogQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام إحصائيات المستخدم مدى الحياة
        /// Get user lifetime statistics
        /// </summary>
        [HttpGet("{userId}/lifetime-stats")]
        public async Task<IActionResult> GetUserLifetimeStats(Guid userId)
        {
            var query = new GetUserLifetimeStatsQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استعلام إشعارات المستخدم
        /// Get user notifications
        /// </summary>
        [HttpGet("{userId}/notifications")]
        public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] GetUserNotificationsQuery query)
        {
            query.UserId = userId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 