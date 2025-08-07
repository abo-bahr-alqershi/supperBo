import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../../../core/widgets/rating_widget.dart';
import '../../../../core/widgets/price_widget.dart';
import '../../domain/entities/search_result.dart';

enum CardDisplayType { list, grid, compact }

class SearchResultCardWidget extends StatelessWidget {
  final SearchResult result;
  final VoidCallback? onTap;
  final VoidCallback? onFavoriteToggle;
  final CardDisplayType displayType;
  final bool showDistance;

  const SearchResultCardWidget({
    super.key,
    required this.result,
    this.onTap,
    this.onFavoriteToggle,
    this.displayType = CardDisplayType.list,
    this.showDistance = true,
  });

  @override
  Widget build(BuildContext context) {
    switch (displayType) {
      case CardDisplayType.list:
        return _buildListCard(context);
      case CardDisplayType.grid:
        return _buildGridCard(context);
      case CardDisplayType.compact:
        return _buildCompactCard(context);
    }
  }

  Widget _buildListCard(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
      child: Container(
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow,
              blurRadius: AppDimensions.blurMedium,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildImageSection(height: 200),
            Padding(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildHeader(),
                  const SizedBox(height: AppDimensions.spacingSm),
                  _buildLocation(),
                  const SizedBox(height: AppDimensions.spacingSm),
                  _buildRatingAndReviews(),
                  const SizedBox(height: AppDimensions.spacingMd),
                  _buildAmenities(),
                  const SizedBox(height: AppDimensions.spacingMd),
                  _buildFooter(),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildGridCard(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow,
              blurRadius: AppDimensions.blurSmall,
              offset: const Offset(0, 1),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              flex: 3,
              child: _buildImageSection(),
            ),
            Expanded(
              flex: 2,
              child: Padding(
                padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      result.name,
                      style: AppTextStyles.bodyMedium.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    Row(
                      children: [
                        Icon(
                          Icons.location_on_outlined,
                          size: AppDimensions.iconXSmall,
                          color: AppColors.textSecondary,
                        ),
                        const SizedBox(width: 2),
                        Expanded(
                          child: Text(
                            result.city,
                            style: AppTextStyles.caption.copyWith(
                              color: AppColors.textSecondary,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                      ],
                    ),
                    RatingWidget(
                      rating: result.averageRating,
                      starSize: 14,
                      showLabel: false,
                      showReviewCount: false,
                    ),
                    PriceWidget(
                      price: result.discountedPrice,
                      originalPrice: result.minPrice != result.discountedPrice 
                          ? result.minPrice 
                          : null,
                      currency: result.currency,
                      displayType: PriceDisplayType.compact,
                      period: 'الليلة',
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCompactCard(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        height: 120,
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(color: AppColors.outline),
        ),
        child: Row(
          children: [
            Container(
              width: 120,
              decoration: BoxDecoration(
                borderRadius: const BorderRadius.horizontal(
                  right: Radius.circular(AppDimensions.borderRadiusMd),
                ),
                image: DecorationImage(
                  image: NetworkImage(result.mainImageUrl ?? ''),
                  fit: BoxFit.cover,
                ),
              ),
            ),
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      result.name,
                      style: AppTextStyles.bodyMedium.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    Row(
                      children: [
                        Icon(
                          Icons.star_rounded,
                          size: AppDimensions.iconSmall,
                          color: AppColors.ratingStar,
                        ),
                        const SizedBox(width: AppDimensions.spacingXs),
                        Text(
                          result.averageRating.toStringAsFixed(1),
                          style: AppTextStyles.caption,
                        ),
                        const Spacer(),
                        Text(
                          '${result.discountedPrice.toStringAsFixed(0)} ${result.currency}',
                          style: AppTextStyles.priceSmall,
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildImageSection({double? height}) {
    return Stack(
      children: [
        CachedImageWidget(
          imageUrl: result.mainImageUrl ?? '',
          height: height,
          width: double.infinity,
          fit: BoxFit.cover,
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(
              displayType == CardDisplayType.grid
                  ? AppDimensions.borderRadiusMd
                  : AppDimensions.borderRadiusLg,
            ),
          ),
        ),
        Positioned(
          top: AppDimensions.paddingSmall,
          right: AppDimensions.paddingSmall,
          child: Row(
            children: [
              if (result.isFeatured)
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingSmall,
                    vertical: AppDimensions.paddingXSmall,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.accent,
                    borderRadius: BorderRadius.circular(
                      AppDimensions.borderRadiusXs,
                    ),
                  ),
                  child: Text(
                    'مميز',
                    style: AppTextStyles.overline.copyWith(
                      color: AppColors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              if (result.isRecommended) ...[
                const SizedBox(width: AppDimensions.spacingXs),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingSmall,
                    vertical: AppDimensions.paddingXSmall,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.success,
                    borderRadius: BorderRadius.circular(
                      AppDimensions.borderRadiusXs,
                    ),
                  ),
                  child: Text(
                    'موصى به',
                    style: AppTextStyles.overline.copyWith(
                      color: AppColors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ],
          ),
        ),
        Positioned(
          top: AppDimensions.paddingSmall,
          left: AppDimensions.paddingSmall,
          child: _buildFavoriteButton(),
        ),
        if (result.minPrice != result.discountedPrice)
          Positioned(
            bottom: AppDimensions.paddingSmall,
            right: AppDimensions.paddingSmall,
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingSmall,
                vertical: AppDimensions.paddingXSmall,
              ),
              decoration: BoxDecoration(
                color: AppColors.error,
                borderRadius: BorderRadius.circular(
                  AppDimensions.borderRadiusXs,
                ),
              ),
              child: Text(
                '${((1 - result.discountedPrice / result.minPrice) * 100).toStringAsFixed(0)}% خصم',
                style: AppTextStyles.overline.copyWith(
                  color: AppColors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildFavoriteButton() {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.white.withOpacity(0.9),
        shape: BoxShape.circle,
      ),
      child: IconButton(
        onPressed: onFavoriteToggle,
        icon: Icon(
          Icons.favorite_border_rounded,
          color: AppColors.error,
          size: AppDimensions.iconMedium,
        ),
        padding: const EdgeInsets.all(AppDimensions.paddingSmall),
        constraints: const BoxConstraints(),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                result.propertyType,
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.primary,
                  fontWeight: FontWeight.w500,
                ),
              ),
              const SizedBox(height: AppDimensions.spacingXs),
              Text(
                result.name,
                style: AppTextStyles.subtitle1.copyWith(
                  fontWeight: FontWeight.bold,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ),
        ),
        if (result.starRating > 0)
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingSmall),
            decoration: BoxDecoration(
              color: AppColors.primary.withOpacity(0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
            ),
            child: Row(
              children: List.generate(
                result.starRating,
                (index) => Icon(
                  Icons.star_rounded,
                  size: AppDimensions.iconSmall,
                  color: AppColors.primary,
                ),
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildLocation() {
    return Row(
      children: [
        Icon(
          Icons.location_on_outlined,
          size: AppDimensions.iconSmall,
          color: AppColors.textSecondary,
        ),
        const SizedBox(width: AppDimensions.spacingXs),
        Expanded(
          child: Text(
            '${result.address}, ${result.city}',
            style: AppTextStyles.bodySmall.copyWith(
              color: AppColors.textSecondary,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ),
        if (showDistance && result.distanceKm != null) ...[
          const SizedBox(width: AppDimensions.spacingSm),
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingSmall,
              vertical: AppDimensions.paddingXSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.info.withOpacity(0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
            ),
            child: Text(
              '${result.distanceKm!.toStringAsFixed(1)} كم',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.info,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ],
    );
  }

  Widget _buildRatingAndReviews() {
    return Row(
      children: [
        RatingWidget(
          rating: result.averageRating,
          reviewCount: result.reviewsCount,
          starSize: 18,
        ),
        const Spacer(),
        if (result.isAvailable)
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingSmall,
              vertical: AppDimensions.paddingXSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.success.withOpacity(0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
            ),
            child: Text(
              'متاح',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.success,
                fontWeight: FontWeight.bold,
              ),
            ),
          )
        else
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingSmall,
              vertical: AppDimensions.paddingXSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.error.withOpacity(0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
            ),
            child: Text(
              'محجوز',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.error,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildAmenities() {
    if (result.mainAmenities.isEmpty) {
      return const SizedBox.shrink();
    }

    return Wrap(
      spacing: AppDimensions.spacingSm,
      runSpacing: AppDimensions.spacingXs,
      children: result.mainAmenities.take(4).map((amenity) {
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
              Icon(
                _getAmenityIcon(amenity),
                size: AppDimensions.iconXSmall,
                color: AppColors.textSecondary,
              ),
              const SizedBox(width: AppDimensions.spacingXs),
              Text(
                amenity,
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          ),
        );
      }).toList(),
    );
  }

  Widget _buildFooter() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (result.minPrice != result.discountedPrice)
              Text(
                '${result.minPrice.toStringAsFixed(0)} ${result.currency}',
                style: AppTextStyles.bodySmall.copyWith(
                  decoration: TextDecoration.lineThrough,
                  color: AppColors.textSecondary,
                ),
              ),
            Row(
              crossAxisAlignment: CrossAxisAlignment.baseline,
              textBaseline: TextBaseline.alphabetic,
              children: [
                Text(
                  result.discountedPrice.toStringAsFixed(0),
                  style: AppTextStyles.price.copyWith(
                    color: result.minPrice != result.discountedPrice
                        ? AppColors.success
                        : AppColors.primary,
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingXs),
                Text(
                  result.currency,
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingXs),
                Text(
                  '/ الليلة',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          ],
        ),
        ElevatedButton(
          onPressed: onTap,
          style: ElevatedButton.styleFrom(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingLarge,
              vertical: AppDimensions.paddingSmall,
            ),
          ),
          child: const Text('عرض التفاصيل'),
        ),
      ],
    );
  }

  IconData _getAmenityIcon(String amenity) {
    switch (amenity.toLowerCase()) {
      case 'واي فاي':
      case 'wifi':
        return Icons.wifi_rounded;
      case 'موقف سيارات':
      case 'parking':
        return Icons.local_parking_rounded;
      case 'مسبح':
      case 'pool':
        return Icons.pool_rounded;
      case 'مطعم':
      case 'restaurant':
        return Icons.restaurant_rounded;
      case 'صالة رياضية':
      case 'gym':
        return Icons.fitness_center_rounded;
      case 'مكيف':
      case 'ac':
        return Icons.ac_unit_rounded;
      default:
        return Icons.check_circle_outline_rounded;
    }
  }
}