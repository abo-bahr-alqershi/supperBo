// lib/features/home/presentation/widgets/sections/base/base_section_widget.dart

import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';

abstract class BaseSectionWidget extends StatelessWidget {
  final HomeSection section;
  final SectionConfig config;

  const BaseSectionWidget({
    super.key,
    required this.section,
    required this.config,
  });

  Widget buildSectionContainer({
    required Widget child,
    EdgeInsets? padding,
    EdgeInsets? margin,
    BoxDecoration? decoration,
  }) {
    return Container(
      padding: padding ?? const EdgeInsets.all(AppDimensions.paddingMedium),
      margin: margin ?? EdgeInsets.zero,
      decoration: decoration ??
          BoxDecoration(
            color: _getBackgroundColor(),
            borderRadius: BorderRadius.circular(
              config.borderRadius,
            ),
          ),
      child: child,
    );
  }

  Widget buildSectionHeader({
    String? title,
    String? subtitle,
    VoidCallback? onViewAll,
  }) {
    return Padding(
      padding: const EdgeInsets.only(
        bottom: AppDimensions.spacingMd,
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (title != null)
                  Text(
                    title,
                    style: AppTextStyles.heading2.copyWith(
                      fontWeight: FontWeight.bold,
                      color: _getTextColor(),
                    ),
                  ),
                if (subtitle != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: _getSecondaryTextColor(),
                    ),
                  ),
                ],
              ],
            ),
          ),
          if (onViewAll != null)
            TextButton(
              onPressed: onViewAll,
              child: Text(
                'عرض الكل',
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.primary,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Color _getBackgroundColor() {
    if (config.backgroundColor != null) {
      return Color(int.parse(config.backgroundColor!.replaceAll('#', '0xFF')));
    }
    return AppColors.surface;
  }

  Color _getTextColor() {
    if (config.textColor != null) {
      return Color(int.parse(config.textColor!.replaceAll('#', '0xFF')));
    }
    return AppColors.textPrimary;
  }

  Color _getSecondaryTextColor() {
    if (config.textColor != null) {
      return Color(int.parse(config.textColor!.replaceAll('#', '0xFF')))
          .withValues(alpha: 0.7);
    }
    return AppColors.textSecondary;
  }

  @override
  Widget build(BuildContext context);
}