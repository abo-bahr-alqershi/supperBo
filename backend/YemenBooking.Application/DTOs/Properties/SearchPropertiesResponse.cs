using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs.PropertySearch;

namespace YemenBooking.Application.DTOs.Properties
{
    /// <summary>
    /// استجابة البحث عن العقارات
    /// Search properties response
    /// </summary>
    public class SearchPropertiesResponse
    {
        /// <summary>
        /// قائمة العقارات المطابقة للبحث
        /// List of properties matching the search
        /// </summary>
        public List<PropertySearchResultDto> Properties { get; set; } = new();

        /// <summary>
        /// العدد الإجمالي للنتائج
        /// Total count of results
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// رقم الصفحة الحالية
        /// Current page number
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// حجم الصفحة
        /// Page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// العدد الإجمالي للصفحات
        /// Total pages count
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// هل يوجد صفحة سابقة
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// هل يوجد صفحة تالية
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// فلاتر البحث المطبقة
        /// Applied search filters
        /// </summary>
        public SearchFiltersDto AppliedFilters { get; set; } = null!;

        /// <summary>
        /// وقت البحث بالميلي ثانية
        /// Search time in milliseconds
        /// </summary>
        public long SearchTimeMs { get; set; }

        /// <summary>
        /// إحصائيات البحث
        /// Search statistics
        /// </summary>
        public SearchStatisticsDto Statistics { get; set; } = null!;
    }

    /// <summary>
    /// نتيجة البحث عن عقار واحد
    /// Single property search result
    /// </summary>
    public class PropertySearchResultDto
    {
        /// <summary>
        /// معرف العقار
        /// Property identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// اسم العقار
        /// Property name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// نوع العقار
        /// Property type
        /// </summary>
        public string PropertyType { get; set; } = string.Empty;

        /// <summary>
        /// العنوان
        /// Address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// المدينة
        /// City
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// السعر الأساسي
        /// Base price
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// العملة
        /// Currency
        /// </summary>
        public string Currency { get; set; } = "YER";

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
        /// الصورة الرئيسية
        /// Main image URL
        /// </summary>
        public string MainImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// المسافة من موقع البحث (بالكيلومتر)
        /// Distance from search location (in kilometers)
        /// </summary>
        public double? DistanceKm { get; set; }

        /// <summary>
        /// هل متاح للحجز
        /// Whether available for booking
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// هل في قائمة المفضلات
        /// Whether in favorites list
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// وسائل الراحة الرئيسية
        /// Main amenities
        /// </summary>
        public List<string> MainAmenities { get; set; } = new();

        /// <summary>
        /// نسبة التطابق مع البحث (0-100)
        /// Match percentage with search (0-100)
        /// </summary>
        public int MatchPercentage { get; set; }
    }
}
