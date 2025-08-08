// lib/features/home/data/models/section_data_model.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart' as core_enum;
import 'property_model.dart';
import 'sponsored_ad_model.dart';
import 'special_offer_model.dart';
import 'city_destination_model.dart';

class SectionDataModel extends Equatable {
  final String sectionId;
  final core_enum.SectionType sectionType;
  final List<PropertyModel> properties;
  final List<SponsoredAdModel> sponsoredAds;
  final List<SpecialOfferModel> specialOffers;
  final List<CityDestinationModel> destinations;
  final Map<String, dynamic> customData;
  final DateTime lastUpdated;
  final String? nextPageToken;
  final bool hasMore;
  final int totalCount;
  final Map<String, dynamic> metadata;

  const SectionDataModel({
    required this.sectionId,
    required this.sectionType,
    required this.properties,
    required this.sponsoredAds,
    required this.specialOffers,
    required this.destinations,
    required this.customData,
    required this.lastUpdated,
    this.nextPageToken,
    this.hasMore = false,
    this.totalCount = 0,
    required this.metadata,
  });

  bool get isEmpty =>
      properties.isEmpty &&
      sponsoredAds.isEmpty &&
      specialOffers.isEmpty &&
      destinations.isEmpty &&
      customData.isEmpty;

  bool get hasData => !isEmpty;

  int get totalItems =>
      properties.length +
      sponsoredAds.length +
      specialOffers.length +
      destinations.length;

  factory SectionDataModel.fromJson(Map<String, dynamic> json) {
    return SectionDataModel(
      sectionId: json['sectionId'] as String,
      sectionType: core_enum.SectionTypeExtension.tryFromString(json['sectionType'] as String? ?? '') ?? core_enum.SectionType.horizontalPropertyList,
      properties: (json['properties'] as List<dynamic>?)
              ?.map((item) => PropertyModel.fromJson(item as Map<String, dynamic>))
              .toList() ??
          [],
      sponsoredAds: (json['sponsoredAds'] as List<dynamic>?)
              ?.map((item) => SponsoredAdModel.fromJson(item as Map<String, dynamic>))
              .toList() ??
          [],
      specialOffers: (json['specialOffers'] as List<dynamic>?)
              ?.map((item) => SpecialOfferModel.fromJson(item as Map<String, dynamic>))
              .toList() ??
          [],
      destinations: (json['destinations'] as List<dynamic>?)
              ?.map((item) => CityDestinationModel.fromJson(item as Map<String, dynamic>))
              .toList() ??
          [],
      customData: json['customData'] as Map<String, dynamic>? ?? {},
      lastUpdated: DateTime.parse(json['lastUpdated'] as String),
      nextPageToken: json['nextPageToken'] as String?,
      hasMore: json['hasMore'] as bool? ?? false,
      totalCount: json['totalCount'] as int? ?? 0,
      metadata: json['metadata'] as Map<String, dynamic>? ?? {},
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'sectionId': sectionId,
      'sectionType': sectionType.value,
      'properties': properties.map((item) => item.toJson()).toList(),
      'sponsoredAds': sponsoredAds.map((item) => item.toJson()).toList(),
      'specialOffers': specialOffers.map((item) => item.toJson()).toList(),
      'destinations': destinations.map((item) => item.toJson()).toList(),
      'customData': customData,
      'lastUpdated': lastUpdated.toIso8601String(),
      'nextPageToken': nextPageToken,
      'hasMore': hasMore,
      'totalCount': totalCount,
      'metadata': metadata,
    };
  }

  SectionDataModel copyWith({
    String? sectionId,
    core_enum.SectionType? sectionType,
    List<PropertyModel>? properties,
    List<SponsoredAdModel>? sponsoredAds,
    List<SpecialOfferModel>? specialOffers,
    List<CityDestinationModel>? destinations,
    Map<String, dynamic>? customData,
    DateTime? lastUpdated,
    String? nextPageToken,
    bool? hasMore,
    int? totalCount,
    Map<String, dynamic>? metadata,
  }) {
    return SectionDataModel(
      sectionId: sectionId ?? this.sectionId,
      sectionType: sectionType ?? this.sectionType,
      properties: properties ?? this.properties,
      sponsoredAds: sponsoredAds ?? this.sponsoredAds,
      specialOffers: specialOffers ?? this.specialOffers,
      destinations: destinations ?? this.destinations,
      customData: customData ?? this.customData,
      lastUpdated: lastUpdated ?? this.lastUpdated,
      nextPageToken: nextPageToken ?? this.nextPageToken,
      hasMore: hasMore ?? this.hasMore,
      totalCount: totalCount ?? this.totalCount,
      metadata: metadata ?? this.metadata,
    );
  }

  SectionDataModel appendData(SectionDataModel newData) {
    return SectionDataModel(
      sectionId: sectionId,
      sectionType: sectionType,
      properties: [...properties, ...newData.properties],
      sponsoredAds: [...sponsoredAds, ...newData.sponsoredAds],
      specialOffers: [...specialOffers, ...newData.specialOffers],
      destinations: [...destinations, ...newData.destinations],
      customData: {...customData, ...newData.customData},
      lastUpdated: newData.lastUpdated,
      nextPageToken: newData.nextPageToken,
      hasMore: newData.hasMore,
      totalCount: newData.totalCount,
      metadata: {...metadata, ...newData.metadata},
    );
  }

  @override
  List<Object?> get props => [
        sectionId,
        sectionType,
        properties,
        sponsoredAds,
        specialOffers,
        destinations,
        customData,
        lastUpdated,
        nextPageToken,
        hasMore,
        totalCount,
        metadata,
      ];
}