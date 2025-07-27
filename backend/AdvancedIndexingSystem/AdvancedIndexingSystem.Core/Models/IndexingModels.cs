using Newtonsoft.Json;

namespace AdvancedIndexingSystem.Core.Models;

/// <summary>
/// عنصر فهرس الحقل الديناميكي
/// Dynamic field index item
/// </summary>
public class DynamicFieldIndexItem
{
    /// <summary>
    /// معرف الحقل
    /// Field identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    [JsonProperty("field_name")]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// نوع الحقل
    /// Field type
    /// </summary>
    [JsonProperty("field_type")]
    public string FieldType { get; set; } = string.Empty;

    /// <summary>
    /// قيمة الحقل
    /// Field value
    /// </summary>
    [JsonProperty("field_value")]
    public string FieldValue { get; set; } = string.Empty;

    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    [JsonProperty("unit_id")]
    public string UnitId { get; set; } = string.Empty;

    /// <summary>
    /// معرف الكيان المرتبط
    /// Associated entity ID
    /// </summary>
    [JsonProperty("entity_id")]
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// نوع الكيان (Property, Unit)
    /// Entity type
    /// </summary>
    [JsonProperty("entity_type")]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ الإنشاء
    /// Created timestamp
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// بيانات إضافية
    /// Additional data
    /// </summary>
    [JsonProperty("additional_data")]
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// هل الحقل قابل للبحث
    /// Is field searchable
    /// </summary>
    [JsonProperty("is_searchable")]
    public bool IsSearchable { get; set; } = true;

    /// <summary>
    /// هل الحقل قابل للفلترة
    /// Is field filterable
    /// </summary>
    [JsonProperty("is_filterable")]
    public bool IsFilterable { get; set; } = true;

    /// <summary>
    /// الوزن في البحث
    /// Search weight
    /// </summary>
    [JsonProperty("search_weight")]
    public float SearchWeight { get; set; } = 1.0f;
}

