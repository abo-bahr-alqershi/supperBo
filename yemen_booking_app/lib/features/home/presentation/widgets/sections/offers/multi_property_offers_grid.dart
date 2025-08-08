// lib/features/home/presentation/widgets/sections/offers/multi_property_offers_grid.dart

import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';

class MultiPropertyOffersGrid extends BaseSectionWidget {
  final List<dynamic> offers;
  final Function(dynamic)? onOfferTap;

  const MultiPropertyOffersGrid({
    super.key,
    required HomeSection section,
    required this.offers,
    required SectionConfig config,
    this.onOfferTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Multi Property Offers Grid'),
    );
  }
}