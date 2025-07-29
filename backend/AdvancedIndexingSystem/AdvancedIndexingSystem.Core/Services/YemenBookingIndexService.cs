using AdvancedIndexingSystem.Core.Events;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace AdvancedIndexingSystem.Core.Services;

/// <summary>
/// خدمة الفهرسة المخصصة لنظام حجز العقارات اليمنية
/// Custom indexing service for Yemen Booking system
/// </summary>
public class YemenBookingIndexService : IYemenBookingIndexService
{
    #region الخصائص والحقول - Properties and Fields

    private readonly IAdvancedIndex<PropertyIndexItem> _propertyIndex;
    private readonly IAdvancedIndex<UnitIndexItem> _unitIndex;
    private readonly IAdvancedIndex<PricingIndexItem> _pricingIndex;
    private readonly IAdvancedIndex<AvailabilityIndexItem> _availabilityIndex;
    private readonly IAdvancedIndex<CityIndexItem> _cityIndex;
    private readonly IAdvancedIndex<AmenityIndexItem> _amenityIndex;

    /// <summary>
    /// فهارس الحقول الديناميكية - مفتاح هو اسم الحقل
    /// Dynamic field indexes - key is field name
    /// </summary>
    private readonly ConcurrentDictionary<string, IAdvancedIndex<Models.DynamicFieldIndexItem>> _dynamicFieldIndexes = new();

    /// <summary>
    /// كاش لتحسين الأداء
    /// Performance cache
    /// </summary>
    private readonly ConcurrentDictionary<string, object> _cache = new();

    /// <summary>
    /// قفل للعمليات المتزامنة
    /// Synchronization lock
    /// </summary>
    private readonly object _lockObject = new();

    /// <summary>
    /// هل تم التهيئة
    /// Is initialized
    /// </summary>
    public bool IsInitialized { get; private set; }

    #endregion

    #region الأحداث - Events

    /// <summary>
    /// حدث فهرسة العقار
    /// Property indexing event
    /// </summary>
    public event EventHandler<PropertyIndexingEventArgs>? PropertyIndexed;

    /// <summary>
    /// حدث فهرسة الوحدة
    /// Unit indexing event
    /// </summary>
    public event EventHandler<UnitIndexingEventArgs>? UnitIndexed;

    /// <summary>
    /// حدث فهرسة التسعير
    /// Pricing indexing event
    /// </summary>
    public event EventHandler<PricingIndexingEventArgs>? PricingIndexed;

    /// <summary>
    /// حدث فهرسة الإتاحة
    /// Availability indexing event
    /// </summary>
    public event EventHandler<AvailabilityIndexingEventArgs>? AvailabilityIndexed;

    /// <summary>
    /// حدث فهرسة الحقل الديناميكي
    /// Dynamic field indexing event
    /// </summary>
    public event EventHandler<DynamicFieldIndexingEventArgs>? DynamicFieldIndexed;

    /// <summary>
    /// حدث فهرسة نوع العقار
    /// Property type indexing event
    /// </summary>
    public event EventHandler<PropertyTypeIndexingEventArgs>? PropertyTypeIndexed;

    /// <summary>
    /// حدث فهرسة نوع الوحدة
    /// Unit type indexing event
    /// </summary>
    public event EventHandler<UnitTypeIndexingEventArgs>? UnitTypeIndexed;

    /// <summary>
    /// حدث فهرسة المدينة
    /// City indexing event
    /// </summary>
    public event EventHandler<CityIndexingEventArgs>? CityIndexed;

    /// <summary>
    /// حدث فهرسة المرافق مع العقار
    /// Facility property indexing event
    /// </summary>
    public event EventHandler<FacilityPropertyIndexingEventArgs>? FacilityPropertyIndexed;

    #endregion

    #region المنشئ - Constructor

    /// <summary>
    /// منشئ خدمة الفهرسة المخصصة
    /// Constructor for custom indexing service
    /// </summary>
    public YemenBookingIndexService(
        IAdvancedIndex<PropertyIndexItem> propertyIndex,
        IAdvancedIndex<UnitIndexItem> unitIndex,
        IAdvancedIndex<PricingIndexItem> pricingIndex,
        IAdvancedIndex<AvailabilityIndexItem> availabilityIndex,
        IAdvancedIndex<CityIndexItem> cityIndex,
        IAdvancedIndex<AmenityIndexItem> amenityIndex)
    {
        _propertyIndex = propertyIndex;
        _unitIndex = unitIndex;
        _pricingIndex = pricingIndex;
        _availabilityIndex = availabilityIndex;
        _cityIndex = cityIndex;
        _amenityIndex = amenityIndex;
    }

    #endregion

    #region التهيئة - Initialization

