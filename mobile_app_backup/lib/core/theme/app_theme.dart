import 'package:flutter/material.dart';
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
      hintColor: AppColors.accent, // Or another accent color
      
      // Define text theme based on light mode
      textTheme: AppTextStyles.getTextTheme(false),
      
      // App Bar Theme
      appBarTheme: AppBarTheme(
        backgroundColor: AppColors.cardBackgroundLight,
        foregroundColor: AppColors.textPrimaryLight,
        elevation: AppDimensions.elevationSm,
        titleTextStyle: AppTextStyles.appBarTitleTextStyle.copyWith(color: AppColors.textPrimaryLight),
        systemOverlayStyle: const SystemUiOverlayStyle(
          statusBarColor: Colors.transparent,
          statusBarIconBrightness: Brightness.dark,
          statusBarBrightness: Brightness.light, // For iOS status bar
        ),
      ),
      
      // Button Themes
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          foregroundColor: Colors.white,
          backgroundColor: AppColors.primary,
          padding: EdgeInsets.symmetric(vertical: AppDimensions.spacingMd),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.buttonTextStyle.copyWith(color: Colors.white),
          minimumSize: const Size(double.infinity, 48), // Example default size
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: AppColors.primary,
          side: BorderSide(color: AppColors.primary),
          padding: EdgeInsets.symmetric(vertical: AppDimensions.spacingMd),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.buttonTextStyle.copyWith(color: AppColors.primary),
          minimumSize: const Size(double.infinity, 48), // Example default size
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.primary,
          padding: EdgeInsets.symmetric(vertical: AppDimensions.spacingSm, horizontal: AppDimensions.spacingMd),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
          ),
          textStyle: AppTextStyles.labelLarge.copyWith(color: AppColors.primary),
        ),
      ),

      // Input Field Theme
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppColors.gray200, // Light gray background for inputs
        contentPadding: EdgeInsets.symmetric(
          vertical: AppDimensions.spacingSm,
          horizontal: AppDimensions.spacingMd,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide.none, // No border line initially
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide(color: AppColors.primary, width: 1.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide(color: AppColors.error, width: 1.5),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide(color: AppColors.error, width: 1.5),
        ),
        hintStyle: AppTextStyles.bodyMedium.copyWith(color: AppColors.gray500),
        labelStyle: AppTextStyles.labelLarge.copyWith(color: AppColors.gray700),
      ),

      // Card Theme
      cardTheme: CardTheme(
        color: AppColors.cardBackgroundLight,
        elevation: AppDimensions.elevationXs,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        ),
        margin: EdgeInsets.zero, // Default margin to zero, handle in UI
      ),

      // Dialog Theme
      dialogTheme: DialogTheme(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        ),
        backgroundColor: AppColors.cardBackgroundLight,
      ),

      // BottomNavigationBar Theme
      bottomNavigationBarTheme: const BottomNavigationBarThemeData(
        backgroundColor: AppColors.cardBackgroundLight,
        selectedItemColor: AppColors.primary,
        unselectedItemColor: AppColors.gray500,
        selectedIconTheme: IconThemeData(size: AppDimensions.iconSizeMd),
        unselectedIconTheme: IconThemeData(size: AppDimensions.iconSizeMd),
        selectedLabelStyle: TextStyle(fontSize: 12),
        unselectedLabelStyle: TextStyle(fontSize: 12),
        type: BottomNavigationBarType.fixed,
      ),

      // FloatingActionButton Theme
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
      ),

      // Icon Theme
      iconTheme: const IconThemeData(
        color: AppColors.primary, // Default icon color
        size: AppDimensions.iconSizeMd,
      ),
      
      // Other theme properties as needed
    );
  }

  static ThemeData get darkTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.dark,
      scaffoldBackgroundColor: AppColors.scaffoldBackgroundDark,
      primaryColor: AppColors.primaryDark, // Use a darker primary for dark mode
      hintColor: AppColors.accent, // Example accent color
      
      // Define text theme based on dark mode
      textTheme: AppTextStyles.getTextTheme(true),

      // App Bar Theme
      appBarTheme: AppBarTheme(
        backgroundColor: AppColors.cardBackgroundDark,
        foregroundColor: AppColors.textPrimaryDark,
        elevation: AppDimensions.elevationSm,
        titleTextStyle: AppTextStyles.appBarTitleTextStyle.copyWith(color: AppColors.textPrimaryDark),
        systemOverlayStyle: const SystemUiOverlayStyle(
          statusBarColor: Colors.transparent,
          statusBarIconBrightness: Brightness.light,
          statusBarBrightness: Brightness.dark, // For iOS status bar
        ),
      ),

      // Button Themes
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          foregroundColor: Colors.white,
          backgroundColor: AppColors.primaryDark,
          padding: EdgeInsets.symmetric(vertical: AppDimensions.spacingMd),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.buttonTextStyle.copyWith(color: Colors.white),
          minimumSize: const Size(double.infinity, 48), // Example default size
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: AppColors.primaryDark,
          side: BorderSide(color: AppColors.primaryDark),
          padding: EdgeInsets.symmetric(vertical: AppDimensions.spacingMd),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          textStyle: AppTextStyles.buttonTextStyle.copyWith(color: AppColors.primaryDark),
          minimumSize: const Size(double.infinity, 48), // Example default size
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.primaryDark,
          padding: EdgeInsets.symmetric(vertical: AppDimensions.spacingSm, horizontal: AppDimensions.spacingMd),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
          ),
          textStyle: AppTextStyles.labelLarge.copyWith(color: AppColors.primaryDark),
        ),
      ),

      // Input Field Theme
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppColors.gray900, // Dark gray background for inputs
        contentPadding: EdgeInsets.symmetric(
          vertical: AppDimensions.spacingSm,
          horizontal: AppDimensions.spacingMd,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide.none, // No border line initially
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide(color: AppColors.primaryDark, width: 1.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide(color: AppColors.error, width: 1.5),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: BorderSide(color: AppColors.error, width: 1.5),
        ),
        hintStyle: AppTextStyles.bodyMedium.copyWith(color: AppColors.gray600),
        labelStyle: AppTextStyles.labelLarge.copyWith(color: AppColors.gray300),
      ),

      // Card Theme
      cardTheme: CardTheme(
        color: AppColors.cardBackgroundDark,
        elevation: AppDimensions.elevationXs,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        ),
        margin: EdgeInsets.zero, // Default margin to zero, handle in UI
      ),

      // Dialog Theme
      dialogTheme: DialogTheme(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        ),
        backgroundColor: AppColors.cardBackgroundDark,
      ),

      // BottomNavigationBar Theme
      bottomNavigationBarTheme: const BottomNavigationBarThemeData(
        backgroundColor: AppColors.cardBackgroundDark,
        selectedItemColor: AppColors.primaryDark,
        unselectedItemColor: AppColors.gray600,
        selectedIconTheme: IconThemeData(size: AppDimensions.iconSizeMd),
        unselectedIconTheme: IconThemeData(size: AppDimensions.iconSizeMd),
        selectedLabelStyle: TextStyle(fontSize: 12),
        unselectedLabelStyle: TextStyle(fontSize: 12),
        type: BottomNavigationBarType.fixed,
      ),

      // FloatingActionButton Theme
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: AppColors.primaryDark,
        foregroundColor: Colors.white,
      ),

      // Icon Theme
      iconTheme: const IconThemeData(
        color: AppColors.primaryDark, // Default icon color
        size: AppDimensions.iconSizeMd,
      ),
      
      // Other theme properties as needed
    );
  }
}