import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'app_colors.dart';
import 'app_text_styles.dart';
import 'app_dimensions.dart';

class AppTheme {
  AppTheme._();

  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.light,
      scaffoldBackgroundColor: AppColors.scaffoldBackgroundLight,
      primaryColor: AppColors.primary,
      hintColor: AppColors.accent,
      fontFamily: 'arabic_font',
      
      // Define text theme based on AppTextStyles
      textTheme: const TextTheme(
        displayLarge: AppTextStyles.displayLarge,
        displayMedium: AppTextStyles.displayMedium,
        displaySmall: AppTextStyles.displaySmall,
        headlineLarge: AppTextStyles.heading1,
        headlineMedium: AppTextStyles.heading2,
        headlineSmall: AppTextStyles.heading3,
        titleLarge: AppTextStyles.subtitle1,
        titleMedium: AppTextStyles.subtitle2,
        bodyLarge: AppTextStyles.bodyLarge,
        bodyMedium: AppTextStyles.bodyMedium,
        bodySmall: AppTextStyles.bodySmall,
        labelLarge: AppTextStyles.button,
        labelMedium: AppTextStyles.caption,
        labelSmall: AppTextStyles.overline,
      ),
      
      // App Bar Theme
      appBarTheme: AppBarTheme(
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.textPrimary,
        elevation: 0,
        titleTextStyle: AppTextStyles.heading3.copyWith(color: AppColors.textPrimary),
        systemOverlayStyle: const SystemUiOverlayStyle(
          statusBarColor: Colors.transparent,
          statusBarIconBrightness: Brightness.dark,
          statusBarBrightness: Brightness.light,
        ),
      ),
      
