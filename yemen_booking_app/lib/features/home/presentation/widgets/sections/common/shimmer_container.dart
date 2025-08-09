// lib/features/home/presentation/widgets/sections/common/shimmer_container.dart

import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/utils/color_extensions.dart';
import 'package:shimmer/shimmer.dart';
import '../../../../../../core/theme/app_colors.dart';

class ShimmerContainer extends StatelessWidget {
  final double? width;
  final double? height;
  final BorderRadius? borderRadius;

  const ShimmerContainer({
    super.key,
    this.width,
    this.height,
    this.borderRadius,
  });

  @override
  Widget build(BuildContext context) {
    return Shimmer.fromColors(
      baseColor: AppColors.shimmer,
      highlightColor: AppColors.shimmer.withValues(alpha: 0.5),
      child: Container(
        width: width,
        height: height,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: borderRadius ?? BorderRadius.circular(8),
        ),
      ),
    );
  }
}