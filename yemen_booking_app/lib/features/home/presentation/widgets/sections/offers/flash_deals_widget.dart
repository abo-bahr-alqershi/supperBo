// lib/features/home/presentation/widgets/sections/offers/flash_deals_widget.dart

import 'package:flutter/material.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';
import '../base/base_section_widget.dart';

class FlashDealsWidget extends BaseSectionWidget {
  final List<dynamic> deals;
  final Function(dynamic)? onDealTap;

  const FlashDealsWidget({
    super.key,
    required HomeSection section,
    required this.deals,
    required SectionConfig config,
    this.onDealTap,
  }) : super(section: section, config: config);

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: const Text('Flash Deals'),
    );
  }
}