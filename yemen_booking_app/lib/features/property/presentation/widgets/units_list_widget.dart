import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/unit.dart';

class UnitsListWidget extends StatelessWidget {
  final List<Unit> units;
  final Function(Unit) onUnitSelect;

  const UnitsListWidget({
    super.key,
    required this.units,
    required this.onUnitSelect,
  });

  @override
  Widget build(BuildContext context) {
    if (units.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.meeting_room_outlined,
              size: 48,
              color: AppColors.textSecondary.withOpacity(0.5),
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              'لا توجد وحدات متاحة',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      itemCount: units.length,
      separatorBuilder: (context, index) => const SizedBox(
        height: AppDimensions.spacingMd,
      ),
      itemBuilder: (context, index) {
        final unit = units[index];
        return _UnitCard(
          unit: unit,
          onTap: () => onUnitSelect(unit),
        );
      },
    );
  }
}

class _UnitCard extends StatelessWidget {
  final Unit unit;
  final VoidCallback onTap;

  const _UnitCard({
    required this.unit,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
      ),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (unit.images.isNotEmpty)
              Stack(
                children: [
                  ClipRRect(
                    borderRadius: const BorderRadius.vertical(
                      top: Radius.circular(AppDimensions.borderRadiusLg),
                    ),
                    child: CachedImageWidget(
                      imageUrl: unit.images.first.url,
                      height: 160,
                      width: double.infinity,
                      fit: BoxFit.cover,
                    ),
                  ),
                  Positioned(
                    top: AppDimensions.paddingSmall,
                    right: AppDimensions.paddingSmall,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: AppDimensions.paddingSmall,
                        vertical: AppDimensions.paddingXSmall,
                      ),
                      decoration: BoxDecoration(
                        color: unit.isAvailable
                            ? AppColors.success
                            : AppColors.error,
                        borderRadius: BorderRadius.circular(
                          AppDimensions.borderRadiusXs,
                        ),
                      ),
                      child: Text(
                        unit.isAvailable ? 'متاح' : 'محجوز',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.white,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ),
                  if (unit.images.length > 1)
                    Positioned(
                      bottom: AppDimensions.paddingSmall,
                      left: AppDimensions.paddingSmall,
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppDimensions.paddingSmall,
                          vertical: AppDimensions.paddingXSmall,
                        ),
                        decoration: BoxDecoration(
                          color: AppColors.black.withOpacity(0.6),
                          borderRadius: BorderRadius.circular(
                            AppDimensions.borderRadiusXs,
                          ),
                        ),
                        child: Row(
                          children: [
                            const Icon(
                              Icons.photo_library_outlined,
                              size: 14,
                              color: AppColors.white,
                            ),
                            const SizedBox(width: AppDimensions.spacingXs),
                            Text(
                              '${unit.images.length}',
                              style: AppTextStyles.caption.copyWith(
                                color: AppColors.white,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                ],
              ),
            Padding(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          unit.name,
                          style: AppTextStyles.subtitle1.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppDimensions.paddingSmall,
                          vertical: AppDimensions.paddingXSmall,
                        ),
                        decoration: BoxDecoration(
                          color: AppColors.primary.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(
                            AppDimensions.borderRadiusXs,
                          ),
                        ),
                        child: Text(
                          unit.unitTypeName,
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.primary,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ],
                  ),
                  if (unit.customFeatures.isNotEmpty) ...[
                    const SizedBox(height: AppDimensions.spacingSm),
                    Text(
                      unit.customFeatures,
                      style: AppTextStyles.bodySmall.copyWith(
                        color: AppColors.textSecondary,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                  const SizedBox(height: AppDimensions.spacingMd),
                  _buildUnitFeatures(),
                  const SizedBox(height: AppDimensions.spacingMd),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'السعر',
                            style: AppTextStyles.caption.copyWith(
                              color: AppColors.textSecondary,
                            ),
                          ),
                          Row(
                            children: [
                              Text(
                                unit.basePrice.amount.toStringAsFixed(0),
                                style: AppTextStyles.priceSmall.copyWith(
                                  color: AppColors.primary,
                                ),
                              ),
                              const SizedBox(width: AppDimensions.spacingXs),
                              Text(
                                '${unit.basePrice.currency} / ${_getPricingPeriod()}',
                                style: AppTextStyles.caption.copyWith(
                                  color: AppColors.textSecondary,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                      Icon(
                        Icons.arrow_forward_ios,
                        size: 16,
                        color: AppColors.textSecondary,
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildUnitFeatures() {
    final features = <Widget>[];
    
    // Add dynamic fields as features
    for (final group in unit.dynamicFields) {
      for (final field in group.fieldValues.take(3)) {
        features.add(_buildFeatureChip(
          _getFieldIcon(field.fieldName),
          '${field.displayName}: ${field.value}',
        ));
      }
    }

    if (features.isEmpty) return const SizedBox.shrink();

    return Wrap(
      spacing: AppDimensions.spacingSm,
      runSpacing: AppDimensions.spacingSm,
      children: features,
    );
  }

  Widget _buildFeatureChip(IconData icon, String label) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
        vertical: AppDimensions.paddingXSmall,
      ),
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: AppColors.textSecondary),
          const SizedBox(width: AppDimensions.spacingXs),
          Text(
            label,
            style: AppTextStyles.caption,
          ),
        ],
      ),
    );
  }

  IconData _getFieldIcon(String fieldName) {
    final name = fieldName.toLowerCase();
    if (name.contains('bed') || name.contains('سرير')) return Icons.bed;
    if (name.contains('bath') || name.contains('حمام')) return Icons.bathroom;
    if (name.contains('area') || name.contains('مساحة')) return Icons.square_foot;
    if (name.contains('floor') || name.contains('طابق')) return Icons.stairs;
    if (name.contains('view') || name.contains('إطلالة')) return Icons.landscape;
    if (name.contains('guest') || name.contains('ضيف')) return Icons.people;
    return Icons.info_outline;
  }

  String _getPricingPeriod() {
    switch (unit.pricingMethod) {
      case PricingMethod.hourly:
        return 'ساعة';
      case PricingMethod.daily:
        return 'ليلة';
      case PricingMethod.weekly:
        return 'أسبوع';
      case PricingMethod.monthly:
        return 'شهر';
    }
  }
}