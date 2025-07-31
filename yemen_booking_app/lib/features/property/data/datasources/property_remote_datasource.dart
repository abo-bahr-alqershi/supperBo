import 'package:dio/dio.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../models/property_detail_model.dart';
import '../models/unit_model.dart';
import '../models/review_model.dart';

abstract class PropertyRemoteDataSource {
  Future<PropertyDetailModel> getPropertyDetails({
    required String propertyId,
    String? userId,
  });

  Future<List<UnitModel>> getPropertyUnits({
    required String propertyId,
    DateTime? checkInDate,
    DateTime? checkOutDate,
  });

  Future<List<ReviewModel>> getPropertyReviews({
    required String propertyId,
    int pageNumber = 1,
    int pageSize = 20,
  });

  Future<bool> addToFavorites({
    required String propertyId,
    required String userId,
  });

  Future<bool> removeFromFavorites({
    required String propertyId,
    required String userId,
  });

  Future<bool> updateViewCount({
    required String propertyId,
  });
}

class PropertyRemoteDataSourceImpl implements PropertyRemoteDataSource {
  final ApiClient apiClient;

  PropertyRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<PropertyDetailModel> getPropertyDetails({
    required String propertyId,
    String? userId,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (userId != null) {
        queryParams['userId'] = userId;
      }

      final response = await apiClient.get(
        '/api/client/properties/$propertyId',
        queryParameters: queryParams,
      );

      if (response.statusCode == 200) {
        final data = response.data['data'];
        return PropertyDetailModel.fromJson(data);
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to load property details');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<List<UnitModel>> getPropertyUnits({
    required String propertyId,
    DateTime? checkInDate,
    DateTime? checkOutDate,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (checkInDate != null) {
        queryParams['checkInDate'] = checkInDate.toIso8601String();
      }
      if (checkOutDate != null) {
        queryParams['checkOutDate'] = checkOutDate.toIso8601String();
      }

      final response = await apiClient.get(
        '/api/client/properties/$propertyId/units',
        queryParameters: queryParams,
      );

      if (response.statusCode == 200) {
        final data = response.data['data'] as List;
        return data.map((json) => UnitModel.fromJson(json)).toList();
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to load units');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<List<ReviewModel>> getPropertyReviews({
    required String propertyId,
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/client/properties/$propertyId/reviews',
        queryParameters: {
          'pageNumber': pageNumber,
          'pageSize': pageSize,
        },
      );

      if (response.statusCode == 200) {
        final data = response.data['data']['items'] as List;
        return data.map((json) => ReviewModel.fromJson(json)).toList();
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to load reviews');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<bool> addToFavorites({
    required String propertyId,
    required String userId,
  }) async {
    try {
      final response = await apiClient.post(
        '/api/client/properties/wishlist',
        data: {
          'propertyId': propertyId,
          'userId': userId,
        },
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        return response.data['isSuccess'] ?? false;
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to add to favorites');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<bool> removeFromFavorites({
    required String propertyId,
    required String userId,
  }) async {
    try {
      final response = await apiClient.delete(
        '/api/client/favorites',
        data: {
          'propertyId': propertyId,
          'userId': userId,
        },
      );

      if (response.statusCode == 200) {
        return response.data['isSuccess'] ?? false;
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to remove from favorites');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }

  @override
  Future<bool> updateViewCount({
    required String propertyId,
  }) async {
    try {
      final response = await apiClient.post(
        '/api/client/properties/view-count',
        data: {
          'propertyId': propertyId,
        },
      );

      if (response.statusCode == 200) {
        return response.data['isSuccess'] ?? false;
      } else {
        throw ServerException(response.data['message'] ?? 'Failed to update view count');
      }
    } on DioException catch (e) {
      throw ServerException(e.message ?? 'Network error occurred');
    } catch (e) {
      throw ServerException('Unexpected error: $e');
    }
  }
}