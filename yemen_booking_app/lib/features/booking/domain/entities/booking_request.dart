import 'package:equatable/equatable.dart';

class BookingRequest extends Equatable {
  final String userId;
  final String unitId;
  final DateTime checkIn;
  final DateTime checkOut;
  final int guestsCount;
  final List<Map<String, dynamic>> services;
  final String? specialRequests;
  final String bookingSource;

  const BookingRequest({
    required this.userId,
    required this.unitId,
    required this.checkIn,
    required this.checkOut,
    required this.guestsCount,
    required this.services,
    this.specialRequests,
    required this.bookingSource,
  });

  int get nightsCount => checkOut.difference(checkIn).inDays;

  @override
  List<Object?> get props => [userId, unitId, checkIn, checkOut, guestsCount, services, specialRequests, bookingSource];
}