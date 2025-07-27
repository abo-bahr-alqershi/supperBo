using System;
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
    /// معالج استعلام الحصول على تقييم حجز محدد
    /// Handler for GetReviewByBookingQuery
    /// </summary>
    public class GetReviewByBookingQueryHandler : IRequestHandler<GetReviewByBookingQuery, ResultDto<ReviewDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetReviewByBookingQueryHandler> _logger;

        public GetReviewByBookingQueryHandler(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService,
            ILogger<GetReviewByBookingQueryHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<ReviewDto>> Handle(GetReviewByBookingQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetReviewByBookingQuery for Booking {BookingId}", request.BookingId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                return ResultDto<ReviewDto>.Failure("يجب تسجيل الدخول لعرض تقييم الحجز");

            // Retrieve review with images and booking for authorization
            var reviewEntity = await _reviewRepository.GetQueryable()
                .AsNoTracking()
                .Include(r => r.Images)
                .Include(r => r.Booking)
                .FirstOrDefaultAsync(r => r.BookingId == request.BookingId, cancellationToken);

            if (reviewEntity == null)
                return ResultDto<ReviewDto>.Failure("لا يوجد تقييم لهذا الحجز");

            // Authorization: allow Admin or the user who made the booking
            if (!await _currentUserService.IsInRoleAsync("Admin") && reviewEntity.Booking.UserId != _currentUserService.UserId)
                return ResultDto<ReviewDto>.Failure("ليس لديك صلاحية لعرض هذا التقييم");

            // Map to DTO
            var dto = new ReviewDto
            {
                Id = reviewEntity.Id,
                BookingId = reviewEntity.BookingId,
                Cleanliness = reviewEntity.Cleanliness,
                Service = reviewEntity.Service,
                Location = reviewEntity.Location,
                Value = reviewEntity.Value,
                Comment = reviewEntity.Comment,
                CreatedAt = reviewEntity.CreatedAt,
                Images = reviewEntity.Images.Select(img => new ReviewImageDto
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
            };

            return ResultDto<ReviewDto>.Ok(dto);
        }
    }
} 