import 'package:equatable/equatable.dart';
import '../../domain/entities/property_detail.dart';
import 'amenity_model.dart';
import 'unit_model.dart';
import 'review_model.dart';

/// <summary>
/// نموذج بيانات عنوان العقار
/// Property address data model  
/// </summary>
class PropertyAddressModel extends Equatable {
  /// <summary>
  /// الشارع
  /// Street
  /// </summary>
  final String street;

  /// <summary>
  /// المدينة
  /// City
  /// </summary>
  final String city;

  /// <summary>
  /// المنطقة/الحي
  /// District/Neighborhood
  /// </summary>
  final String district;

  /// <summary>
  /// الرمز البريدي
  /// Postal code
  /// </summary>
  final String? postalCode;

  /// <summary>
  /// البلد
  /// Country
  /// </summary>
  final String country;

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

  const PropertyAddressModel({
    required this.street,
    required this.city,
    required this.district,
    this.postalCode,
    required this.country,
    this.latitude,
    this.longitude,
  });

  factory PropertyAddressModel.fromJson(Map<String, dynamic> json) {
    return PropertyAddressModel(
      street: json['street'] ?? '',
      city: json['city'] ?? '',
      district: json['district'] ?? '',
      postalCode: json['postalCode'],
      country: json['country'] ?? '',
      latitude: json['latitude']?.toDouble(),
      longitude: json['longitude']?.toDouble(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'street': street,
      'city': city,
      'district': district,
      'postalCode': postalCode,
      'country': country,
      'latitude': latitude,
      'longitude': longitude,
    };
  }

  @override
  List<Object?> get props => [street, city, district, postalCode, country, latitude, longitude];
}

/// <summary>
/// نموذج بيانات معلومات الاتصال
/// Contact information data model
/// </summary>
class PropertyContactModel extends Equatable {
  /// <summary>
  /// رقم الهاتف الرئيسي
  /// Primary phone number
  /// </summary>
  final String? primaryPhone;

  /// <summary>
  /// رقم الهاتف الثانوي
  /// Secondary phone number
  /// </summary>
  final String? secondaryPhone;

  /// <summary>
  /// البريد الإلكتروني
  /// Email address
  /// </summary>
  final String? email;

  /// <summary>
  /// الموقع الإلكتروني
  /// Website URL
  /// </summary>
  final String? website;

  const PropertyContactModel({
    this.primaryPhone,
    this.secondaryPhone,
    this.email,
    this.website,
  });

  factory PropertyContactModel.fromJson(Map<String, dynamic> json) {
    return PropertyContactModel(
      primaryPhone: json['primaryPhone'],
      secondaryPhone: json['secondaryPhone'],
      email: json['email'],
      website: json['website'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'primaryPhone': primaryPhone,
      'secondaryPhone': secondaryPhone,
      'email': email,
      'website': website,
    };
  }

  @override
  List<Object?> get props => [primaryPhone, secondaryPhone, email, website];
}

/// <summary>
/// نموذج بيانات تفاصيل العقار الكاملة
/// Complete property details data model
/// </summary>
class PropertyDetailModel extends Equatable {
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
  final List<AmenityModel> amenities;

  /// <summary>
  /// الوحدات المتاحة
  /// Available units
  /// </summary>
  final List<UnitModel> units;

  /// <summary>
  /// أحدث التقييمات
  /// Recent reviews
  /// </summary>
  final List<ReviewModel> recentReviews;

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

  const PropertyDetailModel({
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
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory PropertyDetailModel.fromJson(Map<String, dynamic> json) {
    return PropertyDetailModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      shortDescription: json['shortDescription'] ?? '',
      propertyType: json['propertyType'] ?? '',
      propertyTypeId: json['propertyTypeId'] ?? '',
      starRating: json['starRating'] ?? 0,
      averageRating: (json['averageRating'] ?? 0.0).toDouble(),
      reviewsCount: json['reviewsCount'] ?? 0,
      address: PropertyAddressModel.fromJson(json['address'] ?? {}),
      contact: PropertyContactModel.fromJson(json['contact'] ?? {}),
      mainImageUrl: json['mainImageUrl'] ?? '',
      imageGallery: List<String>.from(json['imageGallery'] ?? []),
      amenities: (json['amenities'] as List<dynamic>?)
          ?.map((item) => AmenityModel.fromJson(item))
          .toList() ?? [],
      units: (json['units'] as List<dynamic>?)
          ?.map((item) => UnitModel.fromJson(item))
          .toList() ?? [],
      recentReviews: (json['recentReviews'] as List<dynamic>?)
          ?.map((item) => ReviewModel.fromJson(item))
          .toList() ?? [],
      policies: Map<String, dynamic>.from(json['policies'] ?? {}),
      operatingHours: Map<String, String>.from(json['operatingHours'] ?? {}),
      isAvailable: json['isAvailable'] ?? true,
      isFavorite: json['isFavorite'] ?? false,
      basePricePerNight: (json['basePricePerNight'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'YER',
      lastUpdated: DateTime.parse(json['lastUpdated'] ?? DateTime.now().toIso8601String()),
      additionalInfo: Map<String, dynamic>.from(json['additionalInfo'] ?? {}),
      additionalServices: List<Map<String, dynamic>>.from(json['additionalServices'] ?? []),
      licenseNumber: json['licenseNumber'],
      isVerified: json['isVerified'] ?? false,
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
      'description': description,
      'shortDescription': shortDescription,
      'propertyType': propertyType,
      'propertyTypeId': propertyTypeId,
      'starRating': starRating,
      'averageRating': averageRating,
      'reviewsCount': reviewsCount,
      'address': address.toJson(),
      'contact': contact.toJson(),
      'mainImageUrl': mainImageUrl,
      'imageGallery': imageGallery,
      'amenities': amenities.map((item) => item.toJson()).toList(),
      'units': units.map((item) => item.toJson()).toList(),
      'recentReviews': recentReviews.map((item) => item.toJson()).toList(),
      'policies': policies,
      'operatingHours': operatingHours,
      'isAvailable': isAvailable,
      'isFavorite': isFavorite,
      'basePricePerNight': basePricePerNight,
      'currency': currency,
      'lastUpdated': lastUpdated.toIso8601String(),
      'additionalInfo': additionalInfo,
      'additionalServices': additionalServices,
      'licenseNumber': licenseNumber,
      'isVerified': isVerified,
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
      description: description,
      shortDescription: shortDescription,
      propertyType: propertyType,
      propertyTypeId: propertyTypeId,
      starRating: starRating,
      averageRating: averageRating,
      reviewsCount: reviewsCount,
      address: address,
      contact: contact,
      mainImageUrl: mainImageUrl,
      imageGallery: imageGallery,
      amenities: amenities.map((item) => item.toEntity()).toList(),
      units: units.map((item) => item.toEntity()).toList(),
      recentReviews: recentReviews.map((item) => item.toEntity()).toList(),
      policies: policies,
      operatingHours: operatingHours,
      isAvailable: isAvailable,
      isFavorite: isFavorite,
      basePricePerNight: basePricePerNight,
      currency: currency,
      lastUpdated: lastUpdated,
      additionalInfo: additionalInfo,
      additionalServices: additionalServices,
      licenseNumber: licenseNumber,
      isVerified: isVerified,
    );
  }

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