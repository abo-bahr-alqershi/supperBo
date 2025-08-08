// lib/features/home/presentation/widgets/sections/offers/flash_deals_widget.dart

import 'package:flutter/material.dart';
import '../base/base_section_widget.dart';

class FlashDealsWidget extends BaseSectionWidget {
  final List<dynamic> deals;
  final Function(dynamic)? onDealTap;

  const FlashDealsWidget({
    super.key,
    required super.section,
    required this.deals,
    required super.config,
    this.onDealTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Flash Deals'),
    );
  }
}