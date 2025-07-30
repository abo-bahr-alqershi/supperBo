import 'package:equatable/equatable.dart';
import '../../domain/entities/user.dart';

abstract class AuthState extends Equatable {
  const AuthState();

  @override
  List<Object> get props => [];
}

/// Initial state when the auth bloc is first created
class AuthInitial extends AuthState {
  const AuthInitial();
}

/// State when any authentication operation is in progress
class AuthLoading extends AuthState {
  const AuthLoading();
}

/// State when user is successfully authenticated
class AuthAuthenticated extends AuthState {
  final User user;

  const AuthAuthenticated({required this.user});

  @override
  List<Object> get props => [user];
}

/// State when user is not authenticated
class AuthUnauthenticated extends AuthState {
  const AuthUnauthenticated();
}

/// State when an authentication error occurs
class AuthError extends AuthState {
  final String message;

  const AuthError({required this.message});

  @override
  List<Object> get props => [message];
}

/// State when login is successful
class AuthLoginSuccess extends AuthState {
  final User user;

  const AuthLoginSuccess({required this.user});

  @override
  List<Object> get props => [user];
}

/// State when registration is successful
class AuthRegistrationSuccess extends AuthState {
  final User user;

  const AuthRegistrationSuccess({required this.user});

  @override
  List<Object> get props => [user];
}

/// State when logout is successful
class AuthLogoutSuccess extends AuthState {
  const AuthLogoutSuccess();
}

/// State when password reset email/SMS is sent successfully
class AuthPasswordResetSent extends AuthState {
  final String message;

  const AuthPasswordResetSent({required this.message});

  @override
  List<Object> get props => [message];
}

/// State when profile is updated successfully
class AuthProfileUpdateSuccess extends AuthState {
  final User user;

  const AuthProfileUpdateSuccess({required this.user});

  @override
  List<Object> get props => [user];
}

/// State when password is changed successfully
class AuthPasswordChangeSuccess extends AuthState {
  const AuthPasswordChangeSuccess();
}