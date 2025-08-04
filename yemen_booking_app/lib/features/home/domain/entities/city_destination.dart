// lib/features/home/domain/entities/city_destination.dart

import 'dart:math' as math;
import 'package:equatable/equatable.dart';

class CityDestination extends Equatable {
  final String id;
  final String name;
  final String nameAr;
  final String country;
  final String countryAr;
  final String? description;
  final String? descriptionAr;
  final String imageUrl;
  final List<String> additionalImages;
  final double latitude;
  final double longitude;
  final int propertyCount;
  final double averagePrice;
  final String currency;
  final double averageRating;
  final int reviewCount;
  final bool isPopular;
  final bool isFeatured;
  final int priority;
  final List<String> highlights;
  final List<String> highlightsAr;
  final Map<String, dynamic> weatherData;
  final Map<String, dynamic> attractionsData;
  final Map<String, dynamic> metadata;

  const CityDestination({
    required this.id,
    required this.name,
    required this.nameAr,
    required this.country,
    required this.countryAr,
    this.description,
    this.descriptionAr,
    required this.imageUrl,
    required this.additionalImages,
    required this.latitude,
    required this.longitude,
    required this.propertyCount,
    required this.averagePrice,
    required this.currency,
    required this.averageRating,
    required this.reviewCount,
    this.isPopular = false,
    this.isFeatured = false,
    this.priority = 0,
    required this.highlights,
    required this.highlightsAr,
    required this.weatherData,
    required this.attractionsData,
    required this.metadata,
  });

  String getLocalizedName(bool isArabic) => isArabic ? nameAr : name;
  String getLocalizedCountry(bool isArabic) => isArabic ? countryAr : country;
  String? getLocalizedDescription(bool isArabic) => isArabic ? descriptionAr : description;
  List<String> getLocalizedHighlights(bool isArabic) => isArabic ? highlightsAr : highlights;

  String get fullName => '$name, $country';
  String get fullNameAr => '$nameAr، $countryAr';
  String getLocalizedFullName(bool isArabic) => isArabic ? fullNameAr : fullName;

  String get formattedAveragePrice => '${averagePrice.toStringAsFixed(0)} $currency';
  String get formattedRating => averageRating.toStringAsFixed(1);

  bool get hasWeatherData => weatherData.isNotEmpty;
  bool get hasAttractions => attractionsData.isNotEmpty;
  bool get hasReviews => reviewCount > 0;
  bool get hasProperties => propertyCount > 0;

  // Weather data getters
  double? get currentTemperature => weatherData['temperature'] as double?;
  String? get weatherCondition => weatherData['condition'] as String?;
  String? get weatherIcon => weatherData['icon'] as String?;
  int? get humidity => weatherData['humidity'] as int?;
  double? get windSpeed => weatherData['windSpeed'] as double?;

  String get formattedTemperature {
    if (currentTemperature == null) return '';
    return '${currentTemperature!.toStringAsFixed(0)}°C';
  }

  // Attractions data getters
  List<String> get popularAttractions {
    final attractions = attractionsData['popular'] as List<dynamic>?;
    return attractions?.cast<String>() ?? [];
  }

  List<String> get topAttractions => popularAttractions.take(3).toList();

  int get attractionsCount {
    final attractions = attractionsData['count'] as int?;
    return attractions ?? popularAttractions.length;
  }

  // Location helpers
  String get coordinatesString => '${latitude.toStringAsFixed(6)}, ${longitude.toStringAsFixed(6)}';

  double distanceTo(double otherLat, double otherLng) {
    // Simple distance calculation using Haversine formula
    const double earthRadius = 6371; // Earth's radius in kilometers
    final double latDiff = (otherLat - latitude) * (3.14159 / 180);
    final double lngDiff = (otherLng - longitude) * (3.14159 / 180);
    final double a = (latDiff / 2) * (latDiff / 2) +
        (lngDiff / 2) * (lngDiff / 2) * 
        math.cos(latitude * 3.14159 / 180) * 
        math.cos(otherLat * 3.14159 / 180);
    final double c = 2 * math.asin(math.sqrt(a));
    return earthRadius * c;
  }

  // Property statistics
  String get propertyCountText {
    if (propertyCount == 0) return 'No properties';
    if (propertyCount == 1) return '1 property';
    return '$propertyCount properties';
  }

  String get reviewCountText {
    if (reviewCount == 0) return 'No reviews';
    if (reviewCount == 1) return '1 review';
    if (reviewCount < 1000) return '$reviewCount reviews';
    final double thousands = reviewCount / 1000;
    return '${thousands.toStringAsFixed(1)}k reviews';
  }

  // Metadata helpers
  T? getMetadata<T>(String key) {
    try {
      return metadata[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getWeatherData<T>(String key) {
    try {
      return weatherData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getAttractionsData<T>(String key) {
    try {
      return attractionsData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  // Ranking and popularity
  String get popularityLevel {
    if (!isPopular) return 'normal';
    if (isFeatured) return 'featured';
    return 'popular';
  }

  List<String> get tags {
    List<String> cityTags = [];
    if (isPopular) cityTags.add('popular');
    if (isFeatured) cityTags.add('featured');
    if (hasWeatherData) cityTags.add('weather_available');
    if (hasAttractions) cityTags.add('attractions_available');
    if (averageRating >= 4.5) cityTags.add('highly_rated');
    if (propertyCount > 50) cityTags.add('many_properties');
    return cityTags;
  }

  Map<String, dynamic> get analyticsData => {
        'destination_id': id,
        'destination_name': name,
        'country': country,
        'property_count': propertyCount,
        'average_price': averagePrice,
        'average_rating': averageRating,
        'review_count': reviewCount,
        'is_popular': isPopular,
        'is_featured': isFeatured,
        'priority': priority,
        'popularity_level': popularityLevel,
        'tags': tags,
      };

  @override
  List<Object?> get props => [
        id,
        name,
        nameAr,
        country,
        countryAr,
        description,
        descriptionAr,
        imageUrl,
        additionalImages,
        latitude,
        longitude,
        propertyCount,
        averagePrice,
        currency,
        averageRating,
        reviewCount,
        isPopular,
        isFeatured,
        priority,
        highlights,
        highlightsAr,
        weatherData,
        attractionsData,
        metadata,
      ];
}