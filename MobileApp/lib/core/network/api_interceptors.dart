import 'package:dio/dio.dart';
import '../../services/local_storage_service.dart';
import '../constants/storage_constants.dart';
import '../constants/api_constants.dart';
import '../localization/locale_manager.dart';

class AuthInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    final localStorage = LocalStorageService();
    final token = await localStorage.getSecureString(StorageConstants.accessToken);
    
    if (token != null && token.isNotEmpty) {
      options.headers[ApiConstants.authorization] = '${ApiConstants.bearer} $token';
    }
    
    // Add current language to headers
    final locale = await LocaleManager.getCurrentLocale();
    options.headers[ApiConstants.acceptLanguage] = locale.languageCode;
    
    handler.next(options);
  }
}

class ErrorInterceptor extends Interceptor {
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode == 401) {
      // Handle token refresh or logout
      await _handleUnauthorized();
    }
    
    handler.next(err);
  }
  
  Future<void> _handleUnauthorized() async {
    final localStorage = LocalStorageService();
    
    // Try to refresh token
    final refreshToken = await localStorage.getSecureString(StorageConstants.refreshToken);
    
    if (refreshToken != null && refreshToken.isNotEmpty) {
      // TODO: Implement token refresh logic
      // If refresh fails, clear auth data and redirect to login
    } else {
      // Clear auth data
      await localStorage.clearSecureStorage();
      // TODO: Navigate to login screen
    }
  }
}

class LoggingInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    print('REQUEST[${options.method}] => PATH: ${options.path}');
    handler.next(options);
  }
  
  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    print('RESPONSE[${response.statusCode}] => PATH: ${response.requestOptions.path}');
    handler.next(response);
  }
  
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    print('ERROR[${err.response?.statusCode}] => PATH: ${err.requestOptions.path}');
    handler.next(err);
  }
}

class RetryInterceptor extends Interceptor {
  final int maxRetries;
  final Duration retryDelay;
  
  RetryInterceptor({
    this.maxRetries = 3,
    this.retryDelay = const Duration(seconds: 1),
  });
  
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    if (_shouldRetry(err)) {
      final retryCount = err.requestOptions.extra['retryCount'] ?? 0;
      
      if (retryCount < maxRetries) {
        err.requestOptions.extra['retryCount'] = retryCount + 1;
        
        await Future.delayed(retryDelay * (retryCount + 1));
        
        try {
          final response = await Dio().fetch(err.requestOptions);
          handler.resolve(response);
        } catch (e) {
          handler.next(err);
        }
      } else {
        handler.next(err);
      }
    } else {
      handler.next(err);
    }
  }
  
  bool _shouldRetry(DioException err) {
    return err.type == DioExceptionType.connectionTimeout ||
           err.type == DioExceptionType.receiveTimeout ||
           err.type == DioExceptionType.sendTimeout ||
           (err.response?.statusCode ?? 0) >= 500;
  }
}