// lib/features/home/domain/entities/home_section.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart' as core_enum;
import '../../../../core/models/dynamic_content_model.dart';
import 'section_config.dart';

class HomeSection extends Equatable {
  final String id;
  final core_enum.SectionType sectionType; // Match backend and model naming
  final int order;
  final bool isActive;
  final String? title;
  final String? subtitle;
  final String? titleAr; // Match backend
  final String? subtitleAr; // Match backend
  final SectionConfig sectionConfig; // Match backend and model naming
  final List<DynamicContentModel> content;
  final Map<String, dynamic> metadata;
  final DateTime? scheduledAt;
  final DateTime? expiresAt;
  final List<String> targetAudience;
  final int priority;

  const HomeSection({
    required this.id,
    required this.sectionType,
    required this.order,
    required this.isActive,
    this.title,
    this.subtitle,
    this.titleAr,
    this.subtitleAr,
    required this.sectionConfig,
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

  bool get hasValidContent => content.isNotEmpty && content.any((c) => c.isCurrentlyValid);

  bool get isEmpty => content.isEmpty;

  int get validContentCount => content.where((c) => c.isCurrentlyValid).length;

  List<DynamicContentModel> get validContent => content.where((c) => c.isCurrentlyValid).toList();

  String get displayTitle => title ?? sectionType.value.replaceAll('_', ' ').toUpperCase();

  String get sectionKey => '${sectionType.value}_$id';

  bool get requiresProperties => _requiresPropertyData(sectionType);

  bool get isTimeSensitive => _isTimeSensitive(sectionType);

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
        'section_type': sectionType.value,
        'section_order': order,
        'content_count': content.length,
        'valid_content_count': validContentCount,
        'is_visible': isVisible,
        'is_time_sensitive': isTimeSensitive,
        'priority': priority,
      };

  bool _requiresPropertyData(core_enum.SectionType type) {
    switch (type) {
      case core_enum.SectionType.singlePropertyAd:
      case core_enum.SectionType.multiPropertyAd:
      case core_enum.SectionType.horizontalPropertyList:
      case core_enum.SectionType.verticalPropertyGrid:
      case core_enum.SectionType.mixedLayoutList:
      case core_enum.SectionType.compactPropertyList:
      case core_enum.SectionType.premiumCarousel:
        return true;
      default:
        return false;
    }
  }

  bool _isTimeSensitive(core_enum.SectionType type) {
    switch (type) {
      case core_enum.SectionType.limitedTimeOffer:
      case core_enum.SectionType.flashDeals:
      case core_enum.SectionType.seasonalOffer:
        return true;
      default:
        return false;
    }
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
        sectionConfig,
        content,
        metadata,
        scheduledAt,
        expiresAt,
        targetAudience,
        priority,
      ];
}