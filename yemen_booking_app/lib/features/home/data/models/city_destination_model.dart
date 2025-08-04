// lib/features/home/data/models/city_destination_model.dart

import 'package:equatable/equatable.dart';
import '../../domain/entities/city_destination.dart';

class CityDestinationModel extends Equatable {
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
  final DateTime createdAt;
  final DateTime updatedAt;
  final bool isActive;

  const CityDestinationModel({
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
    required this.createdAt,
    required this.updatedAt,
    this.isActive = true,
  });

  String getLocalizedName(bool isArabic) => isArabic ? nameAr : name;
  String getLocalizedCountry(bool isArabic) => isArabic ? countryAr : country;
  String? getLocalizedDescription(bool isArabic) => isArabic ? descriptionAr : description;
  List<String> getLocalizedHighlights(bool isArabic) => isArabic ? highlightsAr : highlights;

  String get fullName => '$name, $country';
  String get fullNameAr => '$nameAr، $countryAr';
  String getLocalizedFullName(bool isArabic) => isArabic ? fullNameAr : fullName;

  bool get hasWeatherData => weatherData.isNotEmpty;
  bool get hasAttractions => attractionsData.isNotEmpty;

  // Weather data getters
  double? get currentTemperature => weatherData['temperature'] as double?;
  String? get weatherCondition => weatherData['condition'] as String?;
  String? get weatherIcon => weatherData['icon'] as String?;

  // Attractions data getters
  List<String> get popularAttractions {
    final attractions = attractionsData['popular'] as List<dynamic>?;
    return attractions?.cast<String>() ?? [];
  }

  factory CityDestinationModel.fromJson(Map<String, dynamic> json) {
    return CityDestinationModel(
      id: json['id'] as String,
      name: json['name'] as String,
      nameAr: json['nameAr'] as String,
      country: json['country'] as String,
      countryAr: json['countryAr'] as String,
      description: json['description'] as String?,
      descriptionAr: json['descriptionAr'] as String?,
      imageUrl: json['imageUrl'] as String,
      additionalImages: (json['additionalImages'] as List<dynamic>?)?.cast<String>() ?? [],
      latitude: (json['latitude'] as num).toDouble(),
      longitude: (json['longitude'] as num).toDouble(),
      propertyCount: json['propertyCount'] as int,
      averagePrice: (json['averagePrice'] as num).toDouble(),
      currency: json['currency'] as String,
      averageRating: (json['averageRating'] as num).toDouble(),
      reviewCount: json['reviewCount'] as int,
      isPopular: json['isPopular'] as bool? ?? false,
      isFeatured: json['isFeatured'] as bool? ?? false,
      priority: json['priority'] as int? ?? 0,
      highlights: (json['highlights'] as List<dynamic>?)?.cast<String>() ?? [],
      highlightsAr: (json['highlightsAr'] as List<dynamic>?)?.cast<String>() ?? [],
      weatherData: json['weatherData'] as Map<String, dynamic>? ?? {},
      attractionsData: json['attractionsData'] as Map<String, dynamic>? ?? {},
      metadata: json['metadata'] as Map<String, dynamic>? ?? {},
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      isActive: json['isActive'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'nameAr': nameAr,
      'country': country,
      'countryAr': countryAr,
      'description': description,
      'descriptionAr': descriptionAr,
      'imageUrl': imageUrl,
      'additionalImages': additionalImages,
      'latitude': latitude,
      'longitude': longitude,
      'propertyCount': propertyCount,
      'averagePrice': averagePrice,
      'currency': currency,
      'averageRating': averageRating,
      'reviewCount': reviewCount,
      'isPopular': isPopular,
      'isFeatured': isFeatured,
      'priority': priority,
      'highlights': highlights,
      'highlightsAr': highlightsAr,
      'weatherData': weatherData,
      'attractionsData': attractionsData,
      'metadata': metadata,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'isActive': isActive,
    };
  }

  CityDestination toEntity() {
    return CityDestination(
      id: id,
      name: name,
      nameAr: nameAr,
      country: country,
      countryAr: countryAr,
      description: description,
      descriptionAr: descriptionAr,
      imageUrl: imageUrl,
      additionalImages: additionalImages,
      latitude: latitude,
      longitude: longitude,
      propertyCount: propertyCount,
      averagePrice: averagePrice,
      currency: currency,
      averageRating: averageRating,
      reviewCount: reviewCount,
      isPopular: isPopular,
      isFeatured: isFeatured,
      priority: priority,
      highlights: highlights,
      highlightsAr: highlightsAr,
      weatherData: weatherData,
      attractionsData: attractionsData,
      metadata: metadata,
    );
  }

  CityDestinationModel copyWith({
    String? id,
    String? name,
    String? nameAr,
    String? country,
    String? countryAr,
    String? description,
    String? descriptionAr,
    String? imageUrl,
    List<String>? additionalImages,
    double? latitude,
    double? longitude,
    int? propertyCount,
    double? averagePrice,
    String? currency,
    double? averageRating,
    int? reviewCount,
    bool? isPopular,
    bool? isFeatured,
    int? priority,
    List<String>? highlights,
    List<String>? highlightsAr,
    Map<String, dynamic>? weatherData,
    Map<String, dynamic>? attractionsData,
    Map<String, dynamic>? metadata,
    DateTime? createdAt,
    DateTime? updatedAt,
    bool? isActive,
  }) {
    return CityDestinationModel(
      id: id ?? this.id,
      name: name ?? this.name,
      nameAr: nameAr ?? this.nameAr,
      country: country ?? this.country,
      countryAr: countryAr ?? this.countryAr,
      description: description ?? this.description,
      descriptionAr: descriptionAr ?? this.descriptionAr,
      imageUrl: imageUrl ?? this.imageUrl,
      additionalImages: additionalImages ?? this.additionalImages,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      propertyCount: propertyCount ?? this.propertyCount,
      averagePrice: averagePrice ?? this.averagePrice,
      currency: currency ?? this.currency,
      averageRating: averageRating ?? this.averageRating,
      reviewCount: reviewCount ?? this.reviewCount,
      isPopular: isPopular ?? this.isPopular,
      isFeatured: isFeatured ?? this.isFeatured,
      priority: priority ?? this.priority,
      highlights: highlights ?? this.highlights,
      highlightsAr: highlightsAr ?? this.highlightsAr,
      weatherData: weatherData ?? this.weatherData,
      attractionsData: attractionsData ?? this.attractionsData,
      metadata: metadata ?? this.metadata,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      isActive: isActive ?? this.isActive,
    );
  }

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
        createdAt,
        updatedAt,
        isActive,
      ];
}