import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'exceptions.dart';
import 'failures.dart';

class ErrorHandler {
  static Either<Failure, T> handle<T>(dynamic error) {
    if (error is DioException) {
      return Left(_handleDioError(error));
    } else if (error is ApiException) {
      return Left(_handleApiException(error));
    } else if (error is ServerException) {
      return Left(ServerFailure(
        message: error.message,
        code: error.statusCode,
        data: error.data,
      ));
    } else if (error is NetworkException) {
      return Left(NetworkFailure(message: error.message));
    } else if (error is CacheException) {
      return Left(CacheFailure(message: error.message));
    } else if (error is AuthenticationException) {
      return Left(AuthenticationFailure(
        message: error.message,
        code: error.statusCode,
      ));
    } else if (error is UnauthorizedException) {
      return Left(UnauthorizedFailure(message: error.message));
    } else if (error is ValidationException) {
      return Left(ValidationFailure(
        message: error.message,
        errors: error.errors,
      ));
    } else if (error is NotFoundException) {
      return Left(NotFoundFailure(message: error.message));
    } else if (error is TimeoutException) {
      return Left(TimeoutFailure(message: error.message));
    } else {
      return Left(UnknownFailure(
        message: error.toString(),
      ));
    }
  }

  static Failure _handleDioError(DioException error) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return const TimeoutFailure();
      case DioExceptionType.badResponse:
        return _handleBadResponse(error.response);
      case DioExceptionType.cancel:
        return const UnknownFailure(message: 'تم إلغاء الطلب');
      case DioExceptionType.connectionError:
        return const NetworkFailure();
      case DioExceptionType.badCertificate:
        return const ServerFailure(message: 'خطأ في شهادة الأمان');
      case DioExceptionType.unknown:
      default:
        return const UnknownFailure();
    }
  }

  static Failure _handleBadResponse(Response? response) {
    if (response == null) {
      return const ServerFailure(message: 'لا توجد استجابة من الخادم');
    }

    final statusCode = response.statusCode;
    final data = response.data;
    String message = 'حدث خطأ';

    if (data is Map<String, dynamic>) {
      message = data['message'] ?? data['error'] ?? message;
    }

    switch (statusCode) {
      case 400:
        return ServerFailure(message: message, code: statusCode);
      case 401:
        if (message.contains('expired') || message.contains('انتهت')) {
          return SessionExpiredFailure(message: message);
        }
        return UnauthorizedFailure(message: message);
      case 403:
        return PermissionDeniedFailure(message: message);
      case 404:
        return NotFoundFailure(message: message);
      case 422:
        Map<String, List<String>>? errors;
        if (data is Map<String, dynamic> && data.containsKey('errors')) {
          errors = Map<String, List<String>>.from(
            data['errors'].map((key, value) => MapEntry(
              key,
              value is List ? List<String>.from(value) : [value.toString()],
            )),
          );
        }
        return ValidationFailure(message: message, errors: errors);
      case 500:
      case 502:
      case 503:
        return ServerFailure(message: message, code: statusCode);
      default:
        return ServerFailure(message: message, code: statusCode);
    }
  }

  static Failure _handleApiException(ApiException error) {
    if (error.statusCode == 401) {
      return UnauthorizedFailure(message: error.message);
    } else if (error.statusCode == 403) {
      return PermissionDeniedFailure(message: error.message);
    } else if (error.statusCode == 404) {
      return NotFoundFailure(message: error.message);
    } else if (error.statusCode == 422) {
      return ValidationFailure(message: error.message);
    } else if (error.type == DioExceptionType.connectionError) {
      return NetworkFailure(message: error.message);
    } else if (error.type == DioExceptionType.connectionTimeout ||
        error.type == DioExceptionType.receiveTimeout ||
        error.type == DioExceptionType.sendTimeout) {
      return TimeoutFailure(message: error.message);
    } else {
      return ServerFailure(
        message: error.message,
        code: error.statusCode,
        data: error.data,
      );
    }
  }

  // Helper method to extract error message
  static String getErrorMessage(Failure failure) {
    if (failure is ValidationFailure && failure.errors != null) {
      // Join all validation errors
      final allErrors = failure.errors!.values
          .expand((errors) => errors)
          .join('\n');
      return allErrors.isNotEmpty ? allErrors : failure.message;
    }
    return failure.message;
  }
}