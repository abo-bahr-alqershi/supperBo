using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth;

/// <summary>
/// معالج أمر تأكيد البريد الإلكتروني
/// Handler for verify email command
/// </summary>
public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ResultDto<VerifyEmailResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    /// <summary>
    /// منشئ معالج أمر تأكيد البريد الإلكتروني
    /// Constructor for verify email command handler
    /// </summary>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="emailVerificationService">خدمة تأكيد البريد الإلكتروني</param>
    /// <param name="logger">مسجل الأحداث</param>
    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationService emailVerificationService,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _emailVerificationService = emailVerificationService;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر تأكيد البريد الإلكتروني
    /// Handle verify email command
    /// </summary>
    /// <param name="request">طلب تأكيد البريد الإلكتروني</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<VerifyEmailResponse>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء عملية تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);

            // التحقق من صحة البيانات المدخلة
            if (request.UserId == Guid.Empty)
            {
                _logger.LogWarning("محاولة تأكيد بريد إلكتروني بمعرف مستخدم غير صالح");
                return ResultDto<VerifyEmailResponse>.Failed("معرف المستخدم غير صالح", "INVALID_USER_ID");
            }

            if (string.IsNullOrWhiteSpace(request.VerificationToken))
            {
                return ResultDto<VerifyEmailResponse>.Failed("رمز التأكيد مطلوب", "VERIFICATION_TOKEN_REQUIRED");
            }

            // البحث عن المستخدم
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                return ResultDto<VerifyEmailResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // التحقق من حالة تأكيد البريد الإلكتروني
            if (user.IsEmailVerified)
            {
                _logger.LogInformation("البريد الإلكتروني مؤكد مسبقاً للمستخدم: {UserId}", request.UserId);
                
                var alreadyVerifiedResponse = new VerifyEmailResponse
                {
                    Success = true,
                    Message = "البريد الإلكتروني مؤكد مسبقاً"
                };

                return ResultDto<VerifyEmailResponse>.Ok(alreadyVerifiedResponse, "البريد الإلكتروني مؤكد مسبقاً");
            }

            // التحقق من صحة رمز التأكيد
            var isValidToken = await _emailVerificationService.ValidateVerificationTokenAsync(
                request.VerificationToken, 
                request.UserId, 
                cancellationToken);

            if (!isValidToken)
            {
                _logger.LogWarning("رمز التأكيد غير صالح أو منتهي الصلاحية للمستخدم: {UserId}", request.UserId);
                return ResultDto<VerifyEmailResponse>.Failed("رمز التأكيد غير صالح أو منتهي الصلاحية", "INVALID_VERIFICATION_TOKEN");
            }

            // تأكيد البريد الإلكتروني
            user.IsEmailVerified = true;
            user.EmailVerifiedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userRepository.UpdateAsync(user, cancellationToken);
            if (!updateResult)
            {
                _logger.LogError("فشل في تحديث حالة تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);
                return ResultDto<VerifyEmailResponse>.Failed("فشل في تأكيد البريد الإلكتروني", "EMAIL_VERIFICATION_FAILED");
            }

            // إلغاء رمز التأكيد بعد الاستخدام
            try
            {
                await _emailVerificationService.InvalidateVerificationTokenAsync(request.VerificationToken, cancellationToken);
                _logger.LogInformation("تم إلغاء رمز التأكيد بعد الاستخدام للمستخدم: {UserId}", request.UserId);
            }
            catch (Exception tokenEx)
            {
                _logger.LogWarning(tokenEx, "فشل في إلغاء رمز التأكيد للمستخدم: {UserId}", request.UserId);
                // لا نفشل العملية بسبب فشل إلغاء الرمز
            }

            _logger.LogInformation("تم تأكيد البريد الإلكتروني بنجاح للمستخدم: {UserId}", request.UserId);

            var response = new VerifyEmailResponse
            {
                Success = true,
                Message = "تم تأكيد البريد الإلكتروني بنجاح. يمكنك الآن الاستفادة من جميع ميزات التطبيق"
            };

            return ResultDto<VerifyEmailResponse>.Ok(response, "تم تأكيد البريد الإلكتروني بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);
            return ResultDto<VerifyEmailResponse>.Failed($"حدث خطأ أثناء تأكيد البريد الإلكتروني: {ex.Message}", "VERIFY_EMAIL_ERROR");
        }
    }
}
