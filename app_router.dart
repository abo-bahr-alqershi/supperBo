import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../features/home/presentation/pages/home_page.dart';
import '../features/auth/presentation/pages/login_page.dart';
import '../features/auth/presentation/pages/register_page.dart';
import '../features/auth/presentation/pages/forgot_password_page.dart';
import '../features/auth/presentation/pages/profile_page.dart';
import '../features/search/presentation/pages/search_page.dart';
import '../features/search/presentation/pages/search_results_page.dart';
import '../features/search/presentation/pages/search_filters_page.dart';
import '../features/property/presentation/pages/property_details_page.dart';
import '../features/property/presentation/pages/property_gallery_page.dart';
import '../features/property/presentation/pages/property_map_page.dart';
import '../features/property/presentation/pages/property_reviews_page.dart';
import '../features/booking/presentation/pages/booking_form_page.dart';
import '../features/booking/presentation/pages/booking_summary_page.dart';
import '../features/booking/presentation/pages/booking_confirmation_page.dart';
import '../features/booking/presentation/pages/booking_details_page.dart';
import '../features/booking/presentation/pages/booking_payment_page.dart';
import '../features/booking/presentation/pages/my_bookings_page.dart';
import '../features/favorites/presentation/pages/favorites_page.dart';
import '../features/notifications/presentation/pages/notifications_page.dart';
import '../features/notifications/presentation/pages/notification_settings_page.dart';
import '../features/settings/presentation/pages/settings_page.dart';
import '../features/settings/presentation/pages/language_settings_page.dart';
import '../features/settings/presentation/pages/about_page.dart';
import '../features/settings/presentation/pages/privacy_policy_page.dart';
import '../features/chat/presentation/pages/conversations_page.dart';
import '../features/chat/presentation/pages/chat_page.dart';
import '../features/chat/presentation/pages/chat_settings_page.dart';
import '../features/payment/presentation/pages/payment_methods_page.dart';
import '../features/payment/presentation/pages/add_payment_method_page.dart';
import '../features/payment/presentation/pages/payment_history_page.dart';
import '../features/review/presentation/pages/reviews_list_page.dart';
import '../features/review/presentation/pages/write_review_page.dart';

