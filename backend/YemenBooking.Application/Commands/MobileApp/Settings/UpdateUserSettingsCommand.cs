using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Commands.MobileApp.Settings
{
    /// <summary>
    /// أمر تحديث إعدادات المستخدم
    /// Command to update user settings
    /// </summary>
    public class UpdateUserSettingsCommand : IRequest<ResultDto<UpdateUserSettingsResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// اللغة المفضلة (ar, en)
    /// </summary>
    public string PreferredLanguage { get; set; } = "ar";
    
    /// <summary>
    /// العملة المفضلة
    /// </summary>
    public string PreferredCurrency { get; set; } = "YER";
    
    /// <summary>
    /// المنطقة الزمنية
    /// </summary>
    public string TimeZone { get; set; } = "Asia/Aden";
    
    /// <summary>
    /// تفعيل الوضع الليلي
    /// </summary>
    public bool DarkMode { get; set; } = false;
    
    /// <summary>
    /// إعدادات إضافية كـ JSON
    /// </summary>
    public string? AdditionalSettingsJson { get; set; }
}

/// <summary>
/// استجابة تحديث إعدادات المستخدم
/// </summary>
public class UpdateUserSettingsResponse
{
    /// <summary>
    /// نجاح العملية
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// رسالة النتيجة
    /// </summary>
    public string Message { get; set; } = string.Empty;
    }
}