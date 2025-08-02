using MediatR;
using YBQuery = YemenBooking.Application.Queries.MobileApp.SearchFilters.GetSearchFiltersQuery;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.SearchFilters;

/// <summary>
/// معالج استعلام الحصول على فلاتر البحث المتاحة
/// Handler for get search filters query
/// </summary>
public class GetSearchFiltersQueryHandler : IRequestHandler<YBQuery, ResultDto<SearchFiltersDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyTypeRepository _propertyTypeRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitTypeRepository _unitTypeRepository;
    private readonly IPropertyServiceRepository _propertyServiceRepository;
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
        IUnitTypeRepository unitTypeRepository,
        IPropertyServiceRepository propertyServiceRepository,
        ILogger<GetSearchFiltersQueryHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _propertyTypeRepository = propertyTypeRepository;
        _amenityRepository = amenityRepository;
        _unitRepository = unitRepository;
        _unitTypeRepository = unitTypeRepository;
        _propertyServiceRepository = propertyServiceRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على فلاتر البحث المتاحة
    /// Handle get search filters query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>فلاتر البحث المتاحة</returns>
    public async Task<ResultDto<SearchFiltersDto>> Handle(YBQuery request, CancellationToken cancellationToken)
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
                activeProperties = activeProperties.Where(p => p.PropertyType?.Id == request.PropertyTypeId.Value);
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

            // جلب أنواع الوحدات المتاحة
            await PopulateUnitTypes(searchFilters, propertiesList, cancellationToken);

            // جلب الخدمات المتاحة
            PopulateServices(searchFilters, propertiesList);

            // جلب قيم الحقول الديناميكية المتاحة
            await PopulateDynamicFieldValues(searchFilters, propertiesList, cancellationToken);

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
                    var prices = units.Select(u => u.BasePrice.Amount).ToList();
                    allPrices.AddRange(prices);
                }
            }

            if (allPrices.Any())
            {
                searchFilters.PriceRange = new PriceRangeDto
                {
                    MinPrice = allPrices.Min(),
                    MaxPrice = allPrices.Max(),
                    AveragePrice = allPrices.Average()
                };
            }
            else
            {
                searchFilters.PriceRange = new PriceRangeDto
                {
                    MinPrice = 0,
                    MaxPrice = 1000000,
                    AveragePrice = 500000
                };
            }

            _logger.LogDebug("تم تحديد نطاق الأسعار: {MinPrice} - {MaxPrice}", 
                searchFilters.PriceRange.MinPrice, searchFilters.PriceRange.MaxPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب نطاق الأسعار");
            searchFilters.PriceRange = new PriceRangeDto { MinPrice = 0, MaxPrice = 1000000, AveragePrice = 500000 };
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
            var propertyTypeIds = properties.Select(p => p.PropertyType?.Id).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();
            var allPropertyTypes = await _propertyTypeRepository.GetAllAsync(cancellationToken);
            var propertyTypes = allPropertyTypes?.Where(pt => propertyTypeIds.Contains(pt.Id)).ToList();

            if (propertyTypes != null && propertyTypes.Any())
            {
                searchFilters.PropertyTypes = propertyTypes.Select(pt => new PropertyTypeFilterDto
                {
                    Id = pt.Id,
                    Name = pt.Name ?? string.Empty,
                    PropertiesCount = properties.Count(p => p.PropertyType?.Id == pt.Id)
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
            // تم تبسيط عملية جلب وسائل الراحة
            var allAmenities = await _amenityRepository.GetAllAsync(cancellationToken);
            
            if (allAmenities != null && allAmenities.Any())
            {
                searchFilters.Amenities = allAmenities.Select(a => new AmenityFilterDto
                {
                    Id = a.Id,
                    Name = a.Name ?? string.Empty,
                    Category = "عام",
                    PropertiesCount = properties.Count(p => p.Amenities.Any(pa => pa.PtaId == a.Id))
                })
                    .OrderBy(a => a.Name)
                    .ToList();
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
                .Where(p => p.StarRating != 0 && p.StarRating > 0)
                .Select(p => p.StarRating)
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
                    var propertyMaxCapacity = units.Max(u => u.MaxCapacity);
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
    /// جلب أنواع الوحدات المتاحة
    /// Populate unit types
    /// </summary>
    private async Task PopulateUnitTypes(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var allUnitTypeIds = new HashSet<Guid>();

            foreach (var property in properties)
            {
                var units = await _unitRepository.GetActiveByPropertyIdAsync(property.Id, cancellationToken);
                foreach (var unit in units)
                {
                    if (unit.UnitTypeId != Guid.Empty)
                    {
                        allUnitTypeIds.Add(unit.UnitTypeId);
                    }
                }
            }

            var unitTypes = await _unitTypeRepository.GetAllUnitTypesAsync(cancellationToken);

            searchFilters.UnitTypes = unitTypes?.Where(ut => allUnitTypeIds.Contains(ut.Id))
                .Select(ut => new UnitTypeFilterDto
                {
                    Id = ut.Id,
                    Name = ut.Name ?? string.Empty,
                    UnitsCount = properties.Sum(p => p.Units.Count(u => u.UnitTypeId == ut.Id))
                }).OrderBy(ut => ut.Name).ToList() ?? new List<UnitTypeFilterDto>();

            _logger.LogDebug("تم جلب {Count} نوع وحدة متاح", searchFilters.UnitTypes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب أنواع الوحدات");
            searchFilters.UnitTypes = new List<UnitTypeFilterDto>();
        }
    }

    /// <summary>
    /// جلب الخدمات المتاحة
    /// Populate services list
    /// </summary>
    private void PopulateServices(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties)
    {
        try
        {
            var serviceGroups = properties
                .Where(p => p.Services != null && p.Services.Any())
                .SelectMany(p => p.Services.Select(s => new { s.Id, s.Name, p.Id }))
                .GroupBy(x => new { x.Id, x.Name })
                .Select(g => new ServiceFilterDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name ?? string.Empty,
                    PropertiesCount = g.Select(x => x.Id).Distinct().Count()
                }).OrderBy(s => s.Name).ToList();

            searchFilters.Services = serviceGroups;

            _logger.LogDebug("تم جلب {Count} خدمة متاحة", searchFilters.Services.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب الخدمات");
            searchFilters.Services = new List<ServiceFilterDto>();
        }
    }

    /// <summary>
    /// جلب قيم الحقول الديناميكية المتاحة
    /// Populate dynamic field values
    /// </summary>
    private async Task PopulateDynamicFieldValues(SearchFiltersDto searchFilters, List<Core.Entities.Property> properties, CancellationToken cancellationToken)
    {
        try
        {
            var dynamicValueCounts = new Dictionary<(string field, string value), int>();

            foreach (var property in properties)
            {
                var units = await _unitRepository.GetActiveByPropertyIdAsync(property.Id, cancellationToken);
                foreach (var unit in units)
                {
                    if (unit.DynamicFields == null || !unit.DynamicFields.Any()) continue;

                    foreach (var kvp in unit.DynamicFields)
                    {
                        var key = (kvp.Key, kvp.Value?.ToString() ?? string.Empty);
                        if (dynamicValueCounts.ContainsKey(key))
                        {
                            dynamicValueCounts[key]++;
                        }
                        else
                        {
                            dynamicValueCounts[key] = 1;
                        }
                    }
                }
            }

            searchFilters.DynamicFieldValues = dynamicValueCounts.Select(d => new DynamicFieldValueFilterDto
            {
                FieldName = d.Key.field,
                Value = d.Key.value,
                Count = d.Value
            }).OrderBy(d => d.FieldName).ThenBy(d => d.Value).ToList();

            _logger.LogDebug("تم جلب {Count} قيمة حقل ديناميكى", searchFilters.DynamicFieldValues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء جلب قيم الحقول الديناميكية");
            searchFilters.DynamicFieldValues = new List<DynamicFieldValueFilterDto>();
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
                MinPrice = 0,
                MaxPrice = 1000000,
                AveragePrice = 500000
            },
            PropertyTypes = new List<PropertyTypeFilterDto>(),
            Amenities = new List<AmenityFilterDto>(),
            UnitTypes = new List<UnitTypeFilterDto>(),
            Services = new List<ServiceFilterDto>(),
            DynamicFieldValues = new List<DynamicFieldValueFilterDto>(),
            StarRatings = new List<int> { 1, 2, 3, 4, 5 },
            MaxGuestCapacity = 10
        };
    }
}
