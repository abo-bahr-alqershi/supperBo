import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../../core/widgets/cached_image_widget.dart';

class HorizontalPropertyList extends StatelessWidget {
  final HomeSection section;
  const HorizontalPropertyList({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    final items = section.validContent;
    return SizedBox(
      height: 220,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: items.length,
        separatorBuilder: (_, __) => const SizedBox(width: 12),
        itemBuilder: (context, index) {
          final data = items[index].contentData;
          final imageUrl = (data['imageUrl'] ?? data['cover']) as String?;
          final title = (data['title'] ?? data['name'])?.toString() ?? '';
          final price = data['price']?.toString();
          return SizedBox(
            width: 180,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(10),
                    child: imageUrl != null
                        ? CachedImageWidget(url: imageUrl, fit: BoxFit.cover)
                        : Container(color: Colors.grey.shade200),
                  ),
                ),
                const SizedBox(height: 8),
                Text(title, maxLines: 1, overflow: TextOverflow.ellipsis),
                if (price != null) Text(price, style: Theme.of(context).textTheme.labelLarge),
              ],
            ),
          );
        },
      ),
    );
  }
}