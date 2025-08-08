// lib/features/home/presentation/widgets/sections/listings/featured_properties_showcase.dart

import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';

class FeaturedPropertiesShowcase extends BaseSectionWidget {
  final List<dynamic> properties;
  final Function(dynamic)? onPropertyTap;

  const FeaturedPropertiesShowcase({
    super.key,
    required HomeSection section,
    required this.properties,
    required SectionConfig config,
    this.onPropertyTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Featured Properties Showcase'),
    );
  }
}