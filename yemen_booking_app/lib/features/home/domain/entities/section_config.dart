// lib/features/home/domain/entities/section_config.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart';
import '../../../../core/enums/section_size_enum.dart';
import '../../../../core/enums/section_animation_enum.dart';

class SectionConfig extends Equatable {
  final String id;
  final SectionType sectionType;
  final Map<String, dynamic> displaySettings;
  final Map<String, dynamic> layoutSettings;
  final Map<String, dynamic> styleSettings;
  final Map<String, dynamic> behaviorSettings;
  final Map<String, dynamic> animationSettings;
  final Map<String, dynamic> cacheSettings;
  final List<String> propertyIds;
  final String? title;
  final String? titleAr;
  final String? subtitle;
  final String? subtitleAr;
  final String? backgroundColor;
  final String? textColor;
  final String? customImage;
  final Map<String, dynamic> customData;

  const SectionConfig({
    required this.id,
    required this.sectionType,
    required this.displaySettings,
    required this.layoutSettings,
    required this.styleSettings,
    required this.behaviorSettings,
    required this.animationSettings,
    required this.cacheSettings,
    required this.propertyIds,
    this.title,
    this.titleAr,
    this.subtitle,
    this.subtitleAr,
    this.backgroundColor,
    this.textColor,
    this.customImage,
    required this.customData,
  });

  // Display settings getters
  int get maxItems => displaySettings['maxItems'] as int? ?? 10;
  bool get showTitle => displaySettings['showTitle'] as bool? ?? true;
  bool get showSubtitle => displaySettings['showSubtitle'] as bool? ?? false;
  bool get showBadge => displaySettings['showBadge'] as bool? ?? false;
  String? get badgeText => displaySettings['badgeText'] as String?;
  bool get showIndicators => displaySettings['showIndicators'] as bool? ?? false;
  bool get showViewAllButton => displaySettings['showViewAllButton'] as bool? ?? false;

  // Layout settings getters
  String get layoutType => layoutSettings['layoutType'] as String? ?? 'horizontal';
  int get columnsCount => layoutSettings['columnsCount'] as int? ?? 2;
  double get itemHeight => (layoutSettings['itemHeight'] as num?)?.toDouble() ?? 200.0;
  double get itemSpacing => (layoutSettings['itemSpacing'] as num?)?.toDouble() ?? 8.0;
  double get sectionPadding => (layoutSettings['sectionPadding'] as num?)?.toDouble() ?? 16.0;
  SectionSize get sectionSize => SectionSize.tryFromString(layoutSettings['sectionSize'] as String?) ?? SectionSize.medium;

  // Style settings getters
  double get borderRadius => (styleSettings['borderRadius'] as num?)?.toDouble() ?? 12.0;
  double get elevation => (styleSettings['elevation'] as num?)?.toDouble() ?? 2.0;
  bool get enableGradient => styleSettings['enableGradient'] as bool? ?? false;
  List<String> get gradientColors {
    final colors = styleSettings['gradientColors'] as List<dynamic>?;
    return colors?.cast<String>() ?? [];
  }

  // Behavior settings getters
  bool get autoPlay => behaviorSettings['autoPlay'] as bool? ?? false;
  int get autoPlayDuration => behaviorSettings['autoPlayDuration'] as int? ?? 5;
  bool get infiniteScroll => behaviorSettings['infiniteScroll'] as bool? ?? true;
  bool get enablePullToRefresh => behaviorSettings['enablePullToRefresh'] as bool? ?? true;
  bool get lazy => behaviorSettings['lazy'] as bool? ?? true;

  // Animation settings getters
  SectionAnimation get animationType => SectionAnimation.tryFromString(animationSettings['animationType'] as String?) ?? SectionAnimation.fade;
  int get animationDuration => animationSettings['animationDuration'] as int? ?? 300;
  bool get parallaxEnabled => animationSettings['parallaxEnabled'] as bool? ?? false;
  bool get enableHeroAnimation => animationSettings['enableHeroAnimation'] as bool? ?? false;

