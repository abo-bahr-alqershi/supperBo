// lib/features/home/domain/entities/section_type.dart

import 'package:equatable/equatable.dart';

class SectionType extends Equatable {
  final String id;
  final String name;
  final String nameAr;
  final String category;
  final String categoryAr;
  final String description;
  final String descriptionAr;
  final bool isActive;
  final Map<String, dynamic> defaultConfiguration;
  final List<String> supportedLayoutTypes;
  final List<String> requiredFields;
  final List<String> optionalFields;
  final Map<String, dynamic> validationRules;
  final Map<String, dynamic> displayOptions;
  final bool requiresPropertyData;
  final bool supportsCustomContent;
  final bool isTimeSensitive;
  final int maxItems;
  final int minItems;
  final Map<String, dynamic> metadata;

  const SectionType({
    required this.id,
    required this.name,
    required this.nameAr,
    required this.category,
    required this.categoryAr,
    required this.description,
    required this.descriptionAr,
    this.isActive = true,
    required this.defaultConfiguration,
    required this.supportedLayoutTypes,
    required this.requiredFields,
    required this.optionalFields,
    required this.validationRules,
    required this.displayOptions,
    this.requiresPropertyData = false,
    this.supportsCustomContent = false,
    this.isTimeSensitive = false,
    this.maxItems = 10,
    this.minItems = 1,
    required this.metadata,
  });

  String getLocalizedName(bool isArabic) => isArabic ? nameAr : name;
  String getLocalizedCategory(bool isArabic) => isArabic ? categoryAr : category;
  String getLocalizedDescription(bool isArabic) => isArabic ? descriptionAr : description;

  bool supportsLayoutType(String layoutType) => supportedLayoutTypes.contains(layoutType);

  bool hasRequiredField(String field) => requiredFields.contains(field);

  bool hasOptionalField(String field) => optionalFields.contains(field);

  List<String> get allFields => [...requiredFields, ...optionalFields];

  bool isValidItemCount(int count) => count >= minItems && count <= maxItems;

  T? getDefaultConfigValue<T>(String key) {
    try {
      return defaultConfiguration[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getValidationRule<T>(String key) {
    try {
      return validationRules[key] as T?;
    } catch (_) {
      return null;
    }
  }

  T? getDisplayOption<T>(String key) {
    try {
      return displayOptions[key] as T?;
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

  Map<String, dynamic> get configurationWithDefaults {
    return {
      'maxItems': maxItems,
      'minItems': minItems,
      'requiresPropertyData': requiresPropertyData,
      'supportsCustomContent': supportsCustomContent,
      'isTimeSensitive': isTimeSensitive,
      'supportedLayoutTypes': supportedLayoutTypes,
      ...defaultConfiguration,
    };
  }

  @override
  List<Object?> get props => [
        id,
        name,
        nameAr,
        category,
        categoryAr,
        description,
        descriptionAr,
        isActive,
        defaultConfiguration,
        supportedLayoutTypes,
        requiredFields,
        optionalFields,
        validationRules,
        displayOptions,
        requiresPropertyData,
        supportsCustomContent,
        isTimeSensitive,
        maxItems,
        minItems,
        metadata,
      ];
}