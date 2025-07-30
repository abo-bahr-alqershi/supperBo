import 'package:dio/dio.dart';

class ServerException implements Exception {
  final String message;
  final int? statusCode;
  final dynamic data;

  ServerException({
    required this.message,
    this.statusCode,
    this.data,
  });
}

class NetworkException implements Exception {
  final String message;

  NetworkException({
    this.message = 'No internet connection',
  });
}

class CacheException implements Exception {
  final String message;

  CacheException({
    this.message = 'Cache error',
  });
}

class AuthenticationException implements Exception {
  final String message;
  final int? statusCode;

  AuthenticationException({
    required this.message,
    this.statusCode,
  });
}

class UnauthorizedException implements Exception {
  final String message;

  UnauthorizedException({
    this.message = 'Unauthorized access',
  });
}

class ValidationException implements Exception {
  final String message;
  final Map<String, List<String>>? errors;

  ValidationException({
    required this.message,
    this.errors,
  });

  // Helper method to parse validation errors from ResultDto
  static ValidationException fromResultDto(dynamic data) {
    if (data is Map<String, dynamic>) {
      final message = data['message'] ?? 'Validation error';
      final errors = <String, List<String>>{};
      
      // Parse errors from ResultDto format
      if (data['errors'] != null) {
        if (data['errors'] is List) {
          errors['general'] = List<String>.from(data['errors']);
        } else if (data['errors'] is Map) {
          (data['errors'] as Map).forEach((key, value) {
            if (value is List) {
              errors[key] = List<String>.from(value);
            } else {
              errors[key] = [value.toString()];
            }
          });
        }
      }
      
      return ValidationException(message: message, errors: errors);
    }
    
    return ValidationException(message: 'Validation error');
  }
}

class NotFoundException implements Exception {
  final String message;

  NotFoundException({
    this.message = 'Resource not found',
  });
}

class TimeoutException implements Exception {
  final String message;

  TimeoutException({
    this.message = 'Request timeout',
  });
}

class ApiException implements Exception {
  final String message;
  final int? statusCode;
  final dynamic data;
  final DioExceptionType? type;

  ApiException({
    required this.message,
    this.statusCode,
    this.data,
    this.type,
  });

  factory ApiException.fromDioException(DioException dioException) {
    String message;
    int? statusCode = dioException.response?.statusCode;
    
    switch (dioException.type) {
      case DioExceptionType.connectionTimeout:
        message = 'Connection timeout';
        break;
      case DioExceptionType.sendTimeout:
        message = 'Send timeout';
        break;
      case DioExceptionType.receiveTimeout:
        message = 'Receive timeout';
        break;
      case DioExceptionType.badResponse:
        message = _handleStatusCode(statusCode);
        break;
      case DioExceptionType.cancel:
        message = 'Request cancelled';
        break;
      case DioExceptionType.connectionError:
        message = 'Connection error';
        break;
      case DioExceptionType.badCertificate:
        message = 'Bad certificate';
        break;
      case DioExceptionType.unknown:
        message = 'Unknown error occurred';
        break;
    }

    return ApiException(
      message: message,
      statusCode: statusCode,
      data: dioException.response?.data,
      type: dioException.type,
    );
  }

  static String _handleStatusCode(int? statusCode) {
    switch (statusCode) {
      case 400:
        return 'Bad request';
      case 401:
        return 'Unauthorized';
      case 403:
        return 'Forbidden';
      case 404:
        return 'Not found';
      case 409:
        return 'Conflict';
      case 422:
        return 'Validation error';
      case 500:
        return 'Internal server error';
      case 502:
        return 'Bad gateway';
      case 503:
        return 'Service unavailable';
      default:
        return 'HTTP error: $statusCode';
    }
  }

  @override
  String toString() {
    return 'ApiException(message: $message, statusCode: $statusCode)';
  }
}