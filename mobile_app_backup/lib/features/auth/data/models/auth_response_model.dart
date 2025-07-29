import '../../domain/entities/auth_response.dart';
import 'user_model.dart';

class AuthResponseModel extends AuthResponse {
  const AuthResponseModel({
    required super.user,
    required super.accessToken,
    required super.refreshToken,
    super.expiresAt,
  });

  factory AuthResponseModel.fromJson(Map<String, dynamic> json) {
    // تحقق من وجود userId أو id
    final userJson = <String, dynamic>{
      'userId': json['userId'] ?? json['id'],
      'name': json['name'],
      'email': json['email'],
      'phone': json['phone'],
      'roles': json['roles'],
      'accessToken': json['accessToken'],
      'refreshToken': json['refreshToken'],
    };

    return AuthResponseModel(
      user: UserModel.fromJson(userJson),
      accessToken: json['accessToken'] ?? '',
      refreshToken: json['refreshToken'] ?? '',
      expiresAt: json['expiresAt'] != null
          ? DateTime.parse(json['expiresAt'])
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userId': user.userId,
      'name': user.name,
      'email': user.email,
      'phone': user.phone,
      'roles': user.roles,
      'accessToken': accessToken,
      'refreshToken': refreshToken,
      'expiresAt': expiresAt?.toIso8601String(),
    };
  }

  factory AuthResponseModel.fromEntity(AuthResponse authResponse) {
    return AuthResponseModel(
      user: authResponse.user,
      accessToken: authResponse.accessToken,
      refreshToken: authResponse.refreshToken,
      expiresAt: authResponse.expiresAt,
    );
  }
}