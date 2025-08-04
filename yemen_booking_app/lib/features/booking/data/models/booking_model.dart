import 'package:equatable/equatable.dart';
import '../../domain/entities/booking.dart';

/// <summary>
/// نموذج بيانات المبلغ المالي
/// Money amount data model
/// </summary>
class MoneyModel extends Equatable {
  /// <summary>
  /// المبلغ
  /// Amount
  /// </summary>
  final double amount;

  /// <summary>
  /// سعر الصرف
  /// Exchange rate
  /// </summary>
  final double exchangeRate;

  /// <summary>
  /// العملة
  /// Currency
  /// </summary>
  final String currency;

  const MoneyModel({
    required this.amount,
    required this.exchangeRate,
    required this.currency,
  });

  factory MoneyModel.fromJson(Map<String, dynamic> json) {
    return MoneyModel(
      amount: (json['amount'] ?? 0.0).toDouble(),
      exchangeRate: (json['exchangeRate'] ?? 1.0).toDouble(),
      currency: json['currency'] ?? 'YER',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'amount': amount,
      'exchangeRate': exchangeRate,
      'currency': currency,
    };
  }

  /// <summary>
  /// تنسيق المبلغ للعرض
  /// Format amount for display
  /// </summary>
  String get formattedAmount {
    return '${amount.toStringAsFixed(0)} $currency';
  }

  @override
  List<Object> get props => [amount, currency];
}

/// <summary>
/// نموذج بيانات ضيف الحجز
/// Booking guest data model
/// </summary>
class BookingGuestModel extends Equatable {
  /// <summary>
  /// الاسم الأول
  /// First name
  /// </summary>
  final String firstName;

  /// <summary>
  /// الاسم الأخير
  /// Last name
  /// </summary>
  final String lastName;

  /// <summary>
  /// البريد الإلكتروني
  /// Email
  /// </summary>
  final String email;

  /// <summary>
  /// رقم الهاتف
  /// Phone number
  /// </summary>
  final String phone;

  /// <summary>
  /// العمر
  /// Age
  /// </summary>
  final int? age;

  /// <summary>
  /// الجنسية
  /// Nationality
  /// </summary>
  final String? nationality;

  /// <summary>
  /// رقم الهوية
  /// ID number
  /// </summary>
  final String? idNumber;

  const BookingGuestModel({
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.phone,
    this.age,
    this.nationality,
    this.idNumber,
  });

  factory BookingGuestModel.fromJson(Map<String, dynamic> json) {
    return BookingGuestModel(
      firstName: json['firstName'] ?? '',
      lastName: json['lastName'] ?? '',
      email: json['email'] ?? '',
      phone: json['phone'] ?? '',
      age: json['age'],
      nationality: json['nationality'],
      idNumber: json['idNumber'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'email': email,
      'phone': phone,
      'age': age,
      'nationality': nationality,
      'idNumber': idNumber,
    };
  }

  /// <summary>
  /// الحصول على الاسم الكامل
  /// Get full name
  /// </summary>
  String get fullName => '$firstName $lastName';

  @override
  List<Object?> get props => [firstName, lastName, email, phone, age, nationality, idNumber];
}

/// <summary>
/// نموذج بيانات خدمة الحجز
/// Booking service data model
/// </summary>
class BookingServiceModel extends Equatable {
  /// <summary>
  /// معرف الخدمة
  /// Service ID
  /// </summary>
  final String serviceId;

  /// <summary>
  /// اسم الخدمة
  /// Service name
  /// </summary>
  final String serviceName;

  /// <summary>
  /// الكمية
  /// Quantity
  /// </summary>
  final int quantity;

  /// <summary>
  /// السعر للوحدة الواحدة
  /// Price per unit
  /// </summary>
  final MoneyModel unitPrice;

  /// <summary>
  /// إجمالي السعر
  /// Total price
  /// </summary>
  final MoneyModel totalPrice;

  /// <summary>
  /// معلومات إضافية
  /// Additional information
  /// </summary>
  final Map<String, dynamic> additionalInfo;

