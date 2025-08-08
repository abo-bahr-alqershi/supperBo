// lib/features/home/domain/entities/special_offer.dart

import 'package:equatable/equatable.dart';

class SpecialOffer extends Equatable {
  final String id;
  final String title;
  final String? subtitle;
  final String description;
  final String? propertyId;
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

  const SpecialOffer({
    required this.id,
    required this.title,
    this.subtitle,
    required this.description,
    this.propertyId,
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
  });

  bool get isCurrentlyActive {
    final now = DateTime.now();
    return isActive && 
           now.isAfter(startDate) && 
           now.isBefore(endDate) &&
           !isMaxUsageReached &&
           isValidToday;
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

  Duration get timeUntilStart {
    final now = DateTime.now();
    if (now.isAfter(startDate)) return Duration.zero;
    return startDate.difference(now);
  }

  bool get isValidToday {
    if (validDays.isEmpty) return true;
    final today = DateTime.now().weekday;
    final dayNames = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
    final todayName = dayNames[today - 1];
    return validDays.contains(todayName);
  }

  double? get finalPrice {
    if (offerPrice != null) return offerPrice;
    if (originalPrice == null) return null;
    if (discountAmount != null) return originalPrice! - discountAmount!;
    return originalPrice! * (1 - (discountPercentage / 100));
  }

  double? get savingsAmount {
    if (originalPrice == null) return null;
    final finalPriceAmount = finalPrice ?? originalPrice!;
    return originalPrice! - finalPriceAmount;
  }

  String get formattedDiscount => '${discountPercentage.toStringAsFixed(0)}%';

  String get formattedOriginalPrice {
    if (originalPrice == null) return '';
    return '${originalPrice!.toStringAsFixed(0)} ${currency ?? ''}';
    }

  String get formattedOfferPrice {
    final price = finalPrice;
    if (price == null) return '';
    return '${price.toStringAsFixed(0)} ${currency ?? ''}';
  }

  String get formattedSavings {
    final savings = savingsAmount;
    if (savings == null) return '';
    return '${savings.toStringAsFixed(0)} ${currency ?? ''}';
  }

  String get displayImage => bannerImageUrl ?? '';

  bool get hasProperty => propertyId != null;

  bool get hasMultipleProperties => propertyIds.length > 1;

  bool get hasPromoCode => promoCode != null && promoCode!.isNotEmpty;

  bool get isUrgent => isFlashDeal || (isLimitedTime && remainingTime.inHours < 24);

  int get remainingUsage {
    if (maxUsage == null) return -1;
    return maxUsage! - currentUsage;
  }

  double get usagePercentage {
    if (maxUsage == null) return 0;
    return (currentUsage / maxUsage!) * 100;
  }

  T? getCondition<T>(String key) {
    try {
      return conditions[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getStyling<T>(String key) {
    try {
      return styling[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getMetadata<T>(String key) {
    try {
      return metadata[key] as T?;
    } catch (_) {
      return null;
    }
  }

  List<String> get tags {
    List<String> offerTags = [];
    if (isFlashDeal) offerTags.add('flash_deal');
    if (isLimitedTime) offerTags.add('limited_time');
    if (isSeasonal) offerTags.add('seasonal');
    if (hasPromoCode) offerTags.add('promo_code');
    if (isUrgent) offerTags.add('urgent');
    return offerTags;
  }

  Map<String, dynamic> get analyticsData => {
        'offer_id': id,
        'offer_type': offerType,
        'discount_percentage': discountPercentage,
        'is_flash_deal': isFlashDeal,
        'is_limited_time': isLimitedTime,
        'is_seasonal': isSeasonal,
        'has_promo_code': hasPromoCode,
        'is_urgent': isUrgent,
        'remaining_time_seconds': remainingTime.inSeconds,
        'usage_percentage': usagePercentage,
        'priority': priority,
        'property_id': propertyId,
        'property_ids': propertyIds,
        'tags': tags,
      };

  @override
  List<Object?> get props => [
        id,
        title,
        subtitle,
        description,
        propertyId,
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
      ];
}