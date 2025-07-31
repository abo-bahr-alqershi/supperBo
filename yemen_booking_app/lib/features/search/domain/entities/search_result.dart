import 'package:equatable/equatable.dart';

/// <summary>
/// كيان نتيجة البحث الفردية
/// Individual search result entity
/// </summary>
class SearchResultItem extends Equatable {
  /// <summary>
  /// معرف العقار
  /// Property ID
  /// </summary>
  final String id;

  /// <summary>
  /// اسم العقار
  /// Property name
  /// </summary>
  final String name;

  /// <summary>
  /// وصف مختصر
  /// Short description
  /// </summary>
  final String shortDescription;

  /// <summary>
  /// المدينة
  /// City
  /// </summary>
  final String city;

  /// <summary>
  /// العنوان
  /// Address
  /// </summary>
  final String address;

  /// <summary>
  /// الصورة الرئيسية
  /// Main image URL
  /// </summary>
  final String mainImageUrl;

  /// <summary>
  /// معرض الصور
  /// Image gallery
  /// </summary>
  final List<String> imageGallery;

  /// <summary>
  /// تصنيف النجوم
  /// Star rating
  /// </summary>
  final int starRating;

  /// <summary>
  /// متوسط التقييم
  /// Average rating
  /// </summary>
  final double averageRating;

  /// <summary>
  /// عدد التقييمات
  /// Reviews count
  /// </summary>
  final int reviewsCount;

  /// <summary>
  /// السعر الأساسي لليلة الواحدة
  /// Base price per night
  /// </summary>
  final double basePricePerNight;

  /// <summary>
  /// العملة
  /// Currency
  /// </summary>
  final String currency;

  /// <summary>
  /// هل في قائمة المفضلات
  /// Is favorite
  /// </summary>
  final bool isFavorite;

  /// <summary>
  /// المسافة من الموقع الحالي بالكيلومتر
  /// Distance from current location in km
  /// </summary>
  final double? distanceKm;

  /// <summary>
  /// نوع العقار
  /// Property type
  /// </summary>
  final String propertyType;

  /// <summary>
  /// أفضل المرافق
  /// Top amenities
  /// </summary>
  final List<String> topAmenities;

  /// <summary>
  /// حالة التوفر
  /// Availability status
  /// </summary>
  final String availabilityStatus;

  /// <summary>
  /// خط العرض
  /// Latitude
  /// </summary>
  final double? latitude;

  /// <summary>
  /// خط الطول
  /// Longitude
  /// </summary>
  final double? longitude;

  const SearchResultItem({
    required this.id,
    required this.name,
    required this.shortDescription,
    required this.city,
    required this.address,
    required this.mainImageUrl,
    required this.imageGallery,
    required this.starRating,
    required this.averageRating,
    required this.reviewsCount,
    required this.basePricePerNight,
    required this.currency,
    required this.isFavorite,
    this.distanceKm,
    required this.propertyType,
    required this.topAmenities,
    required this.availabilityStatus,
    this.latitude,
    this.longitude,
  });

  /// <summary>
  /// التحقق من توفر العقار
  /// Check if property is available
  /// </summary>
  bool get isAvailable => availabilityStatus.toLowerCase() == 'available';

  /// <summary>
  /// التحقق من وجود تقييمات
  /// Check if has reviews
  /// </summary>
  bool get hasReviews => reviewsCount > 0;

  /// <summary>
  /// تنسيق السعر للعرض
  /// Format price for display
  /// </summary>
  String get formattedPrice {
    return '${basePricePerNight.toStringAsFixed(0)} $currency';
  }

  /// <summary>
  /// تنسيق المسافة للعرض
  /// Format distance for display
  /// </summary>
  String? get formattedDistance {
    if (distanceKm == null) return null;
    if (distanceKm! < 1) {
      return '${(distanceKm! * 1000).toInt()} م';
    }
    return '${distanceKm!.toStringAsFixed(1)} كم';
  }

  /// <summary>
  /// تنسيق التقييم للعرض
  /// Format rating for display
  /// </summary>
  String get formattedRating {
    return averageRating.toStringAsFixed(1);
  }

  /// <summary>
  /// الحصول على وصف التقييم
  /// Get rating description
  /// </summary>
  String get ratingDescription {
    if (averageRating >= 4.5) return 'ممتاز';
    if (averageRating >= 4.0) return 'جيد جداً';
    if (averageRating >= 3.5) return 'جيد';
    if (averageRating >= 3.0) return 'مقبول';
    return 'ضعيف';
  }

  /// <summary>
  /// تنسيق عدد التقييمات للعرض
  /// Format reviews count for display
  /// </summary>
  String get formattedReviewsCount {
    if (reviewsCount == 0) return 'لا يوجد تقييمات';
    if (reviewsCount == 1) return 'تقييم واحد';
    if (reviewsCount == 2) return 'تقييمان';
    if (reviewsCount <= 10) return '$reviewsCount تقييمات';
    return '$reviewsCount تقييم';
  }

  /// <summary>
  /// التحقق من وجود موقع جغرافي
  /// Check if has location coordinates
  /// </summary>
  bool get hasLocation => latitude != null && longitude != null;

  /// <summary>
  /// الحصول على العنوان الكامل
  /// Get full address
  /// </summary>
  String get fullAddress => '$address، $city';

  @override
  List<Object?> get props => [
    id,
    name,
    shortDescription,
    city,
    address,
    mainImageUrl,
    imageGallery,
    starRating,
    averageRating,
    reviewsCount,
    basePricePerNight,
    currency,
    isFavorite,
    distanceKm,
    propertyType,
    topAmenities,
    availabilityStatus,
    latitude,
    longitude,
  ];
}

