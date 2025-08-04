// lib/core/models/home_section_base_model.dart

import 'package:equatable/equatable.dart';

abstract class HomeSectionBaseModel extends Equatable {
  final String id;
  final String type;
  final int order;
  final bool isActive;
  final Map<String, dynamic> config;
  final DateTime? createdAt;
  final DateTime? updatedAt;

  const HomeSectionBaseModel({
    required this.id,
    required this.type,
    required this.order,
    required this.isActive,
    required this.config,
    this.createdAt,
    this.updatedAt,
  });

  // Common configuration getters
  String? get title => config['title'] as String?;
  String? get subtitle => config['subtitle'] as String?;
  String? get backgroundColor => config['backgroundColor'] as String?;
  String? get textColor => config['textColor'] as String?;
  String? get customImage => config['customImage'] as String?;
  List<String>? get propertyIds => 
      (config['propertyIds'] as List<dynamic>?)?.cast<String>();
  int get maxItems => config['maxItems'] as int? ?? 10;
  bool get autoPlay => config['autoPlay'] as bool? ?? true;
  int get autoPlayDuration => config['autoPlayDuration'] as int? ?? 5;
  String? get animationType => config['animationType'] as String?;
  String? get layoutType => config['layoutType'] as String?;
  String? get theme => config['theme'] as String?;
  
  // Validation
  bool get isValid => id.isNotEmpty && type.isNotEmpty && order >= 0;
  
  // Check if section needs data
  bool get requiresData => propertyIds != null || hasDataSource;
  
  // Check if section has a data source
  bool get hasDataSource => config['dataSource'] != null;
  
  // Get safe configuration value
  T? getConfigValue<T>(String key) {
    try {
      return config[key] as T?;
    } catch (_) {
      return null;
    }
  }
  
  // Get configuration with default
  T getConfigValueOrDefault<T>(String key, T defaultValue) {
    try {
      return config[key] as T? ?? defaultValue;
    } catch (_) {
      return defaultValue;
    }
  }
  
  // Copy with new configuration
  HomeSectionBaseModel copyWithConfig(Map<String, dynamic> newConfig);
  
  // Convert to JSON
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'type': type,
      'order': order,
      'isActive': isActive,
      'config': config,
      'createdAt': createdAt?.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
    };
  }

  @override
  List<Object?> get props => [
        id,
        type,
        order,
        isActive,
        config,
        createdAt,
        updatedAt,
      ];
}

// Factory constructor helper
class HomeSectionFactory {
  static HomeSectionBaseModel? fromJson(Map<String, dynamic> json) {
    final type = json['type'] as String?;
    if (type == null) return null;
    
    // This will be implemented when we have specific section models
    // For now, return a generic implementation
    return GenericHomeSection.fromJson(json);
  }
}

// Generic implementation for unknown types
class GenericHomeSection extends HomeSectionBaseModel {
  const GenericHomeSection({
    required super.id,
    required super.type,
    required super.order,
    required super.isActive,
    required super.config,
    super.createdAt,
    super.updatedAt,
  });

  factory GenericHomeSection.fromJson(Map<String, dynamic> json) {
    return GenericHomeSection(
      id: json['id'] as String,
      type: json['type'] as String,
      order: json['order'] as int,
      isActive: json['isActive'] as bool,
      config: json['config'] as Map<String, dynamic>,
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'] as String)
          : null,
      updatedAt: json['updatedAt'] != null
          ? DateTime.parse(json['updatedAt'] as String)
          : null,
    );
  }

  @override
  HomeSectionBaseModel copyWithConfig(Map<String, dynamic> newConfig) {
    return GenericHomeSection(
      id: id,
      type: type,
      order: order,
      isActive: isActive,
      config: {...config, ...newConfig},
      createdAt: createdAt,
      updatedAt: updatedAt,
    );
  }
}