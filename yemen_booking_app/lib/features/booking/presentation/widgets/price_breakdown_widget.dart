import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';
import 'package:yemen_booking_app/core/widgets/price_widget.dart';

class PriceBreakdownWidget extends StatelessWidget {
  final int nights;
  final double pricePerNight;
  final double servicesTotal;
  final double taxRate;
  final String currency;
  final double? discount;
  final String? promoCode;

  const PriceBreakdownWidget({
    super.key,
    required this.nights,
    required this.pricePerNight,
    required this.servicesTotal,
    required this.taxRate,
    required this.currency,
    this.discount,
    this.promoCode,
  });

  @override
  Widget build(BuildContext context) {
    final accommodationTotal = nights * pricePerNight;
    final subtotal = accommodationTotal + servicesTotal;
    final discountAmount = discount ?? 0;
    final afterDiscount = subtotal - discountAmount;
    final taxAmount = afterDiscount * taxRate;
    final total = afterDiscount + taxAmount;

    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        boxShadow: const [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurSmall,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(
                Icons.receipt_long,
                color: AppColors.primary,
                size: AppDimensions.iconMedium,
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              Text(
                'تفاصيل السعر',
                style: AppTextStyles.subtitle1.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          
          // Accommodation
          _buildPriceRow(
            label: 'الإقامة ($nights × ${pricePerNight.toStringAsFixed(0)} $currency)',
            amount: accommodationTotal,
            style: AppTextStyles.bodyMedium,
          ),
          
          // Services
          if (servicesTotal > 0) ...[
            const SizedBox(height: AppDimensions.spacingSm),
            _buildPriceRow(
              label: 'الخدمات الإضافية',
              amount: servicesTotal,
              style: AppTextStyles.bodyMedium,
            ),
          ],
          
          // Subtotal
          const SizedBox(height: AppDimensions.spacingMd),
          Container(
            padding: const EdgeInsets.symmetric(vertical: AppDimensions.paddingSmall),
            decoration: const BoxDecoration(
              border: Border(
                top: BorderSide(color: AppColors.divider),
                bottom: BorderSide(color: AppColors.divider),
              ),
            ),
            child: _buildPriceRow(
              label: 'المجموع الفرعي',
              amount: subtotal,
              style: AppTextStyles.bodyMedium.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          
          // Discount
          if (discountAmount > 0) ...[
            const SizedBox(height: AppDimensions.spacingSm),
            Row(
              children: [
                Expanded(
                  child: Row(
                    children: [
                      Text(
                        'الخصم',
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppColors.success,
                        ),
                      ),
                      if (promoCode != null) ...[
                        const SizedBox(width: AppDimensions.spacingXs),
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: AppDimensions.paddingSmall,
                            vertical: 2,
                          ),
                          decoration: BoxDecoration(
                            color: AppColors.success.withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
                          ),
                          child: Text(
                            promoCode!,
                            style: AppTextStyles.caption.copyWith(
                              color: AppColors.success,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
                Text(
                  '- ${discountAmount.toStringAsFixed(0)} $currency',
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppColors.success,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
          ],
          
          // Tax
          const SizedBox(height: AppDimensions.spacingSm),
          _buildPriceRow(
            label: 'الضرائب (${(taxRate * 100).toStringAsFixed(0)}%)',
            amount: taxAmount,
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          
          // Total
          const SizedBox(height: AppDimensions.spacingMd),
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            decoration: BoxDecoration(
              color: AppColors.primary.withValues(alpha: 0.05),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
              border: Border.all(color: AppColors.primary),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'المجموع الكلي',
                  style: AppTextStyles.subtitle1.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
                PriceWidget(
                  price: total,
                  currency: currency,
                  displayType: PriceDisplayType.normal,
                  priceStyle: AppTextStyles.price.copyWith(
                    color: AppColors.primary,
                  ),
                ),
              ],
            ),
          ),
          
          // Payment info
          const SizedBox(height: AppDimensions.spacingMd),
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingSmall),
            decoration: BoxDecoration(
              color: AppColors.info.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
            ),
            child: Row(
              children: [
                const Icon(
                  Icons.info_outline,
                  size: AppDimensions.iconSmall,
                  color: AppColors.info,
                ),
                const SizedBox(width: AppDimensions.spacingXs),
                Expanded(
                  child: Text(
                    'السعر شامل جميع الرسوم والضرائب',
                    style: AppTextStyles.caption.copyWith(
                      color: AppColors.info,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPriceRow({
    required String label,
    required double amount,
    TextStyle? style,
  }) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: style ?? AppTextStyles.bodyMedium,
        ),
        Text(
          '${amount.toStringAsFixed(0)} $currency',
          style: (style ?? AppTextStyles.bodyMedium).copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }
}