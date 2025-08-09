import 'dart:async';
import 'dart:convert';
import 'package:web_socket_channel/web_socket_channel.dart';
import 'package:web_socket_channel/status.dart' as status;
import '../core/constants/api_constants.dart';
import '../core/network/api_client.dart';
import '../features/auth/data/datasources/auth_local_datasource.dart';

class WebSocketService {
  WebSocketChannel? _channel;
  StreamSubscription? _subscription;
  final AuthLocalDataSource? _authLocalDataSource;
  final ApiClient? _apiClient;
  
  // Connection status
  bool _isConnected = false;
  bool get isConnected => _isConnected;
  
  // Reconnection
  Timer? _reconnectionTimer;
  int _reconnectionAttempts = 0;
  static const int _maxReconnectionAttempts = 5;
  static const Duration _reconnectionDelay = Duration(seconds: 5);
  
  // Stream controllers for different event types
  final _connectionStatusController = StreamController<ConnectionStatus>.broadcast();
  final _conversationEventController = StreamController<ConversationEvent>.broadcast();
  final _messageEventController = StreamController<MessageEvent>.broadcast();
  final _reactionEventController = StreamController<ReactionEvent>.broadcast();
  final _statusEventController = StreamController<StatusEvent>.broadcast();
  final _errorController = StreamController<WebSocketError>.broadcast();
  
  // Streams
  Stream<ConnectionStatus> get connectionStatus => _connectionStatusController.stream;
  Stream<ConversationEvent> get conversationEvents => _conversationEventController.stream;
  Stream<MessageEvent> get messageEvents => _messageEventController.stream;
  Stream<ReactionEvent> get reactionEvents => _reactionEventController.stream;
  Stream<StatusEvent> get statusEvents => _statusEventController.stream;
  Stream<WebSocketError> get errors => _errorController.stream;
  
  WebSocketService({
    AuthLocalDataSource? authLocalDataSource,
    ApiClient? apiClient,
  }) : _authLocalDataSource = authLocalDataSource,
       _apiClient = apiClient;

  // Connect to WebSocket (RECEIVE ONLY)
  Future<void> connect() async {
    if (_isConnected) return;
    
    try {
      _connectionStatusController.add(ConnectionStatus.connecting);
      
      // Get JWT token
      final token = await _authLocalDataSource?.getCachedAccessToken();
      if (token == null) {
        throw WebSocketException('No authentication token available');
      }
      
      // Build WebSocket URL
      final wsUrl = '${ApiConstants.socketUrl}/ws?token=$token';
      
      // Create WebSocket connection
      _channel = WebSocketChannel.connect(Uri.parse(wsUrl));
      
      // Listen to messages
      _subscription = _channel!.stream.listen(
        _handleMessage,
        onError: _handleError,
        onDone: _handleDone,
        cancelOnError: false,
      );
      
      _isConnected = true;
      _reconnectionAttempts = 0;
      _connectionStatusController.add(ConnectionStatus.connected);
      
    } catch (e) {
      _handleError(e);
    }
  }

  // Disconnect from WebSocket
  void disconnect() {
    _reconnectionTimer?.cancel();
    _subscription?.cancel();
    _channel?.sink.close(status.goingAway);
    _channel = null;
    _isConnected = false;
    _connectionStatusController.add(ConnectionStatus.disconnected);
  }

