using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Staffs
{
    /// <summary>
    /// أمر لإضافة موظف جديد
    /// Command to add a new staff member
    /// </summary>
    public class AddStaffCommand : IRequest<ResultDto<Guid>>
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// معرف الكيان
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// المنصب
        /// </summary>
        public StaffPosition Position { get; set; }

        /// <summary>
        /// الصلاحيات
        /// </summary>
        public string Permissions { get; set; }
    }
} 