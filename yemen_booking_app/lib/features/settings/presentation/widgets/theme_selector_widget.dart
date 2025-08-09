import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../bloc/settings_bloc.dart';
import '../bloc/settings_event.dart';

class ThemeSelectorWidget extends StatelessWidget {
  final bool isDarkMode;
  final Function(bool)? onChanged;
  final bool showLabel;
  final ThemeSelectorStyle style;

  const ThemeSelectorWidget({
    super.key,
    required this.isDarkMode,
    this.onChanged,
    this.showLabel = false,
    this.style = ThemeSelectorStyle.toggle,
  });

  @override
  Widget build(BuildContext context) {
    switch (style) {
      case ThemeSelectorStyle.toggle:
        return _buildToggleStyle(context);
      case ThemeSelectorStyle.segmented:
        return _buildSegmentedStyle(context);
      case ThemeSelectorStyle.card:
        return _buildCardStyle(context);
      case ThemeSelectorStyle.switchWidget:
        return _buildSwitchStyle(context);
    }
  }

  Widget _buildToggleStyle(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: isDarkMode 
            ? AppColors.primaryDark.withValues(alpha: 0.1) 
            : AppColors.secondary.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.radiusCircular),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          _buildToggleOption(
            icon: Icons.light_mode_rounded,
            isSelected: !isDarkMode,
            onTap: () => _handleThemeChange(false, context),
          ),
          _buildToggleOption(
            icon: Icons.dark_mode_rounded,
            isSelected: isDarkMode,
            onTap: () => _handleThemeChange(true, context),
          ),
        ],
      ),
    );
  }

  Widget _buildToggleOption({
    required IconData icon,
    required bool isSelected,
    required VoidCallback onTap,
  }) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.all(AppDimensions.paddingSmall),
        decoration: BoxDecoration(
          color: isSelected 
              ? (isDarkMode ? AppColors.primaryDark : AppColors.secondary)
              : Colors.transparent,
          borderRadius: BorderRadius.circular(AppDimensions.radiusCircular),
        ),
        child: Icon(
          icon,
          size: 20,
          color: isSelected 
              ? AppColors.white 
              : (isDarkMode ? AppColors.textSecondaryDark : AppColors.textSecondary),
        ),
      ),
    );
  }

  Widget _buildSegmentedStyle(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: AppColors.divider,
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          _buildSegmentOption(
            context: context,
            icon: Icons.light_mode_rounded,
            label: 'فاتح',
            isSelected: !isDarkMode,
            onTap: () => _handleThemeChange(false, context),
          ),
          Container(
            width: 1,
            height: 32,
            color: AppColors.divider,
          ),
          _buildSegmentOption(
            context: context,
            icon: Icons.dark_mode_rounded,
            label: 'داكن',
            isSelected: isDarkMode,
            onTap: () => _handleThemeChange(true, context),
          ),
        ],
      ),
    );
  }

  Widget _buildSegmentOption({
    required BuildContext context,
    required IconData icon,
    required String label,
    required bool isSelected,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        decoration: BoxDecoration(
          color: isSelected 
              ? AppColors.primary.withValues(alpha: 0.1)
              : Colors.transparent,
        ),
        child: Row(
          children: [
            Icon(
              icon,
              size: 18,
              color: isSelected 
                  ? AppColors.primary
                  : AppColors.textSecondary,
            ),
            if (showLabel) ...[
              const SizedBox(width: AppDimensions.spacingXs),
              Text(
                label,
                style: AppTextStyles.bodySmall.copyWith(
                  color: isSelected 
                      ? AppColors.primary
                      : AppColors.textSecondary,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildCardStyle(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingSmall),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (showLabel) ...[
            Text(
              'اختر المظهر',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingSm),
          ],
          Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              _buildCardOption(
                context: context,
                icon: Icons.light_mode_rounded,
                label: 'فاتح',
                isSelected: !isDarkMode,
                onTap: () => _handleThemeChange(false, context),
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              _buildCardOption(
                context: context,
                icon: Icons.dark_mode_rounded,
                label: 'داكن',
                isSelected: isDarkMode,
                onTap: () => _handleThemeChange(true, context),
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              _buildCardOption(
                context: context,
                icon: Icons.phone_android_rounded,
                label: 'تلقائي',
                isSelected: false,
                onTap: () {
                  // Auto theme based on system
                  final brightness = MediaQuery.of(context).platformBrightness;
                  _handleThemeChange(brightness == Brightness.dark, context);
                },
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildCardOption({
    required BuildContext context,
    required IconData icon,
    required String label,
    required bool isSelected,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        decoration: BoxDecoration(
          color: isSelected 
              ? AppColors.primary
              : AppColors.gray200,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              size: 24,
              color: isSelected 
                  ? AppColors.white
                  : AppColors.textSecondary,
            ),
            const SizedBox(height: AppDimensions.spacingXs),
            Text(
              label,
              style: AppTextStyles.caption.copyWith(
                color: isSelected 
                    ? AppColors.white
                    : AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSwitchStyle(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (showLabel) ...[
          Icon(
            isDarkMode ? Icons.dark_mode_rounded : Icons.light_mode_rounded,
            size: 20,
            color: AppColors.textSecondary,
          ),
          const SizedBox(width: AppDimensions.spacingSm),
        ],
        Switch.adaptive(
          value: isDarkMode,
          onChanged: (value) => _handleThemeChange(value, context),
          activeColor: AppColors.primary,
          activeTrackColor: AppColors.primary.withValues(alpha: 0.5),
        ),
      ],
    );
  }

  void _handleThemeChange(bool isDark, BuildContext context) {
    if (onChanged != null) {
      onChanged!(isDark);
    } else {
      context.read<SettingsBloc>().add(UpdateThemeEvent(isDark));
    }
    
    // Show feedback
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          isDark ? 'تم تفعيل الوضع الليلي' : 'تم تفعيل الوضع النهاري',
        ),
        backgroundColor: AppColors.success,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
        duration: const Duration(seconds: 2),
      ),
    );
  }
}

enum ThemeSelectorStyle {
  toggle,
  segmented,
  card,
  switchWidget,
}

// Quick theme toggle for app bar
class QuickThemeToggle extends StatelessWidget {
  final bool isDarkMode;
  final VoidCallback? onToggle;

  const QuickThemeToggle({
    super.key,
    required this.isDarkMode,
    this.onToggle,
  });

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: AnimatedSwitcher(
        duration: const Duration(milliseconds: 300),
        transitionBuilder: (child, animation) {
          return RotationTransition(
            turns: animation,
            child: FadeTransition(
              opacity: animation,
              child: child,
            ),
          );
        },
        child: Icon(
          isDarkMode ? Icons.light_mode_rounded : Icons.dark_mode_rounded,
          key: ValueKey(isDarkMode),
          color: Theme.of(context).iconTheme.color,
        ),
      ),
      onPressed: onToggle ?? () {
        context.read<SettingsBloc>().add(UpdateThemeEvent(!isDarkMode));
      },
      tooltip: isDarkMode ? 'التبديل للوضع النهاري' : 'التبديل للوضع الليلي',
    );
  }
}