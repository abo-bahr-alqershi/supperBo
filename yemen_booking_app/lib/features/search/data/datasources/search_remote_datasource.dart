import 'package:dio/dio.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/models/paginated_result.dart';
import '../../../../core/models/result_dto.dart';
import '../../../../core/network/api_client.dart';
import '../models/search_filter_model.dart';
import '../models/search_result_model.dart';

abstract class SearchRemoteDataSource {
  Future<ResultDto<PaginatedResult<SearchResultModel>>> searchProperties({
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

  Future<ResultDto<SearchFiltersModel>> getSearchFilters();

  Future<ResultDto<List<String>>> getSearchSuggestions({
    required String query,
    int limit = 10,
  });

  Future<ResultDto<PaginatedResult<SearchResultModel>>> getRecommendedProperties({
    String? userId,
    int limit = 10,
  });

  Future<ResultDto<List<String>>> getPopularDestinations({
    int limit = 10,
  });
}

class SearchRemoteDataSourceImpl implements SearchRemoteDataSource {
  final ApiClient apiClient;

  SearchRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<ResultDto<PaginatedResult<SearchResultModel>>> searchProperties({
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
      final queryParams = <String, dynamic>{
        'pageNumber': pageNumber,
        'pageSize': pageSize,
      };

      if (searchQuery != null) queryParams['searchQuery'] = searchQuery;
      if (city != null) queryParams['city'] = city;
      if (propertyType != null) queryParams['propertyType'] = propertyType;
      if (minPrice != null) queryParams['minPrice'] = minPrice;
      if (maxPrice != null) queryParams['maxPrice'] = maxPrice;
      if (minRating != null) queryParams['minRating'] = minRating;
      if (amenities != null && amenities.isNotEmpty) {
        queryParams['amenities'] = amenities.join(',');
      }
      if (checkInDate != null) queryParams['checkInDate'] = checkInDate.toIso8601String();
      if (checkOutDate != null) queryParams['checkOutDate'] = checkOutDate.toIso8601String();
      if (guests != null) queryParams['guests'] = guests;
      if (latitude != null) queryParams['latitude'] = latitude;
      if (longitude != null) queryParams['longitude'] = longitude;
      if (radiusKm != null) queryParams['radiusKm'] = radiusKm;
      if (sortBy != null) queryParams['sortBy'] = sortBy;

      final response = await apiClient.get(
        '/api/client/properties/search',
        queryParameters: queryParams,
      );

      if (response.statusCode == 200) {
        return ResultDto.fromJson(
          response.data,
          (json) => PaginatedResult.fromJson(
            json,
            (itemJson) => SearchResultModel.fromJson(itemJson),
          ),
        );
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to search properties');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<ResultDto<SearchFiltersModel>> getSearchFilters() async {
    try {
      final response = await apiClient.get('/api/client/search-filters/filters');

      if (response.statusCode == 200) {
        return ResultDto.fromJson(
          response.data,
          (json) => SearchFiltersModel.fromJson(json),
        );
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to load search filters');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<ResultDto<List<String>>> getSearchSuggestions({
    required String query,
    int limit = 10,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/client/search/suggestions',
        queryParameters: {
          'query': query,
          'limit': limit,
        },
      );

      if (response.statusCode == 200) {
        return ResultDto.fromJson(
          response.data,
          (json) => List<String>.from(json ?? []),
        );
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to get suggestions');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<ResultDto<PaginatedResult<SearchResultModel>>> getRecommendedProperties({
    String? userId,
    int limit = 10,
  }) async {
    try {
      final queryParams = <String, dynamic>{
        'limit': limit,
      };
      if (userId != null) queryParams['userId'] = userId;

      final response = await apiClient.get(
        '/api/client/search-filters/recommended-properties',
        queryParameters: queryParams,
      );

      if (response.statusCode == 200) {
        return ResultDto.fromJson(
          response.data,
          (json) => PaginatedResult.fromJson(
            json,
            (itemJson) => SearchResultModel.fromJson(itemJson),
          ),
        );
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to get recommended properties');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<ResultDto<List<String>>> getPopularDestinations({
    int limit = 10,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/client/search-filters/popular-destinations',
        queryParameters: {
          'limit': limit,
        },
      );

      if (response.statusCode == 200) {
        return ResultDto.fromJson(
          response.data,
          (json) => List<String>.from(json ?? []),
        );
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to get popular destinations');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }
}