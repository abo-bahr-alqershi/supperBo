using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تسجيل عميل جديد
    /// </summary>
    public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, ResultDto<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RegisterClientCommandHandler> _logger;

        public RegisterClientCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            IEmailService emailService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<RegisterClientCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تسجيل عميل جديد: Email={Email}, Name={Name}", request.Email, request.Name);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failure("الاسم مطلوب");
            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<Guid>.Failure("البريد الإلكتروني مطلوب");
            if (string.IsNullOrWhiteSpace(request.Password))
                return ResultDto<Guid>.Failure("كلمة المرور مطلوبة");
            if (string.IsNullOrWhiteSpace(request.Phone))
                return ResultDto<Guid>.Failure("رقم الهاتف مطلوب");

            // التحقق من وجود البريد الإلكتروني
            if (await _userRepository.CheckEmailExistsAsync(request.Email.Trim(), cancellationToken))
                return ResultDto<Guid>.Failure("البريد الإلكتروني مستخدم بالفعل");

            // تحقق من قوة كلمة المرور
            var (isValid, issues) = await _passwordHashingService.ValidatePasswordStrengthAsync(request.Password, cancellationToken);
            if (!isValid)
                return ResultDto<Guid>.Failure($"كلمة المرور غير قوية: {string.Join(", ", issues)}");

            // إنشاء المستخدم
            var hashedPassword = await _passwordHashingService.HashPasswordAsync(request.Password, cancellationToken);
            var user = new User
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                Password = hashedPassword,
                Phone = request.Phone.Trim(),
                ProfileImage = request.ProfileImage?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                EmailConfirmed = false
            };
            var createdUser = await _userRepository.CreateUserAsync(user, cancellationToken);

            // إرسال بريد ترحيبي
            await _emailService.SendWelcomeEmailAsync(createdUser.Email, createdUser.Name, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "RegisterClient",
                $"تم تسجيل عميل جديد {createdUser.Id}",
                createdUser.Id,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("تم تسجيل العميل بنجاح: UserId={UserId}", createdUser.Id);
            return ResultDto<Guid>.Ok(createdUser.Id, "تم تسجيل العميل بنجاح");
        }
    }
} 