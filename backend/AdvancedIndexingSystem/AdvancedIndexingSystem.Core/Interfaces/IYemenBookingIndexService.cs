using AdvancedIndexingSystem.Core.Events;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Services;

namespace AdvancedIndexingSystem.Core.Interfaces;

/// <summary>
/// واجهة خدمة الفهرسة المخصصة لنظام حجز العقارات اليمنية
/// Interface for Yemen Booking custom indexing service
/// </summary>
public interface IYemenBookingIndexService
{
    #region الخصائص - Properties

    /// <summary>
    /// هل تم التهيئة
    /// Is initialized
    /// </summary>
    bool IsInitialized { get; }

    #endregion

    #region الأحداث - Events

    /// <summary>
    /// حدث فهرسة العقار
    /// Property indexing event
    /// </summary>
    event EventHandler<PropertyIndexingEventArgs>? PropertyIndexed;

    /// <summary>
    /// حدث فهرسة الوحدة
    /// Unit indexing event
    /// </summary>
    event EventHandler<UnitIndexingEventArgs>? UnitIndexed;

    /// <summary>
    /// حدث فهرسة التسعير
    /// Pricing indexing event
    /// </summary>
    event EventHandler<PricingIndexingEventArgs>? PricingIndexed;

    /// <summary>
    /// حدث فهرسة الإتاحة
    /// Availability indexing event
    /// </summary>
    event EventHandler<AvailabilityIndexingEventArgs>? AvailabilityIndexed;

    /// <summary>
    /// حدث فهرسة الحقل الديناميكي
    /// Dynamic field indexing event
    /// </summary>
    event EventHandler<DynamicFieldIndexingEventArgs>? DynamicFieldIndexed;

    /// <summary>
    /// حدث فهرسة نوع العقار
    /// Property type indexing event
    /// </summary>
    event EventHandler<PropertyTypeIndexingEventArgs>? PropertyTypeIndexed;

    /// <summary>
    /// حدث فهرسة نوع الوحدة
    /// Unit type indexing event
    /// </summary>
    event EventHandler<UnitTypeIndexingEventArgs>? UnitTypeIndexed;

    /// <summary>
    /// حدث فهرسة المدينة
    /// City indexing event
    /// </summary>
    event EventHandler<CityIndexingEventArgs>? CityIndexed;

    /// <summary>
    /// حدث فهرسة المرافق مع العقار
    /// Facility property indexing event
    /// </summary>
    event EventHandler<FacilityPropertyIndexingEventArgs>? FacilityPropertyIndexed;

    #endregion

    #region التهيئة - Initialization

    /// <summary>
    /// تهيئة خدمة الفهرسة
    /// Initialize indexing service
    /// </summary>
    Task<bool> InitializeAsync();

    #endregion

    #region فهرسة العقارات - Property Indexing

    /// <summary>
    /// فهرسة عقار جديد أو محدث
    /// Index new or updated property
    /// </summary>
    Task<bool> IndexPropertyAsync(PropertyIndexItem property, UpdateOperationType operationType = UpdateOperationType.Add);

    /// <summary>
    /// فهرسة متعددة للعقارات
    /// Bulk property indexing
    /// </summary>
    Task<bool> BulkIndexPropertiesAsync(IEnumerable<PropertyIndexItem> properties, UpdateOperationType operationType = UpdateOperationType.Add);

    #endregion

    #region فهرسة الوحدات - Unit Indexing

    /// <summary>
    /// فهرسة وحدة جديدة أو محدثة مع الحقول الديناميكية
    /// Index new or updated unit with dynamic fields
    /// </summary>
    Task<bool> IndexUnitAsync(UnitIndexItem unit, UpdateOperationType operationType = UpdateOperationType.Add);

    #endregion

    #region فهرسة التسعير - Pricing Indexing

    /// <summary>
    /// فهرسة قاعدة تسعير جديدة أو محدثة
    /// Index new or updated pricing rule
    /// </summary>
    Task<bool> IndexPricingRuleAsync(Models.PricingIndexItem pricingRule, UpdateOperationType operationType = UpdateOperationType.Add);

    /// <summary>
    /// فهرسة متعددة لقواعد التسعير
    /// Bulk pricing rules indexing
    /// </summary>
    Task<bool> BulkIndexPricingRulesAsync(IEnumerable<BulkPricingRuleData> pricingRules, UpdateOperationType operationType = UpdateOperationType.Add);

    /// <summary>
    /// فهرسة التسعير
    /// Index pricing
    /// </summary>
    Task<bool> IndexPricingAsync(Models.PricingIndexItem pricingItem);

    /// <summary>
    /// حذف فهرس التسعير
    /// Remove pricing index
    /// </summary>
    Task<bool> RemovePricingIndexAsync(string pricingRuleId);

    /// <summary>
    /// فهرسة العملة
    /// Index currency
    /// </summary>
    Task<bool> IndexCurrencyAsync(string currency, decimal amount);

    #endregion

    #region فهرسة الإتاحة - Availability Indexing

