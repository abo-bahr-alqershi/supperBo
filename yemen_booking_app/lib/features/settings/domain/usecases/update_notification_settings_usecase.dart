import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/app_settings.dart';
import '../repositories/settings_repository.dart';

class UpdateNotificationSettingsParams extends Equatable {
  final NotificationSettings notificationSettings;

  const UpdateNotificationSettingsParams({required this.notificationSettings});

  @override
  List<Object> get props => [notificationSettings];
}

class UpdateNotificationSettingsUseCase implements UseCase<AppSettings, UpdateNotificationSettingsParams> {
  final SettingsRepository repository;

  UpdateNotificationSettingsUseCase(this.repository);

  @override
  Future<Either<Failure, AppSettings>> call(UpdateNotificationSettingsParams params) async {
    return await repository.updateNotificationSettings(params.notificationSettings);
  }
}