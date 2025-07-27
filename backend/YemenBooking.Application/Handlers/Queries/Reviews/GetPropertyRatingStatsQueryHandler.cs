using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Reviews;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Reviews
{
    /// <summary>
    /// معالج استعلام إحصائيات تقييم الكيان
    /// Handler for GetPropertyRatingStatsQuery
    /// </summary>
    public class GetPropertyRatingStatsQueryHandler : IRequestHandler<GetPropertyRatingStatsQuery, ResultDto<PropertyRatingStatsDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyRatingStatsQueryHandler> _logger;

        public GetPropertyRatingStatsQueryHandler(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyRatingStatsQueryHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<PropertyRatingStatsDto>> Handle(GetPropertyRatingStatsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Property Rating Stats Query for Property {PropertyId}", request.PropertyId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                return ResultDto<PropertyRatingStatsDto>.Failure("يجب تسجيل الدخول لعرض إحصائيات تقييم الكيان");

            // Authorization: only admins can access rating stats
            if (!await _currentUserService.IsInRoleAsync("Admin"))
                return ResultDto<PropertyRatingStatsDto>.Failure("ليس لديك صلاحية لعرض إحصائيات تقييم الكيان");

            var stats = await _reviewRepository.GetPropertyRatingStatsAsync(request.PropertyId, cancellationToken);
            var dto = new PropertyRatingStatsDto
            {
                AverageRating = stats.AverageRating,
                TotalReviews = stats.TotalReviews
            };

            return ResultDto<PropertyRatingStatsDto>.Ok(dto);
        }
    }
} 