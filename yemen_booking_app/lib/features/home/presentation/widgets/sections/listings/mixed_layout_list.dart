// lib/features/home/presentation/widgets/sections/listings/mixed_layout_list.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class MixedLayoutList extends BaseSectionWidget {
  final List<dynamic> properties;
  final Function(dynamic)? onPropertyTap;

  const MixedLayoutList({
    super.key,
    required super.section,
    required this.properties,
    required super.config,
    this.onPropertyTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Mixed Layout List'),
    );
  }
}