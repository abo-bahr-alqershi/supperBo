using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users
{
    /// <summary>
    /// أمر لتحديث قائمة المفضلة للمستخدم بصيغة JSON
    /// Command to update user favorites JSON
    /// </summary>
    public class UpdateUserFavoritesCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة المفضلة للمستخدم بصيغة JSON
        /// User favorites JSON
        /// </summary>
        public string FavoritesJson { get; set; } = "[]";
    }
} 