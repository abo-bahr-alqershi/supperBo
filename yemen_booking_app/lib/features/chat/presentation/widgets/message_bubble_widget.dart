import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/message.dart';
import 'attachment_preview_widget.dart';
import 'message_status_indicator.dart';
import 'reaction_picker_widget.dart';

class MessageBubbleWidget extends StatefulWidget {
  final Message message;
  final bool isMe;
  final Message? previousMessage;
  final Message? nextMessage;
  final VoidCallback? onReply;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;
  final Function(String)? onReaction;

  const MessageBubbleWidget({
    super.key,
    required this.message,
    required this.isMe,
    this.previousMessage,
    this.nextMessage,
    this.onReply,
    this.onEdit,
    this.onDelete,
    this.onReaction,
  });

  @override
  State<MessageBubbleWidget> createState() => _MessageBubbleWidgetState();
}

class _MessageBubbleWidgetState extends State<MessageBubbleWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  bool _showReactions = false;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 200),
      vsync: this,
    );
    _scaleAnimation = Tween<double>(
      begin: 0.95,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOut,
    ));
    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final showTail = _shouldShowTail();
    final borderRadius = _getBorderRadius(showTail);

    return ScaleTransition(
      scale: _scaleAnimation,
      child: Padding(
        padding: EdgeInsets.only(
          left: widget.isMe ? 48 : 0,
          right: widget.isMe ? 0 : 48,
          top: _getTopPadding(),
          bottom: 4,
        ),
        child: Column(
          crossAxisAlignment: widget.isMe
              ? CrossAxisAlignment.end
              : CrossAxisAlignment.start,
          children: [
            GestureDetector(
              onLongPress: _showOptions,
              onDoubleTap: () {
                if (widget.onReaction != null) {
                  setState(() {
                    _showReactions = !_showReactions;
                  });
                }
              },
              child: Stack(
                children: [
                  Container(
                    constraints: BoxConstraints(
                      maxWidth: MediaQuery.of(context).size.width * 0.75,
                    ),
                    decoration: BoxDecoration(
                      color: widget.isMe
                          ? AppColors.primary
                          : AppColors.surface,
                      borderRadius: borderRadius,
                      boxShadow: [
                        BoxShadow(
                          color: AppColors.shadow.withValues(alpha: 0.1),
                          blurRadius: 4,
                          offset: const Offset(0, 2),
                        ),
                      ],
                    ),
                    child: ClipRRect(
                      borderRadius: borderRadius,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          if (widget.message.replyToMessageId != null)
                            _buildReplySection(),
                          _buildMessageContent(),
                          if (widget.message.attachments.isNotEmpty)
                            _buildAttachments(),
                          _buildFooter(),
                        ],
                      ),
                    ),
                  ),
                  if (showTail) _buildTail(),
                ],
              ),
            ),
            if (widget.message.reactions.isNotEmpty) _buildReactions(),
            if (_showReactions)
              ReactionPickerWidget(
                onReaction: (reaction) {
                  widget.onReaction?.call(reaction);
                  setState(() {
                    _showReactions = false;
                  });
                },
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildReplySection() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      padding: const EdgeInsets.all(AppDimensions.paddingSmall),
      decoration: BoxDecoration(
        color: widget.isMe
            ? Colors.white.withValues(alpha: 0.2)
            : AppColors.primary.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
      ),
      child: Row(
        children: [
          Container(
            width: 3,
            height: 30,
            decoration: BoxDecoration(
              color: widget.isMe
                  ? Colors.white.withValues(alpha: 0.5)
                  : AppColors.primary,
              borderRadius: BorderRadius.circular(1.5),
            ),
          ),
          const SizedBox(width: AppDimensions.spacingSm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'رد على رسالة',
                  style: AppTextStyles.caption.copyWith(
                    color: widget.isMe
                        ? Colors.white.withValues(alpha: 0.8)
                        : AppColors.primary,
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  'محتوى الرسالة المرد عليها',
                  style: AppTextStyles.bodySmall.copyWith(
                    color: widget.isMe
                        ? Colors.white.withValues(alpha: 0.7)
                        : AppColors.textSecondary,
                  ),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMessageContent() {
    if (widget.message.messageType == 'text' && widget.message.content != null) {
      return Padding(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        child: Text(
          widget.message.content!,
          style: AppTextStyles.bodyMedium.copyWith(
            color: widget.isMe ? Colors.white : AppColors.textPrimary,
            height: 1.4,
          ),
        ),
      );
    }

    if (widget.message.messageType == 'location' && widget.message.location != null) {
      return _buildLocationMessage();
    }

    return const SizedBox.shrink();
  }

  Widget _buildLocationMessage() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      height: 150,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
        image: DecorationImage(
          image: NetworkImage(
            'https://maps.googleapis.com/maps/api/staticmap?'
            'center=${widget.message.location!.latitude},${widget.message.location!.longitude}'
            '&zoom=15&size=300x150&markers=${widget.message.location!.latitude},${widget.message.location!.longitude}'
            '&key=YOUR_API_KEY',
          ),
          fit: BoxFit.cover,
        ),
      ),
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              Colors.transparent,
              Colors.black.withValues(alpha: 0.5),
            ],
          ),
        ),
        alignment: Alignment.bottomLeft,
        padding: const EdgeInsets.all(AppDimensions.paddingSmall),
        child: Row(
          children: [
            const Icon(
              Icons.location_on_rounded,
              color: Colors.white,
              size: 16,
            ),
            const SizedBox(width: AppDimensions.spacingXs),
            Expanded(
              child: Text(
                widget.message.location!.address ?? 'موقع',
                style: AppTextStyles.caption.copyWith(
                  color: Colors.white,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAttachments() {
    return Column(
      children: widget.message.attachments.map((attachment) {
        return AttachmentPreviewWidget(
          attachment: attachment,
          isMe: widget.isMe,
        );
      }).toList(),
    );
  }

  Widget _buildFooter() {
    return Padding(
      padding: const EdgeInsets.only(
        left: AppDimensions.paddingMedium,
        right: AppDimensions.paddingSmall,
        bottom: AppDimensions.paddingSmall,
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (widget.message.isEdited)
            Padding(
              padding: const EdgeInsets.only(right: AppDimensions.spacingXs),
              child: Text(
                'معدّل',
                style: AppTextStyles.caption.copyWith(
                  color: widget.isMe
                      ? Colors.white.withValues(alpha: 0.7)
                      : AppColors.textSecondary,
                  fontSize: 10,
                ),
              ),
            ),
          Text(
            _formatTime(widget.message.createdAt),
            style: AppTextStyles.caption.copyWith(
              color: widget.isMe
                  ? Colors.white.withValues(alpha: 0.7)
                  : AppColors.textSecondary,
              fontSize: 11,
            ),
          ),
          if (widget.isMe) ...[
            const SizedBox(width: AppDimensions.spacingXs),
            MessageStatusIndicator(
              status: widget.message.status,
              color: Colors.white.withValues(alpha: 0.7),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildReactions() {
    final groupedReactions = <String, int>{};
    for (final reaction in widget.message.reactions) {
      groupedReactions[reaction.reactionType] =
          (groupedReactions[reaction.reactionType] ?? 0) + 1;
    }

    return Padding(
      padding: const EdgeInsets.only(top: AppDimensions.spacingXs),
      child: Wrap(
        spacing: AppDimensions.spacingXs,
        children: groupedReactions.entries.map((entry) {
          return Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingSmall,
              vertical: AppDimensions.paddingXSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(AppDimensions.radiusCircular),
              border: Border.all(
                color: AppColors.border,
                width: 0.5,
              ),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  _getEmojiForReaction(entry.key),
                  style: const TextStyle(fontSize: 14),
                ),
                if (entry.value > 1) ...[
                  const SizedBox(width: 2),
                  Text(
                    entry.value.toString(),
                    style: AppTextStyles.caption.copyWith(
                      fontSize: 10,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ],
            ),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildTail() {
    return Positioned(
      bottom: 0,
      left: widget.isMe ? null : -8,
      right: widget.isMe ? -8 : null,
      child: CustomPaint(
        painter: _TailPainter(
          color: widget.isMe ? AppColors.primary : AppColors.surface,
          isMe: widget.isMe,
        ),
        size: const Size(10, 15),
      ),
    );
  }

  BorderRadius _getBorderRadius(bool showTail) {
    const radius = AppDimensions.radiusMedium;
    const smallRadius = AppDimensions.radiusXSmall;

    if (widget.isMe) {
      return BorderRadius.only(
        topLeft: const Radius.circular(radius),
        topRight: const Radius.circular(radius),
        bottomLeft: const Radius.circular(radius),
        bottomRight: Radius.circular(showTail ? smallRadius : radius),
      );
    } else {
      return BorderRadius.only(
        topLeft: const Radius.circular(radius),
        topRight: const Radius.circular(radius),
        bottomLeft: Radius.circular(showTail ? smallRadius : radius),
        bottomRight: const Radius.circular(radius),
      );
    }
  }

  bool _shouldShowTail() {
    if (widget.nextMessage == null) return true;
    if (widget.nextMessage!.senderId != widget.message.senderId) return true;
    
    final timeDiff = widget.message.createdAt
        .difference(widget.nextMessage!.createdAt)
        .inMinutes;
    return timeDiff > 1;
  }

  double _getTopPadding() {
    if (widget.previousMessage == null) return AppDimensions.spacingMd;
    if (widget.previousMessage!.senderId != widget.message.senderId) {
      return AppDimensions.spacingMd;
    }
    
    final timeDiff = widget.previousMessage!.createdAt
        .difference(widget.message.createdAt)
        .inMinutes;
    return timeDiff > 1 ? AppDimensions.spacingMd : AppDimensions.spacingXs;
  }

  void _showOptions() {
    HapticFeedback.mediumImpact();
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      builder: (context) => _MessageOptionsSheet(
        message: widget.message,
        isMe: widget.isMe,
        onReply: () {
          Navigator.pop(context);
          widget.onReply?.call();
        },
        onEdit: widget.onEdit != null
            ? () {
                Navigator.pop(context);
                widget.onEdit!();
              }
            : null,
        onDelete: widget.onDelete != null
            ? () {
                Navigator.pop(context);
                widget.onDelete!();
              }
            : null,
        onCopy: () {
          Navigator.pop(context);
          if (widget.message.content != null) {
            Clipboard.setData(ClipboardData(text: widget.message.content!));
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('تم النسخ'),
                duration: Duration(seconds: 1),
              ),
            );
          }
        },
      ),
    );
  }

  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }

  String _getEmojiForReaction(String reactionType) {
    switch (reactionType) {
      case 'like':
        return '👍';
      case 'love':
        return '❤️';
      case 'laugh':
        return '😂';
      case 'sad':
        return '😢';
      case 'angry':
        return '😠';
      case 'wow':
        return '😮';
      default:
        return '👍';
    }
  }
}

class _TailPainter extends CustomPainter {
  final Color color;
  final bool isMe;

  _TailPainter({
    required this.color,
    required this.isMe,
  });

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = color
      ..style = PaintingStyle.fill;

    final path = Path();

    if (isMe) {
      path.moveTo(0, 0);
      path.lineTo(0, size.height - 3);
      path.quadraticBezierTo(
        size.width / 2,
        size.height,
        size.width,
        size.height - 5,
      );
      path.lineTo(size.width, 0);
    } else {
      path.moveTo(size.width, 0);
      path.lineTo(size.width, size.height - 3);
      path.quadraticBezierTo(
        size.width / 2,
        size.height,
        0,
        size.height - 5,
      );
      path.lineTo(0, 0);
    }

    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _MessageOptionsSheet extends StatelessWidget {
  final Message message;
  final bool isMe;
  final VoidCallback? onReply;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;
  final VoidCallback? onCopy;

  const _MessageOptionsSheet({
    required this.message,
    required this.isMe,
    this.onReply,
    this.onEdit,
    this.onDelete,
    this.onCopy,
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
          if (onReply != null)
            ListTile(
              leading: const Icon(Icons.reply_rounded),
              title: const Text('رد'),
              onTap: onReply,
            ),
          if (message.content != null && onCopy != null)
            ListTile(
              leading: const Icon(Icons.copy_rounded),
              title: const Text('نسخ'),
              onTap: onCopy,
            ),
          if (isMe && onEdit != null)
            ListTile(
              leading: const Icon(Icons.edit_rounded),
              title: const Text('تعديل'),
              onTap: onEdit,
            ),
          if (isMe && onDelete != null)
            ListTile(
              leading: const Icon(
                Icons.delete_rounded,
                color: AppColors.error,
              ),
              title: Text(
                'حذف',
                style: AppTextStyles.bodyLarge.copyWith(
                  color: AppColors.error,
                ),
              ),
              onTap: onDelete,
            ),
          const SizedBox(height: AppDimensions.paddingMedium),
        ],
      ),
    );
  }
}