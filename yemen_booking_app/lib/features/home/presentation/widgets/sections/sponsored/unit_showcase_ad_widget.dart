// lib/features/home/presentation/widgets/sections/sponsored/unit_showcase_ad_widget.dart

import 'package:flutter/material.dart';
import '../sponsored/multi_property_ad_widget.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';

class UnitShowcaseAdWidget extends MultiPropertyAdWidget {
  const UnitShowcaseAdWidget({
    super.key,
    required HomeSection section,
    required List<dynamic> units,
    required SectionConfig config,
    Function(dynamic)? onUnitTap,
  }) : super(
          section: section,
          properties: units,
          config: config,
          onPropertyTap: onUnitTap,
        );
}