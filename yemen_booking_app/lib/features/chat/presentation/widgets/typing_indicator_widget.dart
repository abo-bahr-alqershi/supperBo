import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/conversation.dart';

class TypingIndicatorWidget extends StatefulWidget {
  final List<String> typingUserIds;
  final Conversation conversation;

  const TypingIndicatorWidget({
    super.key,
    required this.typingUserIds,
    required this.conversation,
  });

  @override
  State<TypingIndicatorWidget> createState() => _TypingIndicatorWidgetState();
}

class _TypingIndicatorWidgetState extends State<TypingIndicatorWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late List<Animation<double>> _dotAnimations;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 1500),
      vsync: this,
    )..repeat();

    _dotAnimations = List.generate(3, (index) {
      return Tween<double>(
        begin: 0.0,
        end: 1.0,
      ).animate(
        CurvedAnimation(
          parent: _animationController,
          curve: Interval(
            index * 0.2,
            0.6 + index * 0.2,
            curve: Curves.easeInOut,
          ),
        ),
      );
    });
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.typingUserIds.isEmpty) {
      return const SizedBox.shrink();
    }

    final typingUsers = widget.conversation.participants
        .where((p) => widget.typingUserIds.contains(p.id))
        .toList();

    final typingText = _getTypingText(typingUsers);

    return Container(
      margin: const EdgeInsets.only(
        left: 0,
        right: 48,
        bottom: AppDimensions.spacingSm,
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
              vertical: AppDimensions.paddingSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(AppDimensions.radiusMedium),
              boxShadow: [
                BoxShadow(
                  color: AppColors.shadow.withValues(alpha: 0.1),
                  blurRadius: 4,
                  offset: const Offset(0, 2),
                ),
              ],
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                _buildDots(),
                const SizedBox(width: AppDimensions.spacingSm),
                Text(
                  typingText,
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                    fontStyle: FontStyle.italic,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDots() {
    return SizedBox(
      width: 30,
      height: 10,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
        children: List.generate(3, (index) {
          return AnimatedBuilder(
            animation: _dotAnimations[index],
            builder: (context, child) {
              return Transform.translate(
                offset: Offset(0, -4 * _dotAnimations[index].value),
                child: Container(
                  width: 6,
                  height: 6,
                  decoration: BoxDecoration(
                    color: AppColors.primary.withValues(
                      alpha: 0.3 + 0.7 * _dotAnimations[index].value,
                    ),
                    shape: BoxShape.circle,
                  ),
                ),
              );
            },
          );
        }),
      ),
    );
  }

  String _getTypingText(List<ChatUser> typingUsers) {
    if (typingUsers.isEmpty) return '';
    
    if (typingUsers.length == 1) {
      return '${typingUsers.first.name} يكتب';
    } else if (typingUsers.length == 2) {
      return '${typingUsers.first.name} و${typingUsers.last.name} يكتبان';
    } else {
      return 'عدة أشخاص يكتبون';
    }
  }
}