import '../../domain/entities/unit.dart';
import 'dart:convert';

class UnitModel extends Unit {
  const UnitModel({
    required super.id,
    required super.propertyId,
    required super.unitTypeId,
    required super.name,
    required super.basePrice,
    required super.customFeatures,
    required super.isAvailable,
    required super.propertyName,
    required super.unitTypeName,
    required super.pricingMethod,
    required super.fieldValues,
    required super.dynamicFields,
    super.distanceKm,
    required super.images,
  });

  factory UnitModel.fromJson(Map<String, dynamic> json) {
    final unitTypeJson = json['unitType'] as Map<String, dynamic>?;
    final basePriceJson = unitTypeJson == null
        ? (json['basePrice'] as Map<String, dynamic>?)
        : {
            'amount': json['pricePerNight'] ?? 0,
            'currency': json['currency'] ?? 'YER',
            'exchangeRate': 1.0,
          };
    return UnitModel(
      id: json['id'] ?? '',
      propertyId: json['propertyId'] ?? '',
      unitTypeId: unitTypeJson?['id'] ?? json['unitTypeId'] ?? '',
      name: json['name'] ?? '',
      basePrice: MoneyModel.fromJson(basePriceJson ?? {}),
      customFeatures: json['customFeatures'] is String
          ? json['customFeatures']
          : jsonEncode(json['customFeatures'] ?? {}),
      isAvailable: json['isAvailable'] ?? true,
      propertyName: json['propertyName'] ?? '',
      unitTypeName: unitTypeJson?['name'] ?? json['unitTypeName'] ?? '',
      pricingMethod: _parsePricingMethod(json['pricingMethod']),
      fieldValues: (json['fieldValues'] as List?)
              ?.map((e) => UnitFieldValueModel.fromJson(e))
              .toList() ?? [],
      dynamicFields: (json['dynamicFields'] as List?)
              ?.map((e) => FieldGroupWithValuesModel.fromJson(e))
              .toList() ?? [],
      distanceKm: json['distanceKm']?.toDouble(),
      images: (json['images'] as List?)
              ?.map((e) => UnitImageModel.fromJson(e))
              .toList() ?? [],
    );
  }

  static PricingMethod _parsePricingMethod(dynamic method) {
    if (method == null) return PricingMethod.daily;
    switch (method.toString().toLowerCase()) {
      case 'hourly':
        return PricingMethod.hourly;
      case 'weekly':
        return PricingMethod.weekly;
      case 'monthly':
        return PricingMethod.monthly;
      default:
        return PricingMethod.daily;
    }
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'propertyId': propertyId,
      'unitTypeId': unitTypeId,
      'name': name,
      'basePrice': (basePrice as MoneyModel).toJson(),
      'customFeatures': customFeatures,
      'isAvailable': isAvailable,
      'propertyName': propertyName,
      'unitTypeName': unitTypeName,
      'pricingMethod': pricingMethod.name,
      'fieldValues': fieldValues.map((e) => (e as UnitFieldValueModel).toJson()).toList(),
      'dynamicFields': dynamicFields.map((e) => (e as FieldGroupWithValuesModel).toJson()).toList(),
      'distanceKm': distanceKm,
      'images': images.map((e) => (e as UnitImageModel).toJson()).toList(),
    };
  }
}

class MoneyModel extends Money {
  const MoneyModel({
    required super.amount,
    super.currency,
    super.exchangeRate,
  });

  factory MoneyModel.fromJson(Map<String, dynamic> json) {
    return MoneyModel(
      amount: (json['amount'] ?? 0).toDouble(),
      currency: json['currency'] ?? 'YER',
      exchangeRate: (json['exchangeRate'] ?? 1.0).toDouble(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'amount': amount,
      'currency': currency,
      'exchangeRate': exchangeRate,
      'formattedAmount': formattedAmount,
    };
  }
}

class UnitFieldValueModel extends UnitFieldValue {
  const UnitFieldValueModel({
    required super.valueId,
    required super.fieldId,
    required super.fieldName,
    required super.displayName,
    required super.value,
  });

  factory UnitFieldValueModel.fromJson(Map<String, dynamic> json) {
    return UnitFieldValueModel(
      valueId: json['valueId'] ?? '',
      fieldId: json['fieldId'] ?? '',
      fieldName: json['fieldName'] ?? '',
      displayName: json['displayName'] ?? '',
      value: json['value'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'valueId': valueId,
      'fieldId': fieldId,
      'fieldName': fieldName,
      'displayName': displayName,
      'value': value,
    };
  }
}

class FieldGroupWithValuesModel extends FieldGroupWithValues {
  const FieldGroupWithValuesModel({
    required super.groupId,
    required super.groupName,
    required super.displayName,
    required super.description,
    required super.fieldValues,
  });

  factory FieldGroupWithValuesModel.fromJson(Map<String, dynamic> json) {
    return FieldGroupWithValuesModel(
      groupId: json['groupId'] ?? '',
      groupName: json['groupName'] ?? '',
      displayName: json['displayName'] ?? '',
      description: json['description'] ?? '',
      fieldValues: (json['fieldValues'] as List?)
              ?.map((e) => UnitFieldValueModel.fromJson(e))
              .toList() ??
          [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'groupId': groupId,
      'groupName': groupName,
      'displayName': displayName,
      'description': description,
      'fieldValues': fieldValues.map((e) => (e as UnitFieldValueModel).toJson()).toList(),
    };
  }
}

class UnitImageModel extends UnitImage {
  const UnitImageModel({
    required super.id,
    required super.url,
    required super.caption,
    required super.isMain,
    required super.displayOrder,
  });

  factory UnitImageModel.fromJson(Map<String, dynamic> json) {
    return UnitImageModel(
      id: json['id'] ?? '',
      url: json['url'] ?? '',
      caption: json['caption'] ?? '',
      isMain: json['isMain'] ?? false,
      displayOrder: json['displayOrder'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'url': url,
      'caption': caption,
      'isMain': isMain,
      'displayOrder': displayOrder,
    };
  }
}