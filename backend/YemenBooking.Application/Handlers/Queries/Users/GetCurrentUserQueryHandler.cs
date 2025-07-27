using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Application.Queries.Users;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Users
{
    /// <summary>
    /// معالج استعلام الحصول على بيانات المستخدم الحالي
    /// Query handler for GetCurrentUserQuery
    /// </summary>
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ResultDto<UserDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCurrentUserQueryHandler> _logger;

        public GetCurrentUserQueryHandler(
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetCurrentUserQueryHandler> logger)
        {
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("جاري معالجة استعلام بيانات المستخدم الحالي: {UserId}", _currentUserService.UserId);

                if (_currentUserService.UserId == Guid.Empty)
                {
                    return ResultDto<UserDto>.Failure("يجب تسجيل الدخول للوصول إلى بيانات المستخدم");
                }

                var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (user == null)
                {
                    return ResultDto<UserDto>.Failure($"المستخدم بالمعرف {_currentUserService.UserId} غير موجود");
                }

                var userDto = _mapper.Map<UserDto>(user);

                _logger.LogInformation("تم جلب بيانات المستخدم الحالي بنجاح: {UserId}", userDto.Id);
                return ResultDto<UserDto>.Ok(userDto, "تم جلب بيانات المستخدم بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة استعلام بيانات المستخدم الحالي");
                return ResultDto<UserDto>.Failure("حدث خطأ أثناء جلب بيانات المستخدم");
            }
        }
    }
} 