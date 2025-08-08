// lib/features/home/data/models/section_config_model.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart' as core_enum;
import '../../../../core/enums/section_size_enum.dart';
import '../../../../core/enums/section_animation_enum.dart';
import '../../domain/entities/section_config.dart';

class SectionConfigModel extends Equatable {
  final String id;
  final core_enum.SectionType sectionType;
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

  const SectionConfigModel({
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

  factory SectionConfigModel.fromJson(Map<String, dynamic> json) {
    return SectionConfigModel(
      id: json['id'] as String,
      sectionType: core_enum.SectionTypeExtension.tryFromString(json['sectionType'] as String? ?? '') ?? core_enum.SectionType.horizontalPropertyList,
      displaySettings: json['displaySettings'] as Map<String, dynamic>? ?? {},
      layoutSettings: json['layoutSettings'] as Map<String, dynamic>? ?? {},
      styleSettings: json['styleSettings'] as Map<String, dynamic>? ?? {},
      behaviorSettings: json['behaviorSettings'] as Map<String, dynamic>? ?? {},
      animationSettings: json['animationSettings'] as Map<String, dynamic>? ?? {},
      cacheSettings: json['cacheSettings'] as Map<String, dynamic>? ?? {},
      propertyIds: (json['propertyIds'] as List<dynamic>?)?.cast<String>() ?? [],
      title: json['title'] as String?,
      titleAr: json['titleAr'] as String?,
      subtitle: json['subtitle'] as String?,
      subtitleAr: json['subtitleAr'] as String?,
      backgroundColor: json['backgroundColor'] as String?,
      textColor: json['textColor'] as String?,
      customImage: json['customImage'] as String?,
      customData: json['customData'] as Map<String, dynamic>? ?? {},
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'sectionType': sectionType.value,
      'displaySettings': displaySettings,
      'layoutSettings': layoutSettings,
      'styleSettings': styleSettings,
      'behaviorSettings': behaviorSettings,
      'animationSettings': animationSettings,
      'cacheSettings': cacheSettings,
      'propertyIds': propertyIds,
      'title': title,
      'titleAr': titleAr,
      'subtitle': subtitle,
      'subtitleAr': subtitleAr,
      'backgroundColor': backgroundColor,
      'textColor': textColor,
      'customImage': customImage,
      'customData': customData,
    };
  }

  SectionConfig toEntity() {
    return SectionConfig(
      id: id,
      sectionType: sectionType,
      displaySettings: displaySettings,
      layoutSettings: layoutSettings,
      styleSettings: styleSettings,
      behaviorSettings: behaviorSettings,
      animationSettings: animationSettings,
      cacheSettings: cacheSettings,
      propertyIds: propertyIds,
      title: title,
      titleAr: titleAr,
      subtitle: subtitle,
      subtitleAr: subtitleAr,
      backgroundColor: backgroundColor,
      textColor: textColor,
      customImage: customImage,
      customData: customData,
    );
  }

  SectionConfigModel copyWith({
    String? id,
    core_enum.SectionType? sectionType,
    Map<String, dynamic>? displaySettings,
    Map<String, dynamic>? layoutSettings,
    Map<String, dynamic>? styleSettings,
    Map<String, dynamic>? behaviorSettings,
    Map<String, dynamic>? animationSettings,
    Map<String, dynamic>? cacheSettings,
    List<String>? propertyIds,
    String? title,
    String? titleAr,
    String? subtitle,
    String? subtitleAr,
    String? backgroundColor,
    String? textColor,
    String? customImage,
    Map<String, dynamic>? customData,
  }) {
    return SectionConfigModel(
      id: id ?? this.id,
      sectionType: sectionType ?? this.sectionType,
      displaySettings: displaySettings ?? this.displaySettings,
      layoutSettings: layoutSettings ?? this.layoutSettings,
      styleSettings: styleSettings ?? this.styleSettings,
      behaviorSettings: behaviorSettings ?? this.behaviorSettings,
      animationSettings: animationSettings ?? this.animationSettings,
      cacheSettings: cacheSettings ?? this.cacheSettings,
      propertyIds: propertyIds ?? this.propertyIds,
      title: title ?? this.title,
      titleAr: titleAr ?? this.titleAr,
      subtitle: subtitle ?? this.subtitle,
      subtitleAr: subtitleAr ?? this.subtitleAr,
      backgroundColor: backgroundColor ?? this.backgroundColor,
      textColor: textColor ?? this.textColor,
      customImage: customImage ?? this.customImage,
      customData: customData ?? this.customData,
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