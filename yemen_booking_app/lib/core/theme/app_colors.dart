import 'package:flutter/material.dart';

class AppColors {
  AppColors._();
  
  // Primary Colors
  static const Color primary = Color(0xFF2E7D32);
  static const Color primaryLight = Color(0xFF4CAF50);
  static const Color primaryDark = Color(0xFF1B5E20);
  
  // Secondary Colors
  static const Color secondary = Color(0xFFFF6F00);
  static const Color secondaryLight = Color(0xFFFFB300);
  static const Color secondaryDark = Color(0xFFE65100);
  
  // Accent Colors
  static const Color accent = Color(0xFF00BCD4);
  static const Color accentLight = Color(0xFF4DD0E1);
  static const Color accentDark = Color(0xFF00838F);
  
  // Background Colors
  static const Color background = Color(0xFFF5F5F5);
  static const Color backgroundDark = Color(0xFF121212);
  static const Color surface = Color(0xFFFFFFFF);
  static const Color surfaceDark = Color(0xFF1E1E1E);
  static const Color scaffoldBackgroundLight = Color(0xFFF5F5F5);
  static const Color cardBackgroundLight = Color(0xFFFFFFFF);
  
  // Text Colors
  static const Color textPrimary = Color(0xFF212121);
  static const Color textPrimaryLight = Color(0xFF212121);
  static const Color textSecondary = Color(0xFF757575);
  static const Color textHint = Color(0xFFBDBDBD);
  static const Color textDisabled = Color(0xFFE0E0E0);
  static const Color textPrimaryDark = Color(0xFFFFFFFF);
  static const Color textSecondaryDark = Color(0xFFBDBDBD);
  
  // Status Colors
  static const Color success = Color(0xFF4CAF50);
  static const Color warning = Color(0xFFFFC107);
  static const Color error = Color(0xFFF44336);
  static const Color info = Color(0xFF2196F3);
  
  // UI Colors
  static const Color white = Color(0xFFFFFFFF);
  static const Color black = Color(0xFF000000);
  static const Color transparent = Colors.transparent;
  static const Color divider = Color(0xFFE0E0E0);
  static const Color border = Color(0xFFE0E0E0);
  static const Color shadow = Color(0x1A000000);
  static const Color overlay = Color(0x80000000);
  
  // Component Colors
  static const Color inputBackground = Color(0xFFF5F5F5);
  static const Color inputBackgroundDark = Color(0xFF2C2C2C);
  static const Color cardBackground = Color(0xFFFFFFFF);
  static const Color cardBackgroundDark = Color(0xFF2C2C2C);
  static const Color chipBackground = Color(0xFFE0E0E0);
  static const Color chipBackgroundDark = Color(0xFF424242);
  
  // Special Colors
  static const Color shimmer = Color(0xFFE0E0E0);
  static const Color shimmerDark = Color(0xFF424242);
  static const Color disabled = Color(0xFFBDBDBD);
  static const Color gray200 = Color(0xFFEEEEEE);
  static const Color iconDefault = Color(0xFF757575);
  static const Color iconDefaultDark = Color(0xFFBDBDBD);
  static const Color progressBackground = Color(0xFFE0E0E0);
  static const Color progressBackgroundDark = Color(0xFF424242);
  
  // Rating Colors
  static const Color ratingStar = Color(0xFFFFC107);
  static const Color ratingEmpty = Color(0xFFE0E0E0);
  
  // Map Colors
  static const Color mapMarker = Color(0xFFF44336);
  static const Color mapRoute = Color(0xFF2196F3);
  static const Color mapSelected = Color(0xFF4CAF50);
  
  // Booking Status Colors
  static const Color bookingPending = Color(0xFFFFC107);
  static const Color bookingConfirmed = Color(0xFF4CAF50);
  static const Color bookingCancelled = Color(0xFFF44336);
  static const Color bookingCompleted = Color(0xFF2196F3);
  
  // Gradients
  static const LinearGradient primaryGradient = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: [primary, primaryLight],
  );
  
  static const LinearGradient secondaryGradient = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: [secondary, secondaryLight],
  );
  
  static const LinearGradient overlayGradient = LinearGradient(
    begin: Alignment.topCenter,
    end: Alignment.bottomCenter,
    colors: [transparent, overlay],
  );
}