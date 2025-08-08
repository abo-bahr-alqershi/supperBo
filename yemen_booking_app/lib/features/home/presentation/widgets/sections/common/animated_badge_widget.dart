// lib/features/home/presentation/widgets/sections/common/animated_badge_widget.dart

import 'package:flutter/material.dart';

class AnimatedBadgeWidget extends StatefulWidget {
  final String text;
  final Color backgroundColor;
  final Color textColor;
  final bool animate;

  const AnimatedBadgeWidget({
    super.key,
    required this.text,
    required this.backgroundColor,
    required this.textColor,
    this.animate = true,
  });

  @override
  State<AnimatedBadgeWidget> createState() => _AnimatedBadgeWidgetState();
}

class _AnimatedBadgeWidgetState extends State<AnimatedBadgeWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    );
    
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 1.1,
    ).animate(CurvedAnimation(
      parent: _controller,
      curve: Curves.easeInOut,
    ));
    
    if (widget.animate) {
      _controller.repeat(reverse: true);
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _controller,
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimation.value,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
            decoration: BoxDecoration(
              color: widget.backgroundColor,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Text(
              widget.text,
              style: TextStyle(
                color: widget.textColor,
                fontSize: 10,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        );
      },
    );
  }
}