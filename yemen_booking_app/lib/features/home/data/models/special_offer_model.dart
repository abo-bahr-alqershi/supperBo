// lib/features/home/data/models/special_offer_model.dart

import 'package:equatable/equatable.dart';
import '../../domain/entities/special_offer.dart';
import 'property_model.dart';

class SpecialOfferModel extends Equatable {
  final String id;
  final String title;
  final String? subtitle;
  final String description;
  final PropertyModel? property;
  final List<String> propertyIds;
  final String offerType;
  final double discountPercentage;
  final double? discountAmount;
  final double? originalPrice;
  final double? offerPrice;
  final String? currency;
  final DateTime startDate;
  final DateTime endDate;
  final int? maxUsage;
  final int currentUsage;
  final List<String> validDays;
  final Map<String, dynamic> conditions;
  final String? promoCode;
  final String? bannerImageUrl;
  final String? backgroundColor;
  final String? textColor;
  final Map<String, dynamic> styling;
  final bool isFlashDeal;
  final bool isLimitedTime;
  final bool isSeasonal;
  final int priority;
  final Map<String, dynamic> metadata;
  final bool isActive;
  final DateTime createdAt;
  final DateTime updatedAt;

  const SpecialOfferModel({
    required this.id,
    required this.title,
    this.subtitle,
    required this.description,
    this.property,
    required this.propertyIds,
    required this.offerType,
    required this.discountPercentage,
    this.discountAmount,
    this.originalPrice,
    this.offerPrice,
    this.currency,
    required this.startDate,
    required this.endDate,
    this.maxUsage,
    this.currentUsage = 0,
    required this.validDays,
    required this.conditions,
    this.promoCode,
    this.bannerImageUrl,
    this.backgroundColor,
    this.textColor,
    required this.styling,
    this.isFlashDeal = false,
    this.isLimitedTime = false,
    this.isSeasonal = false,
    this.priority = 0,
    required this.metadata,
    this.isActive = true,
    required this.createdAt,
    required this.updatedAt,
  });

  bool get isCurrentlyActive {
    final now = DateTime.now();
    return isActive && 
           now.isAfter(startDate) && 
           now.isBefore(endDate) &&
           !isMaxUsageReached;
  }

  bool get isExpired => DateTime.now().isAfter(endDate);

  bool get isScheduled => DateTime.now().isBefore(startDate);

  bool get isMaxUsageReached {
    if (maxUsage == null) return false;
    return currentUsage >= maxUsage!;
  }

  Duration get remainingTime {
    final now = DateTime.now();
    if (now.isAfter(endDate)) return Duration.zero;
    return endDate.difference(now);
  }

  bool get isValidToday {
    final today = DateTime.now().weekday;
    final dayNames = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
    final todayName = dayNames[today - 1];
    return validDays.isEmpty || validDays.contains(todayName);
  }

