import 'package:equatable/equatable.dart';
import '../../domain/entities/search_result.dart';

/// <summary>
/// نموذج بيانات نتيجة البحث الفردية (متوافق مع PropertySearchResultDto)
/// Individual search result data model (compatible with PropertySearchResultDto)
/// </summary>
class PropertySearchResultDto extends Equatable {
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
  /// نوع العقار
  /// Property type
  /// </summary>
  final String propertyType;

  /// <summary>
  /// العنوان
  /// Address
  /// </summary>
  final String address;

  /// <summary>
  /// المدينة
  /// City
  /// </summary>
  final String city;

  /// <summary>
  /// السعر الأساسي
  /// Base price
  /// </summary>
  final double basePrice;

  /// <summary>
  /// العملة
  /// Currency
  /// </summary>
  final String currency;

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
  /// عدد المراجعات
  /// Reviews count
  /// </summary>
  final int reviewsCount;

  /// <summary>
  /// الصورة الرئيسية
  /// Main image URL
  /// </summary>
  final String mainImageUrl;

  /// <summary>
  /// المسافة من موقع البحث (بالكيلومتر)
  /// Distance from search location (in kilometers)
  /// </summary>
  final double? distanceKm;

  /// <summary>
  /// هل متاح للحجز
  /// Whether available for booking
  /// </summary>
  final bool isAvailable;

  /// <summary>
  /// هل في قائمة المفضلات
  /// Whether in favorites list
  /// </summary>
  final bool isFavorite;

  /// <summary>
  /// وسائل الراحة الرئيسية
  /// Main amenities
  /// </summary>
  final List<String> mainAmenities;

  /// <summary>
  /// نسبة التطابق مع البحث (0-100)
  /// Match percentage with search (0-100)
  /// </summary>
  final int matchPercentage;