/// <summary>
/// كيان نتائج البحث الكاملة
/// Complete search results entity
/// </summary>
class SearchResult extends Equatable {
  /// <summary>
  /// قائمة نتائج البحث
  /// Search results list
  /// </summary>
  final List<SearchResultItem> results;

  /// <summary>
  /// إجمالي عدد النتائج
  /// Total count
  /// </summary>
  final int totalCount;

  /// <summary>
  /// رقم الصفحة الحالية
  /// Current page number
  /// </summary>
  final int currentPage;

  /// <summary>
  /// إجمالي عدد الصفحات
  /// Total pages
  /// </summary>
  final int totalPages;

  /// <summary>
  /// حجم الصفحة
  /// Page size
  /// </summary>
  final int pageSize;

  /// <summary>
  /// هل توجد صفحة تالية
  /// Has next page
  /// </summary>
  final bool hasNextPage;

  /// <summary>
  /// هل توجد صفحة سابقة
  /// Has previous page
  /// </summary>
  final bool hasPreviousPage;

  /// <summary>
  /// فلاتر مطبقة
  /// Applied filters
  /// </summary>
  final Map<String, dynamic> appliedFilters;

  /// <summary>
  /// وقت البحث بالميلي ثانية
  /// Search time in milliseconds
  /// </summary>
  final int searchTimeMs;

  const SearchResult({
    required this.results,
    required this.totalCount,
    required this.currentPage,
    required this.totalPages,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
    required this.appliedFilters,
    required this.searchTimeMs,
  });

  /// <summary>
  /// التحقق من وجود نتائج
  /// Check if has results
  /// </summary>
  bool get hasResults => results.isNotEmpty;

  /// <summary>
  /// التحقق من كون النتائج فارغة
  /// Check if results are empty
  /// </summary>
  bool get isEmpty => results.isEmpty;

  /// <summary>
  /// عدد النتائج في الصفحة الحالية
  /// Number of results in current page
  /// </summary>
  int get currentPageResultsCount => results.length;

  /// <summary>
  /// تنسيق وقت البحث للعرض
  /// Format search time for display
  /// </summary>
  String get formattedSearchTime {
    if (searchTimeMs < 1000) {
      return '$searchTimeMs مللي ثانية';
    }
    final seconds = searchTimeMs / 1000;
    return '${seconds.toStringAsFixed(1)} ثانية';
  }

  /// <summary>
  /// تنسيق عدد النتائج للعرض
  /// Format results count for display
  /// </summary>
  String get formattedResultsCount {
    if (totalCount == 0) return 'لا توجد نتائج';
    if (totalCount == 1) return 'نتيجة واحدة';
    if (totalCount == 2) return 'نتيجتان';
    if (totalCount <= 10) return '$totalCount نتائج';
    return '$totalCount نتيجة';
  }

  /// <summary>
  /// تنسيق معلومات الصفحة للعرض
  /// Format page info for display
  /// </summary>
  String get formattedPageInfo {
    if (totalPages <= 1) return '';
    return 'الصفحة $currentPage من $totalPages';
  }

  /// <summary>
  /// الحصول على النتائج المتاحة فقط
  /// Get only available results
  /// </summary>
  List<SearchResultItem> get availableResults {
    return results.where((result) => result.isAvailable).toList();
  }

  /// <summary>
  /// الحصول على العقارات المفضلة من النتائج
  /// Get favorite properties from results
  /// </summary>
  List<SearchResultItem> get favoriteResults {
    return results.where((result) => result.isFavorite).toList();
  }

  /// <summary>
  /// ترتيب النتائج حسب السعر (تصاعدي)
  /// Sort results by price (ascending)
  /// </summary>
  List<SearchResultItem> get resultsSortedByPriceAsc {
    final sortedResults = List<SearchResultItem>.from(results);
    sortedResults.sort((a, b) => a.basePricePerNight.compareTo(b.basePricePerNight));
    return sortedResults;
  }

  /// <summary>
  /// ترتيب النتائج حسب السعر (تنازلي)
  /// Sort results by price (descending)
  /// </summary>
  List<SearchResultItem> get resultsSortedByPriceDesc {
    final sortedResults = List<SearchResultItem>.from(results);
    sortedResults.sort((a, b) => b.basePricePerNight.compareTo(a.basePricePerNight));
    return sortedResults;
  }

  /// <summary>
  /// ترتيب النتائج حسب التقييم (تنازلي)
  /// Sort results by rating (descending)
  /// </summary>
  List<SearchResultItem> get resultsSortedByRating {
    final sortedResults = List<SearchResultItem>.from(results);
    sortedResults.sort((a, b) => b.averageRating.compareTo(a.averageRating));
    return sortedResults;
  }

  /// <summary>
  /// ترتيب النتائج حسب المسافة (تصاعدي)
  /// Sort results by distance (ascending)
  /// </summary>
  List<SearchResultItem> get resultsSortedByDistance {
    final sortedResults = results.where((r) => r.distanceKm != null).toList();
    sortedResults.sort((a, b) => a.distanceKm!.compareTo(b.distanceKm!));
    return sortedResults;
  }

  @override
  List<Object> get props => [
    results,
    totalCount,
    currentPage,
    totalPages,
    pageSize,
    hasNextPage,
    hasPreviousPage,
    appliedFilters,
    searchTimeMs,
  ];
}