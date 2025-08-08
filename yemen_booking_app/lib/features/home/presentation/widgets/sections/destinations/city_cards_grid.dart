// lib/features/home/presentation/widgets/sections/destinations/city_cards_grid.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class CityCardsGrid extends BaseSectionWidget {
  final List<dynamic> cities;
  final Function(dynamic)? onCityTap;

  const CityCardsGrid({
    super.key,
    required super.section,
    required this.cities,
    required super.config,
    this.onCityTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('City Cards Grid'),
    );
  }
}