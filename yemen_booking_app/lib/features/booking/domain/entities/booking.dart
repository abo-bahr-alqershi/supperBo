import 'package:equatable/equatable.dart';
import '../../data/models/booking_model.dart';

/// <summary>
/// كيان الحجز
/// Booking entity
/// </summary>
class Booking extends Equatable {
  final String id;
  final String bookingNumber;
  final String userId;
  final String propertyId;
  final String propertyName;
  final String unitId;
  final String unitName;
  final DateTime checkIn;
  final DateTime checkOut;
  final int guestsCount;
  final BookingGuestModel primaryGuest;
  final List<BookingGuestModel> additionalGuests;
  final List<BookingServiceModel> services;
  final String? specialRequests;
  final BookingStatus status;
  final MoneyModel basePrice;
  final MoneyModel servicesPrice;
  final MoneyModel taxesAndFees;
  final MoneyModel discounts;
  final MoneyModel totalPrice;
  final DateTime bookedAt;
  final DateTime lastUpdated;
  final String propertyImageUrl;
  final bool canCancel;
  final bool canReview;
  final String bookingSource;
  final Map<String, dynamic> additionalInfo;
  final String? cancellationReason;
  final DateTime? cancelledAt;

  const Booking({
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
  /// حساب عدد الليالي
  /// Calculate number of nights
  /// </summary>
  int get nightsCount => checkOut.difference(checkIn).inDays;

  /// <summary>
  /// التحقق من إمكانية الإلغاء
  /// Check if cancellable
  /// </summary>
  bool get isCancellable => canCancel && status != BookingStatus.cancelled && status != BookingStatus.completed;

  /// <summary>
  /// الحصول على وصف الحالة
  /// Get status description
  /// </summary>
  String get statusDescription {
    switch (status) {
      case BookingStatus.pending:
        return 'معلق';
      case BookingStatus.confirmed:
        return 'مؤكد';
      case BookingStatus.cancelled:
        return 'ملغى';
      case BookingStatus.completed:
        return 'مكتمل';
      case BookingStatus.checkedIn:
        return 'تم تسجيل الوصول';
    }
  }

  @override
  List<Object?> get props => [
    id, bookingNumber, userId, propertyId, propertyName, unitId, unitName,
    checkIn, checkOut, guestsCount, primaryGuest, additionalGuests, services,
    specialRequests, status, basePrice, servicesPrice, taxesAndFees, discounts,
    totalPrice, bookedAt, lastUpdated, propertyImageUrl, canCancel, canReview,
    bookingSource, additionalInfo, cancellationReason, cancelledAt,
  ];
}