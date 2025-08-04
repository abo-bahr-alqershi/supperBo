// lib/core/enums/section_type_enum.dart

enum SectionType {
  // Sponsored Ads
  singlePropertyAd('SINGLE_PROPERTY_AD', 'Sponsored'),
  featuredPropertyAd('FEATURED_PROPERTY_AD', 'Sponsored'),
  multiPropertyAd('MULTI_PROPERTY_AD', 'Sponsored'),
  unitShowcaseAd('UNIT_SHOWCASE_AD', 'Sponsored'),
  
  // Special Offers
  singlePropertyOffer('SINGLE_PROPERTY_OFFER', 'Offers'),
  limitedTimeOffer('LIMITED_TIME_OFFER', 'Offers'),
  seasonalOffer('SEASONAL_OFFER', 'Offers'),
  multiPropertyOffersGrid('MULTI_PROPERTY_OFFERS_GRID', 'Offers'),
  offersCarousel('OFFERS_CAROUSEL', 'Offers'),
  flashDeals('FLASH_DEALS', 'Offers'),
  
  // Property Listings
  horizontalPropertyList('HORIZONTAL_PROPERTY_LIST', 'Listings'),
  verticalPropertyGrid('VERTICAL_PROPERTY_GRID', 'Listings'),
  mixedLayoutList('MIXED_LAYOUT_LIST', 'Listings'),
  compactPropertyList('COMPACT_PROPERTY_LIST', 'Listings'),
  featuredPropertiesShowcase('FEATURED_PROPERTIES_SHOWCASE', 'Listings'),
  
  // Destinations
  cityCardsGrid('CITY_CARDS_GRID', 'Destinations'),
  destinationCarousel('DESTINATION_CAROUSEL', 'Destinations'),
  exploreCities('EXPLORE_CITIES', 'Destinations'),
  
  // Premium Carousels
  premiumCarousel('PREMIUM_CAROUSEL', 'Carousels'),
  interactiveShowcase('INTERACTIVE_SHOWCASE', 'Carousels');

  final String value;
  final String category;

  const SectionType(this.value, this.category);

  static SectionType? fromString(String value) {
    return SectionType.values.firstWhere(
      (type) => type.value == value,
      orElse: () => throw ArgumentError('Invalid section type: $value'),
    );
  }

  static SectionType? tryFromString(String? value) {
    if (value == null) return null;
    try {
      return fromString(value);
    } catch (_) {
      return null;
    }
  }

  bool get isSponsored => category == 'Sponsored';
  bool get isOffer => category == 'Offers';
  bool get isListing => category == 'Listings';
  bool get isDestination => category == 'Destinations';
  bool get isCarousel => category == 'Carousels';

  // Check if section requires property data
  bool get requiresPropertyData {
    return [
      singlePropertyAd,
      featuredPropertyAd,
      multiPropertyAd,
      unitShowcaseAd,
      singlePropertyOffer,
      limitedTimeOffer,
      multiPropertyOffersGrid,
      horizontalPropertyList,
      verticalPropertyGrid,
      mixedLayoutList,
      compactPropertyList,
      featuredPropertiesShowcase,
    ].contains(this);
  }

  // Check if section is time-sensitive
  bool get isTimeSensitive {
    return [
      limitedTimeOffer,
      flashDeals,
      seasonalOffer,
    ].contains(this);
  }

  // Get default configuration for each type
  Map<String, dynamic> get defaultConfig {
    switch (this) {
      case SectionType.singlePropertyAd:
        return {
          'maxItems': 1,
          'autoPlay': false,
          'parallaxEnabled': true,
          'animationType': 'parallax',
        };
      case SectionType.featuredPropertyAd:
        return {
          'maxItems': 1,
          'autoPlay': false,
          'showBadge': true,
          'badgeText': 'FEATURED',
          'animationType': 'scale',
        };
      case SectionType.multiPropertyAd:
        return {
          'maxItems': 4,
          'autoPlay': true,
          'autoPlayDuration': 5,
          'layoutType': 'grid',
        };
      case SectionType.offersCarousel:
        return {
          'maxItems': 10,
          'autoPlay': true,
          'autoPlayDuration': 4,
          'showIndicators': true,
        };
      case SectionType.flashDeals:
        return {
          'maxItems': 6,
          'autoPlay': false,
          'showCountdown': true,
          'animationType': 'pulse',
        };
      default:
        return {
          'maxItems': 10,
          'autoPlay': false,
        };
    }
  }
}