// lib/features/home/presentation/widgets/sections/sponsored/multi_property_ad_widget.dart

import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/utils/color_extensions.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../base/base_section_widget.dart';

class MultiPropertyAdWidget extends BaseSectionWidget {
  final List<dynamic> properties;
  final Function(dynamic)? onPropertyTap;

  const MultiPropertyAdWidget({
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
          GridView.builder(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 2,
              childAspectRatio: 0.85,
              crossAxisSpacing: AppDimensions.spacingMd,
              mainAxisSpacing: AppDimensions.spacingMd,
            ),
            itemCount: properties.take(4).length,
            itemBuilder: (context, index) {
              final property = properties[index];
              return _buildPropertyCard(property);
            },
          ),
        ],
      ),
    );
  }

  Widget _buildPropertyCard(dynamic property) {
    return GestureDetector(
      onTap: () => onPropertyTap?.call(property),
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(config.borderRadius),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow.withValues(alpha: 0.1),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: ClipRRect(
          borderRadius: BorderRadius.circular(config.borderRadius),
          child: Stack(
            children: [
              Positioned.fill(
                child: CachedImageWidget(
                  imageUrl: property.mainImageUrl ?? '',
                  fit: BoxFit.cover,
                ),
              ),
              Positioned(
                bottom: 0,
                left: 0,
                right: 0,
                child: Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      colors: [
                        Colors.transparent,
                        Colors.black.withValues(alpha: 0.7),
                      ],
                    ),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        property.name ?? '',
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      if (property.basePrice != null)
                        Text(
                          '${property.basePrice} ${property.currency}',
                          style: AppTextStyles.caption.copyWith(
                            color: Colors.white,
                          ),
                        ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}