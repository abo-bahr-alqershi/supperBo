import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';

class GuestSelectorWidget extends StatelessWidget {
  final String label;
  final String? subtitle;
  final int count;
  final int minCount;
  final int maxCount;
  final Function(int) onChanged;
  final bool enabled;

  const GuestSelectorWidget({
    super.key,
    required this.label,
    this.subtitle,
    required this.count,
    required this.minCount,
    required this.maxCount,
    required this.onChanged,
    this.enabled = true,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: AppTextStyles.bodyMedium.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              if (subtitle != null) ...[
                const SizedBox(height: 2),
                Text(
                  subtitle!,
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ],
          ),
        ),
        Container(
          decoration: BoxDecoration(
            border: Border.all(color: AppColors.outline),
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              _buildButton(
                icon: Icons.remove,
                onPressed: enabled && count > minCount
                    ? () => onChanged(count - 1)
                    : null,
              ),
              Container(
                constraints: const BoxConstraints(minWidth: 50),
                padding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingMedium,
                  vertical: AppDimensions.paddingSmall,
                ),
                child: Text(
                  count.toString(),
                  style: AppTextStyles.subtitle2.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
              _buildButton(
                icon: Icons.add,
                onPressed: enabled && count < maxCount
                    ? () => onChanged(count + 1)
                    : null,
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildButton({
    required IconData icon,
    VoidCallback? onPressed,
  }) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onPressed,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        child: Container(
          padding: const EdgeInsets.all(AppDimensions.paddingSmall),
          child: Icon(
            icon,
            size: AppDimensions.iconSmall,
            color: onPressed != null ? AppColors.primary : AppColors.disabled,
          ),
        ),
      ),
    );
  }
}