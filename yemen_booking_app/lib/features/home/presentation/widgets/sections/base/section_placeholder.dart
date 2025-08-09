// lib/features/home/presentation/widgets/sections/base/section_placeholder.dart

import 'package:flutter/material.dart';
import 'package:shimmer/shimmer.dart';
import '../../../../../../core/enums/section_type_enum.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_dimensions.dart';

class SectionPlaceholder extends StatelessWidget {
  final SectionType sectionType;
  final double height;

  const SectionPlaceholder({
    super.key,
    required this.sectionType,
    required this.height,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: height,
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Shimmer.fromColors(
        baseColor: AppColors.shimmer,
        highlightColor: AppColors.shimmer.withValues(alpha: 0.5),
        child: _buildPlaceholderContent(),
      ),
    );
  }

  Widget _buildPlaceholderContent() {
    switch (sectionType) {
      case SectionType.horizontalPropertyList:
        return _buildHorizontalListPlaceholder();
      case SectionType.verticalPropertyGrid:
        return _buildGridPlaceholder();
      case SectionType.premiumCarousel:
        return _buildCarouselPlaceholder();
      default:
        return _buildDefaultPlaceholder();
    }
  }

  Widget _buildHorizontalListPlaceholder() {
    return ListView.builder(
      scrollDirection: Axis.horizontal,
      itemCount: 3,
      itemBuilder: (context, index) {
        return Container(
          width: 200,
          margin: const EdgeInsets.only(right: AppDimensions.spacingMd),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
        );
      },
    );
  }

  Widget _buildGridPlaceholder() {
    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        childAspectRatio: 0.8,
        crossAxisSpacing: AppDimensions.spacingMd,
        mainAxisSpacing: AppDimensions.spacingMd,
      ),
      itemCount: 4,
      itemBuilder: (context, index) {
        return Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
        );
      },
    );
  }

  Widget _buildCarouselPlaceholder() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
      ),
    );
  }

  Widget _buildDefaultPlaceholder() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
    );
  }
}