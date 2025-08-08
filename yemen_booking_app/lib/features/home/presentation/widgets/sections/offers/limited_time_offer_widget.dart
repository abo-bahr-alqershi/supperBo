// lib/features/home/presentation/widgets/sections/offers/limited_time_offer_widget.dart

import 'package:flutter/material.dart';
import '../offers/single_property_offer_widget.dart';
import '../../../../domain/entities/home_section.dart';
import '../../../../domain/entities/section_config.dart';

class LimitedTimeOfferWidget extends SinglePropertyOfferWidget {
  const LimitedTimeOfferWidget({
    super.key,
    required HomeSection section,
    required dynamic offer,
    required SectionConfig config,
    Function(dynamic)? onTap,
  }) : super(
          section: section,
          offer: offer,
          config: config,
          onTap: onTap,
        );
}