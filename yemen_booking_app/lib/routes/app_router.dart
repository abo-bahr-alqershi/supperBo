import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:yemen_booking_app/features/review/presentation/pages/reviews_list_page.dart';
import 'package:yemen_booking_app/features/review/presentation/pages/write_review_page.dart';
import 'package:yemen_booking_app/presentation/screens/main_screen.dart';
import 'package:yemen_booking_app/features/auth/presentation/pages/login_page.dart';
import 'package:yemen_booking_app/features/auth/presentation/pages/register_page.dart';
import 'package:yemen_booking_app/features/auth/presentation/pages/forgot_password_page.dart';
import 'package:yemen_booking_app/features/property/presentation/pages/property_details_page.dart';
import 'package:yemen_booking_app/features/booking/presentation/pages/booking_form_page.dart';
import 'package:yemen_booking_app/features/booking/presentation/pages/booking_summary_page.dart';
import 'package:yemen_booking_app/features/booking/presentation/pages/booking_payment_page.dart';
import 'package:yemen_booking_app/features/booking/presentation/pages/booking_confirmation_page.dart';
import 'package:yemen_booking_app/features/booking/presentation/pages/booking_details_page.dart';
import 'package:yemen_booking_app/features/search/presentation/pages/search_filters_page.dart';
import 'package:yemen_booking_app/features/search/presentation/pages/search_results_page.dart';
import 'package:yemen_booking_app/features/chat/presentation/pages/chat_page.dart';
import 'package:yemen_booking_app/features/notifications/presentation/pages/notifications_page.dart';
import 'package:yemen_booking_app/features/favorites/presentation/pages/favorites_page.dart';
import 'package:yemen_booking_app/features/settings/presentation/pages/settings_page.dart';
import 'package:yemen_booking_app/features/payment/presentation/pages/payment_methods_page.dart';
import 'package:yemen_booking_app/features/payment/presentation/pages/add_payment_method_page.dart';
import 'package:yemen_booking_app/features/payment/presentation/pages/payment_history_page.dart';

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