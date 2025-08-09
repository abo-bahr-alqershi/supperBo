import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';
import 'package:yemen_booking_app/core/utils/color_extensions.dart';

class CancellationDeadlineHasExpiredWidget extends StatelessWidget {
  final bool hasExpired;
  final DateTime deadline;
  final String? policy;

  const CancellationDeadlineHasExpiredWidget({
    super.key,
    required this.hasExpired,
    required this.deadline,
    this.policy,
  });

  @override
  Widget build(BuildContext context) {
    final dateFormat = DateFormat('dd MMM yyyy, HH:mm', 'ar');
    final now = DateTime.now();
    final difference = deadline.difference(now);
    
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: hasExpired
            ? AppColors.error.withValues(alpha: 0.1)
            : AppColors.warning.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: hasExpired ? AppColors.error : AppColors.warning,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(
                hasExpired ? Icons.cancel : Icons.warning_amber_rounded,
                color: hasExpired ? AppColors.error : AppColors.warning,
                size: AppDimensions.iconMedium,
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              Expanded(
                child: Text(
                  hasExpired
                      ? 'انتهت مهلة الإلغاء المجاني'
                      : 'سياسة الإلغاء',
                  style: AppTextStyles.subtitle2.copyWith(
                    fontWeight: FontWeight.bold,
                    color: hasExpired ? AppColors.error : AppColors.warning,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          
          if (hasExpired) ...[
            const Text(
              'لقد انتهت المهلة المسموح بها للإلغاء المجاني.',
              style: AppTextStyles.bodyMedium,
            ),
            const SizedBox(height: AppDimensions.spacingSm),
            Text(
              'في حالة الإلغاء الآن، سيتم تطبيق رسوم إلغاء وفقاً لسياسة العقار.',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ] else ...[
            const Text(
              'يمكنك إلغاء الحجز مجاناً حتى:',
              style: AppTextStyles.bodyMedium,
            ),
            const SizedBox(height: AppDimensions.spacingSm),
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(
                    Icons.access_time,
                    size: AppDimensions.iconSmall,
                    color: AppColors.warning,
                  ),
                  const SizedBox(width: AppDimensions.spacingXs),
                  Text(
                    dateFormat.format(deadline),
                    style: AppTextStyles.bodyMedium.copyWith(
                      fontWeight: FontWeight.bold,
                      color: AppColors.warning,
                    ),
                  ),
                ],
              ),
            ),
            if (difference.inDays <= 2) ...[
              const SizedBox(height: AppDimensions.spacingMd),
              _buildTimeRemaining(difference),
            ],
          ],
          
          if (policy != null) ...[
            const SizedBox(height: AppDimensions.spacingMd),
            Container(
              padding: const EdgeInsets.all(AppDimensions.paddingSmall),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'تفاصيل السياسة:',
                    style: AppTextStyles.caption.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: AppDimensions.spacingXs),
                  Text(
                    policy!,
                    style: AppTextStyles.caption.copyWith(
                      color: AppColors.textSecondary,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildTimeRemaining(Duration difference) {
    String timeText;
    IconData icon;
    Color color;

    if (difference.inHours <= 24) {
      timeText = 'متبقي ${difference.inHours} ساعة';
      icon = Icons.timer;
      color = AppColors.error;
    } else {
      timeText = 'متبقي ${difference.inDays} يوم';
      icon = Icons.calendar_today;
      color = AppColors.warning;
    }

    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMedium,
        vertical: AppDimensions.paddingSmall,
      ),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        border: Border.all(color: color),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            icon,
            size: AppDimensions.iconSmall,
            color: color,
          ),
          const SizedBox(width: AppDimensions.spacingXs),
          Text(
            timeText,
            style: AppTextStyles.caption.copyWith(
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
        ],
      ),
    );
  }
}