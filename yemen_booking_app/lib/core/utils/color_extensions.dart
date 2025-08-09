import 'package:flutter/material.dart';

extension ColorExtensions on Color {
  // Fallback for older Flutter where withValues may not exist
  Color withValues({double? alpha, double? red, double? green, double? blue}) {
    final int a = (255 * (alpha ?? (this.alpha / 255))).clamp(0, 255).toInt();
    final int r = (red != null ? (255 * red).clamp(0, 255).toInt() : this.red);
    final int g = (green != null ? (255 * green).clamp(0, 255).toInt() : this.green);
    final int b = (blue != null ? (255 * blue).clamp(0, 255).toInt() : this.blue);
    return Color.fromARGB(a, r, g, b);
  }

  Color withAlphaFraction(double alpha) => withValues(alpha: alpha);
}