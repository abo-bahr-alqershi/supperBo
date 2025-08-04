import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/home_section.dart';
import '../repositories/home_repository.dart';

class GetHomeSectionsUseCase implements UseCase<List<HomeSection>, GetHomeSectionsParams> {
  final HomeRepository repository;

  GetHomeSectionsUseCase(this.repository);

  @override
  Future<Either<Failure, List<HomeSection>>> call(GetHomeSectionsParams params) async {
    return await repository.getHomeSections(userId: params.userId);
  }
}

class GetHomeSectionsParams extends Equatable {
  final String? userId;

  const GetHomeSectionsParams({this.userId});

  @override
  List<Object?> get props => [userId];
}
