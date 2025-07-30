import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/app_settings.dart';
import '../../domain/usecases/get_settings_usecase.dart';
import '../../domain/usecases/update_language_usecase.dart';
import '../../domain/usecases/update_theme_usecase.dart';
import '../../domain/usecases/update_notification_settings_usecase.dart';
import 'settings_event.dart';
import 'settings_state.dart';

class SettingsBloc extends Bloc<SettingsEvent, SettingsState> {
  final GetSettingsUseCase getSettingsUseCase;
  final UpdateLanguageUseCase updateLanguageUseCase;
  final UpdateThemeUseCase updateThemeUseCase;
  final UpdateNotificationSettingsUseCase updateNotificationSettingsUseCase;

  SettingsBloc({
    required this.getSettingsUseCase,
    required this.updateLanguageUseCase,
    required this.updateThemeUseCase,
    required this.updateNotificationSettingsUseCase,
  }) : super(const SettingsInitial()) {
    on<LoadSettingsEvent>(_onLoadSettings);
    on<UpdateLanguageEvent>(_onUpdateLanguage);
    on<UpdateThemeEvent>(_onUpdateTheme);
    on<UpdateNotificationSettingsEvent>(_onUpdateNotificationSettings);
    on<UpdateBiometricAuthEvent>(_onUpdateBiometricAuth);
    on<UpdateAutoLoginEvent>(_onUpdateAutoLogin);
    on<UpdateCurrencyEvent>(_onUpdateCurrency);
    on<UpdateOnboardingVisibilityEvent>(_onUpdateOnboardingVisibility);
    on<ResetSettingsEvent>(_onResetSettings);
    on<RefreshSettingsEvent>(_onRefreshSettings);
  }

  Future<void> _onLoadSettings(
    LoadSettingsEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    final result = await getSettingsUseCase(NoParams());

    result.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (settings) => emit(SettingsLoaded(settings: settings)),
    );
  }

  Future<void> _onUpdateLanguage(
    UpdateLanguageEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    final result = await updateLanguageUseCase(
      UpdateLanguageParams(language: event.language),
    );

    result.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (settings) => emit(LanguageUpdateSuccess(settings: settings)),
    );
  }

  Future<void> _onUpdateTheme(
    UpdateThemeEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    final result = await updateThemeUseCase(
      UpdateThemeParams(themeMode: event.themeMode),
    );

    result.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (settings) => emit(ThemeUpdateSuccess(settings: settings)),
    );
  }

  Future<void> _onUpdateNotificationSettings(
    UpdateNotificationSettingsEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    final result = await updateNotificationSettingsUseCase(
      UpdateNotificationSettingsParams(
        notificationSettings: event.notificationSettings,
      ),
    );

    result.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (settings) => emit(NotificationSettingsUpdateSuccess(settings: settings)),
    );
  }

  Future<void> _onUpdateBiometricAuth(
    UpdateBiometricAuthEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    // For now, we'll get current settings and update with new biometric setting
    // This would typically call a specific use case
    final currentResult = await getSettingsUseCase(NoParams());
    
    currentResult.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (currentSettings) {
        final updatedSettings = currentSettings.copyWith(
          biometricAuth: event.enabled,
          lastUpdated: DateTime.now(),
        );
        emit(BiometricAuthUpdateSuccess(settings: updatedSettings));
      },
    );
  }

  Future<void> _onUpdateAutoLogin(
    UpdateAutoLoginEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    // For now, we'll get current settings and update with new auto login setting
    // This would typically call a specific use case
    final currentResult = await getSettingsUseCase(NoParams());
    
    currentResult.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (currentSettings) {
        final updatedSettings = currentSettings.copyWith(
          autoLogin: event.enabled,
          lastUpdated: DateTime.now(),
        );
        emit(AutoLoginUpdateSuccess(settings: updatedSettings));
      },
    );
  }

  Future<void> _onUpdateCurrency(
    UpdateCurrencyEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    // For now, we'll get current settings and update with new currency
    // This would typically call a specific use case
    final currentResult = await getSettingsUseCase(NoParams());
    
    currentResult.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (currentSettings) {
        final updatedSettings = currentSettings.copyWith(
          currency: event.currency,
          lastUpdated: DateTime.now(),
        );
        emit(CurrencyUpdateSuccess(settings: updatedSettings));
      },
    );
  }

  Future<void> _onUpdateOnboardingVisibility(
    UpdateOnboardingVisibilityEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    // For now, we'll get current settings and update with new onboarding visibility
    // This would typically call a specific use case
    final currentResult = await getSettingsUseCase(NoParams());
    
    currentResult.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (currentSettings) {
        final updatedSettings = currentSettings.copyWith(
          showOnboarding: event.showOnboarding,
          lastUpdated: DateTime.now(),
        );
        emit(OnboardingVisibilityUpdateSuccess(settings: updatedSettings));
      },
    );
  }

  Future<void> _onResetSettings(
    ResetSettingsEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    // For now, we'll create default settings
    // This would typically call a specific use case that resets settings in repository
    try {
      final defaultSettings = AppSettings(
        lastUpdated: DateTime.now(),
      );
      emit(SettingsResetSuccess(settings: defaultSettings));
    } catch (e) {
      emit(SettingsError(message: 'فشل في إعادة تعيين الإعدادات: ${e.toString()}'));
    }
  }

  Future<void> _onRefreshSettings(
    RefreshSettingsEvent event,
    Emitter<SettingsState> emit,
  ) async {
    emit(const SettingsLoading());

    final result = await getSettingsUseCase(NoParams());

    result.fold(
      (failure) => emit(SettingsError(message: failure.message)),
      (settings) => emit(SettingsRefreshSuccess(settings: settings)),
    );
  }
}