class AppRouter {
  static final GoRouter router = GoRouter(
    initialLocation: '/',
    routes: [
      // Home Routes
      GoRoute(
        path: '/',
        name: 'home',
        builder: (context, state) => const HomePage(),
      ),

      // Auth Routes
      GoRoute(
        path: '/login',
        name: 'login',
        builder: (context, state) => const LoginPage(),
      ),
      GoRoute(
        path: '/register',
        name: 'register',
        builder: (context, state) => const RegisterPage(),
      ),
      GoRoute(
        path: '/forgot-password',
        name: 'forgot-password',
        builder: (context, state) => const ForgotPasswordPage(),
      ),
      GoRoute(
        path: '/profile',
        name: 'profile',
        builder: (context, state) => const ProfilePage(),
      ),

      // Search Routes
      GoRoute(
        path: '/search',
        name: 'search',
        builder: (context, state) => const SearchPage(),
      ),
      GoRoute(
        path: '/search/results',
        name: 'search-results',
        builder: (context, state) {
          final query = state.uri.queryParameters['query'] ?? '';
          return SearchResultsPage(query: query);
        },
      ),
      GoRoute(
        path: '/search/filters',
        name: 'search-filters',
        builder: (context, state) => const SearchFiltersPage(),
      ),

      // Property Routes
      GoRoute(
        path: '/property/:id',
        name: 'property-details',
        builder: (context, state) {
          final propertyId = state.pathParameters['id']!;
          return PropertyDetailsPage(propertyId: propertyId);
        },
      ),
      GoRoute(
        path: '/property/:id/gallery',
        name: 'property-gallery',
        builder: (context, state) {
          final propertyId = state.pathParameters['id']!;
          final initialIndex = int.tryParse(state.uri.queryParameters['index'] ?? '0') ?? 0;
          return PropertyGalleryPage(
            propertyId: propertyId,
            initialIndex: initialIndex,
          );
        },
      ),
      GoRoute(
        path: '/property/:id/map',
        name: 'property-map',
        builder: (context, state) {
          final propertyId = state.pathParameters['id']!;
          return PropertyMapPage(propertyId: propertyId);
        },
      ),
      GoRoute(
        path: '/property/:id/reviews',
        name: 'property-reviews',
        builder: (context, state) {
          final propertyId = state.pathParameters['id']!;
          return PropertyReviewsPage(propertyId: propertyId);
        },
      ),

      // Booking Routes
      GoRoute(
        path: '/booking/form/:propertyId',
        name: 'booking-form',
        builder: (context, state) {
          final propertyId = state.pathParameters['propertyId']!;
          final unitId = state.uri.queryParameters['unitId'];
          return BookingFormPage(
            propertyId: propertyId,
            unitId: unitId,
          );
        },
      ),
      GoRoute(
        path: '/booking/summary',
        name: 'booking-summary',
        builder: (context, state) => const BookingSummaryPage(),
      ),
      GoRoute(
        path: '/booking/payment',
        name: 'booking-payment',
        builder: (context, state) {
          final bookingId = state.uri.queryParameters['bookingId'];
          return BookingPaymentPage(bookingId: bookingId);
        },
      ),
      GoRoute(
        path: '/booking/confirmation/:id',
        name: 'booking-confirmation',
        builder: (context, state) {
          final bookingId = state.pathParameters['id']!;
          return BookingConfirmationPage(bookingId: bookingId);
        },
      ),
      GoRoute(
        path: '/booking/details/:id',
        name: 'booking-details',
        builder: (context, state) {
          final bookingId = state.pathParameters['id']!;
          return BookingDetailsPage(bookingId: bookingId);
        },
      ),
      GoRoute(
        path: '/my-bookings',
        name: 'my-bookings',
        builder: (context, state) => const MyBookingsPage(),
      ),

      // Favorites Route
      GoRoute(
        path: '/favorites',
        name: 'favorites',
        builder: (context, state) => const FavoritesPage(),
      ),

      // Notification Routes
      GoRoute(
        path: '/notifications',
        name: 'notifications',
        builder: (context, state) => const NotificationsPage(),
      ),
      GoRoute(
        path: '/notification-settings',
        name: 'notification-settings',
        builder: (context, state) => const NotificationSettingsPage(),
      ),

      // Settings Routes
      GoRoute(
        path: '/settings',
        name: 'settings',
        builder: (context, state) => const SettingsPage(),
      ),
      GoRoute(
        path: '/settings/language',
        name: 'language-settings',
        builder: (context, state) => const LanguageSettingsPage(),
      ),
      GoRoute(
        path: '/settings/about',
        name: 'about',
        builder: (context, state) => const AboutPage(),
      ),
      GoRoute(
        path: '/settings/privacy',
        name: 'privacy-policy',
        builder: (context, state) => const PrivacyPolicyPage(),
      ),

      // Chat Routes
      GoRoute(
        path: '/conversations',
        name: 'conversations',
        builder: (context, state) => const ConversationsPage(),
      ),
      GoRoute(
        path: '/chat/:conversationId',
        name: 'chat',
        builder: (context, state) {
          final conversationId = state.pathParameters['conversationId']!;
          return ChatPage(conversationId: conversationId);
        },
      ),
      GoRoute(
        path: '/chat/settings/:conversationId',
        name: 'chat-settings',
        builder: (context, state) {
          final conversationId = state.pathParameters['conversationId']!;
          return ChatSettingsPage(conversationId: conversationId);
        },
      ),

      // Payment Routes
      GoRoute(
        path: '/payment-methods',
        name: 'payment-methods',
        builder: (context, state) => const PaymentMethodsPage(),
      ),
      GoRoute(
        path: '/add-payment-method',
        name: 'add-payment-method',
        builder: (context, state) => const AddPaymentMethodPage(),
      ),
      GoRoute(
        path: '/payment-history',
        name: 'payment-history',
        builder: (context, state) => const PaymentHistoryPage(),
      ),

      // Review Routes
      GoRoute(
        path: '/reviews/:propertyId',
        name: 'reviews-list',
        builder: (context, state) {
          final propertyId = state.pathParameters['propertyId']!;
          return ReviewsListPage(propertyId: propertyId);
        },
      ),
      GoRoute(
        path: '/write-review/:bookingId',
        name: 'write-review',
        builder: (context, state) {
          final bookingId = state.pathParameters['bookingId']!;
          return WriteReviewPage(bookingId: bookingId);
        },
      ),
    ],
    
    // Error handler
    errorBuilder: (context, state) => Scaffold(
      appBar: AppBar(
        title: const Text('5A-) :J1 EH,H/)'),
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.error_outline,
              size: 64,
              color: Colors.red,
            ),
            SizedBox(height: 16),
            Text(
              ''D5A-) 'DE7DH() :J1 EH,H/)',
              style: TextStyle(fontSize: 18),
            ),
            SizedBox(height: 8),
            Text(
              'J1,I 'D*-BB EF 'D1'(7 H'DE-'HD) E1) #.1I',
              style: TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
      ),
    ),
  );
}