  // Cache settings getters
  bool get enableCache => cacheSettings['enableCache'] as bool? ?? true;
  int get cacheMaxAge => cacheSettings['maxAge'] as int? ?? 3600;
  bool get cacheImages => cacheSettings['cacheImages'] as bool? ?? true;

  String getLocalizedTitle(bool isArabic) => isArabic ? (titleAr ?? title ?? '') : (title ?? '');
  String getLocalizedSubtitle(bool isArabic) => isArabic ? (subtitleAr ?? subtitle ?? '') : (subtitle ?? '');

  bool get hasCustomTitle => title != null || titleAr != null;
  bool get hasCustomSubtitle => subtitle != null || subtitleAr != null;
  bool get hasCustomImage => customImage != null && customImage!.isNotEmpty;
  bool get hasCustomStyling => backgroundColor != null || textColor != null || styleSettings.isNotEmpty;
  bool get hasCustomBehavior => behaviorSettings.isNotEmpty;
  bool get hasAnimation => animationSettings.isNotEmpty && animationType != SectionAnimation.none;

  bool get isHorizontalLayout => layoutType == 'horizontal' || layoutType == 'carousel';
  bool get isVerticalLayout => layoutType == 'vertical' || layoutType == 'list';
  bool get isGridLayout => layoutType == 'grid';
  bool get isMixedLayout => layoutType == 'mixed';

  int get effectiveItemCount {
    if (propertyIds.isEmpty) return maxItems;
    return propertyIds.length.clamp(1, maxItems);
  }

  // Helper methods for specific settings
  T? getDisplaySetting<T>(String key) {
    try {
      return displaySettings[key] as T?;
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

  T? getStyleSetting<T>(String key) {
    try {
      return styleSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getBehaviorSetting<T>(String key) {
    try {
      return behaviorSettings[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getAnimationSetting<T>(String key) {
    try {
      return animationSettings[key] as T?;
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

  T? getCustomData<T>(String key) {
    try {
      return customData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  // Configuration validation
  bool get isValidConfiguration {
    return maxItems > 0 &&
           itemHeight > 0 &&
           itemSpacing >= 0 &&
           sectionPadding >= 0 &&
           borderRadius >= 0 &&
           elevation >= 0 &&
           animationDuration > 0 &&
           cacheMaxAge > 0;
  }

  // Merge with default configuration
  SectionConfig mergeWithDefaults(Map<String, dynamic> defaults) {
    return SectionConfig(
      id: id,
      sectionType: sectionType,
      displaySettings: {...defaults['displaySettings'] ?? {}, ...displaySettings},
      layoutSettings: {...defaults['layoutSettings'] ?? {}, ...layoutSettings},
      styleSettings: {...defaults['styleSettings'] ?? {}, ...styleSettings},
      behaviorSettings: {...defaults['behaviorSettings'] ?? {}, ...behaviorSettings},
      animationSettings: {...defaults['animationSettings'] ?? {}, ...animationSettings},
      cacheSettings: {...defaults['cacheSettings'] ?? {}, ...cacheSettings},
      propertyIds: propertyIds,
      title: title,
      titleAr: titleAr,
      subtitle: subtitle,
      subtitleAr: subtitleAr,
      backgroundColor: backgroundColor,
      textColor: textColor,
      customImage: customImage,
      customData: {...defaults['customData'] ?? {}, ...customData},
    );
  }

  @override
  List<Object?> get props => [
        id,
        sectionType,
        displaySettings,
        layoutSettings,
        styleSettings,
        behaviorSettings,
        animationSettings,
        cacheSettings,
        propertyIds,
        title,
        titleAr,
        subtitle,
        subtitleAr,
        backgroundColor,
        textColor,
        customImage,
        customData,
      ];
}