  // Handle incoming message from WebSocket
  void _handleMessage(dynamic data) {
    try {
      final jsonData = json.decode(data as String);
      final eventType = jsonData['event_type'] as String?;
      final eventData = jsonData['data'] as Map<String, dynamic>?;
      
      if (eventType == null || eventData == null) {
        throw Exception('Invalid message format');
      }
      
      // Route events to appropriate streams based on event_type
      switch (eventType) {
        // Conversation events
        case 'conversation_created':
        case 'conversation_updated':
        case 'conversation_deleted':
          _conversationEventController.add(ConversationEvent(
            type: eventType,
            conversation: ChatConversationDto.fromJson(eventData),
          ));
          break;
          
        // Message events
        case 'new_message':
          _messageEventController.add(MessageEvent(
            type: eventType,
            conversationId: eventData['conversation_id'],
            message: ChatMessageDto.fromJson(eventData['message']),
          ));
          break;
          
        case 'message_edited':
        case 'message_deleted':
          _messageEventController.add(MessageEvent(
            type: eventType,
            conversationId: eventData['conversation_id'],
            message: eventData['message'] != null 
                ? ChatMessageDto.fromJson(eventData['message'])
                : null,
            messageId: eventData['message_id'],
          ));
          break;
          
        // Reaction events
        case 'reaction_added':
        case 'reaction_removed':
          _reactionEventController.add(ReactionEvent(
            type: eventType,
            messageId: eventData['message_id'],
            reaction: MessageReactionDto.fromJson(eventData['reaction']),
          ));
          break;
          
        // Status events
        case 'message_status_updated':
          _statusEventController.add(StatusEvent(
            type: 'message_status',
            messageId: eventData['message_id'],
            status: eventData['status'],
            deliveryReceipt: eventData['delivery_receipt'] != null
                ? DeliveryReceiptDto.fromJson(eventData['delivery_receipt'])
                : null,
          ));
          break;
          
        case 'user_status_updated':
          _statusEventController.add(StatusEvent(
            type: 'user_status',
            userId: eventData['user_id'],
            status: eventData['status'],
            lastSeen: eventData['last_seen'] != null
                ? DateTime.parse(eventData['last_seen'])
                : null,
          ));
          break;
          
        default:
          debugPrint('Unknown event type: $eventType');
      }
    } catch (e) {
      _errorController.add(WebSocketError(
        type: ErrorType.parseError,
        message: 'Failed to parse message: $e',
      ));
    }
  }

  // HTTP Methods for sending commands (NOT WebSocket)
  
  // Send message via HTTP
  Future<void> sendMessage({
    required String conversationId,
    required String messageType,
    String? content,
    LocationDto? location,
    String? replyToMessageId,
    List<String>? attachmentIds,
  }) async {
    if (_apiClient == null) {
      throw WebSocketException('API client not available');
    }
    
    try {
      await _apiClient!.post(
        '/api/common/chat/conversations/$conversationId/messages',
        data: {
          'message_type': messageType,
          if (content != null) 'content': content,
          if (location != null) 'location': location.toJson(),
          if (replyToMessageId != null) 'reply_to_message_id': replyToMessageId,
          if (attachmentIds != null) 'attachment_ids': attachmentIds,
        },
      );
    } catch (e) {
      throw WebSocketException('Failed to send message: $e');
    }
  }

  // Add reaction via HTTP
  Future<void> addReaction({
    required String messageId,
    required String reactionType,
  }) async {
    if (_apiClient == null) {
      throw WebSocketException('API client not available');
    }
    
    try {
      await _apiClient!.post(
        '/api/common/chat/messages/$messageId/reactions',
        data: {
          'reaction_type': reactionType,
        },
      );
    } catch (e) {
      throw WebSocketException('Failed to add reaction: $e');
    }
  }

  // Remove reaction via HTTP
  Future<void> removeReaction({
    required String messageId,
    required String reactionType,
  }) async {
    if (_apiClient == null) {
      throw WebSocketException('API client not available');
    }
    
    try {
      await _apiClient!.delete(
        '/api/common/chat/messages/$messageId/reactions/$reactionType',
      );
    } catch (e) {
      throw WebSocketException('Failed to remove reaction: $e');
    }
  }

  // Update message status via HTTP
  Future<void> updateMessageStatus({
    required String messageId,
    required String status,
  }) async {
    if (_apiClient == null) {
      throw WebSocketException('API client not available');
    }
    
    try {
      await _apiClient!.put(
        '/api/common/chat/messages/$messageId/status',
        data: {
          'status': status,
        },
      );
    } catch (e) {
      throw WebSocketException('Failed to update message status: $e');
    }
  }

  // Handle error
  void _handleError(dynamic error) {
    _errorController.add(WebSocketError(
      type: ErrorType.connectionError,
      message: error.toString(),
    ));
    
    if (_isConnected) {
      _isConnected = false;
      _connectionStatusController.add(ConnectionStatus.error);
      _attemptReconnection();
    }
  }

  // Handle connection closed
  void _handleDone() {
    _isConnected = false;
    _connectionStatusController.add(ConnectionStatus.disconnected);
    _attemptReconnection();
  }

  // Attempt reconnection
  void _attemptReconnection() {
    if (_reconnectionAttempts >= _maxReconnectionAttempts) {
      _connectionStatusController.add(ConnectionStatus.failed);
      return;
    }
    
    _reconnectionAttempts++;
    _connectionStatusController.add(ConnectionStatus.reconnecting);
    
    _reconnectionTimer = Timer(_reconnectionDelay, () {
      connect();
    });
  }

