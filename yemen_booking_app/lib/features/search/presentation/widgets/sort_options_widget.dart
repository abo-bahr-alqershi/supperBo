import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class SortOptionsWidget extends StatelessWidget {
  final String? currentSort;
  final Function(String) onSortChanged;

  const SortOptionsWidget({
    super.key,
    this.currentSort,
    required this.onSortChanged,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () => _showSortOptions(context),
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(
            color: currentSort != null ? AppColors.primary : AppColors.outline,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.sort_rounded,
              size: AppDimensions.iconSmall,
              color: currentSort != null ? AppColors.primary : AppColors.textSecondary,
            ),
            const SizedBox(width: AppDimensions.spacingXs),
            Text(
              _getCurrentSortLabel(),
              style: AppTextStyles.bodySmall.copyWith(
                color: currentSort != null ? AppColors.primary : AppColors.textPrimary,
                fontWeight: currentSort != null ? FontWeight.bold : FontWeight.normal,
              ),
            ),
            const SizedBox(width: AppDimensions.spacingXs),
            const Icon(
              Icons.keyboard_arrow_down_rounded,
              size: AppDimensions.iconSmall,
              color: AppColors.textSecondary,
            ),
          ],
        ),
      ),
    );
  }

  void _showSortOptions(BuildContext context) {
    showModalBottomSheet(
      context: context,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusLg),
        ),
      ),
      builder: (context) {
        return SafeArea(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              _buildBottomSheetHandle(),
              _buildBottomSheetHeader(),
              _buildSortOptionsList(context),
            ],
          ),
        );
      },
    );
  }

  Widget _buildBottomSheetHandle() {
    return Container(
      margin: const EdgeInsets.only(top: AppDimensions.paddingSmall),
      width: 40,
      height: 4,
      decoration: BoxDecoration(
        color: AppColors.divider,
        borderRadius: BorderRadius.circular(2),
      ),
    );
  }

  Widget _buildBottomSheetHeader() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: const BoxDecoration(
        border: Border(
          bottom: BorderSide(
            color: AppColors.divider,
            width: 1,
          ),
        ),
      ),
      child: const Row(
        children: [
          Icon(
            Icons.sort_rounded,
            color: AppColors.primary,
          ),
          SizedBox(width: AppDimensions.spacingSm),
          Text(
            'ترتيب حسب',
            style: AppTextStyles.heading3,
          ),
        ],
      ),
    );
  }

  Widget _buildSortOptionsList(BuildContext context) {
    final sortOptions = [
      {
        'value': 'recommended',
        'label': 'موصى به',
        'icon': Icons.recommend_rounded,
        'description': 'الأكثر ملاءمة لبحثك',
      },
      {
        'value': 'price_asc',
        'label': 'السعر: الأقل أولاً',
        'icon': Icons.arrow_upward_rounded,
        'description': 'من الأرخص إلى الأغلى',
      },
      {
        'value': 'price_desc',
        'label': 'السعر: الأعلى أولاً',
        'icon': Icons.arrow_downward_rounded,
        'description': 'من الأغلى إلى الأرخص',
      },
      {
        'value': 'rating',
        'label': 'التقييم',
        'icon': Icons.star_rounded,
        'description': 'الأعلى تقييماً أولاً',
      },
      {
        'value': 'popularity',
        'label': 'الأكثر شعبية',
        'icon': Icons.trending_up_rounded,
        'description': 'الأكثر حجزاً ومشاهدة',
      },
      {
        'value': 'distance',
        'label': 'المسافة',
        'icon': Icons.near_me_rounded,
        'description': 'الأقرب إليك',
      },
      {
        'value': 'newest',
        'label': 'الأحدث',
        'icon': Icons.new_releases_rounded,
        'description': 'المضاف حديثاً',
      },
    ];

    return Column(
      mainAxisSize: MainAxisSize.min,
      children: sortOptions.map((option) {
        final isSelected = currentSort == option['value'];
        
        return InkWell(
          onTap: () {
            onSortChanged(option['value'] as String);
            Navigator.pop(context);
          },
          child: Container(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            decoration: BoxDecoration(
              color: isSelected ? AppColors.primary.withValues(alpha: 0.05) : null,
              border: isSelected
                  ? const Border(
                      right: BorderSide(
                        color: AppColors.primary,
                        width: 3,
                      ),
                    )
                  : null,
            ),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                  decoration: BoxDecoration(
                    color: isSelected
                        ? AppColors.primary.withValues(alpha: 0.1)
                        : AppColors.background,
                    borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                  ),
                  child: Icon(
                    option['icon'] as IconData,
                    color: isSelected ? AppColors.primary : AppColors.textSecondary,
                    size: AppDimensions.iconMedium,
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        option['label'] as String,
                        style: AppTextStyles.bodyMedium.copyWith(
                          fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                          color: isSelected ? AppColors.primary : AppColors.textPrimary,
                        ),
                      ),
                      const SizedBox(height: AppDimensions.spacingXs),
                      Text(
                        option['description'] as String,
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ),
                if (isSelected)
                  const Icon(
                    Icons.check_circle_rounded,
                    color: AppColors.primary,
                    size: AppDimensions.iconMedium,
                  ),
              ],
            ),
          ),
        );
      }).toList(),
    );
  }

  String _getCurrentSortLabel() {
    switch (currentSort) {
      case 'recommended':
        return 'موصى به';
      case 'price_asc':
        return 'السعر ↑';
      case 'price_desc':
        return 'السعر ↓';
      case 'rating':
        return 'التقييم';
      case 'popularity':
        return 'الشعبية';
      case 'distance':
        return 'المسافة';
      case 'newest':
        return 'الأحدث';
      default:
        return 'ترتيب';
    }
  }
}