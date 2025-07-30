import 'package:flutter/material.dart';
import 'package:equatable/equatable.dart';
import '../../domain/entities/app_settings.dart';

abstract class SettingsEvent extends Equatable {
  const SettingsEvent();

  @override
  List<Object> get props => [];
}

/// Event to load the current settings
class LoadSettingsEvent extends SettingsEvent {
  const LoadSettingsEvent();
}

/// Event to update app language
class UpdateLanguageEvent extends SettingsEvent {
  final AppLanguage language;

  const UpdateLanguageEvent({required this.language});

  @override
  List<Object> get props => [language];
}

/// Event to update app theme
class UpdateThemeEvent extends SettingsEvent {
  final ThemeMode themeMode;

  const UpdateThemeEvent({required this.themeMode});

  @override
  List<Object> get props => [themeMode];
}

/// Event to update notification settings
class UpdateNotificationSettingsEvent extends SettingsEvent {
  final NotificationSettings notificationSettings;

  const UpdateNotificationSettingsEvent({required this.notificationSettings});

  @override
  List<Object> get props => [notificationSettings];
}

/// Event to toggle biometric authentication
class UpdateBiometricAuthEvent extends SettingsEvent {
  final bool enabled;

  const UpdateBiometricAuthEvent({required this.enabled});

  @override
  List<Object> get props => [enabled];
}

/// Event to toggle auto login
class UpdateAutoLoginEvent extends SettingsEvent {
  final bool enabled;

  const UpdateAutoLoginEvent({required this.enabled});

  @override
  List<Object> get props => [enabled];
}

/// Event to update currency
class UpdateCurrencyEvent extends SettingsEvent {
  final String currency;

  const UpdateCurrencyEvent({required this.currency});

  @override
  List<Object> get props => [currency];
}

/// Event to update onboarding visibility
class UpdateOnboardingVisibilityEvent extends SettingsEvent {
  final bool showOnboarding;

  const UpdateOnboardingVisibilityEvent({required this.showOnboarding});

  @override
  List<Object> get props => [showOnboarding];
}

/// Event to reset all settings to default
class ResetSettingsEvent extends SettingsEvent {
  const ResetSettingsEvent();
}

/// Event to refresh settings from storage
class RefreshSettingsEvent extends SettingsEvent {
  const RefreshSettingsEvent();
}