import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../bloc/booking_bloc.dart';
import '../bloc/booking_event.dart';
import '../bloc/booking_state.dart';
import '../widgets/date_picker_widget.dart';
import '../widgets/guest_selector_widget.dart';
import '../widgets/services_selector_widget.dart';

class BookingFormPage extends StatefulWidget {
  final String propertyId;
  final String propertyName;
  final String? unitId;
  final double? pricePerNight;

  const BookingFormPage({
    super.key,
    required this.propertyId,
    required this.propertyName,
    this.unitId,
    this.pricePerNight,
  });

  @override
  State<BookingFormPage> createState() => _BookingFormPageState();
}

class _BookingFormPageState extends State<BookingFormPage> {
  final _formKey = GlobalKey<FormState>();
  final _specialRequestsController = TextEditingController();
  
  DateTime? _checkInDate;
  DateTime? _checkOutDate;
  int _adultsCount = 1;
  int _childrenCount = 0;
  List<Map<String, dynamic>> _selectedServices = [];

  @override
  void dispose() {
    _specialRequestsController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: BlocConsumer<BookingBloc, BookingState>(
        listener: (context, state) {
          if (state is AvailabilityChecked) {
            if (state.isAvailable) {
              _navigateToSummary();
            } else {
              _showUnavailableDialog();
            }
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
          if (state is CheckingAvailability) {
            return const Center(
              child: LoadingWidget(
                type: LoadingType.circular,
                message: 'جاري التحقق من التوفر...',
              ),
            );
          }
          
          return _buildForm();
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
            'حجز ${widget.propertyName}',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            'الخطوة 1 من 3',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
      bottom: const PreferredSize(
        preferredSize: Size.fromHeight(4),
        child: SizedBox(
          height: 4,
          child: LinearProgressIndicator(
            value: 0.33,
            backgroundColor: AppColors.divider,
            valueColor: AlwaysStoppedAnimation<Color>(AppColors.primary),
          ),
        ),
      ),
    );
  }

  Widget _buildForm() {
    return Form(
      key: _formKey,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildSectionTitle('تواريخ الإقامة'),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildDateSelection(),
            
            const SizedBox(height: AppDimensions.spacingLg),
            _buildSectionTitle('عدد الضيوف'),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildGuestSelection(),
            
            const SizedBox(height: AppDimensions.spacingLg),
            _buildSectionTitle('الخدمات الإضافية'),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildServicesSelection(),
            
            const SizedBox(height: AppDimensions.spacingLg),
            _buildSectionTitle('طلبات خاصة (اختياري)'),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildSpecialRequests(),
            
            const SizedBox(height: AppDimensions.spacingXl),
            _buildContinueButton(),
            const SizedBox(height: AppDimensions.spacingLg),
          ],
        ),
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Text(
      title,
      style: AppTextStyles.subtitle1.copyWith(
        fontWeight: FontWeight.bold,
      ),
    );
  }

  Widget _buildDateSelection() {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.outline),
      ),
      child: Column(
        children: [
          DatePickerWidget(
            label: 'تاريخ الوصول',
            selectedDate: _checkInDate,
            firstDate: DateTime.now(),
            lastDate: DateTime.now().add(const Duration(days: 365)),
            onDateSelected: (date) {
              setState(() {
                _checkInDate = date;
                if (_checkOutDate != null && _checkOutDate!.isBefore(date)) {
                  _checkOutDate = null;
                }
              });
            },
            icon: Icons.calendar_today,
          ),
          const Divider(height: 1),
          DatePickerWidget(
            label: 'تاريخ المغادرة',
            selectedDate: _checkOutDate,
            firstDate: _checkInDate?.add(const Duration(days: 1)) ?? DateTime.now().add(const Duration(days: 1)),
            lastDate: DateTime.now().add(const Duration(days: 365)),
            onDateSelected: (date) {
              setState(() {
                _checkOutDate = date;
              });
            },
            enabled: _checkInDate != null,
            icon: Icons.calendar_today,
          ),
        ],
      ),
    );
  }

  Widget _buildGuestSelection() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.outline),
      ),
      child: Column(
        children: [
          GuestSelectorWidget(
            label: 'البالغين',
            count: _adultsCount,
            minCount: 1,
            maxCount: 10,
            onChanged: (count) {
              setState(() {
                _adultsCount = count;
              });
            },
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          GuestSelectorWidget(
            label: 'الأطفال',
            subtitle: '(أقل من 12 سنة)',
            count: _childrenCount,
            minCount: 0,
            maxCount: 5,
            onChanged: (count) {
              setState(() {
                _childrenCount = count;
              });
            },
          ),
        ],
      ),
    );
  }

  Widget _buildServicesSelection() {
    return ServicesSelectorWidget(
      propertyId: widget.propertyId,
      onServicesChanged: (services) {
        setState(() {
          _selectedServices = services;
        });
      },
    );
  }

  Widget _buildSpecialRequests() {
    return TextFormField(
      controller: _specialRequestsController,
      maxLines: 4,
      decoration: InputDecoration(
        hintText: 'أضف أي طلبات أو ملاحظات خاصة...',
        filled: true,
        fillColor: AppColors.surface,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.outline),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.outline),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          borderSide: const BorderSide(color: AppColors.primary, width: 2),
        ),
      ),
    );
  }

  Widget _buildContinueButton() {
    final isValid = _checkInDate != null && _checkOutDate != null;
    
    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: isValid ? _onContinue : null,
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
        child: Text(
          'المتابعة إلى الملخص',
          style: AppTextStyles.button.copyWith(
            color: AppColors.white,
          ),
        ),
      ),
    );
  }

  void _onContinue() {
    if (_formKey.currentState!.validate()) {
      if (context.read<AuthBloc>().state is AuthAuthenticated) {
        // If needed later, retrieve userId from state
      }

      // Check availability first
      context.read<BookingBloc>().add(
        CheckAvailabilityEvent(
          unitId: widget.unitId ?? '',
          checkIn: _checkInDate!,
          checkOut: _checkOutDate!,
          guestsCount: _adultsCount + _childrenCount,
        ),
      );
    }
  }

  void _navigateToSummary() {
    final formData = {
      'propertyId': widget.propertyId,
      'propertyName': widget.propertyName,
      'unitId': widget.unitId,
      'checkIn': _checkInDate,
      'checkOut': _checkOutDate,
      'adultsCount': _adultsCount,
      'childrenCount': _childrenCount,
      'selectedServices': _selectedServices,
      'specialRequests': _specialRequestsController.text,
      'pricePerNight': widget.pricePerNight,
    };

    context.push('/booking/summary', extra: formData);
  }

  void _showUnavailableDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('غير متاح'),
        content: const Text(
          'عذراً، الوحدة غير متاحة في التواريخ المحددة. يرجى اختيار تواريخ أخرى.',
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