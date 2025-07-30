import '../../../../core/models/paginated_result.dart';
import '../../../../core/network/api_client.dart';
import '../models/notification_model.dart';

abstract class NotificationRemoteDataSource {
  Future<PaginatedResult<NotificationModel>> getNotifications({
    int page = 1,
    int limit = 20,
    String? type,
  });

  Future<void> markAsRead(String notificationId);

  Future<void> markAllAsRead();

  Future<void> dismissNotification(String notificationId);

  Future<Map<String, bool>> getNotificationSettings();

  Future<void> updateNotificationSettings(Map<String, bool> settings);

  Future<int> getUnreadCount();
}

class NotificationRemoteDataSourceImpl implements NotificationRemoteDataSource {
  final ApiClient apiClient;

  NotificationRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<PaginatedResult<NotificationModel>> getNotifications({
    int page = 1,
    int limit = 20,
    String? type,
  }) async {
    // TODO: Implement API call
    // For now, return empty result to avoid build errors
    return const PaginatedResult(
      items: [],
      pageNumber: 1,
      pageSize: 20,
      totalCount: 0,
    );
  }

  @override
  Future<void> markAsRead(String notificationId) async {
    // TODO: Implement API call
  }

  @override
  Future<void> markAllAsRead() async {
    // TODO: Implement API call
  }

  @override
  Future<void> dismissNotification(String notificationId) async {
    // TODO: Implement API call
  }

  @override
  Future<Map<String, bool>> getNotificationSettings() async {
    // TODO: Implement API call
    return {};
  }

  @override
  Future<void> updateNotificationSettings(Map<String, bool> settings) async {
    // TODO: Implement API call
  }

  @override
  Future<int> getUnreadCount() async {
    // TODO: Implement API call
    return 0;
  }
}