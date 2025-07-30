import 'package:flutter/material.dart';
import 'app_colors.dart';

class AppTextStyles {
  // Display Styles  
  static const TextStyle displayLarge = TextStyle(
    fontSize: 57,
    fontWeight: FontWeight.w400,
    height: 1.12,
    letterSpacing: -0.25,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle displayMedium = TextStyle(
    fontSize: 45,
    fontWeight: FontWeight.w400,
    height: 1.16,
    letterSpacing: 0,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle displaySmall = TextStyle(
    fontSize: 36,
    fontWeight: FontWeight.w400,
    height: 1.22,
    letterSpacing: 0,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  // Heading Styles
  static const TextStyle heading1 = TextStyle(
    fontSize: 32,
    fontWeight: FontWeight.w600,
    height: 1.25,
    letterSpacing: 0,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle heading2 = TextStyle(
    fontSize: 28,
    fontWeight: FontWeight.w600,
    height: 1.29,
    letterSpacing: 0,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle heading3 = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.w600,
    height: 1.33,
    letterSpacing: 0,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  // Subtitle Styles
  static const TextStyle subtitle1 = TextStyle(
    fontSize: 20,
    fontWeight: FontWeight.w500,
    height: 1.4,
    letterSpacing: 0.15,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle subtitle2 = TextStyle(
    fontSize: 18,
    fontWeight: FontWeight.w500,
    height: 1.44,
    letterSpacing: 0.1,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  // Body Styles
  static const TextStyle bodyLarge = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.w400,
    height: 1.5,
    letterSpacing: 0.5,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle bodyMedium = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w400,
    height: 1.43,
    letterSpacing: 0.25,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle bodySmall = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.w400,
    height: 1.33,
    letterSpacing: 0.4,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  // Button Style
  static const TextStyle button = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w600,
    height: 1.43,
    letterSpacing: 1.25,
    color: AppColors.textPrimary,
    fontFamily: 'arabic_font',
  );
  
  // Caption Style
  static const TextStyle caption = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.w400,
    height: 1.33,
    letterSpacing: 0.4,
    color: AppColors.textSecondary,
    fontFamily: 'arabic_font',
  );
  
  // Overline Style
  static const TextStyle overline = TextStyle(
    fontSize: 10,
    fontWeight: FontWeight.w500,
    height: 1.6,
    letterSpacing: 1.5,
    color: AppColors.textSecondary,
    fontFamily: 'arabic_font',
  );
  
  // Special Styles
  static const TextStyle price = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.w700,
    height: 1.33,
    letterSpacing: 0,
    color: AppColors.primary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle priceSmall = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.w600,
    height: 1.5,
    letterSpacing: 0,
    color: AppColors.primary,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle rating = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w600,
    height: 1.43,
    letterSpacing: 0,
    color: AppColors.ratingStar,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle error = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.w400,
    height: 1.33,
    letterSpacing: 0.4,
    color: AppColors.error,
    fontFamily: 'arabic_font',
  );
  
  static const TextStyle link = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w500,
    height: 1.43,
    letterSpacing: 0.25,
    color: AppColors.primary,
    decoration: TextDecoration.underline,
    fontFamily: 'arabic_font',
  );
  
  // Price text style alias
  static const TextStyle priceTextStyle = price;
}