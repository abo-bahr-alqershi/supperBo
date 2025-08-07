import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/amenity.dart';

class AmenitiesGridWidget extends StatelessWidget {
  final List<Amenity> amenities;

  const AmenitiesGridWidget({
    super.key,
    required this.amenities,
  });

  @override
  Widget build(BuildContext context) {
    if (amenities.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.info_outline,
              size: 48,
              color: AppColors.textSecondary.withOpacity(0.5),
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              'لا توجد مرافق مسجلة',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      );
    }

    // Group amenities by category
    final groupedAmenities = <String, List<Amenity>>{};
    for (final amenity in amenities) {
      final category = amenity.category;
      if (!groupedAmenities.containsKey(category)) {
        groupedAmenities[category] = [];
      }
      groupedAmenities[category]!.add(amenity);
    }

    return ListView.builder(
      padding: EdgeInsets.zero,
      itemCount: groupedAmenities.length,
      itemBuilder: (context, index) {
        final category = groupedAmenities.keys.elementAt(index);
        final categoryAmenities = groupedAmenities[category]!;

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              color: AppColors.background,
              child: Row(
                children: [
                  Icon(
                    _getCategoryIcon(category),
                    size: 20,
                    color: AppColors.primary,
                  ),
                  const SizedBox(width: AppDimensions.spacingSm),
                  Text(
                    category,
                    style: AppTextStyles.subtitle2.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
            GridView.builder(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 3,
                childAspectRatio: 1,
                crossAxisSpacing: AppDimensions.spacingMd,
                mainAxisSpacing: AppDimensions.spacingMd,
              ),
              itemCount: categoryAmenities.length,
              itemBuilder: (context, index) {
                final amenity = categoryAmenities[index];
                return _buildAmenityItem(amenity);
              },
            ),
          ],
        );
      },
    );
  }

  Widget _buildAmenityItem(Amenity amenity) {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: amenity.isActive ? AppColors.primary.withOpacity(0.3) : AppColors.border,
        ),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingSmall),
            decoration: BoxDecoration(
              color: amenity.isActive
                  ? AppColors.primary.withOpacity(0.1)
                  : AppColors.gray200,
              shape: BoxShape.circle,
            ),
            child: Icon(
              _getAmenityIcon(amenity.name),
              size: 24,
              color: amenity.isActive
                  ? AppColors.primary
                  : AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            amenity.name,
            style: AppTextStyles.caption.copyWith(
              color: amenity.isActive
                  ? AppColors.textPrimary
                  : AppColors.textSecondary,
              fontWeight: amenity.isActive ? FontWeight.w500 : FontWeight.normal,
            ),
            textAlign: TextAlign.center,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
          // Removed isPaid block as Amenity has no isPaid field
        ],
      ),
    );
  }

  IconData _getCategoryIcon(String category) {
    switch (category.toLowerCase()) {
      case 'أساسيات':
      case 'basics':
        return Icons.check_circle_outline;
      case 'مرافق':
      case 'facilities':
        return Icons.business;
      case 'خدمات':
      case 'services':
        return Icons.room_service_outlined;
      case 'ترفيه':
      case 'entertainment':
        return Icons.sports_esports_outlined;
      case 'أمان':
      case 'security':
        return Icons.security_outlined;
      default:
        return Icons.category_outlined;
    }
  }

  IconData _getAmenityIcon(String amenity) {
    final name = amenity.toLowerCase();
    if (name.contains('wifi') || name.contains('واي فاي')) return Icons.wifi;
    if (name.contains('parking') || name.contains('موقف')) return Icons.local_parking;
    if (name.contains('pool') || name.contains('مسبح')) return Icons.pool;
    if (name.contains('gym') || name.contains('جيم')) return Icons.fitness_center;
    if (name.contains('kitchen') || name.contains('مطبخ')) return Icons.kitchen;
    if (name.contains('ac') || name.contains('تكييف')) return Icons.ac_unit;
    if (name.contains('tv') || name.contains('تلفاز')) return Icons.tv;
    if (name.contains('elevator') || name.contains('مصعد')) return Icons.elevator;
    if (name.contains('laundry') || name.contains('غسيل')) return Icons.local_laundry_service;
    if (name.contains('breakfast') || name.contains('فطور')) return Icons.free_breakfast;
    if (name.contains('pet') || name.contains('حيوان')) return Icons.pets;
    if (name.contains('smoking') || name.contains('تدخين')) return Icons.smoking_rooms;
    if (name.contains('wheelchair') || name.contains('كرسي')) return Icons.accessible;
    if (name.contains('security') || name.contains('أمن')) return Icons.security;
    if (name.contains('camera') || name.contains('كاميرا')) return Icons.videocam;
    return Icons.check_circle_outline;
  }
}