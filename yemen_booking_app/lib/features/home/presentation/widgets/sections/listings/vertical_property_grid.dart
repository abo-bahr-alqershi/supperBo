import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../../core/widgets/cached_image_widget.dart';

class VerticalPropertyGrid extends StatelessWidget {
  final HomeSection section;
  const VerticalPropertyGrid({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    final items = section.validContent;
    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        mainAxisSpacing: 12,
        crossAxisSpacing: 12,
        childAspectRatio: 0.78,
      ),
      itemCount: items.length,
      itemBuilder: (context, index) {
        final data = items[index].contentData;
        final imageUrl = (data['imageUrl'] ?? data['cover']) as String?;
        final title = (data['title'] ?? data['name'])?.toString() ?? '';
        final price = data['price']?.toString();
        return Column(
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
        );
      },
    );
  }
}