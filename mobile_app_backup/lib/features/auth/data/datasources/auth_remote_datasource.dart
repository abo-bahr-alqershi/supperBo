import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/models/result_dto.dart';
import '../../../../core/network/api_client.dart';
import '../models/auth_response_model.dart';
import '../models/user_model.dart';

abstract class AuthRemoteDataSource {
  Future<AuthResponseModel> login({
    required String emailOrPhone,
    required String password,
    required bool rememberMe,
  });

  Future<AuthResponseModel> register({
    required String name,
    required String email,  
    required String phone,
    required String password,
    required String passwordConfirmation,
  });

  Future<void> logout();

  Future<void> resetPassword({
    required String emailOrPhone,
  });

  Future<AuthResponseModel> refreshToken({
    required String refreshToken,
  });

  Future<UserModel> getCurrentUser();

  Future<void> updateProfile({
    required String name,
    String? email,
    String? phone,
  });

  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String newPasswordConfirmation,
  });
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final ApiClient apiClient;

  AuthRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<AuthResponseModel> login({
    required String emailOrPhone,
    required String password,
    required bool rememberMe,
  }) async {
    try {
      final response = await apiClient.post(
        '/auth/login',
        data: {
          'emailOrPhone': emailOrPhone,
          'password': password,
          'rememberMe': rememberMe,
        },
      );

      final resultDto = ResultDto<Map<String, dynamic>>.fromJson(
        response.data,
        (json) => json,
      );

      if (resultDto.success && resultDto.data != null) {
        return AuthResponseModel.fromJson(resultDto.data!);
      } else {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل تسجيل الدخول',
          statusCode: response.statusCode,
          data: resultDto.errors,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<AuthResponseModel> register({
    required String name,
    required String email,
    required String phone,
    required String password,
    required String passwordConfirmation,
  }) async {
    try {
      final response = await apiClient.post(
        '/auth/register',
        data: {
          'name': name,
          'email': email,
          'phone': phone,
          'password': password,
          'passwordConfirmation': passwordConfirmation,
        },
      );

      final resultDto = ResultDto<Map<String, dynamic>>.fromJson(
        response.data,
        (json) => json,
      );

      if (resultDto.success && resultDto.data != null) {
        return AuthResponseModel.fromJson(resultDto.data!);
      } else {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل إنشاء الحساب',
          statusCode: response.statusCode,
          data: resultDto.errors,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<void> logout() async {
    try {
      final response = await apiClient.post('/auth/logout');
      
      final resultDto = ResultDtoVoid.fromJson(response.data);
      
      if (!resultDto.success) {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل تسجيل الخروج',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<void> resetPassword({required String emailOrPhone}) async {
    try {
      final response = await apiClient.post(
        '/auth/reset-password',
        data: {
          'emailOrPhone': emailOrPhone,
        },
      );

      final resultDto = ResultDtoVoid.fromJson(response.data);
      
      if (!resultDto.success) {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل إرسال رابط إعادة تعيين كلمة المرور',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<AuthResponseModel> refreshToken({required String refreshToken}) async {
    try {
      final response = await apiClient.post(
        '/auth/refresh',
        data: {
          'refreshToken': refreshToken,
        },
      );

      final resultDto = ResultDto<Map<String, dynamic>>.fromJson(
        response.data,
        (json) => json,
      );

      if (resultDto.success && resultDto.data != null) {
        return AuthResponseModel.fromJson(resultDto.data!);
      } else {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل تحديث الجلسة',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<UserModel> getCurrentUser() async {
    try {
      final response = await apiClient.get('/auth/me');

      final resultDto = ResultDto<Map<String, dynamic>>.fromJson(
        response.data,
        (json) => json,
      );

      if (resultDto.success && resultDto.data != null) {
        return UserModel.fromJson(resultDto.data!);
      } else {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل جلب بيانات المستخدم',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
            } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<void> updateProfile({
    required String name,
    String? email,
    String? phone,
  }) async {
    try {
      final data = <String, dynamic>{
        'name': name,
      };
      
      if (email != null) data['email'] = email;
      if (phone != null) data['phone'] = phone;

      final response = await apiClient.put(
        '/auth/profile',
        data: data,
      );

      final resultDto = ResultDtoVoid.fromJson(response.data);
      
      if (!resultDto.success) {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل تحديث الملف الشخصي',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }

  @override
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String newPasswordConfirmation,
  }) async {
    try {
      final response = await apiClient.post(
        '/auth/change-password',
        data: {
          'currentPassword': currentPassword,
          'newPassword': newPassword,
          'newPasswordConfirmation': newPasswordConfirmation,
        },
      );

      final resultDto = ResultDtoVoid.fromJson(response.data);
      
      if (!resultDto.success) {
        throw ServerException(
          message: resultDto.message ?? 
                   resultDto.errors.join(', ') ?? 
                   'فشل تغيير كلمة المرور',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(message: e.toString());
    }
  }
}