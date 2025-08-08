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
import 'features/auth/domain/usecases/check_auth_status_usecase.dart';
import 'features/auth/domain/usecases/get_current_user_usecase.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';

// Features - Settings
import 'features/settings/data/datasources/settings_local_datasource.dart';
import 'features/settings/data/repositories/settings_repository_impl.dart';
import 'features/settings/domain/repositories/settings_repository.dart';
import 'features/settings/domain/usecases/get_settings_usecase.dart';
import 'features/settings/domain/usecases/update_language_usecase.dart';
import 'features/settings/domain/usecases/update_theme_usecase.dart';
import 'features/settings/domain/usecases/update_notification_settings_usecase.dart' as settings_notification;
import 'features/settings/presentation/bloc/settings_bloc.dart';

// Features - Notifications
import 'features/notifications/data/datasources/notification_local_datasource.dart';
import 'features/notifications/data/datasources/notification_remote_datasource.dart';
import 'features/notifications/data/repositories/notification_repository_impl.dart';
import 'features/notifications/domain/repositories/notification_repository.dart';
import 'features/notifications/domain/usecases/get_notifications_usecase.dart';
import 'features/notifications/domain/usecases/mark_as_read_usecase.dart';
import 'features/notifications/domain/usecases/dismiss_notification_usecase.dart';
import 'features/notifications/domain/usecases/update_notification_settings_usecase.dart';
import 'features/notifications/presentation/bloc/notification_bloc.dart';

// Features - Home
import 'features/home/data/datasources/home_local_datasource.dart';
import 'features/home/data/datasources/home_remote_datasource.dart';
import 'features/home/data/repositories/home_repository_impl.dart';
import 'features/home/domain/repositories/home_repository.dart';
import 'features/home/domain/usecases/get_home_config_usecase.dart';
import 'features/home/domain/usecases/get_home_sections_usecase.dart';
import 'features/home/domain/usecases/get_sponsored_ads_usecase.dart';
import 'features/home/domain/usecases/get_city_destinations_usecase.dart';
import 'features/home/domain/usecases/record_ad_impression_usecase.dart';
import 'features/home/domain/usecases/record_ad_click_usecase.dart';
import 'features/home/presentation/bloc/home_bloc.dart';

// Features - Review
import 'features/review/data/datasources/review_remote_datasource.dart';
import 'features/review/data/repositories/review_repository_impl.dart';
import 'features/review/domain/repositories/review_repository.dart';
import 'features/review/domain/usecases/create_review_usecase.dart';
import 'features/review/domain/usecases/get_property_reviews_usecase.dart';
import 'features/review/domain/usecases/get_property_reviews_Summary_usecase.dart';
import 'features/review/domain/usecases/upload_review_images_usecase.dart';
import 'features/review/presentation/bloc/review_bloc.dart';

// Features - Booking (imports added)
import 'features/booking/presentation/bloc/booking_bloc.dart';
import 'features/booking/domain/usecases/create_booking_usecase.dart';
import 'features/booking/domain/usecases/get_booking_details_usecase.dart';
import 'features/booking/domain/usecases/cancel_booking_usecase.dart';
import 'features/booking/domain/usecases/get_user_bookings_usecase.dart';
import 'features/booking/domain/usecases/get_user_bookings_summary_usecase.dart';
import 'features/booking/domain/usecases/add_services_to_booking_usecase.dart';
import 'features/booking/domain/usecases/check_availability_usecase.dart';
import 'features/booking/domain/repositories/booking_repository.dart';
import 'features/booking/data/repositories/booking_repository_impl.dart';
import 'features/booking/data/datasources/booking_remote_datasource.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // Features - Auth
  _initAuth();
  
  // Features - Settings
  _initSettings();
  
  // Features - Notifications
  _initNotifications();

  // Features - Home
  _initHome();
  
  // Features - Review
  _initReview();

  // Features - Booking
  _initBooking();

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
      resetPasswordUseCase: sl(),
      checkAuthStatusUseCase: sl(),
      getCurrentUserUseCase: sl(),
    ),
  );
  
  // Use cases
  sl.registerLazySingleton(() => LoginUseCase(sl()));
  sl.registerLazySingleton(() => RegisterUseCase(sl()));
  sl.registerLazySingleton(() => LogoutUseCase(sl()));
  sl.registerLazySingleton(() => ResetPasswordUseCase(sl()));
  sl.registerLazySingleton(() => CheckAuthStatusUseCase(sl()));
  sl.registerLazySingleton(() => GetCurrentUserUseCase(sl()));
  
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
    () => AuthRemoteDataSourceImpl(apiClient: sl()),
  );
  sl.registerLazySingleton<AuthLocalDataSource>(
    () => AuthLocalDataSourceImpl(sharedPreferences: sl()),
  );
}

void _initSettings() {
  // Bloc
  sl.registerFactory(
    () => SettingsBloc(
      getSettingsUseCase: sl(),
      updateLanguageUseCase: sl(),
      updateThemeUseCase: sl(),
      updateNotificationSettingsUseCase: sl(), 
      localDataSource: AuthLocalDataSourceImpl(sharedPreferences: sl()),
    ),
  );
  
  // Use cases
  sl.registerLazySingleton(() => GetSettingsUseCase(sl()));
  sl.registerLazySingleton(() => UpdateLanguageUseCase(sl()));
  sl.registerLazySingleton(() => UpdateThemeUseCase(sl()));
  sl.registerLazySingleton(() => settings_notification.UpdateNotificationSettingsUseCase(sl()));
  
  // Repository
  sl.registerLazySingleton<SettingsRepository>(
    () => SettingsRepositoryImpl(localDataSource: sl()),
  );
  
  // Data sources
  sl.registerLazySingleton<SettingsLocalDataSource>(
    () => SettingsLocalDataSourceImpl(localStorage: sl()),
  );
}

