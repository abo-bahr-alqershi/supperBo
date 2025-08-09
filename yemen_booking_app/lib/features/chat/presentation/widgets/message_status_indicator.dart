import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';

class MessageStatusIndicator extends StatelessWidget {
  final String status;
  final Color? color;
  final double size;

  const MessageStatusIndicator({
    super.key,
    required this.status,
    this.color,
    this.size = 16,
  });

  @override
  Widget build(BuildContext context) {
    IconData iconData;
    Color iconColor;

    switch (status) {
      case 'sending':
        iconData = Icons.schedule_rounded;
        iconColor = color ?? AppColors.textSecondary;
        break;
      case 'sent':
        iconData = Icons.check_rounded;
        iconColor = color ?? AppColors.textSecondary;
        break;
      case 'delivered':
        iconData = Icons.done_all_rounded;
        iconColor = color ?? AppColors.textSecondary;
        break;
      case 'read':
        iconData = Icons.done_all_rounded;
        iconColor = color ?? AppColors.primary;
        break;
      case 'failed':
        iconData = Icons.error_outline_rounded;
        iconColor = AppColors.error;
        break;
      default:
        iconData = Icons.check_rounded;
        iconColor = color ?? AppColors.textSecondary;
    }

    return Icon(
      iconData,
      size: size,
      color: iconColor,
    );
  }
}