// lib/features/home/presentation/widgets/sections/offers/limited_time_offer_widget.dart

import '../offers/single_property_offer_widget.dart';

class LimitedTimeOfferWidget extends SinglePropertyOfferWidget {
  const LimitedTimeOfferWidget({
    super.key,
    required super.section,
    required super.offer,
    required super.config,
    super.onTap,
  });
}