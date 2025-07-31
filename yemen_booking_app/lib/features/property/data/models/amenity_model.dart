import 'package:equatable/equatable.dart';
import '../../domain/entities/amenity.dart';

/// <summary>
/// نموذج بيانات المرفق/الخدمة
/// Amenity data model
/// </summary>
class AmenityModel extends Equatable {
  /// <summary>
  /// معرف المرفق
  /// Amenity ID
  /// </summary>
  final String id;

  /// <summary>
  /// اسم المرفق
  /// Amenity name
  /// </summary>
  final String name;

  /// <summary>
  /// اسم المرفق بالإنجليزية
  /// Amenity name in English
  /// </summary>
  final String nameEn;

  /// <summary>
  /// وصف المرفق
  /// Amenity description
  /// </summary>
  final String description;

  /// <summary>
  /// وصف المرفق بالإنجليزية
  /// Amenity description in English
  /// </summary>
  final String descriptionEn;

  /// <summary>
  /// أيقونة المرفق
  /// Amenity icon
  /// </summary>
  final String iconUrl;

  /// <summary>
  /// فئة المرفق
  /// Amenity category
  /// </summary>
  final String category;

  /// <summary>
  /// هل المرفق مجاني
  /// Is amenity free
  /// </summary>
  final bool isFree;

  /// <summary>
  /// السعر الإضافي (إن وجد)
  /// Additional price (if any)
  /// </summary>
  final double? additionalPrice;

  /// <summary>
  /// العملة للسعر الإضافي
  /// Currency for additional price
  /// </summary>
  final String? priceCurrency;

  /// <summary>
  /// هل المرفق شائع/مميز
  /// Is amenity popular/featured
  /// </summary>
  final bool isPopular;

  /// <summary>
  /// ترتيب العرض
  /// Display order
  /// </summary>
  final int displayOrder;

  /// <summary>
  /// هل المرفق متاح
  /// Is amenity available
  /// </summary>
  final bool isAvailable;

  /// <summary>
  /// رمز المرفق
  /// Amenity code
  /// </summary>
  final String code;

  /// <summary>
  /// نوع المرفق (مجانى، مدفوع، عند الطلب)
  /// Amenity type (free, paid, on-request)
  /// </summary>
  final String amenityType;

  /// <summary>
  /// معلومات إضافية
  /// Additional information
  /// </summary>
  final Map<String, dynamic> additionalInfo;

  /// <summary>
  /// ساعات التوفر
  /// Availability hours
  /// </summary>
  final Map<String, String>? availabilityHours;

  /// <summary>
  /// هل يحتاج حجز مسبق
  /// Requires advance booking
  /// </summary>
  final bool requiresAdvanceBooking;

  /// <summary>
  /// وقت الحجز المسبق بالساعات
  /// Advance booking hours
  /// </summary>
  final int? advanceBookingHours;

  const AmenityModel({
    required this.id,
    required this.name,
    required this.nameEn,
    required this.description,
    required this.descriptionEn,
    required this.iconUrl,
    required this.category,
    required this.isFree,
    this.additionalPrice,
    this.priceCurrency,
    required this.isPopular,
    required this.displayOrder,
    required this.isAvailable,
    required this.code,
    required this.amenityType,
    required this.additionalInfo,
    this.availabilityHours,
    required this.requiresAdvanceBooking,
    this.advanceBookingHours,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory AmenityModel.fromJson(Map<String, dynamic> json) {
    return AmenityModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      nameEn: json['nameEn'] ?? '',
      description: json['description'] ?? '',
      descriptionEn: json['descriptionEn'] ?? '',
      iconUrl: json['iconUrl'] ?? '',
      category: json['category'] ?? '',
      isFree: json['isFree'] ?? true,
      additionalPrice: json['additionalPrice']?.toDouble(),
      priceCurrency: json['priceCurrency'],
      isPopular: json['isPopular'] ?? false,
      displayOrder: json['displayOrder'] ?? 0,
      isAvailable: json['isAvailable'] ?? true,
      code: json['code'] ?? '',
      amenityType: json['amenityType'] ?? 'free',
      additionalInfo: Map<String, dynamic>.from(json['additionalInfo'] ?? {}),
      availabilityHours: json['availabilityHours'] != null 
          ? Map<String, String>.from(json['availabilityHours']) 
          : null,
      requiresAdvanceBooking: json['requiresAdvanceBooking'] ?? false,
      advanceBookingHours: json['advanceBookingHours'],
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
      'nameEn': nameEn,
      'description': description,
      'descriptionEn': descriptionEn,
      'iconUrl': iconUrl,
      'category': category,
      'isFree': isFree,
      'additionalPrice': additionalPrice,
      'priceCurrency': priceCurrency,
      'isPopular': isPopular,
      'displayOrder': displayOrder,
      'isAvailable': isAvailable,
      'code': code,
      'amenityType': amenityType,
      'additionalInfo': additionalInfo,
      'availabilityHours': availabilityHours,
      'requiresAdvanceBooking': requiresAdvanceBooking,
      'advanceBookingHours': advanceBookingHours,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  Amenity toEntity() {
    return Amenity(
      id: id,
      name: name,
      nameEn: nameEn,
      description: description,
      descriptionEn: descriptionEn,
      iconUrl: iconUrl,
      category: category,
      isFree: isFree,
      additionalPrice: additionalPrice,
      priceCurrency: priceCurrency,
      isPopular: isPopular,
      displayOrder: displayOrder,
      isAvailable: isAvailable,
      code: code,
      amenityType: amenityType,
      additionalInfo: additionalInfo,
      availabilityHours: availabilityHours,
      requiresAdvanceBooking: requiresAdvanceBooking,
      advanceBookingHours: advanceBookingHours,
    );
  }

  @override
  List<Object?> get props => [
    id,
    name,
    nameEn,
    description,
    descriptionEn,
    iconUrl,
    category,
    isFree,
    additionalPrice,
    priceCurrency,
    isPopular,
    displayOrder,
    isAvailable,
    code,
    amenityType,
    additionalInfo,
    availabilityHours,
    requiresAdvanceBooking,
    advanceBookingHours,
  ];
}