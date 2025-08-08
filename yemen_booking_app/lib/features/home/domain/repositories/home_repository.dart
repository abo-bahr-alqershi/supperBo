import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/home_config.dart';
import '../entities/home_section.dart';
import '../entities/sponsored_ad.dart';
import '../entities/city_destination.dart';
import '../../data/models/section_data_model.dart';

abstract class HomeRepository {
  Future<Either<Failure, HomeConfig>> getHomeConfig({String? version});
  Future<Either<Failure, List<HomeSection>>> getHomeSections({String? userId});
  Future<Either<Failure, List<SponsoredAd>>> getSponsoredAds();
  Future<Either<Failure, List<CityDestination>>> getCityDestinations();
  Future<Either<Failure, void>> recordAdImpression({required String adId, String? additionalData});
  Future<Either<Failure, void>> recordAdClick({required String adId, String? additionalData});
  Future<Either<Failure, SectionDataModel?>> getSectionData({required String sectionId});
  Future<Either<Failure, void>> recordSectionImpression({required String sectionId});
  Future<Either<Failure, void>> recordSectionInteraction({required String sectionId, required String interactionType, String? itemId, Map<String, dynamic>? metadata});
}
