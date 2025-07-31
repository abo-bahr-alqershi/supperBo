import 'package:dartz/dartz.dart';
import '../../../../core/error/error_handler.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/models/paginated_result.dart';
import '../../domain/entities/search_filter.dart';
import '../../domain/entities/search_result.dart';
import '../../domain/repositories/search_repository.dart';
import '../datasources/search_remote_datasource.dart';
import '../models/search_filter_model.dart';
import '../models/search_result_model.dart';

class SearchRepositoryImpl implements SearchRepository {
  final SearchRemoteDataSource remoteDataSource;

  SearchRepositoryImpl({
    required this.remoteDataSource,
  });

  @override
  Future<Either<Failure, PaginatedResult<SearchResult>>> searchProperties({
    String? searchQuery,
    String? city,
    String? propertyType,
    double? minPrice,
    double? maxPrice,
    int? minRating,
    List<String>? amenities,
    DateTime? checkInDate,
    DateTime? checkOutDate,
    int? guests,
    double? latitude,
    double? longitude,
    double? radiusKm,
    String? sortBy,
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    try {
      final result = await remoteDataSource.searchProperties(
        searchTerm: searchQuery,
        city: city,
        propertyTypeId: propertyType,
        minPrice: minPrice,
        maxPrice: maxPrice,
        minStarRating: minRating,
        requiredAmenities: amenities,
        unitTypeId: null,
        serviceIds: null,
        dynamicFieldFilters: null,
        checkIn: checkInDate,
        checkOut: checkOutDate,
        guestsCount: guests,
        latitude: latitude,
        longitude: longitude,
        radiusKm: radiusKm,
        sortBy: sortBy,
        pageNumber: pageNumber,
        pageSize: pageSize,
      );

      if (result.isSuccess && result.data != null) {
        final dto = result.data!;
        final paginatedResult = PaginatedResult<SearchResult>(
          items: dto.properties.map((model) => model as SearchResult).toList(),
          pageNumber: dto.currentPage,
          pageSize: dto.pageSize,
          totalCount: dto.totalCount,
          metadata: {
            'totalPages': dto.totalPages,
            'hasPreviousPage': dto.hasPreviousPage,
            'hasNextPage': dto.hasNextPage,
            'appliedFilters': dto.appliedFilters,
            'searchTimeMs': dto.searchTimeMs,
            'statistics': dto.statistics,
          },
        );
        return Right(paginatedResult);
      } else {
        return Left(ServerFailure(result.message ?? 'Search failed'));
      }
    } catch (error) {
      return ErrorHandler.handle(error);
    }
  }

  @override
  Future<Either<Failure, SearchFilters>> getSearchFilters() async {
    try {
      final result = await remoteDataSource.getSearchFilters();

      if (result.isSuccess && result.data != null) {
        return Right(result.data! as SearchFilters);
      } else {
        return Left(ServerFailure(result.message ?? 'Failed to get search filters'));
      }
    } catch (error) {
      return ErrorHandler.handle(error);
    }
  }

  @override
  Future<Either<Failure, List<String>>> getSearchSuggestions({
    required String query,
    int limit = 10,
  }) async {
    try {
      final result = await remoteDataSource.getSearchSuggestions(
        query: query,
        limit: limit,
      );

      if (result.isSuccess && result.data != null) {
        return Right(result.data!);
      } else {
        return Left(ServerFailure(result.message ?? 'Failed to get suggestions'));
      }
    } catch (error) {
      return ErrorHandler.handle(error);
    }
  }

  @override
  Future<Either<Failure, List<SearchResult>>> getRecommendedProperties({
    String? userId,
    int limit = 10,
  }) async {
    try {
      final result = await remoteDataSource.getRecommendedProperties(
        userId: userId,
        limit: limit,
      );

      if (result.isSuccess && result.data != null) {
        final recommendations = result.data!.items
            .map((model) => model as SearchResult)
            .toList();
        return Right(recommendations);
      } else {
        return Left(ServerFailure(result.message ?? 'Failed to get recommendations'));
      }
    } catch (error) {
      return ErrorHandler.handle(error);
    }
  }

  @override
  Future<Either<Failure, List<String>>> getPopularDestinations({
    int limit = 10,
  }) async {
    try {
      final result = await remoteDataSource.getPopularDestinations(
        limit: limit,
      );

      if (result.isSuccess && result.data != null) {
        return Right(result.data!);
      } else {
        return Left(ServerFailure(result.message ?? 'Failed to get popular destinations'));
      }
    } catch (error) {
      return ErrorHandler.handle(error);
    }
  }
}