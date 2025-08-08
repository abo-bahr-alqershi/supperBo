// lib/features/home/presentation/widgets/sections/common/price_tag_widget.dart

import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_text_styles.dart';

class PriceTagWidget extends StatelessWidget {
  final double price;
  final double? originalPrice;
  final String currency;
  final bool showDiscount;

  const PriceTagWidget({
    super.key,
    required this.price,
    this.originalPrice,
    required this.currency,
    this.showDiscount = true,
  });

  @override
  Widget build(BuildContext context) {
    final hasDiscount = originalPrice != null && originalPrice! > price;
    final discountPercentage = hasDiscount 
        ? ((originalPrice! - price) / originalPrice! * 100).round()
        : 0;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (hasDiscount && showDiscount)
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
            decoration: BoxDecoration(
              color: AppColors.error,
              borderRadius: BorderRadius.circular(4),
            ),
            child: Text(
              '$discountPercentage% خصم',
              style: AppTextStyles.caption.copyWith(
                color: Colors.white,
                fontWeight: FontWeight.bold,
                fontSize: 10,
              ),
            ),
          ),
        if (hasDiscount) ...[
          const SizedBox(height: 4),
          Text(
            '$originalPrice $currency',
            style: AppTextStyles.caption.copyWith(
              decoration: TextDecoration.lineThrough,
              color: AppColors.textSecondary,
            ),
          ),
        ],
        Text(
          '$price $currency',
          style: AppTextStyles.priceSmall.copyWith(
            color: hasDiscount ? AppColors.error : AppColors.primary,
          ),
        ),
      ],
    );
  }
}