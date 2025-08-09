// lib/features/home/presentation/widgets/popular_destinations_widget.dart

import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/city_destination.dart';

class PopularDestinationsWidget extends StatefulWidget {
  final List<CityDestination> destinations;
  final Function(CityDestination) onDestinationTap;

  const PopularDestinationsWidget({
    super.key,
    required this.destinations,
    required this.onDestinationTap,
  });

  @override
  State<PopularDestinationsWidget> createState() => 
      _PopularDestinationsWidgetState();
}

class _PopularDestinationsWidgetState extends State<PopularDestinationsWidget> 
    with TickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 600),
      vsync: this,
    );
    
    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeIn,
    ));
    
    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: _fadeAnimation,
      child: Container(
        height: 200,
        padding: const EdgeInsets.only(
          right: AppDimensions.paddingMedium,
        ),
        child: ListView.builder(
          scrollDirection: Axis.horizontal,
          physics: const BouncingScrollPhysics(),
          itemCount: widget.destinations.length,
          itemBuilder: (context, index) {
            final destination = widget.destinations[index];
            return _buildDestinationCard(destination, index);
          },
        ),
      ),
    );
  }

  Widget _buildDestinationCard(CityDestination destination, int index) {
    return GestureDetector(
      onTap: () => widget.onDestinationTap(destination),
      child: TweenAnimationBuilder<double>(
        tween: Tween(begin: 0.0, end: 1.0),
        duration: Duration(milliseconds: 400 + (index * 100)),
        curve: Curves.easeOutBack,
        builder: (context, value, child) {
          return Transform.scale(
            scale: value,
            child: Container(
              width: 160,
              margin: const EdgeInsets.only(left: AppDimensions.spacingMd),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
                boxShadow: [
                  BoxShadow(
                    color: AppColors.shadow.withValues(alpha: 0.1),
                    blurRadius: 10,
                    offset: const Offset(0, 5),
                  ),
                ],
              ),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
                child: Stack(
                  children: [
                    // Destination Image
                    Positioned.fill(
                      child: CachedImageWidget(
                        imageUrl: destination.imageUrl,
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
                              Colors.black.withValues(alpha: 0.7),
                            ],
                            stops: const [0.4, 1.0],
                          ),
                        ),
                      ),
                    ),
                    
                    // Weather Badge (if available)
                    if (destination.hasWeatherData)
                      Positioned(
                        top: AppDimensions.paddingSmall,
                        right: AppDimensions.paddingSmall,
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 8,
                            vertical: 4,
                          ),
                          decoration: BoxDecoration(
                            color: Colors.white.withValues(alpha: 0.9),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text(
                                destination.weatherIcon ?? '☀️',
                                style: const TextStyle(fontSize: 14),
                              ),
                              const SizedBox(width: 4),
                              Text(
                                destination.formattedTemperature,
                                style: AppTextStyles.caption.copyWith(
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    
                    // Popular Badge
                    if (destination.isPopular)
                      Positioned(
                        top: AppDimensions.paddingSmall,
                        left: AppDimensions.paddingSmall,
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 8,
                            vertical: 4,
                          ),
                          decoration: BoxDecoration(
                            color: AppColors.error,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Text(
                            'شائع',
                            style: AppTextStyles.caption.copyWith(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                              fontSize: 10,
                            ),
                          ),
                        ),
                      ),
                    
                    // Destination Info
                    Positioned(
                      bottom: 0,
                      left: 0,
                      right: 0,
                      child: Container(
                        padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              destination.name,
                              style: AppTextStyles.bodyLarge.copyWith(
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                              ),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            ),
                            const SizedBox(height: 2),
                            Row(
                              children: [
                                Icon(
                                  Icons.home_work_outlined,
                                  color: Colors.white.withValues(alpha: 0.8),
                                  size: 14,
                                ),
                                const SizedBox(width: 4),
                                Text(
                                  '${destination.propertyCount} عقار',
                                  style: AppTextStyles.caption.copyWith(
                                    color: Colors.white.withValues(alpha: 0.8),
                                  ),
                                ),
                              ],
                            ),
                            if (destination.averageRating > 0)
                              Row(
                                children: [
                                  const Icon(
                                    Icons.star_rounded,
                                    color: AppColors.ratingStar,
                                    size: 14,
                                  ),
                                  const SizedBox(width: 2),
                                  Text(
                                    destination.formattedRating,
                                    style: AppTextStyles.caption.copyWith(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
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
        },
      ),
    );
  }
}