using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.MobileApp.Properties;

/// <summary>
/// استعلام جلب العقارات المميزة للعميل
/// Query to get featured properties for client
/// </summary>
public class ClientGetFeaturedPropertiesQuery : IRequest<ResultDto<List<ClientFeaturedPropertyDto>>>
{
    /// <summary>
    /// عدد العقارات المطلوبة
    /// Number of properties required
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// معرف المستخدم (اختياري - لتحديد المفضلات)
    /// User ID (optional - to determine favorites)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// الموقع الحالي للمستخدم (خط العرض)
    /// User's current location (latitude)
    /// </summary>
    public decimal? CurrentLatitude { get; set; }

    /// <summary>
    /// الموقع الحالي للمستخدم (خط الطول)
    /// User's current location (longitude)
    /// </summary>
    public decimal? CurrentLongitude { get; set; }

    /// <summary>
    /// البلد/المدينة المفضلة
    /// Preferred country/city
    /// </summary>
    public string? PreferredLocation { get; set; }
}

/// <summary>
/// بيانات العقار المميز للعميل
/// Client featured property data
/// </summary>
public class ClientFeaturedPropertyDto
{
    /// <summary>
    /// معرف العقار
    /// Property ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// اسم العقار
    /// Property name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// وصف مختصر
    /// Short description
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// المدينة
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// العنوان
    /// Address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// الصورة الرئيسية
    /// Main image
    /// </summary>
    public string MainImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// معرض الصور (أول 3 صور)
    /// Image gallery (first 3 images)
    /// </summary>
    public List<string> ImageGallery { get; set; } = new();

    /// <summary>
    /// تصنيف النجوم
    /// Star rating
    /// </summary>
    public int StarRating { get; set; }

    /// <summary>
    /// متوسط التقييم
    /// Average rating
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// عدد المراجعات
    /// Reviews count
    /// </summary>
    public int ReviewsCount { get; set; }

    /// <summary>
    /// السعر الأساسي لليلة الواحدة
    /// Base price per night
    /// </summary>
    public decimal BasePricePerNight { get; set; }

    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    public string Currency { get; set; } = "YER";

    /// <summary>
    /// هل في قائمة المفضلات
    /// Is in favorites
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// المسافة من الموقع الحالي (بالكيلومتر)
    /// Distance from current location (in km)
    /// </summary>
    public decimal? DistanceKm { get; set; }

    /// <summary>
    /// نوع العقار
    /// Property type
    /// </summary>
    public string PropertyType { get; set; } = string.Empty;

    /// <summary>
    /// أفضل الوسائل والخدمات (أول 3)
    /// Top amenities (first 3)
    /// </summary>
    public List<string> TopAmenities { get; set; } = new();

    /// <summary>
    /// حالة التوفر
    /// Availability status
    /// </summary>
    public string AvailabilityStatus { get; set; } = "Available";

    /// <summary>
    /// عرض خاص (إن وجد)
    /// Special offer (if any)
    /// </summary>
    public ClientSpecialOfferDto? SpecialOffer { get; set; }

    /// <summary>
    /// نسبة الحجز (شعبية العقار)
    /// Booking rate (property popularity)
    /// </summary>
    public decimal BookingRate { get; set; }

    /// <summary>
    /// تصنيف مميز (مثل: "الأكثر حجزاً"، "جديد"، "موصى به")
    /// Featured badge (e.g., "Most Booked", "New", "Recommended")
    /// </summary>
    public string? FeaturedBadge { get; set; }
}

/// <summary>
/// بيانات العرض الخاص
/// Special offer data
/// </summary>
public class ClientSpecialOfferDto
{
    /// <summary>
    /// عنوان العرض
    /// Offer title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// وصف العرض
    /// Offer description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// نسبة الخصم
    /// Discount percentage
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// مبلغ الخصم
    /// Discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// تاريخ انتهاء العرض
    /// Offer expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// لون العرض (للتصميم)
    /// Offer color (for design)
    /// </summary>
    public string Color { get; set; } = "#FF6B6B";
}