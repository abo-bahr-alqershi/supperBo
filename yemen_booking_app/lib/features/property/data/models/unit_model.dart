import 'package:equatable/equatable.dart';
import '../../domain/entities/unit.dart';

/// <summary>
/// نموذج بيانات مواصفات الوحدة
/// Unit specifications data model
/// </summary>
class UnitSpecificationsModel extends Equatable {
  /// <summary>
  /// المساحة بالمتر المربع
  /// Area in square meters
  /// </summary>
  final double? area;

  /// <summary>
  /// عدد الغرف
  /// Number of rooms
  /// </summary>
  final int? roomsCount;

  /// <summary>
  /// عدد غرف النوم
  /// Number of bedrooms
  /// </summary>
  final int? bedroomsCount;

  /// <summary>
  /// عدد الحمامات
  /// Number of bathrooms
  /// </summary>
  final int? bathroomsCount;

  /// <summary>
  /// الحد الأقصى للضيوف
  /// Maximum guests capacity
  /// </summary>
  final int maxGuestsCapacity;

  /// <summary>
  /// نوع السرير
  /// Bed type
  /// </summary>
  final String? bedType;

  /// <summary>
  /// عدد الأسرة
  /// Number of beds
  /// </summary>
  final int? bedsCount;

  /// <summary>
  /// الطابق
  /// Floor number
  /// </summary>
  final int? floorNumber;

  /// <summary>
  /// هل يوجد شرفة
  /// Has balcony
  /// </summary>
  final bool hasBalcony;

  /// <summary>
  /// هل يوجد مطبخ
  /// Has kitchen
  /// </summary>
  final bool hasKitchen;

  /// <summary>
  /// نوع المنظر
  /// View type
  /// </summary>
  final String? viewType;

  const UnitSpecificationsModel({
    this.area,
    this.roomsCount,
    this.bedroomsCount,
    this.bathroomsCount,
    required this.maxGuestsCapacity,
    this.bedType,
    this.bedsCount,
    this.floorNumber,
    required this.hasBalcony,
    required this.hasKitchen,
    this.viewType,
  });

  factory UnitSpecificationsModel.fromJson(Map<String, dynamic> json) {
    return UnitSpecificationsModel(
      area: json['area']?.toDouble(),
      roomsCount: json['roomsCount'],
      bedroomsCount: json['bedroomsCount'],
      bathroomsCount: json['bathroomsCount'],
      maxGuestsCapacity: json['maxGuestsCapacity'] ?? 1,
      bedType: json['bedType'],
      bedsCount: json['bedsCount'],
      floorNumber: json['floorNumber'],
      hasBalcony: json['hasBalcony'] ?? false,
      hasKitchen: json['hasKitchen'] ?? false,
      viewType: json['viewType'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'area': area,
      'roomsCount': roomsCount,
      'bedroomsCount': bedroomsCount,
      'bathroomsCount': bathroomsCount,
      'maxGuestsCapacity': maxGuestsCapacity,
      'bedType': bedType,
      'bedsCount': bedsCount,
      'floorNumber': floorNumber,
      'hasBalcony': hasBalcony,
      'hasKitchen': hasKitchen,
      'viewType': viewType,
    };
  }

  @override
  List<Object?> get props => [
    area,
    roomsCount,
    bedroomsCount,
    bathroomsCount,
    maxGuestsCapacity,
    bedType,
    bedsCount,
    floorNumber,
    hasBalcony,
    hasKitchen,
    viewType,
  ];
}

/// <summary>
/// نموذج بيانات أسعار الوحدة
/// Unit pricing data model
/// </summary>
class UnitPricingModel extends Equatable {
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
  /// أسعار خاصة للمواسم
  /// Seasonal pricing
  /// </summary>
  final Map<String, double> seasonalPricing;

  /// <summary>
  /// خصومات الإقامة الطويلة
  /// Long stay discounts
  /// </summary>
  final Map<String, double> longStayDiscounts;

  /// <summary>
  /// رسوم إضافية
  /// Additional fees
  /// </summary>
  final Map<String, double> additionalFees;

  /// <summary>
  /// هل يشمل الضرائب
  /// Includes taxes
  /// </summary>
  final bool includesTaxes;

  /// <summary>
  /// نسبة الضريبة
  /// Tax percentage
  /// </summary>
  final double? taxPercentage;

  const UnitPricingModel({
    required this.basePricePerNight,
    required this.currency,
    required this.seasonalPricing,
    required this.longStayDiscounts,
    required this.additionalFees,
    required this.includesTaxes,
    this.taxPercentage,
  });

