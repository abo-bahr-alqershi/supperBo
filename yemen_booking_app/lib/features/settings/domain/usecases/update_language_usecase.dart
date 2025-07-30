import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/usecases/usecase.dart';
import '../../../../core/error/failures.dart';
import '../entities/app_settings.dart';
import '../repositories/settings_repository.dart';

class UpdateLanguageParams extends Equatable {
  final AppLanguage language;

  const UpdateLanguageParams({required this.language});

  @override
  List<Object> get props => [language];
}

class UpdateLanguageUseCase implements UseCase<AppSettings, UpdateLanguageParams> {
  final SettingsRepository repository;

  UpdateLanguageUseCase(this.repository);

  @override
  Future<Either<Failure, AppSettings>> call(UpdateLanguageParams params) async {
    return await repository.updateLanguage(params.language);
  }
}