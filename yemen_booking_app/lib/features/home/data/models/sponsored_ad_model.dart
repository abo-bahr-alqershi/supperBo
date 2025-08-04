// lib/features/home/data/models/sponsored_ad_model.dart

import 'package:equatable/equatable.dart';
import '../../domain/entities/sponsored_ad.dart';
import 'property_model.dart';

class SponsoredAdModel extends Equatable {
  final String id;
  final String title;
  final String? subtitle;
  final String? description;
  final PropertyModel? property;
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
  final DateTime createdAt;
  final DateTime updatedAt;

  const SponsoredAdModel({
    required this.id,
    required this.title,
    this.subtitle,
    this.description,
    this.property,
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
    required this.createdAt,
    required this.updatedAt,
  });

  bool get isCurrentlyActive {
    final now = DateTime.now();
    return isActive && 
           now.isAfter(startDate) && 
           now.isBefore(endDate);
  }

  bool get isExpired => DateTime.now().isAfter(endDate);

  bool get isScheduled => DateTime.now().isBefore(startDate);

  factory SponsoredAdModel.fromJson(Map<String, dynamic> json) {
    return SponsoredAdModel(
      id: json['id'] as String,
      title: json['title'] as String,
      subtitle: json['subtitle'] as String?,
      description: json['description'] as String?,
      property: json['property'] != null
          ? PropertyModel.fromJson(json['property'] as Map<String, dynamic>)
          : null,
      propertyIds: (json['propertyIds'] as List<dynamic>?)?.cast<String>() ?? [],
      customImageUrl: json['customImageUrl'] as String?,
      backgroundColor: json['backgroundColor'] as String?,
      textColor: json['textColor'] as String?,
      styling: json['styling'] as Map<String, dynamic>? ?? {},
      ctaText: json['ctaText'] as String? ?? 'View Details',
      ctaAction: json['ctaAction'] as String? ?? 'navigate',
      ctaData: json['ctaData'] as Map<String, dynamic>? ?? {},
      startDate: DateTime.parse(json['startDate'] as String),
      endDate: DateTime.parse(json['endDate'] as String),
      priority: json['priority'] as int? ?? 0,
      targetingData: json['targetingData'] as Map<String, dynamic>? ?? {},
      analyticsData: json['analyticsData'] as Map<String, dynamic>? ?? {},
      isActive: json['isActive'] as bool? ?? true,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'subtitle': subtitle,
      'description': description,
      'property': property?.toJson(),
      'propertyIds': propertyIds,
      'customImageUrl': customImageUrl,
      'backgroundColor': backgroundColor,
      'textColor': textColor,
      'styling': styling,
      'ctaText': ctaText,
      'ctaAction': ctaAction,
      'ctaData': ctaData,
      'startDate': startDate.toIso8601String(),
      'endDate': endDate.toIso8601String(),
      'priority': priority,
      'targetingData': targetingData,
      'analyticsData': analyticsData,
      'isActive': isActive,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  SponsoredAd toEntity() {
    return SponsoredAd(
      id: id,
      title: title,
      subtitle: subtitle,
      description: description,
      property: property?.toEntity(),
      propertyIds: propertyIds,
      customImageUrl: customImageUrl,
      backgroundColor: backgroundColor,
      textColor: textColor,
      styling: styling,
      ctaText: ctaText,
      ctaAction: ctaAction,
      ctaData: ctaData,
      startDate: startDate,
      endDate: endDate,
      priority: priority,
      targetingData: targetingData,
      analyticsData: analyticsData,
      isActive: isActive,
    );
  }

  SponsoredAdModel copyWith({
    String? id,
    String? title,
    String? subtitle,
    String? description,
    PropertyModel? property,
    List<String>? propertyIds,
    String? customImageUrl,
    String? backgroundColor,
    String? textColor,
    Map<String, dynamic>? styling,
    String? ctaText,
    String? ctaAction,
    Map<String, dynamic>? ctaData,
    DateTime? startDate,
    DateTime? endDate,
    int? priority,
    Map<String, dynamic>? targetingData,
    Map<String, dynamic>? analyticsData,
    bool? isActive,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return SponsoredAdModel(
      id: id ?? this.id,
      title: title ?? this.title,
      subtitle: subtitle ?? this.subtitle,
      description: description ?? this.description,
      property: property ?? this.property,
      propertyIds: propertyIds ?? this.propertyIds,
      customImageUrl: customImageUrl ?? this.customImageUrl,
      backgroundColor: backgroundColor ?? this.backgroundColor,
      textColor: textColor ?? this.textColor,
      styling: styling ?? this.styling,
      ctaText: ctaText ?? this.ctaText,
      ctaAction: ctaAction ?? this.ctaAction,
      ctaData: ctaData ?? this.ctaData,
      startDate: startDate ?? this.startDate,
      endDate: endDate ?? this.endDate,
      priority: priority ?? this.priority,
      targetingData: targetingData ?? this.targetingData,
      analyticsData: analyticsData ?? this.analyticsData,
      isActive: isActive ?? this.isActive,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [
        id,
        title,
        subtitle,
        description,
        property,
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
        createdAt,
        updatedAt,
      ];
}