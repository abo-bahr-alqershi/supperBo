import 'package:dartz/dartz.dart';
import 'package:flutter/material.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/app_settings.dart';
import '../../domain/repositories/settings_repository.dart';
import '../datasources/settings_local_datasource.dart';

class SettingsRepositoryImpl implements SettingsRepository {
  final SettingsLocalDataSource localDataSource;

  SettingsRepositoryImpl({required this.localDataSource});

  @override
  Future<Either<Failure, AppSettings>> getSettings() async {
    try {
      final settings = await localDataSource.getSettings();
      return Right(settings);
    } catch (e) {
      return const Left(CacheFailure('Failed to get settings'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateLanguage(AppLanguage language) async {
    try {
      final updatedSettings = await localDataSource.updateLanguage(language);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update language'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateTheme(ThemeMode themeMode) async {
    try {
      final updatedSettings = await localDataSource.updateTheme(themeMode);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update theme'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateNotificationSettings(NotificationSettings notificationSettings) async {
    try {
      final updatedSettings = await localDataSource.updateNotificationSettings(notificationSettings);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update notification settings'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateBiometricAuth(bool enabled) async {
    try {
      final updatedSettings = await localDataSource.updateBiometricAuth(enabled);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update biometric auth'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateAutoLogin(bool enabled) async {
    try {
      final updatedSettings = await localDataSource.updateAutoLogin(enabled);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update auto login'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateCurrency(String currency) async {
    try {
      final updatedSettings = await localDataSource.updateCurrency(currency);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update currency'));
    }
  }

  @override
  Future<Either<Failure, AppSettings>> updateOnboardingVisibility(bool showOnboarding) async {
    try {
      final updatedSettings = await localDataSource.updateOnboardingVisibility(showOnboarding);
      return Right(updatedSettings);
    } catch (e) {
      return const Left(CacheFailure('Failed to update onboarding visibility'));
    }
  }

  @override
  Future<Either<Failure, void>> resetSettings() async {
    try {
      await localDataSource.resetSettings();
      return const Right(null);
    } catch (e) {
      return const Left(CacheFailure('Failed to reset settings'));
    }
  }
}