import 'package:flutter/material.dart';
import '../../domain/entities/home_section.dart';

class SectionDetailPage extends StatelessWidget {
  final HomeSection section;
  const SectionDetailPage({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(section.displayTitle)),
      body: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: section.content.length,
        separatorBuilder: (_, __) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          final item = section.content[index];
          return ListTile(
            title: Text(item.contentType),
            subtitle: Text(item.contentData.toString()),
          );
        },
      ),
    );
  }
}