import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class FilterChipsWidget extends StatelessWidget {
  final Map<String, dynamic> filters;
  final Function(String) onRemoveFilter;
  final VoidCallback? onClearAll;

  const FilterChipsWidget({
    super.key,
    required this.filters,
    required this.onRemoveFilter,
    this.onClearAll,
  });

  @override
  Widget build(BuildContext context) {
    if (filters.isEmpty) {
      return const SizedBox.shrink();
    }

    return Container(
      height: 50,
      padding: const EdgeInsets.symmetric(
        vertical: AppDimensions.paddingSmall,
      ),
      child: ListView(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
        ),
        children: [
          if (filters.length > 1 && onClearAll != null) ...[
            ActionChip(
              label: Text(
                'مسح الكل',
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.error,
                ),
              ),
              onPressed: onClearAll,
              backgroundColor: AppColors.error.withValues(alpha: 0.1),
              side: BorderSide(
                color: AppColors.error.withValues(alpha: 0.3),
              ),
              avatar: const Icon(
                Icons.clear_all_rounded,
                size: AppDimensions.iconSmall,
                color: AppColors.error,
              ),
            ),
            const SizedBox(width: AppDimensions.spacingSm),
          ],
          ...filters.entries.map((entry) {
            return Padding(
              padding: const EdgeInsets.only(left: AppDimensions.spacingSm),
              child: _buildFilterChip(
                entry.key,
                entry.value,
                () => onRemoveFilter(entry.key),
              ),
            );
          }),
        ],
      ),
    );
  }

  Widget _buildFilterChip(String key, dynamic value, VoidCallback onDelete) {
    final String label = _getFilterLabel(key, value);
    final IconData icon = _getFilterIcon(key);
    final Color color = _getFilterColor(key);

    return Chip(
      label: Text(
        label,
        style: AppTextStyles.caption.copyWith(
          color: color,
        ),
      ),
      avatar: Icon(
        icon,
        size: AppDimensions.iconSmall,
        color: color,
      ),
      deleteIcon: Icon(
        Icons.close_rounded,
        size: AppDimensions.iconSmall,
        color: color,
      ),
      onDeleted: onDelete,
      backgroundColor: color.withValues(alpha: 0.1),
      side: BorderSide(
        color: color.withValues(alpha: 0.3),
      ),
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
      ),
      visualDensity: VisualDensity.compact,
    );
  }

  String _getFilterLabel(String key, dynamic value) {
    switch (key) {
      case 'searchTerm':
        return value.toString();
      case 'city':
        return value.toString();
      case 'propertyTypeId':
        return _getPropertyTypeName(value);
      case 'minPrice':
        return 'من ${value.toString()} ريال';
      case 'maxPrice':
        return 'إلى ${value.toString()} ريال';
      case 'minStarRating':
        return '$value نجوم فما فوق';
      case 'checkIn':
        return 'دخول: ${_formatDate(value as DateTime)}';
      case 'checkOut':
        return 'خروج: ${_formatDate(value as DateTime)}';
      case 'guestsCount':
        return '$value ضيوف';
      case 'requiredAmenities':
        final List<String> amenities = value as List<String>;
        return '${amenities.length} مرافق';
      case 'serviceIds':
        final List<String> services = value as List<String>;
        return '${services.length} خدمات';
      case 'sortBy':
        return _getSortByLabel(value);
      default:
        return value.toString();
    }
  }

  IconData _getFilterIcon(String key) {
    switch (key) {
      case 'searchTerm':
        return Icons.search_rounded;
      case 'city':
        return Icons.location_on_rounded;
      case 'propertyTypeId':
        return Icons.home_rounded;
      case 'minPrice':
      case 'maxPrice':
        return Icons.attach_money_rounded;
      case 'minStarRating':
        return Icons.star_rounded;
      case 'checkIn':
      case 'checkOut':
        return Icons.calendar_today_rounded;
      case 'guestsCount':
        return Icons.people_rounded;
      case 'requiredAmenities':
        return Icons.apps_rounded;
      case 'serviceIds':
        return Icons.room_service_rounded;
      case 'sortBy':
        return Icons.sort_rounded;
      default:
        return Icons.filter_alt_rounded;
    }
  }

  Color _getFilterColor(String key) {
    switch (key) {
      case 'searchTerm':
        return AppColors.primary;
      case 'city':
        return AppColors.info;
      case 'propertyTypeId':
        return AppColors.secondary;
      case 'minPrice':
      case 'maxPrice':
        return AppColors.success;
      case 'minStarRating':
        return AppColors.ratingStar;
      case 'checkIn':
      case 'checkOut':
        return AppColors.accent;
      case 'guestsCount':
        return AppColors.primary;
      default:
        return AppColors.textSecondary;
    }
  }

  String _getPropertyTypeName(String typeId) {
    switch (typeId) {
      case '1':
        return 'فندق';
      case '2':
        return 'شقة';
      case '3':
        return 'فيلا';
      case '4':
        return 'منتجع';
      default:
        return 'نوع العقار';
    }
  }

  String _getSortByLabel(String sortBy) {
    switch (sortBy) {
      case 'price_asc':
        return 'السعر: الأقل أولاً';
      case 'price_desc':
        return 'السعر: الأعلى أولاً';
      case 'rating':
        return 'التقييم';
      case 'popularity':
        return 'الأكثر شعبية';
      case 'newest':
        return 'الأحدث';
      default:
        return 'ترتيب';
    }
  }

  String _formatDate(DateTime date) {
    return '${date.day}/${date.month}';
  }
}