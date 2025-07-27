using Newtonsoft.Json;

namespace AdvancedIndexingSystem.Core.Models;

/// <summary>
/// نموذج بيانات العقار
/// Property data model
/// </summary>
public class PropertyItem
{
    /// <summary>
    /// معرف العقار الفريد
    /// Unique property identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// عنوان العقار
    /// Property title
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// وصف العقار
    /// Property description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// السعر
    /// Price
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// المدينة
    /// City
    /// </summary>
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// المنطقة
    /// District
    /// </summary>
    [JsonProperty("district")]
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// عدد غرف النوم
    /// Number of bedrooms
    /// </summary>
    [JsonProperty("bedrooms")]
    public int Bedrooms { get; set; }

    /// <summary>
    /// عدد الحمامات
    /// Number of bathrooms
    /// </summary>
    [JsonProperty("bathrooms")]
    public int Bathrooms { get; set; }

    /// <summary>
    /// المساحة بالمتر المربع
    /// Area in square meters
    /// </summary>
    [JsonProperty("area")]
    public double Area { get; set; }

    /// <summary>
    /// نوع العقار
    /// Property type
    /// </summary>
    [JsonProperty("propertyType")]
    public string PropertyType { get; set; } = string.Empty;

    /// <summary>
    /// نوع التدفئة
    /// Heating type
    /// </summary>
    [JsonProperty("heatingType")]
    public string HeatingType { get; set; } = string.Empty;

    /// <summary>
    /// المرافق المتاحة
    /// Available amenities
    /// </summary>
    [JsonProperty("amenities")]
    public List<string> Amenities { get; set; } = new();

    /// <summary>
    /// هل العقار متاح؟
    /// Is property available?
    /// </summary>
    [JsonProperty("isAvailable")]
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// تاريخ الإنشاء
    /// Created date
    /// </summary>
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last updated date
    /// </summary>
    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// معلومات المالك
    /// Owner information
    /// </summary>
    [JsonProperty("owner")]
    public OwnerInfo Owner { get; set; } = new();

    /// <summary>
    /// الموقع الجغرافي
    /// Geographic location
    /// </summary>
    [JsonProperty("location")]
    public LocationInfo Location { get; set; } = new();

    /// <summary>
    /// الصور
    /// Images
    /// </summary>
    [JsonProperty("images")]
    public List<string> Images { get; set; } = new();

    /// <summary>
    /// التقييم
    /// Rating
    /// </summary>
    [JsonProperty("rating")]
    public double Rating { get; set; }

    /// <summary>
    /// عدد المراجعات
    /// Number of reviews
    /// </summary>
    [JsonProperty("reviewsCount")]
    public int ReviewsCount { get; set; }

    /// <summary>
    /// الكلمات المفتاحية
    /// Keywords/Tags
    /// </summary>
    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// حالة العقار
    /// Property status
    /// </summary>
    [JsonProperty("status")]
    public PropertyStatus Status { get; set; } = PropertyStatus.Active;

    /// <summary>
    /// معلومات إضافية (حقول ديناميكية)
    /// Additional information (dynamic fields)
    /// </summary>
    [JsonProperty("additionalInfo")]
    public Dictionary<string, object> AdditionalInfo { get; set; } = new();
}

/// <summary>
/// معلومات المالك
/// Owner information
/// </summary>
public class OwnerInfo
{
    /// <summary>
    /// اسم المالك
    /// Owner name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// رقم الهاتف
    /// Phone number
    /// </summary>
    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// البريد الإلكتروني
    /// Email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// هل المالك موثق؟
    /// Is owner verified?
    /// </summary>
    [JsonProperty("isVerified")]
    public bool IsVerified { get; set; }
}

/// <summary>
/// معلومات الموقع
/// Location information
/// </summary>
public class LocationInfo
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

    /// <summary>
    /// العنوان الكامل
    /// Full address
    /// </summary>
    [JsonProperty("fullAddress")]
    public string FullAddress { get; set; } = string.Empty;

    /// <summary>
    /// الرمز البريدي
    /// Postal code
    /// </summary>
    [JsonProperty("postalCode")]
    public string PostalCode { get; set; } = string.Empty;
}

/// <summary>
/// حالة العقار
/// Property status
/// </summary>
public enum PropertyStatus
{
    /// <summary>
    /// نشط - Active
    /// </summary>
    Active,

    /// <summary>
    /// غير نشط - Inactive
    /// </summary>
    Inactive,

    /// <summary>
    /// محجوز - Reserved
    /// </summary>
    Reserved,

    /// <summary>
    /// مباع - Sold
    /// </summary>
    Sold,

    /// <summary>
    /// مؤجر - Rented
    /// </summary>
    Rented,

    /// <summary>
    /// تحت المراجعة - Under Review
    /// </summary>
    UnderReview,

    /// <summary>
    /// مرفوض - Rejected
    /// </summary>
    Rejected
}