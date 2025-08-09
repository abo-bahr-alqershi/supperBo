import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';

class OnlineStatusIndicator extends StatelessWidget {
  final bool isOnline;
  final double size;
  final bool showBorder;

  const OnlineStatusIndicator({
    super.key,
    required this.isOnline,
    this.size = 12,
    this.showBorder = true,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: isOnline ? AppColors.success : AppColors.textSecondary,
        shape: BoxShape.circle,
        border: showBorder
            ? Border.all(
                color: Theme.of(context).cardColor,
                width: 2,
              )
            : null,
      ),
    );
  }
}