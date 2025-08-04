import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/home_config.dart';
import '../repositories/home_repository.dart';

class GetHomeConfigUseCase implements UseCase<HomeConfig, GetHomeConfigParams> {
  final HomeRepository repository;

  GetHomeConfigUseCase(this.repository);

  @override
  Future<Either<Failure, HomeConfig>> call(GetHomeConfigParams params) async {
    return await repository.getHomeConfig(version: params.version);
  }
}

class GetHomeConfigParams extends Equatable {
  final String? version;

  const GetHomeConfigParams({this.version});

  @override
  List<Object?> get props => [version];
}
