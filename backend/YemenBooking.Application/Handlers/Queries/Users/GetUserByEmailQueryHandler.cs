using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Application.Queries.Users;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Users
{
    /// <summary>
    /// معالج استعلام الحصول على بيانات المستخدم بواسطة البريد الإلكتروني
    /// Query handler for GetUserByEmailQuery
    /// </summary>
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, ResultDto<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByEmailQueryHandler> _logger;

        public GetUserByEmailQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetUserByEmailQueryHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام بيانات المستخدم بالبريد الإلكتروني: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<UserDto>.Failure("البريد الإلكتروني غير صالح");

            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                return ResultDto<UserDto>.Failure($"المستخدم بالبريد الإلكتروني {request.Email} غير موجود");

            var currentUserId = _currentUserService.UserId;
            var roles = _currentUserService.UserRoles;
            if (currentUserId != user.Id && !roles.Contains("Admin"))
                return ResultDto<UserDto>.Failure("ليس لديك صلاحية لعرض بيانات هذا المستخدم");

            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("تم جلب بيانات المستخدم بنجاح بالبريد الإلكتروني: {Email}", request.Email);
            return ResultDto<UserDto>.Ok(userDto, "تم جلب بيانات المستخدم بنجاح");
        }
    }
} 