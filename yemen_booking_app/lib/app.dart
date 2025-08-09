import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'core/theme/app_theme.dart';
import 'core/localization/app_localizations.dart';
import 'core/localization/locale_manager.dart';
import 'routes/app_router.dart';
import 'injection_container.dart';
import 'features/settings/presentation/bloc/settings_bloc.dart';
import 'features/settings/presentation/bloc/settings_event.dart';
import 'features/settings/presentation/bloc/settings_state.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';
import 'features/auth/presentation/bloc/auth_event.dart';
import 'features/notifications/presentation/bloc/notification_bloc.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'features/payment/presentation/bloc/payment_bloc.dart';

class YemenBookingApp extends StatelessWidget {
  const YemenBookingApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        
        BlocProvider(create: (_) => sl<SettingsBloc>()..add(LoadSettingsEvent())),
        BlocProvider(create: (_) => sl<AuthBloc>()..add(const CheckAuthStatusEvent())),
        BlocProvider(create: (_) => sl<NotificationBloc>()),
        BlocProvider(create: (_) => sl<PaymentBloc>()),
      ],
      child: BlocBuilder<SettingsBloc, SettingsState>(
        builder: (context, settingsState) {
          final mode = _themeModeFrom(settingsState);
          final locale = _localeFrom(settingsState);
          return MaterialApp.router(
            title: 'Yemen Booking',
            debugShowCheckedModeBanner: false,
            theme: AppTheme.lightTheme,
            darkTheme: AppTheme.darkTheme,
            themeMode: mode,
            locale: locale,
            localizationsDelegates: const [
              AppLocalizations.delegate,
              GlobalMaterialLocalizations.delegate,
              GlobalWidgetsLocalizations.delegate,
              GlobalCupertinoLocalizations.delegate,
            ],
            supportedLocales: LocaleManager.supportedLocales,
            routerConfig: AppRouter.router,
          );
        },
      ),
    );
  }
}

ThemeMode _themeModeFrom(SettingsState state) {
  if (state is SettingsLoaded) {
    return state.settings.darkMode ? ThemeMode.dark : ThemeMode.light;
  }
  return ThemeMode.system;
}

Locale _localeFrom(SettingsState state) {
  if (state is SettingsLoaded) {
    final code = state.settings.preferredLanguage;
    return Locale(code);
  }
  return const Locale('ar', 'YE');
}


