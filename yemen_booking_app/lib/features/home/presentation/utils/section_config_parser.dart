// lib/features/home/presentation/widgets/utils/section_config_parser.dart

import '../../domain/entities/section_config.dart';

class SectionConfigParser {
  static Map<String, dynamic> parseConfig(SectionConfig config) {
    return {
      'display': config.displaySettings,
      'layout': config.layoutSettings,
      'style': config.styleSettings,
      'behavior': config.behaviorSettings,
      'animation': config.animationSettings,
      'cache': config.cacheSettings,
    };
  }

  static T? getValue<T>(SectionConfig config, String key) {
    final allSettings = {
      ...config.displaySettings,
      ...config.layoutSettings,
      ...config.styleSettings,
      ...config.behaviorSettings,
      ...config.animationSettings,
      ...config.cacheSettings,
      ...config.customData,
    };
    
    return allSettings[key] as T?;
  }
}