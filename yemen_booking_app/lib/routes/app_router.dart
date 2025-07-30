import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class AppRouter {
  static GoRouter get router => _router;
  
  static final GoRouter _router = GoRouter(
    initialLocation: '/',
    routes: <RouteBase>[
      GoRoute(
        path: '/',
        builder: (BuildContext context, GoRouterState state) {
          return const Scaffold(
            body: Center(
              child: Text(
                'مرحباً بك في تطبيق حجوزات اليمن',
                style: TextStyle(fontSize: 24),
                textAlign: TextAlign.center,
              ),
            ),
          );
        },
      ),
      GoRoute(
        path: '/home',
        builder: (BuildContext context, GoRouterState state) {
          return const Scaffold(
            body: Center(
              child: Text('الصفحة الرئيسية'),
            ),
          );
        },
      ),
      GoRoute(
        path: '/login',
        builder: (BuildContext context, GoRouterState state) {
          return const Scaffold(
            body: Center(
              child: Text('صفحة تسجيل الدخول'),
            ),
          );
        },
      ),
      GoRoute(
        path: '/profile',
        builder: (BuildContext context, GoRouterState state) {
          return const Scaffold(
            body: Center(
              child: Text('الملف الشخصي'),
            ),
          );
        },
      ),
    ],
  );
}