  factory UnitPricingModel.fromJson(Map<String, dynamic> json) {
    return UnitPricingModel(
      basePricePerNight: (json['basePricePerNight'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'YER',
      seasonalPricing: Map<String, double>.from(json['seasonalPricing'] ?? {}),
      longStayDiscounts: Map<String, double>.from(json['longStayDiscounts'] ?? {}),
      additionalFees: Map<String, double>.from(json['additionalFees'] ?? {}),
      includesTaxes: json['includesTaxes'] ?? false,
      taxPercentage: json['taxPercentage']?.toDouble(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'basePricePerNight': basePricePerNight,
      'currency': currency,
      'seasonalPricing': seasonalPricing,
      'longStayDiscounts': longStayDiscounts,
      'additionalFees': additionalFees,
      'includesTaxes': includesTaxes,
      'taxPercentage': taxPercentage,
    };
  }

  @override
  List<Object?> get props => [
    basePricePerNight,
    currency,
    seasonalPricing,
    longStayDiscounts,
    additionalFees,
    includesTaxes,
    taxPercentage,
  ];
}

/// <summary>
/// نموذج بيانات الوحدة
/// Unit data model
/// </summary>
class UnitModel extends Equatable {
  /// <summary>
  /// معرف الوحدة
  /// Unit ID
  /// </summary>
  final String id;

  /// <summary>
  /// اسم الوحدة
  /// Unit name
  /// </summary>
  final String name;

  /// <summary>
  /// وصف الوحدة
  /// Unit description
  /// </summary>
  final String description;

  /// <summary>
  /// نوع الوحدة
  /// Unit type
  /// </summary>
  final String unitType;

  /// <summary>
  /// معرف نوع الوحدة
  /// Unit type ID
  /// </summary>
  final String unitTypeId;

  /// <summary>
  /// مواصفات الوحدة
  /// Unit specifications
  /// </summary>
  final UnitSpecificationsModel specifications;

  /// <summary>
  /// تسعير الوحدة
  /// Unit pricing
  /// </summary>
  final UnitPricingModel pricing;

  /// <summary>
  /// صور الوحدة
  /// Unit images
  /// </summary>
  final List<String> images;

  /// <summary>
  /// المرافق الخاصة بالوحدة
  /// Unit amenities
  /// </summary>
  final List<String> amenities;

  /// <summary>
  /// هل الوحدة متاحة
  /// Is unit available
  /// </summary>
  final bool isAvailable;

  /// <summary>
  /// هل مُفعلة للحجز
  /// Is enabled for booking
  /// </summary>
  final bool isBookingEnabled;

  /// <summary>
  /// تواريخ غير متاحة
  /// Unavailable dates
  /// </summary>
  final List<String> unavailableDates;

  /// <summary>
  /// الحد الأدنى لليالي
  /// Minimum nights stay
  /// </summary>
  final int minNightsStay;

  /// <summary>
  /// الحد الأقصى لليالي
  /// Maximum nights stay
  /// </summary>
  final int? maxNightsStay;

  /// <summary>
  /// وقت تسجيل الوصول
  /// Check-in time
  /// </summary>
  final String? checkInTime;

  /// <summary>
  /// وقت تسجيل المغادرة
  /// Check-out time
  /// </summary>
  final String? checkOutTime;

  /// <summary>
  /// معلومات إضافية
  /// Additional information
  /// </summary>
  final Map<String, dynamic> additionalInfo;

  /// <summary>
  /// رقم الوحدة
  /// Unit number
  /// </summary>
  final String? unitNumber;

  /// <summary>
  /// ترتيب العرض
  /// Display order
  /// </summary>
  final int displayOrder;

  const UnitModel({
    required this.id,
    required this.name,
    required this.description,
    required this.unitType,
    required this.unitTypeId,
    required this.specifications,
    required this.pricing,
    required this.images,
    required this.amenities,
    required this.isAvailable,
    required this.isBookingEnabled,
    required this.unavailableDates,
    required this.minNightsStay,
    this.maxNightsStay,
    this.checkInTime,
    this.checkOutTime,
    required this.additionalInfo,
    this.unitNumber,
    required this.displayOrder,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory UnitModel.fromJson(Map<String, dynamic> json) {
    return UnitModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      unitType: json['unitType'] ?? '',
      unitTypeId: json['unitTypeId'] ?? '',
      specifications: UnitSpecificationsModel.fromJson(json['specifications'] ?? {}),
      pricing: UnitPricingModel.fromJson(json['pricing'] ?? {}),
      images: List<String>.from(json['images'] ?? []),
      amenities: List<String>.from(json['amenities'] ?? []),
      isAvailable: json['isAvailable'] ?? true,
      isBookingEnabled: json['isBookingEnabled'] ?? true,
      unavailableDates: List<String>.from(json['unavailableDates'] ?? []),
      minNightsStay: json['minNightsStay'] ?? 1,
      maxNightsStay: json['maxNightsStay'],
      checkInTime: json['checkInTime'],
      checkOutTime: json['checkOutTime'],
      additionalInfo: Map<String, dynamic>.from(json['additionalInfo'] ?? {}),
      unitNumber: json['unitNumber'],
      displayOrder: json['displayOrder'] ?? 0,
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
      'unitType': unitType,
      'unitTypeId': unitTypeId,
      'specifications': specifications.toJson(),
      'pricing': pricing.toJson(),
      'images': images,
      'amenities': amenities,
      'isAvailable': isAvailable,
      'isBookingEnabled': isBookingEnabled,
      'unavailableDates': unavailableDates,
      'minNightsStay': minNightsStay,
      'maxNightsStay': maxNightsStay,
      'checkInTime': checkInTime,
      'checkOutTime': checkOutTime,
      'additionalInfo': additionalInfo,
      'unitNumber': unitNumber,
      'displayOrder': displayOrder,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  Unit toEntity() {
    return Unit(
      id: id,
      name: name,
      description: description,
      unitType: unitType,
      unitTypeId: unitTypeId,
      specifications: specifications,
      pricing: pricing,
      images: images,
      amenities: amenities,
      isAvailable: isAvailable,
      isBookingEnabled: isBookingEnabled,
      unavailableDates: unavailableDates,
      minNightsStay: minNightsStay,
      maxNightsStay: maxNightsStay,
      checkInTime: checkInTime,
      checkOutTime: checkOutTime,
      additionalInfo: additionalInfo,
      unitNumber: unitNumber,
      displayOrder: displayOrder,
    );
  }

  @override
  List<Object?> get props => [
    id,
    name,
    description,
    unitType,
    unitTypeId,
    specifications,
    pricing,
    images,
    amenities,
    isAvailable,
    isBookingEnabled,
    unavailableDates,
    minNightsStay,
    maxNightsStay,
    checkInTime,
    checkOutTime,
    additionalInfo,
    unitNumber,
    displayOrder,
  ];
}