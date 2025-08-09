part of 'chat_bloc.dart';

abstract class ChatEvent extends Equatable {
  const ChatEvent();

  @override
  List<Object?> get props => [];
}

class InitializeChatEvent extends ChatEvent {
  const InitializeChatEvent();
}

class LoadConversationsEvent extends ChatEvent {
  final int pageNumber;
  final int pageSize;

  const LoadConversationsEvent({
    this.pageNumber = 1,
    this.pageSize = 20,
  });

  @override
  List<Object> get props => [pageNumber, pageSize];
}

class LoadMessagesEvent extends ChatEvent {
  final String conversationId;
  final int pageNumber;
  final int pageSize;
  final String? beforeMessageId;

  const LoadMessagesEvent({
    required this.conversationId,
    this.pageNumber = 1,
    this.pageSize = 50,
    this.beforeMessageId,
  });

  @override
  List<Object?> get props => [conversationId, pageNumber, pageSize, beforeMessageId];
}

class SendMessageEvent extends ChatEvent {
  final String conversationId;
  final String messageType;
  final String? content;
  final Location? location;
  final String? replyToMessageId;
  final List<String>? attachmentIds;

  const SendMessageEvent({
    required this.conversationId,
    required this.messageType,
    this.content,
    this.location,
    this.replyToMessageId,
    this.attachmentIds,
  });

  @override
  List<Object?> get props => [
    conversationId,
    messageType,
    content,
    location,
    replyToMessageId,
    attachmentIds,
  ];
}

class CreateConversationEvent extends ChatEvent {
  final List<String> participantIds;
  final String conversationType;
  final String? title;
  final String? description;
  final String? propertyId;

  const CreateConversationEvent({
    required this.participantIds,
    required this.conversationType,
    this.title,
    this.description,
    this.propertyId,
  });

  @override
  List<Object?> get props => [
    participantIds,
    conversationType,
    title,
    description,
    propertyId,
  ];
}

class DeleteConversationEvent extends ChatEvent {
  final String conversationId;

  const DeleteConversationEvent({required this.conversationId});

  @override
  List<Object> get props => [conversationId];
}

class ArchiveConversationEvent extends ChatEvent {
  final String conversationId;

  const ArchiveConversationEvent({required this.conversationId});

  @override
  List<Object> get props => [conversationId];
}

class UnarchiveConversationEvent extends ChatEvent {
  final String conversationId;

  const UnarchiveConversationEvent({required this.conversationId});

  @override
  List<Object> get props => [conversationId];
}

class DeleteMessageEvent extends ChatEvent {
  final String messageId;

  const DeleteMessageEvent({required this.messageId});

  @override
  List<Object> get props => [messageId];
}

class EditMessageEvent extends ChatEvent {
  final String messageId;
  final String content;

  const EditMessageEvent({
    required this.messageId,
    required this.content,
  });

  @override
  List<Object> get props => [messageId, content];
}

class AddReactionEvent extends ChatEvent {
  final String messageId;
  final String reactionType;

  const AddReactionEvent({
    required this.messageId,
    required this.reactionType,
  });

  @override
  List<Object> get props => [messageId, reactionType];
}

class RemoveReactionEvent extends ChatEvent {
  final String messageId;
  final String reactionType;

  const RemoveReactionEvent({
    required this.messageId,
    required this.reactionType,
  });

  @override
  List<Object> get props => [messageId, reactionType];
}

class MarkMessagesAsReadEvent extends ChatEvent {
  final String conversationId;
  final List<String> messageIds;

  const MarkMessagesAsReadEvent({
    required this.conversationId,
    required this.messageIds,
  });

  @override
  List<Object> get props => [conversationId, messageIds];
}

class UploadAttachmentEvent extends ChatEvent {
  final String conversationId;
  final String filePath;
  final String messageType;
  final Function(int, int)? onProgress;

  const UploadAttachmentEvent({
    required this.conversationId,
    required this.filePath,
    required this.messageType,
    this.onProgress,
  });

  @override
  List<Object?> get props => [conversationId, filePath, messageType];
}

class SearchChatsEvent extends ChatEvent {
  final String query;
  final String? conversationId;
  final String? messageType;
  final String? senderId;
  final DateTime? dateFrom;
  final DateTime? dateTo;
  final int page;
  final int limit;

  const SearchChatsEvent({
    required this.query,
    this.conversationId,
    this.messageType,
    this.senderId,
    this.dateFrom,
    this.dateTo,
    this.page = 1,
    this.limit = 20,
  });

  @override
  List<Object?> get props => [
    query,
    conversationId,
    messageType,
    senderId,
    dateFrom,
    dateTo,
    page,
    limit,
  ];
}

class LoadAvailableUsersEvent extends ChatEvent {
  final String? userType;
  final String? propertyId;

  const LoadAvailableUsersEvent({
    this.userType,
    this.propertyId,
  });

  @override
  List<Object?> get props => [userType, propertyId];
}

class UpdateUserStatusEvent extends ChatEvent {
  final String status;

  const UpdateUserStatusEvent({required this.status});

  @override
  List<Object> get props => [status];
}

class LoadChatSettingsEvent extends ChatEvent {
  const LoadChatSettingsEvent();
}

class UpdateChatSettingsEvent extends ChatEvent {
  final bool? notificationsEnabled;
  final bool? soundEnabled;
  final bool? showReadReceipts;
  final bool? showTypingIndicator;
  final String? theme;
  final String? fontSize;
  final bool? autoDownloadMedia;
  final bool? backupMessages;

  const UpdateChatSettingsEvent({
    this.notificationsEnabled,
    this.soundEnabled,
    this.showReadReceipts,
    this.showTypingIndicator,
    this.theme,
    this.fontSize,
    this.autoDownloadMedia,
    this.backupMessages,
  });

  @override
  List<Object?> get props => [
    notificationsEnabled,
    soundEnabled,
    showReadReceipts,
    showTypingIndicator,
    theme,
    fontSize,
    autoDownloadMedia,
    backupMessages,
  ];
}

class SendTypingIndicatorEvent extends ChatEvent {
  final String conversationId;
  final bool isTyping;

  const SendTypingIndicatorEvent({
    required this.conversationId,
    required this.isTyping,
  });

  @override
  List<Object> get props => [conversationId, isTyping];
}

class WebSocketMessageReceivedEvent extends ChatEvent {
  final MessageEvent messageEvent;

  const WebSocketMessageReceivedEvent(this.messageEvent);

  @override
  List<Object> get props => [messageEvent];
}

class WebSocketConversationUpdatedEvent extends ChatEvent {
  final Conversation conversation;

  const WebSocketConversationUpdatedEvent(this.conversation);

  @override
  List<Object> get props => [conversation];
}

class WebSocketTypingIndicatorEvent extends ChatEvent {
  final String conversationId;
  final List<String> typingUserIds;

  const WebSocketTypingIndicatorEvent({
    required this.conversationId,
    required this.typingUserIds,
  });

  @override
  List<Object> get props => [conversationId, typingUserIds];
}

class WebSocketPresenceUpdateEvent extends ChatEvent {
  final String userId;
  final String status;
  final DateTime? lastSeen;

  const WebSocketPresenceUpdateEvent({
    required this.userId,
    required this.status,
    this.lastSeen,
  });

  @override
  List<Object?> get props => [userId, status, lastSeen];
}