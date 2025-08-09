import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/widgets/cached_image_widget.dart';

class UploadReviewImageWidget extends StatelessWidget {
  final String imageUrl;
  final VoidCallback? onTap;
  final VoidCallback? onRemove;
  final double size;

  const UploadReviewImageWidget({
    super.key,
    required this.imageUrl,
    this.onTap,
    this.onRemove,
    this.size = 100,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Stack(
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            child: CachedImageWidget(
              imageUrl: imageUrl,
              width: size,
              height: size,
              fit: BoxFit.cover,
            ),
          ),
          if (onRemove != null)
            Positioned(
              top: 4,
              right: 4,
              child: GestureDetector(
                onTap: onRemove,
                child: Container(
                  padding: const EdgeInsets.all(4),
                  decoration: BoxDecoration(
                    color: AppColors.error.withValues(alpha: 0.9),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.close,
                    size: 16,
                    color: AppColors.white,
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}