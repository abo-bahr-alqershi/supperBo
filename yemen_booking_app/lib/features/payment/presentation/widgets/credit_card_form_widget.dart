/// features/payment/presentation/widgets/credit_card_form_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class CreditCardFormWidget extends StatefulWidget {
  final Function(String) onCardNumberChanged;
  final Function(String) onCardHolderChanged;
  final Function(String) onExpiryDateChanged;
  final Function(String) onCvvChanged;
  final bool showCardPreview;

  const CreditCardFormWidget({
    super.key,
    required this.onCardNumberChanged,
    required this.onCardHolderChanged,
    required this.onExpiryDateChanged,
    required this.onCvvChanged,
    this.showCardPreview = true,
  });

  @override
  State<CreditCardFormWidget> createState() => _CreditCardFormWidgetState();
}

class _CreditCardFormWidgetState extends State<CreditCardFormWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _flipController;
  late Animation<double> _flipAnimation;
  
  final _cardNumberController = TextEditingController();
  final _cardHolderController = TextEditingController();
  final _expiryController = TextEditingController();
  final _cvvController = TextEditingController();
  
  final _cardNumberFocus = FocusNode();
  final _cardHolderFocus = FocusNode();
  final _expiryFocus = FocusNode();
  final _cvvFocus = FocusNode();
  
  bool _isCardFlipped = false;
  String _cardType = '';

  @override
  void initState() {
    super.initState();
    _flipController = AnimationController(
      duration: const Duration(milliseconds: 600),
      vsync: this,
    );
    _flipAnimation = Tween<double>(
      begin: 0,
      end: 1,
    ).animate(CurvedAnimation(
      parent: _flipController,
      curve: Curves.easeInOut,
    ));
    
    _cvvFocus.addListener(() {
      if (_cvvFocus.hasFocus && !_isCardFlipped) {
        _flipCard();
      } else if (!_cvvFocus.hasFocus && _isCardFlipped) {
        _flipCard();
      }
    });
    
    _cardNumberController.addListener(_detectCardType);
  }

  @override
  void dispose() {
    _flipController.dispose();
    _cardNumberController.dispose();
    _cardHolderController.dispose();
    _expiryController.dispose();
    _cvvController.dispose();
    _cardNumberFocus.dispose();
    _cardHolderFocus.dispose();
    _expiryFocus.dispose();
    _cvvFocus.dispose();
    super.dispose();
  }

  void _flipCard() {
    if (_isCardFlipped) {
      _flipController.reverse();
    } else {
      _flipController.forward();
    }
    setState(() {
      _isCardFlipped = !_isCardFlipped;
    });
  }

  void _detectCardType() {
    final number = _cardNumberController.text.replaceAll(' ', '');
    String type = '';
    
    if (number.startsWith('4')) {
      type = 'visa';
    } else if (number.startsWith('5')) {
      type = 'mastercard';
    } else if (number.startsWith('3')) {
      type = 'amex';
    }
    
    if (type != _cardType) {
      setState(() {
        _cardType = type;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        if (widget.showCardPreview) ...[
          _buildCardPreview(),
          const SizedBox(height: AppDimensions.spacingXl),
        ],
        _buildFormFields(),
      ],
    );
  }

  Widget _buildCardPreview() {
    return AnimatedBuilder(
      animation: _flipAnimation,
      builder: (context, child) {
        final isShowingFront = _flipAnimation.value < 0.5;
        return Transform(
          alignment: Alignment.center,
          transform: Matrix4.identity()
            ..setEntry(3, 2, 0.001)
            ..rotateY(_flipAnimation.value * 3.14159),
          child: Container(
            height: 200,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: _getCardGradient(),
              ),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
              boxShadow: [
                BoxShadow(
                  color: AppColors.shadow.withValues(alpha: 0.3),
                  blurRadius: 20,
                  offset: const Offset(0, 10),
                ),
              ],
            ),
            child: isShowingFront
                ? _buildCardFront()
                : Transform(
                    alignment: Alignment.center,
                    transform: Matrix4.identity()..rotateY(3.14159),
                    child: _buildCardBack(),
                  ),
          ),
        );
      },
    );
  }

  Widget _buildCardFront() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              _buildCardTypeIcon(),
              Icon(
                Icons.contactless,
                color: AppColors.white.withValues(alpha: 0.8),
                size: AppDimensions.iconMedium,
              ),
            ],
          ),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                _formatCardNumber(_cardNumberController.text),
                style: AppTextStyles.heading3.copyWith(
                  color: AppColors.white,
                  letterSpacing: 2,
                  fontFamily: 'monospace',
                ),
              ),
              const SizedBox(height: AppDimensions.spacingMd),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'حامل البطاقة',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.white.withValues(alpha: 0.7),
                        ),
                      ),
                      Text(
                        _cardHolderController.text.isEmpty
                            ? 'الاسم الكامل'
                            : _cardHolderController.text.toUpperCase(),
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppColors.white,
                        ),
                      ),
                    ],
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'تاريخ الانتهاء',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.white.withValues(alpha: 0.7),
                        ),
                      ),
                      Text(
                        _expiryController.text.isEmpty
                            ? 'MM/YY'
                            : _expiryController.text,
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppColors.white,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildCardBack() {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Container(
          height: 40,
          color: AppColors.black,
          margin: const EdgeInsets.symmetric(vertical: AppDimensions.spacingLg),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingLarge),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingMedium,
                  vertical: AppDimensions.paddingSmall,
                ),
                decoration: BoxDecoration(
                  color: AppColors.white,
                  borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                ),
                child: Text(
                  _cvvController.text.isEmpty ? 'CVV' : _cvvController.text,
                  style: AppTextStyles.bodyMedium.copyWith(
                    fontFamily: 'monospace',
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildCardTypeIcon() {
    if (_cardType.isEmpty) {
      return Container(
        width: 50,
        height: 30,
        decoration: BoxDecoration(
          color: AppColors.white.withValues(alpha: 0.3),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
        ),
      );
    }
    
    IconData icon;
    switch (_cardType) {
      case 'visa':
        icon = Icons.credit_card;
        break;
      case 'mastercard':
        icon = Icons.credit_card;
        break;
      case 'amex':
        icon = Icons.credit_card;
        break;
      default:
        icon = Icons.credit_card;
    }
    
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
        vertical: AppDimensions.paddingXSmall,
      ),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: AppDimensions.iconSmall, color: AppColors.primary),
          const SizedBox(width: AppDimensions.spacingXs),
          Text(
            _cardType.toUpperCase(),
            style: AppTextStyles.caption.copyWith(
              fontWeight: FontWeight.bold,
              color: AppColors.primary,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFormFields() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        boxShadow: const [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurSmall,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        children: [
          // Card Number Field
          TextFormField(
            controller: _cardNumberController,
            focusNode: _cardNumberFocus,
            decoration: InputDecoration(
              labelText: 'رقم البطاقة',
              hintText: '0000 0000 0000 0000',
              prefixIcon: const Icon(Icons.credit_card),
              suffixIcon: _cardType.isNotEmpty
                  ? Padding(
                      padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                      child: _buildCardTypeIcon(),
                    )
                  : null,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.digitsOnly,
              CardNumberFormatter(),
            ],
            maxLength: 19,
            onChanged: (value) {
              widget.onCardNumberChanged(value.replaceAll(' ', ''));
            },
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'رقم البطاقة مطلوب';
              }
              if (value.replaceAll(' ', '').length < 13) {
                return 'رقم البطاقة غير صالح';
              }
              return null;
            },
          ),
          
          const SizedBox(height: AppDimensions.spacingMd),
          
          // Card Holder Name Field
          TextFormField(
            controller: _cardHolderController,
            focusNode: _cardHolderFocus,
            decoration: InputDecoration(
              labelText: 'اسم حامل البطاقة',
              hintText: 'الاسم كما هو مكتوب على البطاقة',
              prefixIcon: const Icon(Icons.person),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
            ),
            textCapitalization: TextCapitalization.characters,
            onChanged: widget.onCardHolderChanged,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'اسم حامل البطاقة مطلوب';
              }
              return null;
            },
          ),
          
          const SizedBox(height: AppDimensions.spacingMd),
          
          // Expiry Date and CVV Row
          Row(
            children: [
              Expanded(
                child: TextFormField(
                  controller: _expiryController,
                  focusNode: _expiryFocus,
                  decoration: InputDecoration(
                    labelText: 'تاريخ الانتهاء',
                    hintText: 'MM/YY',
                    prefixIcon: const Icon(Icons.calendar_today),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                    ),
                  ),
                  keyboardType: TextInputType.number,
                  inputFormatters: [
                    FilteringTextInputFormatter.digitsOnly,
                    ExpiryDateFormatter(),
                  ],
                  maxLength: 5,
                  onChanged: widget.onExpiryDateChanged,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'التاريخ مطلوب';
                    }
                    if (value.length < 5) {
                      return 'تاريخ غير صالح';
                    }
                    return null;
                  },
                ),
              ),
              const SizedBox(width: AppDimensions.spacingMd),
              Expanded(
                child: TextFormField(
                  controller: _cvvController,
                  focusNode: _cvvFocus,
                  decoration: InputDecoration(
                    labelText: 'CVV',
                    hintText: '123',
                    prefixIcon: const Icon(Icons.lock),
                    helperText: 'الرقم خلف البطاقة',
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                    ),
                  ),
                  keyboardType: TextInputType.number,
                  inputFormatters: [
                    FilteringTextInputFormatter.digitsOnly,
                    LengthLimitingTextInputFormatter(4),
                  ],
                  obscureText: true,
                  onChanged: widget.onCvvChanged,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'CVV مطلوب';
                    }
                    if (value.length < 3) {
                      return 'CVV غير صالح';
                    }
                    return null;
                  },
                ),
              ),
            ],
          ),
          
          const SizedBox(height: AppDimensions.spacingLg),
          
          // Security Note
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            decoration: BoxDecoration(
              color: AppColors.info.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              border: Border.all(
                color: AppColors.info.withValues(alpha: 0.3),
              ),
            ),
            child: Row(
              children: [
                const Icon(
                  Icons.security,
                  color: AppColors.info,
                  size: AppDimensions.iconMedium,
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'معلوماتك آمنة',
                        style: AppTextStyles.subtitle2.copyWith(
                          fontWeight: FontWeight.bold,
                          color: AppColors.info,
                        ),
                      ),
                      Text(
                        'جميع المعلومات مشفرة ومحمية بتقنية SSL',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  List<Color> _getCardGradient() {
    switch (_cardType) {
      case 'visa':
        return [Colors.blue.shade800, Colors.blue.shade400];
      case 'mastercard':
        return [Colors.red.shade800, Colors.orange.shade400];
      case 'amex':
        return [Colors.green.shade800, Colors.green.shade400];
      default:
        return [AppColors.primaryDark, AppColors.primary];
    }
  }

  String _formatCardNumber(String number) {
    final cleaned = number.replaceAll(' ', '');
    if (cleaned.isEmpty) return '•••• •••• •••• ••••';
    
    final formatted = StringBuffer();
    for (int i = 0; i < cleaned.length; i++) {
      if (i > 0 && i % 4 == 0) {
        formatted.write(' ');
      }
      formatted.write(cleaned[i]);
    }
    
    // Fill remaining with dots
    final remaining = 16 - cleaned.length;
    if (remaining > 0) {
      for (int i = 0; i < remaining; i++) {
        if (formatted.isNotEmpty && (cleaned.length + i) % 4 == 0) {
          formatted.write(' ');
        }
        formatted.write('•');
      }
    }
    
    return formatted.toString();
  }
}

// Custom Input Formatters
class CardNumberFormatter extends TextInputFormatter {
  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) {
    final text = newValue.text.replaceAll(' ', '');
    final buffer = StringBuffer();
    
    for (int i = 0; i < text.length; i++) {
      if (i > 0 && i % 4 == 0) {
        buffer.write(' ');
      }
      buffer.write(text[i]);
    }
    
    return TextEditingValue(
      text: buffer.toString(),
      selection: TextSelection.collapsed(offset: buffer.length),
    );
  }
}

class ExpiryDateFormatter extends TextInputFormatter {
  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) {
    final text = newValue.text.replaceAll('/', '');
    final buffer = StringBuffer();
    
    for (int i = 0; i < text.length && i < 4; i++) {
      if (i == 2) {
        buffer.write('/');
      }
      buffer.write(text[i]);
    }
    
    return TextEditingValue(
      text: buffer.toString(),
      selection: TextSelection.collapsed(offset: buffer.length),
    );
  }
}