// lib/features/home/presentation/widgets/sections/carousels/premium_property_carousel.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class PremiumPropertyCarousel extends BaseSectionWidget {
  final List<dynamic> properties;
  final Function(dynamic)? onPropertyTap;

  const PremiumPropertyCarousel({
    super.key,
    required super.section,
    required this.properties,
    required super.config,
    this.onPropertyTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Premium Property Carousel'),
    );
  }
}