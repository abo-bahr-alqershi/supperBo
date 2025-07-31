import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/models/paginated_result.dart';
import '../entities/search_filter.dart';
import '../entities/search_result.dart';

abstract class SearchRepository {
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
  });

  Future<Either<Failure, SearchFilters>> getSearchFilters();

  Future<Either<Failure, List<String>>> getSearchSuggestions({
    required String query,
    int limit = 10,
  });

  Future<Either<Failure, List<SearchResult>>> getRecommendedProperties({
    String? userId,
    int limit = 10,
  });

  Future<Either<Failure, List<String>>> getPopularDestinations({
    int limit = 10,
  });
}