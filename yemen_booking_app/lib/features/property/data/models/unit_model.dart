import 'package:equatable/equatable.dart';
import '../../domain/entities/unit.dart';

/// <summary>
/// نموذج بيانات المبلغ المالي
/// Money data model
/// </summary>
class MoneyDto extends Equatable {
  /// <summary>
  /// المبلغ المالي
  /// Monetary amount
  /// </summary>
  final double amount;

  /// <summary>
  /// رمز العملة
  /// Currency code
  /// </summary>
  final String currency;

  const MoneyDto({
    required this.amount,
    required this.currency,
  });

  factory MoneyDto.fromJson(Map<String, dynamic> json) {
    return MoneyDto(
      amount: (json['amount'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'YER',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'amount': amount,
      'currency': currency,
    };
  }

  /// <summary>
  /// المبلغ المنسق للعرض
  /// Formatted amount for display
  /// </summary>
  String get formattedAmount => '${amount.toStringAsFixed(2)} $currency';

  @override
  List<Object> get props => [amount, currency];
}

/// <summary>
/// نموذج بيانات صورة الوحدة
/// Unit image data model
/// </summary>
class UnitImageDto extends Equatable {
  /// <summary>
  /// معرف الصورة
  /// Image ID
  /// </summary>
  final String id;

  /// <summary>
  /// رابط الصورة
  /// Image URL
  /// </summary>
  final String imageUrl;

  /// <summary>
  /// وصف الصورة
  /// Image description
  /// </summary>
  final String? description;

  /// <summary>
  /// ترتيب العرض
  /// Display order
  /// </summary>
  final int displayOrder;

  const UnitImageDto({
    required this.id,
    required this.imageUrl,
    this.description,
    required this.displayOrder,
  });

  factory UnitImageDto.fromJson(Map<String, dynamic> json) {
    return UnitImageDto(
      id: json['id'] ?? '',
      imageUrl: json['imageUrl'] ?? '',
      description: json['description'],
      displayOrder: json['displayOrder'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'imageUrl': imageUrl,
      'description': description,
      'displayOrder': displayOrder,
    };
  }

  @override
  List<Object?> get props => [id, imageUrl, description, displayOrder];
}

/// <summary>
/// نموذج بيانات قيمة حقل الوحدة
/// Unit field value data model
/// </summary>
class UnitFieldValueDto extends Equatable {
  /// <summary>
  /// معرف القيمة
  /// Value ID
  /// </summary>
  final String valueId;

  /// <summary>
  /// معرف الوحدة
  /// Unit ID
  /// </summary>
  final String unitId;

  /// <summary>
  /// معرف الحقل
  /// Field ID
  /// </summary>
  final String fieldId;

  /// <summary>
  /// اسم الحقل
  /// Field name
  /// </summary>
  final String fieldName;

  /// <summary>
  /// الاسم المعروض للحقل
  /// Display name
  /// </summary>
  final String displayName;

  /// <summary>
  /// قيمة الحقل
  /// Field value
  /// </summary>
  final String value;

  /// <summary>
  /// نوع الحقل
  /// Field type
  /// </summary>
  final String fieldType;

  /// <summary>
  /// تاريخ الإنشاء
  /// Created at
  /// </summary>
  final DateTime createdAt;

  /// <summary>
  /// تاريخ التحديث
  /// Updated at
  /// </summary>
  final DateTime updatedAt;

  const UnitFieldValueDto({
    required this.valueId,
    required this.unitId,
    required this.fieldId,
    required this.fieldName,
    required this.displayName,
    required this.value,
    required this.fieldType,
    required this.createdAt,
    required this.updatedAt,
  });

  factory UnitFieldValueDto.fromJson(Map<String, dynamic> json) {
    return UnitFieldValueDto(
      valueId: json['valueId'] ?? '',
      unitId: json['unitId'] ?? '',
      fieldId: json['fieldId'] ?? '',
      fieldName: json['fieldName'] ?? '',
      displayName: json['displayName'] ?? '',
      value: json['value'] ?? '',
      fieldType: json['fieldType'] ?? '',
      createdAt: DateTime.parse(json['createdAt'] ?? DateTime.now().toIso8601String()),
      updatedAt: DateTime.parse(json['updatedAt'] ?? DateTime.now().toIso8601String()),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'valueId': valueId,
      'unitId': unitId,
      'fieldId': fieldId,
      'fieldName': fieldName,
      'displayName': displayName,
      'value': value,
      'fieldType': fieldType,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  @override
  List<Object> get props => [valueId, unitId, fieldId, fieldName, displayName, value, fieldType, createdAt, updatedAt];
}

/// <summary>
/// نموذج بيانات نوع الوحدة
/// Unit type data model
/// </summary>
class UnitTypeDto extends Equatable {
  /// <summary>
  /// معرف نوع الوحدة
  /// Unit type ID
  /// </summary>
  final String id;

  /// <summary>
  /// اسم نوع الوحدة
  /// Unit type name
  /// </summary>
  final String name;

  /// <summary>
  /// وصف نوع الوحدة
  /// Unit type description
  /// </summary>
  final String? description;

  const UnitTypeDto({
    required this.id,
    required this.name,
    this.description,
  });

  factory UnitTypeDto.fromJson(Map<String, dynamic> json) {
    return UnitTypeDto(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
    };
  }

  @override
  List<Object?> get props => [id, name, description];
}

/// <summary>
/// نموذج بيانات الوحدة (متوافق مع UnitDetailsDto)
/// Unit data model (compatible with UnitDetailsDto)
/// </summary>
class UnitModel extends Equatable {
  /// <summary>
  /// معرف الوحدة
  /// Unit ID
  /// </summary>
  final String id;

  /// <summary>
  /// معرف العقار
  /// Property ID
  /// </summary>
  final String propertyId;

  /// <summary>
  /// اسم العقار
  /// Property name
  /// </summary>
  final String propertyName;

  /// <summary>
  /// اسم الوحدة
  /// Unit name
  /// </summary>
  final String name;

  /// <summary>
  /// نوع الوحدة
  /// Unit type
  /// </summary>
  final UnitTypeDto unitType;

  /// <summary>
  /// معرف نوع الوحدة
  /// Unit type ID
  /// </summary>
  final String unitTypeId;

  /// <summary>
  /// اسم نوع الوحدة
  /// Unit type name
  /// </summary>
  final String unitTypeName;

  /// <summary>
  /// السعر الأساسي
  /// Base price
  /// </summary>
  final MoneyDto basePrice;

  /// <summary>
  /// العملة (اختياري)
  /// Currency (optional)
  /// </summary>
  final String? currency;

  /// <summary>
  /// السعة القصوى
  /// Maximum capacity
  /// </summary>
  final int maxCapacity;

  /// <summary>
  /// عدد المشاهدات
  /// View count
  /// </summary>
  final int viewCount;

  /// <summary>
  /// عدد الحجوزات
  /// Booking count
  /// </summary>
  final int bookingCount;

  /// <summary>
  /// طريقة حساب السعر
  /// Pricing method
  /// </summary>
  final String pricingMethod;

  /// <summary>
  /// حالة التوفر
  /// Availability status
  /// </summary>
  final bool isAvailable;

  /// <summary>
  /// صور الوحدة
  /// Unit images
  /// </summary>
  final List<UnitImageDto> images;

  /// <summary>
  /// الميزات المخصصة (JSON string)
  /// Custom features (JSON string)
  /// </summary>
  final String customFeatures;

  /// <summary>
  /// قيم الحقول الديناميكية
  /// Dynamic field values
  /// </summary>
  final List<UnitFieldValueDto> fieldValues;

  const UnitModel({
    required this.id,
    required this.propertyId,
    required this.propertyName,
    required this.name,
    required this.unitType,
    required this.unitTypeId,
    required this.unitTypeName,
    required this.basePrice,
    this.currency,
    required this.maxCapacity,
    required this.viewCount,
    required this.bookingCount,
    required this.pricingMethod,
    required this.isAvailable,
    required this.images,
    required this.customFeatures,
    required this.fieldValues,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory UnitModel.fromJson(Map<String, dynamic> json) {
    return UnitModel(
      id: json['id'] ?? '',
      propertyId: json['propertyId'] ?? '',
      propertyName: json['propertyName'] ?? '',
      name: json['name'] ?? '',
      unitType: UnitTypeDto.fromJson(json['unitType'] ?? {}),
      unitTypeId: json['unitTypeId'] ?? '',
      unitTypeName: json['unitTypeName'] ?? '',
      basePrice: MoneyDto.fromJson(json['basePrice'] ?? {}),
      currency: json['currency'],
      maxCapacity: json['maxCapacity'] ?? 1,
      viewCount: json['viewCount'] ?? 0,
      bookingCount: json['bookingCount'] ?? 0,
      pricingMethod: json['pricingMethod'] ?? '',
      isAvailable: json['isAvailable'] ?? true,
      images: (json['images'] as List<dynamic>?)
          ?.map((item) => UnitImageDto.fromJson(item))
          .toList() ?? [],
      customFeatures: json['customFeatures'] ?? '{}',
      fieldValues: (json['fieldValues'] as List<dynamic>?)
          ?.map((item) => UnitFieldValueDto.fromJson(item))
          .toList() ?? [],
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'propertyId': propertyId,
      'propertyName': propertyName,
      'name': name,
      'unitType': unitType.toJson(),
      'unitTypeId': unitTypeId,
      'unitTypeName': unitTypeName,
      'basePrice': basePrice.toJson(),
      'currency': currency,
      'maxCapacity': maxCapacity,
      'viewCount': viewCount,
      'bookingCount': bookingCount,
      'pricingMethod': pricingMethod,
      'isAvailable': isAvailable,
      'images': images.map((item) => item.toJson()).toList(),
      'customFeatures': customFeatures,
      'fieldValues': fieldValues.map((item) => item.toJson()).toList(),
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  Unit toEntity() {
    return Unit(
      id: id,
      propertyId: propertyId,
      propertyName: propertyName,
      name: name,
      unitType: unitType,
      unitTypeId: unitTypeId,
      unitTypeName: unitTypeName,
      basePrice: basePrice,
      currency: currency,
      maxCapacity: maxCapacity,
      viewCount: viewCount,
      bookingCount: bookingCount,
      pricingMethod: pricingMethod,
      isAvailable: isAvailable,
      images: images,
      customFeatures: customFeatures,
      fieldValues: fieldValues,
    );
  }

  /// <summary>
  /// الحصول على قيمة حقل معين
  /// Get specific field value
  /// </summary>
  String? getFieldValue(String fieldName) {
    final field = fieldValues.where((f) => f.fieldName == fieldName).firstOrNull;
    return field?.value;
  }

  /// <summary>
  /// التحقق من وجود قيمة حقل معين
  /// Check if field has value
  /// </summary>
  bool hasField(String fieldName) {
    return fieldValues.any((f) => f.fieldName == fieldName && f.value.isNotEmpty);
  }

  @override
  List<Object?> get props => [
    id,
    propertyId,
    propertyName,
    name,
    unitType,
    unitTypeId,
    unitTypeName,
    basePrice,
    currency,
    maxCapacity,
    viewCount,
    bookingCount,
    pricingMethod,
    isAvailable,
    images,
    customFeatures,
    fieldValues,
  ];

  UnitModel copyWith({
    String? id,
    String? propertyId,
    String? propertyName,
    String? name,
    UnitTypeDto? unitType,
    String? unitTypeId,
    String? unitTypeName,
    MoneyDto? basePrice,
    String? currency,
    int? maxCapacity,
    int? viewCount,
    int? bookingCount,
    String? pricingMethod,
    bool? isAvailable,
    List<UnitImageDto>? images,
    String? customFeatures,
    List<UnitFieldValueDto>? fieldValues,
  }) {
    return UnitModel(
      id: id ?? this.id,
      propertyId: propertyId ?? this.propertyId,
      propertyName: propertyName ?? this.propertyName,
      name: name ?? this.name,
      unitType: unitType ?? this.unitType,
      unitTypeId: unitTypeId ?? this.unitTypeId,
      unitTypeName: unitTypeName ?? this.unitTypeName,
      basePrice: basePrice ?? this.basePrice,
      currency: currency ?? this.currency,
      maxCapacity: maxCapacity ?? this.maxCapacity,
      viewCount: viewCount ?? this.viewCount,
      bookingCount: bookingCount ?? this.bookingCount,
      pricingMethod: pricingMethod ?? this.pricingMethod,
      isAvailable: isAvailable ?? this.isAvailable,
      images: images ?? this.images,
      customFeatures: customFeatures ?? this.customFeatures,
      fieldValues: fieldValues ?? this.fieldValues,
    );
  }
}