  const BookingServiceModel({
    required this.serviceId,
    required this.serviceName,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
    required this.additionalInfo,
  });

  factory BookingServiceModel.fromJson(Map<String, dynamic> json) {
    return BookingServiceModel(
      serviceId: json['serviceId'] ?? '',
      serviceName: json['serviceName'] ?? '',
      quantity: json['quantity'] ?? 1,
      unitPrice: MoneyModel.fromJson(json['unitPrice'] ?? {}),
      totalPrice: MoneyModel.fromJson(json['totalPrice'] ?? {}),
      additionalInfo: Map<String, dynamic>.from(json['additionalInfo'] ?? {}),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'serviceId': serviceId,
      'serviceName': serviceName,
      'quantity': quantity,
      'unitPrice': unitPrice.toJson(),
      'totalPrice': totalPrice.toJson(),
      'additionalInfo': additionalInfo,
    };
  }

  @override
  List<Object> get props => [serviceId, serviceName, quantity, unitPrice, totalPrice, additionalInfo];
}

/// <summary>
/// تعداد حالة الحجز
/// Booking status enumeration
/// </summary>
enum BookingStatus {
  /// <summary>
  /// معلق
  /// Pending
  /// </summary>
  pending,

  /// <summary>
  /// مؤكد
  /// Confirmed
  /// </summary>
  confirmed,

  /// <summary>
  /// ملغى
  /// Cancelled
  /// </summary>
  cancelled,

  /// <summary>
  /// مكتمل
  /// Completed
  /// </summary>
  completed,

  /// <summary>
  /// تسجيل الوصول
  /// Checked in
  /// </summary>
  checkedIn,
}

/// <summary>
/// نموذج بيانات الحجز
/// Booking data model
/// </summary>
class BookingModel extends Equatable {
  /// <summary>
  /// معرف الحجز
  /// Booking ID
  /// </summary>
  final String id;

  /// <summary>
  /// رقم الحجز
  /// Booking number
  /// </summary>
  final String bookingNumber;

  /// <summary>
  /// معرف المستخدم
  /// User ID
  /// </summary>
  final String userId;

  /// <summary>
  /// معرف العقار
  /// Property ID
  /// </summary>
  final String propertyId;

  /// <summary>
  /// اسم العقار
  /// Property name
  /// </summary>
  final String propertyName;

  /// <summary>
  /// معرف الوحدة
  /// Unit ID
  /// </summary>
  final String unitId;

  /// <summary>
  /// اسم الوحدة
  /// Unit name
  /// </summary>
  final String unitName;

  /// <summary>
  /// تاريخ تسجيل الوصول
  /// Check-in date
  /// </summary>
  final DateTime checkIn;

  /// <summary>
  /// تاريخ تسجيل المغادرة
  /// Check-out date
  /// </summary>
  final DateTime checkOut;

  /// <summary>
  /// عدد الضيوف
  /// Guests count
  /// </summary>
  final int guestsCount;

  /// <summary>
  /// بيانات الضيف الرئيسي
  /// Primary guest information
  /// </summary>
  final BookingGuestModel primaryGuest;

  /// <summary>
  /// قائمة الضيوف الإضافيين
  /// Additional guests list
  /// </summary>
  final List<BookingGuestModel> additionalGuests;

  /// <summary>
  /// الخدمات المطلوبة
  /// Requested services
  /// </summary>
  final List<BookingServiceModel> services;

  /// <summary>
  /// ملاحظات خاصة
  /// Special requests
  /// </summary>
  final String? specialRequests;

  /// <summary>
  /// حالة الحجز
  /// Booking status
  /// </summary>
  final BookingStatus status;

  /// <summary>
  /// السعر الأساسي
  /// Base price
  /// </summary>
  final MoneyModel basePrice;

  /// <summary>
  /// سعر الخدمات
  /// Services price
  /// </summary>
  final MoneyModel servicesPrice;

  /// <summary>
  /// الضرائب والرسوم
  /// Taxes and fees
  /// </summary>
  final MoneyModel taxesAndFees;

