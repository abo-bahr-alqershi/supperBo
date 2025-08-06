// lib/features/home/data/models/home_section_model.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart';
import '../../../../core/models/dynamic_content_model.dart';
import '../../domain/entities/home_section.dart';
import 'section_config_model.dart';

class HomeSectionModel extends Equatable {
  final String id;
  final SectionType sectionType; // Match backend naming
  final int order;
  final bool isActive;
  final String? title;
  final String? subtitle;
  final String? titleAr; // Added for Arabic support
  final String? subtitleAr; // Added for Arabic support
  final DateTime createdAt;
  final DateTime updatedAt;
  final SectionConfigModel sectionConfig; // Match backend naming
  final List<DynamicContentModel> content;
  final Map<String, dynamic> metadata;
  final DateTime? scheduledAt;
  final DateTime? expiresAt;
  final List<String> targetAudience;
  final int priority;
  
  // Computed properties from backend
  final bool? isVisible;
  final bool? isExpired;
  final bool? isScheduled;
  final bool? isTimeSensitive;

  const HomeSectionModel({
    required this.id,
    required this.sectionType,
    required this.order,
    required this.isActive,
    this.title,
    this.subtitle,
    this.titleAr,
    this.subtitleAr,    
    required this.createdAt,
    required this.updatedAt,
    required this.sectionConfig,
    required this.content,
    required this.metadata,
    this.scheduledAt,
    this.expiresAt,
    required this.targetAudience,
    this.priority = 0,
    this.isVisible,
    this.isExpired,
    this.isScheduled,
    this.isTimeSensitive,
  });

  bool get isCurrentlyScheduled {
    if (scheduledAt == null) return false;
    return DateTime.now().isBefore(scheduledAt!);
  }

  bool get isCurrentlyExpired {
    if (expiresAt == null) return false;
    return DateTime.now().isAfter(expiresAt!);
  }

  bool get isCurrentlyVisible => isActive && !isCurrentlyScheduled && !isCurrentlyExpired;
  
  bool get isTimeSensitiveSection => 
      sectionType == SectionType.limitedTimeOffer ||
      sectionType == SectionType.flashDeals ||
      sectionType == SectionType.seasonalOffer;

  bool get hasValidContent => content.isNotEmpty && content.any((c) => c.isCurrentlyValid);

  factory HomeSectionModel.fromJson(Map<String, dynamic> json) {
    return HomeSectionModel(
      id: json['id'] as String,
      sectionType: SectionType.tryFromString(json['type'] as String?) ?? SectionType.horizontalPropertyList,
      order: json['order'] as int,
      isActive: json['isActive'] as bool,
      title: json['title'] as String?,
      subtitle: json['subtitle'] as String?,
      titleAr: json['titleAr'] as String?,
      subtitleAr: json['subtitleAr'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      sectionConfig: SectionConfigModel.fromJson(json['config'] as Map<String, dynamic>),
      content: (json['content'] as List<dynamic>?)
              ?.map((item) => DynamicContentModel.fromJson(item as Map<String, dynamic>))
              .toList() ??
          [],
      metadata: json['metadata'] as Map<String, dynamic>? ?? {},
      scheduledAt: json['scheduledAt'] != null
          ? DateTime.parse(json['scheduledAt'] as String)
          : null,
      expiresAt: json['expiresAt'] != null
          ? DateTime.parse(json['expiresAt'] as String)
          : null,
      targetAudience: (json['targetAudience'] as List<dynamic>?)?.cast<String>() ?? [],
      priority: json['priority'] as int? ?? 0,
      // Backend computed properties
      isVisible: json['isVisible'] as bool?,
      isExpired: json['isExpired'] as bool?,
      isScheduled: json['isScheduled'] as bool?,
      isTimeSensitive: json['isTimeSensitive'] as bool?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'sectionType': sectionType.value,
      'order': order,
      'isActive': isActive,
      'title': title,
      'subtitle': subtitle,
      'titleAr': titleAr,
      'subtitleAr': subtitleAr,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'sectionConfig': sectionConfig.toJson(),
      'content': content.map((item) => item.toJson()).toList(),
      'metadata': metadata,
      'scheduledAt': scheduledAt?.toIso8601String(),
      'expiresAt': expiresAt?.toIso8601String(),
      'targetAudience': targetAudience,
      'priority': priority,
      // Include computed properties if they exist
      if (isVisible != null) 'isVisible': isVisible,
      if (isExpired != null) 'isExpired': isExpired,
      if (isScheduled != null) 'isScheduled': isScheduled,
      if (isTimeSensitive != null) 'isTimeSensitive': isTimeSensitive,
    };
  }

  HomeSection toEntity() {
    return HomeSection(
      id: id,
      sectionType: sectionType,
      order: order,
      isActive: isActive,
      title: title,
      subtitle: subtitle,
      titleAr: titleAr,
      subtitleAr: subtitleAr,
      sectionConfig: sectionConfig.toEntity(),
      content: content,
      metadata: metadata,
      scheduledAt: scheduledAt,
      expiresAt: expiresAt,
      targetAudience: targetAudience,
      priority: priority,
    );
  }

  HomeSectionModel copyWith({
    String? id,
    SectionType? sectionType,
    int? order,
    bool? isActive,
    String? title,
    String? subtitle,
    String? titleAr,
    String? subtitleAr,
    DateTime? createdAt,
    DateTime? updatedAt,
    SectionConfigModel? sectionConfig,
    List<DynamicContentModel>? content,
    Map<String, dynamic>? metadata,
    DateTime? scheduledAt,
    DateTime? expiresAt,
    List<String>? targetAudience,
    int? priority,
    bool? isVisible,
    bool? isExpired,
    bool? isScheduled,
    bool? isTimeSensitive,
  }) {
    return HomeSectionModel(
      id: id ?? this.id,
      sectionType: sectionType ?? this.sectionType,
      order: order ?? this.order,
      isActive: isActive ?? this.isActive,
      title: title ?? this.title,
      subtitle: subtitle ?? this.subtitle,
      titleAr: titleAr ?? this.titleAr,
      subtitleAr: subtitleAr ?? this.subtitleAr,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      sectionConfig: sectionConfig ?? this.sectionConfig,
      content: content ?? this.content,
      metadata: metadata ?? this.metadata,
      scheduledAt: scheduledAt ?? this.scheduledAt,
      expiresAt: expiresAt ?? this.expiresAt,
      targetAudience: targetAudience ?? this.targetAudience,
      priority: priority ?? this.priority,
      isVisible: isVisible ?? this.isVisible,
      isExpired: isExpired ?? this.isExpired,
      isScheduled: isScheduled ?? this.isScheduled,
      isTimeSensitive: isTimeSensitive ?? this.isTimeSensitive,
    );
  }

  @override
  List<Object?> get props => [
        id,
        sectionType,
        order,
        isActive,
        title,
        subtitle,
        titleAr,
        subtitleAr,
        createdAt,
        updatedAt,
        sectionConfig,
        content,
        metadata,
        scheduledAt,
        expiresAt,
        targetAudience,
        priority,
        isVisible,
        isExpired,
        isScheduled,
        isTimeSensitive,
      ];
}