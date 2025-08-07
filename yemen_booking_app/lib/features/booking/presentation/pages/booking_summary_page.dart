import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/utils/formatters.dart';
import '../../../../core/widgets/price_widget.dart';
import '../bloc/booking_bloc.dart';
import '../widgets/price_breakdown_widget.dart';

class BookingSummaryPage extends StatelessWidget {
  final Map<String, dynamic> bookingData;

  const BookingSummaryPage({
    super.key,
    required this.bookingData,
  });

  @override
  Widget build(BuildContext context) {
    final checkIn = bookingData['checkIn'] as DateTime;
    final checkOut = bookingData['checkOut'] as DateTime;
    final nights = checkOut.difference(checkIn).inDays;
    final pricePerNight = (bookingData['pricePerNight'] ?? 0.0) as double;
    final totalPrice = nights * pricePerNight;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(context),
      body: SingleChildScrollView(
        child: Column(
          children: [
            _buildPropertyCard(context),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildBookingDetails(context, nights),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildGuestDetails(context),
            const SizedBox(height: AppDimensions.spacingMd),
            if (bookingData['selectedServices'].isNotEmpty) ...[
              _buildServicesCard(context),
              const SizedBox(height: AppDimensions.spacingMd),
            ],
            if (bookingData['specialRequests']?.isNotEmpty ?? false) ...[
              _buildSpecialRequestsCard(context),
              const SizedBox(height: AppDimensions.spacingMd),
            ],
            _buildPriceBreakdown(context, nights, pricePerNight, totalPrice),
            const SizedBox(height: AppDimensions.spacingLg),
          ],
        ),
      ),
      bottomNavigationBar: _buildBottomBar(context, totalPrice),
    );
  }

  PreferredSizeWidget _buildAppBar(BuildContext context) {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'ملخص الحجز',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            'الخطوة 2 من 3',
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
            value: 0.66,
            backgroundColor: AppColors.divider,
            valueColor: AlwaysStoppedAnimation<Color>(AppColors.primary),
          ),
        ),
      ),
    );
  }

  Widget _buildPropertyCard(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurSmall,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 80,
            height: 80,
            decoration: BoxDecoration(
              color: AppColors.primary.withOpacity(0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
            ),
            child: Icon(
              Icons.apartment,
              color: AppColors.primary,
              size: AppDimensions.iconLarge,
            ),
          ),
          const SizedBox(width: AppDimensions.spacingMd),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  bookingData['propertyName'],
                  style: AppTextStyles.subtitle1.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
                const SizedBox(height: AppDimensions.spacingXs),
                Row(
                  children: [
                    Icon(
                      Icons.location_on_outlined,
                      size: AppDimensions.iconSmall,
                      color: AppColors.textSecondary,
                    ),
                    const SizedBox(width: AppDimensions.spacingXs),
                    Expanded(
                      child: Text(
                        'صنعاء، اليمن', // This should come from property data
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBookingDetails(BuildContext context, int nights) {
    final checkIn = bookingData['checkIn'] as DateTime;
    final checkOut = bookingData['checkOut'] as DateTime;
    final dateFormat = DateFormat('dd MMM yyyy', 'ar');

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.outline),
      ),
      child: Column(
        children: [
          _buildDetailRow(
            icon: Icons.calendar_today,
            label: 'تاريخ الوصول',
            value: dateFormat.format(checkIn),
          ),
          const Divider(height: AppDimensions.spacingLg),
          _buildDetailRow(
            icon: Icons.calendar_today_outlined,
            label: 'تاريخ المغادرة',
            value: dateFormat.format(checkOut),
          ),
          const Divider(height: AppDimensions.spacingLg),
          _buildDetailRow(
            icon: Icons.nights_stay,
            label: 'عدد الليالي',
            value: '$nights ${nights == 1 ? 'ليلة' : 'ليالي'}',
          ),
        ],
      ),
    );
  }

  Widget _buildGuestDetails(BuildContext context) {
    final adultsCount = bookingData['adultsCount'] as int;
    final childrenCount = bookingData['childrenCount'] as int;
    final totalGuests = adultsCount + childrenCount;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.outline),
      ),
      child: _buildDetailRow(
        icon: Icons.people,
        label: 'عدد الضيوف',
        value: '$totalGuests ضيف ($adultsCount بالغ${childrenCount > 0 ? '، $childrenCount طفل' : ''})',
      ),
    );
  }

  Widget _buildServicesCard(BuildContext context) {
    final services = bookingData['selectedServices'] as List<Map<String, dynamic>>;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
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
            'الخدمات الإضافية',
            style: AppTextStyles.subtitle2.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          ...services.map((service) => Padding(
            padding: const EdgeInsets.only(bottom: AppDimensions.spacingSm),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  service['name'],
                  style: AppTextStyles.bodyMedium,
                ),
                Text(
                  '${service['price']} ريال',
                  style: AppTextStyles.bodyMedium.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
              ],
            ),
          )).toList(),
        ],
      ),
    );
  }

  Widget _buildSpecialRequestsCard(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.info.withOpacity(0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.info),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(
            Icons.info_outline,
            color: AppColors.info,
            size: AppDimensions.iconSmall,
          ),
          const SizedBox(width: AppDimensions.spacingSm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'طلبات خاصة',
                  style: AppTextStyles.caption.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.info,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  bookingData['specialRequests'],
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textPrimary,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPriceBreakdown(
    BuildContext context,
    int nights,
    double pricePerNight,
    double totalPrice,
  ) {
    final services = bookingData['selectedServices'] as List<Map<String, dynamic>>;
    final servicesTotal = services.fold<double>(
      0,
      (sum, service) => sum + (service['price'] as num).toDouble(),
    );

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
      child: PriceBreakdownWidget(
        nights: nights,
        pricePerNight: pricePerNight,
        servicesTotal: servicesTotal,
        taxRate: 0.05, // 5% tax
        currency: 'ريال',
      ),
    );
  }

  Widget _buildDetailRow({
    required IconData icon,
    required String label,
    required String value,
  }) {
    return Row(
      children: [
        Icon(
          icon,
          size: AppDimensions.iconSmall,
          color: AppColors.textSecondary,
        ),
        const SizedBox(width: AppDimensions.spacingSm),
        Text(
          label,
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        const Spacer(),
        Text(
          value,
          style: AppTextStyles.bodyMedium.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildBottomBar(BuildContext context, double totalPrice) {
    final services = bookingData['selectedServices'] as List<Map<String, dynamic>>;
    final servicesTotal = services.fold<double>(
      0,
      (sum, service) => sum + (service['price'] as num).toDouble(),
    );
    final tax = (totalPrice + servicesTotal) * 0.05;
    final grandTotal = totalPrice + servicesTotal + tax;

    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurLarge,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'المجموع الكلي',
                  style: AppTextStyles.subtitle1.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                PriceWidget(
                  price: grandTotal,
                  currency: 'ريال',
                  displayType: PriceDisplayType.normal,
                ),
              ],
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: () => _navigateToPayment(context),
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
                    const Icon(Icons.payment, color: AppColors.white),
                    const SizedBox(width: AppDimensions.spacingSm),
                    Text(
                      'المتابعة إلى الدفع',
                      style: AppTextStyles.button.copyWith(
                        color: AppColors.white,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _navigateToPayment(BuildContext context) {
    context.push('/booking/payment', extra: bookingData);
  }
}