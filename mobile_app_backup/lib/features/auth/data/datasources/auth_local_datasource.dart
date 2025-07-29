import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../../core/error/exceptions.dart';
import '../models/auth_response_model.dart';
import '../models/user_model.dart';

abstract class AuthLocalDataSource {
  Future<void> cacheAuthResponse(AuthResponseModel authResponse);
  Future<AuthResponseModel?> getCachedAuthResponse();
  Future<void> cacheAccessToken(String token);
  Future<String?> getCachedAccessToken();
  Future<void> cacheRefreshToken(String token);
  Future<String?> getCachedRefreshToken();
  Future<void> cacheUser(UserModel user);
  Future<UserModel?> getCachedUser();
  Future<void> clearAuthData();
  Future<bool> isLoggedIn();
}

class AuthLocalDataSourceImpl implements AuthLocalDataSource {
  final SharedPreferences sharedPreferences;

  AuthLocalDataSourceImpl({required this.sharedPreferences});

  static const String _authResponseKey = 'AUTH_RESPONSE';
  static const String _accessTokenKey = 'ACCESS_TOKEN';
  static const String _refreshTokenKey = 'REFRESH_TOKEN';
  static const String _userKey = 'USER_DATA';

  @override
  Future<void> cacheAuthResponse(AuthResponseModel authResponse) async {
    try {
      final jsonString = json.encode(authResponse.toJson());
      await sharedPreferences.setString(_authResponseKey, jsonString);
      await cacheAccessToken(authResponse.accessToken);
      await cacheRefreshToken(authResponse.refreshToken);
      await cacheUser(authResponse.user as UserModel);
    } catch (e) {
      throw CacheException(message: 'فشل حفظ بيانات المصادقة');
    }
  }

  @override
  Future<AuthResponseModel?> getCachedAuthResponse() async {
    try {
      final jsonString = sharedPreferences.getString(_authResponseKey);
      if (jsonString != null) {
        final jsonMap = json.decode(jsonString) as Map<String, dynamic>;
        return AuthResponseModel.fromJson(jsonMap);
      }
      return null;
    } catch (e) {
      throw CacheException(message: 'فشل قراءة بيانات المصادقة');
    }
  }

  @override
  Future<void> cacheAccessToken(String token) async {
    try {
      await sharedPreferences.setString(_accessTokenKey, token);
    } catch (e) {
      throw CacheException(message: 'فشل حفظ رمز الوصول');
    }
  }

  @override
  Future<String?> getCachedAccessToken() async {
    try {
      return sharedPreferences.getString(_accessTokenKey);
    } catch (e) {
      throw CacheException(message: 'فشل قراءة رمز الوصول');
    }
  }

  @override
  Future<void> cacheRefreshToken(String token) async {
    try {
      await sharedPreferences.setString(_refreshTokenKey, token);
    } catch (e) {
      throw CacheException(message: 'فشل حفظ رمز التحديث');
    }
  }

  @override
  Future<String?> getCachedRefreshToken() async {
    try {
      return sharedPreferences.getString(_refreshTokenKey);
    } catch (e) {
      throw CacheException(message: 'فشل قراءة رمز التحديث');
    }
  }

  @override
  Future<void> cacheUser(UserModel user) async {
    try {
      final jsonString = json.encode(user.toJson());
      await sharedPreferences.setString(_userKey, jsonString);
    } catch (e) {
      throw CacheException(message: 'فشل حفظ بيانات المستخدم');
    }
  }

  @override
  Future<UserModel?> getCachedUser() async {
    try {
      final jsonString = sharedPreferences.getString(_userKey);
      if (jsonString != null) {
        final jsonMap = json.decode(jsonString) as Map<String, dynamic>;
        return UserModel.fromJson(jsonMap);
      }
      return null;
    } catch (e) {
      throw CacheException(message: 'فشل قراءة بيانات المستخدم');
    }
  }

  @override
  Future<void> clearAuthData() async {
    try {
      await Future.wait([
        sharedPreferences.remove(_authResponseKey),
        sharedPreferences.remove(_accessTokenKey),
        sharedPreferences.remove(_refreshTokenKey),
        sharedPreferences.remove(_userKey),
      ]);
    } catch (e) {
      throw CacheException(message: 'فشل مسح بيانات المصادقة');
    }
  }

  @override
  Future<bool> isLoggedIn() async {
    try {
      final token = await getCachedAccessToken();
      return token != null && token.isNotEmpty;
    } catch (e) {
      return false;
    }
  }
}