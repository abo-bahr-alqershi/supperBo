// lib/features/home/presentation/widgets/sections/common/carousel_indicators_widget.dart

import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';

class CarouselIndicatorsWidget extends StatelessWidget {
  final int itemCount;
  final int currentIndex;
  final Color? activeColor;
  final Color? inactiveColor;

  const CarouselIndicatorsWidget({
    super.key,
    required this.itemCount,
    required this.currentIndex,
    this.activeColor,
    this.inactiveColor,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(
        itemCount,
        (index) => AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: currentIndex == index ? 24 : 8,
          height: 8,
          decoration: BoxDecoration(
            color: currentIndex == index
                ? (activeColor ?? AppColors.primary)
                : (inactiveColor ?? AppColors.gray200),
            borderRadius: BorderRadius.circular(4),
          ),
        ),
      ),
    );
  }
}