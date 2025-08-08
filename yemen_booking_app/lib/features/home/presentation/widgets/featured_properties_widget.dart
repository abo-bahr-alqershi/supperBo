// lib/features/home/presentation/widgets/featured_properties_widget.dart

import 'package:flutter/material.dart';
import 'package:carousel_slider/carousel_slider.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/featured_property.dart';

class FeaturedPropertiesWidget extends StatefulWidget {
  final List<FeaturedProperty> properties;
  final Function(FeaturedProperty) onPropertyTap;

  const FeaturedPropertiesWidget({
    super.key,
    required this.properties,
    required this.onPropertyTap,
  });

  @override
  State<FeaturedPropertiesWidget> createState() => 
      _FeaturedPropertiesWidgetState();
}

class _FeaturedPropertiesWidgetState extends State<FeaturedPropertiesWidget> 
    with TickerProviderStateMixin {
  int _currentIndex = 0;
  final CarouselController _carouselController = CarouselController();
  late AnimationController _shimmerController;

  @override
  void initState() {
    super.initState();
    _shimmerController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat();
  }

  @override
  void dispose() {
    _shimmerController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Carousel
        CarouselSlider.builder(
          carouselController: _carouselController,
          itemCount: widget.properties.length,
          options: CarouselOptions(
            height: 280,
            viewportFraction: 0.85,
            enlargeCenterPage: true,
            autoPlay: true,
            autoPlayInterval: const Duration(seconds: 5),
            autoPlayAnimationDuration: const Duration(milliseconds: 800),
            autoPlayCurve: Curves.fastOutSlowIn,
            onPageChanged: (index, reason) {
              setState(() {
                _currentIndex = index;
              });
            },
          ),
          itemBuilder: (context, index, realIndex) {
            final property = widget.properties[index];
            return _buildPropertyCard(property, index);
          },
        ),
        
        // Indicators
        const SizedBox(height: AppDimensions.spacingMd),
        _buildIndicators(),
      ],
    );
  }

  Widget _buildPropertyCard(FeaturedProperty property, int index) {
    return GestureDetector(
      onTap: () => widget.onPropertyTap(property),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 300),
        margin: const EdgeInsets.symmetric(
          horizontal: AppDimensions.spacingSm,
          vertical: AppDimensions.spacingSm,
        ),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow.withOpacity(0.15),
              blurRadius: 20,
              offset: const Offset(0, 10),
            ),
          ],
        ),
        child: ClipRRect(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          child: Stack(
            children: [
              // Property Image
              Positioned.fill(
                child: CachedImageWidget(
                  imageUrl: property.displayImage,
                  fit: BoxFit.cover,
                ),
              ),
              
              // Gradient Overlay
              Positioned.fill(
                child: Container(
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      colors: [
                        Colors.transparent,
                        Colors.black.withOpacity(0.7),
                      ],
                      stops: const [0.5, 1.0],
                    ),
                  ),
                ),
              ),
              
              // Property Badge
              if (property.badgeText != null)
                Positioned(
                  top: AppDimensions.paddingMedium,
                  left: AppDimensions.paddingMedium,
                  child: Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppDimensions.paddingSmall,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: _getBadgeColor(property.badgeColor),
                      borderRadius: BorderRadius.circular(
                        AppDimensions.borderRadiusSm,
                      ),
                    ),
                    child: Text(
                      property.badgeText!,
                      style: AppTextStyles.caption.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ),
              
              // Property Info
              Positioned(
                bottom: 0,
                left: 0,
                right: 0,
                child: Container(
                  padding: const EdgeInsets.all(AppDimensions.paddingMedium),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Property Name
                      Text(
                        property.name,
                        style: AppTextStyles.heading3.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      
                      const SizedBox(height: 4),
                      
                      // Location
                      Row(
                        children: [
                          const Icon(
                            Icons.location_on_outlined,
                            color: Colors.white70,
                            size: 16,
                          ),
                          const SizedBox(width: 4),
                          Expanded(
                            child: Text(
                              property.fullAddress,
                              style: AppTextStyles.bodyMedium.copyWith(
                                color: Colors.white70,
                              ),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ],
                      ),
                      
                      const SizedBox(height: 8),
                      
                      // Price and Rating
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          // Price
                          if (property.basePrice != null)
                            Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                if (property.hasDiscount)
                                  Text(
                                    property.formattedOriginalPrice,
                                    style: AppTextStyles.caption.copyWith(
                                      color: Colors.white54,
                                      decoration: TextDecoration.lineThrough,
                                    ),
                                  ),
                                Row(
                                  crossAxisAlignment: CrossAxisAlignment.end,
                                  children: [
                                    Text(
                                      property.formattedPrice,
                                      style: AppTextStyles.price.copyWith(
                                        color: Colors.white,
                                        fontSize: 20,
                                      ),
                                    ),
                                    const SizedBox(width: 4),
                                    Text(
                                      '/ ليلة',
                                      style: AppTextStyles.caption.copyWith(
                                        color: Colors.white70,
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          
                          // Rating
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 8,
                              vertical: 4,
                            ),
                            decoration: BoxDecoration(
                              color: Colors.white.withOpacity(0.2),
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: Row(
                              children: [
                                const Icon(
                                  Icons.star_rounded,
                                  color: AppColors.ratingStar,
                                  size: 16,
                                ),
                                const SizedBox(width: 4),
                                Text(
                                  property.formattedRating,
                                  style: AppTextStyles.bodySmall.copyWith(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ],
                            ),
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
      ),
    );
  }

  Widget _buildIndicators() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(
        widget.properties.length,
        (index) => AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: _currentIndex == index ? 24 : 8,
          height: 8,
          decoration: BoxDecoration(
            color: _currentIndex == index
                ? AppColors.primary
                : AppColors.gray200,
            borderRadius: BorderRadius.circular(4),
          ),
        ),
      ),
    );
  }

  Color _getBadgeColor(String? color) {
    switch (color?.toLowerCase()) {
      case 'red':
        return AppColors.error;
      case 'green':
        return AppColors.success;
      case 'blue':
        return AppColors.info;
      case 'orange':
        return AppColors.warning;
      default:
        return AppColors.primary;
    }
  }
}