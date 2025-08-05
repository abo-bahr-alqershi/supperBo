// lib/core/models/dynamic_content_model.dart

import 'package:equatable/equatable.dart';

class DynamicContentModel extends Equatable {
  final String id;
  final String sectionId;
  final String contentType;
  final Map<String, dynamic> contentData; // Match backend naming
  final Map<String, dynamic> metadata;
  final DateTime? expiresAt;
  final int displayOrder; // Match backend
  final bool isActive; // Match backend
  final DateTime createdAt;
  final DateTime updatedAt;
  
  // Computed properties from backend
  final bool? isValid;
  final bool? isExpired;

  const DynamicContentModel({
    required this.id,
    required this.sectionId,
    required this.contentType,
    required this.contentData,
    required this.metadata,
    this.expiresAt,
    required this.displayOrder,
    required this.isActive,
    required this.createdAt,
    required this.updatedAt,
    this.isValid,
    this.isExpired,
  });

  // Check if content is expired
  bool get isCurrentlyExpired {
    if (expiresAt == null) return false;
    return DateTime.now().isAfter(expiresAt!);
  }

  // Check if content is valid
  bool get isCurrentlyValid => isActive && !isCurrentlyExpired && contentData.isNotEmpty;

  // Get typed data
  T? getData<T>(String key) {
    try {
      return contentData[key] as T?;
    } catch (_) {
      return null;
    }
  }

  // Get typed metadata
  T? getMetadata<T>(String key) {
    try {
      return metadata[key] as T?;
    } catch (_) {
      return null;
    }
  }

  // Common data getters
  String? get title => getData<String>('title');
  String? get description => getData<String>('description');
  String? get imageUrl => getData<String>('imageUrl');
  double? get price => getData<double>('price');
  double? get discountPercentage => getData<double>('discountPercentage');
  String? get ctaText => getData<String>('ctaText');
  String? get ctaAction => getData<String>('ctaAction');
  
  // Metadata getters
  int? get priority => getMetadata<int>('priority');
  List<String>? get tags => (metadata['tags'] as List<dynamic>?)?.cast<String>();
  String? get targetAudience => getMetadata<String>('targetAudience');
  Map<String, dynamic>? get analytics => getMetadata<Map<String, dynamic>>('analytics');

  factory DynamicContentModel.fromJson(Map<String, dynamic> json) {
    return DynamicContentModel(
      id: json['id'] as String,
      sectionId: json['sectionId'] as String,
      contentType: json['contentType'] as String,
      contentData: json['contentData'] as Map<String, dynamic>,
      metadata: json['metadata'] as Map<String, dynamic>? ?? {},
      expiresAt: json['expiresAt'] != null
          ? DateTime.parse(json['expiresAt'] as String)
          : null,
      displayOrder: json['displayOrder'] as int,
      isActive: json['isActive'] as bool,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      // Backend computed properties
      isValid: json['isValid'] as bool?,
      isExpired: json['isExpired'] as bool?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'sectionId': sectionId,
      'contentType': contentType,
      'contentData': contentData,
      'metadata': metadata,
      'expiresAt': expiresAt?.toIso8601String(),
      'displayOrder': displayOrder,
      'isActive': isActive,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      // Include computed properties if available
      if (isValid != null) 'isValid': isValid,
      if (isExpired != null) 'isExpired': isExpired,
    };
  }

  DynamicContentModel copyWith({
    String? id,
    String? sectionId,
    String? contentType,
    Map<String, dynamic>? contentData,
    Map<String, dynamic>? metadata,
    DateTime? expiresAt,
    int? displayOrder,
    bool? isActive,
    DateTime? createdAt,
    DateTime? updatedAt,
    bool? isValid,
    bool? isExpired,
  }) {
    return DynamicContentModel(
      id: id ?? this.id,
      sectionId: sectionId ?? this.sectionId,
      contentType: contentType ?? this.contentType,
      contentData: contentData ?? this.contentData,
      metadata: metadata ?? this.metadata,
      expiresAt: expiresAt ?? this.expiresAt,
      displayOrder: displayOrder ?? this.displayOrder,
      isActive: isActive ?? this.isActive,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      isValid: isValid ?? this.isValid,
      isExpired: isExpired ?? this.isExpired,
    );
  }

  @override
  List<Object?> get props => [
        id,
        sectionId,
        contentType,
        contentData,
        metadata,
        expiresAt,
        displayOrder,
        isActive,
        createdAt,
        updatedAt,
        isValid,
        isExpired,
      ];
}

// Content type constants
class DynamicContentTypes {
  static const String property = 'PROPERTY';
  static const String offer = 'OFFER';
  static const String advertisement = 'ADVERTISEMENT';
  static const String destination = 'DESTINATION';
  static const String announcement = 'ANNOUNCEMENT';
  static const String banner = 'BANNER';
  static const String promotion = 'PROMOTION';
}