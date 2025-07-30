import 'package:equatable/equatable.dart';

abstract class AuthEvent extends Equatable {
  const AuthEvent();

  @override
  List<Object> get props => [];
}

/// Event to check the current authentication status
class CheckAuthStatusEvent extends AuthEvent {
  const CheckAuthStatusEvent();
}

/// Event to login with email/phone and password
class LoginEvent extends AuthEvent {
  final String emailOrPhone;
  final String password;
  final bool rememberMe;

  const LoginEvent({
    required this.emailOrPhone,
    required this.password,
    this.rememberMe = false,
  });

  @override
  List<Object> get props => [emailOrPhone, password, rememberMe];
}

/// Event to register a new user
class RegisterEvent extends AuthEvent {
  final String name;
  final String email;
  final String phone;
  final String password;
  final String passwordConfirmation;

  const RegisterEvent({
    required this.name,
    required this.email,
    required this.phone,
    required this.password,
    required this.passwordConfirmation,
  });

  @override
  List<Object> get props => [name, email, phone, password, passwordConfirmation];
}

/// Event to logout the current user
class LogoutEvent extends AuthEvent {
  const LogoutEvent();
}

/// Event to reset password
class ResetPasswordEvent extends AuthEvent {
  final String emailOrPhone;

  const ResetPasswordEvent({
    required this.emailOrPhone,
  });

  @override
  List<Object> get props => [emailOrPhone];
}

/// Event to refresh authentication token
class RefreshTokenEvent extends AuthEvent {
  const RefreshTokenEvent();
}

/// Event to update user profile
class UpdateProfileEvent extends AuthEvent {
  final String name;
  final String? email;
  final String? phone;

  const UpdateProfileEvent({
    required this.name,
    this.email,
    this.phone,
  });

  @override
  List<Object> get props => [name, email ?? '', phone ?? ''];
}

/// Event to change password
class ChangePasswordEvent extends AuthEvent {
  final String currentPassword;
  final String newPassword;
  final String newPasswordConfirmation;

  const ChangePasswordEvent({
    required this.currentPassword,
    required this.newPassword,
    required this.newPasswordConfirmation,
  });

  @override
  List<Object> get props => [currentPassword, newPassword, newPasswordConfirmation];
}