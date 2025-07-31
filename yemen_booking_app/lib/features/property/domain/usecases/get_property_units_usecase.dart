import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/unit.dart' hide Unit;
import '../repositories/property_repository.dart';

class GetPropertyUnitsUseCase implements UseCase<List<Unit>, GetPropertyUnitsParams> {
  final PropertyRepository repository;

  GetPropertyUnitsUseCase({required this.repository});

  @override
  Future<Either<Failure, List<Unit>>> call(GetPropertyUnitsParams params) async {
    return await repository.getPropertyUnits(
      propertyId: params.propertyId,
      checkInDate: params.checkInDate,
      checkOutDate: params.checkOutDate,
    );
  }
}

class GetPropertyUnitsParams extends Equatable {
  final String propertyId;
  final DateTime? checkInDate;
  final DateTime? checkOutDate;

  const GetPropertyUnitsParams({
    required this.propertyId,
    this.checkInDate,
    this.checkOutDate,
  });

  @override
  List<Object?> get props => [propertyId, checkInDate, checkOutDate];
}