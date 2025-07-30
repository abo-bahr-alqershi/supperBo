import 'dart:convert';
import 'dart:io';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:timezone/timezone.dart' as tz;
import 'package:dio/dio.dart';
import '../core/network/api_client.dart';
import '../core/constants/api_constants.dart';
import 'local_storage_service.dart';
import '../features/auth/data/datasources/auth_local_datasource.dart';

class NotificationService {
  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _localNotifications = 
      FlutterLocalNotificationsPlugin();
  
  final ApiClient? _apiClient;
  final LocalStorageService? _localStorage;
  final AuthLocalDataSource? _authLocalDataSource;

  NotificationService({
    ApiClient? apiClient,
    LocalStorageService? localStorage,
    AuthLocalDataSource? authLocalDataSource,
  }) : _apiClient = apiClient,
       _localStorage = localStorage,
       _authLocalDataSource = authLocalDataSource;

  // Static init method for backward compatibility
  static Future<void> init() async {
    final service = NotificationService();
    await service.initialize();
  }

  // Initialize notification service
  Future<void> initialize() async {
    // Request permission
    await _requestPermission();

    // Initialize local notifications
    await _initializeLocalNotifications();

    // Configure Firebase messaging
    await _configureFirebaseMessaging();

    // Get and register FCM token
    await _registerFcmToken();

    // Handle token refresh
    _firebaseMessaging.onTokenRefresh.listen(_onTokenRefresh);
  }

  // Request notification permission
  Future<void> _requestPermission() async {
    final settings = await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
      provisional: false,
      announcement: false,
      carPlay: false,
      criticalAlert: false,
    );

