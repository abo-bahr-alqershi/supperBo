using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Reviews;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Reviews
{
    /// <summary>
    /// معالج استعلام الحصول على تقييمات كيان محدد
    /// Handler for GetReviewsByPropertyQuery
    /// </summary>
    public class GetReviewsByPropertyQueryHandler : IRequestHandler<GetReviewsByPropertyQuery, PaginatedResult<ReviewDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetReviewsByPropertyQueryHandler> _logger;

        public GetReviewsByPropertyQueryHandler(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService,
            ILogger<GetReviewsByPropertyQueryHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<ReviewDto>> Handle(GetReviewsByPropertyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetReviewsByPropertyQuery for Property {PropertyId}", request.PropertyId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new UnauthorizedException("يجب تسجيل الدخول لعرض تقييمات الكيان");

            // Authorization: Admin or property owner
            if (!await _currentUserService.IsInRoleAsync("Admin"))
            {
                var userProps = _currentUserService.PropertyId;
                if (userProps != request.PropertyId)
                    throw new ForbiddenException("ليس لديك صلاحية لعرض تقييمات هذا الكيان");
            }

            // Build query with filters
            var query = _reviewRepository.GetQueryable()
                .AsNoTracking()
                .Include(r => r.Images)
                .Include(r => r.Booking).ThenInclude(b => b.Unit)
                .Where(r => r.Booking.Unit.PropertyId == request.PropertyId);

            if (request.MinRating.HasValue)
                query = query.Where(r => r.Value >= request.MinRating.Value);
            if (request.MaxRating.HasValue)
                query = query.Where(r => r.Value <= request.MaxRating.Value);
            if (request.IsPendingApproval.HasValue)
                query = query.Where(r => r.IsPendingApproval == request.IsPendingApproval.Value);
            if (request.HasResponse.HasValue)
                query = request.HasResponse.Value
                    ? query.Where(r => r.ResponseText != null)
                    : query.Where(r => r.ResponseText == null);
            if (request.ReviewedAfter.HasValue)
                query = query.Where(r => r.CreatedAt >= request.ReviewedAfter.Value);

            // Sorting
            query = request.SortBy switch
            {
                "cleanliness_rating" => query.OrderByDescending(r => r.Cleanliness),
                "service_rating" => query.OrderByDescending(r => r.Service),
                "review_date" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            // Pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var reviews = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = reviews.Select(r => new ReviewDto
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

            return PaginatedResult<ReviewDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
} 