void _initNotifications() {
  // Bloc
  sl.registerFactory(
    () => NotificationBloc(
      getNotificationsUseCase: sl(),
      markAsReadUseCase: sl(),
      dismissNotificationUseCase: sl(),
      updateNotificationSettingsUseCase: sl(),
    ),
  );
  
  // Use cases
  sl.registerLazySingleton(() => GetNotificationsUseCase(sl()));
  sl.registerLazySingleton(() => MarkAsReadUseCase(sl()));
  sl.registerLazySingleton(() => DismissNotificationUseCase(sl()));
  sl.registerLazySingleton(() => UpdateNotificationSettingsUseCase(sl()));
  
  // Repository
  sl.registerLazySingleton<NotificationRepository>(
    () => NotificationRepositoryImpl(
      remoteDataSource: sl(),
      localDataSource: sl(),
      networkInfo: sl(),
    ),
  );
  
  // Data sources
  sl.registerLazySingleton<NotificationRemoteDataSource>(
    () => NotificationRemoteDataSourceImpl(apiClient: sl()),
  );
  sl.registerLazySingleton<NotificationLocalDataSource>(
    () => NotificationLocalDataSourceImpl(localStorage: sl()),
  );
}

void _initHome() {
  // Bloc
  sl.registerFactory(
    () => HomeBloc(
      getHomeConfigUseCase: sl(),
      getHomeSectionsUseCase: sl(),
      getSponsoredAdsUseCase: sl(),
      getCityDestinationsUseCase: sl(),
      recordAdImpressionUseCase: sl(),
      recordAdClickUseCase: sl(),
    ),
  );

  // Use cases
  sl.registerLazySingleton(() => GetHomeConfigUseCase(sl()));
  sl.registerLazySingleton(() => GetHomeSectionsUseCase(sl()));
  sl.registerLazySingleton(() => GetSponsoredAdsUseCase(sl()));
  sl.registerLazySingleton(() => GetCityDestinationsUseCase(sl()));
  sl.registerLazySingleton(() => RecordAdImpressionUseCase(sl()));
  sl.registerLazySingleton(() => RecordAdClickUseCase(sl()));

  // Repository
  sl.registerLazySingleton<HomeRepository>(
    () => HomeRepositoryImpl(
      remoteDataSource: sl(),
      localDataSource: sl(),
      networkInfo: sl(),
    ),
  );

  // Data sources
  sl.registerLazySingleton<HomeRemoteDataSource>(
    () => HomeRemoteDataSourceImpl(apiClient: sl()),
  );
  sl.registerLazySingleton<HomeLocalDataSource>(
    () => HomeLocalDataSourceImpl(sharedPreferences: sl()),
  );
}

void _initReview() {
  // Bloc
  sl.registerFactory(
    () => ReviewBloc(
      createReviewUseCase: sl(),
      getPropertyReviewsUseCase: sl(),
      getPropertyReviewsSummaryUseCase: sl(),
      uploadReviewImagesUseCase: sl(),
    ),
  );

  // Use cases
  sl.registerLazySingleton(() => CreateReviewUseCase(sl()));
  sl.registerLazySingleton(() => GetPropertyReviewsUseCase(sl()));
  sl.registerLazySingleton(() => GetPropertyReviewsSummaryUseCase(sl()));
  sl.registerLazySingleton(() => UploadReviewImagesUseCase(sl()));

  // Repository
  sl.registerLazySingleton<ReviewRepository>(
    () => ReviewRepositoryImpl(
      remoteDataSource: sl(),
      networkInfo: sl(),
    ),
  );

  // Data sources
  sl.registerLazySingleton<ReviewRemoteDataSource>(
    () => ReviewRemoteDataSourceImpl(apiClient: sl()),
  );
}

void _initBooking() {
  // Bloc
  sl.registerFactory(
    () => BookingBloc(
      createBookingUseCase: sl(),
      getBookingDetailsUseCase: sl(),
      cancelBookingUseCase: sl(),
      getUserBookingsUseCase: sl(),
      getUserBookingsSummaryUseCase: sl(),
      addServicesToBookingUseCase: sl(),
      checkAvailabilityUseCase: sl(),
    ),
  );

  // Use cases
  sl.registerLazySingleton(() => CreateBookingUseCase(sl()));
  sl.registerLazySingleton(() => GetBookingDetailsUseCase(sl()));
  sl.registerLazySingleton(() => CancelBookingUseCase(sl()));
  sl.registerLazySingleton(() => GetUserBookingsUseCase(sl()));
  sl.registerLazySingleton(() => GetUserBookingsSummaryUseCase(sl()));
  sl.registerLazySingleton(() => AddServicesToBookingUseCase(sl()));
  sl.registerLazySingleton(() => CheckAvailabilityUseCase(sl()));

  // Repository
  sl.registerLazySingleton<BookingRepository>(
    () => BookingRepositoryImpl(
      remoteDataSource: sl(),
      internetConnectionChecker: sl(),
    ),
  );

  // Data sources
  sl.registerLazySingleton<BookingRemoteDataSource>(
    () => BookingRemoteDataSourceImpl(apiClient: sl()),
  );
}

void _initCore() {
  // Network
  sl.registerLazySingleton<NetworkInfo>(() => NetworkInfoImpl(sl()));
  
  // API Client
  sl.registerLazySingleton<ApiClient>(() => ApiClient(sl()));
  
  // Services
  sl.registerLazySingleton(() => LocalStorageService(sl()));
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