  /// <summary>
  /// الخصومات
  /// Discounts
  /// </summary>
  final MoneyModel discounts;

  /// <summary>
  /// السعر الإجمالي
  /// Total price
  /// </summary>
  final MoneyModel totalPrice;

  /// <summary>
  /// تاريخ إنشاء الحجز
  /// Booking creation date
  /// </summary>
  final DateTime bookedAt;

  /// <summary>
  /// تاريخ آخر تحديث
  /// Last updated date
  /// </summary>
  final DateTime lastUpdated;

  /// <summary>
  /// رابط صورة العقار
  /// Property image URL
  /// </summary>
  final String propertyImageUrl;

  /// <summary>
  /// هل يمكن إلغاء الحجز
  /// Can cancel booking
  /// </summary>
  final bool canCancel;

  /// <summary>
  /// هل يمكن تقييم الحجز
  /// Can review booking
  /// </summary>
  final bool canReview;

  /// <summary>
  /// مصدر الحجز
  /// Booking source
  /// </summary>
  final String bookingSource;

  /// <summary>
  /// معلومات إضافية
  /// Additional information
  /// </summary>
  final Map<String, dynamic> additionalInfo;

  /// <summary>
  /// سبب الإلغاء (إن وجد)
  /// Cancellation reason (if any)
  /// </summary>
  final String? cancellationReason;

  /// <summary>
  /// تاريخ الإلغاء (إن وجد)
  /// Cancellation date (if any)
  /// </summary>
  final DateTime? cancelledAt;

