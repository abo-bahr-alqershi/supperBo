/// features/payment/presentation/widgets/payment_method_card_widget.dart

import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/enums/payment_method_enum.dart';

class PaymentMethodCardWidget extends StatelessWidget {
  final PaymentMethod paymentMethod;
  final bool isSaved;
  final VoidCallback onTap;
  final VoidCallback? onDelete;
  final String? lastFourDigits;
  final bool isSelected;

  const PaymentMethodCardWidget({
    super.key,
    required this.paymentMethod,
    required this.isSaved,
    required this.onTap,
    this.onDelete,
    this.lastFourDigits,
    this.isSelected = false,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: isSelected
              ? AppColors.primary.withValues(alpha: 0.05)
              : AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(
            color: isSelected ? AppColors.primary : AppColors.outline,
            width: isSelected ? 2 : 1,
          ),
          boxShadow: isSelected
              ? [
                  BoxShadow(
                    color: AppColors.primary.withValues(alpha: 0.2),
                    blurRadius: 8,
                    offset: const Offset(0, 2),
                  ),
                ]
              : [
                  const BoxShadow(
                    color: AppColors.shadow,
                    blurRadius: 4,
                    offset: Offset(0, 2),
                  ),
                ],
        ),
        child: Row(
          children: [
            _buildIcon(),
            const SizedBox(width: AppDimensions.spacingMd),
            Expanded(child: _buildDetails()),
            if (isSaved && onDelete != null) _buildDeleteButton(),
            if (isSelected) _buildSelectedIndicator(),
          ],
        ),
      ),
    );
  }

  Widget _buildIcon() {
    return Container(
      width: 56,
      height: 56,
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            _getMethodColor().withValues(alpha: 0.2),
            _getMethodColor().withValues(alpha: 0.1),
          ],
        ),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Center(
        child: Icon(
          _getMethodIcon(),
          color: _getMethodColor(),
          size: AppDimensions.iconMedium,
        ),
      ),
    );
  }

  Widget _buildDetails() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Text(
              paymentMethod.displayNameAr,
              style: AppTextStyles.subtitle2.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            if (isSaved) ...[
              const SizedBox(width: AppDimensions.spacingSm),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingSmall,
                  vertical: 2,
                ),
                decoration: BoxDecoration(
                  color: AppColors.success.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
                  border: Border.all(
                    color: AppColors.success.withValues(alpha: 0.3),
                  ),
                ),
                child: Text(
                  'محفوظة',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.success,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ],
        ),
        const SizedBox(height: AppDimensions.spacingXs),
        Text(
          _getMethodDescription(),
          style: AppTextStyles.caption.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        if (lastFourDigits != null) ...[
          const SizedBox(height: AppDimensions.spacingXs),
          Row(
            children: [
              Text(
                '•••• •••• •••• ',
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                  letterSpacing: 2,
                ),
              ),
              Text(
                lastFourDigits!,
                style: AppTextStyles.caption.copyWith(
                  fontWeight: FontWeight.bold,
                  letterSpacing: 1,
                ),
              ),
            ],
          ),
        ],
      ],
    );
  }

  Widget _buildDeleteButton() {
    return IconButton(
      onPressed: onDelete,
      icon: const Icon(
        Icons.delete_outline,
        color: AppColors.error,
        size: AppDimensions.iconSmall,
      ),
      tooltip: 'حذف',
    );
  }

  Widget _buildSelectedIndicator() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingXSmall),
      decoration: const BoxDecoration(
        color: AppColors.primary,
        shape: BoxShape.circle,
      ),
      child: const Icon(
        Icons.check,
        color: AppColors.white,
        size: AppDimensions.iconSmall,
      ),
    );
  }

  Color _getMethodColor() {
    switch (paymentMethod) {
      case PaymentMethod.jwaliWallet:
        return const Color(0xFF00BCD4);
      case PaymentMethod.cashWallet:
        return const Color(0xFF4CAF50);
      case PaymentMethod.oneCashWallet:
        return const Color(0xFFFF9800);
      case PaymentMethod.floskWallet:
        return const Color(0xFF9C27B0);
      case PaymentMethod.jaibWallet:
        return const Color(0xFF3F51B5);
      case PaymentMethod.cash:
        return AppColors.success;
      case PaymentMethod.paypal:
        return const Color(0xFF00457C);
      case PaymentMethod.creditCard:
        return AppColors.primary;
    }
  }

  IconData _getMethodIcon() {
    switch (paymentMethod) {
      case PaymentMethod.jwaliWallet:
      case PaymentMethod.cashWallet:
      case PaymentMethod.oneCashWallet:
      case PaymentMethod.floskWallet:
      case PaymentMethod.jaibWallet:
        return Icons.account_balance_wallet;
      case PaymentMethod.cash:
        return Icons.money;
      case PaymentMethod.paypal:
        return Icons.payment;
      case PaymentMethod.creditCard:
        return Icons.credit_card;
    }
  }

  String _getMethodDescription() {
    switch (paymentMethod) {
      case PaymentMethod.jwaliWallet:
        return 'الدفع عبر محفظة جوالي';
      case PaymentMethod.cashWallet:
        return 'الدفع عبر كاش محفظة';
      case PaymentMethod.oneCashWallet:
        return 'الدفع عبر محفظة ون كاش';
      case PaymentMethod.floskWallet:
        return 'الدفع عبر محفظة فلوس';
      case PaymentMethod.jaibWallet:
        return 'الدفع عبر محفظة جيب';
      case PaymentMethod.cash:
        return 'الدفع نقداً عند الوصول';
      case PaymentMethod.paypal:
        return 'الدفع عبر PayPal';
      case PaymentMethod.creditCard:
        return 'Visa, Mastercard, American Express';
    }
  }
}