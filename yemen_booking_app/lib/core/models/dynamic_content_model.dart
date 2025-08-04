// lib/core/models/dynamic_content_model.dart

import 'package:equatable/equatable.dart';

class DynamicContentModel extends Equatable {
  final String id;
  final String sectionId;
  final String contentType;
  final Map<String, dynamic> data;
  final Map<String, dynamic> metadata;
  final DateTime? expiresAt;
  final DateTime createdAt;
  final DateTime updatedAt;

  const DynamicContentModel({
    required this.id,
    required this.sectionId,
    required this.contentType,
    required this.data,
    required this.metadata,
    this.expiresAt,
    required this.createdAt,
    required this.updatedAt,
  });

  // Check if content is expired
  bool get isExpired {
    if (expiresAt == null) return false;
    return DateTime.now().isAfter(expiresAt!);
  }

  // Check if content is valid
  bool get isValid => !isExpired && data.isNotEmpty;

  // Get typed data
  T? getData<T>(String key) {
    try {
      return data[key] as T?;
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
      data: json['data'] as Map<String, dynamic>,
      metadata: json['metadata'] as Map<String, dynamic>? ?? {},
      expiresAt: json['expiresAt'] != null
          ? DateTime.parse(json['expiresAt'] as String)
          : null,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'sectionId': sectionId,
      'contentType': contentType,
      'data': data,
      'metadata': metadata,
      'expiresAt': expiresAt?.toIso8601String(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  DynamicContentModel copyWith({
    String? id,
    String? sectionId,
    String? contentType,
    Map<String, dynamic>? data,
    Map<String, dynamic>? metadata,
    DateTime? expiresAt,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return DynamicContentModel(
      id: id ?? this.id,
      sectionId: sectionId ?? this.sectionId,
      contentType: contentType ?? this.contentType,
      data: data ?? this.data,
      metadata: metadata ?? this.metadata,
      expiresAt: expiresAt ?? this.expiresAt,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [
        id,
        sectionId,
        contentType,
        data,
        metadata,
        expiresAt,
        createdAt,
        updatedAt,
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