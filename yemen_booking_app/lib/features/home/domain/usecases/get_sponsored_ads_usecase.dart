import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/sponsored_ad.dart';
import '../repositories/home_repository.dart';

class GetSponsoredAdsUseCase implements UseCase<List<SponsoredAd>, NoParams> {
  final HomeRepository repository;

  GetSponsoredAdsUseCase(this.repository);

  @override
  Future<Either<Failure, List<SponsoredAd>>> call(NoParams params) async {
    return await repository.getSponsoredAds();
  }
}
