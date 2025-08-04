// lib/features/home/data/models/home_config_model.dart

import 'package:equatable/equatable.dart';
import '../../domain/entities/home_config.dart';

class HomeConfigModel extends Equatable {
  final String id;
  final String version;
  final bool isActive;
  final DateTime createdAt;
  final DateTime updatedAt;
  final DateTime? publishedAt;
  final Map<String, dynamic> globalSettings;
  final Map<String, dynamic> themeSettings;
  final Map<String, dynamic> layoutSettings;
  final Map<String, dynamic> cacheSettings;
  final Map<String, dynamic> analyticsSettings;
  final List<String> enabledFeatures;
  final Map<String, dynamic> experimentalFeatures;

  const HomeConfigModel({
    required this.id,
    required this.version,
    required this.isActive,
    required this.createdAt,
    required this.updatedAt,
    this.publishedAt,
    required this.globalSettings,
    required this.themeSettings,
    required this.layoutSettings,
    required this.cacheSettings,
    required this.analyticsSettings,
    required this.enabledFeatures,
    required this.experimentalFeatures,
  });

  bool get isPublished => publishedAt != null && isActive;

  // Global settings getters
  int get refreshInterval => globalSettings['refreshInterval'] as int? ?? 300;
  bool get enablePullToRefresh => globalSettings['enablePullToRefresh'] as bool? ?? true;
  bool get enableInfiniteScroll => globalSettings['enableInfiniteScroll'] as bool? ?? false;
  int get maxConcurrentRequests => globalSettings['maxConcurrentRequests'] as int? ?? 5;
  
  // Theme settings getters
  String get primaryColor => themeSettings['primaryColor'] as String? ?? '#007AFF';
  String get accentColor => themeSettings['accentColor'] as String? ?? '#FF3B30';
  bool get enableDarkMode => themeSettings['enableDarkMode'] as bool? ?? false;
  String get fontFamily => themeSettings['fontFamily'] as String? ?? 'Cairo';
  
  // Layout settings getters
  double get sectionSpacing => (layoutSettings['sectionSpacing'] as num?)?.toDouble() ?? 16.0;
  double get itemSpacing => (layoutSettings['itemSpacing'] as num?)?.toDouble() ?? 8.0;
  double get borderRadius => (layoutSettings['borderRadius'] as num?)?.toDouble() ?? 12.0;
  bool get enableSectionHeaders => layoutSettings['enableSectionHeaders'] as bool? ?? true;
  
  // Cache settings getters
  int get cacheMaxAge => cacheSettings['maxAge'] as int? ?? 3600;
  int get cacheMaxSize => cacheSettings['maxSize'] as int? ?? 100;
  bool get enableOfflineMode => cacheSettings['enableOfflineMode'] as bool? ?? true;
  
  // Analytics settings getters
  bool get enableAnalytics => analyticsSettings['enabled'] as bool? ?? true;
  bool get trackImpressions => analyticsSettings['trackImpressions'] as bool? ?? true;
  bool get trackInteractions => analyticsSettings['trackInteractions'] as bool? ?? true;

  factory HomeConfigModel.fromJson(Map<String, dynamic> json) {
    return HomeConfigModel(
      id: json['id'] as String,
      version: json['version'] as String,
      isActive: json['isActive'] as bool,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      publishedAt: json['publishedAt'] != null
          ? DateTime.parse(json['publishedAt'] as String)
          : null,
      globalSettings: json['globalSettings'] as Map<String, dynamic>? ?? {},
      themeSettings: json['themeSettings'] as Map<String, dynamic>? ?? {},
      layoutSettings: json['layoutSettings'] as Map<String, dynamic>? ?? {},
      cacheSettings: json['cacheSettings'] as Map<String, dynamic>? ?? {},
      analyticsSettings: json['analyticsSettings'] as Map<String, dynamic>? ?? {},
      enabledFeatures: (json['enabledFeatures'] as List<dynamic>?)?.cast<String>() ?? [],
      experimentalFeatures: json['experimentalFeatures'] as Map<String, dynamic>? ?? {},
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'version': version,
      'isActive': isActive,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'publishedAt': publishedAt?.toIso8601String(),
      'globalSettings': globalSettings,
      'themeSettings': themeSettings,
      'layoutSettings': layoutSettings,
      'cacheSettings': cacheSettings,
      'analyticsSettings': analyticsSettings,
      'enabledFeatures': enabledFeatures,
      'experimentalFeatures': experimentalFeatures,
    };
  }

  HomeConfig toEntity() {
    return HomeConfig(
      id: id,
      version: version,
      isActive: isActive,
      publishedAt: publishedAt,
      globalSettings: globalSettings,
      themeSettings: themeSettings,
      layoutSettings: layoutSettings,
      cacheSettings: cacheSettings,
      analyticsSettings: analyticsSettings,
      enabledFeatures: enabledFeatures,
      experimentalFeatures: experimentalFeatures,
    );
  }

  HomeConfigModel copyWith({
    String? id,
    String? version,
    bool? isActive,
    DateTime? createdAt,
    DateTime? updatedAt,
    DateTime? publishedAt,
    Map<String, dynamic>? globalSettings,
    Map<String, dynamic>? themeSettings,
    Map<String, dynamic>? layoutSettings,
    Map<String, dynamic>? cacheSettings,
    Map<String, dynamic>? analyticsSettings,
    List<String>? enabledFeatures,
    Map<String, dynamic>? experimentalFeatures,
  }) {
    return HomeConfigModel(
      id: id ?? this.id,
      version: version ?? this.version,
      isActive: isActive ?? this.isActive,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      publishedAt: publishedAt ?? this.publishedAt,
      globalSettings: globalSettings ?? this.globalSettings,
      themeSettings: themeSettings ?? this.themeSettings,
      layoutSettings: layoutSettings ?? this.layoutSettings,
      cacheSettings: cacheSettings ?? this.cacheSettings,
      analyticsSettings: analyticsSettings ?? this.analyticsSettings,
      enabledFeatures: enabledFeatures ?? this.enabledFeatures,
      experimentalFeatures: experimentalFeatures ?? this.experimentalFeatures,
    );
  }

  @override
  List<Object?> get props => [
        id,
        version,
        isActive,
        createdAt,
        updatedAt,
        publishedAt,
        globalSettings,
        themeSettings,
        layoutSettings,
        cacheSettings,
        analyticsSettings,
        enabledFeatures,
        experimentalFeatures,
      ];
}