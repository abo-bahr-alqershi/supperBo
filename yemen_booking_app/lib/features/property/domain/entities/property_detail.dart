import 'package:equatable/equatable.dart';
import '../../data/models/property_detail_model.dart';
import 'amenity.dart';
import 'unit.dart';

/// <summary>
/// كيان تفاصيل العقار
/// Property detail entity
/// </summary>
class PropertyDetail extends Equatable {
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
  /// الوصف التفصيلي
  /// Detailed description
  /// </summary>
  final String description;

  /// <summary>
  /// وصف مختصر
  /// Short description
  /// </summary>
  final String shortDescription;

  /// <summary>
  /// نوع العقار
  /// Property type
  /// </summary>
  final String propertyType;

  /// <summary>
  /// معرف نوع العقار
  /// Property type ID
  /// </summary>
  final String propertyTypeId;

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
  /// عنوان العقار
  /// Property address
  /// </summary>
  final PropertyAddressModel address;

  /// <summary>
  /// معلومات الاتصال
  /// Contact information
  /// </summary>
  final PropertyContactModel contact;

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
  /// المرافق والخدمات
  /// Amenities
  /// </summary>
  final List<Amenity> amenities;

  /// <summary>
  /// الوحدات المتاحة
  /// Available units
  /// </summary>
  final List<Unit> units;

  /// <summary>
  /// أحدث التقييمات
  /// Recent reviews
  /// </summary>
  final List<dynamic> recentReviews;

  /// <summary>
  /// سياسات العقار
  /// Property policies
  /// </summary>
  final Map<String, dynamic> policies;

  /// <summary>
  /// ساعات العمل
  /// Operating hours
  /// </summary>
  final Map<String, String> operatingHours;

  /// <summary>
  /// هل العقار متاح
  /// Is property available
  /// </summary>
  final bool isAvailable;

  /// <summary>
  /// هل في المفضلات
  /// Is favorite
  /// </summary>
  final bool isFavorite;

  /// <summary>
  /// السعر الأساسي لليلة
  /// Base price per night
  /// </summary>
  final double basePricePerNight;

  /// <summary>
  /// العملة
  /// Currency
  /// </summary>
  final String currency;

  /// <summary>
  /// تاريخ آخر تحديث
  /// Last updated
  /// </summary>
  final DateTime lastUpdated;

  /// <summary>
  /// معلومات إضافية
  /// Additional information
  /// </summary>
  final Map<String, dynamic> additionalInfo;

  /// <summary>
  /// الخدمات الإضافية
  /// Additional services
  /// </summary>
  final List<Map<String, dynamic>> additionalServices;

  /// <summary>
  /// رقم الترخيص
  /// License number
  /// </summary>
  final String? licenseNumber;

  /// <summary>
  /// حالة التحقق
  /// Verification status
  /// </summary>
  final bool isVerified;

  const PropertyDetail({
    required this.id,
    required this.name,
    required this.description,
    required this.shortDescription,
    required this.propertyType,
    required this.propertyTypeId,
    required this.starRating,
    required this.averageRating,
    required this.reviewsCount,
    required this.address,
    required this.contact,
    required this.mainImageUrl,
    required this.imageGallery,
    required this.amenities,
    required this.units,
    required this.recentReviews,
    required this.policies,
    required this.operatingHours,
    required this.isAvailable,
    required this.isFavorite,
    required this.basePricePerNight,
    required this.currency,
    required this.lastUpdated,
    required this.additionalInfo,
    required this.additionalServices,
    this.licenseNumber,
    required this.isVerified,
  });

  /// <summary>
  /// التحقق من وجود موقع جغرافي
  /// Check if has location coordinates
  /// </summary>
  bool get hasLocation => address.latitude != null && address.longitude != null;

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
  /// الحصول على العنوان الكامل
  /// Get full address
  /// </summary>
  String get fullAddress => '${address.street}، ${address.district}، ${address.city}';

  /// <summary>
  /// الحصول على المرافق الشائعة
  /// Get popular amenities
  /// </summary>
  List<Amenity> get popularAmenities {
    return amenities.where((amenity) => amenity.isPopular).toList();
  }

  /// <summary>
  /// الحصول على المرافق المجانية
  /// Get free amenities
  /// </summary>
  List<Amenity> get freeAmenities {
    return amenities.where((amenity) => amenity.isFree).toList();
  }

  /// <summary>
  /// الحصول على المرافق المدفوعة
  /// Get paid amenities
  /// </summary>
  List<Amenity> get paidAmenities {
    return amenities.where((amenity) => !amenity.isFree).toList();
  }

  /// <summary>
  /// الحصول على الوحدات المتاحة
  /// Get available units
  /// </summary>
  List<Unit> get availableUnits {
    return units.where((unit) => unit.isAvailable && unit.isBookingEnabled).toList();
  }

  /// <summary>
  /// الحصول على أقل سعر للوحدات
  /// Get minimum unit price
  /// </summary>
  double get minUnitPrice {
    if (units.isEmpty) return basePricePerNight;
    return units.map((unit) => unit.pricing.basePricePerNight).reduce((a, b) => a < b ? a : b);
  }

  /// <summary>
  /// الحصول على أعلى سعر للوحدات
  /// Get maximum unit price
  /// </summary>
  double get maxUnitPrice {
    if (units.isEmpty) return basePricePerNight;
    return units.map((unit) => unit.pricing.basePricePerNight).reduce((a, b) => a > b ? a : b);
  }

  /// <summary>
  /// التحقق من توفر خدمة معينة
  /// Check if specific amenity is available
  /// </summary>
  bool hasAmenity(String amenityCode) {
    return amenities.any((amenity) => amenity.code == amenityCode && amenity.isAvailable);
  }

  /// <summary>
  /// الحصول على المرافق حسب الفئة
  /// Get amenities by category
  /// </summary>
  List<Amenity> getAmenitiesByCategory(String category) {
    return amenities.where((amenity) => amenity.category == category).toList();
  }

  /// <summary>
  /// التحقق من إمكانية الحجز
  /// Check if bookable
  /// </summary>
  bool get isBookable => isAvailable && isVerified && availableUnits.isNotEmpty;

  @override
  List<Object?> get props => [
    id,
    name,
    description,
    shortDescription,
    propertyType,
    propertyTypeId,
    starRating,
    averageRating,
    reviewsCount,
    address,
    contact,
    mainImageUrl,
    imageGallery,
    amenities,
    units,
    recentReviews,
    policies,
    operatingHours,
    isAvailable,
    isFavorite,
    basePricePerNight,
    currency,
    lastUpdated,
    additionalInfo,
    additionalServices,
    licenseNumber,
    isVerified,
  ];
}