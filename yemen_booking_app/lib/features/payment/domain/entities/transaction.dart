import 'package:equatable/equatable.dart';
import '../../data/models/transaction_model.dart';

class Transaction extends Equatable {
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

  const Transaction({
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

  bool get isCompleted => status == TransactionStatus.completed;
  bool get isPending => status == TransactionStatus.pending;
  bool get isFailed => status == TransactionStatus.failed;

  String get formattedAmount => '${amount.toStringAsFixed(2)} $currency';

  @override
  List<Object?> get props => [id, paymentMethodId, bookingId, userId, amount, currency, status, createdAt, completedAt, failureReason, metadata];
}