// lib/features/home/presentation/widgets/sections/destinations/city_cards_grid.dart

import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';

class CityCardsGrid extends BaseSectionWidget {
  final List<dynamic> cities;
  final Function(dynamic)? onCityTap;

  const CityCardsGrid({
    super.key,
    required HomeSection section,
    required this.cities,
    required SectionConfig config,
    this.onCityTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('City Cards Grid'),
    );
  }
}