using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.Commands.Properties;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using System.Linq;
using CommandImageDto = YemenBooking.Application.DTOs.PropertyImageDto;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تسجيل مالك كيان جديد مع بيانات الكيان الكاملة
    /// </summary>
    public class RegisterPropertyOwnerCommandHandler : IRequestHandler<RegisterPropertyOwnerCommand, ResultDto<OwnerRegistrationResultDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RegisterPropertyOwnerCommandHandler> _logger;

        public RegisterPropertyOwnerCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            IEmailService emailService,
            IMediator mediator,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<RegisterPropertyOwnerCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<OwnerRegistrationResultDto>> Handle(RegisterPropertyOwnerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تسجيل مالك كيان جديد: Email={Email}, Name={Name}", request.Email, request.Name);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<OwnerRegistrationResultDto>.Failure("الاسم مطلوب");
            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<OwnerRegistrationResultDto>.Failure("البريد الإلكتروني مطلوب");
            if (string.IsNullOrWhiteSpace(request.Password))
                return ResultDto<OwnerRegistrationResultDto>.Failure("كلمة المرور مطلوبة");
            if (string.IsNullOrWhiteSpace(request.Phone))
                return ResultDto<OwnerRegistrationResultDto>.Failure("رقم الهاتف مطلوب");

            // التحقق من وجود البريد الإلكتروني
            if (await _userRepository.CheckEmailExistsAsync(request.Email.Trim(), cancellationToken))
                return ResultDto<OwnerRegistrationResultDto>.Failure("البريد الإلكتروني مستخدم بالفعل");

            // تحقق من قوة كلمة المرور
            var (isValid, issues) = await _passwordHashingService.ValidatePasswordStrengthAsync(request.Password, cancellationToken);
            if (!isValid)
                return ResultDto<OwnerRegistrationResultDto>.Failure($"كلمة المرور غير قوية: {string.Join(", ", issues)}");

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

            // Map DTO images to command images
            var mappedInitialImages = request.InitialImages?
                .Select(i => new CommandImageDto
                {
                    Url = i.Url,
                    IsMain = i.IsMain,
                    AltText = i.AltText,
                    DisplayOrder = i.DisplayOrder
                })
                .ToList();

            // إنشاء الكيان مع الحقول الديناميكية
            var createPropertyCmd = new CreatePropertyCommand
            {
                OwnerId = createdUser.Id,
                PropertyTypeId = request.PropertyTypeId,
                Name = request.PropertyName,
                Description = request.Description,
                Address = request.Address,
                City = request.City,
                Latitude = request.Latitude.GetValueOrDefault(),
                Longitude = request.Longitude.GetValueOrDefault(),
                StarRating = request.StarRating,
            };
        
            var propertyResult = await _mediator.Send(createPropertyCmd, cancellationToken);
            if (!propertyResult.IsSuccess)
                return ResultDto<OwnerRegistrationResultDto>.Failure(propertyResult.Message!);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "RegisterPropertyOwner",
                $"تم تسجيل مالك كيان جديد {createdUser.Id} مع الكيان {propertyResult.Data}",
                createdUser.Id,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("تم تسجيل مالك الكيان بنجاح: UserId={UserId}, PropertyId={PropertyId}", createdUser.Id, propertyResult.Data);
            return ResultDto<OwnerRegistrationResultDto>.Ok(
                new OwnerRegistrationResultDto
                {
                    UserId = createdUser.Id,
                    PropertyId = propertyResult.Data
                },
                "تم تسجيل مالك الكيان بنجاح");
        }
    }
} 