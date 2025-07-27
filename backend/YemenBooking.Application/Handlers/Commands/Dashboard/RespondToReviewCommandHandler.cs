using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Commands.Dashboard;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Entities;

namespace YemenBooking.Application.Handlers.Commands.Dashboard
{
    /// <summary>
    /// معالج أمر الرد على مراجعة من قبل المالك
    /// Responds to a review and includes:
    /// - التحقق من الإدخال
    /// - صلاحيات المالك أو المسؤول
    /// - تحديث رد المراجعة ضمن معاملة
    /// - تسجيل التدقيق
    /// - نشر الحدث
    /// </summary>
    public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand, ResultDto<bool>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<RespondToReviewCommandHandler> _logger;

        public RespondToReviewCommandHandler(
            IReviewRepository reviewRepository,
            IBookingRepository bookingRepository,
            IUnitRepository unitRepository,
            IPropertyRepository propertyRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            INotificationService notificationService,
            IEventPublisher eventPublisher,
            ILogger<RespondToReviewCommandHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
            _unitRepository = unitRepository;
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _notificationService = notificationService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ResultDto<bool>> Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء الرد على التقييم: ReviewId={ReviewId}", request.ReviewId);

            // التحقق من صحة الإدخال
            if (request.ReviewId == Guid.Empty)
                throw new BusinessRuleException("InvalidReviewId", "معرف التقييم غير صالح");
            if (string.IsNullOrWhiteSpace(request.ResponseText))
                throw new BusinessRuleException("InvalidResponseText", "نص الرد مطلوب");
            if (request.OwnerId == Guid.Empty)
                throw new BusinessRuleException("InvalidOwnerId", "معرف المالك مطلوب");

            // التحقق من وجود المراجعة
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException("Review", request.ReviewId.ToString());

            // جلب الحجز والوحدة لتحديد الملكية
            var booking = await _bookingRepository.GetBookingByIdAsync(review.BookingId, cancellationToken);
            if (booking == null)
                throw new NotFoundException("Booking", review.BookingId.ToString());
            var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
            if (unit == null)
                throw new NotFoundException("Unit", booking.UnitId.ToString());

            // التحقق من الصلاحيات
            if (_currentUserService.Role != "Admin" && request.OwnerId != _currentUserService.UserId)
                throw new ForbiddenException("غير مصرح لك بالرد على التقييم");
            if (_currentUserService.Role != "Admin")
            {
                var property = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
                if (property == null || property.OwnerId != _currentUserService.UserId)
                    throw new ForbiddenException("غير مصرح لك بالرد على التقييم");
            }

            // التحقق من عدم وجود رد مسبق
            if (!string.IsNullOrWhiteSpace(review.ResponseText))
                throw new BusinessRuleException("AlreadyResponded", "تم الرد على هذا التقييم مسبقًا");

            // تنفيذ الرد ضمن معاملة
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                review.ResponseText = request.ResponseText;
                review.ResponseDate = DateTime.UtcNow;
                review.UpdatedBy = _currentUserService.UserId;
                review.UpdatedAt = DateTime.UtcNow;
                await _reviewRepository.UpdateReviewAsync(review, cancellationToken);

                // تسجيل التدقيق
                await _auditService.LogActivityAsync(
                    "Review",
                    review.Id.ToString(),
                    "Respond",
                    $"تم الرد على التقييم {review.Id}",
                    null,
                    review,
                    cancellationToken);

                // نشر الحدث
                // await _eventPublisher.PublishEventAsync(new ReviewRespondedEvent
                // {
                //     ReviewId = review.Id,
                //     RespondedBy = _currentUserService.UserId,
                //     RespondedAt = DateTime.UtcNow,
                //     ResponseText = request.ResponseText
                // }, cancellationToken);

                // إرسال إشعار للضيف
                var user = await _userRepository.GetUserByIdAsync(booking.UserId, cancellationToken);
                await _notificationService.SendGuestNoteNotificationAsync(new GuestNoteNotification
                {
                    BookingId = booking.Id,
                    BookingNumber = booking.Id.ToString(),
                    GuestName = user?.Name ?? string.Empty,
                    GuestEmail = user?.Email,
                    NoteTitle = "رد المالك على تقييمك",
                    NoteContent = request.ResponseText,
                    RequiresResponse = false
                }, cancellationToken);
            });

            return ResultDto<bool>.Ok(true);
        }
    }

    /// <summary>
    /// حدث الرد على مراجعة
    /// Review responded event
    /// </summary>
    public class ReviewRespondedEvent
    {
        public Guid ReviewId { get; set; }
        public Guid RespondedBy { get; set; }
        public DateTime RespondedAt { get; set; }
        public string ResponseText { get; set; } = string.Empty;
    }
} 