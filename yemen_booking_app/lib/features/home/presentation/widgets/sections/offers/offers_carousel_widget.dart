// lib/features/home/presentation/widgets/sections/offers/offers_carousel_widget.dart

import 'package:flutter/material.dart';
import 'package:carousel_slider/carousel_slider.dart';
import '../base/base_section_widget.dart';

class OffersCarouselWidget extends BaseSectionWidget {
  final List<dynamic> offers;
  final Function(dynamic)? onOfferTap;

  const OffersCarouselWidget({
    super.key,
    required super.section,
    required this.offers,
    required super.config,
    this.onOfferTap,
  });

  @override
  Widget build(BuildContext context) {
    return buildSectionContainer(
      child: CarouselSlider.builder(
        itemCount: offers.length,
        options: CarouselOptions(
          height: 260,
          viewportFraction: 0.9,
          autoPlay: config.autoPlay,
        ),
        itemBuilder: (context, index, realIndex) {
          return Container(
            margin: const EdgeInsets.symmetric(horizontal: 8),
            child: const Text('Offer Carousel Item'),
          );
        },
      ),
    );
  }
}