      // Button Themes
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          foregroundColor: Colors.white,
          backgroundColor: AppColors.primary,
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 24),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.button.copyWith(color: Colors.white),
          minimumSize: const Size(double.infinity, 48),
        ),
      ),
      
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: AppColors.primary,
          side: const BorderSide(color: AppColors.primary),
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 24),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.button.copyWith(color: AppColors.primary),
          minimumSize: const Size(double.infinity, 48),
        ),
      ),
      
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.primary,
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
          ),
          textStyle: AppTextStyles.button.copyWith(color: AppColors.primary),
        ),
      ),

      // Input Field Theme
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppColors.surface,
        contentPadding: const EdgeInsets.symmetric(
          vertical: 12,
          horizontal: 16,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.outline),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.primary, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.error, width: 2),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.error, width: 2),
        ),
        hintStyle: AppTextStyles.bodyMedium.copyWith(color: AppColors.textSecondary),
        labelStyle: AppTextStyles.bodyMedium.copyWith(color: AppColors.textSecondary),
      ),

      // Card Theme
      cardTheme: const CardTheme(
        color: AppColors.surface,
        elevation: 2,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppDimensions.borderRadiusLg)),
        ),
      ),

      // Dialog Theme
      dialogTheme: const DialogTheme(
        backgroundColor: AppColors.surface,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppDimensions.borderRadiusLg)),
        ),
      ),

      // BottomNavigationBar Theme
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        backgroundColor: AppColors.surface,
        selectedItemColor: AppColors.primary,
        unselectedItemColor: AppColors.textSecondary,
        selectedIconTheme: const IconThemeData(size: 24),
        unselectedIconTheme: const IconThemeData(size: 24),
        selectedLabelStyle: AppTextStyles.caption.copyWith(color: AppColors.primary),
        unselectedLabelStyle: AppTextStyles.caption.copyWith(color: AppColors.textSecondary),
        type: BottomNavigationBarType.fixed,
      ),

      // FloatingActionButton Theme
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
      ),

      // Icon Theme
      iconTheme: const IconThemeData(
        color: AppColors.primary,
        size: 24,
      ),
    );
  }

  static ThemeData get darkTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.dark,
      scaffoldBackgroundColor: AppColors.backgroundDark,
      primaryColor: AppColors.primary,
      hintColor: AppColors.accent,
      fontFamily: 'arabic_font',
      
      // Define text theme for dark mode
      textTheme: TextTheme(
        displayLarge: AppTextStyles.displayLarge.copyWith(color: AppColors.textPrimaryDark),
        displayMedium: AppTextStyles.displayMedium.copyWith(color: AppColors.textPrimaryDark),
        displaySmall: AppTextStyles.displaySmall.copyWith(color: AppColors.textPrimaryDark),
        headlineLarge: AppTextStyles.heading1.copyWith(color: AppColors.textPrimaryDark),
        headlineMedium: AppTextStyles.heading2.copyWith(color: AppColors.textPrimaryDark),
        headlineSmall: AppTextStyles.heading3.copyWith(color: AppColors.textPrimaryDark),
        titleLarge: AppTextStyles.subtitle1.copyWith(color: AppColors.textPrimaryDark),
        titleMedium: AppTextStyles.subtitle2.copyWith(color: AppColors.textPrimaryDark),
        bodyLarge: AppTextStyles.bodyLarge.copyWith(color: AppColors.textPrimaryDark),
        bodyMedium: AppTextStyles.bodyMedium.copyWith(color: AppColors.textPrimaryDark),
        bodySmall: AppTextStyles.bodySmall.copyWith(color: AppColors.textSecondaryDark),
        labelLarge: AppTextStyles.button.copyWith(color: AppColors.textPrimaryDark),
        labelMedium: AppTextStyles.caption.copyWith(color: AppColors.textSecondaryDark),
        labelSmall: AppTextStyles.overline.copyWith(color: AppColors.textSecondaryDark),
      ),

      // App Bar Theme
      appBarTheme: AppBarTheme(
        backgroundColor: AppColors.surfaceDark,
        foregroundColor: AppColors.textPrimaryDark,
        elevation: 0,
        titleTextStyle: AppTextStyles.heading3.copyWith(color: AppColors.textPrimaryDark),
        systemOverlayStyle: const SystemUiOverlayStyle(
          statusBarColor: Colors.transparent,
          statusBarIconBrightness: Brightness.light,
          statusBarBrightness: Brightness.dark,
        ),
      ),

      // Button Themes
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          foregroundColor: Colors.white,
          backgroundColor: AppColors.primary,
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 24),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.button.copyWith(color: Colors.white),
          minimumSize: const Size(double.infinity, 48),
        ),
      ),

      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: AppColors.primary,
          side: const BorderSide(color: AppColors.primary),
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 24),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.button.copyWith(color: AppColors.primary),
          minimumSize: const Size(double.infinity, 48),
        ),
      ),

      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.primary,
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
          ),
          textStyle: AppTextStyles.button.copyWith(color: AppColors.primary),
        ),
      ),

      // Input Field Theme
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppColors.surfaceDark,
        contentPadding: const EdgeInsets.symmetric(
          vertical: 12,
          horizontal: 16,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.outlineDark),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.primary, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.error, width: 2),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.error, width: 2),
        ),
        hintStyle: AppTextStyles.bodyMedium.copyWith(color: AppColors.textSecondaryDark),
        labelStyle: AppTextStyles.bodyMedium.copyWith(color: AppColors.textSecondaryDark),
      ),

      // Card Theme
      cardTheme: const CardTheme(
        color: AppColors.surfaceDark,
        elevation: 2,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppDimensions.borderRadiusLg)),
        ),
      ),

      // Dialog Theme
      dialogTheme: const DialogTheme(
        backgroundColor: AppColors.surfaceDark,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.all(Radius.circular(AppDimensions.borderRadiusLg)),
        ),
      ),

      // BottomNavigationBar Theme
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        backgroundColor: AppColors.surfaceDark,
        selectedItemColor: AppColors.primary,
        unselectedItemColor: AppColors.textSecondaryDark,
        selectedIconTheme: const IconThemeData(size: 24),
        unselectedIconTheme: const IconThemeData(size: 24),
        selectedLabelStyle: AppTextStyles.caption.copyWith(color: AppColors.primary),
        unselectedLabelStyle: AppTextStyles.caption.copyWith(color: AppColors.textSecondaryDark),
        type: BottomNavigationBarType.fixed,
      ),

      // FloatingActionButton Theme
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
      ),

      // Icon Theme
      iconTheme: const IconThemeData(
        color: AppColors.primary,
        size: 24,
      ),
    );
  }
}