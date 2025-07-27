using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Analytics;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Analytics
{
    /// <summary>
    /// معالج استعلام متابعة تقدم ولاء المستخدم
    /// Handles GetUserLoyaltyProgressQuery and returns user loyalty progress data
    /// </summary>
    public class GetUserLoyaltyProgressQueryHandler : IRequestHandler<GetUserLoyaltyProgressQuery, ResultDto<LoyaltyProgressDto>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUserLoyaltyProgressQueryHandler> _logger;

        public GetUserLoyaltyProgressQueryHandler(
            IDashboardService dashboardService,
            ICurrentUserService currentUserService,
            ILogger<GetUserLoyaltyProgressQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<LoyaltyProgressDto>> Handle(GetUserLoyaltyProgressQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء متابعة تقدم ولاء المستخدم: {UserId}", request.UserId);

            if (request.UserId == Guid.Empty)
            {
                _logger.LogWarning("معرف المستخدم غير صالح");
                return ResultDto<LoyaltyProgressDto>.Failure("معرف المستخدم غير صالح");
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                return ResultDto<LoyaltyProgressDto>.Failure("يجب تسجيل الدخول لعرض هذا التقدم");
            }

            if (_currentUserService.UserId != request.UserId && _currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى هذا التقدم");
                return ResultDto<LoyaltyProgressDto>.Failure("ليس لديك صلاحية الوصول إلى هذا التقدم");
            }

            var data = await _dashboardService.GetUserLoyaltyProgressAsync(request.UserId);
            return ResultDto<LoyaltyProgressDto>.Ok(data);
        }
    }
} 