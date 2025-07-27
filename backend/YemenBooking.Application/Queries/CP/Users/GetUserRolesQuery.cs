using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.Users
{
    /// <summary>
    /// استعلام للحصول على أدوار المستخدم
    /// Query to get user roles
    /// </summary>
    public class GetUserRolesQuery : IRequest<PaginatedResult<RoleDto>>
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// رقم الصفحة
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
} 