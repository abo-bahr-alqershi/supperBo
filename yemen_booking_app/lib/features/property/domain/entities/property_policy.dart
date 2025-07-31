import 'package:equatable/equatable.dart';

/// <summary>
/// كيان سياسة الإلغاء
/// Cancellation policy entity
/// </summary>
class CancellationPolicy extends Equatable {
  /// <summary>
  /// نوع السياسة
  /// Policy type
  /// </summary>
  final String policyType;

  /// <summary>
  /// ساعات الإلغاء المجاني
  /// Free cancellation hours
  /// </summary>
  final int freeCancellationHours;

  /// <summary>
  /// نسبة الاسترداد حسب الوقت
  /// Refund percentage by time
  /// </summary>
  final Map<String, double> refundPercentages;

  /// <summary>
  /// رسوم الإلغاء
  /// Cancellation fees
  /// </summary>
  final Map<String, double> cancellationFees;

  /// <summary>
  /// شروط خاصة
  /// Special conditions
  /// </summary>
  final List<String> specialConditions;

  const CancellationPolicy({
    required this.policyType,
    required this.freeCancellationHours,
    required this.refundPercentages,
    required this.cancellationFees,
    required this.specialConditions,
  });

  factory CancellationPolicy.fromJson(Map<String, dynamic> json) {
    return CancellationPolicy(
      policyType: json['policyType'] ?? 'standard',
      freeCancellationHours: json['freeCancellationHours'] ?? 24,
      refundPercentages: Map<String, double>.from(json['refundPercentages'] ?? {}),
      cancellationFees: Map<String, double>.from(json['cancellationFees'] ?? {}),
      specialConditions: List<String>.from(json['specialConditions'] ?? []),
    );
  }

  /// <summary>
  /// الحصول على وصف السياسة
  /// Get policy description
  /// </summary>
  String get policyDescription {
    switch (policyType.toLowerCase()) {
      case 'flexible':
        return 'مرنة - إلغاء مجاني حتى $freeCancellationHours ساعة قبل الوصول';
      case 'moderate':
        return 'متوسطة - إلغاء مجاني حتى $freeCancellationHours ساعة قبل الوصول مع رسوم جزئية';
      case 'strict':
        return 'صارمة - رسوم إلغاء عالية أو عدم استرداد';
      case 'non-refundable':
        return 'غير قابل للاسترداد';
      default:
        return 'سياسة قياسية';
    }
  }

  /// <summary>
  /// حساب نسبة الاسترداد
  /// Calculate refund percentage
  /// </summary>
  double calculateRefundPercentage(int hoursBeforeArrival) {
    if (hoursBeforeArrival >= freeCancellationHours) {
      return 100.0;
    }
    
    // البحث في نسب الاسترداد المحددة
    for (final entry in refundPercentages.entries) {
      final hours = int.tryParse(entry.key) ?? 0;
      if (hoursBeforeArrival >= hours) {
        return entry.value;
      }
    }
    
    return 0.0;
  }

  @override
  List<Object> get props => [policyType, freeCancellationHours, refundPercentages, cancellationFees, specialConditions];
}

/// <summary>
/// كيان سياسة تسجيل الدخول والخروج
/// Check-in/out policy entity
/// </summary>
class CheckInOutPolicy extends Equatable {
  /// <summary>
  /// وقت تسجيل الدخول من
  /// Check-in time from
  /// </summary>
  final String checkInFrom;

  /// <summary>
  /// وقت تسجيل الدخول حتى
  /// Check-in time until  
  /// </summary>
  final String checkInUntil;

  /// <summary>
  /// وقت تسجيل الخروج حتى
  /// Check-out time until
  /// </summary>
  final String checkOutUntil;

  /// <summary>
  /// تسجيل دخول متأخر متاح
  /// Late check-in available
  /// </summary>
  final bool lateCheckInAvailable;

  /// <summary>
  /// رسوم تسجيل الدخول المتأخر
  /// Late check-in fees
  /// </summary>
  final double? lateCheckInFee;

