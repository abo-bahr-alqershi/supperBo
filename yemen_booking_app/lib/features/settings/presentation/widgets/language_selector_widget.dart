import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../bloc/settings_bloc.dart';
import '../bloc/settings_event.dart';

class LanguageSelectorWidget extends StatelessWidget {
  final String currentLanguage;
  final Function(String)? onLanguageChanged;
  final bool showLabel;
  final bool expanded;

  const LanguageSelectorWidget({
    super.key,
    required this.currentLanguage,
    this.onLanguageChanged,
    this.showLabel = false,
    this.expanded = false,
  });

  @override
  Widget build(BuildContext context) {
    if (expanded) {
      return _buildExpandedSelector(context);
    }
    return _buildCompactSelector(context);
  }

  Widget _buildCompactSelector(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
        vertical: AppDimensions.paddingXSmall,
      ),
      decoration: BoxDecoration(
        color: AppColors.primary.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        border: Border.all(
          color: AppColors.primary.withValues(alpha: 0.3),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (showLabel) ...[
            const Icon(
              Icons.language_rounded,
              size: 18,
              color: AppColors.primary,
            ),
            const SizedBox(width: AppDimensions.spacingXs),
          ],
          DropdownButton<String>(
            value: currentLanguage,
            isDense: true,
            underline: const SizedBox.shrink(),
            icon: const Icon(
              Icons.keyboard_arrow_down_rounded,
              size: 20,
              color: AppColors.primary,
            ),
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.primary,
              fontWeight: FontWeight.w600,
            ),
            dropdownColor: isDark ? AppColors.surfaceDark : AppColors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            items: _buildDropdownItems(),
            onChanged: (value) {
              if (value != null && value != currentLanguage) {
                if (onLanguageChanged != null) {
                  onLanguageChanged!(value);
                } else {
                  context.read<SettingsBloc>().add(UpdateLanguageEvent(value));
                }
              }
            },
          ),
        ],
      ),
    );
  }

  Widget _buildExpandedSelector(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return Container(
      decoration: BoxDecoration(
        color: isDark ? AppColors.surfaceDark : AppColors.white,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        border: Border.all(
          color: AppColors.divider,
          width: 1,
        ),
      ),
      child: Column(
        children: [
          _buildLanguageOption(
            context: context,
            code: 'ar',
            name: 'العربية',
            englishName: 'Arabic',
            flag: '🇾🇪',
            isSelected: currentLanguage == 'ar',
          ),
          const Divider(height: 1, thickness: 0.5),
          _buildLanguageOption(
            context: context,
            code: 'en',
            name: 'English',
            englishName: 'الإنجليزية',
            flag: '🇬🇧',
            isSelected: currentLanguage == 'en',
          ),
        ],
      ),
    );
  }

  Widget _buildLanguageOption({
    required BuildContext context,
    required String code,
    required String name,
    required String englishName,
    required String flag,
    required bool isSelected,
  }) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: () {
          if (!isSelected) {
            if (onLanguageChanged != null) {
              onLanguageChanged!(code);
            } else {
              context.read<SettingsBloc>().add(UpdateLanguageEvent(code));
            }
            
            // Show success message
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(
                  code == 'ar' 
                      ? 'تم تغيير اللغة إلى العربية' 
                      : 'Language changed to English',
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
        },
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        child: Container(
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          child: Row(
            children: [
              // Flag
              Text(
                flag,
                style: const TextStyle(fontSize: 28),
              ),
              
              const SizedBox(width: AppDimensions.spacingMd),
              
              // Language Names
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      name,
                      style: AppTextStyles.bodyLarge.copyWith(
                        color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
                        fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      englishName,
                      style: AppTextStyles.caption.copyWith(
                        color: isDark ? AppColors.textSecondaryDark : AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
              
              // Selected Indicator
              if (isSelected)
                Container(
                  padding: const EdgeInsets.all(2),
                  decoration: const BoxDecoration(
                    color: AppColors.primary,
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.check,
                    color: AppColors.white,
                    size: 16,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  List<DropdownMenuItem<String>> _buildDropdownItems() {
    return [
      const DropdownMenuItem(
        value: 'ar',
        child: Row(
          children: [
            Text('🇾🇪', style: TextStyle(fontSize: 18)),
            SizedBox(width: 8),
            Text('العربية'),
          ],
        ),
      ),
      const DropdownMenuItem(
        value: 'en',
        child: Row(
          children: [
            Text('🇬🇧', style: TextStyle(fontSize: 18)),
            SizedBox(width: 8),
            Text('English'),
          ],
        ),
      ),
    ];
  }
}

// Quick language toggle button
class QuickLanguageToggle extends StatelessWidget {
  final String currentLanguage;
  final VoidCallback? onToggle;

  const QuickLanguageToggle({
    super.key,
    required this.currentLanguage,
    this.onToggle,
  });

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: AppColors.primary.withValues(alpha: 0.1),
          shape: BoxShape.circle,
        ),
        child: Text(
          currentLanguage == 'ar' ? 'EN' : 'ع',
          style: AppTextStyles.button.copyWith(
            color: AppColors.primary,
            fontSize: 12,
          ),
        ),
      ),
      onPressed: onToggle ?? () {
        final newLang = currentLanguage == 'ar' ? 'en' : 'ar';
        context.read<SettingsBloc>().add(UpdateLanguageEvent(newLang));
      },
    );
  }
}