  const BookingModel({
    required this.id,
    required this.bookingNumber,
    required this.userId,
    required this.propertyId,
    required this.propertyName,
    required this.unitId,
    required this.unitName,
    required this.checkIn,
    required this.checkOut,
    required this.guestsCount,
    required this.primaryGuest,
    required this.additionalGuests,
    required this.services,
    this.specialRequests,
    required this.status,
    required this.basePrice,
    required this.servicesPrice,
    required this.taxesAndFees,
    required this.discounts,
    required this.totalPrice,
    required this.bookedAt,
    required this.lastUpdated,
    required this.propertyImageUrl,
    required this.canCancel,
    required this.canReview,
    required this.bookingSource,
    required this.additionalInfo,
    this.cancellationReason,
    this.cancelledAt,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory BookingModel.fromJson(Map<String, dynamic> json) {
    return BookingModel(
      id: json['id'] ?? '',
      bookingNumber: json['bookingNumber'] ?? '',
      userId: json['userId'] ?? '',
      propertyId: json['propertyId'] ?? '',
      propertyName: json['propertyName'] ?? '',
      unitId: json['unitId'] ?? '',
      unitName: json['unitName'] ?? '',
      checkIn: DateTime.parse(json['checkIn'] ?? DateTime.now().toIso8601String()),
      checkOut: DateTime.parse(json['checkOut'] ?? DateTime.now().toIso8601String()),
      guestsCount: json['guestsCount'] ?? 1,
      primaryGuest: BookingGuestModel.fromJson(json['primaryGuest'] ?? {}),
      additionalGuests: (json['additionalGuests'] as List<dynamic>?)
          ?.map((guest) => BookingGuestModel.fromJson(guest))
          .toList() ?? [],
      services: (json['services'] as List<dynamic>?)
          ?.map((service) => BookingServiceModel.fromJson(service))
          .toList() ?? [],
      specialRequests: json['specialRequests'],
      status: _parseBookingStatus(json['status']),
      basePrice: MoneyModel.fromJson(json['basePrice'] ?? {}),
      servicesPrice: MoneyModel.fromJson(json['servicesPrice'] ?? {}),
      taxesAndFees: MoneyModel.fromJson(json['taxesAndFees'] ?? {}),
      discounts: MoneyModel.fromJson(json['discounts'] ?? {}),
      totalPrice: MoneyModel.fromJson(json['totalPrice'] ?? {}),
      bookedAt: DateTime.parse(json['bookedAt'] ?? DateTime.now().toIso8601String()),
      lastUpdated: DateTime.parse(json['lastUpdated'] ?? DateTime.now().toIso8601String()),
      propertyImageUrl: json['propertyImageUrl'] ?? '',
      canCancel: json['canCancel'] ?? false,
      canReview: json['canReview'] ?? false,
      bookingSource: json['bookingSource'] ?? 'MobileApp',
      additionalInfo: Map<String, dynamic>.from(json['additionalInfo'] ?? {}),
      cancellationReason: json['cancellationReason'],
      cancelledAt: json['cancelledAt'] != null 
          ? DateTime.parse(json['cancelledAt']) 
          : null,
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'bookingNumber': bookingNumber,
      'userId': userId,
      'propertyId': propertyId,
      'propertyName': propertyName,
      'unitId': unitId,
      'unitName': unitName,
      'checkIn': checkIn.toIso8601String(),
      'checkOut': checkOut.toIso8601String(),
      'guestsCount': guestsCount,
      'primaryGuest': primaryGuest.toJson(),
      'additionalGuests': additionalGuests.map((guest) => guest.toJson()).toList(),
      'services': services.map((service) => service.toJson()).toList(),
      'specialRequests': specialRequests,
      'status': status.name,
      'basePrice': basePrice.toJson(),
      'servicesPrice': servicesPrice.toJson(),
      'taxesAndFees': taxesAndFees.toJson(),
      'discounts': discounts.toJson(),
      'totalPrice': totalPrice.toJson(),
      'bookedAt': bookedAt.toIso8601String(),
      'lastUpdated': lastUpdated.toIso8601String(),
      'propertyImageUrl': propertyImageUrl,
      'canCancel': canCancel,
      'canReview': canReview,
      'bookingSource': bookingSource,
      'additionalInfo': additionalInfo,
      'cancellationReason': cancellationReason,
      'cancelledAt': cancelledAt?.toIso8601String(),
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  Booking toEntity() {
    return Booking(
      id: id,
      bookingNumber: bookingNumber,
      userId: userId,
      propertyId: propertyId,
      propertyName: propertyName,
      unitId: unitId,
      unitName: unitName,
      checkIn: checkIn,
      checkOut: checkOut,
      guestsCount: guestsCount,
      primaryGuest: primaryGuest,
      additionalGuests: additionalGuests,
      services: services,
      specialRequests: specialRequests,
      status: status,
      basePrice: basePrice,
      servicesPrice: servicesPrice,
      taxesAndFees: taxesAndFees,
      discounts: discounts,
      totalPrice: totalPrice,
      bookedAt: bookedAt,
      lastUpdated: lastUpdated,
      propertyImageUrl: propertyImageUrl,
      canCancel: canCancel,
      canReview: canReview,
      bookingSource: bookingSource,
      additionalInfo: additionalInfo,
      cancellationReason: cancellationReason,
      cancelledAt: cancelledAt,
    );
  }

  /// <summary>
  /// تحليل حالة الحجز من النص
  /// Parse booking status from string
  /// </summary>
  static BookingStatus _parseBookingStatus(String? status) {
    switch (status?.toLowerCase()) {
      case 'pending':
        return BookingStatus.pending;
      case 'confirmed':
        return BookingStatus.confirmed;
      case 'cancelled':
        return BookingStatus.cancelled;
      case 'completed':
        return BookingStatus.completed;
      case 'checkedin':
        return BookingStatus.checkedIn;
      default:
        return BookingStatus.pending;
    }
  }

  @override
  List<Object?> get props => [
    id,
    bookingNumber,
    userId,
    propertyId,
    propertyName,
    unitId,
    unitName,
    checkIn,
    checkOut,
    guestsCount,
    primaryGuest,
    additionalGuests,
    services,
    specialRequests,
    status,
    basePrice,
    servicesPrice,
    taxesAndFees,
    discounts,
    totalPrice,
    bookedAt,
    lastUpdated,
    propertyImageUrl,
    canCancel,
    canReview,
    bookingSource,
    additionalInfo,
    cancellationReason,
    cancelledAt,
  ];
}