// lib/features/home/presentation/widgets/sections/offers/seasonal_offer_widget.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class SeasonalOfferWidget extends BaseSectionWidget {
  final List<dynamic> offers;
  final Function(dynamic)? onOfferTap;

  const SeasonalOfferWidget({
    super.key,
    required super.section,
    required this.offers,
    required super.config,
    this.onOfferTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Seasonal Offers'),
    );
  }
}