import 'package:equatable/equatable.dart';
import '../../domain/entities/payment_method.dart';

class PaymentMethodModel extends Equatable {
  final String id;
  final String name;
  final String type;
  final String provider;
  final bool isEnabled;
  final Map<String, dynamic> configuration;
  final List<String> supportedCurrencies;
  final double minAmount;
  final double maxAmount;
  final double processingFee;
  final String iconUrl;

  const PaymentMethodModel({
    required this.id,
    required this.name,
    required this.type,
    required this.provider,
    required this.isEnabled,
    required this.configuration,
    required this.supportedCurrencies,
    required this.minAmount,
    required this.maxAmount,
    required this.processingFee,
    required this.iconUrl,
  });

  factory PaymentMethodModel.fromJson(Map<String, dynamic> json) {
    return PaymentMethodModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      type: json['type'] ?? '',
      provider: json['provider'] ?? '',
      isEnabled: json['isEnabled'] ?? true,
      configuration: Map<String, dynamic>.from(json['configuration'] ?? {}),
      supportedCurrencies: List<String>.from(json['supportedCurrencies'] ?? []),
      minAmount: (json['minAmount'] ?? 0.0).toDouble(),
      maxAmount: (json['maxAmount'] ?? 999999.0).toDouble(),
      processingFee: (json['processingFee'] ?? 0.0).toDouble(),
      iconUrl: json['iconUrl'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'type': type,
      'provider': provider,
      'isEnabled': isEnabled,
      'configuration': configuration,
      'supportedCurrencies': supportedCurrencies,
      'minAmount': minAmount,
      'maxAmount': maxAmount,
      'processingFee': processingFee,
      'iconUrl': iconUrl,
    };
  }

  PaymentMethod toEntity() {
    return PaymentMethod(
      id: id,
      name: name,
      type: type,
      provider: provider,
      isEnabled: isEnabled,
      configuration: configuration,
      supportedCurrencies: supportedCurrencies,
      minAmount: minAmount,
      maxAmount: maxAmount,
      processingFee: processingFee,
      iconUrl: iconUrl,
    );
  }

  @override
  List<Object> get props => [id, name, type, provider, isEnabled, configuration, supportedCurrencies, minAmount, maxAmount, processingFee, iconUrl];
}