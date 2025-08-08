import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/home_repository.dart';

class TrackSectionImpressionUseCase implements UseCase<void, TrackSectionImpressionParams> {
  final HomeRepository repository;

  TrackSectionImpressionUseCase(this.repository);

  @override
  Future<Either<Failure, void>> call(TrackSectionImpressionParams params) async {
    return await repository.recordSectionImpression(sectionId: params.sectionId);
  }
}

class TrackSectionImpressionParams {
  final String sectionId;
  const TrackSectionImpressionParams({required this.sectionId});
}