import 'package:dio/dio.dart';
import 'package:yemen_booking_app/core/models/paginated_result.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/error/exceptions.dart';
import '../models/search_properties_response_model.dart';
import '../../../../core/models/result_dto.dart';
import '../../../../core/network/api_client.dart';
import '../models/search_filter_model.dart';
import '../models/search_result_model.dart';

abstract class SearchRemoteDataSource {
  Future<ResultDto<SearchPropertiesResponseModel>> searchProperties({
    String? searchTerm,
    String? city,
    String? propertyTypeId,
    double? minPrice,
    double? maxPrice,
    int? minStarRating,
    List<String>? requiredAmenities,
    String? unitTypeId,
    List<String>? serviceIds,
    Map<String, dynamic>? dynamicFieldFilters,
    DateTime? checkIn,
    DateTime? checkOut,
    int? guestsCount,
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
  Future<ResultDto<SearchPropertiesResponseModel>> searchProperties({
    String? searchTerm,
    String? city,
    String? propertyTypeId,
    double? minPrice,
    double? maxPrice,
    int? minStarRating,
    List<String>? requiredAmenities,
    String? unitTypeId,
    List<String>? serviceIds,
    Map<String, dynamic>? dynamicFieldFilters,
    DateTime? checkIn,
    DateTime? checkOut,
    int? guestsCount,
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
      if (searchTerm != null) queryParams['searchTerm'] = searchTerm;
      if (city != null) queryParams['city'] = city;
      if (propertyTypeId != null) queryParams['propertyTypeId'] = propertyTypeId;
      if (minPrice != null) queryParams['minPrice'] = minPrice;
      if (maxPrice != null) queryParams['maxPrice'] = maxPrice;
      if (minStarRating != null) queryParams['minStarRating'] = minStarRating;
      if (requiredAmenities != null && requiredAmenities.isNotEmpty) {
        queryParams['requiredAmenities'] = requiredAmenities.join(',');
      }
      if (unitTypeId != null) queryParams['unitTypeId'] = unitTypeId;
      if (serviceIds != null && serviceIds.isNotEmpty) {
        queryParams['serviceIds'] = serviceIds.join(',');
      }
      if (dynamicFieldFilters != null && dynamicFieldFilters.isNotEmpty) {
        queryParams['dynamicFieldFilters'] = dynamicFieldFilters;
      }
      if (checkIn != null) queryParams['checkIn'] = checkIn.toIso8601String();
      if (checkOut != null) queryParams['checkOut'] = checkOut.toIso8601String();
      if (guestsCount != null) queryParams['guestsCount'] = guestsCount;
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
          (dataJson) => SearchPropertiesResponseModel.fromJson(
            dataJson as Map<String, dynamic>,
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
          (json) => (json as List<dynamic>?)?.map((e) => e as String).toList() ?? [],
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
          (dataJson) => (dataJson as List)
              .map<String>((e) => (e as Map<String, dynamic>)['cityName'] as String)
              .toList(),
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