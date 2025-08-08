// lib/features/home/presentation/widgets/sections/listings/compact_property_list.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class CompactPropertyList extends BaseSectionWidget {
  final List<dynamic> properties;
  final Function(dynamic)? onPropertyTap;

  const CompactPropertyList({
    super.key,
    required super.section,
    required this.properties,
    required super.config,
    this.onPropertyTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Compact Property List'),
    );
  }
}