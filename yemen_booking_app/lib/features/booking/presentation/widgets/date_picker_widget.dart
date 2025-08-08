import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';

class DatePickerWidget extends StatelessWidget {
  final String label;
  final DateTime? selectedDate;
  final DateTime firstDate;
  final DateTime lastDate;
  final Function(DateTime) onDateSelected;
  final bool enabled;
  final IconData? icon;
  final String? errorText;

  const DatePickerWidget({
    super.key,
    required this.label,
    this.selectedDate,
    required this.firstDate,
    required this.lastDate,
    required this.onDateSelected,
    this.enabled = true,
    this.icon,
    this.errorText,
  });

  @override
  Widget build(BuildContext context) {
    final dateFormat = DateFormat('dd MMM yyyy', 'ar');
    
    return InkWell(
      onTap: enabled ? () => _selectDate(context) : null,
      child: Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: enabled ? AppColors.surface : AppColors.surface.withValues(alpha: 0.5),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        child: Row(
          children: [
            if (icon != null) ...[
              Icon(
                icon,
                color: enabled
                    ? (errorText != null ? AppColors.error : AppColors.primary)
                    : AppColors.disabled,
                size: AppDimensions.iconMedium,
              ),
              const SizedBox(width: AppDimensions.spacingMd),
            ],
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: AppTextStyles.caption.copyWith(
                      color: enabled
                          ? (errorText != null ? AppColors.error : AppColors.textSecondary)
                          : AppColors.disabled,
                    ),
                  ),
                  const SizedBox(height: AppDimensions.spacingXs),
                  Text(
                    selectedDate != null
                        ? dateFormat.format(selectedDate!)
                        : 'اختر التاريخ',
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: enabled
                          ? (selectedDate != null
                              ? AppColors.textPrimary
                              : AppColors.textHint)
                          : AppColors.disabled,
                      fontWeight: selectedDate != null ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                  if (errorText != null) ...[
                    const SizedBox(height: AppDimensions.spacingXs),
                    Text(
                      errorText!,
                      style: AppTextStyles.caption.copyWith(
                        color: AppColors.error,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            Icon(
              Icons.calendar_today_outlined,
              color: enabled ? AppColors.textSecondary : AppColors.disabled,
              size: AppDimensions.iconSmall,
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _selectDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: selectedDate ?? firstDate,
      firstDate: firstDate,
      lastDate: lastDate,
      locale: const Locale('ar'),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: AppColors.primary,
              onPrimary: AppColors.white,
              surface: AppColors.surface,
              onSurface: AppColors.textPrimary,
            ),
            textButtonTheme: TextButtonThemeData(
              style: TextButton.styleFrom(
                foregroundColor: AppColors.primary,
              ),
            ), dialogTheme: const DialogThemeData(backgroundColor: AppColors.surface),
          ),
          child: child!,
        );
      },
    );
    
    if (picked != null && picked != selectedDate) {
      onDateSelected(picked);
    }
  }
}