import 'package:equatable/equatable.dart';

/// <summary>
/// كيان فلتر البحث
/// Search filter entity
/// </summary>
class SearchFilter extends Equatable {
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
  /// قائمة معرفات المرافق المطلوبة
  /// Required amenities IDs list
  /// </summary>
  final List<String> requiredAmenities;

  /// <summary>
  /// معرف نوع الوحدة المطلوب
  /// Required unit type ID
  /// </summary>
  final String? unitTypeId;

  /// <summary>
  /// معرفات الخدمات المطلوبة
  /// Required service IDs
  /// </summary>
  final List<String> serviceIds;

  /// <summary>
  /// فلاتر الحقول الديناميكية
  /// Dynamic field filters
  /// </summary>
  final Map<String, dynamic> dynamicFieldFilters;

  /// <summary>
  /// خط العرض للبحث بالموقع
  /// Latitude for location search
  /// </summary>
  final double? latitude;

  /// <summary>
  /// خط الطول للبحث بالموقع
  /// Longitude for location search
  /// </summary>
  final double? longitude;

  /// <summary>
  /// نصف قطر البحث بالكيلومتر
  /// Search radius in kilometers
  /// </summary>
  final int? radiusKm;

  /// <summary>
  /// ترتيب النتائج
  /// Sort order
  /// </summary>
  final String? sortBy;

  /// <summary>
  /// رقم الصفحة
  /// Page number
  /// </summary>
  final int pageNumber;

  /// <summary>
  /// حجم الصفحة
  /// Page size
  /// </summary>
  final int pageSize;

  const SearchFilter({
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
    this.unitTypeId,
    this.serviceIds = const [],
    this.dynamicFieldFilters = const {},
    this.latitude,
    this.longitude,
    this.radiusKm,
    this.sortBy,
    this.pageNumber = 1,
    this.pageSize = 20,
  });

  /// <summary>
  /// إنشاء نسخة محدثة من الكيان
  /// Create updated copy of the entity
  /// </summary>
  SearchFilter copyWith({
    String? searchTerm,
    String? city,
    DateTime? checkIn,
    DateTime? checkOut,
    int? guestsCount,
    String? propertyTypeId,
    double? minPrice,
    double? maxPrice,
    int? minStarRating,
    List<String>? requiredAmenities,
    String? unitTypeId,
    List<String>? serviceIds,
    Map<String, dynamic>? dynamicFieldFilters,
    double? latitude,
    double? longitude,
    int? radiusKm,
    String? sortBy,
    int? pageNumber,
    int? pageSize,
  }) {
    return SearchFilter(
      searchTerm: searchTerm ?? this.searchTerm,
      city: city ?? this.city,
      checkIn: checkIn ?? this.checkIn,
      checkOut: checkOut ?? this.checkOut,
      guestsCount: guestsCount ?? this.guestsCount,
      propertyTypeId: propertyTypeId ?? this.propertyTypeId,
      minPrice: minPrice ?? this.minPrice,
      maxPrice: maxPrice ?? this.maxPrice,
      minStarRating: minStarRating ?? this.minStarRating,
      requiredAmenities: requiredAmenities ?? this.requiredAmenities,
      unitTypeId: unitTypeId ?? this.unitTypeId,
      serviceIds: serviceIds ?? this.serviceIds,
      dynamicFieldFilters: dynamicFieldFilters ?? this.dynamicFieldFilters,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      radiusKm: radiusKm ?? this.radiusKm,
      sortBy: sortBy ?? this.sortBy,
      pageNumber: pageNumber ?? this.pageNumber,
      pageSize: pageSize ?? this.pageSize,
    );
  }

  /// <summary>
  /// التحقق من وجود فلاتر مطبقة
  /// Check if filters are applied
  /// </summary>
  bool hasFilters() {
    return searchTerm != null ||
        city != null ||
        checkIn != null ||
        checkOut != null ||
        guestsCount != null ||
        propertyTypeId != null ||
        minPrice != null ||
        maxPrice != null ||
        minStarRating != null ||
        requiredAmenities.isNotEmpty ||
        unitTypeId != null ||
        serviceIds.isNotEmpty ||
        dynamicFieldFilters.isNotEmpty ||
        latitude != null ||
        longitude != null ||
        radiusKm != null;
  }

  /// <summary>
  /// التحقق من وجود فلتر موقع
  /// Check if location filter is applied
  /// </summary>
  bool hasLocationFilter() {
    return latitude != null && longitude != null;
  }

  /// <summary>
  /// التحقق من وجود فلتر تاريخ
  /// Check if date filter is applied
  /// </summary>
  bool hasDateFilter() {
    return checkIn != null && checkOut != null;
  }

  /// <summary>
  /// التحقق من وجود فلتر سعر
  /// Check if price filter is applied
  /// </summary>
  bool hasPriceFilter() {
    return minPrice != null || maxPrice != null;
  }

  /// <summary>
  /// حساب عدد الليالي
  /// Calculate number of nights
  /// </summary>
  int? calculateNights() {
    if (checkIn != null && checkOut != null) {
      return checkOut!.difference(checkIn!).inDays;
    }
    return null;
  }

  /// <summary>
  /// تنسيق معايير البحث للعرض
  /// Format search criteria for display
  /// </summary>
  String getDisplayString() {
    List<String> criteria = [];
    
    if (searchTerm?.isNotEmpty == true) {
      criteria.add('البحث: $searchTerm');
    }
    
    if (city?.isNotEmpty == true) {
      criteria.add('المدينة: $city');
    }
    
    if (hasDateFilter()) {
      final nights = calculateNights();
      criteria.add('$nights ${nights == 1 ? 'ليلة' : 'ليالي'}');
    }
    
    if (guestsCount != null) {
      criteria.add('$guestsCount ${guestsCount == 1 ? 'ضيف' : 'ضيوف'}');
    }
    
    if (hasPriceFilter()) {
      if (minPrice != null && maxPrice != null) {
        criteria.add('السعر: $minPrice - $maxPrice');
      } else if (minPrice != null) {
        criteria.add('السعر من: $minPrice');
      } else if (maxPrice != null) {
        criteria.add('السعر حتى: $maxPrice');
      }
    }
    
    return criteria.isEmpty ? 'جميع العقارات' : criteria.join(' • ');
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
    unitTypeId,
    serviceIds,
    dynamicFieldFilters,
    latitude,
    longitude,
    radiusKm,
    sortBy,
    pageNumber,
    pageSize,
  ];
}