    print('Notification permission status: ${settings.authorizationStatus}');
  }

  // Initialize local notifications
  Future<void> _initializeLocalNotifications() async {
    const androidSettings = AndroidInitializationSettings('@mipmap/ic_launcher');
    const iosSettings = DarwinInitializationSettings(
      requestAlertPermission: true,
      requestBadgePermission: true,
      requestSoundPermission: true,
    );

    const initSettings = InitializationSettings(
      android: androidSettings,
      iOS: iosSettings,
    );

    await _localNotifications.initialize(
      initSettings,
      onDidReceiveNotificationResponse: _handleNotificationTap,
    );
  }

  // Configure Firebase messaging
  Future<void> _configureFirebaseMessaging() async {
    // Foreground message handler
    FirebaseMessaging.onMessage.listen(_handleForegroundMessage);

    // Background message handler
    FirebaseMessaging.onBackgroundMessage(_handleBackgroundMessage);

    // Message opened app handler
    FirebaseMessaging.onMessageOpenedApp.listen(_handleMessageOpenedApp);

    // Check if app was opened by notification
    final initialMessage = await _firebaseMessaging.getInitialMessage();
    if (initialMessage != null) {
      _handleMessageOpenedApp(initialMessage);
    }
  }

  // Register FCM token with backend
  Future<void> _registerFcmToken() async {
    try {
      final token = await _firebaseMessaging.getToken();
      if (token != null) {
        await _sendTokenToServer(token);
        await LocalStorageService.setString('fcm_token', token);
      }
    } catch (e) {
      print('Error registering FCM token: $e');
    }
  }

  // Send token to server
  Future<void> _sendTokenToServer(String token) async {
    if (_apiClient == null || _authLocalDataSource == null) return;

    try {
      final user = await _authLocalDataSource!.getCachedUser();
      if (user == null) return;

      final deviceType = Platform.isIOS ? 'iOS' : 'Android';

      await _apiClient!.post(
        '/api/fcm/register',
        data: {
          'userId': user.userId,
          'token': token,
          'deviceType': deviceType,
        },
      );
    } catch (e) {
      print('Error sending FCM token to server: $e');
    }
  }

  // Handle token refresh
  Future<void> _onTokenRefresh(String token) async {
    await _sendTokenToServer(token);
    await LocalStorageService.setString('fcm_token', token);
  }

  // Unregister FCM token
  Future<void> unregisterFcmToken() async {
    if (_apiClient == null || _authLocalDataSource == null) return;

    try {
      final user = await _authLocalDataSource!.getCachedUser();
      if (user == null) return;

      await _apiClient!.post(
        '/api/fcm/unregister',
        data: {
          'userId': user.userId,
        },
      );

      await _firebaseMessaging.deleteToken();
    } catch (e) {
      print('Error unregistering FCM token: $e');
    }
  }

  // Handle foreground messages
  Future<void> _handleForegroundMessage(RemoteMessage message) async {
    print('Foreground message received: ${message.messageId}');
    
    // Show local notification
    await _showLocalNotification(message);
  }

  // Handle background messages (static function required)
  static Future<void> _handleBackgroundMessage(RemoteMessage message) async {
    print('Background message received: ${message.messageId}');
  }

  // Handle notification tap when app is opened
  void _handleMessageOpenedApp(RemoteMessage message) {
    print('Message opened app: ${message.messageId}');
    _navigateToScreen(message.data);
  }

  // Handle local notification tap
  void _handleNotificationTap(NotificationResponse response) {
    if (response.payload != null) {
      final data = json.decode(response.payload!);
      _navigateToScreen(data);
    }
  }

  // Show local notification
  Future<void> _showLocalNotification(RemoteMessage message) async {
    final notification = message.notification;
    if (notification == null) return;

    const androidDetails = AndroidNotificationDetails(
      'yemen_booking_channel',
      'Yemen Booking Notifications',
      channelDescription: 'إشعارات تطبيق حجوزات اليمن',
      importance: Importance.high,
      priority: Priority.high,
      showWhen: true,
    );

    const iosDetails = DarwinNotificationDetails(
      presentAlert: true,
      presentBadge: true,
      presentSound: true,
    );

    const details = NotificationDetails(
      android: androidDetails,
      iOS: iosDetails,
    );

    await _localNotifications.show(
      notification.hashCode,
      notification.title,
      notification.body,
      details,
      payload: json.encode(message.data),
    );
  }

  // Navigate to appropriate screen based on notification data
  void _navigateToScreen(Map<String, dynamic> data) {
    final type = data['type'];
    final id = data['id'];

    switch (type) {
      case 'booking':
        // Navigate to booking details
        print('Navigate to booking: $id');
        break;
      case 'property':
        // Navigate to property details
        print('Navigate to property: $id');
        break;
      case 'chat':
        // Navigate to chat
        print('Navigate to chat: $id');
        break;
      case 'promotion':
        // Navigate to promotion
        print('Navigate to promotion: $id');
        break;
      default:
        // Navigate to notifications page
        print('Navigate to notifications');
    }
  }

  // Check if notifications are enabled
  Future<bool> areNotificationsEnabled() async {
    final settings = await _firebaseMessaging.getNotificationSettings();
    return settings.authorizationStatus == AuthorizationStatus.authorized;
  }

  // Open notification settings
  Future<void> openNotificationSettings() async {
    await _firebaseMessaging.requestPermission();
  }

  // Get pending notifications
  Future<List<PendingNotificationRequest>> getPendingNotifications() async {
    return await _localNotifications.pendingNotificationRequests();
  }

  // Cancel notification
  Future<void> cancelNotification(int id) async {
    await _localNotifications.cancel(id);
  }

  // Cancel all notifications
  Future<void> cancelAllNotifications() async {
    await _localNotifications.cancelAll();
  }

  // Schedule notification
  Future<void> scheduleNotification({
    required int id,
    required String title,
    required String body,
    required DateTime scheduledDate,
    Map<String, dynamic>? payload,
  }) async {
    const androidDetails = AndroidNotificationDetails(
      'yemen_booking_scheduled',
      'Yemen Booking Scheduled',
      channelDescription: 'إشعارات مجدولة لتطبيق حجوزات اليمن',
      importance: Importance.high,
      priority: Priority.high,
    );

    const iosDetails = DarwinNotificationDetails(
      presentAlert: true,
      presentBadge: true,
      presentSound: true,
    );

    const details = NotificationDetails(
      android: androidDetails,
      iOS: iosDetails,
    );

    await _localNotifications.zonedSchedule(
      id,
      title,
      body,
      tz.TZDateTime.from(scheduledDate, tz.local),
      details,
      androidScheduleMode: AndroidScheduleMode.exactAllowWhileIdle,
      uiLocalNotificationDateInterpretation:
          UILocalNotificationDateInterpretation.absoluteTime,
      payload: payload != null ? json.encode(payload) : null,
    );
  }

  // Get FCM token
  Future<String?> getFcmToken() async {
    return await _firebaseMessaging.getToken();
  }

  // Subscribe to topic
  Future<void> subscribeToTopic(String topic) async {
    await _firebaseMessaging.subscribeToTopic(topic);
  }

  // Unsubscribe from topic
  Future<void> unsubscribeFromTopic(String topic) async {
    await _firebaseMessaging.unsubscribeFromTopic(topic);
  }
}