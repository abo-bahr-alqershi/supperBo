import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/models/paginated_result.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/property_repository.dart';

class GetPropertyReviewsUseCase implements UseCase<List<PropertyReview>, GetPropertyReviewsParams> {
  final PropertyRepository repository;

  GetPropertyReviewsUseCase({required this.repository});

  @override
  Future<Either<Failure, List<PropertyReview>>> call(GetPropertyReviewsParams params) async {
    return await repository.getPropertyReviews(
      propertyId: params.propertyId,
      pageNumber: params.pageNumber,
      pageSize: params.pageSize,
    );
  }
}

class GetPropertyReviewsParams extends Equatable {
  final String propertyId;
  final int pageNumber;
  final int pageSize;

  const GetPropertyReviewsParams({
    required this.propertyId,
    this.pageNumber = 1,
    this.pageSize = 20,
  });

  @override
  List<Object?> get props => [propertyId, pageNumber, pageSize];
}