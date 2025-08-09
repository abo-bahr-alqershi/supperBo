import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:photo_view/photo_view.dart';
import 'package:photo_view/photo_view_gallery.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/property_detail.dart';

class PropertyGalleryPage extends StatefulWidget {
  final List<PropertyImage> images;
  final int initialIndex;

  const PropertyGalleryPage({
    super.key,
    required this.images,
    this.initialIndex = 0,
  });

  @override
  State<PropertyGalleryPage> createState() => _PropertyGalleryPageState();
}

class _PropertyGalleryPageState extends State<PropertyGalleryPage> {
  late PageController _pageController;
  late int _currentIndex;
  bool _showInfo = true;

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.initialIndex;
    _pageController = PageController(initialPage: widget.initialIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.black,
      body: Stack(
        children: [
          _buildPhotoGallery(),
          _buildTopBar(),
          if (_showInfo) _buildBottomInfo(),
          _buildPageIndicator(),
        ],
      ),
    );
  }

  Widget _buildPhotoGallery() {
    return GestureDetector(
      onTap: () {
        setState(() {
          _showInfo = !_showInfo;
        });
      },
      child: PhotoViewGallery.builder(
        scrollPhysics: const BouncingScrollPhysics(),
        builder: (BuildContext context, int index) {
          return PhotoViewGalleryPageOptions(
            imageProvider: NetworkImage(widget.images[index].url),
            initialScale: PhotoViewComputedScale.contained,
            minScale: PhotoViewComputedScale.contained,
            maxScale: PhotoViewComputedScale.covered * 3,
            heroAttributes: PhotoViewHeroAttributes(
              tag: widget.images[index].id,
            ),
          );
        },
        itemCount: widget.images.length,
        loadingBuilder: (context, event) => Center(
          child: SizedBox(
            width: 30.0,
            height: 30.0,
            child: CircularProgressIndicator(
              value: event == null
                  ? 0
                  : event.cumulativeBytesLoaded / (event.expectedTotalBytes ?? 1),
              valueColor: const AlwaysStoppedAnimation<Color>(AppColors.primary),
            ),
          ),
        ),
        backgroundDecoration: const BoxDecoration(
          color: AppColors.black,
        ),
        pageController: _pageController,
        onPageChanged: (index) {
          setState(() {
            _currentIndex = index;
          });
        },
      ),
    );
  }

  Widget _buildTopBar() {
    return AnimatedOpacity(
      opacity: _showInfo ? 1.0 : 0.0,
      duration: const Duration(milliseconds: 200),
      child: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              AppColors.black.withValues(alpha: 0.7),
              AppColors.transparent,
            ],
          ),
        ),
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                IconButton(
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(
                    Icons.close,
                    color: AppColors.white,
                    size: 28,
                  ),
                ),
                Text(
                  '${_currentIndex + 1} / ${widget.images.length}',
                  style: AppTextStyles.subtitle1.copyWith(
                    color: AppColors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                IconButton(
                  onPressed: _shareImage,
                  icon: const Icon(
                    Icons.share_outlined,
                    color: AppColors.white,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildBottomInfo() {
    final currentImage = widget.images[_currentIndex];
    
    return AnimatedOpacity(
      opacity: _showInfo ? 1.0 : 0.0,
      duration: const Duration(milliseconds: 200),
      child: Positioned(
        bottom: 0,
        left: 0,
        right: 0,
        child: Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.bottomCenter,
              end: Alignment.topCenter,
              colors: [
                AppColors.black.withValues(alpha: 0.8),
                AppColors.transparent,
              ],
            ),
          ),
          padding: EdgeInsets.only(
            left: AppDimensions.paddingMedium,
            right: AppDimensions.paddingMedium,
            bottom: MediaQuery.of(context).padding.bottom + AppDimensions.paddingMedium,
            top: AppDimensions.paddingLarge,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              if (currentImage.caption.isNotEmpty) ...[
                Text(
                  currentImage.caption,
                  style: AppTextStyles.bodyLarge.copyWith(
                    color: AppColors.white,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingSm),
              ],
              if (currentImage.altText.isNotEmpty)
                Text(
                  currentImage.altText,
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppColors.white.withValues(alpha: 0.8),
                  ),
                ),
              if (currentImage.tags.isNotEmpty) ...[
                const SizedBox(height: AppDimensions.spacingMd),
                Wrap(
                  spacing: AppDimensions.spacingSm,
                  children: currentImage.tags.split(',').map((tag) {
                    return Chip(
                      label: Text(
                        tag.trim(),
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.white,
                        ),
                      ),
                      backgroundColor: AppColors.white.withValues(alpha: 0.2),
                      padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingSmall),
                    );
                  }).toList(),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildPageIndicator() {
    if (widget.images.length <= 1) return const SizedBox.shrink();

    return Positioned(
      bottom: MediaQuery.of(context).padding.bottom + 100,
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
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: List.generate(
              widget.images.length,
              (index) => AnimatedContainer(
                duration: const Duration(milliseconds: 200),
                margin: const EdgeInsets.symmetric(horizontal: 3),
                width: _currentIndex == index ? 24 : 8,
                height: 8,
                decoration: BoxDecoration(
                  color: _currentIndex == index
                      ? AppColors.primary
                      : AppColors.white.withValues(alpha: 0.5),
                  borderRadius: BorderRadius.circular(4),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  void _shareImage() {
    // Implement share functionality
  }
}