part of 'settings_bloc.dart';

abstract class SettingsState extends Equatable {
  const SettingsState();

  @override
  List<Object> get props => [];
}

class SettingsInitial extends SettingsState {}

class SettingsLoading extends SettingsState {}

class SettingsLoaded extends SettingsState {
  final ThemeMode themeMode;
  final Locale locale;

  const SettingsLoaded({
    required this.themeMode,
    required this.locale,
  });

  @override
  List<Object> get props => [themeMode, locale];

  SettingsLoaded copyWith({
    ThemeMode? themeMode,
    Locale? locale,
  }) {
    return SettingsLoaded(
      themeMode: themeMode ?? this.themeMode,
      locale: locale ?? this.locale,
    );
  }
}

class SettingsError extends SettingsState {
  final String message;

  const SettingsError(this.message);

  @override
  List<Object> get props => [message];
}