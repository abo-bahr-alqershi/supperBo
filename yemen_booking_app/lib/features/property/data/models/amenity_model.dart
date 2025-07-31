import '../../domain/entities/amenity.dart';

class AmenityModel extends Amenity {
  const AmenityModel({
    required super.id,
    required super.name,
    required super.description,
    required super.iconUrl,
    required super.category,
    required super.isActive,
    required super.displayOrder,
    required super.createdAt,
  });

  factory AmenityModel.fromJson(Map<String, dynamic> json) {
    return AmenityModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      iconUrl: json['iconUrl'] ?? '',
      category: json['category'] ?? '',
      isActive: json['isActive'] ?? true,
      displayOrder: json['displayOrder'] ?? 0,
      createdAt: DateTime.parse(json['createdAt'] ?? DateTime.now().toIso8601String()),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'iconUrl': iconUrl,
      'category': category,
      'isActive': isActive,
      'displayOrder': displayOrder,
      'createdAt': createdAt.toIso8601String(),
    };
  }

  factory AmenityModel.fromEntity(Amenity amenity) {
    return AmenityModel(
      id: amenity.id,
      name: amenity.name,
      description: amenity.description,
      iconUrl: amenity.iconUrl,
      category: amenity.category,
      isActive: amenity.isActive,
      displayOrder: amenity.displayOrder,
      createdAt: amenity.createdAt,
    );
  }
}