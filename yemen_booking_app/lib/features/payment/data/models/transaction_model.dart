import 'package:equatable/equatable.dart';
import '../../domain/entities/transaction.dart';

enum TransactionStatus { pending, completed, failed, cancelled, refunded }

class TransactionModel extends Equatable {
  final String id;
  final String paymentMethodId;
  final String bookingId;
  final String userId;
  final double amount;
  final String currency;
  final TransactionStatus status;
  final DateTime createdAt;
  final DateTime? completedAt;
  final String? failureReason;
  final Map<String, dynamic> metadata;

  const TransactionModel({
    required this.id,
    required this.paymentMethodId,
    required this.bookingId,
    required this.userId,
    required this.amount,
    required this.currency,
    required this.status,
    required this.createdAt,
    this.completedAt,
    this.failureReason,
    required this.metadata,
  });

  factory TransactionModel.fromJson(Map<String, dynamic> json) {
    return TransactionModel(
      id: json['id'] ?? '',
      paymentMethodId: json['paymentMethodId'] ?? '',
      bookingId: json['bookingId'] ?? '',
      userId: json['userId'] ?? '',
      amount: (json['amount'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'YER',
      status: _parseStatus(json['status']),
      createdAt: DateTime.parse(json['createdAt'] ?? DateTime.now().toIso8601String()),
      completedAt: json['completedAt'] != null ? DateTime.parse(json['completedAt']) : null,
      failureReason: json['failureReason'],
      metadata: Map<String, dynamic>.from(json['metadata'] ?? {}),
    );
  }

  static TransactionStatus _parseStatus(String? status) {
    switch (status?.toLowerCase()) {
      case 'pending': return TransactionStatus.pending;
      case 'completed': return TransactionStatus.completed;
      case 'failed': return TransactionStatus.failed;
      case 'cancelled': return TransactionStatus.cancelled;
      case 'refunded': return TransactionStatus.refunded;
      default: return TransactionStatus.pending;
    }
  }

  Transaction toEntity() {
    return Transaction(
      id: id,
      paymentMethodId: paymentMethodId,
      bookingId: bookingId,
      userId: userId,
      amount: amount,
      currency: currency,
      status: status,
      createdAt: createdAt,
      completedAt: completedAt,
      failureReason: failureReason,
      metadata: metadata,
    );
  }

  @override
  List<Object?> get props => [id, paymentMethodId, bookingId, userId, amount, currency, status, createdAt, completedAt, failureReason, metadata];
}