  /// <summary>
  /// تسجيل خروج مبكر متاح
  /// Early check-out available
  /// </summary>
  final bool earlyCheckOutAvailable;

  /// <summary>
  /// خروج متأخر متاح
  /// Late check-out available
  /// </summary>
  final bool lateCheckOutAvailable;

  /// <summary>
  /// رسوم الخروج المتأخر
  /// Late check-out fees
  /// </summary>
  final double? lateCheckOutFee;

  /// <summary>
  /// تعليمات خاصة
  /// Special instructions
  /// </summary>
  final List<String> specialInstructions;

  const CheckInOutPolicy({
    required this.checkInFrom,
    required this.checkInUntil,
    required this.checkOutUntil,
    required this.lateCheckInAvailable,
    this.lateCheckInFee,
    required this.earlyCheckOutAvailable,
    required this.lateCheckOutAvailable,
    this.lateCheckOutFee,
    required this.specialInstructions,
  });

  factory CheckInOutPolicy.fromJson(Map<String, dynamic> json) {
    return CheckInOutPolicy(
      checkInFrom: json['checkInFrom'] ?? '14:00',
      checkInUntil: json['checkInUntil'] ?? '22:00',
      checkOutUntil: json['checkOutUntil'] ?? '11:00',
      lateCheckInAvailable: json['lateCheckInAvailable'] ?? false,
      lateCheckInFee: json['lateCheckInFee']?.toDouble(),
      earlyCheckOutAvailable: json['earlyCheckOutAvailable'] ?? true,
      lateCheckOutAvailable: json['lateCheckOutAvailable'] ?? false,
      lateCheckOutFee: json['lateCheckOutFee']?.toDouble(),
      specialInstructions: List<String>.from(json['specialInstructions'] ?? []),
    );
  }

  @override
  List<Object?> get props => [
    checkInFrom,
    checkInUntil, 
    checkOutUntil,
    lateCheckInAvailable,
    lateCheckInFee,
    earlyCheckOutAvailable,
    lateCheckOutAvailable,
    lateCheckOutFee,
    specialInstructions,
  ];
}

/// <summary>
/// كيان سياسات العقار
/// Property policies entity
/// </summary>
class PropertyPolicy extends Equatable {
  /// <summary>
  /// سياسة الإلغاء
  /// Cancellation policy
  /// </summary>
  final CancellationPolicy cancellationPolicy;

  /// <summary>
  /// سياسة تسجيل الدخول والخروج
  /// Check-in/out policy
  /// </summary>
  final CheckInOutPolicy checkInOutPolicy;

  /// <summary>
  /// الحد الأدنى للعمر
  /// Minimum age
  /// </summary>
  final int minimumAge;

  /// <summary>
  /// هل يُسمح بالأطفال
  /// Children allowed
  /// </summary>
  final bool childrenAllowed;

  /// <summary>
  /// هل يُسمح بالحيوانات الأليفة
  /// Pets allowed
  /// </summary>
  final bool petsAllowed;

  /// <summary>
  /// رسوم الحيوانات الأليفة
  /// Pet fees
  /// </summary>
  final double? petFees;

  /// <summary>
  /// هل يُسمح بالتدخين
  /// Smoking allowed
  /// </summary>
  final bool smokingAllowed;

  /// <summary>
  /// هل يُسمح بالحفلات/الأحداث
  /// Parties/events allowed
  /// </summary>
  final bool eventsAllowed;

  /// <summary>
  /// ساعات الهدوء
  /// Quiet hours
  /// </summary>
  final Map<String, String> quietHours;

  /// <summary>
  /// الحد الأقصى للضيوف
  /// Maximum guests
  /// </summary>
  final int? maxGuests;

  /// <summary>
  /// قواعد المنزل
  /// House rules
  /// </summary>
  final List<String> houseRules;

  /// <summary>
  /// السياسات الإضافية
  /// Additional policies
  /// </summary>
  final Map<String, dynamic> additionalPolicies;

  /// <summary>
  /// متطلبات الهوية
  /// ID requirements
  /// </summary>
  final List<String> idRequirements;

