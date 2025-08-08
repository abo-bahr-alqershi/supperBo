// lib/features/home/presentation/widgets/sections/listings/vertical_property_grid.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class VerticalPropertyGrid extends BaseSectionWidget {
  final List<dynamic> properties;
  final bool isFullScreen;
  final Function(dynamic)? onPropertyTap;

  const VerticalPropertyGrid({
    super.key,
    required super.section,
    required this.properties,
    required super.config,
    this.isFullScreen = false,
    this.onPropertyTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Vertical Property Grid'),
    );
  }
}