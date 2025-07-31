import 'package:equatable/equatable.dart';
import '../../data/models/unit_model.dart';

/// <summary>
/// كيان الوحدة
/// Unit entity
/// </summary>
class Unit extends Equatable {
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

  const Unit({
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
  /// التحقق من قابلية الحجز
  /// Check if bookable
  /// </summary>
  bool get isBookable => isAvailable && isBookingEnabled;

  /// <summary>
  /// تنسيق السعر للعرض
  /// Format price for display
  /// </summary>
  String get formattedPrice {
    return '${pricing.basePricePerNight.toStringAsFixed(0)} ${pricing.currency}';
  }

  /// <summary>
  /// الحصول على وصف المواصفات
  /// Get specifications description
  /// </summary>
  String get specificationsDescription {
    List<String> specs = [];
    
    if (specifications.bedroomsCount != null) {
      specs.add('${specifications.bedroomsCount} غرف نوم');
    }
    
    if (specifications.bathroomsCount != null) {
      specs.add('${specifications.bathroomsCount} حمامات');
    }
    
    if (specifications.area != null) {
      specs.add('${specifications.area} م²');
    }
    
    specs.add('${specifications.maxGuestsCapacity} ضيوف');
    
    return specs.join(' • ');
  }

  /// <summary>
  /// التحقق من توفر الوحدة في تاريخ معين
  /// Check if unit is available on specific date
  /// </summary>
  bool isAvailableOnDate(DateTime date) {
    if (!isAvailable || !isBookingEnabled) return false;
    
    final dateString = date.toIso8601String().split('T')[0];
    return !unavailableDates.contains(dateString);
  }

  /// <summary>
  /// التحقق من توفر الوحدة في فترة معينة
  /// Check if unit is available for a date range
  /// </summary>
  bool isAvailableForDateRange(DateTime checkIn, DateTime checkOut) {
    if (!isAvailable || !isBookingEnabled) return false;
    
    final nights = checkOut.difference(checkIn).inDays;
    if (nights < minNightsStay) return false;
    if (maxNightsStay != null && nights > maxNightsStay!) return false;
    
    DateTime currentDate = checkIn;
    while (currentDate.isBefore(checkOut)) {
      if (!isAvailableOnDate(currentDate)) return false;
      currentDate = currentDate.add(const Duration(days: 1));
    }
    
    return true;
  }

  /// <summary>
  /// حساب السعر الإجمالي لفترة معينة
  /// Calculate total price for a date range
  /// </summary>
  double calculateTotalPrice(DateTime checkIn, DateTime checkOut) {
    final nights = checkOut.difference(checkIn).inDays;
    if (nights <= 0) return 0;
    
    double totalPrice = pricing.basePricePerNight * nights;
    
    // تطبيق خصومات الإقامة الطويلة
    if (pricing.longStayDiscounts.isNotEmpty) {
      if (nights >= 30 && pricing.longStayDiscounts.containsKey('monthly')) {
        totalPrice *= (1 - pricing.longStayDiscounts['monthly']! / 100);
      } else if (nights >= 7 && pricing.longStayDiscounts.containsKey('weekly')) {
        totalPrice *= (1 - pricing.longStayDiscounts['weekly']! / 100);
      }
    }
    
    // إضافة الرسوم الإضافية
    for (final fee in pricing.additionalFees.values) {
      totalPrice += fee;
    }
    
    return totalPrice;
  }

  /// <summary>
  /// التحقق من وجود مطبخ
  /// Check if has kitchen
  /// </summary>
  bool get hasKitchen => specifications.hasKitchen;

  /// <summary>
  /// التحقق من وجود شرفة
  /// Check if has balcony
  /// </summary>
  bool get hasBalcony => specifications.hasBalcony;

  /// <summary>
  /// الحصول على نوع السرير
  /// Get bed type description  
  /// </summary>
  String get bedTypeDescription {
    if (specifications.bedType == null) return 'غير محدد';
    
    switch (specifications.bedType!.toLowerCase()) {
      case 'single':
        return 'سرير مفرد';
      case 'double':
        return 'سرير مزدوج';
      case 'queen':
        return 'سرير كوين';
      case 'king':
        return 'سرير كينج';
      case 'twin':
        return 'سريران منفصلان';
      default:
        return specifications.bedType!;
    }
  }

  /// <summary>
  /// الحصول على وصف المنظر
  /// Get view description
  /// </summary>
  String get viewDescription {
    if (specifications.viewType == null) return 'منظر عادي';
    
    switch (specifications.viewType!.toLowerCase()) {
      case 'sea':
        return 'إطلالة بحرية';
      case 'mountain':
        return 'إطلالة جبلية';
      case 'city':
        return 'إطلالة على المدينة';
      case 'garden':
        return 'إطلالة على الحديقة';
      case 'pool':
        return 'إطلالة على المسبح';
      default:
        return specifications.viewType!;
    }
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