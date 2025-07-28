import 'package:dartz/dartz.dart';
import 'package:internet_connection_checker/internet_connection_checker.dart';
import '../../../../core/error/error_handler.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/auth_response.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_local_datasource.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_response_model.dart';
import '../models/user_model.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource remoteDataSource;
  final AuthLocalDataSource localDataSource;
  final InternetConnectionChecker internetConnectionChecker;

  AuthRepositoryImpl({
    required this.remoteDataSource,
    required this.localDataSource,
    required this.internetConnectionChecker,
  });

  @override
  Future<Either<Failure, AuthResponse>> login({
    required String emailOrPhone,
    required String password,
    required bool rememberMe,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final authResponse = await remoteDataSource.login(
          emailOrPhone: emailOrPhone,
          password: password,
          rememberMe: rememberMe,
        );
        
        if (rememberMe) {
          await localDataSource.cacheAuthResponse(authResponse);
        } else {
          await localDataSource.cacheAccessToken(authResponse.accessToken);
          await localDataSource.cacheRefreshToken(authResponse.refreshToken);
          await localDataSource.cacheUser(authResponse.user as UserModel);
        }
        
        return Right(authResponse);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, AuthResponse>> register({
    required String name,
    required String email,
    required String phone,
    required String password,
    required String passwordConfirmation,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final authResponse = await remoteDataSource.register(
          name: name,
          email: email,
          phone: phone,
          password: password,
          passwordConfirmation: passwordConfirmation,
        );
        
        await localDataSource.cacheAuthResponse(authResponse);
        
        return Right(authResponse);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, void>> logout() async {
    try {
      if (await internetConnectionChecker.hasConnection) {
        await remoteDataSource.logout();
      }
      await localDataSource.clearAuthData();
      return const Right(null);
    } catch (e) {
      // Clear local data even if remote logout fails
      await localDataSource.clearAuthData();
      return const Right(null);
    }
  }

  @override
  Future<Either<Failure, void>> resetPassword({
    required String emailOrPhone,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        await remoteDataSource.resetPassword(emailOrPhone: emailOrPhone);
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, AuthResponse>> refreshToken({
    required String refreshToken,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final authResponse = await remoteDataSource.refreshToken(
          refreshToken: refreshToken,
        );
        
        await localDataSource.cacheAuthResponse(authResponse);
        
        return Right(authResponse);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, User>> getCurrentUser() async {
    try {
      // Try to get cached user first
      final cachedUser = await localDataSource.getCachedUser();
      if (cachedUser != null) {
        // If online, fetch fresh data
        if (await internetConnectionChecker.hasConnection) {
          try {
            final user = await remoteDataSource.getCurrentUser();
            await localDataSource.cacheUser(user);
            return Right(user);
          } catch (e) {
            // Return cached user if remote fails
            return Right(cachedUser);
          }
        }
        return Right(cachedUser);
      }
      
      // No cached user, must fetch from remote
      if (await internetConnectionChecker.hasConnection) {
        final user = await remoteDataSource.getCurrentUser();
        await localDataSource.cacheUser(user);
        return Right(user);
      } else {
        return const Left(NetworkFailure());
      }
    } catch (e) {
      return ErrorHandler.handle(e);
    }
  }

  @override
  Future<Either<Failure, void>> updateProfile({
    required String name,
    String? email,
    String? phone,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        await remoteDataSource.updateProfile(
          name: name,
          email: email,
          phone: phone,
        );
        
        // Update cached user
        final cachedUser = await localDataSource.getCachedUser();
        if (cachedUser != null) {
          final updatedUser = UserModel(
            userId: cachedUser.userId,
            name: name,
            email: email ?? cachedUser.email,
            phone: phone ?? cachedUser.phone,
            roles: cachedUser.roles,
            profileImage: cachedUser.profileImage,
            emailVerifiedAt: cachedUser.emailVerifiedAt,
            phoneVerifiedAt: cachedUser.phoneVerifiedAt,
            createdAt: cachedUser.createdAt,
            updatedAt: DateTime.now(),
          );
          await localDataSource.cacheUser(updatedUser);
        }
        
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, void>> changePassword({
    required String currentPassword,
    required String newPassword,
    required String newPasswordConfirmation,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        await remoteDataSource.changePassword(
          currentPassword: currentPassword,
          newPassword: newPassword,
          newPasswordConfirmation: newPasswordConfirmation,
        );
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

    @override
  Future<Either<Failure, bool>> checkAuthStatus() async {
    try {
      final isLoggedIn = await localDataSource.isLoggedIn();
      if (!isLoggedIn) {
        return const Right(false);
      }

      // Check if we have cached auth data
      final cachedAuth = await localDataSource.getCachedAuthResponse();
      if (cachedAuth == null) {
        return const Right(false);
      }

      // If online, validate token with server
      if (await internetConnectionChecker.hasConnection) {
        try {
          await remoteDataSource.getCurrentUser();
          return const Right(true);
        } catch (e) {
          // Token might be expired, try to refresh
          final refreshToken = await localDataSource.getCachedRefreshToken();
          if (refreshToken != null) {
            try {
              final newAuth = await remoteDataSource.refreshToken(
                refreshToken: refreshToken,
              );
              await localDataSource.cacheAuthResponse(newAuth);
              return const Right(true);
            } catch (e) {
              // Refresh failed, user needs to login again
              await localDataSource.clearAuthData();
              return const Right(false);
            }
          }
          return const Right(false);
        }
      }
      
      // Offline, return true if we have cached data
      return const Right(true);
    } catch (e) {
      return const Right(false);
    }
  }

  @override
  Future<void> saveAuthData(AuthResponse authResponse) async {
    await localDataSource.cacheAuthResponse(
      AuthResponseModel.fromEntity(authResponse),
    );
  }

  @override
  Future<void> clearAuthData() async {
    await localDataSource.clearAuthData();
  }

  @override
  Future<String?> getAccessToken() async {
    return await localDataSource.getCachedAccessToken();
  }

  @override
  Future<String?> getRefreshToken() async {
    return await localDataSource.getCachedRefreshToken();
  }
}