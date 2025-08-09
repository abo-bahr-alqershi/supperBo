// lib/features/home/presentation/widgets/sections/common/parallax_image_widget.dart

import 'package:flutter/material.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';

class ParallaxImageWidget extends StatelessWidget {
  final String imageUrl;
  final double height;
  final double parallaxOffset;

  const ParallaxImageWidget({
    super.key,
    required this.imageUrl,
    required this.height,
    this.parallaxOffset = 0.3,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        return ClipRect(
          child: Transform.translate(
            offset: Offset(0, parallaxOffset * 50),
            child: CachedImageWidget(
              imageUrl: imageUrl,
              height: height * 1.3,
              fit: BoxFit.cover,
            ),
          ),
        );
      },
    );
  }
}