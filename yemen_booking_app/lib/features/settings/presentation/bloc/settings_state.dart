import 'package:flutter/material.dart';
import 'package:equatable/equatable.dart';
import '../../domain/entities/app_settings.dart';

abstract class SettingsState extends Equatable {
  const SettingsState();

  @override
  List<Object> get props => [];
}

/// Initial state when the settings bloc is first created
class SettingsInitial extends SettingsState {
  const SettingsInitial();
}

/// State when any settings operation is in progress
class SettingsLoading extends SettingsState {
  const SettingsLoading();
}

/// State when settings are successfully loaded
class SettingsLoaded extends SettingsState {
  final AppSettings settings;

  const SettingsLoaded({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when a settings error occurs
class SettingsError extends SettingsState {
  final String message;

  const SettingsError({required this.message});

  @override
  List<Object> get props => [message];
}

/// State when language is successfully updated
class LanguageUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const LanguageUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when theme is successfully updated
class ThemeUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const ThemeUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when notification settings are successfully updated
class NotificationSettingsUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const NotificationSettingsUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when biometric auth setting is successfully updated
class BiometricAuthUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const BiometricAuthUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when auto login setting is successfully updated
class AutoLoginUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const AutoLoginUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when currency is successfully updated
class CurrencyUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const CurrencyUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when onboarding visibility is successfully updated
class OnboardingVisibilityUpdateSuccess extends SettingsState {
  final AppSettings settings;

  const OnboardingVisibilityUpdateSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when settings are successfully reset
class SettingsResetSuccess extends SettingsState {
  final AppSettings settings;

  const SettingsResetSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}

/// State when settings are successfully refreshed
class SettingsRefreshSuccess extends SettingsState {
  final AppSettings settings;

  const SettingsRefreshSuccess({required this.settings});

  @override
  List<Object> get props => [settings];
}