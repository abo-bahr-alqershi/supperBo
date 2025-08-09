import 'package:flutter/material.dart';
import 'package:smooth_page_indicator/smooth_page_indicator.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/property_detail.dart';

class PropertyImagesSliderWidget extends StatefulWidget {
  final List<PropertyImage> images;
  final Function(int)? onImageTap;
  final double height;

  const PropertyImagesSliderWidget({
    super.key,
    required this.images,
    this.onImageTap,
    this.height = 350,
  });

  @override
  State<PropertyImagesSliderWidget> createState() =>
      _PropertyImagesSliderWidgetState();
}

class _PropertyImagesSliderWidgetState
    extends State<PropertyImagesSliderWidget> {
  final PageController _pageController = PageController();
  int _currentIndex = 0;

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.images.isEmpty) {
      return Container(
        height: widget.height,
        color: AppColors.gray200,
        child: Center(
          child: Icon(
            Icons.image_not_supported_outlined,
            size: 64,
            color: AppColors.textSecondary.withValues(alpha: 0.5),
          ),
        ),
      );
    }

    return Stack(
      children: [
        SizedBox(
          height: widget.height,
          child: PageView.builder(
            controller: _pageController,
            onPageChanged: (index) {
              setState(() {
                _currentIndex = index;
              });
            },
            itemCount: widget.images.length,
            itemBuilder: (context, index) {
              return GestureDetector(
                onTap: () => widget.onImageTap?.call(index),
                child: CachedImageWidget(
                  imageUrl: widget.images[index].url,
                  fit: BoxFit.cover,
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [
                      AppColors.transparent,
                      AppColors.black.withValues(alpha: 0.3),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
        if (widget.images.length > 1)
          Positioned(
            bottom: AppDimensions.paddingMedium,
            left: 0,
            right: 0,
            child: Center(
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingMedium,
                  vertical: AppDimensions.paddingSmall,
                ),
                decoration: BoxDecoration(
                  color: AppColors.black.withValues(alpha: 0.5),
                  borderRadius: BorderRadius.circular(
                    AppDimensions.borderRadiusLg,
                  ),
                ),
                child: SmoothPageIndicator(
                  controller: _pageController,
                  count: widget.images.length,
                  effect: WormEffect(
                    dotHeight: 8,
                    dotWidth: 8,
                    activeDotColor: AppColors.white,
                    dotColor: AppColors.white.withValues(alpha: 0.4),
                    spacing: AppDimensions.spacingSm,
                  ),
                ),
              ),
            ),
          ),
        if (widget.images.length > 1)
          Positioned(
            top: AppDimensions.paddingLarge + MediaQuery.of(context).padding.top,
            right: AppDimensions.paddingMedium,
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingSmall,
                vertical: AppDimensions.paddingXSmall,
              ),
              decoration: BoxDecoration(
                color: AppColors.black.withValues(alpha: 0.5),
                borderRadius: BorderRadius.circular(
                  AppDimensions.borderRadiusXs,
                ),
              ),
              child: Text(
                '${_currentIndex + 1} / ${widget.images.length}',
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ),
      ],
    );
  }
}