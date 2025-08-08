// lib/features/home/presentation/widgets/sections/destinations/destination_carousel.dart

import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';

class DestinationCarousel extends BaseSectionWidget {
  final List<dynamic> destinations;
  final Function(dynamic)? onDestinationTap;

  const DestinationCarousel({
    super.key,
    required HomeSection section,
    required this.destinations,
    required SectionConfig config,
    this.onDestinationTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Destination Carousel'),
    );
  }
}