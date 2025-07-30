import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';

part 'settings_event.dart';
part 'settings_state.dart';

class SettingsBloc extends Bloc<SettingsEvent, SettingsState> {
  SettingsBloc() : super(SettingsInitial()) {
    on<LoadSettingsEvent>(_onLoadSettings);
    on<ChangeThemeEvent>(_onChangeTheme);
    on<ChangeLanguageEvent>(_onChangeLanguage);
  }

  void _onLoadSettings(LoadSettingsEvent event, Emitter<SettingsState> emit) {
    // Load default settings
    emit(SettingsLoaded(
      themeMode: ThemeMode.system,
      locale: const Locale('ar'),
    ));
  }

  void _onChangeTheme(ChangeThemeEvent event, Emitter<SettingsState> emit) {
    if (state is SettingsLoaded) {
      final currentState = state as SettingsLoaded;
      emit(currentState.copyWith(themeMode: event.themeMode));
    }
  }

  void _onChangeLanguage(ChangeLanguageEvent event, Emitter<SettingsState> emit) {
    if (state is SettingsLoaded) {
      final currentState = state as SettingsLoaded;
      emit(currentState.copyWith(locale: event.locale));
    }
  }
}