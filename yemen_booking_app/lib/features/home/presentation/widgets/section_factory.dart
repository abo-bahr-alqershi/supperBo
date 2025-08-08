// lib/features/home/presentation/widgets/section_factory.dart

import 'package:flutter/material.dart';

import '../../../../core/enums/section_type_enum.dart';
import '../../domain/entities/home_section.dart';
import 'sections/sponsored/single_property_ad_widget.dart';
import 'sections/sponsored/multi_property_ad_widget.dart';
import 'sections/sponsored/unit_showcase_ad_widget.dart';
import 'sections/offers/single_property_offer_widget.dart';
import 'sections/offers/limited_time_offer_widget.dart';
import 'sections/offers/seasonal_offer_widget.dart';
import 'sections/offers/multi_property_offers_grid.dart';
import 'sections/offers/offers_carousel_widget.dart';
import 'sections/offers/flash_deals_widget.dart';
import 'sections/listings/horizontal_property_list.dart';
import 'sections/listings/vertical_property_grid.dart';
import 'sections/listings/mixed_layout_list.dart';
import 'sections/listings/compact_property_list.dart';
import 'sections/destinations/city_cards_grid.dart';
import 'sections/destinations/destination_carousel.dart';
import 'sections/destinations/explore_cities_widget.dart';
import 'sections/carousels/premium_property_carousel.dart';
import 'sections/carousels/interactive_showcase_carousel.dart';

class SectionFactory {
  static Widget createSection({
    required HomeSection section,
    required dynamic data,
    bool isFullScreen = false,
    Function(dynamic)? onItemTap,
  }) {
    final config = section.sectionConfig;
    
    switch (section.sectionType) {
      // Sponsored Sections
      case SectionType.singlePropertyAd:
        return SinglePropertyAdWidget(
          section: section,
          property: data,
          config: config,
          onTap: onItemTap,
        );
      
      case SectionType.multiPropertyAd:
        return MultiPropertyAdWidget(
          section: section,
          properties: data,
          config: config,
          onPropertyTap: onItemTap,
        );
      
      case SectionType.unitShowcaseAd:
        return UnitShowcaseAdWidget(
          section: section,
          units: data,
          config: config,
          onUnitTap: onItemTap,
        );
      
      // Offers Sections
      case SectionType.singlePropertyOffer:
        return SinglePropertyOfferWidget(
          section: section,
          offer: data,
          config: config,
          onTap: onItemTap,
        );
      
      case SectionType.limitedTimeOffer:
        return LimitedTimeOfferWidget(
          section: section,
          offer: data,
          config: config,
          onTap: onItemTap,
        );
      
      case SectionType.seasonalOffer:
        return SeasonalOfferWidget(
          section: section,
          offers: data,
          config: config,
          onOfferTap: onItemTap,
        );
      
      case SectionType.multiPropertyOffersGrid:
        return MultiPropertyOffersGrid(
          section: section,
          offers: data,
          config: config,
          onOfferTap: onItemTap,
        );
      
      case SectionType.offersCarousel:
        return OffersCarouselWidget(
          section: section,
          offers: data,
          config: config,
          onOfferTap: onItemTap,
        );
      
      case SectionType.flashDeals:
        return FlashDealsWidget(
          section: section,
          deals: data,
          config: config,
          onDealTap: onItemTap,
        );
      
      // Listings Sections
      case SectionType.horizontalPropertyList:
        return HorizontalPropertyList(
          section: section,
          properties: data,
          config: config,
          onPropertyTap: onItemTap,
        );
      
      case SectionType.verticalPropertyGrid:
        return VerticalPropertyGrid(
          section: section,
          properties: data,
          config: config,
          isFullScreen: isFullScreen,
          onPropertyTap: onItemTap,
        );
      
      case SectionType.mixedLayoutList:
        return MixedLayoutList(
          section: section,
          properties: data,
          config: config,
          onPropertyTap: onItemTap,
        );
      
      case SectionType.compactPropertyList:
        return CompactPropertyList(
          section: section,
          properties: data,
          config: config,
          onPropertyTap: onItemTap,
        );
      
      // Destinations Sections
      case SectionType.cityCardsGrid:
        return CityCardsGrid(
          section: section,
          cities: data,
          config: config,
          onCityTap: onItemTap,
        );
      
      case SectionType.destinationCarousel:
        return DestinationCarousel(
          section: section,
          destinations: data,
          config: config,
          onDestinationTap: onItemTap,
        );
      
      case SectionType.exploreCities:
        return ExploreCitiesWidget(
          section: section,
          cities: data,
          config: config,
          onCityTap: onItemTap,
        );
      
      // Premium Carousels
      case SectionType.premiumCarousel:
        return PremiumPropertyCarousel(
          section: section,
          properties: data,
          config: config,
          onPropertyTap: onItemTap,
        );
      
      case SectionType.interactiveShowcase:
        return InteractiveShowcaseCarousel(
          section: section,
          items: data,
          config: config,
          onItemTap: onItemTap,
        );
      
      default:
        return Container(
          padding: const EdgeInsets.all(16),
          child: Text(
            'Unknown section type: ${section.sectionType.value}',
            style: const TextStyle(color: Colors.red),
          ),
        );
    }
  }
}