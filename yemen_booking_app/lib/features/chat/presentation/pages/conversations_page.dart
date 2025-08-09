import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../../../core/widgets/error_widget.dart' as app;
import '../../../../core/widgets/loading_widget.dart';
import '../../domain/entities/conversation.dart';
import '../bloc/chat_bloc.dart';
import '../widgets/conversation_item_widget.dart';
import '../widgets/chat_fab.dart';
import 'chat_page.dart';
import 'new_conversation_page.dart';

class ConversationsPage extends StatefulWidget {
  const ConversationsPage({super.key});

  @override
  State<ConversationsPage> createState() => _ConversationsPageState();
}

class _ConversationsPageState extends State<ConversationsPage> 
    with TickerProviderStateMixin {
  final ScrollController _scrollController = ScrollController();
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;
  bool _isSearching = false;
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
    _loadConversations();
    _scrollController.addListener(_onScroll);
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
    _scrollController.dispose();
    _animationController.dispose();
    super.dispose();
  }

  void _loadConversations() {
    context.read<ChatBloc>().add(const InitializeChatEvent());
  }

  void _onScroll() {
    if (_isBottom) {
      final state = context.read<ChatBloc>().state;
      if (state is ChatLoaded && !state.isLoadingMore) {
        context.read<ChatBloc>().add(
          LoadConversationsEvent(
            pageNumber: (state.conversations.length ~/ 20) + 1,
          ),
        );
      }
    }
  }

  bool get _isBottom {
    if (!_scrollController.hasClients) return false;
    final maxScroll = _scrollController.position.maxScrollExtent;
    final currentScroll = _scrollController.offset;
    return currentScroll >= (maxScroll * 0.9);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      body: AnnotatedRegion<SystemUiOverlayStyle>(
        value: SystemUiOverlayStyle(
          statusBarColor: Colors.transparent,
          statusBarIconBrightness: Theme.of(context).brightness == Brightness.light
              ? Brightness.dark
              : Brightness.light,
        ),
        child: SafeArea(
          child: Column(
            children: [
              _buildHeader(),
              Expanded(
                child: BlocBuilder<ChatBloc, ChatState>(
                  builder: (context, state) {
                    if (state is ChatLoading) {
                      return const LoadingWidget(
                        type: LoadingType.circular,
                        message: 'جاري تحميل المحادثات...',
                      );
                    }

                    if (state is ChatError) {
                      return app.CustomErrorWidget(
                        message: state.message,
                        type: app.ErrorType.general,
                        onRetry: _loadConversations,
                      );
                    }

                    if (state is ChatLoaded) {
                      final conversations = _filterConversations(state.conversations);
                      
                      if (conversations.isEmpty) {
                        return EmptyWidget(
                          message: _isSearching 
                              ? 'لا توجد نتائج للبحث'
                              : 'لا توجد محادثات حتى الآن',
                          emptyImage: 'assets/images/empty_chat.svg',
                          actionWidget: !_isSearching
                              ? ElevatedButton.icon(
                                  onPressed: _startNewConversation,
                                  icon: const Icon(Icons.add_comment_rounded),
                                  label: const Text('بدء محادثة جديدة'),
                                )
                              : null,
                        );
                      }

                      return _buildConversationsList(conversations, state);
                    }

                    return const SizedBox.shrink();
                  },
                ),
              ),
            ],
          ),
        ),
      ),
      floatingActionButton: ChatFAB(
        onPressed: _startNewConversation,
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  'المحادثات',
                  style: AppTextStyles.heading2.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
              IconButton(
                onPressed: () {
                  setState(() {
                    _isSearching = !_isSearching;
                    if (!_isSearching) {
                      _searchQuery = '';
                    }
                  });
                },
                icon: Icon(
                  _isSearching ? Icons.close_rounded : Icons.search_rounded,
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          ),
          AnimatedSize(
            duration: const Duration(milliseconds: 200),
            child: _isSearching ? _buildSearchBar() : const SizedBox.shrink(),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Container(
      margin: const EdgeInsets.only(top: AppDimensions.spacingMd),
      height: 48,
      decoration: BoxDecoration(
        color: AppColors.inputBackground,
        borderRadius: BorderRadius.circular(AppDimensions.radiusLarge),
      ),
      child: TextField(
        autofocus: true,
        onChanged: (value) {
          setState(() {
            _searchQuery = value;
          });
        },
        style: AppTextStyles.bodyMedium,
        decoration: InputDecoration(
          hintText: 'ابحث عن محادثة...',
          hintStyle: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textHint,
          ),
          prefixIcon: const Icon(
            Icons.search_rounded,
            color: AppColors.textHint,
          ),
          border: InputBorder.none,
          contentPadding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMedium,
            vertical: AppDimensions.paddingSmall,
          ),
        ),
      ),
    );
  }

  Widget _buildConversationsList(
    List<Conversation> conversations,
    ChatLoaded state,
  ) {
    return RefreshIndicator(
      onRefresh: () async {
        context.read<ChatBloc>().add(const LoadConversationsEvent());
      },
      color: AppColors.primary,
      child: CustomScrollView(
        controller: _scrollController,
        physics: const BouncingScrollPhysics(),
        slivers: [
          SliverList(
            delegate: SliverChildBuilderDelegate(
              (context, index) {
                if (index >= conversations.length) {
                  return state.isLoadingMore
                      ? const Padding(
                          padding: EdgeInsets.all(AppDimensions.paddingMedium),
                          child: LoadingWidget(
                            type: LoadingType.dots,
                            size: 30,
                          ),
                        )
                      : const SizedBox.shrink();
                }

                final conversation = conversations[index];
                final typingUsers = state.typingUsers[conversation.id] ?? [];
                final currentUserId = 'current_user'; // Get from auth

                return FadeTransition(
                  opacity: _fadeAnimation,
                  child: SlideTransition(
                    position: Tween<Offset>(
                      begin: const Offset(0.1, 0),
                      end: Offset.zero,
                    ).animate(CurvedAnimation(
                      parent: _animationController,
                      curve: Interval(
                        index * 0.05,
                        1.0,
                        curve: Curves.easeOut,
                      ),
                    )),
                    child: ConversationItemWidget(
                      conversation: conversation,
                      currentUserId: currentUserId,
                      typingUserIds: typingUsers,
                      onTap: () => _openChat(conversation),
                      onLongPress: () => _showConversationOptions(conversation),
                    ),
                  ),
                );
              },
              childCount: conversations.length + (state.isLoadingMore ? 1 : 0),
            ),
          ),
        ],
      ),
    );
  }

  List<Conversation> _filterConversations(List<Conversation> conversations) {
    if (_searchQuery.isEmpty) return conversations;
    
    return conversations.where((conversation) {
      final title = conversation.title?.toLowerCase() ?? '';
      final lastMessage = conversation.lastMessage?.content?.toLowerCase() ?? '';
      final query = _searchQuery.toLowerCase();
      
      return title.contains(query) || lastMessage.contains(query);
    }).toList();
  }

  void _openChat(Conversation conversation) {
    HapticFeedback.lightImpact();
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => ChatPage(conversation: conversation),
      ),
    );
  }

  void _startNewConversation() {
    HapticFeedback.mediumImpact();
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const NewConversationPage(),
      ),
    );
  }

  void _showConversationOptions(Conversation conversation) {
    HapticFeedback.mediumImpact();
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      builder: (context) => _ConversationOptionsSheet(
        conversation: conversation,
        onArchive: () {
          context.read<ChatBloc>().add(
            conversation.isArchived
                ? UnarchiveConversationEvent(conversationId: conversation.id)
                : ArchiveConversationEvent(conversationId: conversation.id),
          );
        },
        onDelete: () {
          _confirmDelete(conversation);
        },
      ),
    );
  }

  void _confirmDelete(Conversation conversation) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('حذف المحادثة'),
        content: const Text('هل أنت متأكد من حذف هذه المحادثة؟'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('إلغاء'),
          ),
          TextButton(
            onPressed: () {
              context.read<ChatBloc>().add(
                DeleteConversationEvent(conversationId: conversation.id),
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
}

class _ConversationOptionsSheet extends StatelessWidget {
  final Conversation conversation;
  final VoidCallback onArchive;
  final VoidCallback onDelete;

  const _ConversationOptionsSheet({
    required this.conversation,
    required this.onArchive,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        borderRadius: const BorderRadius.vertical(
          top: Radius.circular(AppDimensions.radiusXLarge),
        ),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 40,
            height: 4,
            margin: const EdgeInsets.symmetric(
              vertical: AppDimensions.paddingMedium,
            ),
            decoration: BoxDecoration(
              color: AppColors.divider,
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          ListTile(
            leading: Icon(
              conversation.isArchived 
                  ? Icons.unarchive_rounded 
                  : Icons.archive_rounded,
              color: AppColors.textSecondary,
            ),
            title: Text(
              conversation.isArchived ? 'إلغاء الأرشفة' : 'أرشفة',
              style: AppTextStyles.bodyLarge,
            ),
            onTap: () {
              Navigator.pop(context);
              onArchive();
            },
          ),
          ListTile(
            leading: const Icon(
              Icons.delete_rounded,
              color: AppColors.error,
            ),
            title: Text(
              'حذف المحادثة',
              style: AppTextStyles.bodyLarge.copyWith(
                color: AppColors.error,
              ),
            ),
            onTap: () {
              Navigator.pop(context);
              onDelete();
            },
          ),
          const SizedBox(height: AppDimensions.paddingMedium),
        ],
      ),
    );
  }
}