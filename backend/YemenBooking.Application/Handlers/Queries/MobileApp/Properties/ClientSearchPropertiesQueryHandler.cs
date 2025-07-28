using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using PSResultDto = YemenBooking.Application.DTOs.PropertySearch.PropertySearchResultDto;
using YemenBooking.Application.Queries.MobileApp.Properties;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Properties;

/// <summary>
/// معالج استعلام البحث عن الكيانات للعميل
/// Handler for client search properties query
/// </summary>
public class ClientSearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, ResultDto<SearchPropertiesResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientSearchPropertiesQueryHandler> _logger;

    public ClientSearchPropertiesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<ClientSearchPropertiesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام البحث عن الكيانات
    /// Handle search properties query
    /// </summary>
    /// <param name="request">الطلب</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<SearchPropertiesResponse>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء البحث عن الكيانات - المدينة: {City}, البحث: {SearchTerm}", request.City, request.SearchTerm);

            // التحقق من صحة المعاملات
            var validationError = ValidateRequest(request);
            if (validationError != null)
            {
                return validationError;
            }

            // البحث في قاعدة البيانات
            var searchResult = await SearchProperties(request);

            _logger.LogInformation("تم العثور على {Count} كيان من أصل {Total}", searchResult.Properties.Count, searchResult.TotalCount);

            return searchResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء البحث عن الكيانات");
            return new SearchPropertiesResponse
            {
                Properties = new List<PSResultDto>(),
                TotalCount = 0,
                CurrentPage = request.PageNumber,
                TotalPages = 0
            };
        }
    }

    /// <summary>
    /// التحقق من صحة طلب البحث
    /// Validate search request
    /// </summary>
    private SearchPropertiesResponse? ValidateRequest(SearchPropertiesQuery request)
    {
        if (request.PageNumber < 1)
        {
            return new SearchPropertiesResponse
            {
                Properties = new List<PSResultDto>(),
                TotalCount = 0,
                CurrentPage = 1,
                TotalPages = 0
            };
        }

        if (request.PageSize < 1 || request.PageSize > 100)
        {
            return new SearchPropertiesResponse
            {
                Properties = new List<PSResultDto>(),
                TotalCount = 0,
                CurrentPage = request.PageNumber,
                TotalPages = 0
            };
        }

        if (request.CheckIn.HasValue && request.CheckOut.HasValue && request.CheckIn >= request.CheckOut)
        {
            return new SearchPropertiesResponse
            {
                Properties = new List<PSResultDto>(),
                TotalCount = 0,
                CurrentPage = request.PageNumber,
                TotalPages = 0
            };
        }

        if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
        {
            return new SearchPropertiesResponse
            {
                Properties = new List<PSResultDto>(),
                TotalCount = 0,
                CurrentPage = request.PageNumber,
                TotalPages = 0
            };
        }

        return null;
    }

    /// <summary>
    /// البحث في قاعدة البيانات
    /// Search in database
    /// </summary>
    private async Task<SearchPropertiesResponse> SearchProperties(SearchPropertiesQuery request)
    {
        var propertyRepo = _unitOfWork.Repository<Core.Entities.Property>();
        var allProperties = await propertyRepo.GetAllAsync();

        // تطبيق مرشحات البحث
        var filteredProperties = allProperties.AsEnumerable();

        // البحث بالنص
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            filteredProperties = filteredProperties.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                p.City.ToLower().Contains(searchTerm));
        }

        // البحث بالمدينة
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            filteredProperties = filteredProperties.Where(p =>
                p.City.ToLower().Contains(request.City.ToLower()));
        }

        // البحث بنوع الكيان
        if (request.PropertyTypeId.HasValue)
        {
            filteredProperties = filteredProperties.Where(p =>
                p.TypeId == request.PropertyTypeId.Value);
        }

        // البحث بتصنيف النجوم
        if (request.MinStarRating.HasValue)
        {
            filteredProperties = filteredProperties.Where(p =>
                p.StarRating >= request.MinStarRating.Value);
        }

        // حساب إجمالي النتائج قبل التصفية بالصفحات
        var totalCount = filteredProperties.Count();

        // تطبيق الترتيب
        filteredProperties = ApplySorting(filteredProperties, request);

        // تطبيق الصفحات
        var paginatedProperties = filteredProperties
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // تحويل إلى DTOs
        var propertyDtos = new List<PSResultDto>();
        foreach (var property in paginatedProperties)
        {
            var dto = await MapToSearchResultDto(property, request);
            propertyDtos.Add(dto);
        }

        return new SearchPropertiesResponse
        {
            Properties = propertyDtos,
            TotalCount = totalCount,
            CurrentPage = request.PageNumber,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    /// <summary>
    /// تطبيق الترتيب
    /// Apply sorting
    /// </summary>
    private IEnumerable<Core.Entities.Property> ApplySorting(IEnumerable<Core.Entities.Property> properties, SearchPropertiesQuery request)
    {
        return request.SortBy?.ToLower() switch
        {
            "price_asc" => properties.OrderBy(p => GetMinPriceForProperty(p.Id)),
            "price_desc" => properties.OrderByDescending(p => GetMinPriceForProperty(p.Id)),
            "rating" => properties.OrderByDescending(p => p.AverageRating),
            "name" => properties.OrderBy(p => p.Name),
            "newest" => properties.OrderByDescending(p => p.CreatedAt),
            _ => properties.OrderByDescending(p => p.AverageRating) // افتراضي: الأعلى تقييماً
        };
    }

    /// <summary>
    /// تحويل الكيان إلى DTO
    /// Map property to search result DTO
    /// </summary>
    private async Task<PSResultDto> MapToSearchResultDto(Core.Entities.Property property, SearchPropertiesQuery request)
    {
        var dto = new PSResultDto
        {
            Id = property.Id,
            Name = property.Name,
            PropertyType = await GetPropertyTypeName(property.TypeId),
            Address = property.Address,
            City = property.City,
            StarRating = property.StarRating,
            AverageRating = property.AverageRating,
            ReviewsCount = await GetReviewsCount(property.Id),
            MinPrice = GetMinPriceForProperty(property.Id),
            Currency = "YER", // عملة افتراضية
            MainImageUrl = GetMainImageUrl(property.Id),
            IsFavorite = false, // سيتم تحديثه لاحقاً بناءً على المستخدم
            MainAmenities = await GetMainAmenities(property.Id)
        };

        // حساب المسافة إذا تم توفير إحداثيات
        if (request.Latitude.HasValue && request.Longitude.HasValue && 
            property.Latitude != 0 && property.Longitude != 0)
        {
            dto.DistanceKm = CalculateDistance(
                (double)request.Latitude.Value, (double)request.Longitude.Value,
                (double)property.Latitude, (double)property.Longitude);
        }

        return dto;
    }

    /// <summary>
    /// الحصول على اسم نوع الكيان
    /// Get property type name
    /// </summary>
    private async Task<string> GetPropertyTypeName(Guid propertyTypeId)
    {
        try
        {
            var propertyTypeRepo = _unitOfWork.Repository<Core.Entities.PropertyType>();
            var propertyType = await propertyTypeRepo.GetByIdAsync(propertyTypeId);
            return propertyType?.Name ?? "غير محدد";
        }
        catch
        {
            return "غير محدد";
        }
    }

    /// <summary>
    /// الحصول على عدد المراجعات
    /// Get reviews count
    /// </summary>
    private async Task<int> GetReviewsCount(Guid propertyId)
    {
        try
        {
            var reviewRepo = _unitOfWork.Repository<Core.Entities.Review>();
            var reviews = await reviewRepo.GetAllAsync();
            return reviews.Count(r => r.PropertyId == propertyId);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// الحصول على أقل سعر للكيان
    /// Get minimum price for property
    /// </summary>
    private decimal GetMinPriceForProperty(Guid propertyId)
    {
        // سعر افتراضي - سيتم تحسينه لاحقاً للحصول على السعر الفعلي من الوحدات
        return 100m;
    }

    /// <summary>
    /// الحصول على رابط الصورة الرئيسية
    /// Get main image URL
    /// </summary>
    private string GetMainImageUrl(Guid propertyId)
    {
        // صورة افتراضية - سيتم تحسينه لاحقاً
        return "/images/property-placeholder.jpg";
    }

    /// <summary>
    /// الحصول على وسائل الراحة الرئيسية
    /// Get main amenities
    /// </summary>
    private async Task<List<string>> GetMainAmenities(Guid propertyId)
    {
        try
        {
            // سيتم تطوير هذا لاحقاً للحصول على وسائل الراحة الفعلية
            return new List<string> { "واي فاي مجاني", "مواقف سيارات", "مكيف هواء" };
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// حساب المسافة بين نقطتين بالكيلومتر
    /// Calculate distance between two points in kilometers
    /// </summary>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadius = 6371; // نصف قطر الأرض بالكيلومتر

        var lat1Rad = Math.PI * lat1 / 180;
        var lat2Rad = Math.PI * lat2 / 180;
        var deltaLat = Math.PI * (lat2 - lat1) / 180;
        var deltaLon = Math.PI * (lon2 - lon1) / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadius * c;
    }
}