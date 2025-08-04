import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/home_config.dart';
import '../entities/home_section.dart';
import '../entities/sponsored_ad.dart';
import '../entities/city_destination.dart';

abstract class HomeRepository {
  Future<Either<Failure, HomeConfig>> getHomeConfig({String? version});
  Future<Either<Failure, List<HomeSection>>> getHomeSections({String? userId});
  Future<Either<Failure, List<SponsoredAd>>> getSponsoredAds();
  Future<Either<Failure, List<CityDestination>>> getCityDestinations();
  Future<Either<Failure, void>> recordAdImpression({required String adId, String? additionalData});
  Future<Either<Failure, void>> recordAdClick({required String adId, String? additionalData});
}
