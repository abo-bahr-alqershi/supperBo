using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Properties;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Properties;

/// <summary>
/// معالج استعلام الحصول على العقارات المميزة للعميل
/// Handler for client get featured properties query
/// </summary>
public class ClientGetFeaturedPropertiesQueryHandler : IRequestHandler<ClientGetFeaturedPropertiesQuery, ResultDto<List<ClientFeaturedPropertyDto>>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly ISpecialOfferRepository _specialOfferRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<ClientGetFeaturedPropertiesQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام العقارات المميزة للعميل
    /// Constructor for client get featured properties query handler
    /// </summary>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="favoriteRepository">مستودع المفضلات</param>
    /// <param name="reviewRepository">مستودع المراجعات</param>
    /// <param name="propertyImageRepository">مستودع صور العقارات</param>
    /// <param name="amenityRepository">مستودع وسائل الراحة</param>
    /// <param name="specialOfferRepository">مستودع العروض الخاصة</param>
    /// <param name="bookingRepository">مستودع الحجوزات</param>
    /// <param name="logger">مسجل الأحداث</param>
    public ClientGetFeaturedPropertiesQueryHandler(
        IPropertyRepository propertyRepository,
        IFavoriteRepository favoriteRepository,
        IReviewRepository reviewRepository,
        IPropertyImageRepository propertyImageRepository,
        IAmenityRepository amenityRepository,
        ISpecialOfferRepository specialOfferRepository,
        IBookingRepository bookingRepository,
        ILogger<ClientGetFeaturedPropertiesQueryHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _favoriteRepository = favoriteRepository;
        _reviewRepository = reviewRepository;
        _propertyImageRepository = propertyImageRepository;
        _amenityRepository = amenityRepository;
        _specialOfferRepository = specialOfferRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على العقارات المميزة للعميل
    /// Handle client get featured properties query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة العقارات المميزة</returns>
    public async Task<ResultDto<List<ClientFeaturedPropertyDto>>> Handle(ClientGetFeaturedPropertiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام العقارات المميزة للعميل. العدد المطلوب: {Limit}, معرف المستخدم: {UserId}", 
                request.Limit, request.UserId);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // الحصول على العقارات المميزة
            var featuredProperties = await _propertyRepository.GetFeaturedPropertiesAsync(
                request.Limit * 2, // جلب عدد أكبر للفلترة والترتيب
                cancellationToken);

            if (featuredProperties == null || !featuredProperties.Any())
            {
                _logger.LogInformation("لا توجد عقارات مميزة متاحة");
                
                return ResultDto<List<ClientFeaturedPropertyDto>>.Ok(
                    new List<ClientFeaturedPropertyDto>(), 
                    "لا توجد عقارات مميزة متاحة حالياً"
                );
            }

            // تحويل العقارات إلى DTOs مع جلب البيانات المرتبطة بشكل متوازي
            var featuredPropertyDtos = (await Task.WhenAll(featuredProperties.Select(async property =>
            {
                var imagesTask = _propertyImageRepository.GetByPropertyIdAsync(property.Id, cancellationToken);
                var reviewsTask = _reviewRepository.GetByPropertyIdAsync(property.Id, cancellationToken);
                var amenitiesTask = _amenityRepository.GetAmenitiesByPropertyAsync(property.Id, cancellationToken);
                var offersTask = _specialOfferRepository.GetOffersByPropertyIdAsync(property.Id);
                var bookingRateTask = CalculateBookingRate(property.Id, cancellationToken);
                var isFavTask = request.UserId.HasValue
                    ? _favoriteRepository.IsPropertyFavoriteAsync(request.UserId.Value, property.Id, cancellationToken)
                    : Task.FromResult(false);
                await Task.WhenAll(imagesTask, reviewsTask, amenitiesTask, offersTask, bookingRateTask, isFavTask);
                var propertyImages = imagesTask.Result;
                var propertyReviews = reviewsTask.Result;
                var propertyAmenities = amenitiesTask.Result;
                var specialOffer = offersTask.Result.FirstOrDefault();
                var averageRating = propertyReviews?.Any() == true ? propertyReviews.Average(r => r.AverageRating) : 0;
                var reviewsCount = propertyReviews?.Count() ?? 0;
                var bookingRate = bookingRateTask.Result;
                var isFavorite = isFavTask.Result;
                var distanceKm = CalculateDistance(
                    request.CurrentLatitude, request.CurrentLongitude,
                    property.Latitude, property.Longitude);
                return new ClientFeaturedPropertyDto
                {
                    Id = property.Id,
                    Name = property.Name ?? string.Empty,
                    ShortDescription = property.ShortDescription ?? string.Empty,
                    City = property.City ?? string.Empty,
                    Address = property.Address ?? string.Empty,
                    MainImageUrl = propertyImages?.FirstOrDefault(img => img.IsMain)?.Url
                        ?? propertyImages?.FirstOrDefault()?.Url ?? string.Empty,
                    ImageGallery = propertyImages?.Take(3).Select(img => img.Url ?? string.Empty).ToList() ?? new List<string>(),
                    StarRating = property.StarRating,
                    AverageRating = Math.Round(averageRating, 1),
                    ReviewsCount = reviewsCount,
                    BasePricePerNight = property.BasePricePerNight,
                    Currency = property.Currency ?? "YER",
                    IsFavorite = isFavorite,
                    DistanceKm = distanceKm,
                    PropertyType = property.PropertyType?.Name ?? string.Empty,
                    TopAmenities = propertyAmenities?.Take(3).Select(a => a.PropertyTypeAmenity?.Amenity.Name ?? string.Empty).ToList() ?? new List<string>(),
                    AvailabilityStatus = GetAvailabilityStatus(property),
                    SpecialOffer = MapSpecialOffer(specialOffer),
                    BookingRate = bookingRate,
                    FeaturedBadge = GetFeaturedBadge(property, bookingRate, averageRating)
                };
            }))).ToList();

            // ترتيب العقارات حسب الأولوية
            var sortedProperties = SortFeaturedProperties(featuredPropertyDtos, request);

            // أخذ العدد المطلوب فقط
            var finalProperties = sortedProperties.Take(request.Limit).ToList();

            _logger.LogInformation("تم العثور على {Count} عقار مميز", finalProperties.Count);

            return ResultDto<List<ClientFeaturedPropertyDto>>.Ok(
                finalProperties, 
                $"تم العثور على {finalProperties.Count} عقار مميز"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على العقارات المميزة للعميل");
            return ResultDto<List<ClientFeaturedPropertyDto>>.Failed(
                $"حدث خطأ أثناء الحصول على العقارات المميزة: {ex.Message}", 
                "GET_FEATURED_PROPERTIES_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<List<ClientFeaturedPropertyDto>> ValidateRequest(ClientGetFeaturedPropertiesQuery request)
    {
        if (request.Limit < 1 || request.Limit > 50)
        {
            _logger.LogWarning("عدد العقارات المطلوبة يجب أن يكون بين 1 و 50");
            return ResultDto<List<ClientFeaturedPropertyDto>>.Failed("عدد العقارات المطلوبة يجب أن يكون بين 1 و 50", "INVALID_LIMIT");
        }

        // التحقق من صحة إحداثيات الموقع إذا تم تحديدها
        if (request.CurrentLatitude.HasValue || request.CurrentLongitude.HasValue)
        {
            if (!request.CurrentLatitude.HasValue || !request.CurrentLongitude.HasValue)
            {
                _logger.LogWarning("يجب تحديد كل من خط العرض وخط الطول");
                return ResultDto<List<ClientFeaturedPropertyDto>>.Failed("يجب تحديد كل من خط العرض وخط الطول", "INCOMPLETE_LOCATION");
            }

            if (Math.Abs(request.CurrentLatitude.Value) > 90 || Math.Abs(request.CurrentLongitude.Value) > 180)
            {
                _logger.LogWarning("إحداثيات الموقع غير صحيحة");
                return ResultDto<List<ClientFeaturedPropertyDto>>.Failed("إحداثيات الموقع غير صحيحة", "INVALID_COORDINATES");
            }
        }

        return ResultDto<List<ClientFeaturedPropertyDto>>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// حساب معدل الحجز للعقار
    /// Calculate booking rate for property
    /// </summary>
    /// <param name="propertyId">معرف العقار</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>معدل الحجز</returns>
    private async Task<decimal> CalculateBookingRate(Guid propertyId, CancellationToken cancellationToken)
    {
        try
        {
            // حساب معدل الحجز خلال آخر 30 يوم
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var recentBookings = await _bookingRepository.GetByPropertyIdAndDateRangeAsync(
                propertyId, thirtyDaysAgo, DateTime.UtcNow, cancellationToken);

            var totalBookings = recentBookings?.Count() ?? 0;
            
            // تحويل إلى نسبة مئوية (افتراض أن 10 حجوزات شهرياً = 100%)
            return Math.Min(100, (totalBookings / 10.0m) * 100);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء حساب معدل الحجز للعقار: {PropertyId}", propertyId);
            return 0;
        }
    }

    /// <summary>
    /// حساب المسافة بين نقطتين
    /// Calculate distance between two points
    /// </summary>
    /// <param name="lat1">خط العرض الأول</param>
    /// <param name="lon1">خط الطول الأول</param>
    /// <param name="lat2">خط العرض الثاني</param>
    /// <param name="lon2">خط الطول الثاني</param>
    /// <returns>المسافة بالكيلومتر</returns>
    private decimal? CalculateDistance(decimal? lat1, decimal? lon1, decimal? lat2, decimal? lon2)
    {
        if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
        {
            return null;
        }

        const double R = 6371; // نصف قطر الأرض بالكيلومتر

        var dLat = ToRadians((double)(lat2.Value - lat1.Value));
        var dLon = ToRadians((double)(lon2.Value - lon1.Value));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1.Value)) * Math.Cos(ToRadians((double)lat2.Value)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = R * c;

        return Math.Round((decimal)distance, 1);
    }

    /// <summary>
    /// تحويل الدرجات إلى راديان
    /// Convert degrees to radians
    /// </summary>
    /// <param name="degrees">الدرجات</param>
    /// <returns>الراديان</returns>
    private double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    /// <summary>
    /// الحصول على حالة التوفر
    /// Get availability status
    /// </summary>
    /// <param name="property">العقار</param>
    /// <returns>حالة التوفر</returns>
    private string GetAvailabilityStatus(Core.Entities.Property property)
    {
        if (!property.IsActive)
        {
            return "غير متاح";
        }

        // يمكن إضافة منطق أكثر تعقيداً هنا للتحقق من توفر الوحدات
        return "متاح";
    }

    /// <summary>
    /// تحويل العرض الخاص إلى DTO
    /// Map special offer to DTO
    /// </summary>
    /// <param name="specialOffer">العرض الخاص</param>
    /// <returns>DTO العرض الخاص</returns>
    private ClientSpecialOfferDto? MapSpecialOffer(Core.Entities.SpecialOffer? specialOffer)
    {
        if (specialOffer == null || !specialOffer.IsActive || 
            (specialOffer.ExpiryDate.HasValue && specialOffer.ExpiryDate.Value < DateTime.UtcNow))
        {
            return null;
        }

        return new ClientSpecialOfferDto
        {
            Title = specialOffer.Title ?? string.Empty,
            Description = specialOffer.Description ?? string.Empty,
            DiscountPercentage = specialOffer.DiscountPercentage,
            DiscountAmount = specialOffer.DiscountAmount,
            ExpiryDate = specialOffer.ExpiryDate,
            Color = specialOffer.Color ?? "#FF6B6B"
        };
    }

    /// <summary>
    /// الحصول على شارة العقار المميز
    /// Get featured badge for property
    /// </summary>
    /// <param name="property">العقار</param>
    /// <param name="bookingRate">معدل الحجز</param>
    /// <param name="averageRating">متوسط التقييم</param>
    /// <returns>شارة العقار المميز</returns>
    private string? GetFeaturedBadge(Core.Entities.Property property, decimal bookingRate, decimal averageRating)
    {
        // العقارات الجديدة (أقل من 30 يوم)
        if (property.CreatedAt > DateTime.UtcNow.AddDays(-30))
        {
            return "جديد";
        }

        // العقارات الأكثر حجزاً
        if (bookingRate >= 80)
        {
            return "الأكثر حجزاً";
        }

        // العقارات عالية التقييم
        if (averageRating >= 4.5m)
        {
            return "موصى به";
        }

        // العقارات المميزة حسب النظام
        if (property.IsFeatured)
        {
            return "مميز";
        }

        return null;
    }

    /// <summary>
    /// ترتيب العقارات المميزة حسب الأولوية
    /// Sort featured properties by priority
    /// </summary>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>العقارات مرتبة</returns>
    private List<ClientFeaturedPropertyDto> SortFeaturedProperties(
        List<ClientFeaturedPropertyDto> properties, 
        ClientGetFeaturedPropertiesQuery request)
    {
        return properties
            .OrderByDescending(p => p.SpecialOffer != null ? 1 : 0) // العروض الخاصة أولاً
            .ThenByDescending(p => p.FeaturedBadge == "الأكثر حجزاً" ? 1 : 0) // الأكثر حجزاً
            .ThenByDescending(p => p.FeaturedBadge == "موصى به" ? 1 : 0) // الموصى به
            .ThenByDescending(p => p.AverageRating) // التقييم الأعلى
            .ThenBy(p => p.DistanceKm ?? 1000) // الأقرب للموقع الحالي
            .ThenByDescending(p => p.BookingRate) // معدل الحجز
            .ThenBy(p => p.BasePricePerNight) // السعر الأقل
            .ToList();
    }
}
