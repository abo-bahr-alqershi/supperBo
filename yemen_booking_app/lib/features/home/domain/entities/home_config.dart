// lib/features/home/domain/entities/home_config.dart

import 'package:equatable/equatable.dart';

class HomeConfig extends Equatable {
  final String id;
  final String version;
  final bool isActive;
  final DateTime? publishedAt;
  final Map<String, dynamic> globalSettings;
  final Map<String, dynamic> themeSettings;
  final Map<String, dynamic> layoutSettings;
  final Map<String, dynamic> cacheSettings;
  final Map<String, dynamic> analyticsSettings;
  final List<String> enabledFeatures;
  final Map<String, dynamic> experimentalFeatures;

  const HomeConfig({
    required this.id,
    required this.version,
    required this.isActive,
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

  bool isFeatureEnabled(String feature) => enabledFeatures.contains(feature);

  bool isExperimentalFeatureEnabled(String feature) {
    return experimentalFeatures[feature] as bool? ?? false;
  }

  T? getGlobalSetting<T>(String key) {
    try {
      return globalSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getThemeSetting<T>(String key) {
    try {
      return themeSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getLayoutSetting<T>(String key) {
    try {
      return layoutSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getCacheSetting<T>(String key) {
    try {
      return cacheSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getAnalyticsSetting<T>(String key) {
    try {
      return analyticsSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getExperimentalFeature<T>(String key) {
    try {
      return experimentalFeatures[key] as T?;
    } catch (_) {
      return null;
    }
  }

  @override
  List<Object?> get props => [
        id,
        version,
        isActive,
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