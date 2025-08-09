import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/conversation.dart';

class ChatAppBar extends StatelessWidget implements PreferredSizeWidget {
  final Conversation conversation;
  final String currentUserId;
  final VoidCallback onBackPressed;
  final VoidCallback onInfoPressed;
  final VoidCallback? onCallPressed;
  final VoidCallback? onVideoCallPressed;

  const ChatAppBar({
    super.key,
    required this.conversation,
    required this.currentUserId,
    required this.onBackPressed,
    required this.onInfoPressed,
    this.onCallPressed,
    this.onVideoCallPressed,
  });

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);

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

    final statusText = _getStatusText();

    return Container(
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
      child: SafeArea(
        child: Container(
          height: kToolbarHeight,
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingSmall,
          ),
          child: Row(
            children: [
              IconButton(
                onPressed: () {
                  HapticFeedback.lightImpact();
                  onBackPressed();
                },
                icon: const Icon(
                  Icons.arrow_back_ios_new_rounded,
                  size: 22,
                ),
              ),
              GestureDetector(
                onTap: () {
                  HapticFeedback.lightImpact();
                  onInfoPressed();
                },
                child: Row(
                  children: [
                    _buildAvatar(displayImage, displayName),
                    const SizedBox(width: AppDimensions.spacingMd),
                    Expanded(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            displayName,
                            style: AppTextStyles.subtitle1.copyWith(
                              fontWeight: FontWeight.w600,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                          if (statusText != null)
                            Text(
                              statusText,
                              style: AppTextStyles.caption.copyWith(
                                color: _getStatusColor(),
                              ),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const Spacer(),
              if (onCallPressed != null)
                IconButton(
                  onPressed: () {
                    HapticFeedback.lightImpact();
                    onCallPressed!();
                  },
                  icon: const Icon(
                    Icons.call_rounded,
                    size: 24,
                  ),
                ),
              if (onVideoCallPressed != null)
                IconButton(
                  onPressed: () {
                    HapticFeedback.lightImpact();
                    onVideoCallPressed!();
                  },
                  icon: const Icon(
                    Icons.videocam_rounded,
                    size: 24,
                  ),
                ),
              IconButton(
                onPressed: () {
                  HapticFeedback.lightImpact();
                  onInfoPressed();
                },
                icon: const Icon(
                  Icons.more_vert_rounded,
                  size: 24,
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
          width: 40,
          height: 40,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: AppColors.primary.withValues(alpha: 0.1),
          ),
          child: imageUrl != null
              ? ClipOval(
                  child: CachedImageWidget(
                    imageUrl: imageUrl,
                    width: 40,
                    height: 40,
                    fit: BoxFit.cover,
                  ),
                )
              : Center(
                  child: Text(
                    _getInitials(name),
                    style: AppTextStyles.bodyMedium.copyWith(
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
      width: 10,
      height: 10,
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

  String? _getStatusText() {
    if (conversation.isDirectChat) {
      final otherParticipant = conversation.getOtherParticipant(currentUserId);
      if (otherParticipant == null) return null;
      
      if (otherParticipant.isOnline) {
        return 'متصل الآن';
      } else if (otherParticipant.lastSeen != null) {
        return 'آخر ظهور ${_formatLastSeen(otherParticipant.lastSeen!)}';
      }
    } else {
      final onlineCount = conversation.participants
          .where((p) => p.isOnline && p.id != currentUserId)
          .length;
      if (onlineCount > 0) {
        return '$onlineCount متصل';
      }
    }
    return null;
  }

  Color _getStatusColor() {
    final otherParticipant = conversation.isDirectChat
        ? conversation.getOtherParticipant(currentUserId)
        : null;
    
    if (otherParticipant?.isOnline == true) {
      return AppColors.success;
    }
    return AppColors.textSecondary;
  }

  String _formatLastSeen(DateTime lastSeen) {
    final now = DateTime.now();
    final difference = now.difference(lastSeen);
    
    if (difference.inMinutes < 1) {
      return 'الآن';
    } else if (difference.inMinutes < 60) {
      return 'منذ ${difference.inMinutes} دقيقة';
    } else if (difference.inHours < 24) {
      return 'منذ ${difference.inHours} ساعة';
    } else if (difference.inDays == 1) {
      return 'أمس';
    } else {
      return 'منذ ${difference.inDays} يوم';
    }
  }

  String _getInitials(String name) {
    final parts = name.trim().split(' ');
    if (parts.isEmpty) return '';
    if (parts.length == 1) return parts.first[0].toUpperCase();
    return '${parts.first[0]}${parts.last[0]}'.toUpperCase();
  }
}