  factory SpecialOfferModel.fromJson(Map<String, dynamic> json) {
    return SpecialOfferModel(
      id: json['id'] as String,
      title: json['title'] as String,
      subtitle: json['subtitle'] as String?,
      description: json['description'] as String,
      property: json['property'] != null
          ? PropertyModel.fromJson(json['property'] as Map<String, dynamic>)
          : null,
      propertyIds: (json['propertyIds'] as List<dynamic>?)?.cast<String>() ?? [],
      offerType: json['offerType'] as String,
      discountPercentage: (json['discountPercentage'] as num).toDouble(),
      discountAmount: json['discountAmount'] != null
          ? (json['discountAmount'] as num).toDouble()
          : null,
      originalPrice: json['originalPrice'] != null
          ? (json['originalPrice'] as num).toDouble()
          : null,
      offerPrice: json['offerPrice'] != null
          ? (json['offerPrice'] as num).toDouble()
          : null,
      currency: json['currency'] as String?,
      startDate: DateTime.parse(json['startDate'] as String),
      endDate: DateTime.parse(json['endDate'] as String),
      maxUsage: json['maxUsage'] as int?,
      currentUsage: json['currentUsage'] as int? ?? 0,
      validDays: (json['validDays'] as List<dynamic>?)?.cast<String>() ?? [],
      conditions: json['conditions'] as Map<String, dynamic>? ?? {},
      promoCode: json['promoCode'] as String?,
      bannerImageUrl: json['bannerImageUrl'] as String?,
      backgroundColor: json['backgroundColor'] as String?,
      textColor: json['textColor'] as String?,
      styling: json['styling'] as Map<String, dynamic>? ?? {},
      isFlashDeal: json['isFlashDeal'] as bool? ?? false,
      isLimitedTime: json['isLimitedTime'] as bool? ?? false,
      isSeasonal: json['isSeasonal'] as bool? ?? false,
      priority: json['priority'] as int? ?? 0,
      metadata: json['metadata'] as Map<String, dynamic>? ?? {},
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
      'offerType': offerType,
      'discountPercentage': discountPercentage,
      'discountAmount': discountAmount,
      'originalPrice': originalPrice,
      'offerPrice': offerPrice,
      'currency': currency,
      'startDate': startDate.toIso8601String(),
      'endDate': endDate.toIso8601String(),
      'maxUsage': maxUsage,
      'currentUsage': currentUsage,
      'validDays': validDays,
      'conditions': conditions,
      'promoCode': promoCode,
      'bannerImageUrl': bannerImageUrl,
      'backgroundColor': backgroundColor,
      'textColor': textColor,
      'styling': styling,
      'isFlashDeal': isFlashDeal,
      'isLimitedTime': isLimitedTime,
      'isSeasonal': isSeasonal,
      'priority': priority,
      'metadata': metadata,
      'isActive': isActive,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  SpecialOffer toEntity() {
    return SpecialOffer(
      id: id,
      title: title,
      subtitle: subtitle,
      description: description,
      propertyId: property?.id,
      propertyIds: propertyIds,
      offerType: offerType,
      discountPercentage: discountPercentage,
      discountAmount: discountAmount,
      originalPrice: originalPrice,
      offerPrice: offerPrice,
      currency: currency,
      startDate: startDate,
      endDate: endDate,
      maxUsage: maxUsage,
      currentUsage: currentUsage,
      validDays: validDays,
      conditions: conditions,
      promoCode: promoCode,
      bannerImageUrl: bannerImageUrl,
      backgroundColor: backgroundColor,
      textColor: textColor,
      styling: styling,
      isFlashDeal: isFlashDeal,
      isLimitedTime: isLimitedTime,
      isSeasonal: isSeasonal,
      priority: priority,
      metadata: metadata,
      isActive: isActive,
    );
  }

  SpecialOfferModel copyWith({
    String? id,
    String? title,
    String? subtitle,
    String? description,
    PropertyModel? property,
    List<String>? propertyIds,
    String? offerType,
    double? discountPercentage,
    double? discountAmount,
    double? originalPrice,
    double? offerPrice,
    String? currency,
    DateTime? startDate,
    DateTime? endDate,
    int? maxUsage,
    int? currentUsage,
    List<String>? validDays,
    Map<String, dynamic>? conditions,
    String? promoCode,
    String? bannerImageUrl,
    String? backgroundColor,
    String? textColor,
    Map<String, dynamic>? styling,
    bool? isFlashDeal,
    bool? isLimitedTime,
    bool? isSeasonal,
    int? priority,
    Map<String, dynamic>? metadata,
    bool? isActive,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return SpecialOfferModel(
      id: id ?? this.id,
      title: title ?? this.title,
      subtitle: subtitle ?? this.subtitle,
      description: description ?? this.description,
      property: property ?? this.property,
      propertyIds: propertyIds ?? this.propertyIds,
      offerType: offerType ?? this.offerType,
      discountPercentage: discountPercentage ?? this.discountPercentage,
      discountAmount: discountAmount ?? this.discountAmount,
      originalPrice: originalPrice ?? this.originalPrice,
      offerPrice: offerPrice ?? this.offerPrice,
      currency: currency ?? this.currency,
      startDate: startDate ?? this.startDate,
      endDate: endDate ?? this.endDate,
      maxUsage: maxUsage ?? this.maxUsage,
      currentUsage: currentUsage ?? this.currentUsage,
      validDays: validDays ?? this.validDays,
      conditions: conditions ?? this.conditions,
      promoCode: promoCode ?? this.promoCode,
      bannerImageUrl: bannerImageUrl ?? this.bannerImageUrl,
      backgroundColor: backgroundColor ?? this.backgroundColor,
      textColor: textColor ?? this.textColor,
      styling: styling ?? this.styling,
      isFlashDeal: isFlashDeal ?? this.isFlashDeal,
      isLimitedTime: isLimitedTime ?? this.isLimitedTime,
      isSeasonal: isSeasonal ?? this.isSeasonal,
      priority: priority ?? this.priority,
      metadata: metadata ?? this.metadata,
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
        offerType,
        discountPercentage,
        discountAmount,
        originalPrice,
        offerPrice,
        currency,
        startDate,
        endDate,
        maxUsage,
        currentUsage,
        validDays,
        conditions,
        promoCode,
        bannerImageUrl,
        backgroundColor,
        textColor,
        styling,
        isFlashDeal,
        isLimitedTime,
        isSeasonal,
        priority,
        metadata,
        isActive,
        createdAt,
        updatedAt,
      ];
}