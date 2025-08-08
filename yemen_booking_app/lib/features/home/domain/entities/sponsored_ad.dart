// lib/features/home/domain/entities/sponsored_ad.dart

import 'package:equatable/equatable.dart';

class SponsoredAd extends Equatable {
  final String id;
  final String title;
  final String? subtitle;
  final String? description;
  final String? propertyId;
  final List<String> propertyIds;
  final String? customImageUrl;
  final String? backgroundColor;
  final String? textColor;
  final Map<String, dynamic> styling;
  final String ctaText;
  final String ctaAction;
  final Map<String, dynamic> ctaData;
  final DateTime startDate;
  final DateTime endDate;
  final int priority;
  final Map<String, dynamic> targetingData;
  final Map<String, dynamic> analyticsData;
  final bool isActive;

  const SponsoredAd({
    required this.id,
    required this.title,
    this.subtitle,
    this.description,
    this.propertyId,
    required this.propertyIds,
    this.customImageUrl,
    this.backgroundColor,
    this.textColor,
    required this.styling,
    required this.ctaText,
    required this.ctaAction,
    required this.ctaData,
    required this.startDate,
    required this.endDate,
    required this.priority,
    required this.targetingData,
    required this.analyticsData,
    this.isActive = true,
  });

  bool get isCurrentlyActive {
    final now = DateTime.now();
    return isActive && 
           now.isAfter(startDate) && 
           now.isBefore(endDate);
  }

  bool get isExpired => DateTime.now().isAfter(endDate);

  bool get isScheduled => DateTime.now().isBefore(startDate);

  Duration get remainingTime {
    final now = DateTime.now();
    if (now.isAfter(endDate)) return Duration.zero;
    return endDate.difference(now);
  }

  Duration get timeUntilStart {
    final now = DateTime.now();
    if (now.isAfter(startDate)) return Duration.zero;
    return startDate.difference(now);
  }

  String get displayImage => customImageUrl ?? '';

  bool get hasCustomStyling => styling.isNotEmpty;

  bool get hasProperty => propertyId != null;

  bool get hasMultipleProperties => propertyIds.length > 1;

  String get adType {
    if (hasMultipleProperties) return 'multi_property';
    if (hasProperty) return 'single_property';
    return 'custom';
  }

  T? getStyling<T>(String key) {
    try {
      return styling[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getTargetingData<T>(String key) {
    try {
      return targetingData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getAnalyticsData<T>(String key) {
    try {
      return analyticsData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getCtaData<T>(String key) {
    try {
      return ctaData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  Map<String, dynamic> get impressionData => {
        'ad_id': id,
        'ad_type': adType,
        'property_id': propertyId,
        'property_ids': propertyIds,
        'priority': priority,
        'is_active': isCurrentlyActive,
        'remaining_time_seconds': remainingTime.inSeconds,
        ...analyticsData,
      };

  Map<String, dynamic> get clickData => {
        'ad_id': id,
        'ad_type': adType,
        'cta_text': ctaText,
        'cta_action': ctaAction,
        'property_id': propertyId,
        'property_ids': propertyIds,
        ...ctaData,
        ...analyticsData,
      };

  @override
  List<Object?> get props => [
        id,
        title,
        subtitle,
        description,
        propertyId,
        propertyIds,
        customImageUrl,
        backgroundColor,
        textColor,
        styling,
        ctaText,
        ctaAction,
        ctaData,
        startDate,
        endDate,
        priority,
        targetingData,
        analyticsData,
        isActive,
      ];
}