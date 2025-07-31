import 'package:equatable/equatable.dart';
import '../../domain/entities/search_filter.dart';

/// <summary>
/// نموذج بيانات فلتر البحث
/// Search filter data model
/// </summary>
class SearchFilterModel extends Equatable {
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

  const SearchFilterModel({
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
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory SearchFilterModel.fromJson(Map<String, dynamic> json) {
    return SearchFilterModel(
      searchTerm: json['searchTerm'],
      city: json['city'],
      checkIn: json['checkIn'] != null 
          ? DateTime.parse(json['checkIn']) 
          : null,
      checkOut: json['checkOut'] != null 
          ? DateTime.parse(json['checkOut']) 
          : null,
      guestsCount: json['guestsCount'],
      propertyTypeId: json['propertyTypeId'],
      minPrice: json['minPrice']?.toDouble(),
      maxPrice: json['maxPrice']?.toDouble(),
      minStarRating: json['minStarRating'],
      requiredAmenities: List<String>.from(json['requiredAmenities'] ?? []),
      unitTypeId: json['unitTypeId'],
      serviceIds: List<String>.from(json['serviceIds'] ?? []),
      dynamicFieldFilters: Map<String, dynamic>.from(json['dynamicFieldFilters'] ?? {}),
      latitude: json['latitude']?.toDouble(),
      longitude: json['longitude']?.toDouble(),
      radiusKm: json['radiusKm'],
      sortBy: json['sortBy'],
      pageNumber: json['pageNumber'] ?? 1,
      pageSize: json['pageSize'] ?? 20,
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
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
      'unitTypeId': unitTypeId,
      'serviceIds': serviceIds,
      'dynamicFieldFilters': dynamicFieldFilters,
      'latitude': latitude,
      'longitude': longitude,
      'radiusKm': radiusKm,
      'sortBy': sortBy,
      'pageNumber': pageNumber,
      'pageSize': pageSize,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  SearchFilter toEntity() {
    return SearchFilter(
      searchTerm: searchTerm,
      city: city,
      checkIn: checkIn,
      checkOut: checkOut,
      guestsCount: guestsCount,
      propertyTypeId: propertyTypeId,
      minPrice: minPrice,
      maxPrice: maxPrice,
      minStarRating: minStarRating,
      requiredAmenities: requiredAmenities,
      unitTypeId: unitTypeId,
      serviceIds: serviceIds,
      dynamicFieldFilters: dynamicFieldFilters,
      latitude: latitude,
      longitude: longitude,
      radiusKm: radiusKm,
      sortBy: sortBy,
      pageNumber: pageNumber,
      pageSize: pageSize,
    );
  }

  /// <summary>
  /// إنشاء نسخة محدثة من النموذج
  /// Create updated copy of the model
  /// </summary>
  SearchFilterModel copyWith({
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
    return SearchFilterModel(
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