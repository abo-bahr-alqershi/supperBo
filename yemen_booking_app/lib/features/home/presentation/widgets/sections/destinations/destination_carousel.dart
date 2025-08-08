// lib/features/home/presentation/widgets/sections/destinations/destination_carousel.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class DestinationCarousel extends BaseSectionWidget {
  final List<dynamic> destinations;
  final Function(dynamic)? onDestinationTap;

  const DestinationCarousel({
    super.key,
    required super.section,
    required this.destinations,
    required super.config,
    this.onDestinationTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Destination Carousel'),
    );
  }
}