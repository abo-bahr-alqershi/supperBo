using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.Users
{
    /// <summary>
    /// استعلام للحصول على بيانات المستخدم بواسطة البريد الإلكتروني
    /// Query to get user details by email
    /// </summary>
    public class GetUserByEmailQuery : IRequest<ResultDto<UserDto>>
    {
        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; }
    }
} 