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

class YemenBookingApp extends StatelessWidget {
  const YemenBookingApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => sl<SettingsBloc>()..add(const LoadSettingsEvent())),
        BlocProvider(create: (_) => sl<AuthBloc>()..add(const CheckAuthStatusEvent())),
        BlocProvider(create: (_) => sl<NotificationBloc>()),
      ],
      child: BlocBuilder<SettingsBloc, SettingsState>(
        builder: (context, settingsState) {
          return MaterialApp.router(
            title: 'Yemen Booking',
            debugShowCheckedModeBanner: false,
            theme: AppTheme.lightTheme,
            darkTheme: AppTheme.darkTheme,
            themeMode: settingsState is SettingsLoaded 
                ? settingsState.settings.themeMode 
                : ThemeMode.system,
            locale: settingsState is SettingsLoaded
                ? settingsState.settings.locale
                : const Locale('ar', 'YE'),
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