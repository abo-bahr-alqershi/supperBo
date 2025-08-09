import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/rating_widget.dart';
import '../../domain/entities/property_detail.dart';

class PropertyHeaderWidget extends StatelessWidget {
  final PropertyDetail property;
  final bool isFavorite;
  final VoidCallback onFavoriteToggle;
  final VoidCallback onShare;

  const PropertyHeaderWidget({
    super.key,
    required this.property,
    required this.isFavorite,
    required this.onFavoriteToggle,
    required this.onShare,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      color: AppColors.surface,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      property.name,
                      style: AppTextStyles.heading2.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: AppDimensions.spacingSm),
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: AppDimensions.paddingSmall,
                            vertical: AppDimensions.paddingXSmall,
                          ),
                          decoration: BoxDecoration(
                            color: AppColors.primary.withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(
                              AppDimensions.borderRadiusXs,
                            ),
                          ),
                          child: Text(
                            property.typeName,
                            style: AppTextStyles.caption.copyWith(
                              color: AppColors.primary,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                        const SizedBox(width: AppDimensions.spacingSm),
                        if (property.starRating > 0) ...[
                          Row(
                            children: List.generate(
                              property.starRating,
                              (index) => const Icon(
                                Icons.star,
                                size: 16,
                                color: AppColors.ratingStar,
                              ),
                            ),
                          ),
                        ],
                      ],
                    ),
                  ],
                ),
              ),
              Row(
                children: [
                  IconButton(
                    onPressed: onShare,
                    icon: const Icon(Icons.share_outlined),
                    color: AppColors.textSecondary,
                  ),
                  IconButton(
                    onPressed: onFavoriteToggle,
                    icon: Icon(
                      isFavorite ? Icons.favorite : Icons.favorite_border,
                      color: isFavorite ? AppColors.error : AppColors.textSecondary,
                    ),
                  ),
                ],
              ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildLocationRow(),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildStatsRow(),
          if (property.averageRating > 0) ...[
            const SizedBox(height: AppDimensions.spacingMd),
            _buildRatingRow(),
          ],
        ],
      ),
    );
  }

  Widget _buildLocationRow() {
    return Row(
      children: [
        const Icon(
          Icons.location_on_outlined,
          size: 20,
          color: AppColors.textSecondary,
        ),
        const SizedBox(width: AppDimensions.spacingSm),
        Expanded(
          child: Text(
            '${property.address}, ${property.city}',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildStatsRow() {
    return Row(
      children: [
        _buildStatItem(
          icon: Icons.visibility_outlined,
          value: property.viewCount.toString(),
          label: 'مشاهدة',
        ),
        const SizedBox(width: AppDimensions.spacingLg),
        _buildStatItem(
          icon: Icons.bookmark_outline,
          value: property.bookingCount.toString(),
          label: 'حجز',
        ),
        const SizedBox(width: AppDimensions.spacingLg),
        _buildStatItem(
          icon: Icons.home_outlined,
          value: property.units.length.toString(),
          label: 'وحدة',
        ),
      ],
    );
  }

  Widget _buildStatItem({
    required IconData icon,
    required String value,
    required String label,
  }) {
    return Row(
      children: [
        Icon(icon, size: 18, color: AppColors.textSecondary),
        const SizedBox(width: AppDimensions.spacingXs),
        Text(
          value,
          style: AppTextStyles.bodyMedium.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(width: AppDimensions.spacingXs),
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }

  Widget _buildRatingRow() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Row(
        children: [
          RatingWidget(
            rating: property.averageRating,
            reviewCount: property.reviewsCount,
            starSize: 20,
          ),
          const Spacer(),
          Text(
            '${property.reviewsCount} تقييم',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }
}