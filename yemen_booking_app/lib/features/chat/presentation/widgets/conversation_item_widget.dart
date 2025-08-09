import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/utils/date_utils.dart' as app_date;
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/conversation.dart';

class ConversationItemWidget extends StatelessWidget {
  final Conversation conversation;
  final String currentUserId;
  final List<String> typingUserIds;
  final VoidCallback onTap;
  final VoidCallback? onLongPress;

  const ConversationItemWidget({
    super.key,
    required this.conversation,
    required this.currentUserId,
    this.typingUserIds = const [],
    required this.onTap,
    this.onLongPress,
  });

  @override
  Widget build(BuildContext context) {
    final otherParticipant = conversation.isDirectChat
        ? conversation.getOtherParticipant(currentUserId)
        : null;

    final displayName = conversation.title ??
        otherParticipant?.name ??
        'محادثة';

    final displayImage = conversation.avatar ??
        otherParticipant?.profileImage;

    final isTyping = typingUserIds.isNotEmpty;

    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: () {
          HapticFeedback.lightImpact();
          onTap();
        },
        onLongPress: onLongPress != null ? () {
          HapticFeedback.mediumImpact();
          onLongPress!();
        } : null,
        child: Container(
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMedium,
            vertical: AppDimensions.paddingSmall,
          ),
          decoration: BoxDecoration(
            border: Border(
              bottom: BorderSide(
                color: AppColors.divider.withValues(alpha: 0.5),
                width: 0.5,
              ),
            ),
          ),
          child: Row(
            children: [
              _buildAvatar(displayImage, displayName),
              const SizedBox(width: AppDimensions.spacingMd),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            displayName,
                            style: AppTextStyles.subtitle2.copyWith(
                              fontWeight: conversation.hasUnreadMessages
                                  ? FontWeight.w700
                                  : FontWeight.w500,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        if (conversation.isMuted)
                          Padding(
                            padding: const EdgeInsets.only(
                              left: AppDimensions.spacingXs,
                            ),
                            child: Icon(
                              Icons.notifications_off_rounded,
                              size: 14,
                              color: AppColors.textSecondary.withValues(alpha: 0.5),
                            ),
                          ),
                        const SizedBox(width: AppDimensions.spacingSm),
                        Text(
                          _formatTime(conversation.lastMessage?.createdAt ??
                              conversation.updatedAt),
                          style: AppTextStyles.caption.copyWith(
                            color: conversation.hasUnreadMessages
                                ? AppColors.primary
                                : AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: AppDimensions.spacingXs),
                    Row(
                      children: [
                        if (conversation.lastMessage != null &&
                            conversation.lastMessage!.senderId == currentUserId)
                          _buildMessageStatus(conversation.lastMessage!.status),
                        Expanded(
                          child: isTyping
                              ? _buildTypingIndicator()
                              : _buildLastMessage(),
                        ),
                        if (conversation.hasUnreadMessages)
                          _buildUnreadBadge(),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildAvatar(String? imageUrl, String name) {
    return Stack(
      children: [
        Container(
          width: 56,
          height: 56,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: AppColors.primary.withValues(alpha: 0.1),
          ),
          child: imageUrl != null
              ? ClipOval(
                  child: CachedImageWidget(
                    imageUrl: imageUrl,
                    width: 56,
                    height: 56,
                    fit: BoxFit.cover,
                  ),
                )
              : Center(
                  child: Text(
                    _getInitials(name),
                    style: AppTextStyles.subtitle1.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
        ),
        if (conversation.isDirectChat)
          Positioned(
            bottom: 0,
            right: 0,
            child: _buildOnlineIndicator(),
          ),
      ],
    );
  }

  Widget _buildOnlineIndicator() {
    final otherParticipant = conversation.getOtherParticipant(currentUserId);
    if (otherParticipant == null || !otherParticipant.isOnline) {
      return const SizedBox.shrink();
    }

    return Container(
      width: 14,
      height: 14,
      decoration: BoxDecoration(
        color: AppColors.success,
        shape: BoxShape.circle,
        border: Border.all(
          color: AppColors.surface,
          width: 2,
        ),
      ),
    );
  }

  Widget _buildMessageStatus(String status) {
    IconData icon;
    Color color;

    switch (status) {
      case 'sent':
        icon = Icons.check_rounded;
        color = AppColors.textSecondary;
        break;
      case 'delivered':
        icon = Icons.done_all_rounded;
        color = AppColors.textSecondary;
        break;
      case 'read':
        icon = Icons.done_all_rounded;
        color = AppColors.primary;
        break;
      case 'failed':
        icon = Icons.error_outline_rounded;
        color = AppColors.error;
        break;
      default:
        icon = Icons.schedule_rounded;
        color = AppColors.textSecondary;
    }

    return Padding(
      padding: const EdgeInsets.only(right: AppDimensions.spacingXs),
      child: Icon(
        icon,
        size: 16,
        color: color,
      ),
    );
  }

  Widget _buildLastMessage() {
    if (conversation.lastMessage == null) {
      return Text(
        'ابدأ المحادثة',
        style: AppTextStyles.bodyMedium.copyWith(
          color: AppColors.textSecondary,
          fontStyle: FontStyle.italic,
        ),
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
      );
    }

    String messageText = '';
    Widget? prefix;

    switch (conversation.lastMessage!.messageType) {
      case 'text':
        messageText = conversation.lastMessage!.content ?? '';
        break;
      case 'image':
        prefix = const Icon(Icons.image_rounded, size: 16);
        messageText = 'صورة';
        break;
      case 'video':
        prefix = const Icon(Icons.videocam_rounded, size: 16);
        messageText = 'فيديو';
        break;
      case 'audio':
        prefix = const Icon(Icons.mic_rounded, size: 16);
        messageText = 'رسالة صوتية';
        break;
      case 'document':
        prefix = const Icon(Icons.attach_file_rounded, size: 16);
        messageText = 'مستند';
        break;
      case 'location':
        prefix = const Icon(Icons.location_on_rounded, size: 16);
        messageText = 'موقع';
        break;
    }

    return Row(
      children: [
        if (prefix != null) ...[
          prefix,
          const SizedBox(width: AppDimensions.spacingXs),
        ],
        Expanded(
          child: Text(
            messageText,
            style: AppTextStyles.bodyMedium.copyWith(
              color: conversation.hasUnreadMessages
                  ? AppColors.textPrimary
                  : AppColors.textSecondary,
              fontWeight: conversation.hasUnreadMessages
                  ? FontWeight.w500
                  : FontWeight.normal,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ),
      ],
    );
  }

  Widget _buildTypingIndicator() {
    return Row(
      children: [
        Text(
          'يكتب',
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.primary,
            fontStyle: FontStyle.italic,
          ),
        ),
        const SizedBox(width: AppDimensions.spacingXs),
        SizedBox(
          width: 20,
          height: 10,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: List.generate(3, (index) {
              return TweenAnimationBuilder<double>(
                tween: Tween(begin: 0, end: 1),
                duration: Duration(milliseconds: 600 + (index * 200)),
                builder: (context, value, child) {
                  return Container(
                    width: 4,
                    height: 4,
                    decoration: BoxDecoration(
                      color: AppColors.primary.withValues(alpha: value),
                      shape: BoxShape.circle,
                    ),
                  );
                },
                onEnd: () {},
              );
            }),
          ),
        ),
      ],
    );
  }

  Widget _buildUnreadBadge() {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
        vertical: AppDimensions.paddingXSmall,
      ),
      decoration: BoxDecoration(
        color: AppColors.primary,
        borderRadius: BorderRadius.circular(AppDimensions.radiusCircular),
      ),
      constraints: const BoxConstraints(
        minWidth: 20,
        minHeight: 20,
      ),
      child: Center(
        child: Text(
          conversation.unreadCount > 99
              ? '99+'
              : conversation.unreadCount.toString(),
          style: AppTextStyles.caption.copyWith(
            color: Colors.white,
            fontWeight: FontWeight.w600,
            fontSize: 10,
          ),
        ),
      ),
    );
  }

  String _formatTime(DateTime dateTime) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final yesterday = today.subtract(const Duration(days: 1));
    final messageDate = DateTime(
      dateTime.year,
      dateTime.month,
      dateTime.day,
    );

    if (messageDate == today) {
      return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
    } else if (messageDate == yesterday) {
      return 'أمس';
    } else if (now.difference(dateTime).inDays < 7) {
      final days = ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];
      return days[dateTime.weekday % 7];
    } else {
      return '${dateTime.day}/${dateTime.month}';
    }
  }

  String _getInitials(String name) {
    final parts = name.trim().split(' ');
    if (parts.isEmpty) return '';
    if (parts.length == 1) return parts.first[0].toUpperCase();
    return '${parts.first[0]}${parts.last[0]}'.toUpperCase();
  }
}