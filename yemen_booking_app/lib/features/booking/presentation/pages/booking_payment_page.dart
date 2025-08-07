import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../domain/entities/booking_request.dart';
import '../bloc/booking_bloc.dart';
import '../bloc/booking_event.dart';
import '../bloc/booking_state.dart';
import '../widgets/payment_methods_widget.dart';

class BookingPaymentPage extends StatefulWidget {
  final Map<String, dynamic> bookingData;

  const BookingPaymentPage({
    super.key,
    required this.bookingData,
  });

  @override
  State<BookingPaymentPage> createState() => _BookingPaymentPageState();
}

class _BookingPaymentPageState extends State<BookingPaymentPage> {
  String? _selectedPaymentMethod;
  bool _acceptTerms = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: BlocConsumer<BookingBloc, BookingState>(
        listener: (context, state) {
          if (state is BookingCreated) {
            context.push(
              '/booking/confirmation',
              extra: state.booking,
            );
          } else if (state is BookingError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: AppColors.error,
              ),
            );
          }
        },
        builder: (context, state) {
          if (state is BookingLoading) {
            return const Center(
              child: LoadingWidget(
                type: LoadingType.circular,
                message: 'جاري معالجة الحجز...',
              ),
            );
          }

          return _buildContent();
        },
      ),
    );
  }

  PreferredSizeWidget _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'الدفع',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            'الخطوة 3 من 3',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
      bottom: PreferredSize(
        preferredSize: const Size.fromHeight(4),
        child: Container(
          height: 4,
          child: LinearProgressIndicator(
            value: 1.0,
            backgroundColor: AppColors.divider,
            valueColor: AlwaysStoppedAnimation<Color>(AppColors.primary),
          ),
        ),
      ),
    );
  }

  Widget _buildContent() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildPaymentMethodsSection(),
          const SizedBox(height: AppDimensions.spacingLg),
          _buildBookingSummary(),
          const SizedBox(height: AppDimensions.spacingLg),
          _buildTermsAndConditions(),
          const SizedBox(height: AppDimensions.spacingXl),
          _buildPayButton(),
          const SizedBox(height: AppDimensions.spacingLg),
          _buildSecurityInfo(),
        ],
      ),
    );
  }

  Widget _buildPaymentMethodsSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'طريقة الدفع',
          style: AppTextStyles.subtitle1.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        PaymentMethodsWidget(
          selectedMethod: _selectedPaymentMethod,
          onMethodSelected: (method) {
            setState(() {
              _selectedPaymentMethod = method;
            });
          },
        ),
      ],
    );
  }

  Widget _buildBookingSummary() {
    final checkIn = widget.bookingData['checkIn'] as DateTime;
    final checkOut = widget.bookingData['checkOut'] as DateTime;
    final nights = checkOut.difference(checkIn).inDays;
    final pricePerNight = (widget.bookingData['pricePerNight'] ?? 0.0) as double;
    final services = widget.bookingData['selectedServices'] as List<Map<String, dynamic>>;
    final servicesTotal = services.fold<double>(
      0,
      (sum, service) => sum + (service['price'] as num).toDouble(),
    );
    final subtotal = (nights * pricePerNight) + servicesTotal;
    final tax = subtotal * 0.05;
    final total = subtotal + tax;

    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.outline),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'ملخص الحجز',
            style: AppTextStyles.subtitle2.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildSummaryRow(
            label: 'الإقامة ($nights ليالي)',
            value: '${(nights * pricePerNight).toStringAsFixed(0)} ريال',
          ),
          if (servicesTotal > 0) ...[
            const SizedBox(height: AppDimensions.spacingSm),
            _buildSummaryRow(
              label: 'الخدمات الإضافية',
              value: '${servicesTotal.toStringAsFixed(0)} ريال',
            ),
          ],
          const SizedBox(height: AppDimensions.spacingSm),
          _buildSummaryRow(
            label: 'الضرائب (5%)',
            value: '${tax.toStringAsFixed(0)} ريال',
          ),
          const Divider(height: AppDimensions.spacingLg),
          _buildSummaryRow(
            label: 'المجموع الكلي',
            value: '${total.toStringAsFixed(0)} ريال',
            isTotal: true,
          ),
        ],
      ),
    );
  }

  Widget _buildSummaryRow({
    required String label,
    required String value,
    bool isTotal = false,
  }) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: isTotal
              ? AppTextStyles.subtitle2.copyWith(fontWeight: FontWeight.bold)
              : AppTextStyles.bodyMedium.copyWith(color: AppColors.textSecondary),
        ),
        Text(
          value,
          style: isTotal
              ? AppTextStyles.subtitle1.copyWith(
                  fontWeight: FontWeight.bold,
                  color: AppColors.primary,
                )
              : AppTextStyles.bodyMedium.copyWith(fontWeight: FontWeight.bold),
        ),
      ],
    );
  }

  Widget _buildTermsAndConditions() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.warning.withOpacity(0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.warning),
      ),
      child: Column(
        children: [
          Row(
            children: [
              Checkbox(
                value: _acceptTerms,
                onChanged: (value) {
                  setState(() {
                    _acceptTerms = value ?? false;
                  });
                },
                activeColor: AppColors.primary,
              ),
              Expanded(
                child: Text(
                  'أوافق على الشروط والأحكام وسياسة الإلغاء',
                  style: AppTextStyles.bodyMedium,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            'يمكن إلغاء الحجز مجاناً قبل 24 ساعة من تاريخ الوصول',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPayButton() {
    final isValid = _selectedPaymentMethod != null && _acceptTerms;

    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: isValid ? _processPayment : null,
        style: ElevatedButton.styleFrom(
          backgroundColor: AppColors.primary,
          disabledBackgroundColor: AppColors.disabled,
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
            const Icon(Icons.lock, color: AppColors.white, size: 20),
            const SizedBox(width: AppDimensions.spacingSm),
            Text(
              'دفع وتأكيد الحجز',
              style: AppTextStyles.button.copyWith(
                color: AppColors.white,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSecurityInfo() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.info.withOpacity(0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Row(
        children: [
          Icon(
            Icons.security,
            color: AppColors.info,
            size: AppDimensions.iconMedium,
          ),
          const SizedBox(width: AppDimensions.spacingSm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'الدفع الآمن',
                  style: AppTextStyles.caption.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.info,
                  ),
                ),
                Text(
                  'جميع معلومات الدفع محمية ومشفرة',
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

  void _processPayment() {
    final authState = context.read<AuthBloc>().state;
    if (authState is! AuthAuthenticated) {
      // Navigate to login
      context.push('/login');
      return;
    }

    final checkIn = widget.bookingData['checkIn'] as DateTime;
    final checkOut = widget.bookingData['checkOut'] as DateTime;
    final adultsCount = widget.bookingData['adultsCount'] as int;
    final childrenCount = widget.bookingData['childrenCount'] as int;
    final services = widget.bookingData['selectedServices'] as List<Map<String, dynamic>>;

    final bookingRequest = BookingRequest(
      userId: authState.user.userId,
      unitId: widget.bookingData['unitId'] ?? '',
      checkIn: checkIn,
      checkOut: checkOut,
      guestsCount: adultsCount + childrenCount,
      services: services.map((s) => BookingServiceRequest(
        serviceId: s['id'],
        quantity: s['quantity'] ?? 1,
      )).toList(),
      specialRequests: widget.bookingData['specialRequests'],
      bookingSource: 'MobileApp',
    );

    context.read<BookingBloc>().add(
      CreateBookingEvent(bookingRequest: bookingRequest),
    );
  }
}