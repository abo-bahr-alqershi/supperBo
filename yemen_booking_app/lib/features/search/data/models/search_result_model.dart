import 'package:equatable/equatable.dart';
import '../../domain/entities/search_result.dart';

/// <summary>
/// نموذج بيانات نتيجة البحث الفردية
/// Individual search result data model
/// </summary>
class SearchResultItemModel extends Equatable {
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

  const SearchResultItemModel({
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
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory SearchResultItemModel.fromJson(Map<String, dynamic> json) {
    return SearchResultItemModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      shortDescription: json['shortDescription'] ?? '',
      city: json['city'] ?? '',
      address: json['address'] ?? '',
      mainImageUrl: json['mainImageUrl'] ?? '',
      imageGallery: List<String>.from(json['imageGallery'] ?? []),
      starRating: json['starRating'] ?? 0,
      averageRating: (json['averageRating'] ?? 0.0).toDouble(),
      reviewsCount: json['reviewsCount'] ?? 0,
      basePricePerNight: (json['basePricePerNight'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'YER',
      isFavorite: json['isFavorite'] ?? false,
      distanceKm: json['distanceKm']?.toDouble(),
      propertyType: json['propertyType'] ?? '',
      topAmenities: List<String>.from(json['topAmenities'] ?? []),
      availabilityStatus: json['availabilityStatus'] ?? 'Available',
      latitude: json['latitude']?.toDouble(),
      longitude: json['longitude']?.toDouble(),
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
      'shortDescription': shortDescription,
      'city': city,
      'address': address,
      'mainImageUrl': mainImageUrl,
      'imageGallery': imageGallery,
      'starRating': starRating,
      'averageRating': averageRating,
      'reviewsCount': reviewsCount,
      'basePricePerNight': basePricePerNight,
      'currency': currency,
      'isFavorite': isFavorite,
      'distanceKm': distanceKm,
      'propertyType': propertyType,
      'topAmenities': topAmenities,
      'availabilityStatus': availabilityStatus,
      'latitude': latitude,
      'longitude': longitude,
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
      shortDescription: shortDescription,
      city: city,
      address: address,
      mainImageUrl: mainImageUrl,
      imageGallery: imageGallery,
      starRating: starRating,
      averageRating: averageRating,
      reviewsCount: reviewsCount,
      basePricePerNight: basePricePerNight,
      currency: currency,
      isFavorite: isFavorite,
      distanceKm: distanceKm,
      propertyType: propertyType,
      topAmenities: topAmenities,
      availabilityStatus: availabilityStatus,
      latitude: latitude,
      longitude: longitude,
    );
  }

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
/// نموذج بيانات نتائج البحث الكاملة
/// Complete search results data model
/// </summary>
class SearchResultModel extends Equatable {
  /// <summary>
  /// قائمة نتائج البحث
  /// Search results list
  /// </summary>
  final List<SearchResultItemModel> results;

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

  const SearchResultModel({
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
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory SearchResultModel.fromJson(Map<String, dynamic> json) {
    return SearchResultModel(
      results: (json['results'] as List<dynamic>?)
          ?.map((item) => SearchResultItemModel.fromJson(item))
          .toList() ?? [],
      totalCount: json['totalCount'] ?? 0,
      currentPage: json['currentPage'] ?? 1,
      totalPages: json['totalPages'] ?? 1,
      pageSize: json['pageSize'] ?? 20,
      hasNextPage: json['hasNextPage'] ?? false,
      hasPreviousPage: json['hasPreviousPage'] ?? false,
      appliedFilters: Map<String, dynamic>.from(json['appliedFilters'] ?? {}),
      searchTimeMs: json['searchTimeMs'] ?? 0,
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
  Map<String, dynamic> toJson() {
    return {
      'results': results.map((item) => item.toJson()).toList(),
      'totalCount': totalCount,
      'currentPage': currentPage,
      'totalPages': totalPages,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
      'appliedFilters': appliedFilters,
      'searchTimeMs': searchTimeMs,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  SearchResult toEntity() {
    return SearchResult(
      results: results.map((item) => item.toEntity()).toList(),
      totalCount: totalCount,
      currentPage: currentPage,
      totalPages: totalPages,
      pageSize: pageSize,
      hasNextPage: hasNextPage,
      hasPreviousPage: hasPreviousPage,
      appliedFilters: appliedFilters,
      searchTimeMs: searchTimeMs,
    );
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