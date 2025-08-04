// lib/core/enums/section_size_enum.dart

import 'package:flutter/material.dart';

enum SectionSize {
  compact('COMPACT', 0.5),
  small('SMALL', 0.75),
  medium('MEDIUM', 1.0),
  large('LARGE', 1.25),
  extraLarge('EXTRA_LARGE', 1.5),
  fullScreen('FULL_SCREEN', 2.0);

  final String value;
  final double scaleFactor;

  const SectionSize(this.value, this.scaleFactor);

  static SectionSize fromString(String value) {
    return SectionSize.values.firstWhere(
      (size) => size.value == value,
      orElse: () => SectionSize.medium,
    );
  }

  static SectionSize? tryFromString(String? value) {
    if (value == null) return null;
    try {
      return fromString(value);
    } catch (_) {
      return null;
    }
  }

  // Get scaled value based on base value
  double scale(double baseValue) => baseValue * scaleFactor;

  // Get appropriate text scale factor
  double get textScaleFactor {
    switch (this) {
      case SectionSize.compact:
        return 0.85;
      case SectionSize.small:
        return 0.9;
      case SectionSize.medium:
        return 1.0;
      case SectionSize.large:
        return 1.1;
      case SectionSize.extraLarge:
        return 1.2;
      case SectionSize.fullScreen:
        return 1.3;
    }
  }

  // Get appropriate spacing
  double get spacing {
    switch (this) {
      case SectionSize.compact:
        return 8.0;
      case SectionSize.small:
        return 12.0;
      case SectionSize.medium:
        return 16.0;
      case SectionSize.large:
        return 20.0;
      case SectionSize.extraLarge:
        return 24.0;
      case SectionSize.fullScreen:
        return 32.0;
    }
  }

  // Get appropriate padding
  EdgeInsets get padding {
    switch (this) {
      case SectionSize.compact:
        return const EdgeInsets.all(8.0);
      case SectionSize.small:
        return const EdgeInsets.all(12.0);
      case SectionSize.medium:
        return const EdgeInsets.all(16.0);
      case SectionSize.large:
        return const EdgeInsets.all(20.0);
      case SectionSize.extraLarge:
        return const EdgeInsets.all(24.0);
      case SectionSize.fullScreen:
        return const EdgeInsets.all(32.0);
    }
  }

  // Get appropriate border radius
  double get borderRadius {
    switch (this) {
      case SectionSize.compact:
        return 6.0;
      case SectionSize.small:
        return 8.0;
      case SectionSize.medium:
        return 12.0;
      case SectionSize.large:
        return 16.0;
      case SectionSize.extraLarge:
        return 20.0;
      case SectionSize.fullScreen:
        return 0.0;
    }
  }

  // Check if size is considered small
  bool get isSmall => [SectionSize.compact, SectionSize.small].contains(this);

  // Check if size is considered large
  bool get isLarge => [SectionSize.large, SectionSize.extraLarge, SectionSize.fullScreen].contains(this);
}

// Extension for responsive sizing
extension SectionSizeExtension on SectionSize {
  SectionSize getResponsiveSize(double screenWidth) {
    if (screenWidth < 360) {
      // Small phones
      switch (this) {
        case SectionSize.extraLarge:
        case SectionSize.fullScreen:
          return SectionSize.large;
        case SectionSize.large:
          return SectionSize.medium;
        default:
          return this;
      }
    } else if (screenWidth < 600) {
      // Normal phones
      return this;
    } else if (screenWidth < 900) {
      // Tablets
      switch (this) {
        case SectionSize.compact:
          return SectionSize.small;
        case SectionSize.small:
          return SectionSize.medium;
        default:
          return this;
      }
    } else {
      // Large tablets and desktops
      switch (this) {
        case SectionSize.compact:
          return SectionSize.medium;
        case SectionSize.small:
          return SectionSize.medium;
        case SectionSize.medium:
          return SectionSize.large;
        default:
          return this;
      }
    }
  }
}