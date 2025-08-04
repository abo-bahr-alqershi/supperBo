// lib/features/home/domain/entities/home_section.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart';
import '../../../../core/models/dynamic_content_model.dart';
import 'section_config.dart';

class HomeSection extends Equatable {
  final String id;
  final SectionType type;
  final int order;
  final bool isActive;
  final String? title;
  final String? subtitle;
  final SectionConfig config;
  final List<DynamicContentModel> content;
  final Map<String, dynamic> metadata;
  final DateTime? scheduledAt;
  final DateTime? expiresAt;
  final List<String> targetAudience;
  final int priority;

  const HomeSection({
    required this.id,
    required this.type,
    required this.order,
    required this.isActive,
    this.title,
    this.subtitle,
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

  bool get isEmpty => content.isEmpty;

  int get validContentCount => content.where((c) => c.isValid).length;

  List<DynamicContentModel> get validContent => content.where((c) => c.isValid).toList();

  String get displayTitle => title ?? type.value.replaceAll('_', ' ').toUpperCase();

  String get sectionKey => '${type.value}_$id';

  bool get requiresProperties => type.requiresPropertyData;

  bool get isTimeSensitive => type.isTimeSensitive;

  Duration? get remainingTime {
    if (expiresAt == null) return null;
    final now = DateTime.now();
    if (now.isAfter(expiresAt!)) return null;
    return expiresAt!.difference(now);
  }

  Duration? get timeUntilScheduled {
    if (scheduledAt == null) return null;
    final now = DateTime.now();
    if (now.isAfter(scheduledAt!)) return null;
    return scheduledAt!.difference(now);
  }

  Map<String, dynamic> get analyticsMetadata => {
        'section_id': id,
        'section_type': type.value,
        'section_order': order,
        'content_count': content.length,
        'valid_content_count': validContentCount,
        'is_visible': isVisible,
        'is_time_sensitive': isTimeSensitive,
        'priority': priority,
      };

  @override
  List<Object?> get props => [
        id,
        type,
        order,
        isActive,
        title,
        subtitle,
        config,
        content,
        metadata,
        scheduledAt,
        expiresAt,
        targetAudience,
        priority,
      ];
}