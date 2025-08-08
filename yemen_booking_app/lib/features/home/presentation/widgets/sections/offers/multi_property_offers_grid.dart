// lib/features/home/presentation/widgets/sections/offers/multi_property_offers_grid.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class MultiPropertyOffersGrid extends BaseSectionWidget {
  final List<dynamic> offers;
  final Function(dynamic)? onOfferTap;

  const MultiPropertyOffersGrid({
    super.key,
    required super.section,
    required this.offers,
    required super.config,
    this.onOfferTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Multi Property Offers Grid'),
    );
  }
}