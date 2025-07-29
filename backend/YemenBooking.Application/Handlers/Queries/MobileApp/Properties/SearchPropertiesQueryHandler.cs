using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Properties;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using YemenBooking.Application.DTOs.PropertySearch;
using YemenBooking.Core.Interfaces.Repositories;
using System.Globalization;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Services;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Properties;

/// <summary>
/// معالج استعلام البحث عن العقارات
/// Handler for search properties query
/// </summary>
public class SearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, ResultDto<SearchPropertiesResponse>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyTypeRepository _propertyTypeRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<SearchPropertiesQueryHandler> _logger;
    private readonly IYemenBookingIndexService _indexService;

    /// <summary>
    /// منشئ معالج استعلام البحث عن العقارات
    /// Constructor for search properties query handler
    /// </summary>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="propertyTypeRepository">مستودع أنواع العقارات</param>
    /// <param name="amenityRepository">مستودع وسائل الراحة</param>
    /// <param name="favoriteRepository">مستودع المفضلات</param>
    /// <param name="reviewRepository">مستودع المراجعات</param>
    /// <param name="logger">مسجل الأحداث</param>
    public SearchPropertiesQueryHandler(
        IPropertyRepository propertyRepository,
        IPropertyTypeRepository propertyTypeRepository,
        IAmenityRepository amenityRepository,
        IFavoriteRepository favoriteRepository,
        IReviewRepository reviewRepository,
        IYemenBookingIndexService indexService,
        ILogger<SearchPropertiesQueryHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _propertyTypeRepository = propertyTypeRepository;
        _amenityRepository = amenityRepository;
        _favoriteRepository = favoriteRepository;
        _reviewRepository = reviewRepository;
        _logger = logger;
        _indexService = indexService;
    }

    /// <summary>
    /// معالجة استعلام البحث عن العقارات
    /// Handle search properties query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتائج البحث</returns>
    public async Task<ResultDto<SearchPropertiesResponse>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء البحث عن العقارات. كلمة البحث: {SearchTerm}, المدينة: {City}, الصفحة: {PageNumber}", 
                request.SearchTerm ?? "غير محدد", request.City ?? "غير محدد", request.PageNumber);

            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
                return validationResult;

            if (!_indexService.IsInitialized)
                await _indexService.InitializeAsync();

            // بناء طلب البحث باستخدام الفهرسة المتقدمة
            var indexRequest = new PropertySearchRequest
            {
                Query = request.SearchTerm,
                City = request.City,
                PropertyType = request.PropertyTypeId.HasValue
                    ? (await _propertyTypeRepository.GetByIdAsync(request.PropertyTypeId.Value, cancellationToken))?.Name
                    : null,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                MinRating = request.MinStarRating,
                AmenityIds = request.RequiredAmenities?.Select(id => id.ToString()).ToList(),
                SortBy = request.SortBy,
                SortOrder = request.SortBy?.ToLower().EndsWith("_desc") ? "Descending" : "Ascending",
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var indexResult = await _indexService.SearchPropertiesAsync(indexRequest);

            var propertyDtos = indexResult.Items.Select(item => new PropertySearchResultDto
            {
                Id = Guid.Parse(item.Id),
                Name = item.Name,
                Description = item.Description,
                Address = item.Address,
                City = item.City,
                StarRating = item.StarRating,
                AverageRating = item.AverageRating,
                ReviewsCount = 0,
                MinPrice = item.MinPrice,
                Currency = "YER",
                MainImageUrl = item.ImageUrls?.FirstOrDefault() ?? string.Empty,
                IsRecommended = false,
                DistanceKm = null,
                IsAvailable = item.UnitIds.Any(),
                AvailableUnitsCount = item.UnitIds.Count,
                PropertyType = item.PropertyType,
                IsFeatured = false,
                MainAmenities = item.AmenityIds ?? new List<string>()
            }).ToList();

            var response = new SearchPropertiesResponse
            {
                Properties = propertyDtos,
                TotalCount = indexResult.TotalCount,
                CurrentPage = indexResult.PageNumber,
                TotalPages = indexResult.TotalPages
            };

            _logger.LogInformation("تم العثور على {Count} عقار من أصل {Total} عقار", propertyDtos.Count, indexResult.TotalCount);
            return ResultDto<SearchPropertiesResponse>.Ok(
                response, 
                $"تم العثور على {propertyDtos.Count} عقار من أصل {indexResult.TotalCount}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء البحث عن العقارات");
            return ResultDto<SearchPropertiesResponse>.Failed(
                $"حدث خطأ أثناء البحث عن العقارات: {ex.Message}", 
                "SEARCH_PROPERTIES_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<SearchPropertiesResponse> ValidateRequest(SearchPropertiesQuery request)
    {
        if (request.PageNumber < 1)
        {
            _logger.LogWarning("رقم الصفحة يجب أن يكون أكبر من صفر");
            return ResultDto<SearchPropertiesResponse>.Failed("رقم الصفحة يجب أن يكون أكبر من صفر", "INVALID_PAGE_NUMBER");
        }

        if (request.PageSize < 1 || request.PageSize > 100)
        {
            _logger.LogWarning("حجم الصفحة يجب أن يكون بين 1 و 100");
            return ResultDto<SearchPropertiesResponse>.Failed("حجم الصفحة يجب أن يكون بين 1 و 100", "INVALID_PAGE_SIZE");
        }

        if (request.CheckIn.HasValue && request.CheckOut.HasValue && request.CheckIn >= request.CheckOut)
        {
            _logger.LogWarning("تاريخ الوصول يجب أن يكون قبل تاريخ المغادرة");
            return ResultDto<SearchPropertiesResponse>.Failed("تاريخ الوصول يجب أن يكون قبل تاريخ المغادرة", "INVALID_DATE_RANGE");
        }

        if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
        {
            _logger.LogWarning("السعر الأدنى يجب أن يكون أقل من السعر الأقصى");
            return ResultDto<SearchPropertiesResponse>.Failed("السعر الأدنى يجب أن يكون أقل من السعر الأقصى", "INVALID_PRICE_RANGE");
        }

        return ResultDto<SearchPropertiesResponse>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// بناء معايير البحث
    /// Build search criteria
    /// </summary>
    /// <param name="request">طلب البحث</param>
    /// <returns>معايير البحث</returns>
    private Dictionary<string, object> BuildSearchCriteria(SearchPropertiesQuery request)
    {
        var criteria = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            criteria["SearchTerm"] = request.SearchTerm;

        if (!string.IsNullOrWhiteSpace(request.City))
            criteria["City"] = request.City;

        if (request.CheckIn.HasValue)
            criteria["CheckIn"] = request.CheckIn.Value;

        if (request.CheckOut.HasValue)
            criteria["CheckOut"] = request.CheckOut.Value;

        if (request.GuestsCount.HasValue)
            criteria["GuestsCount"] = request.GuestsCount.Value;

        if (request.PropertyTypeId.HasValue)
            criteria["PropertyTypeId"] = request.PropertyTypeId.Value;

        if (request.MinPrice.HasValue)
            criteria["MinPrice"] = request.MinPrice.Value;

        if (request.MaxPrice.HasValue)
            criteria["MaxPrice"] = request.MaxPrice.Value;

        if (request.MinStarRating.HasValue)
            criteria["MinStarRating"] = request.MinStarRating.Value;

        if (request.RequiredAmenities.Any())
            criteria["RequiredAmenities"] = request.RequiredAmenities;

        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            criteria["Latitude"] = request.Latitude.Value;
            criteria["Longitude"] = request.Longitude.Value;
            criteria["RadiusKm"] = request.RadiusKm ?? 50; // افتراضي 50 كم
        }

        return criteria;
    }

    /// <summary>
    /// تطبيق الترتيب على النتائج
    /// Apply sorting to results
    /// </summary>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="sortBy">معيار الترتيب</param>
    /// <returns>قائمة مرتبة</returns>
    private List<YemenBooking.Application.DTOs.PropertySearch.PropertySearchResultDto> ApplySorting(List<YemenBooking.Application.DTOs.PropertySearch.PropertySearchResultDto> properties, string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "price_asc" => properties.OrderBy(p => p.MinPrice).ToList(),
            "price_desc" => properties.OrderByDescending(p => p.MinPrice).ToList(),
            "rating" => properties.OrderByDescending(p => p.AverageRating).ThenByDescending(p => p.ReviewsCount).ToList(),
            "distance" => properties.OrderBy(p => p.DistanceKm ?? double.MaxValue).ToList(),
            "popularity" => properties.OrderByDescending(p => p.ReviewsCount).ThenByDescending(p => p.AverageRating).ToList(),
            _ => properties.OrderByDescending(p => p.AverageRating).ThenByDescending(p => p.ReviewsCount).ToList()
        };
    }

    /// <summary>
    /// حساب المسافة بين نقطتين جغرافيتين
    /// Calculate distance between two geographic points
    /// </summary>
    /// <param name="lat1">خط العرض الأول</param>
    /// <param name="lon1">خط الطول الأول</param>
    /// <param name="lat2">خط العرض الثاني</param>
    /// <param name="lon2">خط الطول الثاني</param>
    /// <returns>المسافة بالكيلومتر</returns>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // نصف قطر الأرض بالكيلومتر

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    /// <summary>
    /// تحويل الدرجات إلى راديان
    /// Convert degrees to radians
    /// </summary>
    /// <param name="degrees">الدرجات</param>
    /// <returns>الراديان</returns>
    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
