using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth
{
    /// <summary>
    /// معالج أمر إعادة إرسال رمز تأكيد البريد الإلكتروني
    /// Handler for resend email verification command
    /// </summary>
    public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, ResultDto<ResendEmailVerificationResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly ILogger<ResendEmailVerificationCommandHandler> _logger;

        /// <summary>
        /// منشئ معالج أمر إعادة إرسال رمز تأكيد البريد الإلكتروني
        /// Constructor for resend email verification command handler
        /// </summary>
        /// <param name="userRepository">مستودع المستخدمين</param>
        /// <param name="emailService">خدمة البريد الإلكتروني</param>
        /// <param name="emailVerificationService">خدمة تأكيد البريد الإلكتروني</param>
        /// <param name="logger">مسجل الأحداث</param>
        public ResendEmailVerificationCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IEmailVerificationService emailVerificationService,
        ILogger<ResendEmailVerificationCommandHandler> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _emailVerificationService = emailVerificationService;
            _logger = logger;
        }

        /// <summary>
        /// معالجة أمر إعادة إرسال رمز تأكيد البريد الإلكتروني
        /// Handle resend email verification command
        /// </summary>
        /// <param name="request">طلب إعادة إرسال رمز التأكيد</param>
        /// <param name="cancellationToken">رمز الإلغاء</param>
        /// <returns>نتيجة العملية</returns>
        public async Task<ResultDto<ResendEmailVerificationResponse>> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء عملية إعادة إرسال رمز تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);

                // التحقق من صحة البيانات المدخلة
                if (request.UserId == Guid.Empty)
                {
                    _logger.LogWarning("محاولة إعادة إرسال رمز التأكيد بمعرف مستخدم غير صالح");
                    return ResultDto<ResendEmailVerificationResponse>.Failed("معرف المستخدم غير صالح", "INVALID_USER_ID");
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return ResultDto<ResendEmailVerificationResponse>.Failed("البريد الإلكتروني مطلوب", "EMAIL_REQUIRED");
                }

                // التحقق من صحة تنسيق البريد الإلكتروني
                var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                if (!emailRegex.IsMatch(request.Email))
                {
                    return ResultDto<ResendEmailVerificationResponse>.Failed("تنسيق البريد الإلكتروني غير صحيح", "INVALID_EMAIL_FORMAT");
                }

                // البحث عن المستخدم
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                    return ResultDto<ResendEmailVerificationResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
                }

                // التحقق من تطابق البريد الإلكتروني
                if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("البريد الإلكتروني غير متطابق للمستخدم: {UserId}", request.UserId);
                    return ResultDto<ResendEmailVerificationResponse>.Failed("البريد الإلكتروني غير متطابق مع الحساب", "EMAIL_MISMATCH");
                }

                // التحقق من حالة تأكيد البريد الإلكتروني
                if (user.IsEmailVerified)
                {
                    _logger.LogInformation("البريد الإلكتروني مؤكد مسبقاً للمستخدم: {UserId}", request.UserId);

                    var alreadyVerifiedResponse = new ResendEmailVerificationResponse
                    {
                        Success = true,
                        Message = "البريد الإلكتروني مؤكد مسبقاً"
                    };

                    return ResultDto<ResendEmailVerificationResponse>.Ok(alreadyVerifiedResponse, "البريد الإلكتروني مؤكد مسبقاً");
                }

                // التحقق من عدد المحاولات الأخيرة
                var recentAttemptsCount = await _emailVerificationService.GetRecentAttemptsCountAsync(request.UserId, TimeSpan.FromMinutes(15), cancellationToken);
                if (recentAttemptsCount >= 3)
                {
                    _logger.LogWarning("تجاوز حد محاولات إعادة إرسال رمز التأكيد للمستخدم: {UserId}", request.UserId);

                    var tooManyAttemptsResponse = new ResendEmailVerificationResponse
                    {
                        Success = false,
                        Message = "تم تجاوز الحد الأقصى لمحاولات إعادة الإرسال. يرجى المحاولة بعد 15 دقيقة",
                        RetryAfterSeconds = 900 // 15 دقيقة
                    };

                    return ResultDto<ResendEmailVerificationResponse>.Failed(tooManyAttemptsResponse, "تم تجاوز الحد الأقصى لمحاولات إعادة الإرسال", "TOO_MANY_ATTEMPTS");
                }

                // التحقق من الوقت المنقضي منذ آخر إرسال
                var lastSentTime = await _emailVerificationService.GetLastSentTimeAsync(request.UserId, cancellationToken);
                if (lastSentTime.HasValue)
                {
                    var timeSinceLastSent = DateTime.UtcNow - lastSentTime.Value;
                    var minimumWaitTime = TimeSpan.FromMinutes(2); // حد أدنى دقيقتان بين الإرسالات

                    if (timeSinceLastSent < minimumWaitTime)
                    {
                        var remainingSeconds = (int)(minimumWaitTime - timeSinceLastSent).TotalSeconds;

                        var waitResponse = new ResendEmailVerificationResponse
                        {
                            Success = false,
                            Message = $"يرجى الانتظار {remainingSeconds} ثانية قبل إعادة الإرسال",
                            RetryAfterSeconds = remainingSeconds
                        };

                        return ResultDto<ResendEmailVerificationResponse>.Failed(waitResponse, "يجب الانتظار قبل إعادة الإرسال", "WAIT_REQUIRED");
                    }
                }

                // إنشاء رمز تأكيد جديد
                var verificationToken = await _emailVerificationService.GenerateVerificationTokenAsync(request.UserId, cancellationToken);
                if (string.IsNullOrEmpty(verificationToken))
                {
                    _logger.LogError("فشل في إنشاء رمز تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);
                    return ResultDto<ResendEmailVerificationResponse>.Failed("فشل في إنشاء رمز التأكيد", "TOKEN_GENERATION_FAILED");
                }

                // إرسال بريد التأكيد
                var sendResult = await _emailService.SendEmailVerificationAsync(request.Email, verificationToken, user.Name, cancellationToken);
                if (!sendResult)
                {
                    _logger.LogError("فشل في إرسال بريد تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);

                    // حذف الرمز المُنشأ في حالة فشل الإرسال
                    await _emailVerificationService.InvalidateVerificationTokenAsync(verificationToken, cancellationToken);

                    return ResultDto<ResendEmailVerificationResponse>.Failed("فشل في إرسال بريد التأكيد", "SEND_FAILED");
                }

                // تسجيل محاولة الإرسال
                await _emailVerificationService.RecordSendAttemptAsync(request.UserId, cancellationToken);

                _logger.LogInformation("تم إعادة إرسال رمز تأكيد البريد الإلكتروني بنجاح للمستخدم: {UserId}", request.UserId);

                var response = new ResendEmailVerificationResponse
                {
                    Success = true,
                    Message = "تم إرسال رمز التأكيد إلى بريدك الإلكتروني. يرجى التحقق من صندوق الوارد وصندوق الرسائل غير المرغوب فيها"
                };

                return ResultDto<ResendEmailVerificationResponse>.Ok(response, "تم إرسال رمز التأكيد بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إعادة إرسال رمز تأكيد البريد الإلكتروني للمستخدم: {UserId}", request.UserId);
                return ResultDto<ResendEmailVerificationResponse>.Failed($"حدث خطأ أثناء إعادة إرسال رمز التأكيد: {ex.Message}", "RESEND_VERIFICATION_ERROR");
            }
        }
    }
}
