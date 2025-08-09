import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class SettingsItemWidget extends StatelessWidget {
  final IconData icon;
  final String title;
  final String? subtitle;
  final Widget? trailing;
  final VoidCallback? onTap;
  final bool showArrow;
  final Color? iconColor;
  final bool isDestructive;
  final bool enabled;

  const SettingsItemWidget({
    super.key,
    required this.icon,
    required this.title,
    this.subtitle,
    this.trailing,
    this.onTap,
    this.showArrow = false,
    this.iconColor,
    this.isDestructive = false,
    this.enabled = true,
  });

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final effectiveIconColor = isDestructive 
        ? AppColors.error 
        : iconColor ?? AppColors.primary;
    
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: enabled ? onTap : null,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        child: Opacity(
          opacity: enabled ? 1.0 : 0.5,
          child: Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
              vertical: AppDimensions.paddingMedium,
            ),
            child: Row(
              children: [
                // Icon Container
                Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: effectiveIconColor.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                  ),
                  child: Icon(
                    icon,
                    color: effectiveIconColor,
                    size: AppDimensions.iconMedium,
                  ),
                ),
                
                const SizedBox(width: AppDimensions.spacingMd),
                
                // Title and Subtitle
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        title,
                        style: AppTextStyles.bodyLarge.copyWith(
                          color: isDestructive 
                              ? AppColors.error 
                              : (isDark ? AppColors.textPrimaryDark : AppColors.textPrimary),
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                      if (subtitle != null) ...[
                        const SizedBox(height: AppDimensions.spacingXs),
                        Text(
                          subtitle!,
                          style: AppTextStyles.bodySmall.copyWith(
                            color: isDark ? AppColors.textSecondaryDark : AppColors.textSecondary,
                          ),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ],
                  ),
                ),
                
                // Trailing Widget or Arrow
                if (trailing != null)
                  trailing!
                else if (showArrow)
                  Icon(
                    Icons.arrow_forward_ios_rounded,
                    size: 16,
                    color: isDark ? AppColors.textSecondaryDark : AppColors.textSecondary,
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

// Variation for toggle items
class SettingsToggleItem extends StatelessWidget {
  final IconData icon;
  final String title;
  final String? subtitle;
  final bool value;
  final ValueChanged<bool>? onChanged;
  final Color? iconColor;
  final bool enabled;

  const SettingsToggleItem({
    super.key,
    required this.icon,
    required this.title,
    this.subtitle,
    required this.value,
    required this.onChanged,
    this.iconColor,
    this.enabled = true,
  });

  @override
  Widget build(BuildContext context) {
    return SettingsItemWidget(
      icon: icon,
      title: title,
      subtitle: subtitle,
      iconColor: iconColor,
      enabled: enabled,
      trailing: Switch.adaptive(
        value: value,
        onChanged: enabled ? onChanged : null,
        activeColor: AppColors.primary,
      ),
      onTap: enabled 
          ? () => onChanged?.call(!value)
          : null,
    );
  }
}

// Variation for section headers
class SettingsSectionHeader extends StatelessWidget {
  final String title;
  final EdgeInsetsGeometry? padding;

  const SettingsSectionHeader({
    super.key,
    required this.title,
    this.padding,
  });

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return Container(
      padding: padding ?? const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMedium,
        vertical: AppDimensions.paddingSmall,
      ),
      child: Text(
        title.toUpperCase(),
        style: AppTextStyles.caption.copyWith(
          color: isDark ? AppColors.textSecondaryDark : AppColors.textSecondary,
          fontWeight: FontWeight.w600,
          letterSpacing: 0.5,
        ),
      ),
    );
  }
}