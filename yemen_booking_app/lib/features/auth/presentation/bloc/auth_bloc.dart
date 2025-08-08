import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/usecases/login_usecase.dart';
import '../../domain/usecases/register_usecase.dart';
import '../../domain/usecases/logout_usecase.dart';
import '../../domain/usecases/reset_password_usecase.dart';
import '../../domain/usecases/check_auth_status_usecase.dart';
import '../../domain/usecases/get_current_user_usecase.dart';
import 'auth_event.dart';
import 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final LoginUseCase loginUseCase;
  final RegisterUseCase registerUseCase;
  final LogoutUseCase logoutUseCase;
  final ResetPasswordUseCase resetPasswordUseCase;
  final CheckAuthStatusUseCase checkAuthStatusUseCase;
  final GetCurrentUserUseCase getCurrentUserUseCase;

  AuthBloc({
    required this.loginUseCase,
    required this.registerUseCase,
    required this.logoutUseCase,
    required this.resetPasswordUseCase,
    required this.checkAuthStatusUseCase,
    required this.getCurrentUserUseCase,
  }) : super(const AuthInitial()) {
    on<CheckAuthStatusEvent>(_onCheckAuthStatus);
    on<LoginEvent>(_onLogin);
    on<RegisterEvent>(_onRegister);
    on<LogoutEvent>(_onLogout);
    on<ResetPasswordEvent>(_onResetPassword);
  }

  Future<void> _onCheckAuthStatus(
    CheckAuthStatusEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final result = await checkAuthStatusUseCase(NoParams());
    
    await result.fold(
      (failure) async => emit(AuthError(message: _mapFailureToMessage(failure))),
      (isAuthenticated) async {
        if (isAuthenticated) {
          // Get current user details
          final userResult = await getCurrentUserUseCase(NoParams());
          await userResult.fold(
            (failure) async => emit(AuthError(message: _mapFailureToMessage(failure))),
            (user) async => emit(AuthAuthenticated(user: user)),
          );
        } else {
          emit(const AuthUnauthenticated());
        }
      },
    );
  }

  Future<void> _onLogin(
    LoginEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final params = LoginParams(
      emailOrPhone: event.emailOrPhone,
      password: event.password,
      rememberMe: event.rememberMe,
    );

    final result = await loginUseCase(params);
    
    await result.fold(
      (failure) async => emit(AuthError(message: _mapFailureToMessage(failure))),
      (authResponse) async {
        emit(AuthLoginSuccess(user: authResponse.user));
        // Also set the authenticated state
        emit(AuthAuthenticated(user: authResponse.user));
      },
    );
  }

  Future<void> _onRegister(
    RegisterEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final params = RegisterParams(
      name: event.name,
      email: event.email,
      phone: event.phone,
      password: event.password,
      passwordConfirmation: event.passwordConfirmation,
    );

    final result = await registerUseCase(params);
    
    await result.fold(
      (failure) async => emit(AuthError(message: _mapFailureToMessage(failure))),
      (authResponse) async {
        emit(AuthRegistrationSuccess(user: authResponse.user));
        // Also set the authenticated state
        emit(AuthAuthenticated(user: authResponse.user));
      },
    );
  }

  Future<void> _onLogout(
    LogoutEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final result = await logoutUseCase(NoParams());
    
    await result.fold(
      (failure) async => emit(AuthError(message: _mapFailureToMessage(failure))),
      (_) async {
        emit(const AuthLogoutSuccess());
        // Also set the unauthenticated state
        emit(const AuthUnauthenticated());
      },
    );
  }

  Future<void> _onResetPassword(
    ResetPasswordEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final params = ResetPasswordParams(
      emailOrPhone: event.emailOrPhone,
    );

    final result = await resetPasswordUseCase(params);
    
    await result.fold(
      (failure) async => emit(AuthError(message: _mapFailureToMessage(failure))),
      (_) async => emit(const AuthPasswordResetSent(
        message: 'Password reset instructions have been sent to your email/phone.',
      )),
    );
  }

  String _mapFailureToMessage(Failure failure) {
    switch (failure.runtimeType) {
      case ServerFailure:
        return 'Server error occurred. Please try again later.';
      case CacheFailure:
        return 'Local storage error occurred.';
      case NetworkFailure:
        return 'Please check your internet connection.';
      case ValidationFailure:
        return (failure as ValidationFailure).message;
      case AuthenticationFailure:
        return 'Invalid credentials. Please check your email/phone and password.';
      case UnauthorizedFailure:
        return 'You are not authorized to perform this action.';
      case SessionExpiredFailure:
        return 'Your session has expired. Please login again.';
      case PermissionDeniedFailure:
        return 'You do not have permission to perform this action.';
      default:
        return 'An unexpected error occurred. Please try again.';
    }
  }
}