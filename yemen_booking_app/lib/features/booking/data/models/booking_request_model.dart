import 'package:equatable/equatable.dart';
import '../../domain/entities/booking_request.dart';

/// <summary>
/// نموذج بيانات طلب الحجز
/// Booking request data model
/// </summary>
class BookingRequestModel extends Equatable {
  /// <summary>
  /// معرف المستخدم
  /// User ID
  /// </summary>
  final String userId;

  /// <summary>
  /// معرف الوحدة
  /// Unit ID
  /// </summary>
  final String unitId;

  /// <summary>
  /// تاريخ الوصول
  /// Check-in date
  /// </summary>
  final DateTime checkIn;

  /// <summary>
  /// تاريخ المغادرة
  /// Check-out date
  /// </summary>
  final DateTime checkOut;

  /// <summary>
  /// عدد الضيوف
  /// Guests count
  /// </summary>
  final int guestsCount;

  /// <summary>
  /// الخدمات المطلوبة
  /// Requested services
  /// </summary>
  final List<Map<String, dynamic>> services;

  /// <summary>
  /// ملاحظات خاصة
  /// Special requests
  /// </summary>
  final String? specialRequests;

  /// <summary>
  /// مصدر الحجز
  /// Booking source
  /// </summary>
  final String bookingSource;

  const BookingRequestModel({
    required this.userId,
    required this.unitId,
    required this.checkIn,
    required this.checkOut,
    required this.guestsCount,
    required this.services,
    this.specialRequests,
    required this.bookingSource,
  });

  factory BookingRequestModel.fromJson(Map<String, dynamic> json) {
    return BookingRequestModel(
      userId: json['userId'] ?? '',
      unitId: json['unitId'] ?? '',
      checkIn: DateTime.parse(json['checkIn']),
      checkOut: DateTime.parse(json['checkOut']),
      guestsCount: json['guestsCount'] ?? 1,
      services: List<Map<String, dynamic>>.from(json['services'] ?? []),
      specialRequests: json['specialRequests'],
      bookingSource: json['bookingSource'] ?? 'MobileApp',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'unitId': unitId,
      'checkIn': checkIn.toIso8601String(),
      'checkOut': checkOut.toIso8601String(),
      'guestsCount': guestsCount,
      'services': services,
      'specialRequests': specialRequests,
      'bookingSource': bookingSource,
    };
  }

  BookingRequest toEntity() {
    return BookingRequest(
      userId: userId,
      unitId: unitId,
      checkIn: checkIn,
      checkOut: checkOut,
      guestsCount: guestsCount,
      services: services,
      specialRequests: specialRequests,
      bookingSource: bookingSource,
    );
  }

  @override
  List<Object?> get props => [userId, unitId, checkIn, checkOut, guestsCount, services, specialRequests, bookingSource];
}