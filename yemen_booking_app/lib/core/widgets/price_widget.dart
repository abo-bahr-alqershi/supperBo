import 'package:flutter/material.dart';
import '../utils/color_extensions.dart';
import '../theme/app_colors.dart';
import '../theme/app_text_styles.dart';
import '../theme/app_dimensions.dart';

class PriceWidget extends StatelessWidget {
  final double price;
  final double? originalPrice;
  final String currency;
  final PriceDisplayType displayType;
  final TextStyle? priceStyle;
  final TextStyle? originalPriceStyle;
  final TextStyle? currencyStyle;
  final String? period;
  final bool showCurrencySymbol;
  final MainAxisAlignment alignment;
  final bool animate;

  const PriceWidget({
    super.key,
    required this.price,
    this.originalPrice,
    this.currency = 'YER',
    this.displayType = PriceDisplayType.normal,
    this.priceStyle,
    this.originalPriceStyle,
    this.currencyStyle,
    this.period,
    this.showCurrencySymbol = true,
    this.alignment = MainAxisAlignment.start,
    this.animate = false,
  });

  @override
  Widget build(BuildContext context) {
    switch (displayType) {
      case PriceDisplayType.normal:
        return _buildNormalPrice(context);
      case PriceDisplayType.discount:
        return _buildDiscountPrice(context);
      case PriceDisplayType.compact:
        return _buildCompactPrice(context);
      case PriceDisplayType.detailed:
        return _buildDetailedPrice(context);
    }
  }

  Widget _buildNormalPrice(BuildContext context) {
    final Widget priceWidget = Row(
      mainAxisAlignment: alignment,
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          _formatPrice(price),
          style: priceStyle ?? AppTextStyles.price,
        ),
        if (showCurrencySymbol) ...[
          const SizedBox(width: AppDimensions.spacingXs),
          Text(
            currency,
            style: currencyStyle ?? AppTextStyles.priceSmall.copyWith(
              fontWeight: FontWeight.normal,
            ),
          ),
        ],
        if (period != null) ...[
          const SizedBox(width: AppDimensions.spacingXs),
          Text(
            '/ $period',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ],
    );

    if (animate) {
      return TweenAnimationBuilder<double>(
        tween: Tween(begin: 0, end: price),
        duration: const Duration(milliseconds: 500),
        builder: (context, value, child) {
          return Row(
            mainAxisAlignment: alignment,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                _formatPrice(value),
                style: priceStyle ?? AppTextStyles.price,
              ),
              if (showCurrencySymbol) ...[
                const SizedBox(width: AppDimensions.spacingXs),
                Text(
                  currency,
                  style: currencyStyle ?? AppTextStyles.priceSmall.copyWith(
                    fontWeight: FontWeight.normal,
                  ),
                ),
              ],
            ],
          );
        },
      );
    }

    return priceWidget;
  }

  Widget _buildDiscountPrice(BuildContext context) {
    if (originalPrice == null || originalPrice == price) {
      return _buildNormalPrice(context);
    }

    final double discountPercentage = 
        ((originalPrice! - price) / originalPrice! * 100);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        Row(
          mainAxisAlignment: alignment,
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              _formatPrice(price),
              style: priceStyle ?? AppTextStyles.price.copyWith(
                color: AppColors.success,
              ),
            ),
            if (showCurrencySymbol) ...[
              const SizedBox(width: AppDimensions.spacingXs),
              Text(
                currency,
                style: currencyStyle ?? AppTextStyles.priceSmall.copyWith(
                  fontWeight: FontWeight.normal,
                  color: AppColors.success,
                ),
              ),
            ],
            const SizedBox(width: AppDimensions.spacingSm),
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingSmall,
                vertical: AppDimensions.paddingXSmall,
              ),
              decoration: BoxDecoration(
                color: AppColors.success.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
              ),
              child: Text(
                '${discountPercentage.toStringAsFixed(0)}% خصم',
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.success,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: AppDimensions.spacingXs),
        Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              _formatPrice(originalPrice!),
              style: originalPriceStyle ?? AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
                decoration: TextDecoration.lineThrough,
              ),
            ),
            if (showCurrencySymbol) ...[
              const SizedBox(width: AppDimensions.spacingXs),
              Text(
                currency,
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                  decoration: TextDecoration.lineThrough,
                ),
              ),
            ],
          ],
        ),
      ],
    );
  }

  Widget _buildCompactPrice(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
        vertical: AppDimensions.paddingXSmall,
      ),
      decoration: BoxDecoration(
        color: AppColors.primary.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            _formatPrice(price),
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.primary,
              fontWeight: FontWeight.bold,
            ),
          ),
          if (showCurrencySymbol) ...[
            const SizedBox(width: 2),
            Text(
              currency,
              style: AppTextStyles.caption.copyWith(
                color: AppColors.primary,
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildDetailedPrice(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: AppColors.outline,
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          if (originalPrice != null && originalPrice != price) ...[
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'السعر الأصلي:',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
                Text(
                  '${_formatPrice(originalPrice!)} $currency',
                  style: AppTextStyles.bodyMedium.copyWith(
                    decoration: TextDecoration.lineThrough,
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppDimensions.spacingXs),
          ],
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                period != null ? 'السعر / $period:' : 'السعر:',
                style: AppTextStyles.bodyMedium.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                '${_formatPrice(price)} $currency',
                style: AppTextStyles.price.copyWith(
                  color: originalPrice != null && originalPrice != price
                      ? AppColors.success
                      : AppColors.primary,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  String _formatPrice(double value) {
    if (value >= 1000000) {
      return '${(value / 1000000).toStringAsFixed(1)}M';
    } else if (value >= 1000) {
      return '${(value / 1000).toStringAsFixed(1)}K';
    }
    return value.toStringAsFixed(0);
  }
}

enum PriceDisplayType {
  normal,
  discount,
  compact,
  detailed,
}