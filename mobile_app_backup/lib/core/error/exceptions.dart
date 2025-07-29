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

// Keep the existing ApiException class as is...