using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تسجيل الدخول وإصدار التوكنات
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResultDto<AuthResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IAuthenticationService authenticationService,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<LoginCommandHandler> logger)
        {
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء عملية تسجيل الدخول: Email={Email}", request.Email);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return ResultDto<AuthResultDto>.Failed("البريد الإلكتروني وكلمة المرور مطلوبان");

            // التحقق من تأكيد البريد الإلكتروني
            var userEntity = await _userRepository.GetUserByEmailAsync(request.Email.Trim(), cancellationToken);
            if (userEntity == null)
                return ResultDto<AuthResultDto>.Failed("المستخدم غير موجود");
            if (!userEntity.EmailConfirmed)
                return ResultDto<AuthResultDto>.Failed("يجب تأكيد البريد الإلكتروني قبل تسجيل الدخول");
            // التحقق من حالة الحساب
            // if (!userEntity.IsActive)
            //     return ResultDto<AuthResultDto>.Failed("حساب المستخدم معطل");

            // تنفيذ المصادقة الحقيقية
            YemenBooking.Core.DTOs.Common.AuthResultDto coreResult;
            try
            {
                coreResult = await _authenticationService.LoginAsync(request.Email.Trim(), request.Password, cancellationToken);
            }
            catch (Exception)
            {
                return ResultDto<AuthResultDto>.Failed("فشل تسجيل الدخول");
            }
            // تحويل نتيجة المصادقة إلى DTO الخاص بالتطبيق
            var authResult = new AuthResultDto
            {
                AccessToken = coreResult.AccessToken,
                RefreshToken = coreResult.RefreshToken,
                ExpiresAt = coreResult.ExpiresAt,
                UserId = coreResult.UserId,
                UserName = coreResult.UserName,
                Email = coreResult.Email,
                Role = coreResult.Role,
                ProfileImage = coreResult.ProfileImage,
                PropertyId = coreResult.PropertyId,
                PropertyName = coreResult.PropertyName,
                StaffId = coreResult.StaffId
            };

            // تحديث آخر تسجيل دخول
            var user = await _userRepository.GetUserByIdAsync(authResult.UserId, cancellationToken);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user, cancellationToken);
                authResult.SettingsJson = user.SettingsJson;
                authResult.FavoritesJson = user.FavoritesJson;
            }

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "Login",
                $"تم تسجيل الدخول للمستخدم {authResult.UserId}",
                authResult.UserId,
                "User",
                authResult.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تسجيل الدخول بنجاح: UserId={UserId}", authResult.UserId);
            return ResultDto<AuthResultDto>.Succeeded(authResult, "تم تسجيل الدخول بنجاح");
        }
    }
} 