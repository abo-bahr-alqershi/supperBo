import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/app_settings.dart';
import '../repositories/settings_repository.dart';

class UpdateThemeParams extends Equatable {
  final ThemeMode themeMode;

  const UpdateThemeParams({required this.themeMode});

  @override
  List<Object> get props => [themeMode];
}

class UpdateThemeUseCase implements UseCase<AppSettings, UpdateThemeParams> {
  final SettingsRepository repository;

  UpdateThemeUseCase(this.repository);

  @override
  Future<Either<Failure, AppSettings>> call(UpdateThemeParams params) async {
    return await repository.updateTheme(params.themeMode);
  }
}