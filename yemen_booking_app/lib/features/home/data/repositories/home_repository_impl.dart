import 'package:dartz/dartz.dart';
import '../../../../core/error/error_handler.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/city_destination.dart';
import '../../domain/entities/home_config.dart';
import '../../domain/entities/home_section.dart';
import '../../domain/entities/sponsored_ad.dart';
import '../../domain/repositories/home_repository.dart';
import '../datasources/home_local_datasource.dart';
import '../datasources/home_remote_datasource.dart';

class HomeRepositoryImpl implements HomeRepository {
  final HomeRemoteDataSource remoteDataSource;
  final HomeLocalDataSource localDataSource;
  final NetworkInfo networkInfo;

  HomeRepositoryImpl({
    required this.remoteDataSource,
    required this.localDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, HomeConfig>> getHomeConfig({String? version}) async {
    if (await networkInfo.isConnected) {
      try {
        final remoteConfig = await remoteDataSource.getHomeConfig(version: version);
        await localDataSource.cacheHomeConfig(remoteConfig);
        return Right(remoteConfig.toEntity());
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      try {
        final localConfig = await localDataSource.getCachedHomeConfig();
        if (localConfig != null) {
          return Right(localConfig.toEntity());
        } else {
          return const Left(NetworkFailure('لا يوجد اتصال بالإنترنت ولا توجد بيانات محفوظة'));
        }
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    }
  }

  @override
  Future<Either<Failure, List<HomeSection>>> getHomeSections({String? userId}) async {
    if (await networkInfo.isConnected) {
      try {
        final remoteSections = await remoteDataSource.getHomeSections(userId: userId);
        await localDataSource.cacheHomeSections(remoteSections);
        return Right(remoteSections.map((s) => s.toEntity()).toList());
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      try {
        final localSections = await localDataSource.getCachedHomeSections();
        if (localSections != null) {
          return Right(localSections.map((s) => s.toEntity()).toList());
        } else {
          return const Left(NetworkFailure('لا يوجد اتصال بالإنترنت ولا توجد بيانات محفوظة'));
        }
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    }
  }

  @override
  Future<Either<Failure, List<SponsoredAd>>> getSponsoredAds() async {
    if (await networkInfo.isConnected) {
      try {
        final result = await remoteDataSource.getSponsoredAds();
        return Right(result.map((e) => e.toEntity()).toList());
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, List<CityDestination>>> getCityDestinations() async {
    if (await networkInfo.isConnected) {
      try {
        final result = await remoteDataSource.getCityDestinations();
        return Right(result.map((e) => e.toEntity()).toList());
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, void>> recordAdImpression({required String adId, String? additionalData}) async {
    try {
      await remoteDataSource.recordAdImpression(adId: adId, additionalData: additionalData);
      return const Right(null);
    } catch (e) {
      return ErrorHandler.handle(e);
    }
  }

  @override
  Future<Either<Failure, void>> recordAdClick({required String adId, String? additionalData}) async {
    try {
      await remoteDataSource.recordAdClick(adId: adId, additionalData: additionalData);
      return const Right(null);
    } catch (e) {
      return ErrorHandler.handle(e);
    }
  }
}
