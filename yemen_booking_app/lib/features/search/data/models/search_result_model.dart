import '../../domain/entities/search_result.dart';

class SearchResultModel extends SearchResult {
  const SearchResultModel({
    required super.id,
    required super.name,
    required super.description,
    required super.address,
    required super.city,
    required super.starRating,
    required super.averageRating,
    required super.reviewsCount,
    required super.minPrice,
    required super.discountedPrice,
    required super.currency,
    super.mainImageUrl,
    required super.isRecommended,
    super.distanceKm,
    required super.latitude,
    required super.longitude,
    super.unitId,
    required super.dynamicFieldValues,
    required super.isAvailable,
    required super.availableUnitsCount,
    required super.propertyType,
    required super.isFeatured,
    required super.mainAmenities,
    required super.reviews,
  });

  factory SearchResultModel.fromJson(Map<String, dynamic> json) {
    return SearchResultModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      address: json['address'] ?? '',
      city: json['city'] ?? '',
      starRating: json['starRating'] ?? 0,
      averageRating: (json['averageRating'] ?? 0).toDouble(),
      reviewsCount: json['reviewsCount'] ?? 0,
      minPrice: (json['minPrice'] ?? 0).toDouble(),
      discountedPrice: (json['discountedPrice'] ?? 0).toDouble(),
      currency: json['currency'] ?? 'YER',
      mainImageUrl: json['mainImageUrl'],
      isRecommended: json['isRecommended'] ?? false,
      distanceKm: json['distanceKm']?.toDouble(),
      latitude: (json['latitude'] ?? 0).toDouble(),
      longitude: (json['longitude'] ?? 0).toDouble(),
      unitId: json['unitId'],
      dynamicFieldValues: Map<String, dynamic>.from(json['dynamicFieldValues'] ?? {}),
      isAvailable: json['isAvailable'] ?? true,
      availableUnitsCount: json['availableUnitsCount'] ?? 0,
      propertyType: json['propertyType'] ?? '',
      isFeatured: json['isFeatured'] ?? false,
      mainAmenities: List<String>.from(json['mainAmenities'] ?? []),
      reviews: (json['reviews'] as List?)
              ?.map((e) => ReviewModel.fromJson(e))
              .toList() ??
          [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'address': address,
      'city': city,
      'starRating': starRating,
      'averageRating': averageRating,
      'reviewsCount': reviewsCount,
      'minPrice': minPrice,
      'discountedPrice': discountedPrice,
      'currency': currency,
      'mainImageUrl': mainImageUrl,
      'isRecommended': isRecommended,
      'distanceKm': distanceKm,
      'latitude': latitude,
      'longitude': longitude,
      'unitId': unitId,
      'dynamicFieldValues': dynamicFieldValues,
      'isAvailable': isAvailable,
      'availableUnitsCount': availableUnitsCount,
      'propertyType': propertyType,
      'isFeatured': isFeatured,
      'mainAmenities': mainAmenities,
      'reviews': reviews.map((e) => (e as ReviewModel).toJson()).toList(),
    };
  }
}

class ReviewModel extends Review {
  const ReviewModel({
    required super.id,
    required super.userId,
    required super.userName,
    required super.rating,
    super.comment,
    required super.createdAt,
  });

  factory ReviewModel.fromJson(Map<String, dynamic> json) {
    return ReviewModel(
      id: json['id'] ?? '',
      userId: json['userId'] ?? '',
      userName: json['userName'] ?? '',
      rating: (json['rating'] ?? 0).toDouble(),
      comment: json['comment'],
      createdAt: DateTime.parse(json['createdAt'] ?? DateTime.now().toIso8601String()),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'userName': userName,
      'rating': rating,
      'comment': comment,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}