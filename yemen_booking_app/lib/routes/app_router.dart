import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:yemen_booking_app/features/review/presentation/pages/reviews_list_page.dart';
import 'package:yemen_booking_app/features/review/presentation/pages/write_review_page.dart';
import 'package:yemen_booking_app/presentation/screens/main_screen.dart';
import 'package:yemen_booking_app/presentation/screens/splash_screen.dart';

class AppRouter {
  static GoRouter get router => _router;
  
  static final GoRouter _router = GoRouter(
    initialLocation: '/',
    routes: <RouteBase>[
      GoRoute(
        path: '/',
        builder: (BuildContext context, GoRouterState state) {
          return const SplashScreen();
        },
      ),
      GoRoute(
        path: '/main',
        builder: (BuildContext context, GoRouterState state) {
          return const MainScreen();
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
      // Review Routes
      GoRoute(
        path: '/reviews/:propertyId',
        builder: (context, state) {
          final propertyId = state.pathParameters['propertyId']!;
          final propertyName = state.extra as String? ?? '';
          return ReviewsListPage(
            propertyId: propertyId,
            propertyName: propertyName,
          );
        },
      ),
      GoRoute(
        path: '/review/write',
        builder: (context, state) {
          final extras = state.extra as Map<String, dynamic>;
          return WriteReviewPage(
            bookingId: extras['bookingId'] as String,
            propertyId: extras['propertyId'] as String,
            propertyName: extras['propertyName'] as String,
          );
        },
      ),
    ],
  );
}