import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../../core/widgets/cached_image_widget.dart';

class CityCardsGrid extends StatelessWidget {
  final HomeSection section;
  const CityCardsGrid({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    final items = section.validContent;
    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 3,
        mainAxisSpacing: 12,
        crossAxisSpacing: 12,
        childAspectRatio: 0.85,
      ),
      itemCount: items.length,
      itemBuilder: (context, index) {
        final data = items[index].contentData;
        final imageUrl = (data['imageUrl'] ?? data['cover']) as String?;
        final name = (data['name'] ?? data['title'])?.toString() ?? '';
        return Column(
          children: [
            Expanded(
              child: ClipRRect(
                borderRadius: BorderRadius.circular(10),
                child: imageUrl != null
                    ? CachedImageWidget(url: imageUrl, fit: BoxFit.cover)
                    : Container(color: Colors.grey.shade200),
              ),
            ),
            const SizedBox(height: 6),
            Text(name, maxLines: 1, overflow: TextOverflow.ellipsis),
          ],
        );
      },
    );
  }
}