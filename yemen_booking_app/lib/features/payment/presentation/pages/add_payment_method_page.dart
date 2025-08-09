/// features/payment/presentation/pages/add_payment_method_page.dart

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/enums/payment_method_enum.dart';
import '../bloc/payment_bloc.dart';
import '../bloc/payment_event.dart';
import '../bloc/payment_state.dart';
import '../widgets/credit_card_form_widget.dart';

class AddPaymentMethodPage extends StatefulWidget {
  final PaymentMethod? initialMethod;

  const AddPaymentMethodPage({
    super.key,
    this.initialMethod,
  });

  @override
  State<AddPaymentMethodPage> createState() => _AddPaymentMethodPageState();
}

class _AddPaymentMethodPageState extends State<AddPaymentMethodPage>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _cardAnimation;
  late Animation<double> _formAnimation;

  PaymentMethod _selectedMethod = PaymentMethod.creditCard;
  final _formKey = GlobalKey<FormState>();
  bool _saveCard = false;

  @override
  void initState() {
    super.initState();
    if (widget.initialMethod != null) {
      _selectedMethod = widget.initialMethod!;
    }
    _setupAnimations();
  }

  void _setupAnimations() {
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _cardAnimation = CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.0, 0.6, curve: Curves.easeOutBack),
    );

    _formAnimation = CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.3, 1.0, curve: Curves.easeOut),
    );

    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: BlocListener<PaymentBloc, PaymentState>(
        listener: (context, state) {
          if (state is PaymentDetailsValid) {
            _savePaymentMethod();
          } else if (state is PaymentDetailsInvalid) {
            _showErrors(state.errors);
          }
        },
        child: SingleChildScrollView(
          child: Column(
            children: [
              _buildMethodSelector(),
              AnimatedSwitcher(
                duration: const Duration(milliseconds: 300),
                child: _buildFormContent(),
              ),
            ],
          ),
        ),
      ),
    );
  }

  PreferredSizeWidget _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      title: Text(
        'إضافة طريقة دفع',
        style: AppTextStyles.heading3.copyWith(
          fontWeight: FontWeight.bold,
        ),
      ),
      actions: [
        IconButton(
          onPressed: _showHelp,
          icon: const Icon(Icons.help_outline),
        ),
      ],
    );
  }

  Widget _buildMethodSelector() {
    return Container(
      height: 100,
      margin: const EdgeInsets.symmetric(vertical: AppDimensions.paddingMedium),
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
        itemCount: PaymentMethod.values.length,
        itemBuilder: (context, index) {
          final method = PaymentMethod.values[index];
          final isSelected = method == _selectedMethod;
          
          return GestureDetector(
            onTap: () {
              setState(() {
                _selectedMethod = method;
              });
              _animationController.reset();
              _animationController.forward();
            },
            child: AnimatedContainer(
              duration: const Duration(milliseconds: 200),
              width: 80,
              margin: const EdgeInsets.only(right: AppDimensions.spacingMd),
              decoration: BoxDecoration(
                color: isSelected ? AppColors.primary : AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                border: Border.all(
                  color: isSelected ? AppColors.primary : AppColors.outline,
                  width: isSelected ? 2 : 1,
                ),
                boxShadow: isSelected
                    ? [
                        BoxShadow(
                          color: AppColors.primary.withValues(alpha: 0.3),
                          blurRadius: 8,
                          offset: const Offset(0, 4),
                        ),
                      ]
                    : null,
              ),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    _getMethodIcon(method),
                    color: isSelected ? AppColors.white : AppColors.textSecondary,
                    size: AppDimensions.iconMedium,
                  ),
                  const SizedBox(height: AppDimensions.spacingSm),
                  Text(
                    method.displayNameAr.split(' ').first,
                    style: AppTextStyles.caption.copyWith(
                      color: isSelected ? AppColors.white : AppColors.textSecondary,
                      fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                    ),
                    textAlign: TextAlign.center,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildFormContent() {
    if (_selectedMethod == PaymentMethod.creditCard) {
      return _buildCreditCardForm();
    } else if (_selectedMethod.isWallet) {
      return _buildWalletForm();
    } else if (_selectedMethod == PaymentMethod.paypal) {
      return _buildPayPalForm();
    } else {
      return _buildCashInfo();
    }
  }

  Widget _buildCreditCardForm() {
    return AnimatedBuilder(
      animation: _formAnimation,
      builder: (context, child) {
        return Transform.translate(
          offset: Offset(0, 50 * (1 - _formAnimation.value)),
          child: Opacity(
            opacity: _formAnimation.value,
            child: Container(
              margin: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    CreditCardFormWidget(
                      onCardNumberChanged: (value) {},
                      onCardHolderChanged: (value) {},
                      onExpiryDateChanged: (value) {},
                      onCvvChanged: (value) {},
                    ),
                    const SizedBox(height: AppDimensions.spacingLg),
                    _buildSaveCardOption(),
                    const SizedBox(height: AppDimensions.spacingXl),
                    _buildSubmitButton(),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildWalletForm() {
    return AnimatedBuilder(
      animation: _formAnimation,
      builder: (context, child) {
        return Transform.translate(
          offset: Offset(0, 50 * (1 - _formAnimation.value)),
          child: Opacity(
            opacity: _formAnimation.value,
            child: Container(
              margin: const EdgeInsets.all(AppDimensions.paddingMedium),
              padding: const EdgeInsets.all(AppDimensions.paddingLarge),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
                boxShadow: const [
                  BoxShadow(
                    color: AppColors.shadow,
                    blurRadius: AppDimensions.blurMedium,
                    offset: Offset(0, 4),
                  ),
                ],
              ),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    Icon(
                      _getMethodIcon(_selectedMethod),
                      size: AppDimensions.iconXLarge,
                      color: AppColors.primary,
                    ),
                    const SizedBox(height: AppDimensions.spacingMd),
                    Text(
                      _selectedMethod.displayNameAr,
                      style: AppTextStyles.subtitle1.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: AppDimensions.spacingLg),
                    TextFormField(
                      decoration: InputDecoration(
                        labelText: 'رقم المحفظة',
                        prefixIcon: const Icon(Icons.phone_android),
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                        ),
                      ),
                      keyboardType: TextInputType.phone,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'رقم المحفظة مطلوب';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: AppDimensions.spacingMd),
                    TextFormField(
                      decoration: InputDecoration(
                        labelText: 'رمز التحقق',
                        prefixIcon: const Icon(Icons.lock),
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                        ),
                      ),
                      obscureText: true,
                      keyboardType: TextInputType.number,
                      maxLength: 6,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'رمز التحقق مطلوب';
                        }
                        if (value.length < 4) {
                          return 'رمز التحقق غير صالح';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: AppDimensions.spacingXl),
                    _buildSubmitButton(),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildPayPalForm() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        boxShadow: const [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurMedium,
            offset: Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        children: [
          Image.asset(
            'assets/images/paypal.png',
            height: 60,
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          Text(
            'سيتم توجيهك إلى موقع PayPal',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Text(
            'قم بتسجيل الدخول إلى حساب PayPal الخاص بك لإكمال عملية الربط',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: AppDimensions.spacingXl),
          ElevatedButton.icon(
            onPressed: _connectPayPal,
            icon: const Icon(Icons.link),
            label: const Text('ربط حساب PayPal'),
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFF00457C),
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingLarge,
                vertical: AppDimensions.paddingMedium,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCashInfo() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppColors.success.withValues(alpha: 0.1),
            AppColors.success.withValues(alpha: 0.05),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        border: Border.all(
          color: AppColors.success.withValues(alpha: 0.3),
        ),
      ),
      child: Column(
        children: [
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingLarge),
            decoration: BoxDecoration(
              color: AppColors.success.withValues(alpha: 0.2),
              shape: BoxShape.circle,
            ),
            child: const Icon(
              Icons.money,
              size: AppDimensions.iconXLarge,
              color: AppColors.success,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          Text(
            'الدفع نقداً عند الوصول',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
              color: AppColors.success,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Text(
            'ستدفع المبلغ كاملاً عند تسجيل الوصول في العقار',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          const Divider(),
          const SizedBox(height: AppDimensions.spacingLg),
          _buildInfoItem(
            Icons.check_circle,
            'لا حاجة لبطاقة ائتمان',
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildInfoItem(
            Icons.security,
            'آمن وموثوق',
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildInfoItem(
            Icons.cancel,
            'إلغاء مجاني حتى 24 ساعة قبل الوصول',
          ),
          const SizedBox(height: AppDimensions.spacingXl),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, PaymentMethod.cash),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.success,
              minimumSize: const Size(double.infinity, 48),
            ),
            child: const Text('اختيار الدفع نقداً'),
          ),
        ],
      ),
    );
  }

  Widget _buildInfoItem(IconData icon, String text) {
    return Row(
      children: [
        Icon(
          icon,
          color: AppColors.success,
          size: AppDimensions.iconSmall,
        ),
        const SizedBox(width: AppDimensions.spacingSm),
        Expanded(
          child: Text(
            text,
            style: AppTextStyles.bodyMedium,
          ),
        ),
      ],
    );
  }

  Widget _buildSaveCardOption() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.info.withValues(alpha: 0.05),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: AppColors.info.withValues(alpha: 0.2),
        ),
      ),
      child: Row(
        children: [
          Checkbox(
            value: _saveCard,
            onChanged: (value) {
              setState(() {
                _saveCard = value ?? false;
              });
            },
            activeColor: AppColors.primary,
          ),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'حفظ البطاقة للاستخدام المستقبلي',
                  style: AppTextStyles.bodyMedium.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  'البيانات محمية ومشفرة بأعلى معايير الأمان',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSubmitButton() {
    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: _validateAndSave,
        style: ElevatedButton.styleFrom(
          backgroundColor: AppColors.primary,
          padding: const EdgeInsets.symmetric(
            vertical: AppDimensions.paddingMedium,
          ),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.add_card, color: AppColors.white),
            const SizedBox(width: AppDimensions.spacingSm),
            Text(
              'إضافة طريقة الدفع',
              style: AppTextStyles.button.copyWith(
                color: AppColors.white,
              ),
            ),
          ],
        ),
      ),
    );
  }

  IconData _getMethodIcon(PaymentMethod method) {
    switch (method) {
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

  void _validateAndSave() {
    if (_formKey.currentState?.validate() ?? false) {
      context.read<PaymentBloc>().add(
        ValidatePaymentDetailsEvent(
          paymentMethod: _selectedMethod,
          // Add form field values here
        ),
      );
    }
  }

  void _savePaymentMethod() {
    Navigator.pop(context, _selectedMethod);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: const Row(
          children: [
            Icon(Icons.check_circle, color: AppColors.white),
            SizedBox(width: 8),
            Text('تمت إضافة طريقة الدفع بنجاح'),
          ],
        ),
        backgroundColor: AppColors.success,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
      ),
    );
  }

  void _showErrors(Map<String, String> errors) {
    final errorMessage = errors.values.join('\n');
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(errorMessage),
        backgroundColor: AppColors.error,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  void _connectPayPal() {
    // Implement PayPal connection
  }

  void _showHelp() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        title: const Row(
          children: [
            Icon(Icons.help_outline, color: AppColors.primary),
            SizedBox(width: 8),
            Text('المساعدة'),
          ],
        ),
        content: const SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'كيفية إضافة طريقة دفع:',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
              SizedBox(height: 8),
              Text('1. اختر طريقة الدفع المناسبة'),
              Text('2. أدخل البيانات المطلوبة'),
              Text('3. تحقق من صحة البيانات'),
              Text('4. اضغط على زر الإضافة'),
              SizedBox(height: 16),
              Text(
                'ملاحظة:',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
              SizedBox(height: 8),
              Text('جميع البيانات محمية ومشفرة بأعلى معايير الأمان'),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('حسناً'),
          ),
        ],
      ),
    );
  }
}