  /// <summary>
  /// طرق الدفع المقبولة
  /// Accepted payment methods
  /// </summary>
  final List<String> acceptedPaymentMethods;

  /// <summary>
  /// رسوم إضافية للضيوف الإضافيين
  /// Extra guest fees
  /// </summary>
  final double? extraGuestFee;

  const PropertyPolicy({
    required this.cancellationPolicy,
    required this.checkInOutPolicy,
    required this.minimumAge,
    required this.childrenAllowed,
    required this.petsAllowed,
    this.petFees,
    required this.smokingAllowed,
    required this.eventsAllowed,
    required this.quietHours,
    this.maxGuests,
    required this.houseRules,
    required this.additionalPolicies,
    required this.idRequirements,
    required this.acceptedPaymentMethods,
    this.extraGuestFee,
  });

  factory PropertyPolicy.fromJson(Map<String, dynamic> json) {
    return PropertyPolicy(
      cancellationPolicy: CancellationPolicy.fromJson(json['cancellationPolicy'] ?? {}),
      checkInOutPolicy: CheckInOutPolicy.fromJson(json['checkInOutPolicy'] ?? {}),
      minimumAge: json['minimumAge'] ?? 18,
      childrenAllowed: json['childrenAllowed'] ?? true,
      petsAllowed: json['petsAllowed'] ?? false,
      petFees: json['petFees']?.toDouble(),
      smokingAllowed: json['smokingAllowed'] ?? false,
      eventsAllowed: json['eventsAllowed'] ?? false,
      quietHours: Map<String, String>.from(json['quietHours'] ?? {'from': '22:00', 'to': '08:00'}),
      maxGuests: json['maxGuests'],
      houseRules: List<String>.from(json['houseRules'] ?? []),
      additionalPolicies: Map<String, dynamic>.from(json['additionalPolicies'] ?? {}),
      idRequirements: List<String>.from(json['idRequirements'] ?? ['valid_id']),
      acceptedPaymentMethods: List<String>.from(json['acceptedPaymentMethods'] ?? []),
      extraGuestFee: json['extraGuestFee']?.toDouble(),
    );
  }

  /// <summary>
  /// التحقق من السماح بعدد الضيوف
  /// Check if guest count is allowed
  /// </summary>
  bool isGuestCountAllowed(int guestCount) {
    if (maxGuests == null) return true;
    return guestCount <= maxGuests!;
  }

  /// <summary>
  /// حساب رسوم الضيوف الإضافيين
  /// Calculate extra guest fees
  /// </summary>
  double calculateExtraGuestFees(int guestCount, int baseGuestCount) {
    if (extraGuestFee == null || guestCount <= baseGuestCount) {
      return 0.0;
    }
    
    final extraGuests = guestCount - baseGuestCount;
    return extraGuests * extraGuestFee!;
  }

  /// <summary>
  /// الحصول على ملخص السياسات
  /// Get policies summary
  /// </summary>
  Map<String, String> get policySummary {
    return {
      'الإلغاء': cancellationPolicy.policyDescription,
      'تسجيل الدخول': '${checkInOutPolicy.checkInFrom} - ${checkInOutPolicy.checkInUntil}',
      'تسجيل الخروج': 'حتى ${checkInOutPolicy.checkOutUntil}',
      'الأطفال': childrenAllowed ? 'مسموح' : 'غير مسموح',
      'الحيوانات الأليفة': petsAllowed ? 'مسموح' : 'غير مسموح',
      'التدخين': smokingAllowed ? 'مسموح' : 'غير مسموح',
      'الحفلات': eventsAllowed ? 'مسموح' : 'غير مسموح',
    };
  }

  @override
  List<Object?> get props => [
    cancellationPolicy,
    checkInOutPolicy,
    minimumAge,
    childrenAllowed,
    petsAllowed,
    petFees,
    smokingAllowed,
    eventsAllowed,
    quietHours,
    maxGuests,
    houseRules,
    additionalPolicies,
    idRequirements,
    acceptedPaymentMethods,
    extraGuestFee,
  ];
}