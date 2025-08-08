// lib/features/home/presentation/widgets/sections/sponsored/unit_showcase_ad_widget.dart

import '../sponsored/multi_property_ad_widget.dart';

class UnitShowcaseAdWidget extends MultiPropertyAdWidget {
  const UnitShowcaseAdWidget({
    super.key,
    required super.section,
    required List<dynamic> units,
    required super.config,
    Function(dynamic)? onUnitTap,
  }) : super(
          properties: units,
          onPropertyTap: onUnitTap,
        );
}