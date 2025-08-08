// lib/features/home/presentation/widgets/sections/carousels/interactive_showcase_carousel.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class InteractiveShowcaseCarousel extends BaseSectionWidget {
  final List<dynamic> items;
  final Function(dynamic)? onItemTap;

  const InteractiveShowcaseCarousel({
    super.key,
    required super.section,
    required this.items,
    required super.config,
    this.onItemTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Interactive Showcase Carousel'),
    );
  }
}