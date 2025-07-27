using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.Queries.Analytics;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Application.Queries.Notifications;
using YemenBooking.Application.Queries.Users;

namespace YemenBooking.Api.Controllers.Property
{
    public class UsersController : BasePropertyController
    {
        public UsersController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// تسجيل صاحب كيان جديد
        /// Register a new property owner with property details
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterPropertyOwner([FromBody] RegisterPropertyOwnerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات المستخدم
        /// Update user profile details
        /// </summary>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserCommand command)
        {
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات المستخدم بواسطة المعرف
        /// Get user details by ID
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب المستخدمين حسب الدور
        /// Get users by role
        /// </summary>
        [HttpGet("by-role")]
        public async Task<IActionResult> GetUsersByRole([FromQuery] GetUsersByRoleQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات المستخدم بواسطة البريد الإلكتروني
        /// Get user details by email
        /// </summary>
        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] GetUserByEmailQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// البحث عن المستخدمين
        /// Search users with filters, sorting, and pagination
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersQuery query)
        {
            var result = await _mediator.Send(query);
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