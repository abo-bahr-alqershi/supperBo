import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/message.dart';
import '../bloc/chat_bloc.dart';
import '../widgets/message_bubble_widget.dart';
import '../widgets/message_input_widget.dart';
import '../widgets/typing_indicator_widget.dart';
import '../widgets/chat_app_bar.dart';
import '../widgets/message_status_indicator.dart';
import '../widgets/attachment_preview_widget.dart';
import 'chat_settings_page.dart';
import 'package:provider/provider.dart';
import '../providers/typing_indicator_provider.dart';

class ChatPage extends StatefulWidget {
  final Conversation conversation;

  const ChatPage({
    super.key,
    required this.conversation,
  });

  @override
  State<ChatPage> createState() => _ChatPageState();
}

class _ChatPageState extends State<ChatPage> 
    with WidgetsBindingObserver, TickerProviderStateMixin {
  final ScrollController _scrollController = ScrollController();
  final TextEditingController _messageController = TextEditingController();
  final FocusNode _messageFocusNode = FocusNode();
  
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;
  
  Timer? _typingTimer;
  bool _isTyping = false;
  bool _showScrollToBottom = false;
  String? _replyToMessageId;
  Message? _editingMessage;
  
  final String currentUserId = 'current_user'; // Get from auth

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _initializeAnimations();
    _loadMessages();
    _scrollController.addListener(_onScroll);
    _messageController.addListener(_onTypingChanged);

    // Initialize typing indicator provider
    final typingProvider = context.read<TypingIndicatorProvider>();
    
    // Listen to WebSocket typing events
    webSocketService.typingEvents.listen((event) {
      for (final userId in event.typingUserIds) {
        typingProvider.setUserTyping(
          conversationId: event.conversationId,
          userId: userId,
          isTyping: true,
        );
      }
    });
  }

  void _initializeAnimations() {
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    _fadeAnimation = CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    );
    _animationController.forward();
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _scrollController.dispose();
    _messageController.dispose();
    _messageFocusNode.dispose();
    _animationController.dispose();
    _typingTimer?.cancel();
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) {
      _markMessagesAsRead();
    }
  }

  void _loadMessages() {
    context.read<ChatBloc>().add(
      LoadMessagesEvent(
        conversationId: widget.conversation.id,
      ),
    );
  }

  void _onScroll() {
    // Check if should show scroll to bottom button
    if (_scrollController.hasClients) {
      final showButton = _scrollController.offset > 200;
      if (showButton != _showScrollToBottom) {
        setState(() {
          _showScrollToBottom = showButton;
        });
      }

      // Load more messages when scrolled to top
      if (_scrollController.position.pixels == 
          _scrollController.position.maxScrollExtent) {
        _loadMoreMessages();
      }
    }
  }

  void _loadMoreMessages() {
    final state = context.read<ChatBloc>().state;
    if (state is ChatLoaded && !state.isLoadingMessages) {
      final messages = state.messages[widget.conversation.id] ?? [];
      if (messages.isNotEmpty) {
        context.read<ChatBloc>().add(
          LoadMessagesEvent(
            conversationId: widget.conversation.id,
            pageNumber: (messages.length ~/ 50) + 1,
            beforeMessageId: messages.last.id,
          ),
        );
      }
    }
  }

  void _onTypingChanged() {
    if (_messageController.text.isNotEmpty && !_isTyping) {
      _isTyping = true;
      context.read<ChatBloc>().add(
        SendTypingIndicatorEvent(
          conversationId: widget.conversation.id,
          isTyping: true,
        ),
      );
    }

    _typingTimer?.cancel();
    _typingTimer = Timer(const Duration(seconds: 3), () {
      if (_isTyping) {
        _isTyping = false;
        context.read<ChatBloc>().add(
          SendTypingIndicatorEvent(
            conversationId: widget.conversation.id,
            isTyping: false,
          ),
        );
      }
    });
  }

  void _markMessagesAsRead() {
    final state = context.read<ChatBloc>().state;
    if (state is ChatLoaded) {
      final messages = state.messages[widget.conversation.id] ?? [];
      final unreadMessages = messages
          .where((m) => m.senderId != currentUserId && !m.isRead)
          .map((m) => m.id)
          .toList();
      
      if (unreadMessages.isNotEmpty) {
        context.read<ChatBloc>().add(
          MarkMessagesAsReadEvent(
            conversationId: widget.conversation.id,
            messageIds: unreadMessages,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          children: [
            ChatAppBar(
              conversation: widget.conversation,
              currentUserId: currentUserId,
              onBackPressed: () => Navigator.pop(context),
              onInfoPressed: () => _openChatSettings(),
            ),
            Expanded(
              child: Stack(
                children: [
                  _buildMessagesList(),
                  if (_showScrollToBottom) _buildScrollToBottomButton(),
                ],
              ),
            ),
            _buildBottomSection(),
          ],
        ),
      ),
    );
  }

  Widget _buildMessagesList() {
    return Consumer<TypingIndicatorProvider>(
    builder: (context, typingProvider, child) {
      final typingUsers = typingProvider.getTypingUsersForConversation(
        widget.conversation.id,
      );
      return BlocBuilder<ChatBloc, ChatState>(
        builder: (context, state) {
          if (state is! ChatLoaded) {
            return const LoadingWidget(
              type: LoadingType.circular,
              message: 'جاري تحميل الرسائل...',
            );
          }

          final messages = state.messages[widget.conversation.id] ?? [];
          final typingUsers = state.typingUsers[widget.conversation.id] ?? [];

          if (messages.isEmpty) {
            return Center(
              child: Container(
                padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                margin: const EdgeInsets.all(AppDimensions.paddingLarge),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  borderRadius: BorderRadius.circular(AppDimensions.radiusMedium),
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.waving_hand_rounded,
                      size: 48,
                      color: AppColors.primary.withValues(alpha: 0.5),
                    ),
                    const SizedBox(height: AppDimensions.spacingMd),
                    Text(
                      'ابدأ المحادثة',
                      style: AppTextStyles.subtitle1,
                    ),
                    const SizedBox(height: AppDimensions.spacingSm),
                    Text(
                      'اكتب رسالتك الأولى للبدء',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
            );
          }

          return ListView.builder(
            controller: _scrollController,
            reverse: true,
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
              vertical: AppDimensions.paddingSmall,
            ),
            itemCount: messages.length + (typingUsers.isNotEmpty ? 1 : 0),
            itemBuilder: (context, index) {
              if (index == 0 && typingUsers.isNotEmpty) {
                return Padding(
                  padding: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
                  child: TypingIndicatorWidget(
                    typingUserIds: typingUsers,
                    conversation: widget.conversation,
                  ),
                );
              }

              final messageIndex = typingUsers.isNotEmpty ? index - 1 : index;
              final message = messages[messageIndex];
              final previousMessage = messageIndex < messages.length - 1 
                  ? messages[messageIndex + 1] 
                  : null;
              final nextMessage = messageIndex > 0 
                  ? messages[messageIndex - 1] 
                  : null;

              final showDateSeparator = previousMessage == null ||
                  !_isSameDay(message.createdAt, previousMessage.createdAt);

              return Column(
                children: [
                  if (showDateSeparator) _buildDateSeparator(message.createdAt),
                  FadeTransition(
                    opacity: _fadeAnimation,
                    child: MessageBubbleWidget(
                      message: message,
                      isMe: message.senderId == currentUserId,
                      previousMessage: previousMessage,
                      nextMessage: nextMessage,
                      onReply: () => _setReplyTo(message),
                      onEdit: message.senderId == currentUserId 
                          ? () => _startEditingMessage(message)
                          : null,
                      onDelete: message.senderId == currentUserId
                          ? () => _deleteMessage(message)
                          : null,
                      onReaction: (reactionType) => _addReaction(message, reactionType),
                    ),
                  ),
                ],
              );
            },
          );
        },
      );
      },
    );
  }

  Widget _buildDateSeparator(DateTime date) {
    final text = _getDateSeparatorText(date);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppDimensions.spacingLg),
      child: Row(
        children: [
          Expanded(
            child: Container(
              height: 1,
              color: AppColors.divider,
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
            ),
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
              ),
              child: Text(
                text,
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ),
          ),
          Expanded(
            child: Container(
              height: 1,
              color: AppColors.divider,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildScrollToBottomButton() {
    return Positioned(
      bottom: AppDimensions.paddingMedium,
      right: AppDimensions.paddingMedium,
      child: AnimatedOpacity(
        opacity: _showScrollToBottom ? 1.0 : 0.0,
        duration: const Duration(milliseconds: 200),
        child: FloatingActionButton.small(
          onPressed: _scrollToBottom,
          backgroundColor: AppColors.surface,
          elevation: 4,
          child: const Icon(
            Icons.keyboard_arrow_down_rounded,
            color: AppColors.textPrimary,
          ),
        ),
      ),
    );
  }

  Widget _buildBottomSection() {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (_replyToMessageId != null) _buildReplySection(),
        if (_editingMessage != null) _buildEditSection(),
        Container(
          decoration: BoxDecoration(
            color: AppColors.surface,
            boxShadow: [
              BoxShadow(
                color: AppColors.shadow.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, -2),
              ),
            ],
          ),
          child: MessageInputWidget(
            controller: _messageController,
            focusNode: _messageFocusNode,
            conversationId: widget.conversation.id,
            replyToMessageId: _replyToMessageId,
            editingMessage: _editingMessage,
            onSend: _sendMessage,
            onAttachment: _pickAttachment,
            onLocation: _shareLocation,
            onCancelReply: () {
              setState(() {
                _replyToMessageId = null;
              });
            },
            onCancelEdit: () {
              setState(() {
                _editingMessage = null;
                _messageController.clear();
              });
            },
          ),
        ),
      ],
    );
  }

  Widget _buildReplySection() {
    final state = context.read<ChatBloc>().state;
    if (state is! ChatLoaded) return const SizedBox.shrink();
    
    final messages = state.messages[widget.conversation.id] ?? [];
    final replyMessage = messages.firstWhere(
      (m) => m.id == _replyToMessageId,
      orElse: () => messages.first,
    );

    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.primary.withValues(alpha: 0.1),
        border: Border(
          left: BorderSide(
            color: AppColors.primary,
            width: 3,
          ),
        ),
      ),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'الرد على',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.primary,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  replyMessage.content ?? 'رسالة',
                  style: AppTextStyles.bodySmall,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
          IconButton(
            onPressed: () {
              setState(() {
                _replyToMessageId = null;
              });
            },
            icon: const Icon(
              Icons.close_rounded,
              size: 20,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildEditSection() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.warning.withValues(alpha: 0.1),
        border: Border(
          left: BorderSide(
            color: AppColors.warning,
            width: 3,
          ),
        ),
      ),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'تعديل الرسالة',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.warning,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  _editingMessage?.content ?? '',
                  style: AppTextStyles.bodySmall,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
          IconButton(
            onPressed: () {
              setState(() {
                _editingMessage = null;
                _messageController.clear();
              });
            },
            icon: const Icon(
              Icons.close_rounded,
              size: 20,
            ),
          ),
        ],
      ),
    );
  }

  void _sendMessage(String content) {
    if (content.trim().isEmpty) return;

    HapticFeedback.lightImpact();

    if (_editingMessage != null) {
      context.read<ChatBloc>().add(
        EditMessageEvent(
          messageId: _editingMessage!.id,
          content: content,
        ),
      );
      setState(() {
        _editingMessage = null;
      });
    } else {
      context.read<ChatBloc>().add(
        SendMessageEvent(
          conversationId: widget.conversation.id,
          messageType: 'text',
          content: content,
          replyToMessageId: _replyToMessageId,
        ),
      );
      setState(() {
        _replyToMessageId = null;
      });
    }

    _messageController.clear();
    _scrollToBottom();
  }

  void _pickAttachment() {
    HapticFeedback.mediumImpact();
    // Implement attachment picker
  }

  void _shareLocation() {
    HapticFeedback.mediumImpact();
    // Implement location sharing
  }

  void _setReplyTo(Message message) {
    setState(() {
      _replyToMessageId = message.id;
    });
    _messageFocusNode.requestFocus();
  }

  void _startEditingMessage(Message message) {
    setState(() {
      _editingMessage = message;
      _messageController.text = message.content ?? '';
    });
    _messageFocusNode.requestFocus();
  }

  void _deleteMessage(Message message) {
    HapticFeedback.mediumImpact();
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('حذف الرسالة'),
        content: const Text('هل أنت متأكد من حذف هذه الرسالة؟'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('إلغاء'),
          ),
          TextButton(
            onPressed: () {
              context.read<ChatBloc>().add(
                DeleteMessageEvent(messageId: message.id),
              );
              Navigator.pop(context);
            },
            style: TextButton.styleFrom(foregroundColor: AppColors.error),
            child: const Text('حذف'),
          ),
        ],
      ),
    );
  }

  void _addReaction(Message message, String reactionType) {
    HapticFeedback.lightImpact();
    
    final hasReaction = message.reactions.any(
      (r) => r.userId == currentUserId && r.reactionType == reactionType,
    );

    if (hasReaction) {
      context.read<ChatBloc>().add(
        RemoveReactionEvent(
          messageId: message.id,
          reactionType: reactionType,
        ),
      );
    } else {
      context.read<ChatBloc>().add(
        AddReactionEvent(
          messageId: message.id,
          reactionType: reactionType,
        ),
      );
    }
  }

  void _scrollToBottom() {
    if (_scrollController.hasClients) {
      _scrollController.animateTo(
        0,
        duration: const Duration(milliseconds: 300),
        curve: Curves.easeOut,
      );
    }
  }

  void _openChatSettings() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => ChatSettingsPage(
          conversation: widget.conversation,
        ),
      ),
    );
  }

  bool _isSameDay(DateTime date1, DateTime date2) {
    return date1.year == date2.year &&
        date1.month == date2.month &&
        date1.day == date2.day;
  }

  String _getDateSeparatorText(DateTime date) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final yesterday = today.subtract(const Duration(days: 1));
    final messageDate = DateTime(date.year, date.month, date.day);

    if (messageDate == today) {
      return 'اليوم';
    } else if (messageDate == yesterday) {
      return 'أمس';
    } else if (now.difference(date).inDays < 7) {
      final days = ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];
      return days[date.weekday % 7];
    } else {
      return '${date.day}/${date.month}/${date.year}';
    }
  }
}