  const PropertySearchResultDto({
    required this.id,
    required this.name,
    required this.propertyType,
    required this.address,
    required this.city,
    required this.basePrice,
    required this.currency,
    required this.starRating,
    required this.averageRating,
    required this.reviewsCount,
    required this.mainImageUrl,
    this.distanceKm,
    required this.isAvailable,
    required this.isFavorite,
    required this.mainAmenities,
    required this.matchPercentage,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory PropertySearchResultDto.fromJson(Map<String, dynamic> json) {
    return PropertySearchResultDto(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      propertyType: json['propertyType'] ?? '',
      address: json['address'] ?? '',
      city: json['city'] ?? '',
      basePrice: (json['basePrice'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'YER',
      starRating: json['starRating'] ?? 0,
      averageRating: (json['averageRating'] ?? 0.0).toDouble(),
      reviewsCount: json['reviewsCount'] ?? 0,
      mainImageUrl: json['mainImageUrl'] ?? '',
      distanceKm: json['distanceKm']?.toDouble(),
      isAvailable: json['isAvailable'] ?? true,
      isFavorite: json['isFavorite'] ?? false,
      mainAmenities: List<String>.from(json['mainAmenities'] ?? []),
      matchPercentage: json['matchPercentage'] ?? 100,
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'propertyType': propertyType,
      'address': address,
      'city': city,
      'basePrice': basePrice,
      'currency': currency,
      'starRating': starRating,
      'averageRating': averageRating,
      'reviewsCount': reviewsCount,
      'mainImageUrl': mainImageUrl,
      'distanceKm': distanceKm,
      'isAvailable': isAvailable,
      'isFavorite': isFavorite,
      'mainAmenities': mainAmenities,
      'matchPercentage': matchPercentage,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  SearchResultItem toEntity() {
    return SearchResultItem(
      id: id,
      name: name,
      propertyType: propertyType,
      address: address,
      city: city,
      basePrice: basePrice,
      currency: currency,
      starRating: starRating,
      averageRating: averageRating,
      reviewsCount: reviewsCount,
      mainImageUrl: mainImageUrl,
      distanceKm: distanceKm,
      isAvailable: isAvailable,
      isFavorite: isFavorite,
      mainAmenities: mainAmenities,
      matchPercentage: matchPercentage,
    );
  }

  @override
  List<Object?> get props => [
    id,
    name,
    propertyType,
    address,
    city,
    basePrice,
    currency,
    starRating,
    averageRating,
    reviewsCount,
    mainImageUrl,
    distanceKm,
    isAvailable,
    isFavorite,
    mainAmenities,
    matchPercentage,
  ];
}

/// <summary>
/// نموذج بيانات فلاتر البحث
/// Search filters data model
/// </summary>
class SearchFiltersDto extends Equatable {
  /// <summary>
  /// كلمة البحث
  /// Search term
  /// </summary>
  final String? searchTerm;

  /// <summary>
  /// المدينة
  /// City
  /// </summary>
  final String? city;

  /// <summary>
  /// تاريخ الوصول
  /// Check-in date
  /// </summary>
  final DateTime? checkIn;

  /// <summary>
  /// تاريخ المغادرة
  /// Check-out date
  /// </summary>
  final DateTime? checkOut;

  /// <summary>
  /// عدد الضيوف
  /// Guests count
  /// </summary>
  final int? guestsCount;

  /// <summary>
  /// معرف نوع العقار
  /// Property type ID
  /// </summary>
  final String? propertyTypeId;

  /// <summary>
  /// السعر الأدنى
  /// Minimum price
  /// </summary>
  final double? minPrice;

  /// <summary>
  /// السعر الأقصى
  /// Maximum price
  /// </summary>
  final double? maxPrice;

  /// <summary>
  /// تصنيف النجوم الأدنى
  /// Minimum star rating
  /// </summary>
  final int? minStarRating;

  /// <summary>
  /// وسائل الراحة المطلوبة
  /// Required amenities
  /// </summary>
  final List<String> requiredAmenities;

  const SearchFiltersDto({
    this.searchTerm,
    this.city,
    this.checkIn,
    this.checkOut,
    this.guestsCount,
    this.propertyTypeId,
    this.minPrice,
    this.maxPrice,
    this.minStarRating,
    this.requiredAmenities = const [],
  });

  factory SearchFiltersDto.fromJson(Map<String, dynamic> json) {
    return SearchFiltersDto(
      searchTerm: json['searchTerm'],
      city: json['city'],
      checkIn: json['checkIn'] != null ? DateTime.parse(json['checkIn']) : null,
      checkOut: json['checkOut'] != null ? DateTime.parse(json['checkOut']) : null,
      guestsCount: json['guestsCount'],
      propertyTypeId: json['propertyTypeId'],
      minPrice: json['minPrice']?.toDouble(),
      maxPrice: json['maxPrice']?.toDouble(),
      minStarRating: json['minStarRating'],
      requiredAmenities: List<String>.from(json['requiredAmenities'] ?? []),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'searchTerm': searchTerm,
      'city': city,
      'checkIn': checkIn?.toIso8601String(),
      'checkOut': checkOut?.toIso8601String(),
      'guestsCount': guestsCount,
      'propertyTypeId': propertyTypeId,
      'minPrice': minPrice,
      'maxPrice': maxPrice,
      'minStarRating': minStarRating,
      'requiredAmenities': requiredAmenities,
    };
  }

  @override
  List<Object?> get props => [
    searchTerm,
    city,
    checkIn,
    checkOut,
    guestsCount,
    propertyTypeId,
    minPrice,
    maxPrice,
    minStarRating,
    requiredAmenities,
  ];
}

/// <summary>
/// نموذج بيانات إحصائيات البحث
/// Search statistics data model
/// </summary>
class SearchStatisticsDto extends Equatable {
  /// <summary>
  /// عدد العقارات المطابقة تماماً
  /// Exact matches count
  /// </summary>
  final int exactMatches;

  /// <summary>
  /// عدد العقارات المطابقة جزئياً
  /// Partial matches count
  /// </summary>
  final int partialMatches;

  /// <summary>
  /// عدد العقارات المتاحة
  /// Available properties count
  /// </summary>
  final int availableCount;

  /// <summary>
  /// متوسط السعر
  /// Average price
  /// </summary>
  final double averagePrice;

  /// <summary>
  /// السعر الأدنى
  /// Minimum price
  /// </summary>
  final double minPrice;

  /// <summary>
  /// السعر الأقصى
  /// Maximum price
  /// </summary>
  final double maxPrice;

  const SearchStatisticsDto({
    required this.exactMatches,
    required this.partialMatches,
    required this.availableCount,
    required this.averagePrice,
    required this.minPrice,
    required this.maxPrice,
  });

  factory SearchStatisticsDto.fromJson(Map<String, dynamic> json) {
    return SearchStatisticsDto(
      exactMatches: json['exactMatches'] ?? 0,
      partialMatches: json['partialMatches'] ?? 0,
      availableCount: json['availableCount'] ?? 0,
      averagePrice: (json['averagePrice'] ?? 0.0).toDouble(),
      minPrice: (json['minPrice'] ?? 0.0).toDouble(),
      maxPrice: (json['maxPrice'] ?? 0.0).toDouble(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'exactMatches': exactMatches,
      'partialMatches': partialMatches,
      'availableCount': availableCount,
      'averagePrice': averagePrice,
      'minPrice': minPrice,
      'maxPrice': maxPrice,
    };
  }

  @override
  List<Object> get props => [exactMatches, partialMatches, availableCount, averagePrice, minPrice, maxPrice];
}

/// <summary>
/// نموذج بيانات نتائج البحث الكاملة (متوافق مع SearchPropertiesResponse)
/// Complete search results data model (compatible with SearchPropertiesResponse)
/// </summary>
class SearchPropertiesResponse extends Equatable {
  /// <summary>
  /// قائمة العقارات المطابقة للبحث
  /// List of properties matching the search
  /// </summary>
  final List<PropertySearchResultDto> properties;

  /// <summary>
  /// العدد الإجمالي للنتائج
  /// Total count of results
  /// </summary>
  final int totalCount;

  /// <summary>
  /// رقم الصفحة الحالية
  /// Current page number
  /// </summary>
  final int currentPage;

  /// <summary>
  /// حجم الصفحة
  /// Page size
  /// </summary>
  final int pageSize;

  /// <summary>
  /// العدد الإجمالي للصفحات
  /// Total pages count
  /// </summary>
  final int totalPages;

  /// <summary>
  /// هل يوجد صفحة سابقة
  /// Whether there is a previous page
  /// </summary>
  final bool hasPreviousPage;

  /// <summary>
  /// هل يوجد صفحة تالية
  /// Whether there is a next page
  /// </summary>
  final bool hasNextPage;

  /// <summary>
  /// فلاتر البحث المطبقة
  /// Applied search filters
  /// </summary>
  final SearchFiltersDto appliedFilters;

  /// <summary>
  /// وقت البحث بالميلي ثانية
  /// Search time in milliseconds
  /// </summary>
  final int searchTimeMs;

  /// <summary>
  /// إحصائيات البحث
  /// Search statistics
  /// </summary>
  final SearchStatisticsDto statistics;

  const SearchPropertiesResponse({
    required this.properties,
    required this.totalCount,
    required this.currentPage,
    required this.pageSize,
    required this.totalPages,
    required this.hasPreviousPage,
    required this.hasNextPage,
    required this.appliedFilters,
    required this.searchTimeMs,
    required this.statistics,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory SearchPropertiesResponse.fromJson(Map<String, dynamic> json) {
    return SearchPropertiesResponse(
      properties: (json['properties'] as List<dynamic>?)
          ?.map((item) => PropertySearchResultDto.fromJson(item))
          .toList() ?? [],
      totalCount: json['totalCount'] ?? 0,
      currentPage: json['currentPage'] ?? 1,
      pageSize: json['pageSize'] ?? 20,
      totalPages: json['totalPages'] ?? 1,
      hasPreviousPage: json['hasPreviousPage'] ?? false,
      hasNextPage: json['hasNextPage'] ?? false,
      appliedFilters: SearchFiltersDto.fromJson(json['appliedFilters'] ?? {}),
      searchTimeMs: json['searchTimeMs'] ?? 0,
      statistics: SearchStatisticsDto.fromJson(json['statistics'] ?? {}),
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
  Map<String, dynamic> toJson() {
    return {
      'properties': properties.map((item) => item.toJson()).toList(),
      'totalCount': totalCount,
      'currentPage': currentPage,
      'pageSize': pageSize,
      'totalPages': totalPages,
      'hasPreviousPage': hasPreviousPage,
      'hasNextPage': hasNextPage,
      'appliedFilters': appliedFilters.toJson(),
      'searchTimeMs': searchTimeMs,
      'statistics': statistics.toJson(),
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  SearchResult toEntity() {
    return SearchResult(
      properties: properties.map((item) => item.toEntity()).toList(),
      totalCount: totalCount,
      currentPage: currentPage,
      pageSize: pageSize,
      totalPages: totalPages,
      hasPreviousPage: hasPreviousPage,
      hasNextPage: hasNextPage,
      appliedFilters: appliedFilters,
      searchTimeMs: searchTimeMs,
      statistics: statistics,
    );
  }

  @override
  List<Object> get props => [
    properties,
    totalCount,
    currentPage,
    pageSize,
    totalPages,
    hasPreviousPage,
    hasNextPage,
    appliedFilters,
    searchTimeMs,
    statistics,
  ];
}