using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.SearchFilters;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.SearchFilters;

/// <summary>
/// معالج استعلام الحصول على فلاتر البحث المتاحة
/// Handler for get search filters query
/// </summary>
public class GetSearchFiltersQueryHandler : IRequestHandler<GetSearchFiltersQuery, ResultDto<SearchFiltersDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyTypeRepository _propertyTypeRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<GetSearchFiltersQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام فلاتر البحث
    /// Constructor for get search filters query handler
    /// </summary>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="propertyTypeRepository">مستودع أنواع العقارات</param>
    /// <param name="amenityRepository">مستودع وسائل الراحة</param>
    /// <param name="unitRepository">مستودع الوحدات</param>
    /// <param name="logger">مسجل الأحداث</param>
    public GetSearchFiltersQueryHandler(
        IPropertyRepository propertyRepository,
        IPropertyTypeRepository propertyTypeRepository,
        IAmenityRepository amenityRepository,
        IUnitRepository unitRepository,
        ILogger<GetSearchFiltersQueryHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _propertyTypeRepository = propertyTypeRepository;
        _amenityRepository = amenityRepository;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على فلاتر البحث المتاحة
    /// Handle get search filters query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>فلاتر البحث المتاحة</returns>
    public async Task<ResultDto<SearchFiltersDto>> Handle(GetSearchFiltersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام فلاتر البحث. نوع العقار: {PropertyTypeId}, المدينة: {City}", 
                request.PropertyTypeId, request.City);

            var searchFilters = new SearchFiltersDto();

            // الحصول على جميع العقارات النشطة
            var activeProperties = await _propertyRepository.GetActivePropertiesAsync(cancellationToken);
            
            if (activeProperties == null || !activeProperties.Any())
            {
                _logger.LogInformation("لا توجد عقارات نشطة في النظام");
                return ResultDto<SearchFiltersDto>.Ok(GetDefaultFilters(), "لا توجد عقارات متاحة حالياً");
            }

            // فلترة العقارات حسب المدينة إذا تم تحديدها
            if (!string.IsNullOrWhiteSpace(request.City))
            {
                activeProperties = activeProperties.Where(p => 
                    p.City != null && p.City.Equals(request.City, StringComparison.OrdinalIgnoreCase));
            }

            // فلترة العقارات حسب نوع العقار إذا تم تحديده
            if (request.PropertyTypeId.HasValue)
            {
                activeProperties = activeProperties.Where(p => p.PropertyTypeId == request.PropertyTypeId.Value);
            }

            var propertiesList = activeProperties.ToList();

            // جلب المدن المتاحة
            await PopulateAvailableCities(searchFilters, propertiesList, cancellationToken);

            // جلب نطاق الأسعار
            await PopulatePriceRange(searchFilters, propertiesList, cancellationToken);

            // جلب أنواع العقارات المتاحة
            await PopulatePropertyTypes(searchFilters, propertiesList, cancellationToken);

            // جلب وسائل الراحة المتاحة
            await PopulateAmenities(searchFilters, propertiesList, cancellationToken);

            // جلب تصنيفات النجوم المتاحة
            PopulateStarRatings(searchFilters, propertiesList);

            // جلب الحد الأقصى للسعة
            await PopulateMaxGuestCapacity(searchFilters, propertiesList, cancellationToken);

            _logger.LogInformation("تم جلب فلاتر البحث بنجاح. المدن: {CitiesCount}, أنواع العقارات: {PropertyTypesCount}, وسائل الراحة: {AmenitiesCount}", 
                searchFilters.AvailableCities.Count, searchFilters.PropertyTypes.Count, searchFilters.Amenities.Count);

            return ResultDto<SearchFiltersDto>.Ok(searchFilters, "تم جلب فلاتر البحث بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب فلاتر البحث");
            return ResultDto<SearchFiltersDto>.Failed(
                $"حدث خطأ أثناء جلب فلاتر البحث: {ex.Message}", 
                "GET_SEARCH_FILTERS_ERROR"
            );
        }
    }

    /// <summary>
    /// جلب المدن المتاحة
    /// Populate available cities
    /// </summary>
    /// <param name="searchFilters">فلاتر البحث</param>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task PopulateAvailableCities(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var cities = properties
                .Where(p => !string.IsNullOrWhiteSpace(p.City))
                .Select(p => p.City!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            searchFilters.AvailableCities = cities;
            
            _logger.LogDebug("تم جلب {Count} مدينة متاحة", cities.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب المدن المتاحة");
            searchFilters.AvailableCities = new List<string>();
        }
    }

    /// <summary>
    /// جلب نطاق الأسعار
    /// Populate price range
    /// </summary>
    /// <param name="searchFilters">فلاتر البحث</param>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task PopulatePriceRange(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var allPrices = new List<decimal>();

            foreach (var property in properties)
            {
                var units = await _unitRepository.GetActiveByPropertyIdAsync(property.Id, cancellationToken);
                if (units != null && units.Any())
                {
                    var propertyPrices = units.Select(u => u.BasePrice).ToList();
                    allPrices.AddRange(propertyPrices);
                }
            }

            if (allPrices.Any())
            {
                searchFilters.PriceRange = new PriceRangeDto
                {
                    Min = allPrices.Min(),
                    Max = allPrices.Max(),
                    Currency = "YER" // العملة الافتراضية
                };
            }
            else
            {
                searchFilters.PriceRange = new PriceRangeDto
                {
                    Min = 0,
                    Max = 1000000,
                    Currency = "YER"
                };
            }

            _logger.LogDebug("تم تحديد نطاق الأسعار: {Min} - {Max} {Currency}", 
                searchFilters.PriceRange.Min, searchFilters.PriceRange.Max, searchFilters.PriceRange.Currency);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب نطاق الأسعار");
            searchFilters.PriceRange = new PriceRangeDto { Min = 0, Max = 1000000, Currency = "YER" };
        }
    }

    /// <summary>
    /// جلب أنواع العقارات المتاحة
    /// Populate property types
    /// </summary>
    /// <param name="searchFilters">فلاتر البحث</param>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task PopulatePropertyTypes(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var propertyTypeIds = properties.Select(p => p.PropertyTypeId).Distinct().ToList();
            var propertyTypes = await _propertyTypeRepository.GetByIdsAsync(propertyTypeIds, cancellationToken);

            if (propertyTypes != null && propertyTypes.Any())
            {
                searchFilters.PropertyTypes = propertyTypes.Select(pt => new PropertyTypeFilterDto
                {
                    Id = pt.Id,
                    Name = pt.Name ?? string.Empty,
                    Count = properties.Count(p => p.PropertyTypeId == pt.Id)
                })
                .OrderBy(pt => pt.Name)
                .ToList();
            }

            _logger.LogDebug("تم جلب {Count} نوع عقار متاح", searchFilters.PropertyTypes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب أنواع العقارات");
            searchFilters.PropertyTypes = new List<PropertyTypeFilterDto>();
        }
    }

    /// <summary>
    /// جلب وسائل الراحة المتاحة
    /// Populate amenities
    /// </summary>
    /// <param name="searchFilters">فلاتر البحث</param>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task PopulateAmenities(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var allAmenityIds = new HashSet<Guid>();

            // جمع جميع معرفات وسائل الراحة من العقارات
            foreach (var property in properties)
            {
                if (property.PropertyAmenities != null)
                {
                    foreach (var amenity in property.PropertyAmenities)
                    {
                        allAmenityIds.Add(amenity.AmenityId);
                    }
                }
            }

            if (allAmenityIds.Any())
            {
                var amenities = await _amenityRepository.GetByIdsAsync(allAmenityIds.ToList(), cancellationToken);
                
                if (amenities != null && amenities.Any())
                {
                    searchFilters.Amenities = amenities.Select(a => new AmenityFilterDto
                    {
                        Id = a.Id,
                        Name = a.Name ?? string.Empty,
                        Category = a.Category ?? string.Empty,
                        Count = properties.Count(p => p.PropertyAmenities != null && 
                                               p.PropertyAmenities.Any(pa => pa.AmenityId == a.Id))
                    })
                    .OrderBy(a => a.Category)
                    .ThenBy(a => a.Name)
                    .ToList();
                }
            }

            _logger.LogDebug("تم جلب {Count} وسيلة راحة متاحة", searchFilters.Amenities.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب وسائل الراحة");
            searchFilters.Amenities = new List<AmenityFilterDto>();
        }
    }

    /// <summary>
    /// جلب تصنيفات النجوم المتاحة
    /// Populate star ratings
    /// </summary>
    /// <param name="searchFilters">فلاتر البحث</param>
    /// <param name="properties">قائمة العقارات</param>
    private void PopulateStarRatings(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties)
    {
        try
        {
            var starRatings = properties
                .Where(p => p.StarRating.HasValue && p.StarRating.Value > 0)
                .Select(p => p.StarRating!.Value)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            if (!starRatings.Any())
            {
                // إضافة تصنيفات افتراضية إذا لم توجد
                starRatings = new List<int> { 1, 2, 3, 4, 5 };
            }

            searchFilters.StarRatings = starRatings;
            
            _logger.LogDebug("تم جلب {Count} تصنيف نجوم متاح", starRatings.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب تصنيفات النجوم");
            searchFilters.StarRatings = new List<int> { 1, 2, 3, 4, 5 };
        }
    }

    /// <summary>
    /// جلب الحد الأقصى للسعة
    /// Populate maximum guest capacity
    /// </summary>
    /// <param name="searchFilters">فلاتر البحث</param>
    /// <param name="properties">قائمة العقارات</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task PopulateMaxGuestCapacity(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var maxCapacities = new List<int>();

            foreach (var property in properties)
            {
                var units = await _unitRepository.GetActiveByPropertyIdAsync(property.Id, cancellationToken);
                if (units != null && units.Any())
                {
                    var propertyMaxCapacity = units.Max(u => u.MaxGuests);
                    maxCapacities.Add(propertyMaxCapacity);
                }
            }

            searchFilters.MaxGuestCapacity = maxCapacities.Any() ? maxCapacities.Max() : 10;
            
            _logger.LogDebug("تم تحديد الحد الأقصى للسعة: {MaxCapacity}", searchFilters.MaxGuestCapacity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب الحد الأقصى للسعة");
            searchFilters.MaxGuestCapacity = 10;
        }
    }

    /// <summary>
    /// الحصول على فلاتر افتراضية
    /// Get default filters
    /// </summary>
    /// <returns>فلاتر البحث الافتراضية</returns>
    private SearchFiltersDto GetDefaultFilters()
    {
        return new SearchFiltersDto
        {
            AvailableCities = new List<string> { "صنعاء", "عدن", "تعز", "الحديدة", "إب" },
            PriceRange = new PriceRangeDto
            {
                Min = 0,
                Max = 1000000,
                Currency = "YER"
            },
            PropertyTypes = new List<PropertyTypeFilterDto>(),
            Amenities = new List<AmenityFilterDto>(),
            StarRatings = new List<int> { 1, 2, 3, 4, 5 },
            MaxGuestCapacity = 10
        };
    }
}
