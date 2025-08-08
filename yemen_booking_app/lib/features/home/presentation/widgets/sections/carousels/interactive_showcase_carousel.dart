// lib/features/home/presentation/widgets/sections/carousels/interactive_showcase_carousel.dart

import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';

class InteractiveShowcaseCarousel extends BaseSectionWidget {
  final List<dynamic> items;
  final Function(dynamic)? onItemTap;

  const InteractiveShowcaseCarousel({
    super.key,
    required HomeSection section,
    required this.items,
    required SectionConfig config,
    this.onItemTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Interactive Showcase Carousel'),
    );
  }
}