  // Dispose
  void dispose() {
    disconnect();
    _connectionStatusController.close();
    _conversationEventController.close();
    _messageEventController.close();
    _reactionEventController.close();
    _statusEventController.close();
    _errorController.close();
  }
}

// DTO Models matching backend exactly

class ChatConversationDto {
  final String conversationId;
  final String conversationType; // "direct" | "group"
  final String? title;
  final String? description;
  final String? avatar;
  final DateTime createdAt;
  final DateTime updatedAt;
  final ChatMessageDto? lastMessage;
  final int unreadCount;
  final bool isArchived;
  final bool isMuted;
  final String? propertyId;
  final List<ChatUserDto> participants;

  ChatConversationDto({
    required this.conversationId,
    required this.conversationType,
    this.title,
    this.description,
    this.avatar,
    required this.createdAt,
    required this.updatedAt,
    this.lastMessage,
    required this.unreadCount,
    required this.isArchived,
    required this.isMuted,
    this.propertyId,
    required this.participants,
  });

  factory ChatConversationDto.fromJson(Map<String, dynamic> json) {
    return ChatConversationDto(
      conversationId: json['conversation_id'],
      conversationType: json['conversation_type'],
      title: json['title'],
      description: json['description'],
      avatar: json['avatar'],
      createdAt: DateTime.parse(json['created_at']),
      updatedAt: DateTime.parse(json['updated_at']),
      lastMessage: json['last_message'] != null
          ? ChatMessageDto.fromJson(json['last_message'])
          : null,
      unreadCount: json['unread_count'] ?? 0,
      isArchived: json['is_archived'] ?? false,
      isMuted: json['is_muted'] ?? false,
      propertyId: json['property_id'],
      participants: (json['participants'] as List)
          .map((p) => ChatUserDto.fromJson(p))
          .toList(),
    );
  }
}

class ChatMessageDto {
  final String messageId;
  final String conversationId;
  final String senderId;
  final String messageType; // "text", "image", "audio", etc
  final String? content;
  final LocationDto? location;
  final String? replyToMessageId;
  final List<MessageReactionDto> reactions;
  final List<ChatAttachmentDto> attachments;
  final DateTime createdAt;
  final DateTime updatedAt;
  final String status; // "sent","delivered","read","failed"
  final bool isEdited;
  final DateTime? editedAt;
  final DeliveryReceiptDto? deliveryReceipt;

  ChatMessageDto({
    required this.messageId,
    required this.conversationId,
    required this.senderId,
    required this.messageType,
    this.content,
    this.location,
    this.replyToMessageId,
    required this.reactions,
    required this.attachments,
    required this.createdAt,
    required this.updatedAt,
    required this.status,
    required this.isEdited,
    this.editedAt,
    this.deliveryReceipt,
  });

  factory ChatMessageDto.fromJson(Map<String, dynamic> json) {
    return ChatMessageDto(
      messageId: json['message_id'],
      conversationId: json['conversation_id'],
      senderId: json['sender_id'],
      messageType: json['message_type'],
      content: json['content'],
      location: json['location'] != null
          ? LocationDto.fromJson(json['location'])
          : null,
      replyToMessageId: json['reply_to_message_id'],
      reactions: (json['reactions'] as List? ?? [])
          .map((r) => MessageReactionDto.fromJson(r))
          .toList(),
      attachments: (json['attachments'] as List? ?? [])
          .map((a) => ChatAttachmentDto.fromJson(a))
          .toList(),
      createdAt: DateTime.parse(json['created_at']),
      updatedAt: DateTime.parse(json['updated_at']),
      status: json['status'],
      isEdited: json['is_edited'] ?? false,
      editedAt: json['edited_at'] != null
          ? DateTime.parse(json['edited_at'])
          : null,
      deliveryReceipt: json['delivery_receipt'] != null
          ? DeliveryReceiptDto.fromJson(json['delivery_receipt'])
          : null,
    );
  }
}

class MessageReactionDto {
  final String messageId;
  final String reactionType; // "like","love","laugh",...
  final String userId;

  MessageReactionDto({
    required this.messageId,
    required this.reactionType,
    required this.userId,
  });

  factory MessageReactionDto.fromJson(Map<String, dynamic> json) {
    return MessageReactionDto(
      messageId: json['message_id'],
      reactionType: json['reaction_type'],
      userId: json['user_id'],
    );
  }

