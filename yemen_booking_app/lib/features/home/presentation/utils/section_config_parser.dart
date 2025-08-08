class SectionConfigParser {
  static bool showViewAll(Map<String, dynamic> displaySettings) {
    return (displaySettings['showViewAllButton'] ?? displaySettings['showViewAll'] ?? false) == true;
  }

  static int itemsPerRow(Map<String, dynamic> layoutSettings, {int fallback = 2}) {
    return (layoutSettings['itemsPerRow'] as int?) ?? fallback;
  }

  static double itemSpacing(Map<String, dynamic> layoutSettings, {double fallback = 12}) {
    final v = layoutSettings['itemSpacing'];
    if (v is num) return v.toDouble();
    return fallback;
  }
}