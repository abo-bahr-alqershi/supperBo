import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/home_repository.dart';

class TrackSectionInteractionUseCase implements UseCase<void, TrackSectionInteractionParams> {
  final HomeRepository repository;

  TrackSectionInteractionUseCase(this.repository);

  @override
  Future<Either<Failure, void>> call(TrackSectionInteractionParams params) async {
    return await repository.recordSectionInteraction(
      sectionId: params.sectionId,
      interactionType: params.interactionType,
      itemId: params.itemId,
      metadata: params.metadata,
    );
  }
}

class TrackSectionInteractionParams {
  final String sectionId;
  final String interactionType;
  final String? itemId;
  final Map<String, dynamic>? metadata;

  const TrackSectionInteractionParams({
    required this.sectionId,
    required this.interactionType,
    this.itemId,
    this.metadata,
  });
}