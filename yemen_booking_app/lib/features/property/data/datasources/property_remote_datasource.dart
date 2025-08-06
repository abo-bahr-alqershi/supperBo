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
    required int guestsCount,
  });

  Future<List<ReviewModel>> getPropertyReviews({
    required String propertyId,
    int pageNumber = 1,
    int pageSize = 20,
    String? sortBy,
    String? sortDirection,
    bool withImagesOnly = false,
    String? userId,
  });

  Future<bool> addToFavorites({
    required String propertyId,
    required String userId,
    String? notes,
    DateTime? desiredVisitDate,
    double? expectedBudget,
    String currency = 'YER',
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
    required int guestsCount,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/client/units/available',
        queryParameters: <String, dynamic>{
          'propertyId': propertyId,
          if (checkInDate != null) 'checkIn': checkInDate.toIso8601String(),
          if (checkOutDate != null) 'checkOut': checkOutDate.toIso8601String(),
          'guestsCount': guestsCount,
        },
      );

      if (response.statusCode == 200) {
        // extract the units list from the wrapper
        final data = response.data['data']['units'] as List;
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
    String? sortBy,
    String? sortDirection,
    bool withImagesOnly = false,
    String? userId,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/client/reviews/property',
        queryParameters: <String, dynamic>{
          'propertyId': propertyId,
          'pageNumber': pageNumber,
          'pageSize': pageSize,
          if (sortBy != null) 'sortBy': sortBy,
          if (sortDirection != null) 'sortDirection': sortDirection,
          'withImagesOnly': withImagesOnly,
          if (userId != null) 'userId': userId,
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
    String? notes,
    DateTime? desiredVisitDate,
    double? expectedBudget,
    String currency = 'YER',
  }) async {
    try {
      final response = await apiClient.post(
        '/api/client/properties/wishlist',
        data: <String, dynamic>{
          'propertyId': propertyId,
          'userId': userId,
          if (notes != null) 'notes': notes,
          if (desiredVisitDate != null) 'desiredVisitDate': desiredVisitDate.toIso8601String(),
          if (expectedBudget != null) 'expectedBudget': expectedBudget,
          'currency': currency,
        },
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        // the API returns the boolean in the 'data' key
        return response.data['data'] ?? false;
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
        // extract the result from the 'data' key
        return response.data['data'] ?? false;
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
        // API uses 'data' for the boolean result
        return response.data['data'] ?? false;
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