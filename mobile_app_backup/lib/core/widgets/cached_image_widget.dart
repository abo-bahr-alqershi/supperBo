import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart'; // Add to pubspec.yaml
import '../theme/app_colors.dart';
import '../constants/api_constants.dart'; // For imageBaseUrl

class CachedImageWidget extends StatelessWidget {
  final String? imageUrl;
  final double? width;
  final double? height;
  final BoxFit fit;
  final String? placeholderImage; // Local asset path for placeholder
  final String? errorImage; // Local asset path for error fallback

  const CachedImageWidget({
    super.key,
    required this.imageUrl,
    this.width,
    this.height,
    this.fit = BoxFit.cover,
    this.placeholderImage,
    this.errorImage,
  });

  @override
  Widget build(BuildContext context) {
    // Construct full image URL if needed (e.g., if imageUrl is relative)
    // If imageUrl already contains the full URL, this part can be simplified.
    final String fullImageUrl = imageUrl != null && !imageUrl!.startsWith('http')
        ? '${ApiConstants.imageBaseUrl}/$imageUrl'
        : imageUrl ?? '';

    return CachedNetworkImage(
      imageUrl: fullImageUrl,
      width: width,
      height: height,
      fit: fit,
      placeholder: (context, url) => Container(
        width: width,
        height: height,
        color: AppColors.gray200, // Placeholder background color
        child: placeholderImage != null
            ? Image.asset(placeholderImage!, fit: BoxFit.contain)
            : const Center(child: Icon(Icons.image_outlined, color: AppColors.gray500)), // Default placeholder icon
      ),
      errorWidget: (context, url, error) => Container(
        width: width,
        height: height,
        color: AppColors.gray200, // Error background color
        child: errorImage != null
            ? Image.asset(errorImage!, fit: BoxFit.contain)
            : const Center(child: Icon(Icons.error_outline, color: AppColors.error)), // Default error icon
      ),
      imageBuilder: (context, imageProvider) => Container(
        width: width,
        height: height,
        decoration: BoxDecoration(
          image: DecorationImage(
            image: imageProvider,
            fit: fit,
          ),
        ),
      ),
    );
  }
}