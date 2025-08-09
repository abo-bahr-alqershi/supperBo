/// features/payment/presentation/widgets/transaction_item_widget.dart

import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/enums/payment_method_enum.dart';
import '../../domain/entities/transaction.dart';

class TransactionItemWidget extends StatelessWidget {
  final Transaction transaction;
  final VoidCallback onTap;

  const TransactionItemWidget({
    super.key,
    required this.transaction,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        margin: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          boxShadow: const [
            BoxShadow(
              color: AppColors.shadow,
              blurRadius: 4,
              offset: Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          children: [
            _buildHeader(),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildDetails(),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildFooter(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      children: [
        _buildStatusIcon(),
        const SizedBox(width: AppDimensions.spacingMd),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                transaction.propertyName,
                style: AppTextStyles.subtitle2.copyWith(
                  fontWeight: FontWeight.bold,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: AppDimensions.spacingXs),
              Text(
                transaction.unitName,
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ),
        ),
        _buildAmount(),
      ],
    );
  }

  Widget _buildStatusIcon() {
    final color = _getStatusColor();
    final icon = _getStatusIcon();
    
    return Container(
      width: 48,
      height: 48,
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Center(
        child: Icon(
          icon,
          color: color,
          size: AppDimensions.iconMedium,
        ),
      ),
    );
  }

  Widget _buildAmount() {
    final isRefund = transaction.status == PaymentStatus.refunded ||
        transaction.status == PaymentStatus.partiallyRefunded;
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Row(
          children: [
            if (isRefund)
              Text(
                '-',
                style: AppTextStyles.subtitle1.copyWith(
                  fontWeight: FontWeight.bold,
                  color: AppColors.error,
                ),
              ),
            Text(
              '${transaction.amount.toStringAsFixed(0)}',
              style: AppTextStyles.subtitle1.copyWith(
                fontWeight: FontWeight.bold,
                color: isRefund ? AppColors.error : AppColors.textPrimary,
              ),
            ),
            const SizedBox(width: AppDimensions.spacingXs),
            Text(
              transaction.currency,
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
        if (transaction.fees > 0)
          Text(
            '+ ${transaction.fees.toStringAsFixed(0)} رسوم',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
      ],
    );
  }

  Widget _buildDetails() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingSmall),
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          _buildDetailItem(
            icon: Icons.confirmation_number,
            label: 'رقم الحجز',
            value: transaction.bookingNumber,
          ),
          Container(
            width: 1,
            height: 30,
            color: AppColors.divider,
          ),
          _buildDetailItem(
            icon: _getPaymentMethodIcon(),
            label: 'طريقة الدفع',
            value: transaction.paymentMethod.displayNameAr,
          ),
        ],
      ),
    );
  }

  Widget _buildDetailItem({
    required IconData icon,
    required String label,
    required String value,
  }) {
    return Expanded(
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            icon,
            size: AppDimensions.iconSmall,
            color: AppColors.textSecondary,
          ),
          const SizedBox(width: AppDimensions.spacingXs),
          Flexible(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
                Text(
                  value,
                  style: AppTextStyles.caption.copyWith(
                    fontWeight: FontWeight.bold,
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

  Widget _buildFooter() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Row(
          children: [
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingSmall,
                vertical: AppDimensions.paddingXSmall,
              ),
              decoration: BoxDecoration(
                color: _getStatusColor().withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
                border: Border.all(
                  color: _getStatusColor().withValues(alpha: 0.3),
                ),
              ),
              child: Text(
                transaction.status.displayNameAr,
                style: AppTextStyles.caption.copyWith(
                  color: _getStatusColor(),
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            if (transaction.canRefund) ...[
              const SizedBox(width: AppDimensions.spacingSm),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingSmall,
                  vertical: AppDimensions.paddingXSmall,
                ),
                decoration: BoxDecoration(
                  color: AppColors.warning.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.replay,
                      size: AppDimensions.iconXSmall,
                      color: AppColors.warning,
                    ),
                    const SizedBox(width: AppDimensions.spacingXs),
                    Text(
                      'قابل للاسترداد',
                      style: AppTextStyles.caption.copyWith(
                        color: AppColors.warning,
                        fontSize: 10,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
        Row(
          children: [
            Icon(
              Icons.access_time,
              size: AppDimensions.iconXSmall,
              color: AppColors.textSecondary,
            ),
            const SizedBox(width: AppDimensions.spacingXs),
            Text(
              _formatDate(transaction.createdAt),
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Color _getStatusColor() {
    switch (transaction.status) {
      case PaymentStatus.successful:
        return AppColors.success;
      case PaymentStatus.failed:
        return AppColors.error;
      case PaymentStatus.pending:
        return AppColors.warning;
      case PaymentStatus.refunded:
      case PaymentStatus.partiallyRefunded:
        return AppColors.info;
      case PaymentStatus.voided:
        return AppColors.textSecondary;
    }
  }

  IconData _getStatusIcon() {
    switch (transaction.status) {
      case PaymentStatus.successful:
        return Icons.check_circle;
      case PaymentStatus.failed:
        return Icons.cancel;
      case PaymentStatus.pending:
        return Icons.hourglass_empty;
      case PaymentStatus.refunded:
      case PaymentStatus.partiallyRefunded:
        return Icons.replay;
      case PaymentStatus.voided:
        return Icons.block;
    }
  }

  IconData _getPaymentMethodIcon() {
    switch (transaction.paymentMethod) {
      case PaymentMethod.creditCard:
        return Icons.credit_card;
      case PaymentMethod.cash:
        return Icons.money;
      case PaymentMethod.paypal:
        return Icons.payment;
      default:
        return Icons.account_balance_wallet;
    }
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final dateToCheck = DateTime(date.year, date.month, date.day);
    
    if (dateToCheck == today) {
      return 'اليوم ${DateFormat('HH:mm').format(date)}';
    } else if (dateToCheck == today.subtract(const Duration(days: 1))) {
      return 'أمس ${DateFormat('HH:mm').format(date)}';
    } else if (date.year == now.year) {
      return DateFormat('dd MMM - HH:mm', 'ar').format(date);
    } else {
      return DateFormat('dd/MM/yyyy', 'ar').format(date);
    }
  }
}