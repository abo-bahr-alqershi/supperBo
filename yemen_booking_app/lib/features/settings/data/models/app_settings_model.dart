import 'package:flutter/material.dart';
import '../../domain/entities/app_settings.dart';

class NotificationSettingsModel extends NotificationSettings {
  const NotificationSettingsModel({
    super.pushNotifications,
    super.emailNotifications,
    super.smsNotifications,
    super.bookingReminders,
    super.promotionalOffers,
    super.chatMessages,
  });

  factory NotificationSettingsModel.fromJson(Map<String, dynamic> json) {
    return NotificationSettingsModel(
      pushNotifications: json['push_notifications'] ?? true,
      emailNotifications: json['email_notifications'] ?? true,
      smsNotifications: json['sms_notifications'] ?? false,
      bookingReminders: json['booking_reminders'] ?? true,
      promotionalOffers: json['promotional_offers'] ?? false,
      chatMessages: json['chat_messages'] ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'push_notifications': pushNotifications,
      'email_notifications': emailNotifications,
      'sms_notifications': smsNotifications,
      'booking_reminders': bookingReminders,
      'promotional_offers': promotionalOffers,
      'chat_messages': chatMessages,
    };
  }

  factory NotificationSettingsModel.fromEntity(NotificationSettings entity) {
    return NotificationSettingsModel(
      pushNotifications: entity.pushNotifications,
      emailNotifications: entity.emailNotifications,
      smsNotifications: entity.smsNotifications,
      bookingReminders: entity.bookingReminders,
      promotionalOffers: entity.promotionalOffers,
      chatMessages: entity.chatMessages,
    );
  }
}

class AppSettingsModel extends AppSettings {
  const AppSettingsModel({
    super.themeMode,
    super.language,
    super.notificationSettings,
    super.biometricAuth,
    super.autoLogin,
    super.currency,
    super.showOnboarding,
    required super.lastUpdated,
  });

  factory AppSettingsModel.fromJson(Map<String, dynamic> json) {
    return AppSettingsModel(
      themeMode: _parseThemeMode(json['theme_mode']),
      language: _parseLanguage(json['language']),
      notificationSettings: json['notification_settings'] != null
          ? NotificationSettingsModel.fromJson(json['notification_settings'])
          : const NotificationSettingsModel(),
      biometricAuth: json['biometric_auth'] ?? false,
      autoLogin: json['auto_login'] ?? false,
      currency: json['currency'] ?? 'YER',
      showOnboarding: json['show_onboarding'] ?? true,
      lastUpdated: json['last_updated'] != null
          ? DateTime.parse(json['last_updated'])
          : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'theme_mode': _themeModelToString(themeMode),
      'language': _languageToString(language),
      'notification_settings': notificationSettings is NotificationSettingsModel
          ? (notificationSettings as NotificationSettingsModel).toJson()
          : NotificationSettingsModel.fromEntity(notificationSettings).toJson(),
      'biometric_auth': biometricAuth,
      'auto_login': autoLogin,
      'currency': currency,
      'show_onboarding': showOnboarding,
      'last_updated': lastUpdated.toIso8601String(),
    };
  }

  factory AppSettingsModel.fromEntity(AppSettings entity) {
    return AppSettingsModel(
      themeMode: entity.themeMode,
      language: entity.language,
      notificationSettings: entity.notificationSettings is NotificationSettingsModel
          ? entity.notificationSettings as NotificationSettingsModel
          : NotificationSettingsModel.fromEntity(entity.notificationSettings),
      biometricAuth: entity.biometricAuth,
      autoLogin: entity.autoLogin,
      currency: entity.currency,
      showOnboarding: entity.showOnboarding,
      lastUpdated: entity.lastUpdated,
    );
  }

  static ThemeMode _parseThemeMode(String? value) {
    switch (value) {
      case 'light':
        return ThemeMode.light;
      case 'dark':
        return ThemeMode.dark;
      case 'system':
      default:
        return ThemeMode.system;
    }
  }

  static String _themeModelToString(ThemeMode themeMode) {
    switch (themeMode) {
      case ThemeMode.light:
        return 'light';
      case ThemeMode.dark:
        return 'dark';
      case ThemeMode.system:
        return 'system';
    }
  }

  static AppLanguage _parseLanguage(String? value) {
    switch (value) {
      case 'english':
        return AppLanguage.english;
      case 'arabic':
      default:
        return AppLanguage.arabic;
    }
  }

  static String _languageToString(AppLanguage language) {
    switch (language) {
      case AppLanguage.arabic:
        return 'arabic';
      case AppLanguage.english:
        return 'english';
    }
  }

  @override
  AppSettingsModel copyWith({
    ThemeMode? themeMode,
    AppLanguage? language,
    Locale? locale,
    NotificationSettings? notificationSettings,
    bool? biometricAuth,
    bool? autoLogin,
    String? currency,
    bool? showOnboarding,
    DateTime? lastUpdated,
  }) {
    return AppSettingsModel(
      themeMode: themeMode ?? this.themeMode,
      language: language ?? this.language,
      notificationSettings: notificationSettings ?? this.notificationSettings,
      biometricAuth: biometricAuth ?? this.biometricAuth,
      autoLogin: autoLogin ?? this.autoLogin,
      currency: currency ?? this.currency,
      showOnboarding: showOnboarding ?? this.showOnboarding,
      lastUpdated: lastUpdated ?? this.lastUpdated,
    );
  }
}