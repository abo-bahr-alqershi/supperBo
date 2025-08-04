// lib/features/home/domain/entities/featured_property.dart

import 'package:equatable/equatable.dart';

class FeaturedProperty extends Equatable {
  final String id;
  final String name;
  final String address;
  final String city;
  final double latitude;
  final double longitude;
  final int starRating;
  final String description;
  final List<String> images;
  final double? basePrice;
  final String? currency;
  final List<String> amenities;
  final double averageRating;
  final int viewCount;
  final int bookingCount;
  final String? mainImageUrl;
  final bool isFeatured;
  final double? discountPercentage;
  final String? propertyType;
  final String? featuredReason;
  final DateTime? featuredDate;
  final DateTime? featuredUntil;
  final String? badgeText;
  final String? badgeColor;
  final String? promotionalMessage;

  const FeaturedProperty({
    required this.id,
    required this.name,
    required this.address,
    required this.city,
    required this.latitude,
    required this.longitude,
    required this.starRating,
    required this.description,
    required this.images,
    this.basePrice,
    this.currency,
    required this.amenities,
    required this.averageRating,
    required this.viewCount,
    required this.bookingCount,
    this.mainImageUrl,
    this.isFeatured = false,
    this.discountPercentage,
    this.propertyType,
    this.featuredReason,
    this.featuredDate,
    this.featuredUntil,
    this.badgeText,
    this.badgeColor,
    this.promotionalMessage,
  });

  String get displayImage => mainImageUrl ?? (images.isNotEmpty ? images.first : '');
  
  bool get hasDiscount => discountPercentage != null && discountPercentage! > 0;
  
  double? get discountedPrice {
    if (basePrice == null || !hasDiscount) return null;
    return basePrice! * (1 - (discountPercentage! / 100));
  }

  String get fullAddress => '$address, $city';
  
  bool get isCurrentlyFeatured {
    if (!isFeatured) return false;
    if (featuredUntil == null) return true;
    return DateTime.now().isBefore(featuredUntil!);
  }

  String get formattedRating => averageRating.toStringAsFixed(1);
  
  String get formattedPrice {
    if (basePrice == null) return '';
    final price = hasDiscount ? discountedPrice! : basePrice!;
    return '${price.toStringAsFixed(0)} ${currency ?? ''}';
  }

  String get formattedOriginalPrice {
    if (basePrice == null || !hasDiscount) return '';
    return '${basePrice!.toStringAsFixed(0)} ${currency ?? ''}';
  }

  List<String> get topAmenities => amenities.take(3).toList();

  @override
  List<Object?> get props => [
        id,
        name,
        address,
        city,
        latitude,
        longitude,
        starRating,
        description,
        images,
        basePrice,
        currency,
        amenities,
        averageRating,
        viewCount,
        bookingCount,
        mainImageUrl,
        isFeatured,
        discountPercentage,
        propertyType,
        featuredReason,
        featuredDate,
        featuredUntil,
        badgeText,
        badgeColor,
        promotionalMessage,
      ];
}