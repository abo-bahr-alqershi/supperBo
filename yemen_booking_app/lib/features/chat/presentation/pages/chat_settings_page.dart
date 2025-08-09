import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/attachment.dart';
import '../bloc/chat_bloc.dart';
import '../widgets/media_grid_widget.dart';
import '../widgets/participant_item_widget.dart';

class ChatSettingsPage extends StatefulWidget {
  final Conversation conversation;

  const ChatSettingsPage({
    super.key,
    required this.conversation,
  });

  @override
  State<ChatSettingsPage> createState() => _ChatSettingsPageState();
}

class _ChatSettingsPageState extends State<ChatSettingsPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final String currentUserId = 'current_user'; // Get from auth
  
  bool _notificationsEnabled = true;
  bool _showReadReceipts = true;
  String _theme = 'default';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _loadSettings();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  void _loadSettings() {
    context.read<ChatBloc>().add(const LoadChatSettingsEvent());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: CustomScrollView(
        slivers: [
          _buildSliverAppBar(),
          SliverToBoxAdapter(
            child: Column(
              children: [
                _buildInfoSection(),
                _buildTabBar(),
              ],
            ),
          ),
          SliverFillRemaining(
            child: TabBarView(
              controller: _tabController,
              children: [
                _buildMediaTab(),
                _buildParticipantsTab(),
                _buildSettingsTab(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSliverAppBar() {
    final displayImage = widget.conversation.avatar ??
        widget.conversation.getOtherParticipant(currentUserId)?.profileImage;

    return SliverAppBar(
      expandedHeight: 200,
      pinned: true,
      backgroundColor: AppColors.primary,
      leading: IconButton(
        onPressed: () {
          HapticFeedback.lightImpact();
          Navigator.pop(context);
        },
        icon: const Icon(
          Icons.arrow_back_ios_new_rounded,
          color: Colors.white,
        ),
      ),
      flexibleSpace: FlexibleSpaceBar(
        background: Stack(
          fit: StackFit.expand,
          children: [
            if (displayImage != null)
              CachedImageWidget(
                imageUrl: displayImage,
                fit: BoxFit.cover,
              )
            else
              Container(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: [
                      AppColors.primary,
                      AppColors.primaryDark,
                    ],
                  ),
                ),
              ),
            Container(
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: [
                    Colors.transparent,
                    Colors.black.withValues(alpha: 0.7),
                  ],
                ),
              ),
            ),
          ],
        ),
        title: Text(
          widget.conversation.title ??
              widget.conversation.getOtherParticipant(currentUserId)?.name ??
              'معلومات المحادثة',
          style: AppTextStyles.heading3.copyWith(
            color: Colors.white,
          ),
        ),
        centerTitle: true,
      ),
      actions: [
        PopupMenuButton<String>(
          icon: const Icon(
            Icons.more_vert_rounded,
            color: Colors.white,
          ),
          onSelected: _handleMenuAction,
          itemBuilder: (context) => [
            const PopupMenuItem(
              value: 'search',
              child: Row(
                children: [
                  Icon(Icons.search_rounded),
                  SizedBox(width: AppDimensions.spacingMd),
                  Text('بحث في المحادثة'),
                ],
              ),
            ),
            const PopupMenuItem(
              value: 'export',
              child: Row(
                children: [
                  Icon(Icons.download_rounded),
                  SizedBox(width: AppDimensions.spacingMd),
                  Text('تصدير المحادثة'),
                ],
              ),
            ),
            if (widget.conversation.isGroupChat)
              const PopupMenuItem(
                value: 'leave',
                child: Row(
                  children: [
                    Icon(Icons.exit_to_app_rounded, color: AppColors.error),
                    SizedBox(width: AppDimensions.spacingMd),
                    Text('مغادرة المجموعة', style: TextStyle(color: AppColors.error)),
                  ],
                ),
              ),
            const PopupMenuItem(
              value: 'clear',
              child: Row(
                children: [
                  Icon(Icons.clear_all_rounded, color: AppColors.error),
                  SizedBox(width: AppDimensions.spacingMd),
                  Text('مسح المحادثة', style: TextStyle(color: AppColors.error)),
                ],
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildInfoSection() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      color: AppColors.surface,
      child: Column(
        children: [
          if (widget.conversation.description != null) ...[
            Text(
              widget.conversation.description!,
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: AppDimensions.spacingMd),
          ],
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              _InfoItem(
                icon: Icons.message_rounded,
                label: 'رسائل',
                value: '1,234',
              ),
              _InfoItem(
                icon: Icons.image_rounded,
                label: 'وسائط',
                value: '89',
              ),
              _InfoItem(
                icon: Icons.attach_file_rounded,
                label: 'ملفات',
                value: '12',
              ),
              if (widget.conversation.isGroupChat)
                _InfoItem(
                  icon: Icons.group_rounded,
                  label: 'أعضاء',
                  value: widget.conversation.participants.length.toString(),
                ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildTabBar() {
    return Container(
      color: AppColors.surface,
      child: TabBar(
        controller: _tabController,
        labelColor: AppColors.primary,
        unselectedLabelColor: AppColors.textSecondary,
        indicatorColor: AppColors.primary,
        tabs: const [
          Tab(text: 'الوسائط'),
          Tab(text: 'الأعضاء'),
          Tab(text: 'الإعدادات'),
        ],
      ),
    );
  }

  Widget _buildMediaTab() {
    return BlocBuilder<ChatBloc, ChatState>(
      builder: (context, state) {
        if (state is! ChatLoaded) {
          return const LoadingWidget();
        }

        final messages = state.messages[widget.conversation.id] ?? [];
        final mediaMessages = messages.where((m) {
          return ['image', 'video'].contains(m.messageType) ||
              m.attachments.any((a) => a.isImage || a.isVideo);
        }).toList();

        if (mediaMessages.isEmpty) {
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  Icons.photo_library_rounded,
                  size: 64,
                  color: AppColors.textSecondary.withValues(alpha: 0.5),
                ),
                const SizedBox(height: AppDimensions.spacingMd),
                Text(
                  'لا توجد وسائط',
                  style: AppTextStyles.bodyLarge.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          );
        }

        return MediaGridWidget(
          messages: mediaMessages,
          onMediaTap: (message) {
            // Open media viewer
          },
        );
      },
    );
  }

  Widget _buildParticipantsTab() {
    if (widget.conversation.isDirectChat) {
      final otherParticipant = widget.conversation.getOtherParticipant(currentUserId);
      if (otherParticipant == null) return const SizedBox.shrink();

      return SingleChildScrollView(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Column(
          children: [
            ParticipantItemWidget(
              participant: otherParticipant,
              isAdmin: false,
              onTap: () => _viewParticipantProfile(otherParticipant),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.symmetric(vertical: AppDimensions.paddingMedium),
      itemCount: widget.conversation.participants.length + 1,
      itemBuilder: (context, index) {
        if (index == 0) {
          return Padding(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            child: ElevatedButton.icon(
              onPressed: _addParticipant,
              icon: const Icon(Icons.person_add_rounded),
              label: const Text('إضافة عضو'),
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.primary.withValues(alpha: 0.1),
                foregroundColor: AppColors.primary,
                elevation: 0,
              ),
            ),
          );
        }

        final participant = widget.conversation.participants[index - 1];
        final isAdmin = index == 1; // First participant is admin

        return ParticipantItemWidget(
          participant: participant,
          isAdmin: isAdmin,
          isCurrentUser: participant.id == currentUserId,
          onTap: () => _viewParticipantProfile(participant),
          onRemove: isAdmin && participant.id != currentUserId
              ? () => _removeParticipant(participant)
              : null,
        );
      },
    );
  }

  Widget _buildSettingsTab() {
    return BlocBuilder<ChatBloc, ChatState>(
      builder: (context, state) {
        if (state is ChatLoaded && state.settings != null) {
          _notificationsEnabled = state.settings!.notificationsEnabled;
          _showReadReceipts = state.settings!.showReadReceipts;
          _theme = state.settings!.theme;
        }

        return ListView(
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          children: [
            _buildSettingSection(
              title: 'الإشعارات',
              children: [
                SwitchListTile(
                  title: const Text('تفعيل الإشعارات'),
                  subtitle: const Text('تلقي إشعارات للرسائل الجديدة'),
                  value: _notificationsEnabled,
                  onChanged: (value) {
                    setState(() {
                      _notificationsEnabled = value;
                    });
                    _updateSettings();
                  },
                  activeColor: AppColors.primary,
                ),
                ListTile(
                  title: const Text('نغمة الإشعار'),
                  subtitle: const Text('افتراضي'),
                  trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
                  onTap: _changeNotificationSound,
                ),
              ],
            ),
            _buildSettingSection(
              title: 'الخصوصية',
              children: [
                SwitchListTile(
                  title: const Text('إيصالات القراءة'),
                  subtitle: const Text('السماح للآخرين بمعرفة متى قرأت رسائلهم'),
                  value: _showReadReceipts,
                  onChanged: (value) {
                    setState(() {
                      _showReadReceipts = value;
                    });
                    _updateSettings();
                  },
                  activeColor: AppColors.primary,
                ),
                ListTile(
                  title: const Text('حظر المستخدم'),
                  subtitle: const Text('منع هذا المستخدم من مراسلتك'),
                  trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
                  onTap: _blockUser,
                ),
              ],
            ),
            _buildSettingSection(
              title: 'المظهر',
              children: [
                ListTile(
                  title: const Text('خلفية المحادثة'),
                  subtitle: Text(_theme == 'default' ? 'افتراضي' : _theme),
                  trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
                  onTap: _changeChatTheme,
                ),
                ListTile(
                  title: const Text('حجم الخط'),
                  subtitle: const Text('متوسط'),
                  trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
                  onTap: _changeFontSize,
                ),
              ],
            ),
            _buildSettingSection(
              title: 'التخزين',
              children: [
                ListTile(
                  title: const Text('تنزيل الوسائط تلقائياً'),
                  subtitle: const Text('عند استخدام WiFi'),
                  trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
                  onTap: _configureAutoDownload,
                ),
                ListTile(
                  title: const Text('مسح ذاكرة التخزين المؤقت'),
                  subtitle: const Text('حذف الملفات المؤقتة'),
                  trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
                  onTap: _clearCache,
                ),
              ],
            ),
          ],
        );
      },
    );
  }

  Widget _buildSettingSection({
    required String title,
    required List<Widget> children,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMedium,
            vertical: AppDimensions.paddingSmall,
          ),
          child: Text(
            title,
            style: AppTextStyles.subtitle2.copyWith(
              color: AppColors.primary,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        Card(
          margin: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
          child: Column(children: children),
        ),
      ],
    );
  }

  void _handleMenuAction(String action) {
    switch (action) {
      case 'search':
        _searchInChat();
        break;
      case 'export':
        _exportChat();
        break;
      case 'leave':
        _leaveGroup();
        break;
      case 'clear':
        _clearChat();
        break;
    }
  }

  void _updateSettings() {
    context.read<ChatBloc>().add(
      UpdateChatSettingsEvent(
        notificationsEnabled: _notificationsEnabled,
        showReadReceipts: _showReadReceipts,
        theme: _theme,
      ),
    );
  }

  void _viewParticipantProfile(ChatUser participant) {
    // Navigate to participant profile
  }

  void _addParticipant() {
    // Show add participant dialog
  }

  void _removeParticipant(ChatUser participant) {
    // Show confirmation and remove participant
  }

  void _changeNotificationSound() {
    // Show notification sound picker
  }

  void _blockUser() {
    // Show block user confirmation
  }

  void _changeChatTheme() {
    // Show theme picker
  }

  void _changeFontSize() {
    // Show font size picker
  }

  void _configureAutoDownload() {
    // Show auto download settings
  }

  void _clearCache() {
    // Show clear cache confirmation
  }

  void _searchInChat() {
    // Navigate to search page
  }

  void _exportChat() {
    // Export chat history
  }

  void _leaveGroup() {
    // Show leave group confirmation
  }

  void _clearChat() {
    // Show clear chat confirmation
  }
}

class _InfoItem extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _InfoItem({
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Icon(
          icon,
          color: AppColors.primary,
          size: 24,
        ),
        const SizedBox(height: AppDimensions.spacingSm),
        Text(
          value,
          style: AppTextStyles.subtitle1.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }
}