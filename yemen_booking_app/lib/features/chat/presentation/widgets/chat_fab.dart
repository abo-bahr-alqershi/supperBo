import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';

class ChatFAB extends StatefulWidget {
  final VoidCallback onPressed;

  const ChatFAB({
    super.key,
    required this.onPressed,
  });

  @override
  State<ChatFAB> createState() => _ChatFABState();
}

class _ChatFABState extends State<ChatFAB> with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  late Animation<double> _rotationAnimation;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 200),
      vsync: this,
    );

    _scaleAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.elasticOut,
    ));

    _rotationAnimation = Tween<double>(
      begin: 0.0,
      end: 0.5,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
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
    return AnimatedBuilder(
      animation: _animationController,
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimation.value,
          child: Transform.rotate(
            angle: _rotationAnimation.value * 3.14159,
            child: FloatingActionButton(
              onPressed: () {
                HapticFeedback.mediumImpact();
                widget.onPressed();
              },
              backgroundColor: AppColors.primary,
              elevation: 6,
              child: const Icon(
                Icons.message_rounded,
                color: Colors.white,
                size: 24,
              ),
            ),
          ),
        );
      },
    );
  }
}