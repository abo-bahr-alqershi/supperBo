using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام المراجعات المعلقة للموافقة
    /// Handler for GetPendingReviewsQuery
    /// </summary>
    public class GetPendingReviewsQueryHandler : IRequestHandler<GetPendingReviewsQuery, ResultDto<IEnumerable<ReviewDto>>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPendingReviewsQueryHandler> _logger;

        public GetPendingReviewsQueryHandler(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPendingReviewsQueryHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<ReviewDto>>> Handle(GetPendingReviewsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetPendingReviewsQuery");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                return ResultDto<IEnumerable<ReviewDto>>.Failure("يجب تسجيل الدخول لعرض المراجعات المعلقة");

            var role = _currentUserService.Role;
            if (role != "Admin")
                return ResultDto<IEnumerable<ReviewDto>>.Failure("ليس لديك صلاحية لعرض المراجعات المعلقة");

            var reviews = await _reviewRepository.GetQueryable()
                .Where(r => r.IsPendingApproval)
                .Include(r => r.Images)
                .ToListAsync(cancellationToken);

            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                BookingId = r.BookingId,
                Cleanliness = r.Cleanliness,
                Service = r.Service,
                Location = r.Location,
                Value = r.Value,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                Images = r.Images.Select(img => new ReviewImageDto
                {
                    Id = img.Id,
                    ReviewId = img.ReviewId,
                    Name = img.Name,
                    Url = img.Url,
                    SizeBytes = img.SizeBytes,
                    Type = img.Type,
                    Category = img.Category,
                    Caption = img.Caption,
                    AltText = img.AltText,
                    UploadedAt = img.UploadedAt
                }).ToList()
            }).ToList();

            return ResultDto<IEnumerable<ReviewDto>>.Ok(reviewDtos);
        }
    }
} 