    /// <summary>
    /// فهرسة إتاحة جديدة أو محدثة
    /// Index new or updated availability
    /// </summary>
    Task<bool> IndexAvailabilityAsync(Models.AvailabilityIndexItem availability, UpdateOperationType operationType = UpdateOperationType.Add);

    /// <summary>
    /// فهرسة متعددة للإتاحة
    /// Bulk availability indexing
    /// </summary>
    Task<bool> BulkIndexAvailabilityAsync(IEnumerable<BulkAvailabilityData> availabilities, UpdateOperationType operationType = UpdateOperationType.Add);

    /// <summary>
    /// حذف فهرس الإتاحة
    /// Remove availability index
    /// </summary>
    Task<bool> RemoveAvailabilityIndexAsync(string availabilityId);

    #endregion

    #region فهرسة الحقول الديناميكية - Dynamic Fields Indexing

    /// <summary>
    /// فهرسة الحقل الديناميكي
    /// Index dynamic field
    /// </summary>
    Task<bool> IndexDynamicFieldAsync(Models.DynamicFieldIndexItem dynamicField);

    /// <summary>
    /// حذف فهرس الحقل الديناميكي
    /// Remove dynamic field index
    /// </summary>
    Task<bool> RemoveDynamicFieldIndexAsync(string fieldId);

    /// <summary>
    /// ضمان وجود فهرس الحقل الديناميكي
    /// Ensure dynamic field index exists
    /// </summary>
    Task<bool> EnsureDynamicFieldIndexExistsAsync(string fieldName, string fieldType);

    #endregion

    #region فهرسة المدن - City Indexing

    /// <summary>
    /// إضافة عقار لفهرس المدينة
    /// Add property to city index
    /// </summary>
    Task<bool> AddPropertyToCityIndexAsync(string cityName, string propertyId);

    /// <summary>
    /// حذف عقار من فهرس المدينة
    /// Remove property from city index
    /// </summary>
    Task<bool> RemovePropertyFromCityIndexAsync(string cityName, string propertyId);

    /// <summary>
    /// تحديث إحصائيات المدينة
    /// Update city statistics
    /// </summary>
    Task<bool> UpdateCityStatisticsAsync(string cityName, Dictionary<string, object> stats);

    #endregion

    #region فهرسة المرافق - Amenity Indexing

    /// <summary>
    /// ربط العقار بالمرفق
    /// Associate property with facility
    /// </summary>
    Task<bool> AssociatePropertyWithFacilityAsync(string propertyId, string facilityName, Dictionary<string, object> facilityData);

    /// <summary>
    /// إلغاء ربط العقار بالمرفق
    /// Dissociate property from facility
    /// </summary>
    Task<bool> DissociatePropertyFromFacilityAsync(string propertyId, string facilityName);

    /// <summary>
    /// تحديث بيانات مرفق العقار
    /// Update property facility data
    /// </summary>
    Task<bool> UpdatePropertyFacilityDataAsync(string propertyId, string facilityName, Dictionary<string, object> facilityData);

    #endregion

    #region فهرسة أنواع العقارات والوحدات - Property and Unit Types Indexing

    /// <summary>
    /// فهرسة نوع العقار
    /// Index property type
    /// </summary>
    Task<bool> IndexPropertyTypeAsync(PropertyTypeIndexItem propertyType);

    /// <summary>
    /// حذف فهرس نوع العقار
    /// Remove property type index
    /// </summary>
    Task<bool> RemovePropertyTypeIndexAsync(string propertyTypeId);

    /// <summary>
    /// فهرسة نوع الوحدة
    /// Index unit type
    /// </summary>
    Task<bool> IndexUnitTypeAsync(UnitTypeIndexItem unitType);

    /// <summary>
    /// حذف فهرس نوع الوحدة
    /// Remove unit type index
    /// </summary>
    Task<bool> RemoveUnitTypeIndexAsync(string unitTypeId);

    #endregion

    #region البحث المتقدم - Advanced Search

    /// <summary>
    /// البحث المتقدم في العقارات مع تقاطع الفهارس
    /// Advanced property search with index intersection
    /// </summary>
    Task<SearchResult<PropertyIndexItem>> SearchPropertiesAsync(PropertySearchRequest request);

    /// <summary>
    /// البحث في الوحدات مع فلترة الإتاحة والتسعير
    /// Search units with availability and pricing filtering
    /// </summary>
    Task<SearchResult<UnitIndexItem>> SearchUnitsAsync(UnitSearchRequest request);

    #endregion

    #region حفظ وتحميل الفهارس - Save and Load Indexes

    /// <summary>
    /// حفظ جميع الفهارس إلى ملفات
    /// Save all indexes to files
    /// </summary>
    Task<bool> SaveAllIndexesAsync(string basePath);

    /// <summary>
    /// تحميل جميع الفهارس من الملفات
    /// Load all indexes from files
    /// </summary>
    Task<bool> LoadAllIndexesAsync(string basePath);

    /// <summary>
    /// حفظ الفهارس
    /// Save indexes
    /// </summary>
    Task<bool> SaveIndexesAsync();

    #endregion
}