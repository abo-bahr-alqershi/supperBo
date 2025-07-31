import 'package:equatable/equatable.dart';
import '../../domain/entities/property_detail.dart';

/// <summary>
/// نموذج بيانات العرض الخاص
/// Special offer data model
/// </summary>
class ClientSpecialOfferDto extends Equatable {
  /// <summary>
  /// عنوان العرض
  /// Offer title
  /// </summary>
  final String title;

  /// <summary>
  /// وصف العرض
  /// Offer description
  /// </summary>
  final String description;

  /// <summary>
  /// نسبة الخصم
  /// Discount percentage
  /// </summary>
  final double discountPercentage;

  /// <summary>
  /// مبلغ الخصم
  /// Discount amount
  /// </summary>
  final double discountAmount;

  /// <summary>
  /// تاريخ انتهاء العرض
  /// Offer expiry date
  /// </summary>
  final DateTime? expiryDate;

  /// <summary>
  /// لون العرض (للتصميم)
  /// Offer color (for design)
  /// </summary>
  final String color;

  const ClientSpecialOfferDto({
    required this.title,
    required this.description,
    required this.discountPercentage,
    required this.discountAmount,
    this.expiryDate,
    required this.color,
  });

  factory ClientSpecialOfferDto.fromJson(Map<String, dynamic> json) {
    return ClientSpecialOfferDto(
      title: json['title'] ?? '',
      description: json['description'] ?? '',
      discountPercentage: (json['discountPercentage'] ?? 0.0).toDouble(),
      discountAmount: (json['discountAmount'] ?? 0.0).toDouble(),
      expiryDate: json['expiryDate'] != null ? DateTime.parse(json['expiryDate']) : null,
      color: json['color'] ?? '#FF6B6B',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'title': title,
      'description': description,
      'discountPercentage': discountPercentage,
      'discountAmount': discountAmount,
      'expiryDate': expiryDate?.toIso8601String(),
      'color': color,
    };
  }

  @override
  List<Object?> get props => [title, description, discountPercentage, discountAmount, expiryDate, color];
}

/// <summary>
/// نموذج بيانات العقار المميز للعميل (متوافق مع ClientFeaturedPropertyDto)
/// Client featured property data model (compatible with ClientFeaturedPropertyDto)
/// </summary>
class ClientFeaturedPropertyDto extends Equatable {
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
  /// Main image
  /// </summary>
  final String mainImageUrl;

  /// <summary>
  /// معرض الصور (أول 3 صور)
  /// Image gallery (first 3 images)
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
  /// عدد المراجعات
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
  /// Is in favorites
  /// </summary>
  final bool isFavorite;

  /// <summary>
  /// المسافة من الموقع الحالي (بالكيلومتر)
  /// Distance from current location (in km)
  /// </summary>
  final double? distanceKm;

  /// <summary>
  /// نوع العقار
  /// Property type
  /// </summary>
  final String propertyType;

  /// <summary>
  /// أفضل الوسائل والخدمات (أول 3)
  /// Top amenities (first 3)
  /// </summary>
  final List<String> topAmenities;

  /// <summary>
  /// حالة التوفر
  /// Availability status
  /// </summary>
  final String availabilityStatus;

  /// <summary>
  /// عرض خاص (إن وجد)
  /// Special offer (if any)
  /// </summary>
  final ClientSpecialOfferDto? specialOffer;

  /// <summary>
  /// نسبة الحجز (شعبية العقار)
  /// Booking rate (property popularity)
  /// </summary>
  final double bookingRate;

  /// <summary>
  /// تصنيف مميز (مثل: "الأكثر حجزاً"، "جديد"، "موصى به")
  /// Featured badge (e.g., "Most Booked", "New", "Recommended")
  /// </summary>
  final String? featuredBadge;

  const ClientFeaturedPropertyDto({
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
    this.specialOffer,
    required this.bookingRate,
    this.featuredBadge,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory ClientFeaturedPropertyDto.fromJson(Map<String, dynamic> json) {
    return ClientFeaturedPropertyDto(
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
      specialOffer: json['specialOffer'] != null 
          ? ClientSpecialOfferDto.fromJson(json['specialOffer']) 
          : null,
      bookingRate: (json['bookingRate'] ?? 0.0).toDouble(),
      featuredBadge: json['featuredBadge'],
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
      'specialOffer': specialOffer?.toJson(),
      'bookingRate': bookingRate,
      'featuredBadge': featuredBadge,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  PropertyDetail toEntity() {
    return PropertyDetail(
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
      specialOffer: specialOffer,
      bookingRate: bookingRate,
      featuredBadge: featuredBadge,
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
    specialOffer,
    bookingRate,
    featuredBadge,
  ];
}