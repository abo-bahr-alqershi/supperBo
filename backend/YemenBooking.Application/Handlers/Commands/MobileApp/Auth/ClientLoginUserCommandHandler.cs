using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth;

/// <summary>
/// معالج أمر تسجيل دخول المستخدم للعميل
/// Handler for client login user command
/// </summary>
public class ClientLoginUserCommandHandler : IRequestHandler<ClientLoginUserCommand, ResultDto<ClientLoginUserResponse>>
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<ClientLoginUserCommandHandler> _logger;

    public ClientLoginUserCommandHandler(
        IAuthenticationService authService,
        ILogger<ClientLoginUserCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر تسجيل دخول المستخدم
    /// Handle login user command
    /// </summary>
    /// <param name="request">الطلب</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<ClientLoginUserResponse>> Handle(ClientLoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("محاولة تسجيل دخول للمستخدم: {EmailOrPhone}", request.EmailOrPhone);

            // التحقق من صحة البيانات
            if (string.IsNullOrWhiteSpace(request.EmailOrPhone))
            {
                return ResultDto<ClientLoginUserResponse>.Failed("البريد الإلكتروني أو رقم الهاتف مطلوب", "EMAIL_OR_PHONE_REQUIRED");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return ResultDto<ClientLoginUserResponse>.Failed("كلمة المرور مطلوبة", "PASSWORD_REQUIRED");
            }

            // استخدام خدمة المصادقة للتحقق من المستخدم وإصدار التوكنات
            var authResult = await _authService.LoginAsync(request.EmailOrPhone, request.Password, cancellationToken);

            if (authResult == null)
            {
                _logger.LogWarning("فشل تسجيل الدخول: بيانات غير صحيحة {EmailOrPhone}", request.EmailOrPhone);
                return ResultDto<ClientLoginUserResponse>.Failed("البريد الإلكتروني أو كلمة المرور غير صحيحة", "INVALID_CREDENTIALS");
            }

            // تحويل النتيجة إلى التنسيق المطلوب للعميل
            var response = new ClientLoginUserResponse
            {
                UserId = authResult.UserId,
                Name = authResult.UserName,
                Email = authResult.Email,
                Phone = "", // سيتم تحديثه من قاعدة البيانات إذا لزم الأمر
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken,
                Roles = new List<string> { authResult.Role }
            };

            _logger.LogInformation("تم تسجيل دخول المستخدم بنجاح {UserId}", authResult.UserId);
            
            return ResultDto<ClientLoginUserResponse>.Ok(response, "تم تسجيل الدخول بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تسجيل دخول المستخدم {EmailOrPhone}", request.EmailOrPhone);
            return ResultDto<ClientLoginUserResponse>.Failed($"حدث خطأ أثناء تسجيل الدخول: {ex.Message}", "LOGIN_ERROR");
        }
    }
}