// lib/features/home/presentation/widgets/sections/destinations/explore_cities_widget.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class ExploreCitiesWidget extends BaseSectionWidget {
  final List<dynamic> cities;
  final Function(dynamic)? onCityTap;

  const ExploreCitiesWidget({
    super.key,
    required super.section,
    required this.cities,
    required super.config,
    this.onCityTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Explore Cities'),
    );
  }
}