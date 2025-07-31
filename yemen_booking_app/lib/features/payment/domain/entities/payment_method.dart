import 'package:equatable/equatable.dart';

class PaymentMethod extends Equatable {
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

  const PaymentMethod({
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

  bool canProcess(double amount, String currency) {
    return isEnabled &&
           amount >= minAmount &&
           amount <= maxAmount &&
           supportedCurrencies.contains(currency);
  }

  @override
  List<Object> get props => [id, name, type, provider, isEnabled, configuration, supportedCurrencies, minAmount, maxAmount, processingFee, iconUrl];
}