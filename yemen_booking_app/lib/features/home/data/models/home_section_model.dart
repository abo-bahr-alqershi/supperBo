// lib/features/home/data/models/home_section_model.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart';
import '../../../../core/models/dynamic_content_model.dart';
import '../../domain/entities/home_section.dart';
import 'section_config_model.dart';

class HomeSectionModel extends Equatable {
  final String id;
  final SectionType type;
  final int order;
  final bool isActive;
  final String? title;
  final String? subtitle;
  final DateTime createdAt;
  final DateTime updatedAt;
  final SectionConfigModel config;
  final List<DynamicContentModel> content;
  final Map<String, dynamic> metadata;
  final DateTime? scheduledAt;
  final DateTime? expiresAt;
  final List<String> targetAudience;
  final int priority;

  const HomeSectionModel({
    required this.id,
    required this.type,
    required this.order,
    required this.isActive,
    this.title,
    this.subtitle,
    required this.createdAt,
    required this.updatedAt,
    required this.config,
    required this.content,
    required this.metadata,
    this.scheduledAt,
    this.expiresAt,
    required this.targetAudience,
    this.priority = 0,
  });

  bool get isScheduled {
    if (scheduledAt == null) return false;
    return DateTime.now().isBefore(scheduledAt!);
  }

  bool get isExpired {
    if (expiresAt == null) return false;
    return DateTime.now().isAfter(expiresAt!);
  }

  bool get isVisible => isActive && !isScheduled && !isExpired;

  bool get hasValidContent => content.isNotEmpty && content.any((c) => c.isValid);

  factory HomeSectionModel.fromJson(Map<String, dynamic> json) {
    return HomeSectionModel(
      id: json['id'] as String,
      type: SectionType.tryFromString(json['type'] as String?) ?? SectionType.horizontalPropertyList,
      order: json['order'] as int,
      isActive: json['isActive'] as bool,
      title: json['title'] as String?,
      subtitle: json['subtitle'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      config: SectionConfigModel.fromJson(json['config'] as Map<String, dynamic>),
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
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'type': type.value,
      'order': order,
      'isActive': isActive,
      'title': title,
      'subtitle': subtitle,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'config': config.toJson(),
      'content': content.map((item) => item.toJson()).toList(),
      'metadata': metadata,
      'scheduledAt': scheduledAt?.toIso8601String(),
      'expiresAt': expiresAt?.toIso8601String(),
      'targetAudience': targetAudience,
      'priority': priority,
    };
  }

  HomeSection toEntity() {
    return HomeSection(
      id: id,
      type: type,
      order: order,
      isActive: isActive,
      title: title,
      subtitle: subtitle,
      config: config.toEntity(),
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
    SectionType? type,
    int? order,
    bool? isActive,
    String? title,
    String? subtitle,
    DateTime? createdAt,
    DateTime? updatedAt,
    SectionConfigModel? config,
    List<DynamicContentModel>? content,
    Map<String, dynamic>? metadata,
    DateTime? scheduledAt,
    DateTime? expiresAt,
    List<String>? targetAudience,
    int? priority,
  }) {
    return HomeSectionModel(
      id: id ?? this.id,
      type: type ?? this.type,
      order: order ?? this.order,
      isActive: isActive ?? this.isActive,
      title: title ?? this.title,
      subtitle: subtitle ?? this.subtitle,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      config: config ?? this.config,
      content: content ?? this.content,
      metadata: metadata ?? this.metadata,
      scheduledAt: scheduledAt ?? this.scheduledAt,
      expiresAt: expiresAt ?? this.expiresAt,
      targetAudience: targetAudience ?? this.targetAudience,
      priority: priority ?? this.priority,
    );
  }

  @override
  List<Object?> get props => [
        id,
        type,
        order,
        isActive,
        title,
        subtitle,
        createdAt,
        updatedAt,
        config,
        content,
        metadata,
        scheduledAt,
        expiresAt,
        targetAudience,
        priority,
      ];
}