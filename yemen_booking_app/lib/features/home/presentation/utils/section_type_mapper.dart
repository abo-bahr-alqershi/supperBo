import '../../../core/enums/section_type_enum.dart' as core;

class SectionTypeMapper {
  static core.SectionType? tryParse(String? value) {
    if (value == null) return null;
    return core.SectionType.tryFromString(value);
  }

  static String toDisplayName(core.SectionType type) {
    return type.value.replaceAll('_', ' ');
  }
}