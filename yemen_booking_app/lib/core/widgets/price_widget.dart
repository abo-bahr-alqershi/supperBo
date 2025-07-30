import 'package:flutter/material.dart';
import '../theme/app_text_styles.dart';
import '../constants/app_constants.dart'; // For currency symbol
import '../utils/formatters.dart'; // Assuming Formatters has currency formatting

class PriceWidget extends StatelessWidget {
  final double price;
  final TextStyle? priceTextStyle;
  final String currencySymbol;
  final bool showCurrencySymbol;
  final bool formatWithCommas; // Whether to use locale-based formatting

  const PriceWidget({
    super.key,
    required this.price,
    this.priceTextStyle,
    this.currencySymbol = AppConstants.currencySymbol, // Default Yemen Riyal symbol
    this.showCurrencySymbol = true,
    this.formatWithCommas = true,
  });

  @override
  Widget build(BuildContext context) {
    final effectiveTextStyle = priceTextStyle ?? AppTextStyles.priceTextStyle;
    
    String formattedPrice;
    if (formatWithCommas) {
      // Use locale-specific formatting if available and desired
      // For simplicity, using a basic formatter here. Consider Intl package for robust localization.
      formattedPrice = Formatters.formatCurrency(price, currencySymbol); // Assume Formatters class exists
    } else {
      formattedPrice = '$currencySymbol ${price.toStringAsFixed(2)}'; // Basic formatting
    }

    // If you want to show the currency symbol separately or adjust positioning
    final String priceValue = formatWithCommas
        ? formattedPrice.replaceAll(currencySymbol, '').trim() // Remove symbol if already included by formatter
        : price.toStringAsFixed(2);

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (showCurrencySymbol)
          Text(
            currencySymbol,
            style: effectiveTextStyle.copyWith(
              // Optionally adjust symbol style
            ),
          ),
        if (showCurrencySymbol) const SizedBox(width: 4.0), // Space between symbol and value
        Text(
          priceValue,
          style: effectiveTextStyle,
        ),
      ],
    );
  }
}