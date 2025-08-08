// lib/features/home/presentation/widgets/sections/offers/single_property_offer_widget.dart

import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';
import '../common/countdown_timer_widget.dart';

class SinglePropertyOfferWidget extends BaseSectionWidget {
  final dynamic offer;
  final Function(dynamic)? onTap;

  const SinglePropertyOfferWidget({
    super.key,
    required HomeSection section,
    required this.offer,
    required SectionConfig config,
    this.onTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: GestureDetector(
        onTap: () => onTap?.call(offer),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Stack(
              children: [
                Container(
                  height: 200,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(config.borderRadius),
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(config.borderRadius),
                    child: CachedImageWidget(
                      imageUrl: offer.bannerImageUrl ?? '',
                      fit: BoxFit.cover,
                    ),
                  ),
                ),
                
                // Discount Badge
                Positioned(
                  top: 16,
                  right: 16,
                  child: Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                    decoration: BoxDecoration(
                      color: AppColors.error,
                      borderRadius: BorderRadius.circular(20),
                    ),
                    child: Text(
                      '${offer.discountPercentage}% خصم',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ),
              ],
            ),
            
            const SizedBox(height: 16),
            
            Text(
              offer.title ?? '',
              style: AppTextStyles.heading3.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            
            if (offer.subtitle != null) ...[
              const SizedBox(height: 4),
              Text(
                offer.subtitle!,
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ],
            
            const SizedBox(height: 12),
            
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    if (offer.originalPrice != null)
                      Text(
                        '${offer.originalPrice} ${offer.currency}',
                        style: AppTextStyles.bodyMedium.copyWith(
                          decoration: TextDecoration.lineThrough,
                          color: AppColors.textSecondary,
                        ),
                      ),
                    Text(
                      '${offer.offerPrice} ${offer.currency}',
                      style: AppTextStyles.price.copyWith(
                        color: AppColors.primary,
                      ),
                    ),
                  ],
                ),
                
                if (offer.endDate != null)
                  CountdownTimerWidget(
                    endTime: offer.endDate!,
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}