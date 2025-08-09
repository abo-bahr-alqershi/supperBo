import 'dart:io';
import 'package:dio/dio.dart';
import 'package:yemen_booking_app/features/chat/data/models/chat_settings_model.dart';
import 'package:yemen_booking_app/features/chat/data/models/chat_user_model.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exceptions.dart';
import '../../../../core/error/exceptions.dart' hide ApiException;
import '../../domain/entities/message.dart';
import '../../domain/repositories/chat_repository.dart';
import '../models/attachment_model.dart';
import '../models/conversation_model.dart';
import '../models/message_model.dart';

abstract class ChatRemoteDataSource {
  Future<String> createConversation({
    required List<String> participantIds,
    required String conversationType,
    String? title,
    String? description,
    String? propertyId,
  });

  Future<List<ConversationModel>> getConversations({
    required int pageNumber,
    required int pageSize,
  });

  Future<ConversationModel> getConversationById(String conversationId);

  Future<void> archiveConversation(String conversationId);

  Future<void> unarchiveConversation(String conversationId);

  Future<void> deleteConversation(String conversationId);

  Future<List<MessageModel>> getMessages({
    required String conversationId,
    required int pageNumber,
    required int pageSize,
    String? beforeMessageId,
  });

  Future<MessageModel> sendMessage({
    required String conversationId,
    required String messageType,
    String? content,
    Location? location,
    String? replyToMessageId,
    List<String>? attachmentIds,
  });

  Future<MessageModel> editMessage({
    required String messageId,
    required String content,
  });

  Future<void> deleteMessage(String messageId);

  Future<void> markAsRead({
    required String conversationId,
    required List<String> messageIds,
  });

  Future<void> addReaction({
    required String messageId,
    required String reactionType,
  });

  Future<void> removeReaction({
    required String messageId,
    required String reactionType,
  });

  Future<AttachmentModel> uploadAttachment({
    required String conversationId,
    required String filePath,
    required String messageType,
    ProgressCallback? onSendProgress,
  });

  Future<SearchResult> searchChats({
    required String query,
    String? conversationId,
    String? messageType,
    String? senderId,
    DateTime? dateFrom,
    DateTime? dateTo,
    required int page,
    required int limit,
  });

  Future<List<ChatUserModel>> getAvailableUsers({
    String? userType,
    String? propertyId,
  });

  Future<void> updateUserStatus(String status);

  Future<ChatSettingsModel> getChatSettings();

  Future<ChatSettingsModel> updateChatSettings({
    bool? notificationsEnabled,
    bool? soundEnabled,
    bool? showReadReceipts,
    bool? showTypingIndicator,
    String? theme,
    String? fontSize,
    bool? autoDownloadMedia,
    bool? backupMessages,
  });
}

class ChatRemoteDataSourceImpl implements ChatRemoteDataSource {
  final ApiClient apiClient;

  ChatRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<String> createConversation({
    required List<String> participantIds,
    required String conversationType,
    String? title,
    String? description,
    String? propertyId,
  }) async {
    try {
      final response = await apiClient.post(
        '/api/common/chat/conversations',
        data: {
          'participantIds': participantIds,
          'conversationType': conversationType,
          if (title != null) 'title': title,
          if (description != null) 'description': description,
          if (propertyId != null) 'propertyId': propertyId,
        },
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        return response.data['data'] as String;
      }
      throw ServerException(response.data['message'] ?? 'فشل إنشاء المحادثة');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<List<ConversationModel>> getConversations({
    required int pageNumber,
    required int pageSize,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/common/chat/conversations',
        queryParameters: {
          'pageNumber': pageNumber,
          'pageSize': pageSize,
        },
      );

      if (response.data['conversations'] != null) {
        return (response.data['conversations'] as List)
            .map((json) => ConversationModel.fromJson(json))
            .toList();
      }
      return [];
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<ConversationModel> getConversationById(String conversationId) async {
    try {
      final response = await apiClient.get(
        '/api/common/chat/conversations/$conversationId',
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        return ConversationModel.fromJson(response.data['data']);
      }
      throw ServerException(response.data['message'] ?? 'المحادثة غير موجودة');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> archiveConversation(String conversationId) async {
    try {
      final response = await apiClient.post(
        '/api/common/chat/conversations/$conversationId/archive',
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل أرشفة المحادثة');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> unarchiveConversation(String conversationId) async {
    try {
      final response = await apiClient.post(
        '/api/common/chat/conversations/$conversationId/unarchive',
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل إلغاء أرشفة المحادثة');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> deleteConversation(String conversationId) async {
    try {
      final response = await apiClient.delete(
        '/api/common/chat/conversations/$conversationId',
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل حذف المحادثة');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<List<MessageModel>> getMessages({
    required String conversationId,
    required int pageNumber,
    required int pageSize,
    String? beforeMessageId,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/common/chat/conversations/$conversationId/messages',
        queryParameters: {
          'pageNumber': pageNumber,
          'pageSize': pageSize,
          if (beforeMessageId != null) 'beforeMessageId': beforeMessageId,
        },
      );

      if (response.data['messages'] != null) {
        return (response.data['messages'] as List)
            .map((json) => MessageModel.fromJson(json))
            .toList();
      }
      return [];
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<MessageModel> sendMessage({
    required String conversationId,
    required String messageType,
    String? content,
    Location? location,
    String? replyToMessageId,
    List<String>? attachmentIds,
  }) async {
    try {
      final formData = FormData.fromMap({
        'messageType': messageType,
        if (content != null) 'content': content,
        if (location != null) 'locationJson': {
          'latitude': location.latitude,
          'longitude': location.longitude,
          if (location.address != null) 'address': location.address,
        },
        if (replyToMessageId != null) 'replyToMessageId': replyToMessageId,
        if (attachmentIds != null && attachmentIds.isNotEmpty) 
          'attachmentIds': attachmentIds,
      });

      final response = await apiClient.post(
        '/api/common/chat/conversations/$conversationId/messages',
        data: formData,
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        return MessageModel.fromJson(response.data['data']);
      }
      throw ServerException(response.data['message'] ?? 'فشل إرسال الرسالة');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<MessageModel> editMessage({
    required String messageId,
    required String content,
  }) async {
    try {
      final response = await apiClient.put(
        '/api/common/chat/messages/$messageId',
        data: {
          'content': content,
        },
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        return MessageModel.fromJson(response.data['data']);
      }
      throw ServerException(response.data['message'] ?? 'فشل تعديل الرسالة');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> deleteMessage(String messageId) async {
    try {
      final response = await apiClient.delete(
        '/api/common/chat/messages/$messageId',
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل حذف الرسالة');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> markAsRead({
    required String conversationId,
    required List<String> messageIds,
  }) async {
    try {
      for (final messageId in messageIds) {
        await apiClient.put(
          '/api/common/chat/messages/$messageId/status',
          data: {
            'status': 'read',
          },
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> addReaction({
    required String messageId,
    required String reactionType,
  }) async {
    try {
      final response = await apiClient.post(
        '/api/common/chat/messages/$messageId/reactions',
        data: {
          'reactionType': reactionType,
        },
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل إضافة التفاعل');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> removeReaction({
    required String messageId,
    required String reactionType,
  }) async {
    try {
      final response = await apiClient.delete(
        '/api/common/chat/messages/$messageId/reactions/$reactionType',
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل إزالة التفاعل');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<AttachmentModel> uploadAttachment({
    required String conversationId,
    required String filePath,
    required String messageType,
    ProgressCallback? onSendProgress,
  }) async {
    try {
      final file = File(filePath);
      final formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(
          filePath,
          filename: file.path.split('/').last,
        ),
        'message_type': messageType,
        'conversationId': conversationId,
      });

      final response = await apiClient.upload(
        '/api/common/chat/upload',
        formData: formData,
        onSendProgress: onSendProgress,
      );

      if (response.data['success'] == true && response.data['attachment'] != null) {
        return AttachmentModel.fromJson(response.data['attachment']);
      }
      throw ServerException(response.data['message'] ?? 'فشل رفع المرفق');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<SearchResult> searchChats({
    required String query,
    String? conversationId,
    String? messageType,
    String? senderId,
    DateTime? dateFrom,
    DateTime? dateTo,
    required int page,
    required int limit,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/common/chat/search',
        queryParameters: {
          'query': query,
          if (conversationId != null) 'conversationId': conversationId,
          if (messageType != null) 'messageType': messageType,
          if (senderId != null) 'senderId': senderId,
          if (dateFrom != null) 'dateFrom': dateFrom.toIso8601String(),
          if (dateTo != null) 'dateTo': dateTo.toIso8601String(),
          'page': page,
          'limit': limit,
        },
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        final data = response.data['data'];
        return SearchResult(
          messages: (data['messages'] as List? ?? [])
              .map((json) => MessageModel.fromJson(json))
              .toList(),
          conversations: (data['conversations'] as List? ?? [])
              .map((json) => ConversationModel.fromJson(json))
              .toList(),
          totalCount: data['total_count'] ?? 0,
          hasMore: data['has_more'] ?? false,
          nextPageNumber: data['next_page'],
        );
      }
      throw ServerException(response.data['message'] ?? 'فشل البحث');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<List<ChatUserModel>> getAvailableUsers({
    String? userType,
    String? propertyId,
  }) async {
    try {
      final response = await apiClient.get(
        '/api/common/chat/users/available',
        queryParameters: {
          if (userType != null) 'userType': userType,
          if (propertyId != null) 'propertyId': propertyId,
        },
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        return (response.data['data'] as List)
            .map((json) => ChatUserModel.fromJson(json))
            .toList();
      }
      return [];
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> updateUserStatus(String status) async {
    try {
      final response = await apiClient.put(
        '/api/common/chat/users/status',
        data: {
          'status': status,
        },
      );

      if (response.data['success'] != true) {
        throw ServerException(response.data['message'] ?? 'فشل تحديث الحالة');
      }
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<ChatSettingsModel> getChatSettings() async {
    try {
      final response = await apiClient.get('/api/common/chat/settings');

      if (response.data['success'] == true && response.data['data'] != null) {
        return ChatSettingsModel.fromJson(response.data['data']);
      }
      throw ServerException(response.data['message'] ?? 'فشل جلب الإعدادات');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }

  @override
  Future<ChatSettingsModel> updateChatSettings({
    bool? notificationsEnabled,
    bool? soundEnabled,
    bool? showReadReceipts,
    bool? showTypingIndicator,
    String? theme,
    String? fontSize,
    bool? autoDownloadMedia,
    bool? backupMessages,
  }) async {
    try {
      final response = await apiClient.put(
        '/api/common/chat/settings',
        data: {
          if (notificationsEnabled != null) 'notificationsEnabled': notificationsEnabled,
          if (soundEnabled != null) 'soundEnabled': soundEnabled,
          if (showReadReceipts != null) 'showReadReceipts': showReadReceipts,
          if (showTypingIndicator != null) 'showTypingIndicator': showTypingIndicator,
          if (theme != null) 'theme': theme,
          if (fontSize != null) 'fontSize': fontSize,
          if (autoDownloadMedia != null) 'autoDownloadMedia': autoDownloadMedia,
          if (backupMessages != null) 'backupMessages': backupMessages,
        },
      );

      if (response.data['success'] == true && response.data['data'] != null) {
        return ChatSettingsModel.fromJson(response.data['data']);
      }
      throw ServerException(response.data['message'] ?? 'فشل تحديث الإعدادات');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    } catch (e) {
      if (e is ServerException) rethrow;
      throw ServerException(e.toString());
    }
  }
}