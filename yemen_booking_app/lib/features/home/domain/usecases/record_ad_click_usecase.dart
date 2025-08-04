import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/home_repository.dart';

class RecordAdClickUseCase implements UseCase<void, RecordAdClickParams> {
  final HomeRepository repository;

  RecordAdClickUseCase(this.repository);

  @override
  Future<Either<Failure, void>> call(RecordAdClickParams params) async {
    return await repository.recordAdClick(adId: params.adId, additionalData: params.additionalData);
  }
}

class RecordAdClickParams extends Equatable {
  final String adId;
  final String? additionalData;

  const RecordAdClickParams({required this.adId, this.additionalData});

  @override
  List<Object?> get props => [adId, additionalData];
}
