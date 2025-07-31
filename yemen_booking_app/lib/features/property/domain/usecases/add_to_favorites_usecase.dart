import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/property_repository.dart';

class AddToFavoritesUseCase implements UseCase<bool, AddToFavoritesParams> {
  final PropertyRepository repository;

  AddToFavoritesUseCase({required this.repository});

  @override
  Future<Either<Failure, bool>> call(AddToFavoritesParams params) async {
    return await repository.addToFavorites(
      propertyId: params.propertyId,
      userId: params.userId,
    );
  }
}

class AddToFavoritesParams extends Equatable {
  final String propertyId;
  final String userId;

  const AddToFavoritesParams({
    required this.propertyId,
    required this.userId,
  });

  @override
  List<Object?> get props => [propertyId, userId];
}