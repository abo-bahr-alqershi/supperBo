import 'package:flutter/material.dart';
import 'package:equatable/equatable.dart';

enum AppLanguage {
  arabic,
  english,
}

class NotificationSettings extends Equatable {
  final bool pushNotifications;
  final bool emailNotifications;
  final bool smsNotifications;
  final bool bookingReminders;
  final bool promotionalOffers;
  final bool chatMessages;

  const NotificationSettings({
    this.pushNotifications = true,
    this.emailNotifications = true,
    this.smsNotifications = false,
    this.bookingReminders = true,
    this.promotionalOffers = false,
    this.chatMessages = true,
  });

  NotificationSettings copyWith({
    bool? pushNotifications,
    bool? emailNotifications,
    bool? smsNotifications,
    bool? bookingReminders,
    bool? promotionalOffers,
    bool? chatMessages,
  }) {
    return NotificationSettings(
      pushNotifications: pushNotifications ?? this.pushNotifications,
      emailNotifications: emailNotifications ?? this.emailNotifications,
      smsNotifications: smsNotifications ?? this.smsNotifications,
      bookingReminders: bookingReminders ?? this.bookingReminders,
      promotionalOffers: promotionalOffers ?? this.promotionalOffers,
      chatMessages: chatMessages ?? this.chatMessages,
    );
  }

  @override
  List<Object> get props => [
        pushNotifications,
        emailNotifications,
        smsNotifications,
        bookingReminders,
        promotionalOffers,
        chatMessages,
      ];
}

class AppSettings extends Equatable {
  final ThemeMode themeMode;
  final AppLanguage language;
  final Locale locale;
  final NotificationSettings notificationSettings;
  final bool biometricAuth;
  final bool autoLogin;
  final String currency;
  final bool showOnboarding;
  final DateTime lastUpdated;

  const AppSettings({
    this.themeMode = ThemeMode.system,
    this.language = AppLanguage.arabic,
    this.locale = const Locale('ar', 'YE'),
    this.notificationSettings = const NotificationSettings(),
    this.biometricAuth = false,
    this.autoLogin = false,
    this.currency = 'YER',
    this.showOnboarding = true,
    required this.lastUpdated,
  });

  AppSettings copyWith({
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
    return AppSettings(
      themeMode: themeMode ?? this.themeMode,
      language: language ?? this.language,
      locale: locale ?? this.locale,
      notificationSettings: notificationSettings ?? this.notificationSettings,
      biometricAuth: biometricAuth ?? this.biometricAuth,
      autoLogin: autoLogin ?? this.autoLogin,
      currency: currency ?? this.currency,
      showOnboarding: showOnboarding ?? this.showOnboarding,
      lastUpdated: lastUpdated ?? this.lastUpdated,
    );
  }

  @override
  List<Object> get props => [
        themeMode,
        language,
        locale,
        notificationSettings,
        biometricAuth,
        autoLogin,
        currency,
        showOnboarding,
        lastUpdated,
      ];
}