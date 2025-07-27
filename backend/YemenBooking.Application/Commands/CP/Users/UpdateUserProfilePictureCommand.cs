using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Users;

/// <summary>
/// أمر لتحديث صورة ملف المستخدم الشخصي
/// Command to update user profile picture
/// </summary>
public class UpdateUserProfilePictureCommand : IRequest<ResultDto<bool>>
{
    public Guid UserId { get; set; } // معرف المستخدم
    public string ProfileImageUrl { get; set; } = string.Empty; // رابط الصورة الجديدة
}
