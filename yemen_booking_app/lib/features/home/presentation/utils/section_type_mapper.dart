// lib/features/home/presentation/widgets/utils/section_type_mapper.dart

import '../../../../../core/enums/section_type_enum.dart';

class SectionTypeMapper {
  static SectionType fromString(String type) {
    return SectionType.tryFromString(type) ?? SectionType.horizontalPropertyList;
  }

  static String toString(SectionType type) {
    return type.value;
  }

  static String getDisplayName(SectionType type) {
    switch (type) {
      case SectionType.singlePropertyAd:
        return 'إعلان عقار واحد';
      case SectionType.multiPropertyAd:
        return 'عقارات متعددة';
      case SectionType.horizontalPropertyList:
        return 'قائمة عقارات أفقية';
      case SectionType.verticalPropertyGrid:
        return 'شبكة عقارات عمودية';
      default:
        return type.value;
    }
  }
}