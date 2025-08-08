import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/enums/section_type_enum.dart';

class SectionPlaceholder extends StatelessWidget {
  final SectionType sectionType;
  final double height;

  const SectionPlaceholder({
    super.key,
    required this.sectionType,
    this.height = 200,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: height,
      margin: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMedium,
      ),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(12),
        boxShadow: const [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: 8,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Center(
        child: Text(
          _placeholderText(),
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textSecondary,
          ),
          textAlign: TextAlign.center,
        ),
      ),
    );
  }

  String _placeholderText() {
    switch (sectionType) {
      case SectionType.horizontalPropertyList:
        return 'جاري تحميل قائمة العقارات...';
      case SectionType.verticalPropertyGrid:
        return 'جاري تحميل شبكة العقارات...';
      case SectionType.cityCardsGrid:
        return 'جاري تحميل المدن...';
      case SectionType.premiumCarousel:
        return 'جاري تحميل المعروض المميز...';
      default:
        return 'جاري تحميل المحتوى...';
    }
  }
}