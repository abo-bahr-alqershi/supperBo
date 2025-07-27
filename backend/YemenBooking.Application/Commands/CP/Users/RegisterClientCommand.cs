using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users
{
    /// <summary>
    /// أمر لتسجيل عميل جديد
    /// Command to register a new client user
    /// </summary>
    public class RegisterClientCommand : IRequest<ResultDto<Guid>>
    {
        /// <summary>
        /// اسم العميل
        /// Client name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// بريد العميل الإلكتروني
        /// Client email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// كلمة مرور العميل
        /// Client password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// رقم هاتف العميل
        /// Client phone number
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// رابط صورة الملف الشخصي (اختياري)
        /// Profile image URL (optional)
        /// </summary>
        public string? ProfileImage { get; set; }
    }
} 