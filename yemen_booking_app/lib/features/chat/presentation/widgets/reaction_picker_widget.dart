import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';

class ReactionPickerWidget extends StatefulWidget {
  final Function(String) onReaction;

  const ReactionPickerWidget({
    super.key,
    required this.onReaction,
  });

  @override
  State<ReactionPickerWidget> createState() => _ReactionPickerWidgetState();
}

class _ReactionPickerWidgetState extends State<ReactionPickerWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late List<Animation<double>> _scaleAnimations;

  final List<ReactionItem> reactions = [
    ReactionItem(emoji: '👍', type: 'like'),
    ReactionItem(emoji: '❤️', type: 'love'),
    ReactionItem(emoji: '😂', type: 'laugh'),
    ReactionItem(emoji: '😮', type: 'wow'),
    ReactionItem(emoji: '😢', type: 'sad'),
    ReactionItem(emoji: '😠', type: 'angry'),
  ];

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );

    _scaleAnimations = List.generate(reactions.length, (index) {
      return Tween<double>(
        begin: 0.0,
        end: 1.0,
      ).animate(
        CurvedAnimation(
          parent: _animationController,
          curve: Interval(
            index * 0.1,
            0.5 + index * 0.1,
            curve: Curves.elasticOut,
          ),
        ),
      );
    });

    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(top: AppDimensions.spacingSm),
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMedium,
        vertical: AppDimensions.paddingSmall,
      ),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.radiusLarge),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow.withValues(alpha: 0.2),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: List.generate(reactions.length, (index) {
          return AnimatedBuilder(
            animation: _scaleAnimations[index],
            builder: (context, child) {
              return Transform.scale(
                scale: _scaleAnimations[index].value,
                child: _ReactionButton(
                  reaction: reactions[index],
                  onTap: () {
                    HapticFeedback.lightImpact();
                    widget.onReaction(reactions[index].type);
                  },
                ),
              );
            },
          );
        }),
      ),
    );
  }
}

class _ReactionButton extends StatefulWidget {
  final ReactionItem reaction;
  final VoidCallback onTap;

  const _ReactionButton({
    required this.reaction,
    required this.onTap,
  });

  @override
  State<_ReactionButton> createState() => _ReactionButtonState();
}

class _ReactionButtonState extends State<_ReactionButton>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 150),
      vsync: this,
    );
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 1.3,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    ));
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTapDown: (_) => _animationController.forward(),
      onTapUp: (_) {
        _animationController.reverse();
        widget.onTap();
      },
      onTapCancel: () => _animationController.reverse(),
      child: AnimatedBuilder(
        animation: _scaleAnimation,
        builder: (context, child) {
          return Transform.scale(
            scale: _scaleAnimation.value,
            child: Container(
              padding: const EdgeInsets.all(AppDimensions.paddingSmall),
              child: Text(
                widget.reaction.emoji,
                style: const TextStyle(fontSize: 24),
              ),
            ),
          );
        },
      ),
    );
  }
}

class ReactionItem {
  final String emoji;
  final String type;

  ReactionItem({
    required this.emoji,
    required this.type,
  });
}