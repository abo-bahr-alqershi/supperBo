// lib/features/home/data/models/featured_property_model.dart

import 'package:equatable/equatable.dart';
import '../../domain/entities/featured_property.dart';
import 'property_model.dart';

class FeaturedPropertyModel extends Equatable {
  final PropertyModel property;
  final String featuredReason;
  final DateTime featuredDate;
  final DateTime? featuredUntil;
  final int featuredPriority;
  final Map<String, dynamic> featuredMetadata;
  final bool isActive;
  final String? badgeText;
  final String? badgeColor;
  final double? promotionalPrice;
  final String? promotionalMessage;

  const FeaturedPropertyModel({
    required this.property,
    required this.featuredReason,
    required this.featuredDate,
    this.featuredUntil,
    required this.featuredPriority,
    required this.featuredMetadata,
    this.isActive = true,
    this.badgeText,
    this.badgeColor,
    this.promotionalPrice,
    this.promotionalMessage,
  });

  bool get isExpired {
    if (featuredUntil == null) return false;
    return DateTime.now().isAfter(featuredUntil!);
  }

  bool get isCurrentlyFeatured => isActive && !isExpired;

  factory FeaturedPropertyModel.fromJson(Map<String, dynamic> json) {
    return FeaturedPropertyModel(
      property: PropertyModel.fromJson(json['property'] as Map<String, dynamic>),
      featuredReason: json['featuredReason'] as String,
      featuredDate: DateTime.parse(json['featuredDate'] as String),
      featuredUntil: json['featuredUntil'] != null
          ? DateTime.parse(json['featuredUntil'] as String)
          : null,
      featuredPriority: json['featuredPriority'] as int,
      featuredMetadata: json['featuredMetadata'] as Map<String, dynamic>? ?? {},
      isActive: json['isActive'] as bool? ?? true,
      badgeText: json['badgeText'] as String?,
      badgeColor: json['badgeColor'] as String?,
      promotionalPrice: json['promotionalPrice'] != null
          ? (json['promotionalPrice'] as num).toDouble()
          : null,
      promotionalMessage: json['promotionalMessage'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'property': property.toJson(),
      'featuredReason': featuredReason,
      'featuredDate': featuredDate.toIso8601String(),
      'featuredUntil': featuredUntil?.toIso8601String(),
      'featuredPriority': featuredPriority,
      'featuredMetadata': featuredMetadata,
      'isActive': isActive,
      'badgeText': badgeText,
      'badgeColor': badgeColor,
      'promotionalPrice': promotionalPrice,
      'promotionalMessage': promotionalMessage,
    };
  }

  FeaturedProperty toEntity() {
    return FeaturedProperty(
      id: property.id,
      name: property.name,
      address: property.address,
      city: property.city,
      latitude: property.latitude,
      longitude: property.longitude,
      starRating: property.starRating,
      description: property.description,
      images: property.images,
      basePrice: promotionalPrice ?? property.basePrice,
      currency: property.currency,
      amenities: property.amenities,
      averageRating: property.averageRating,
      viewCount: property.viewCount,
      bookingCount: property.bookingCount,
      mainImageUrl: property.mainImageUrl,
      isFeatured: true,
      discountPercentage: property.discountPercentage,
      propertyType: property.propertyType,
      featuredReason: featuredReason,
      featuredDate: featuredDate,
      featuredUntil: featuredUntil,
      badgeText: badgeText,
      badgeColor: badgeColor,
      promotionalMessage: promotionalMessage,
    );
  }

  FeaturedPropertyModel copyWith({
    PropertyModel? property,
    String? featuredReason,
    DateTime? featuredDate,
    DateTime? featuredUntil,
    int? featuredPriority,
    Map<String, dynamic>? featuredMetadata,
    bool? isActive,
    String? badgeText,
    String? badgeColor,
    double? promotionalPrice,
    String? promotionalMessage,
  }) {
    return FeaturedPropertyModel(
      property: property ?? this.property,
      featuredReason: featuredReason ?? this.featuredReason,
      featuredDate: featuredDate ?? this.featuredDate,
      featuredUntil: featuredUntil ?? this.featuredUntil,
      featuredPriority: featuredPriority ?? this.featuredPriority,
      featuredMetadata: featuredMetadata ?? this.featuredMetadata,
      isActive: isActive ?? this.isActive,
      badgeText: badgeText ?? this.badgeText,
      badgeColor: badgeColor ?? this.badgeColor,
      promotionalPrice: promotionalPrice ?? this.promotionalPrice,
      promotionalMessage: promotionalMessage ?? this.promotionalMessage,
    );
  }

  @override
  List<Object?> get props => [
        property,
        featuredReason,
        featuredDate,
        featuredUntil,
        featuredPriority,
        featuredMetadata,
        isActive,
        badgeText,
        badgeColor,
        promotionalPrice,
        promotionalMessage,
      ];
}