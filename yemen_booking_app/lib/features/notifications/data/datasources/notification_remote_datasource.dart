import '../../../../core/models/paginated_result.dart';
import '../../../../core/models/result_dto.dart';
import '../../../../core/network/api_client.dart';
import '../models/notification_model.dart';

abstract class NotificationRemoteDataSource {
  Future<PaginatedResult<NotificationModel>> getNotifications({
    int page = 1,
    int limit = 20,
    String? type,
  });

  Future<void> markAsRead(String notificationId);

  Future<void> markAllAsRead({String? userId});

  Future<void> dismissNotification(String notificationId);

  Future<Map<String, bool>> getNotificationSettings({String? userId});

  Future<void> updateNotificationSettings(Map<String, bool> settings, {String? userId});

  Future<int> getUnreadCount({String? userId});
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
    final response = await apiClient.get(
      '/api/client/notifications',
      queryParameters: {
        'pageNumber': page,
        'pageSize': limit,
        if (type != null) 'type': type,
      },
    );

    if (response.statusCode == 200) {
      final result = ResultDto.fromJson(response.data, (json) => json);
      final data = result.data!;
      final items = (data['items'] as List<dynamic>?)?.map((e) => NotificationModel.fromJson(e)).toList() ?? <NotificationModel>[];
      return PaginatedResult<NotificationModel>(
        items: items,
        pageNumber: data['pageNumber'] ?? page,
        pageSize: data['pageSize'] ?? limit,
        totalCount: data['totalCount'] ?? items.length,
      );
    }

    return const PaginatedResult(items: [], pageNumber: 1, pageSize: 20, totalCount: 0);
  }

  @override
  Future<void> markAsRead(String notificationId) async {
    await apiClient.put(
      '/api/client/notifications/mark-as-read',
      data: {
        'notificationId': notificationId,
      },
    );
  }

  @override
  Future<void> markAllAsRead({String? userId}) async {
    await apiClient.put(
      '/api/client/notifications/mark-all-as-read',
      data: {
        if (userId != null) 'userId': userId,
      },
    );
  }

  @override
  Future<void> dismissNotification(String notificationId) async {
    await apiClient.delete(
      '/api/client/notifications/dismiss',
      data: {
        'notificationId': notificationId,
      },
    );
  }

  @override
  Future<Map<String, bool>> getNotificationSettings({String? userId}) async {
    return {};
  }

  @override
  Future<void> updateNotificationSettings(Map<String, bool> settings, {String? userId}) async {
    await apiClient.put(
      '/api/client/notifications/settings',
      data: {
        if (userId != null) 'userId': userId,
        'settings': settings,
      },
    );
  }

  @override
  Future<int> getUnreadCount({String? userId}) async {
    final response = await apiClient.get(
      '/api/client/notifications/summary',
      queryParameters: {
        if (userId != null) 'userId': userId,
      },
    );
    if (response.statusCode == 200) {
      final result = ResultDto.fromJson(response.data, (json) => json);
      final data = result.data ?? {};
      return (data['unreadCount'] as int?) ?? 0;
    }
    return 0;
  }
}