    /// <summary>
    /// تهيئة خدمة الفهرسة
    /// Initialize indexing service
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        try
        {
            lock (_lockObject)
            {
                if (IsInitialized)
                    return true;

                IsInitialized = true;
            }

            // تهيئة الفهارس المختلفة
            await Task.WhenAll(
                InitializePropertyIndexAsync(),
                InitializeUnitIndexAsync(),
                InitializePricingIndexAsync(),
                InitializeAvailabilityIndexAsync(),
                InitializeCityIndexAsync(),
                InitializeAmenityIndexAsync()
            );

            return true;
        }
        catch
        {
            IsInitialized = false;
            return false;
        }
    }

    private async Task InitializePropertyIndexAsync()
    {
        // تكوين فهرس العقارات
        // Property index configuration
    }

    private async Task InitializeUnitIndexAsync()
    {
        // تكوين فهرس الوحدات
        // Unit index configuration
    }

    private async Task InitializePricingIndexAsync()
    {
        // تكوين فهرس التسعير
        // Pricing index configuration
    }

    private async Task InitializeAvailabilityIndexAsync()
    {
        // تكوين فهرس الإتاحة
        // Availability index configuration
    }

    private async Task InitializeCityIndexAsync()
    {
        // تكوين فهرس المدن
        // City index configuration
    }

    private async Task InitializeAmenityIndexAsync()
    {
        // تكوين فهرس المرافق
        // Amenity index configuration
    }

    /// <summary>
    /// إنشاء أو الحصول على فهرس حقل ديناميكي
    /// Create or get dynamic field index
    /// </summary>
    private async Task<IAdvancedIndex<DynamicFieldIndexItem>> GetOrCreateDynamicFieldIndexAsync(string fieldName, string fieldType)
    {
        if (_dynamicFieldIndexes.TryGetValue(fieldName, out var existingIndex))
        {
            return existingIndex;
        }

        // إنشاء تكوين فهرس للحقل الديناميكي
        var config = new IndexConfiguration
        {
            IndexId = $"dynamic_field_{fieldName}",
            IndexName = $"DynamicField_{fieldName}",
            ArabicName = $"فهرس الحقل الديناميكي {fieldName}",
            Description = $"فهرس للحقل الديناميكي {fieldName} من النوع {fieldType}",
            IndexType = IndexType.Dynamic,
            Priority = IndexPriority.Medium
        };

        // إنشاء فهرس جديد للحقل الديناميكي
        var newIndex = new AdvancedIndexService<DynamicFieldIndexItem>(config);
        
        _dynamicFieldIndexes.TryAdd(fieldName, newIndex);
        return newIndex;
    }

    #endregion

    #region فهرسة العقارات - Property Indexing

    /// <summary>
    /// فهرسة عقار جديد أو محدث
    /// Index new or updated property
    /// </summary>
    public async Task<bool> IndexPropertyAsync(PropertyIndexItem property, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        try
        {
            bool result = operationType switch
            {
                UpdateOperationType.Add => await _propertyIndex.AddItemAsync(property.Id, property),
                UpdateOperationType.Update => await _propertyIndex.UpdateItemAsync(property.Id, property),
                UpdateOperationType.Remove => await _propertyIndex.RemoveItemAsync(property.Id),
                _ => false
            };

            if (result)
            {
                // تحديث فهرس المدن
                await UpdateCityIndexAsync(property.City, property.Id, operationType);

                // إثارة الحدث
                PropertyIndexed?.Invoke(this, new PropertyIndexingEventArgs(
                    Guid.Parse(property.Id), 
                    property.Name, 
                    property.City, 
                    property.PropertyType, 
                    operationType));

                // تحديث الكاش
                InvalidatePropertyCache(property.Id);
            }

            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة متعددة للعقارات
    /// Bulk property indexing
    /// </summary>
    public async Task<bool> BulkIndexPropertiesAsync(IEnumerable<PropertyIndexItem> properties, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        var tasks = properties.Select(p => IndexPropertyAsync(p, operationType));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    #endregion

    #region فهرسة الوحدات - Unit Indexing

    /// <summary>
    /// فهرسة وحدة جديدة أو محدثة مع الحقول الديناميكية
    /// Index new or updated unit with dynamic fields
    /// </summary>
    public async Task<bool> IndexUnitAsync(UnitIndexItem unit, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        try
        {
            bool result = operationType switch
            {
                UpdateOperationType.Add => await _unitIndex.AddItemAsync(unit.Id, unit),
                UpdateOperationType.Update => await _unitIndex.UpdateItemAsync(unit.Id, unit),
                UpdateOperationType.Remove => await _unitIndex.RemoveItemAsync(unit.Id),
                _ => false
            };

            if (result)
            {
                // فهرسة قواعد التسعير المرتبطة
                if (unit.PricingRules.Any())
                {
                    var pricingRules = unit.PricingRules.OfType<BulkPricingRuleData>();
                    await BulkIndexPricingRulesAsync(pricingRules, operationType);
                }

                // فهرسة الإتاحة المرتبطة
                if (unit.Availability.Any())
                {
                    var availabilityData = unit.Availability.OfType<BulkAvailabilityData>();
                    await BulkIndexAvailabilityAsync(availabilityData, operationType);
                }

                // فهرسة الحقول الديناميكية
                await IndexUnitDynamicFieldsAsync(unit.Id, unit.DynamicFields, operationType);

                // إثارة الحدث
                UnitIndexed?.Invoke(this, new UnitIndexingEventArgs(
                    Guid.Parse(unit.Id),
                    Guid.Parse(unit.PropertyId),
                    unit.Name,
                    unit.UnitType,
                    unit.BasePrice,
                    unit.Currency,
                    operationType)
                {
                    DynamicFields = unit.DynamicFields,
                    PricingRules = unit.PricingRules,
                    Availability = unit.Availability
                });

                // تحديث الكاش
                InvalidateUnitCache(unit.Id);
            }

            return result;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region فهرسة الحقول الديناميكية - Dynamic Fields Indexing

    /// <summary>
    /// فهرسة الحقول الديناميكية للوحدة
    /// Index unit dynamic fields
    /// </summary>
    private async Task<bool> IndexUnitDynamicFieldsAsync(string unitId, Dictionary<string, object> dynamicFields, UpdateOperationType operationType)
    {
        if (!dynamicFields.Any())
            return true;

        var tasks = new List<Task<bool>>();

        foreach (var field in dynamicFields)
        {
            tasks.Add(IndexDynamicFieldAsync(unitId, field.Key, field.Value, operationType));
        }

        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    /// <summary>
    /// فهرسة حقل ديناميكي واحد
    /// Index single dynamic field
    /// </summary>
    private async Task<bool> IndexDynamicFieldAsync(string unitId, string fieldName, object fieldValue, UpdateOperationType operationType)
    {
        try
        {
            var fieldType = fieldValue?.GetType().Name ?? "string";
            var fieldIndex = await GetOrCreateDynamicFieldIndexAsync(fieldName, fieldType);

            var dynamicFieldItem = new DynamicFieldIndexItem
            {
                Id = $"{unitId}_{fieldName}",
                UnitId = unitId,
                FieldName = fieldName,
                FieldValue = fieldValue?.ToString() ?? "",
                FieldType = fieldType,
                CreatedAt = DateTime.UtcNow
            };

            bool result = operationType switch
            {
                UpdateOperationType.Add => await fieldIndex.AddItemAsync(dynamicFieldItem.Id, dynamicFieldItem),
                UpdateOperationType.Update => await fieldIndex.UpdateItemAsync(dynamicFieldItem.Id, dynamicFieldItem),
                UpdateOperationType.Remove => await fieldIndex.RemoveItemAsync(dynamicFieldItem.Id),
                _ => false
            };

            if (result)
            {
                // إثارة حدث فهرسة الحقل الديناميكي
                DynamicFieldIndexed?.Invoke(this, new DynamicFieldIndexingEventArgs(
                    fieldName, fieldValue?.ToString() ?? "", fieldType, unitId, operationType));
            }

            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// البحث في الحقول الديناميكية
    /// Search dynamic fields
    /// </summary>
    public async Task<List<string>> SearchUnitsByDynamicFieldAsync(string fieldName, object fieldValue, string operatorType = "equals")
    {
        if (!_dynamicFieldIndexes.TryGetValue(fieldName, out var fieldIndex))
        {
            return new List<string>();
        }

        var searchRequest = new SearchRequest
        {
            Filters = new List<SearchFilter>
            {
                new SearchFilter { Field = "FieldValue", Value = fieldValue, Operator = operatorType }
            }
        };

        var searchResult = await fieldIndex.SearchAsync(searchRequest);
        return searchResult.Items.Select(item => item.UnitId).ToList();
    }

    /// <summary>
    /// البحث المتقدم في الحقول الديناميكية مع عدة شروط
    /// Advanced search in dynamic fields with multiple conditions
    /// </summary>
    public async Task<List<string>> SearchUnitsByMultipleDynamicFieldsAsync(Dictionary<string, object> fieldFilters)
    {
        if (!fieldFilters.Any())
            return new List<string>();

        var allMatchingUnits = new List<HashSet<string>>();

        foreach (var filter in fieldFilters)
        {
            var unitIds = await SearchUnitsByDynamicFieldAsync(filter.Key, filter.Value);
            allMatchingUnits.Add(new HashSet<string>(unitIds));
        }

        // تقاطع النتائج - الوحدات التي تطابق جميع الشروط
        var result = allMatchingUnits.First();
        foreach (var set in allMatchingUnits.Skip(1))
        {
            result.IntersectWith(set);
        }

        return result.ToList();
    }

    #endregion

    #region فهرسة التسعير - Pricing Indexing

    /// <summary>
    /// فهرسة قاعدة تسعير جديدة أو محدثة
    /// Index new or updated pricing rule
    /// </summary>
    public async Task<bool> IndexPricingRuleAsync(PricingIndexItem pricingRule, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        try
        {
            bool result = operationType switch
            {
                UpdateOperationType.Add => await _pricingIndex.AddItemAsync(pricingRule.Id, pricingRule),
                UpdateOperationType.Update => await _pricingIndex.UpdateItemAsync(pricingRule.Id, pricingRule),
                UpdateOperationType.Remove => await _pricingIndex.RemoveItemAsync(pricingRule.Id),
                _ => false
            };

            if (result)
            {
                // إثارة الحدث
                PricingIndexed?.Invoke(this, new PricingIndexingEventArgs(
                    Guid.Parse(pricingRule.Id),
                    Guid.Parse(pricingRule.UnitId),
                    pricingRule.PriceType,
                    pricingRule.PriceAmount,
                    pricingRule.Currency,
                    pricingRule.StartDate,
                    pricingRule.EndDate,
                    operationType));

                // تحديث الكاش
                InvalidatePricingCache(pricingRule.UnitId);
            }

            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة متعددة لقواعد التسعير
    /// Bulk pricing rules indexing
    /// </summary>
    public async Task<bool> BulkIndexPricingRulesAsync(IEnumerable<BulkPricingRuleData> pricingRules, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        var indexItems = pricingRules.Select(pr => new PricingIndexItem
        {
            Id = pr.Id.ToString(),
            UnitId = "", // Will be set by caller
            PriceType = pr.PriceType,
            PriceAmount = pr.PriceAmount,
            Currency = pr.Currency,
            StartDate = pr.StartDate,
            EndDate = pr.EndDate,
            PricingTier = pr.PricingTier,
            PercentageChange = pr.PercentageChange
        });

        var tasks = indexItems.Select(item => IndexPricingRuleAsync(item, operationType));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    #endregion

    #region فهرسة الإتاحة - Availability Indexing

    /// <summary>
    /// فهرسة إتاحة جديدة أو محدثة
    /// Index new or updated availability
    /// </summary>
    public async Task<bool> IndexAvailabilityAsync(AvailabilityIndexItem availability, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        try
        {
            bool result = operationType switch
            {
                UpdateOperationType.Add => await _availabilityIndex.AddItemAsync(availability.Id, availability),
                UpdateOperationType.Update => await _availabilityIndex.UpdateItemAsync(availability.Id, availability),
                UpdateOperationType.Remove => await _availabilityIndex.RemoveItemAsync(availability.Id),
                _ => false
            };

            if (result)
            {
                // إثارة الحدث
                AvailabilityIndexed?.Invoke(this, new AvailabilityIndexingEventArgs(
                    Guid.Parse(availability.Id),
                    Guid.Parse(availability.UnitId),
                    availability.StartDate,
                    availability.EndDate,
                    availability.Status,
                    operationType));

                // تحديث الكاش
                InvalidateAvailabilityCache(availability.UnitId);
            }

            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة متعددة للإتاحة
    /// Bulk availability indexing
    /// </summary>
    public async Task<bool> BulkIndexAvailabilityAsync(IEnumerable<BulkAvailabilityData> availabilities, UpdateOperationType operationType = UpdateOperationType.Add)
    {
        var indexItems = availabilities.Select(av => new AvailabilityIndexItem
        {
            Id = av.Id.ToString(),
            UnitId = "", // Will be set by caller
            StartDate = av.StartDate,
            EndDate = av.EndDate,
            Status = av.Status,
            Reason = av.Reason
        });

        var tasks = indexItems.Select(item => IndexAvailabilityAsync(item, operationType));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    #endregion

    #region فهرسة المدن - City Indexing

    /// <summary>
    /// تحديث فهرس المدن
    /// Update city index
    /// </summary>
    private async Task<bool> UpdateCityIndexAsync(string cityName, string propertyId, UpdateOperationType operationType)
    {
        try
        {
            var cityItem = await GetOrCreateCityIndexItemAsync(cityName);
            
            switch (operationType)
            {
                case UpdateOperationType.Add:
                    if (!cityItem.PropertyIds.Contains(propertyId))
                        cityItem.PropertyIds.Add(propertyId);
                    break;
                case UpdateOperationType.Remove:
                    cityItem.PropertyIds.Remove(propertyId);
                    break;
            }

            return await _cityIndex.UpdateItemAsync(cityItem.Id, cityItem);
        }
        catch
        {
            return false;
        }
    }

    private async Task<CityIndexItem> GetOrCreateCityIndexItemAsync(string cityName)
    {
        var searchRequest = new SearchRequest
        {
            Query = cityName,
            Filters = new List<SearchFilter>
            {
                new SearchFilter { Field = "CityName", Value = cityName, Operator = "equals" }
            }
        };

        var searchResult = await _cityIndex.SearchAsync(searchRequest);
        
        if (searchResult.Items.Any())
        {
            return searchResult.Items.First();
        }

        // إنشاء عنصر جديد للمدينة
        var newCityItem = new Models.CityIndexItem
        {
            Id = Guid.NewGuid().ToString(),
            CityName = cityName,
            PropertyIds = new List<string>(),
            LastUpdated = DateTime.UtcNow
        };

        await _cityIndex.AddItemAsync(newCityItem.Id, newCityItem);
        return newCityItem;
    }

    #endregion

    #region البحث المتقدم - Advanced Search

    /// <summary>
    /// البحث المتقدم في العقارات مع تقاطع الفهارس
    /// Advanced property search with index intersection
    /// </summary>
    public async Task<SearchResult<PropertyIndexItem>> SearchPropertiesAsync(PropertySearchRequest request)
    {
        var filters = new List<SearchFilter>();

        // فلترة حسب المدينة
        if (!string.IsNullOrEmpty(request.City))
        {
            filters.Add(new SearchFilter { Field = "City", Value = request.City, Operator = "equals" });
        }

        // فلترة حسب نوع العقار
        if (!string.IsNullOrEmpty(request.PropertyType))
        {
            filters.Add(new SearchFilter { Field = "PropertyType", Value = request.PropertyType, Operator = "equals" });
        }

        // فلترة حسب النطاق السعري
        if (request.MinPrice.HasValue)
        {
            filters.Add(new SearchFilter { Field = "MinPrice", Value = request.MinPrice.Value, Operator = "gte" });
        }

        if (request.MaxPrice.HasValue)
        {
            filters.Add(new SearchFilter { Field = "MaxPrice", Value = request.MaxPrice.Value, Operator = "lte" });
        }

        // فلترة حسب التقييم
        if (request.MinRating.HasValue)
        {
            filters.Add(new SearchFilter { Field = "AverageRating", Value = request.MinRating.Value, Operator = "gte" });
        }

        var searchRequest = new SearchRequest
        {
            Query = request.Query ?? "",
            Filters = filters,
            SortBy = request.SortBy ?? "CreatedAt",
            SortOrder = request.SortOrder ?? "desc",
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return await _propertyIndex.SearchAsync(searchRequest);
    }

    /// <summary>
    /// البحث في الوحدات مع فلترة الإتاحة والتسعير
    /// Search units with availability and pricing filtering
    /// </summary>
    public async Task<SearchResult<UnitIndexItem>> SearchUnitsAsync(UnitSearchRequest request)
    {
        var filters = new List<SearchFilter>();

        // فلترة حسب العقار
        if (request.PropertyId.HasValue)
        {
            filters.Add(new SearchFilter { Field = "PropertyId", Value = request.PropertyId.Value.ToString(), Operator = "equals" });
        }

        // فلترة حسب نوع الوحدة
        if (!string.IsNullOrEmpty(request.UnitType))
        {
            filters.Add(new SearchFilter { Field = "UnitType", Value = request.UnitType, Operator = "equals" });
        }

        // فلترة حسب السعر
        if (request.MinPrice.HasValue)
        {
            filters.Add(new SearchFilter { Field = "BasePrice", Value = request.MinPrice.Value, Operator = "gte" });
        }

        if (request.MaxPrice.HasValue)
        {
            filters.Add(new SearchFilter { Field = "BasePrice", Value = request.MaxPrice.Value, Operator = "lte" });
        }

        // فلترة حسب السعة
        if (request.MinCapacity.HasValue)
        {
            filters.Add(new SearchFilter { Field = "MaxCapacity", Value = request.MinCapacity.Value, Operator = "gte" });
        }

        // فلترة حسب الإتاحة
        if (request.CheckInDate.HasValue && request.CheckOutDate.HasValue)
        {
            // البحث في فهرس الإتاحة للوحدات المتاحة
            var availableUnits = await GetAvailableUnitsAsync(request.CheckInDate.Value, request.CheckOutDate.Value);
            if (availableUnits.Any())
            {
                filters.Add(new SearchFilter { Field = "Id", Value = availableUnits, Operator = "in" });
            }
            else
            {
                // لا توجد وحدات متاحة
                return new SearchResult<UnitIndexItem> { Items = new List<UnitIndexItem>(), TotalCount = 0 };
            }
        }

        // فلترة حسب الحقول الديناميكية
        List<string>? dynamicFieldFilteredUnits = null;
        if (request.DynamicFieldFilters != null && request.DynamicFieldFilters.Any())
        {
            dynamicFieldFilteredUnits = await SearchUnitsByMultipleDynamicFieldsAsync(request.DynamicFieldFilters);
            if (dynamicFieldFilteredUnits.Any())
            {
                filters.Add(new SearchFilter { Field = "Id", Value = dynamicFieldFilteredUnits, Operator = "in" });
            }
            else
            {
                // لا توجد وحدات تطابق الحقول الديناميكية
                return new SearchResult<UnitIndexItem> { Items = new List<UnitIndexItem>(), TotalCount = 0 };
            }
        }

        var searchRequest = new SearchRequest
        {
            Query = request.Query ?? "",
            Filters = filters,
            SortBy = request.SortBy ?? "BasePrice",
            SortOrder = request.SortOrder ?? "asc",
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return await _unitIndex.SearchAsync(searchRequest);
    }

    /// <summary>
    /// الحصول على الوحدات المتاحة في فترة محددة
    /// Get available units for specific period
    /// </summary>
    private async Task<List<string>> GetAvailableUnitsAsync(DateTime checkIn, DateTime checkOut)
    {
        // البحث في فهرس الإتاحة عن الوحدات غير المتاحة في الفترة المحددة
        var unavailableSearchRequest = new SearchRequest
        {
            Filters = new List<SearchFilter>
            {
                new SearchFilter { Field = "Status", Value = "unavailable", Operator = "equals" },
                new SearchFilter { Field = "StartDate", Value = checkOut, Operator = "lt" },
                new SearchFilter { Field = "EndDate", Value = checkIn, Operator = "gt" }
            }
        };

        var unavailableResult = await _availabilityIndex.SearchAsync(unavailableSearchRequest);
        var unavailableUnitIds = unavailableResult.Items.Select(a => a.UnitId).Distinct().ToList();

        // البحث في جميع الوحدات واستبعاد غير المتاحة
        var allUnitsRequest = new SearchRequest
        {
            Filters = new List<SearchFilter>()
        };

        var allUnitsResult = await _unitIndex.SearchAsync(allUnitsRequest);
        var availableUnitIds = allUnitsResult.Items
            .Where(u => !unavailableUnitIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToList();

        return availableUnitIds;
    }

    #endregion

    #region إدارة الكاش - Cache Management

    private void InvalidatePropertyCache(string propertyId)
    {
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith($"property_{propertyId}")).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    private void InvalidateUnitCache(string unitId)
    {
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith($"unit_{unitId}")).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    private void InvalidatePricingCache(string unitId)
    {
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith($"pricing_{unitId}")).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    private void InvalidateAvailabilityCache(string unitId)
    {
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith($"availability_{unitId}")).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    #endregion

    #region حفظ وتحميل الفهارس - Save and Load Indexes

    /// <summary>
    /// حفظ جميع الفهارس إلى ملفات
    /// Save all indexes to files
    /// </summary>
    public async Task<bool> SaveAllIndexesAsync(string basePath)
    {
        try
        {
            var tasks = new[]
            {
                _propertyIndex.SaveToFileAsync(Path.Combine(basePath, "property_index.json")),
                _unitIndex.SaveToFileAsync(Path.Combine(basePath, "unit_index.json")),
                _pricingIndex.SaveToFileAsync(Path.Combine(basePath, "pricing_index.json")),
                _availabilityIndex.SaveToFileAsync(Path.Combine(basePath, "availability_index.json")),
                _cityIndex.SaveToFileAsync(Path.Combine(basePath, "city_index.json")),
                _amenityIndex.SaveToFileAsync(Path.Combine(basePath, "amenity_index.json"))
            };

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// تحميل جميع الفهارس من الملفات
    /// Load all indexes from files
    /// </summary>
    public async Task<bool> LoadAllIndexesAsync(string basePath)
    {
        try
        {
            var tasks = new[]
            {
                _propertyIndex.LoadFromFileAsync(Path.Combine(basePath, "property_index.json")),
                _unitIndex.LoadFromFileAsync(Path.Combine(basePath, "unit_index.json")),
                _pricingIndex.LoadFromFileAsync(Path.Combine(basePath, "pricing_index.json")),
                _availabilityIndex.LoadFromFileAsync(Path.Combine(basePath, "availability_index.json")),
                _cityIndex.LoadFromFileAsync(Path.Combine(basePath, "city_index.json")),
                _amenityIndex.LoadFromFileAsync(Path.Combine(basePath, "amenity_index.json"))
            };

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
        catch
        {
            return false;
        }
    }

        #region الوظائف المتقدمة - Advanced Functions

    /// <summary>
    /// فهرسة الحقل الديناميكي
    /// Index dynamic field
    /// </summary>
    public async Task<bool> IndexDynamicFieldAsync(Models.DynamicFieldIndexItem dynamicField)
    {
        try
        {
            // استخدام اسم الحقل كمعرف للفهرس
            var fieldIndexName = $"dynamic_field_{dynamicField.FieldName}";
            
            if (!_dynamicFieldIndexes.ContainsKey(fieldIndexName))
            {
                await EnsureDynamicFieldIndexExistsAsync(dynamicField.FieldName, dynamicField.FieldType);
            }

            if (_dynamicFieldIndexes.TryGetValue(fieldIndexName, out var fieldIndex))
            {
                var result = await fieldIndex.AddItemAsync(dynamicField.Id, dynamicField);
                
                if (result)
                {
                    DynamicFieldIndexed?.Invoke(this, new DynamicFieldIndexingEventArgs(
                        dynamicField.FieldName, 
                        dynamicField.FieldValue, 
                        dynamicField.FieldType, 
                        dynamicField.EntityId, 
                        UpdateOperationType.Add));
                }
                
                return result;
            }
            
            return false;
        }
        catch
        {
            // Log error
            return false;
        }
    }

    /// <summary>
    /// حذف فهرس الحقل الديناميكي
    /// Remove dynamic field index
    /// </summary>
    public async Task<bool> RemoveDynamicFieldIndexAsync(string fieldId)
    {
        try
        {
            foreach (var fieldIndex in _dynamicFieldIndexes.Values)
            {
                if (await fieldIndex.RemoveItemAsync(fieldId))
                {
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ضمان وجود فهرس الحقل الديناميكي
    /// Ensure dynamic field index exists
    /// </summary>
    public async Task<bool> EnsureDynamicFieldIndexExistsAsync(string fieldName, string fieldType)
    {
        try
        {
            var fieldIndexName = $"dynamic_field_{fieldName}";
            
            if (!_dynamicFieldIndexes.ContainsKey(fieldIndexName))
            {
                var config = new IndexConfiguration
                {
                    IndexId = fieldIndexName,
                    IndexName = $"Dynamic Field Index for {fieldName}",
                    Description = $"فهرس الحقل الديناميكي: {fieldName}",
                    IndexType = IndexType.DynamicFieldIndex,
                    MaxItems = 1000000
                };

                var newIndex = new AdvancedIndexService<Models.DynamicFieldIndexItem>(config);
                
                _dynamicFieldIndexes.TryAdd(fieldIndexName, newIndex);
                return true;
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة التسعير
    /// Index pricing
    /// </summary>
    public async Task<bool> IndexPricingAsync(PricingIndexItem pricingItem)
    {
        try
        {
            var result = await _pricingIndex.AddItemAsync(pricingItem.Id, pricingItem);
            
            if (result)
            {
                PricingIndexed?.Invoke(this, new PricingIndexingEventArgs(
                    Guid.Parse(pricingItem.Id),
                    Guid.Parse(pricingItem.UnitId),
                    pricingItem.PriceType,
                    pricingItem.PriceAmount,
                    pricingItem.Currency,
                    pricingItem.StartDate,
                    pricingItem.EndDate,
                    UpdateOperationType.Add));
            }
            
            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// حذف فهرس التسعير
    /// Remove pricing index
    /// </summary>
    public async Task<bool> RemovePricingIndexAsync(string pricingRuleId)
    {
        try
        {
            return await _pricingIndex.RemoveItemAsync(pricingRuleId);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة العملة
    /// Index currency
    /// </summary>
    public async Task<bool> IndexCurrencyAsync(string currency, decimal amount)
    {
        try
        {
            // يمكن إضافة فهرس خاص بالعملات في المستقبل
            // For now, this is a placeholder for currency indexing
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// حذف فهرس الإتاحة
    /// Remove availability index
    /// </summary>
    public async Task<bool> RemoveAvailabilityIndexAsync(string availabilityId)
    {
        try
        {
            return await _availabilityIndex.RemoveItemAsync(availabilityId);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// إضافة عقار لفهرس المدينة
    /// Add property to city index
    /// </summary>
    public async Task<bool> AddPropertyToCityIndexAsync(string cityName, string propertyId)
    {
        try
        {
            var cityIndexItem = new Models.CityIndexItem
            {
                Id = cityName,
                CityName = cityName,
                PropertyIds = new List<string> { propertyId },
                LastUpdated = DateTime.UtcNow
            };

            var result = await _cityIndex.AddItemAsync(cityName, cityIndexItem);
            
            if (result)
            {
                CityIndexed?.Invoke(this, new CityIndexingEventArgs(
                    Guid.NewGuid(),
                    cityName,
                    UpdateOperationType.Add));
            }
            
            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// حذف عقار من فهرس المدينة
    /// Remove property from city index
    /// </summary>
    public async Task<bool> RemovePropertyFromCityIndexAsync(string cityName, string propertyId)
    {
        try
        {
            // Implementation for removing property from city index
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// تحديث إحصائيات المدينة
    /// Update city statistics
    /// </summary>
    public async Task<bool> UpdateCityStatisticsAsync(string cityName, Dictionary<string, object> stats)
    {
        try
        {
            // Implementation for updating city statistics
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ربط العقار بالمرفق
    /// Associate property with facility
    /// </summary>
    public async Task<bool> AssociatePropertyWithFacilityAsync(string propertyId, string facilityName, Dictionary<string, object> facilityData)
    {
        try
        {
            var amenityItem = new Models.AmenityIndexItem
            {
                Id = facilityName,
                AmenityName = facilityName,
                PropertyIds = new List<string> { propertyId },
                AmenityData = facilityData,
                LastUpdated = DateTime.UtcNow
            };

            var result = await _amenityIndex.AddItemAsync(facilityName, amenityItem);
            
            if (result)
            {
                FacilityPropertyIndexed?.Invoke(this, new FacilityPropertyIndexingEventArgs(
                    Guid.Parse(propertyId),
                    Guid.NewGuid(),
                    facilityName,
                    UpdateOperationType.Add));
            }
            
            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// إلغاء ربط العقار بالمرفق
    /// Dissociate property from facility
    /// </summary>
    public async Task<bool> DissociatePropertyFromFacilityAsync(string propertyId, string facilityName)
    {
        try
        {
            // Implementation for dissociating property from facility
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// تحديث بيانات مرفق العقار
    /// Update property facility data
    /// </summary>
    public async Task<bool> UpdatePropertyFacilityDataAsync(string propertyId, string facilityName, Dictionary<string, object> facilityData)
    {
        try
        {
            // Implementation for updating property facility data
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة نوع العقار
    /// Index property type
    /// </summary>
    public async Task<bool> IndexPropertyTypeAsync(PropertyTypeIndexItem propertyType)
    {
        try
        {
            // Implementation for indexing property type
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// حذف فهرس نوع العقار
    /// Remove property type index
    /// </summary>
    public async Task<bool> RemovePropertyTypeIndexAsync(string propertyTypeId)
    {
        try
        {
            // Implementation for removing property type index
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// فهرسة نوع الوحدة
    /// Index unit type
    /// </summary>
    public async Task<bool> IndexUnitTypeAsync(UnitTypeIndexItem unitType)
    {
        try
        {
            // Implementation for indexing unit type
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// حذف فهرس نوع الوحدة
    /// Remove unit type index
    /// </summary>
    public async Task<bool> RemoveUnitTypeIndexAsync(string unitTypeId)
    {
        try
        {
            // Implementation for removing unit type index
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// حفظ الفهارس
    /// Save indexes
    /// </summary>
    public async Task<bool> SaveIndexesAsync()
    {
        try
        {
            var tasks = new List<Task<bool>>();
            
            // حفظ الفهارس الأساسية
            tasks.Add(_propertyIndex.SaveToFileAsync("property_index.json"));
            tasks.Add(_unitIndex.SaveToFileAsync("unit_index.json"));
            tasks.Add(_pricingIndex.SaveToFileAsync("pricing_index.json"));
            tasks.Add(_availabilityIndex.SaveToFileAsync("availability_index.json"));
            tasks.Add(_cityIndex.SaveToFileAsync("city_index.json"));
            tasks.Add(_amenityIndex.SaveToFileAsync("amenity_index.json"));

            // حفظ الفهارس الديناميكية
            foreach (var kvp in _dynamicFieldIndexes)
            {
                tasks.Add(kvp.Value.SaveToFileAsync($"dynamic_field_{kvp.Key}.json"));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
        catch
        {
            return false;
        }
    }

    #endregion
}

#endregion

#region نماذج عناصر الفهرس - Index Item Models

/// <summary>
/// عنصر فهرس العقار
/// Property index item
/// </summary>
public class PropertyIndexItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int StarRating { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
    public int BookingCount { get; set; }
    public decimal AverageRating { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public List<string> AmenityIds { get; set; } = new();
    public List<string> UnitIds { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// عنصر فهرس الوحدة
/// Unit index item
/// </summary>
public class UnitIndexItem
{
    public string Id { get; set; } = string.Empty;
    public string PropertyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public bool IsAvailable { get; set; }
    public int ViewCount { get; set; }
    public int BookingCount { get; set; }
    public Dictionary<string, object> DynamicFields { get; set; } = new();
    public List<object> PricingRules { get; set; } = new();
    public List<object> Availability { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public string CustomFeatures { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
}



#endregion

#region نماذج طلبات البحث - Search Request Models

/// <summary>
/// طلب البحث في العقارات
/// Property search request
/// </summary>
public class PropertySearchRequest
{
    public string? Query { get; set; }
    public string? City { get; set; }
    public string? PropertyType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinRating { get; set; }
    public List<string>? AmenityIds { get; set; }
    /// <summary>
    /// تاريخ الوصول للفترة المطلوبة
    /// Check-in date for availability filtering
    /// </summary>
    public DateTime? CheckInDate { get; set; }

    /// <summary>
    /// تاريخ المغادرة للفترة المطلوبة
    /// Check-out date for availability filtering
    /// </summary>
    public DateTime? CheckOutDate { get; set; }

    /// <summary>
    /// عدد الضيوف للفترة المطلوبة
    /// Guests count for availability filtering
    /// </summary>
    public int? GuestsCount { get; set; }

    /// <summary>
    /// فلاتر الحقول الديناميكية (مفتاح-قيمة)
    /// Dynamic field filters
    /// </summary>
    public Dictionary<string, object>? DynamicFieldFilters { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// طلب البحث في الوحدات
/// Unit search request
/// </summary>
public class UnitSearchRequest
{
    public string? Query { get; set; }
    public Guid? PropertyId { get; set; }
    public string? UnitType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinCapacity { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public Dictionary<string, object>? DynamicFieldFilters { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

#endregion