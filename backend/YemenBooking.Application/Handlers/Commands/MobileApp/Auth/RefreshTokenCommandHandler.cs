using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth;

/// <summary>
/// معالج أمر تحديث رمز الوصول
/// Handler for refresh token command
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResultDto<RefreshTokenResponse>>
{
    private readonly IAuthenticationService _authService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    /// <summary>
    /// منشئ معالج أمر تحديث رمز الوصول
    /// Constructor for refresh token command handler
    /// </summary>
    /// <param name="authService">خدمة المصادقة</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="logger">مسجل الأحداث</param>
    public RefreshTokenCommandHandler(
        IAuthenticationService authService,
        IUserRepository userRepository,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _authService = authService;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر تحديث رمز الوصول
    /// Handle refresh token command
    /// </summary>
    /// <param name="request">طلب تحديث رمز الوصول</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء عملية تحديث رمز الوصول");

            // التحقق من صحة البيانات المدخلة
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogWarning("محاولة تحديث رمز الوصول برمز تحديث فارغ");
                return ResultDto<RefreshTokenResponse>.Failed("رمز التحديث مطلوب", "REFRESH_TOKEN_REQUIRED");
            }

            if (string.IsNullOrWhiteSpace(request.AccessToken))
            {
                _logger.LogWarning("محاولة تحديث رمز الوصول برمز وصول فارغ");
                return ResultDto<RefreshTokenResponse>.Failed("رمز الوصول مطلوب", "ACCESS_TOKEN_REQUIRED");
            }

            // استخراج معرف المستخدم من رمز الوصول المنتهي الصلاحية
            var userIdClaim = await _tokenService.GetUserIdFromExpiredTokenAsync(request.AccessToken, cancellationToken);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("فشل في استخراج معرف المستخدم من رمز الوصول");
                return ResultDto<RefreshTokenResponse>.Failed("رمز الوصول غير صالح", "INVALID_ACCESS_TOKEN");
            }

            // التحقق من صحة رمز التحديث
            var isValidRefreshToken = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken, userId, cancellationToken);
            if (!isValidRefreshToken)
            {
                _logger.LogWarning("رمز التحديث غير صالح للمستخدم: {UserId}", userId);
                return ResultDto<RefreshTokenResponse>.Failed("رمز التحديث غير صالح أو منتهي الصلاحية", "INVALID_REFRESH_TOKEN");
            }

            // الحصول على معلومات المستخدم لإنشاء رموز جديدة
            var userInfo = await _authService.GetUserInfoAsync(userId, cancellationToken);
            if (userInfo == null)
            {
                _logger.LogWarning("لم يتم العثور على معلومات المستخدم: {UserId}", userId);
                return ResultDto<RefreshTokenResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // إنشاء رموز جديدة
            var tokenResult = await _tokenService.GenerateTokensAsync(userInfo, cancellationToken);
            if (tokenResult == null)
            {
                _logger.LogError("فشل في إنشاء رموز جديدة للمستخدم: {UserId}", userId);
                return ResultDto<RefreshTokenResponse>.Failed("فشل في إنشاء رموز جديدة", "TOKEN_GENERATION_FAILED");
            }

            // إلغاء رمز التحديث القديم
            try
            {
                await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
                _logger.LogInformation("تم إلغاء رمز التحديث القديم للمستخدم: {UserId}", userId);
            }
            catch (Exception revokeEx)
            {
                _logger.LogWarning(revokeEx, "فشل في إلغاء رمز التحديث القديم للمستخدم: {UserId}", userId);
                // لا نفشل العملية بسبب فشل إلغاء الرمز القديم
            }

            _logger.LogInformation("تم تحديث رمز الوصول بنجاح للمستخدم: {UserId}", userId);

            var response = new RefreshTokenResponse
            {
                NewAccessToken = tokenResult.AccessToken,
                NewRefreshToken = tokenResult.RefreshToken,
                AccessTokenExpiry = tokenResult.AccessTokenExpiry
            };

            return ResultDto<RefreshTokenResponse>.Ok(response, "تم تحديث رمز الوصول بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تحديث رمز الوصول");
            return ResultDto<RefreshTokenResponse>.Failed($"حدث خطأ أثناء تحديث رمز الوصول: {ex.Message}", "REFRESH_TOKEN_ERROR");
        }
    }
}
