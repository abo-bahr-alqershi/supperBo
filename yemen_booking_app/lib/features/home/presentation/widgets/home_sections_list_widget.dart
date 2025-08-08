import 'package:flutter/material.dart';
import '../../domain/entities/home_section.dart';
import 'section_builder_widget.dart';

class HomeSectionsListSliver extends StatelessWidget {
  final List<HomeSection> sections;
  const HomeSectionsListSliver({super.key, required this.sections});

  @override
  Widget build(BuildContext context) {
    final visible = sections.where((s) => s.isVisible).toList()..sort((a, b) => a.order.compareTo(b.order));
    return SliverList(
      delegate: SliverChildBuilderDelegate(
        (context, index) => Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
          child: SectionBuilderWidget(section: visible[index]),
        ),
        childCount: visible.length,
      ),
    );
  }
}