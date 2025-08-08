import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../base/base_section_widget.dart';

class PremiumPropertyCarousel extends BaseSectionWidget {
  final List<dynamic> properties;
  final Function(dynamic)? onPropertyTap;

  const PremiumPropertyCarousel({
    super.key,
    required super.section,
    required this.properties,
    required super.config,
    this.onPropertyTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (config.showTitle)
            buildSectionHeader(
              title: section.title,
              subtitle: section.subtitle,
            ),
          const SizedBox(height: AppDimensions.spacingSm),
          SizedBox(
            height: 300,
            child: PageView.builder(
              controller: PageController(viewportFraction: 0.88),
              itemCount: properties.length,
              itemBuilder: (context, index) {
                final property = properties[index];
                return _buildPremiumCard(property);
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPremiumCard(dynamic property) {
    return GestureDetector(
      onTap: () => onPropertyTap?.call(property),
      child: Container(
        margin: const EdgeInsets.only(right: AppDimensions.spacingMd),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(config.borderRadius),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow.withValues(alpha: 0.12),
              blurRadius: 12,
              offset: const Offset(0, 6),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Hero image area
            Container(
              height: 200,
              decoration: BoxDecoration(
                borderRadius: BorderRadius.vertical(
                  top: Radius.circular(config.borderRadius),
                ),
                image: DecorationImage(
                  image: NetworkImage(property.mainImageUrl ?? ''),
                  fit: BoxFit.cover,
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    property.name ?? '',
                    style: AppTextStyles.heading3.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 6),
                  Row(
                    children: [
                      const Icon(
                        Icons.location_on_outlined,
                        size: 16,
                        color: AppColors.textSecondary,
                      ),
                      const SizedBox(width: 4),
                      Expanded(
                        child: Text(
                          property.city ?? '',
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.textSecondary,
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      const SizedBox(width: AppDimensions.spacingSm),
                      if (property.averageRating != null)
                        Row(
                          children: [
                            const Icon(
                              Icons.star_rounded,
                              size: 18,
                              color: AppColors.ratingStar,
                            ),
                            const SizedBox(width: 2),
                            Text(
                              property.averageRating.toString(),
                              style: AppTextStyles.bodyMedium.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ],
                        ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  if (property.basePrice != null)
                    Text(
                      '${property.basePrice} ${property.currency}',
                      style: AppTextStyles.price,
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}