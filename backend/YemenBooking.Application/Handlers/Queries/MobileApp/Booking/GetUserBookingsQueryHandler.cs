using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Booking;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Booking;

/// <summary>
/// معالج استعلام الحصول على حجوزات المستخدم
/// Handler for get user bookings query
/// </summary>
public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, ResultDto<UserBookingsResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserBookingsQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام حجوزات المستخدم
    /// Constructor for get user bookings query handler
    /// </summary>
    /// <param name="bookingRepository">مستودع الحجوزات</param>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="unitRepository">مستودع الوحدات</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="logger">مسجل الأحداث</param>
    public GetUserBookingsQueryHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        IUserRepository userRepository,
        ILogger<GetUserBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على حجوزات المستخدم
    /// Handle get user bookings query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة حجوزات المستخدم</returns>
    public async Task<ResultDto<UserBookingsResponse>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام حجوزات المستخدم. معرف المستخدم: {UserId}, الحالة: {Status}, الصفحة: {PageNumber}", 
                request.UserId, request.Status?.ToString() ?? "جميع الحالات", request.PageNumber);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                return ResultDto<UserBookingsResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // الحصول على حجوزات المستخدم مع التصفح
            var (bookings, totalCount) = await _bookingRepository.GetUserBookingsAsync(
                request.UserId, 
                request.Status, 
                request.PageNumber, 
                request.PageSize, 
                cancellationToken);

            if (bookings == null || !bookings.Any())
            {
                _logger.LogInformation("لم يتم العثور على حجوزات للمستخدم: {UserId}", request.UserId);
                
                return ResultDto<UserBookingsResponse>.Ok(
                    new UserBookingsResponse
                    {
                        Bookings = new List<BookingDto>(),
                        TotalCount = 0,
                        CurrentPage = request.PageNumber,
                        TotalPages = 0
                    }, 
                    "لا توجد حجوزات متاحة"
                );
            }

            // تحويل البيانات إلى DTO
            var bookingDtos = new List<BookingDto>();
            
            foreach (var booking in bookings)
            {
                // الحصول على تفاصيل الوحدة والعقار
                var unit = await _unitRepository.GetByIdAsync(booking.UnitId, cancellationToken);
                var property = unit != null ? await _propertyRepository.GetByIdAsync(unit.PropertyId, cancellationToken) : null;

                // تحديد إمكانية الإلغاء والتقييم
                var canCancel = CanCancelBooking(booking);
                var canReview = CanReviewBooking(booking);

                var bookingDto = new BookingDto
                {
                    Id = booking.Id,
                    BookingNumber = booking.BookingNumber ?? string.Empty,
                    PropertyName = property?.Name ?? "غير متاح",
                    UnitName = unit?.Name ?? "غير متاح",
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    GuestsCount = booking.GuestsCount,
                    TotalPrice = booking.TotalPrice,
                    Currency = booking.Currency ?? "YER",
                    Status = booking.Status,
                    BookedAt = booking.BookedAt,
                    PropertyImageUrl = property?.MainImageUrl ?? string.Empty,
                    CanCancel = canCancel,
                    CanReview = canReview
                };

                bookingDtos.Add(bookingDto);
            }

            // ترتيب الحجوزات حسب تاريخ الحجز (الأحدث أولاً)
            bookingDtos = bookingDtos.OrderByDescending(b => b.BookedAt).ToList();

            // حساب إجمالي عدد الصفحات
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var response = new UserBookingsResponse
            {
                Bookings = bookingDtos,
                TotalCount = totalCount,
                CurrentPage = request.PageNumber,
                TotalPages = totalPages
            };

            _logger.LogInformation("تم الحصول على {Count} حجز للمستخدم {UserId} بنجاح", bookingDtos.Count, request.UserId);

            return ResultDto<UserBookingsResponse>.Ok(
                response, 
                $"تم الحصول على {bookingDtos.Count} حجز من أصل {totalCount}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على حجوزات المستخدم. معرف المستخدم: {UserId}", request.UserId);
            return ResultDto<UserBookingsResponse>.Failed(
                $"حدث خطأ أثناء الحصول على الحجوزات: {ex.Message}", 
                "GET_USER_BOOKINGS_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<UserBookingsResponse> ValidateRequest(GetUserBookingsQuery request)
    {
        if (request.UserId == Guid.Empty)
        {
            _logger.LogWarning("معرف المستخدم مطلوب");
            return ResultDto<UserBookingsResponse>.Failed("معرف المستخدم مطلوب", "USER_ID_REQUIRED");
        }

        if (request.PageNumber < 1)
        {
            _logger.LogWarning("رقم الصفحة يجب أن يكون أكبر من صفر");
            return ResultDto<UserBookingsResponse>.Failed("رقم الصفحة يجب أن يكون أكبر من صفر", "INVALID_PAGE_NUMBER");
        }

        if (request.PageSize < 1 || request.PageSize > 100)
        {
            _logger.LogWarning("حجم الصفحة يجب أن يكون بين 1 و 100");
            return ResultDto<UserBookingsResponse>.Failed("حجم الصفحة يجب أن يكون بين 1 و 100", "INVALID_PAGE_SIZE");
        }

        return ResultDto<UserBookingsResponse>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// تحديد إمكانية إلغاء الحجز
    /// Determine if booking can be cancelled
    /// </summary>
    /// <param name="booking">الحجز</param>
    /// <returns>هل يمكن الإلغاء</returns>
    private bool CanCancelBooking(Core.Entities.Booking booking)
    {
        // يمكن إلغاء الحجز إذا كان في حالة مؤكد أو معلق وقبل تاريخ الوصول بـ 24 ساعة على الأقل
        return (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Pending) &&
               booking.CheckIn > DateTime.UtcNow.AddHours(24);
    }

    /// <summary>
    /// تحديد إمكانية تقييم الحجز
    /// Determine if booking can be reviewed
    /// </summary>
    /// <param name="booking">الحجز</param>
    /// <returns>هل يمكن التقييم</returns>
    private bool CanReviewBooking(Core.Entities.Booking booking)
    {
        // يمكن تقييم الحجز إذا كان مكتمل وبعد تاريخ المغادرة
        return booking.Status == BookingStatus.Completed && 
               booking.CheckOut <= DateTime.UtcNow &&
               booking.CustomerRating == null; // لم يتم التقييم بعد
    }
}
