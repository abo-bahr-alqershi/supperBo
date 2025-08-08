// lib/features/home/presentation/widgets/sections/sponsored/single_property_ad_widget.dart

import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../base/base_section_widget.dart';

class SinglePropertyAdWidget extends BaseSectionWidget {
  final dynamic property;
  final Function(dynamic)? onTap;

  const SinglePropertyAdWidget({
    super.key,
    required super.section,
    required this.property,
    required super.config,
    this.onTap,
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
          GestureDetector(
            onTap: () => onTap?.call(property),
            child: Container(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(config.borderRadius),
                boxShadow: [
                  BoxShadow(
                    color: AppColors.shadow.withValues(alpha: 0.12),
                    blurRadius: 16,
                    offset: const Offset(0, 8),
                  ),
                ],
              ),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(config.borderRadius),
                child: Stack(
                  children: [
                    Positioned.fill(
                      child: CachedImageWidget(
                        imageUrl: (property?.mainImageUrl ?? property?.imageUrl ?? '').toString(),
                        fit: BoxFit.cover,
                      ),
                    ),
                    Positioned(
                      bottom: 0,
                      left: 0,
                      right: 0,
                      child: Container(
                        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            colors: [
                              Colors.transparent,
                              Colors.black.withValues(alpha: 0.75),
                            ],
                          ),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              (property?.name ?? property?.title ?? '').toString(),
                              style: AppTextStyles.heading3.copyWith(
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                              ),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            ),
                            const SizedBox(height: 4),
                            Row(
                              children: [
                                if (property?.city != null)
                                  Expanded(
                                    child: Text(
                                      (property?.city ?? '').toString(),
                                      style: AppTextStyles.bodySmall.copyWith(
                                        color: Colors.white70,
                                      ),
                                      maxLines: 1,
                                      overflow: TextOverflow.ellipsis,
                                    ),
                                  ),
                                if (property?.basePrice != null)
                                  Text(
                                    '${property?.basePrice ?? ''} ${property?.currency ?? ''}',
                                    style: AppTextStyles.bodySmall.copyWith(
                                      color: Colors.white,
                                      fontWeight: FontWeight.w600,
                                    ),
                                  ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}