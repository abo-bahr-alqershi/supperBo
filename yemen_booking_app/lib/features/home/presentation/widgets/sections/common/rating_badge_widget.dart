// lib/features/home/presentation/widgets/sections/common/rating_badge_widget.dart

import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/utils/color_extensions.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_text_styles.dart';

class RatingBadgeWidget extends StatelessWidget {
  final double rating;
  final int? reviewCount;
  final bool showCount;

  const RatingBadgeWidget({
    super.key,
    required this.rating,
    this.reviewCount,
    this.showCount = true,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.9),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(
            Icons.star_rounded,
            size: 16,
            color: AppColors.ratingStar,
          ),
          const SizedBox(width: 4),
          Text(
            rating.toStringAsFixed(1),
            style: AppTextStyles.caption.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          if (showCount && reviewCount != null) ...[
            const SizedBox(width: 4),
            Text(
              '($reviewCount)',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ],
      ),
    );
  }
}