using AdvancedIndexingSystem.Core.Events;
using AdvancedIndexingSystem.Core.Services;
using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Interfaces.Services;

/// <summary>
/// واجهة خدمة الفهرسة للعقارات والوحدات
/// Property and unit indexing service interface
/// </summary>
public interface IIndexingService
{
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

    #endregion

    #region فهرسة العقارات - Property Indexing

    /// <summary>
    /// فهرسة عقار جديد
    /// Index new property
    /// </summary>
    Task<bool> IndexPropertyAsync(Property property);

    /// <summary>
    /// تحديث فهرسة عقار
    /// Update property index
    /// </summary>
    Task<bool> UpdatePropertyIndexAsync(Property property);

    /// <summary>
    /// حذف عقار من الفهرس
    /// Remove property from index
    /// </summary>
    Task<bool> RemovePropertyIndexAsync(Guid propertyId);

    #endregion

    #region فهرسة الوحدات - Unit Indexing

    /// <summary>
    /// فهرسة وحدة جديدة
    /// Index new unit
    /// </summary>
    Task<bool> IndexUnitAsync(Unit unit);

    /// <summary>
    /// تحديث فهرسة وحدة
    /// Update unit index
    /// </summary>
    Task<bool> UpdateUnitIndexAsync(Unit unit);

    /// <summary>
    /// حذف وحدة من الفهرس
    /// Remove unit from index
    /// </summary>
    Task<bool> RemoveUnitIndexAsync(Guid unitId);

    #endregion

    #region فهرسة التسعير - Pricing Indexing

    /// <summary>
    /// فهرسة قاعدة تسعير جديدة
    /// Index new pricing rule
    /// </summary>
    Task<bool> IndexPricingRuleAsync(PricingRule pricingRule);

    /// <summary>
    /// تحديث فهرسة قاعدة تسعير
    /// Update pricing rule index
    /// </summary>
    Task<bool> UpdatePricingRuleIndexAsync(PricingRule pricingRule);

    /// <summary>
    /// حذف قاعدة تسعير من الفهرس
    /// Remove pricing rule from index
    /// </summary>
    Task<bool> RemovePricingRuleIndexAsync(Guid pricingRuleId);

    #endregion

    #region فهرسة الإتاحة - Availability Indexing

    /// <summary>
    /// فهرسة إتاحة جديدة
    /// Index new availability
    /// </summary>
    Task<bool> IndexAvailabilityAsync(UnitAvailability availability);

    /// <summary>
    /// تحديث فهرسة إتاحة
    /// Update availability index
    /// </summary>
    Task<bool> UpdateAvailabilityIndexAsync(UnitAvailability availability);

    /// <summary>
    /// حذف إتاحة من الفهرس
    /// Remove availability from index
    /// </summary>
    Task<bool> RemoveAvailabilityIndexAsync(Guid availabilityId);

    #endregion

    #region فهرسة المرافق - Amenity Indexing

    /// <summary>
    /// فهرسة ربط مرفق بعقار
    /// Index property-amenity association
    /// </summary>
    Task<bool> IndexPropertyAmenityAsync(PropertyAmenity propertyAmenity);

    /// <summary>
    /// حذف ربط مرفق من العقار
    /// Remove property-amenity association
    /// </summary>
    Task<bool> RemovePropertyAmenityIndexAsync(Guid propertyId, Guid amenityId);

    #endregion

    #region فهرسة أنواع العقارات والوحدات - Property and Unit Types Indexing

    /// <summary>
    /// فهرسة نوع عقار جديد
    /// Index new property type
    /// </summary>
    Task<bool> IndexPropertyTypeAsync(PropertyType propertyType);

    /// <summary>
    /// تحديث فهرسة نوع عقار
    /// Update property type index
    /// </summary>
    Task<bool> UpdatePropertyTypeIndexAsync(PropertyType propertyType);

    /// <summary>
    /// فهرسة نوع وحدة جديد
    /// Index new unit type
    /// </summary>
    Task<bool> IndexUnitTypeAsync(UnitType unitType);

    /// <summary>
    /// تحديث فهرسة نوع وحدة
    /// Update unit type index
    /// </summary>
    Task<bool> UpdateUnitTypeIndexAsync(UnitType unitType);

    #endregion

    #region فهرسة المدن - City Indexing

    /// <summary>
    /// تحديث فهرسة المدن
    /// Update city indexing
    /// </summary>
    Task<bool> UpdateCityIndexAsync(string cityName, Guid propertyId, bool isAdd = true);

    #endregion

    #region البحث - Search

    /// <summary>
    /// البحث في العقارات
    /// Search properties
    /// </summary>
    Task<List<PropertyIndexItem>> SearchPropertiesAsync(PropertySearchRequest request);

    /// <summary>
    /// البحث في الوحدات
    /// Search units
    /// </summary>
    Task<List<UnitIndexItem>> SearchUnitsAsync(UnitSearchRequest request);

    #endregion

    #region التهيئة والحفظ - Initialization and Save

    /// <summary>
    /// تهيئة خدمة الفهرسة
    /// Initialize indexing service
    /// </summary>
    Task<bool> InitializeAsync();

    /// <summary>
    /// حفظ الفهارس
    /// Save indexes
    /// </summary>
    Task<bool> SaveIndexesAsync();

    /// <summary>
    /// تحميل الفهارس
    /// Load indexes
    /// </summary>
    Task<bool> LoadIndexesAsync();

    #endregion
}