import 'package:dartz/dartz.dart';
import 'package:flutter/material.dart';
import '../../../../core/error/failures.dart';
import '../entities/app_settings.dart';

abstract class SettingsRepository {
  Future<Either<Failure, AppSettings>> getSettings();
  Future<Either<Failure, AppSettings>> updateLanguage(AppLanguage language);
  Future<Either<Failure, AppSettings>> updateTheme(ThemeMode themeMode);
  Future<Either<Failure, AppSettings>> updateNotificationSettings(NotificationSettings notificationSettings);
  Future<Either<Failure, AppSettings>> updateBiometricAuth(bool enabled);
  Future<Either<Failure, AppSettings>> updateAutoLogin(bool enabled);
  Future<Either<Failure, AppSettings>> updateCurrency(String currency);
  Future<Either<Failure, AppSettings>> updateOnboardingVisibility(bool showOnboarding);
  Future<Either<Failure, void>> resetSettings();
}