import 'package:get_it/get_it.dart';
import 'package:dio/dio.dart';
import 'package:internet_connection_checker/internet_connection_checker.dart';
import 'package:shared_preferences/shared_preferences.dart';

// Core
import 'core/network/api_client.dart';
import 'core/network/network_info.dart';

// Services
import 'services/local_storage_service.dart';
import 'services/location_service.dart';
import 'services/notification_service.dart';
import 'services/analytics_service.dart';
import 'services/deep_link_service.dart';
import 'services/websocket_service.dart';

// Features - Auth
import 'features/auth/data/datasources/auth_local_datasource.dart';
import 'features/auth/data/datasources/auth_remote_datasource.dart';
import 'features/auth/data/repositories/auth_repository_impl.dart';
import 'features/auth/domain/repositories/auth_repository.dart';
import 'features/auth/domain/usecases/login_usecase.dart';
import 'features/auth/domain/usecases/register_usecase.dart';
import 'features/auth/domain/usecases/logout_usecase.dart';
import 'features/auth/domain/usecases/reset_password_usecase.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';

// Features - Settings
import 'features/settings/data/datasources/settings_local_datasource.dart';
import 'features/settings/data/repositories/settings_repository_impl.dart';
import 'features/settings/domain/repositories/settings_repository.dart';
import 'features/settings/domain/usecases/get_settings_usecase.dart';
import 'features/settings/domain/usecases/update_language_usecase.dart';
import 'features/settings/domain/usecases/update_theme_usecase.dart';
import 'features/settings/presentation/bloc/settings_bloc.dart';

// Features - Notifications
import 'features/notifications/presentation/bloc/notification_bloc.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // Features - Auth
  _initAuth();
  
  // Features - Settings
  _initSettings();
  
  // Features - Notifications
  _initNotifications();
  
  // Core
  _initCore();
  
  // External
  await _initExternal();
}

void _initAuth() {
  // Bloc
  sl.registerFactory(
    () => AuthBloc(
      loginUseCase: sl(),
      registerUseCase: sl(),
      logoutUseCase: sl(),
    ),
  );
  
  // Use cases
  sl.registerLazySingleton(() => LoginUseCase(sl()));
  sl.registerLazySingleton(() => RegisterUseCase(sl()));
  sl.registerLazySingleton(() => LogoutUseCase(sl()));
  
  // Repository
  sl.registerLazySingleton<AuthRepository>(
    () => AuthRepositoryImpl(
      remoteDataSource: sl(),
      localDataSource: sl(),
      internetConnectionChecker: sl(),
    ),
  );
  
  // Data sources
  sl.registerLazySingleton<AuthRemoteDataSource>(
    () => AuthRemoteDataSourceImpl(
      apiClient: sl(),
    ),
  );
  sl.registerLazySingleton<AuthLocalDataSource>(
    () => AuthLocalDataSourceImpl(
      sharedPreferences: sl(),
    ),
  );
}

void _initSettings() {
  // Bloc
  sl.registerFactory(
    () => SettingsBloc(),
  );
  
  // TODO: Implement settings use cases and repository later
}

void _initNotifications() {
  // Bloc
  sl.registerFactory(() => NotificationBloc());
}

void _initCore() {
  // Network - comment out for now since NetworkInfo is not defined
  // sl.registerLazySingleton<NetworkInfo>(() => NetworkInfoImpl(sl()));
  
  // API Client
  sl.registerLazySingleton<ApiClient>(() => ApiClient(sl()));
  
  // Services
  // LocalStorageService is static, no need to register
  sl.registerLazySingleton(() => LocationService());
  sl.registerLazySingleton(() => NotificationService());
  sl.registerLazySingleton(() => AnalyticsService());
  sl.registerLazySingleton(() => DeepLinkService());
  sl.registerLazySingleton(() => WebSocketService());
}

Future<void> _initExternal() async {
  // Shared Preferences
  final sharedPreferences = await SharedPreferences.getInstance();
  sl.registerLazySingleton(() => sharedPreferences);
  
  // Dio
  sl.registerLazySingleton(() => Dio());
  
  // Internet Connection Checker
  sl.registerLazySingleton(() => InternetConnectionChecker());
}