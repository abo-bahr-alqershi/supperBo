import 'package:flutter/material.dart';
import '../theme/app_colors.dart';
import '../theme/app_dimensions.dart';
import '../theme/app_text_styles.dart';
import '../utils/color_extensions.dart';

class CustomErrorWidget extends StatelessWidget {
  final String? message;
  final String? title;
  final VoidCallback? onRetry;
  final Widget? icon;
  final ErrorType type;

  const CustomErrorWidget({
    super.key,
    this.message,
    this.title,
    this.onRetry,
    this.icon,
    this.type = ErrorType.general,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      child: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            _buildIcon(context),
            const SizedBox(height: AppDimensions.spacingLg),
            _buildTitle(context),
            if (message != null) ...[
              const SizedBox(height: AppDimensions.spacingSm),
              _buildMessage(context),
            ],
            if (onRetry != null) ...[
              const SizedBox(height: AppDimensions.spacingXl),
              _buildRetryButton(context),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildIcon(BuildContext context) {
    if (icon != null) return icon!;

    IconData iconData;
    Color iconColor;

    switch (type) {
      case ErrorType.network:
        iconData = Icons.wifi_off_rounded;
        iconColor = AppColors.error;
        break;
      case ErrorType.server:
        iconData = Icons.cloud_off_rounded;
        iconColor = AppColors.error;
        break;
      case ErrorType.notFound:
        iconData = Icons.search_off_rounded;
        iconColor = AppColors.textSecondary;
        break;
      case ErrorType.permission:
        iconData = Icons.lock_outline_rounded;
        iconColor = AppColors.warning;
        break;
      case ErrorType.general:
      default:
        iconData = Icons.error_outline_rounded;
        iconColor = AppColors.error;
        break;
    }

    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      decoration: BoxDecoration(
        color: iconColor.withValues(alpha: 0.1),
        shape: BoxShape.circle,
      ),
      child: Icon(
        iconData,
        size: AppDimensions.iconXLarge,
        color: iconColor,
      ),
    );
  }

  Widget _buildTitle(BuildContext context) {
    String displayTitle = title ?? _getDefaultTitle();
    
    return Text(
      displayTitle,
      style: AppTextStyles.heading3.copyWith(
        color: AppColors.textPrimary,
      ),
      textAlign: TextAlign.center,
    );
  }

  Widget _buildMessage(BuildContext context) {
    return Text(
      message!,
      style: AppTextStyles.bodyMedium.copyWith(
        color: AppColors.textSecondary,
      ),
      textAlign: TextAlign.center,
      maxLines: 3,
      overflow: TextOverflow.ellipsis,
    );
  }

  Widget _buildRetryButton(BuildContext context) {
    return ElevatedButton.icon(
      onPressed: onRetry,
      icon: const Icon(Icons.refresh_rounded),
      label: const Text('إعادة المحاولة'),
      style: ElevatedButton.styleFrom(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingLarge,
          vertical: AppDimensions.paddingMedium,
        ),
      ),
    );
  }

  String _getDefaultTitle() {
    switch (type) {
      case ErrorType.network:
        return 'لا يوجد اتصال بالإنترنت';
      case ErrorType.server:
        return 'خطأ في الخادم';
      case ErrorType.notFound:
        return 'لم يتم العثور على النتائج';
      case ErrorType.permission:
        return 'ليس لديك صلاحية';
      case ErrorType.general:
      default:
        return 'حدث خطأ ما';
    }
  }
}

enum ErrorType {
  general,
  network,
  server,
  notFound,
  permission,
}