import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/home_repository.dart';

class RecordAdImpressionUseCase implements UseCase<void, RecordAdImpressionParams> {
  final HomeRepository repository;

  RecordAdImpressionUseCase(this.repository);

  @override
  Future<Either<Failure, void>> call(RecordAdImpressionParams params) async {
    return await repository.recordAdImpression(adId: params.adId, additionalData: params.additionalData);
  }
}

class RecordAdImpressionParams extends Equatable {
  final String adId;
  final String? additionalData;

  const RecordAdImpressionParams({required this.adId, this.additionalData});

  @override
  List<Object?> get props => [adId, additionalData];
}