/// <summary>
/// عنصر فهرس نوع العقار
/// Property type index item
/// </summary>
public class PropertyTypeIndexItem
{
    /// <summary>
    /// معرف نوع العقار
    /// Property type identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// اسم نوع العقار
    /// Property type name
    /// </summary>
    [JsonProperty("type_name")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// الوصف
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// الحقول المرتبطة
    /// Associated fields
    /// </summary>
    [JsonProperty("associated_fields")]
    public List<string> AssociatedFields { get; set; } = new();

    /// <summary>
    /// المرافق الافتراضية
    /// Default amenities
    /// </summary>
    [JsonProperty("default_amenities")]
    public List<string> DefaultAmenities { get; set; } = new();

    /// <summary>
    /// أنواع الوحدات المدعومة
    /// Supported unit types
    /// </summary>
    [JsonProperty("supported_unit_types")]
    public List<string> SupportedUnitTypes { get; set; } = new();

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// بيانات إضافية
    /// Additional type data
    /// </summary>
    [JsonProperty("type_data")]
    public Dictionary<string, object> TypeData { get; set; } = new();

    /// <summary>
    /// هل النوع نشط
    /// Is type active
    /// </summary>
    [JsonProperty("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// عدد العقارات المرتبطة
    /// Associated properties count
    /// </summary>
    [JsonProperty("properties_count")]
    public int PropertiesCount { get; set; } = 0;
}

/// <summary>
/// عنصر فهرس نوع الوحدة
/// Unit type index item
/// </summary>
public class UnitTypeIndexItem
{
    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// اسم نوع الوحدة
    /// Unit type name
    /// </summary>
    [JsonProperty("type_name")]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// معرف نوع العقار المرتبط
    /// Associated property type ID
    /// </summary>
    [JsonProperty("property_type_id")]
    public string PropertyTypeId { get; set; } = string.Empty;

    /// <summary>
    /// الوصف
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// الحد الأقصى للسعة
    /// Maximum capacity
    /// </summary>
    [JsonProperty("max_capacity")]
    public int MaxCapacity { get; set; } = 1;

    /// <summary>
    /// الحقول المرتبطة
    /// Associated fields
    /// </summary>
    [JsonProperty("associated_fields")]
    public List<string> AssociatedFields { get; set; } = new();

    /// <summary>
    /// قواعد التسعير الافتراضية
    /// Default pricing rules
    /// </summary>
    [JsonProperty("default_pricing_rules")]
    public Dictionary<string, object> DefaultPricingRules { get; set; } = new();

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// بيانات إضافية
    /// Additional type data
    /// </summary>
    [JsonProperty("type_data")]
    public Dictionary<string, object> TypeData { get; set; } = new();

    /// <summary>
    /// هل النوع نشط
    /// Is type active
    /// </summary>
    [JsonProperty("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// عدد الوحدات المرتبطة
    /// Associated units count
    /// </summary>
    [JsonProperty("units_count")]
    public int UnitsCount { get; set; } = 0;
}

/// <summary>
/// عنصر فهرس المدينة
/// City index item
/// </summary>
public class CityIndexItem
{
    /// <summary>
    /// معرف المدينة
    /// City identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// اسم المدينة
    /// City name
    /// </summary>
    [JsonProperty("city_name")]
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// المحافظة
    /// Governorate
    /// </summary>
    [JsonProperty("governorate")]
    public string Governorate { get; set; } = string.Empty;

    /// <summary>
    /// المديرية
    /// District
    /// </summary>
    [JsonProperty("district")]
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// معرفات العقارات في المدينة
    /// Property IDs in the city
    /// </summary>
    [JsonProperty("property_ids")]
    public List<string> PropertyIds { get; set; } = new();

    /// <summary>
    /// إحصائيات المدينة
    /// City statistics
    /// </summary>
    [JsonProperty("statistics")]
    public Dictionary<string, object> Statistics { get; set; } = new();

    /// <summary>
    /// متوسط الأسعار
    /// Average prices
    /// </summary>
    [JsonProperty("average_prices")]
    public Dictionary<string, decimal> AveragePrices { get; set; } = new();

    /// <summary>
    /// عدد العقارات
    /// Properties count
    /// </summary>
    [JsonProperty("properties_count")]
    public int PropertiesCount { get; set; } = 0;

    /// <summary>
    /// معدل الإشغال
    /// Occupancy rate
    /// </summary>
    [JsonProperty("occupancy_rate")]
    public float OccupancyRate { get; set; } = 0.0f;

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// هل المدينة نشطة
    /// Is city active
    /// </summary>
    [JsonProperty("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الموقع الجغرافي
    /// Geographic location
    /// </summary>
    [JsonProperty("coordinates")]
    public GeoLocation? Coordinates { get; set; }
}

/// <summary>
/// الموقع الجغرافي
/// Geographic location
/// </summary>
public class GeoLocation
{
    /// <summary>
    /// خط العرض
    /// Latitude
    /// </summary>
    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    /// <summary>
    /// خط الطول
    /// Longitude
    /// </summary>
    [JsonProperty("longitude")]
    public double Longitude { get; set; }
}

/// <summary>
/// عنصر فهرس المرفق
/// Amenity index item
/// </summary>
public class AmenityIndexItem
{
    /// <summary>
    /// معرف المرفق
    /// Amenity identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// اسم المرفق
    /// Amenity name
    /// </summary>
    [JsonProperty("amenity_name")]
    public string AmenityName { get; set; } = string.Empty;

    /// <summary>
    /// الفئة
    /// Category
    /// </summary>
    [JsonProperty("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// معرفات العقارات المرتبطة
    /// Associated property IDs
    /// </summary>
    [JsonProperty("property_ids")]
    public List<string> PropertyIds { get; set; } = new();

    /// <summary>
    /// بيانات المرفق الإضافية
    /// Additional amenity data
    /// </summary>
    [JsonProperty("amenity_data")]
    public Dictionary<string, object> AmenityData { get; set; } = new();

    /// <summary>
    /// هل المرفق مجاني
    /// Is amenity free
    /// </summary>
    [JsonProperty("is_free")]
    public bool IsFree { get; set; } = true;

    /// <summary>
    /// التكلفة الإضافية
    /// Additional cost
    /// </summary>
    [JsonProperty("additional_cost")]
    public decimal AdditionalCost { get; set; } = 0m;

    /// <summary>
    /// عملة التكلفة
    /// Cost currency
    /// </summary>
    [JsonProperty("cost_currency")]
    public string CostCurrency { get; set; } = "YER";

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// هل المرفق نشط
    /// Is amenity active
    /// </summary>
    [JsonProperty("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الأولوية في العرض
    /// Display priority
    /// </summary>
    [JsonProperty("display_priority")]
    public int DisplayPriority { get; set; } = 0;
}

/// <summary>
/// بيانات التسعير المجمعة للنماذج
/// Bulk pricing rule data for models
/// </summary>
public class BulkPricingRuleData
{
    /// <summary>
    /// معرف قاعدة التسعير
    /// Pricing rule identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    [JsonProperty("unit_id")]
    public string UnitId { get; set; } = string.Empty;

    /// <summary>
    /// نوع السعر
    /// Price type
    /// </summary>
    [JsonProperty("price_type")]
    public string PriceType { get; set; } = string.Empty;

    /// <summary>
    /// طبقة التسعير
    /// Pricing tier
    /// </summary>
    [JsonProperty("pricing_tier")]
    public string PricingTier { get; set; } = string.Empty;

    /// <summary>
    /// نسبة التغيير المئوية
    /// Percentage change
    /// </summary>
    [JsonProperty("percentage_change")]
    public decimal PercentageChange { get; set; } = 0m;

    /// <summary>
    /// مبلغ السعر
    /// Price amount
    /// </summary>
    [JsonProperty("price_amount")]
    public decimal PriceAmount { get; set; }

    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = "YER";

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    [JsonProperty("end_date")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// بيانات إضافية
    /// Additional data
    /// </summary>
    [JsonProperty("additional_data")]
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// بيانات الإتاحة المجمعة للنماذج
/// Bulk availability data for models
/// </summary>
public class BulkAvailabilityData
{
    /// <summary>
    /// معرف الإتاحة
    /// Availability identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    [JsonProperty("unit_id")]
    public string UnitId { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    [JsonProperty("end_date")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// حالة الإتاحة
    /// Availability status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// السبب
    /// Reason
    /// </summary>
    [JsonProperty("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// بيانات إضافية
    /// Additional data
    /// </summary>
    [JsonProperty("additional_data")]
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// عنصر فهرس التسعير
/// Pricing index item
/// </summary>
public class PricingIndexItem
{
    /// <summary>
    /// معرف التسعير
    /// Pricing identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    [JsonProperty("unit_id")]
    public string UnitId { get; set; } = string.Empty;

    /// <summary>
    /// نوع السعر
    /// Price type
    /// </summary>
    [JsonProperty("price_type")]
    public string PriceType { get; set; } = string.Empty;

    /// <summary>
    /// مبلغ السعر
    /// Price amount
    /// </summary>
    [JsonProperty("price_amount")]
    public decimal PriceAmount { get; set; }

    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    [JsonProperty("end_date")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// فئة التسعير
    /// Pricing tier
    /// </summary>
    [JsonProperty("pricing_tier")]
    public string? PricingTier { get; set; }

    /// <summary>
    /// نسبة التغيير
    /// Percentage change
    /// </summary>
    [JsonProperty("percentage_change")]
    public decimal? PercentageChange { get; set; }

    /// <summary>
    /// الحد الأدنى للسعر
    /// Minimum price
    /// </summary>
    [JsonProperty("min_price")]
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// الحد الأقصى للسعر
    /// Maximum price
    /// </summary>
    [JsonProperty("max_price")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// الوصف
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// تاريخ الإنشاء
    /// Created timestamp
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// عنصر فهرس الإتاحة
/// Availability index item
/// </summary>
public class AvailabilityIndexItem
{
    /// <summary>
    /// معرف الإتاحة
    /// Availability identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    [JsonProperty("unit_id")]
    public string UnitId { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    [JsonProperty("end_date")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// حالة الإتاحة
    /// Availability status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// السبب
    /// Reason
    /// </summary>
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// ملاحظات
    /// Notes
    /// </summary>
    [JsonProperty("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// معرف الحجز (إن وجد)
    /// Booking identifier (if any)
    /// </summary>
    [JsonProperty("booking_id")]
    public Guid? BookingId { get; set; }

    /// <summary>
    /// تاريخ الإنشاء
    /// Created timestamp
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated timestamp
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }
}
