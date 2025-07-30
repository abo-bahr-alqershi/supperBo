import 'package:flutter/material.dart';
import '../../../../services/local_storage_service.dart';
import '../../domain/entities/app_settings.dart';
import '../models/app_settings_model.dart';

abstract class SettingsLocalDataSource {
  Future<AppSettings> getSettings();
  Future<AppSettings> updateLanguage(AppLanguage language);
  Future<AppSettings> updateTheme(ThemeMode themeMode);
  Future<AppSettings> updateNotificationSettings(NotificationSettings notificationSettings);
  Future<AppSettings> updateBiometricAuth(bool enabled);
  Future<AppSettings> updateAutoLogin(bool enabled);
  Future<AppSettings> updateCurrency(String currency);
  Future<AppSettings> updateOnboardingVisibility(bool showOnboarding);
  Future<void> resetSettings();
}

class SettingsLocalDataSourceImpl implements SettingsLocalDataSource {
  final LocalStorageService localStorage;

  SettingsLocalDataSourceImpl({required this.localStorage});

  @override
  Future<AppSettings> getSettings() async {
    // Get default settings or cached settings
    return AppSettingsModel(
      language: AppLanguage.arabic,
      themeMode: ThemeMode.system,
      notificationSettings: const NotificationSettings(),
      biometricAuth: false,
      autoLogin: false,
      currency: 'YER',
      showOnboarding: true,
      lastUpdated: DateTime.now(),
    );
  }

  @override
  Future<AppSettings> updateLanguage(AppLanguage language) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      language: language,
      lastUpdated: DateTime.now(),
    );
    
    // Save to localStorage
    await localStorage.saveLanguage(language == AppLanguage.arabic ? 'ar' : 'en');
    
    return updatedSettings;
  }

  @override
  Future<AppSettings> updateTheme(ThemeMode themeMode) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      themeMode: themeMode,
      lastUpdated: DateTime.now(),
    );
    
    // Save to localStorage
    String themeString = '';
    switch (themeMode) {
      case ThemeMode.light:
        themeString = 'light';
        break;
      case ThemeMode.dark:
        themeString = 'dark';
        break;
      case ThemeMode.system:
        themeString = 'system';
        break;
    }
    await localStorage.saveTheme(themeString);
    
    return updatedSettings;
  }

  @override
  Future<AppSettings> updateNotificationSettings(NotificationSettings notificationSettings) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      notificationSettings: notificationSettings,
      lastUpdated: DateTime.now(),
    );
    
    return updatedSettings;
  }

  @override
  Future<AppSettings> updateBiometricAuth(bool enabled) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      biometricAuth: enabled,
      lastUpdated: DateTime.now(),
    );
    
    return updatedSettings;
  }

  @override
  Future<AppSettings> updateAutoLogin(bool enabled) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      autoLogin: enabled,
      lastUpdated: DateTime.now(),
    );
    
    return updatedSettings;
  }

  @override
  Future<AppSettings> updateCurrency(String currency) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      currency: currency,
      lastUpdated: DateTime.now(),
    );
    
    return updatedSettings;
  }

  @override
  Future<AppSettings> updateOnboardingVisibility(bool showOnboarding) async {
    final currentSettings = await getSettings();
    final updatedSettings = AppSettingsModel.fromEntity(currentSettings).copyWith(
      showOnboarding: showOnboarding,
      lastUpdated: DateTime.now(),
    );
    
    await localStorage.setOnboardingCompleted(!showOnboarding);
    
    return updatedSettings;
  }

  @override
  Future<void> resetSettings() async {
    await localStorage.clearAll();
  }
}