  Map<String, dynamic> toJson() => {
    'message_id': messageId,
    'reaction_type': reactionType,
    'user_id': userId,
  };
}

class DeliveryReceiptDto {
  final DateTime? deliveredAt;
  final DateTime? readAt;
  final List<String> readBy;

  DeliveryReceiptDto({
    this.deliveredAt,
    this.readAt,
    required this.readBy,
  });

  factory DeliveryReceiptDto.fromJson(Map<String, dynamic> json) {
    return DeliveryReceiptDto(
      deliveredAt: json['delivered_at'] != null
          ? DateTime.parse(json['delivered_at'])
          : null,
      readAt: json['read_at'] != null
          ? DateTime.parse(json['read_at'])
          : null,
      readBy: List<String>.from(json['read_by'] ?? []),
    );
  }
}

class ChatUserDto {
  final String userId;
  final String userName;
  final String? avatar;
  final String status; // "online","offline","away"
  final DateTime? lastSeen;

  ChatUserDto({
    required this.userId,
    required this.userName,
    this.avatar,
    required this.status,
    this.lastSeen,
  });

  factory ChatUserDto.fromJson(Map<String, dynamic> json) {
    return ChatUserDto(
      userId: json['user_id'],
      userName: json['user_name'],
      avatar: json['avatar'],
      status: json['status'],
      lastSeen: json['last_seen'] != null
          ? DateTime.parse(json['last_seen'])
          : null,
    );
  }
}

class ChatAttachmentDto {
  final String attachmentId;
  final String attachmentType; // "image","video","audio","document"
  final String fileName;
  final String fileUrl;
  final int fileSize;
  final String? mimeType;
  final String? thumbnailUrl;
  final Map<String, dynamic>? metadata;

  ChatAttachmentDto({
    required this.attachmentId,
    required this.attachmentType,
    required this.fileName,
    required this.fileUrl,
    required this.fileSize,
    this.mimeType,
    this.thumbnailUrl,
    this.metadata,
  });

  factory ChatAttachmentDto.fromJson(Map<String, dynamic> json) {
    return ChatAttachmentDto(
      attachmentId: json['attachment_id'],
      attachmentType: json['attachment_type'],
      fileName: json['file_name'],
      fileUrl: json['file_url'],
      fileSize: json['file_size'],
      mimeType: json['mime_type'],
      thumbnailUrl: json['thumbnail_url'],
      metadata: json['metadata'],
    );
  }
}

class LocationDto {
  final double latitude;
  final double longitude;
  final String? address;

  LocationDto({
    required this.latitude,
    required this.longitude,
    this.address,
  });

  factory LocationDto.fromJson(Map<String, dynamic> json) {
    return LocationDto(
      latitude: json['latitude'].toDouble(),
      longitude: json['longitude'].toDouble(),
      address: json['address'],
    );
  }

  Map<String, dynamic> toJson() => {
    'latitude': latitude,
    'longitude': longitude,
    if (address != null) 'address': address,
  };
}

// Event Models
class ConversationEvent {
  final String type;
  final ChatConversationDto conversation;

  ConversationEvent({
    required this.type,
    required this.conversation,
  });
}

class MessageEvent {
  final String type;
  final String conversationId;
  final ChatMessageDto? message;
  final String? messageId;

  MessageEvent({
    required this.type,
    required this.conversationId,
    this.message,
    this.messageId,
  });
}

class ReactionEvent {
  final String type;
  final String messageId;
  final MessageReactionDto reaction;

  ReactionEvent({
    required this.type,
    required this.messageId,
    required this.reaction,
  });
}

class StatusEvent {
  final String type; // "message_status" or "user_status"
  final String? messageId;
  final String? userId;
  final String status;
  final DeliveryReceiptDto? deliveryReceipt;
  final DateTime? lastSeen;

  StatusEvent({
    required this.type,
    this.messageId,
    this.userId,
    required this.status,
    this.deliveryReceipt,
    this.lastSeen,
  });
}

class WebSocketError {
  final ErrorType type;
  final String message;

  WebSocketError({
    required this.type,
    required this.message,
  });
}

// Enums
enum ConnectionStatus {
  connecting,
  connected,
  disconnected,
  reconnecting,
  error,
  failed,
}

enum ErrorType {
  connectionError,
  parseError,
  notConnected,
  authError,
}

// Exception
class WebSocketException implements Exception {
  final String message;
  WebSocketException(this.message);
  
  @override
  String toString() => message;
}