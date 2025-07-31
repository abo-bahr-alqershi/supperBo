import 'package:equatable/equatable.dart';

/// <summary>
/// كيان المرفق/الخدمة
/// Amenity entity
/// </summary>
class Amenity extends Equatable {
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

  const Amenity({
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
  /// الحصول على الاسم المناسب حسب اللغة
  /// Get appropriate name based on language
  /// </summary>
  String getLocalizedName([String language = 'ar']) {
    return language == 'en' ? nameEn : name;
  }

  /// <summary>
  /// الحصول على الوصف المناسب حسب اللغة
  /// Get appropriate description based on language
  /// </summary>
  String getLocalizedDescription([String language = 'ar']) {
    return language == 'en' ? descriptionEn : description;
  }

  /// <summary>
  /// تنسيق السعر الإضافي للعرض
  /// Format additional price for display
  /// </summary>
  String? get formattedAdditionalPrice {
    if (!isFree && additionalPrice != null) {
      return '${additionalPrice!.toStringAsFixed(0)} ${priceCurrency ?? 'YER'}';
    }
    return null;
  }

  /// <summary>
  /// الحصول على وصف النوع
  /// Get type description
  /// </summary>
  String get typeDescription {
    switch (amenityType.toLowerCase()) {
      case 'free':
        return 'مجاني';
      case 'paid':
        return 'مدفوع';
      case 'on-request':
        return 'عند الطلب';
      default:
        return amenityType;
    }
  }

  /// <summary>
  /// الحصول على وصف الفئة
  /// Get category description
  /// </summary>
  String get categoryDescription {
    switch (category.toLowerCase()) {
      case 'connectivity':
        return 'الاتصال والإنترنت';
      case 'food':
        return 'الطعام والشراب';
      case 'entertainment':
        return 'الترفيه';
      case 'wellness':
        return 'الصحة والعافية';
      case 'business':
        return 'الأعمال';
      case 'transportation':
        return 'النقل والمواصلات';
      case 'safety':
        return 'الأمان والسلامة';
      case 'accessibility':
        return 'إمكانية الوصول';
      case 'cleaning':
        return 'التنظيف';
      case 'comfort':
        return 'الراحة';
      default:
        return category;
    }
  }

  /// <summary>
  /// التحقق من التوفر في وقت معين
  /// Check if available at specific time
  /// </summary>
  bool isAvailableAt(DateTime dateTime) {
    if (!isAvailable) return false;
    
    if (availabilityHours == null) return true;
    
    final dayOfWeek = _getDayOfWeek(dateTime.weekday);
    final timeSlot = availabilityHours![dayOfWeek];
    
    if (timeSlot == null || timeSlot.isEmpty) return false;
    
    // التحقق من أوقات التوفر (تحتاج لتطوير أكثر تفصيلاً)
    return true;
  }

  /// <summary>
  /// الحصول على اسم اليوم
  /// Get day name
  /// </summary>
  String _getDayOfWeek(int weekday) {
    switch (weekday) {
      case 1: return 'monday';
      case 2: return 'tuesday';
      case 3: return 'wednesday';
      case 4: return 'thursday';
      case 5: return 'friday';
      case 6: return 'saturday';
      case 7: return 'sunday';
      default: return 'monday';
    }
  }

  /// <summary>
  /// التحقق من إمكانية الحجز المسبق
  /// Check if advance booking is possible
  /// </summary>
  bool canBookInAdvance(DateTime requestedTime) {
    if (!requiresAdvanceBooking) return true;
    if (advanceBookingHours == null) return false;
    
    final now = DateTime.now();
    final hoursUntil = requestedTime.difference(now).inHours;
    
    return hoursUntil >= advanceBookingHours!;
  }

  /// <summary>
  /// الحصول على الحد الأدنى لوقت الحجز المسبق
  /// Get minimum advance booking time
  /// </summary>
  String get minimumAdvanceBookingDescription {
    if (!requiresAdvanceBooking || advanceBookingHours == null) {
      return 'غير مطلوب';
    }
    
    if (advanceBookingHours! < 24) {
      return '$advanceBookingHours ساعة مسبقاً';
    } else {
      final days = (advanceBookingHours! / 24).round();
      return '$days ${days == 1 ? 'يوم' : 'أيام'} مسبقاً';
    }
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