import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/city_destination.dart';
import '../repositories/home_repository.dart';

class GetCityDestinationsUseCase implements UseCase<List<CityDestination>, NoParams> {
  final HomeRepository repository;

  GetCityDestinationsUseCase(this.repository);

  @override
  Future<Either<Failure, List<CityDestination>>> call(NoParams params) async {
    return await repository.getCityDestinations();
  }
}
