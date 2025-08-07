import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';

class PaymentMethodsWidget extends StatelessWidget {
  final String? selectedMethod;
  final Function(String) onMethodSelected;

  const PaymentMethodsWidget({
    super.key,
    this.selectedMethod,
    required this.onMethodSelected,
  });

  final List<Map<String, dynamic>> _paymentMethods = const [
    {
      'id': 'cash',
      'name': 'الدفع نقداً عند الوصول',
      'description': 'ادفع مباشرة عند تسجيل الوصول',
      'icon': Icons.money,
      'color': Color(0xFF4CAF50),
      'available': true,
    },
    {
      'id': 'card',
      'name': 'بطاقة الائتمان / الخصم',
      'description': 'Visa, Mastercard, American Express',
      'icon': Icons.credit_card,
      'color': Color(0xFF2196F3),
      'available': true,
    },
    {
      'id': 'wallet',
      'name': 'المحفظة الإلكترونية',
      'description': 'فلوسي، كاش، موبي كاش',
      'icon': Icons.account_balance_wallet,
      'color': Color(0xFF9C27B0),
      'available': true,
    },
    {
      'id': 'bank',
      'name': 'التحويل البنكي',
      'description': 'تحويل مباشر من حسابك البنكي',
      'icon': Icons.account_balance,
      'color': Color(0xFFFF9800),
      'available': true,
    },
    {
      'id': 'paypal',
      'name': 'PayPal',
      'description': 'ادفع باستخدام حساب PayPal',
      'icon': Icons.payment,
      'color': Color(0xFF00457C),
      'available': false,
    },
  ];

  @override
  Widget build(BuildContext context) {
    return Column(
      children: _paymentMethods.map((method) {
        final isSelected = selectedMethod == method['id'];
        final isAvailable = method['available'] as bool;
        
        return Opacity(
          opacity: isAvailable ? 1.0 : 0.5,
          child: InkWell(
            onTap: isAvailable ? () => onMethodSelected(method['id']) : null,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            child: Container(
              margin: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              decoration: BoxDecoration(
                color: isSelected
                    ? AppColors.primary.withOpacity(0.05)
                    : AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                border: Border.all(
                  color: isSelected ? AppColors.primary : AppColors.outline,
                  width: isSelected ? 2 : 1,
                ),
              ),
              child: Row(
                children: [
                  Container(
                    width: 24,
                    height: 24,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      border: Border.all(
                        color: isSelected ? AppColors.primary : AppColors.outline,
                        width: 2,
                      ),
                      color: isSelected ? AppColors.primary : Colors.transparent,
                    ),
                    child: isSelected
                        ? const Icon(
                            Icons.check,
                            size: 16,
                            color: Colors.white,
                          )
                        : null,
                  ),
                  const SizedBox(width: AppDimensions.spacingMd),
                  Container(
                    padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                    decoration: BoxDecoration(
                      color: (method['color'] as Color).withOpacity(0.1),
                      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                    ),
                    child: Icon(
                      method['icon'] as IconData,
                      color: method['color'] as Color,
                      size: AppDimensions.iconMedium,
                    ),
                  ),
                  const SizedBox(width: AppDimensions.spacingMd),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            Text(
                              method['name'],
                              style: AppTextStyles.bodyMedium.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            if (!isAvailable) ...[
                              const SizedBox(width: AppDimensions.spacingSm),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: AppDimensions.paddingSmall,
                                  vertical: 2,
                                ),
                                decoration: BoxDecoration(
                                  color: AppColors.disabled.withOpacity(0.2),
                                  borderRadius: BorderRadius.circular(
                                    AppDimensions.borderRadiusXs,
                                  ),
                                ),
                                child: Text(
                                  'غير متاح',
                                  style: AppTextStyles.caption.copyWith(
                                    color: AppColors.disabled,
                                  ),
                                ),
                              ),
                            ],
                          ],
                        ),
                        const SizedBox(height: 2),
                        Text(
                          method['description'],
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ),
                  if (isSelected)
                    Icon(
                      Icons.check_circle,
                      color: AppColors.primary,
                      size: AppDimensions.iconMedium,
                    ),
                ],
              ),
            ),
          ),
        );
      }).toList(),
    );
  }
}