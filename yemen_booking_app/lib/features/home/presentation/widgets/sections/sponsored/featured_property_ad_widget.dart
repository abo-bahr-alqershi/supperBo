import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../../core/widgets/cached_image_widget.dart';

class FeaturedPropertyAdWidget extends StatelessWidget {
  final HomeSection section;
  const FeaturedPropertyAdWidget({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    if (section.validContent.isEmpty) return const SizedBox.shrink();
    final data = section.validContent.first.contentData;
    final imageUrl = (data['imageUrl'] ?? data['cover']) as String?;
    final title = (data['title'] ?? data['name'])?.toString() ?? '';
    final badge = (section.sectionConfig.additionalSettings['badgeText'] ?? 'مميز').toString();

    return Stack(
      children: [
        AspectRatio(
          aspectRatio: 16 / 9,
          child: ClipRRect(
            borderRadius: BorderRadius.circular(12),
            child: imageUrl != null
                ? CachedImageWidget(url: imageUrl, fit: BoxFit.cover)
                : Container(color: Colors.grey.shade200),
          ),
        ),
        Positioned(
          top: 12,
          left: 12,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
            decoration: BoxDecoration(
              color: Colors.black.withOpacity(0.6),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Text(badge, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
          ),
        ),
        Positioned(
          bottom: 12,
          right: 12,
          left: 12,
          child: Text(
            title,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
                  shadows: const [Shadow(color: Colors.black54, blurRadius: 6)],
                ),
          ),
        )
      ],
    );
  }
}