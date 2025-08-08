import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../theme/app_colors.dart';
import '../theme/app_dimensions.dart';

class CachedImageWidget extends StatelessWidget {
  final String imageUrl;
  final double? width;
  final double? height;
  final BoxFit fit;
  final BorderRadius? borderRadius;
  final Widget? placeholder;
  final Widget? errorWidget;
  final bool showLoadingIndicator;
  final Color? backgroundColor;
  final List<BoxShadow>? boxShadow;
  final Gradient? gradient;
  final BlendMode? colorBlendMode;
  final Color? color;

  const CachedImageWidget({
    super.key,
    required this.imageUrl,
    this.width,
    this.height,
    this.fit = BoxFit.cover,
    this.borderRadius,
    this.placeholder,
    this.errorWidget,
    this.showLoadingIndicator = true,
    this.backgroundColor,
    this.boxShadow,
    this.gradient,
    this.colorBlendMode,
    this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: backgroundColor ?? AppColors.surface,
        borderRadius: borderRadius,
        boxShadow: boxShadow,
      ),
      child: ClipRRect(
        borderRadius: borderRadius ?? BorderRadius.zero,
        child: Stack(
          fit: StackFit.expand,
          children: [
            CachedNetworkImage(
              imageUrl: imageUrl,
              fit: fit,
              width: width,
              height: height,
              color: color,
              colorBlendMode: colorBlendMode,
              placeholder: (context, url) => placeholder ?? _buildPlaceholder(),
              errorWidget: (context, url, error) => 
                errorWidget ?? _buildErrorWidget(),
            ),
            if (gradient != null)
              Container(
                decoration: BoxDecoration(
                  gradient: gradient,
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildPlaceholder() {
    return Container(
      color: AppColors.shimmer.withValues(alpha: 0.1),
      child: showLoadingIndicator
          ? Center(
              child: SizedBox(
                width: 24,
                height: 24,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  valueColor: AlwaysStoppedAnimation<Color>(
                    AppColors.primary.withValues(alpha: 0.5),
                  ),
                ),
              ),
            )
          : const SizedBox.shrink(),
    );
  }

  Widget _buildErrorWidget() {
    return Container(
      color: AppColors.surface,
      child: Center(
        child: Icon(
          Icons.image_not_supported_outlined,
          color: AppColors.textSecondary.withValues(alpha: 0.5),
          size: AppDimensions.iconLarge,
        ),
      ),
    );
  }
}