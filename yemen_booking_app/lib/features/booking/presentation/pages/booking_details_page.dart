import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import 'package:qr_flutter/qr_flutter.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/price_widget.dart';
import '../../../../core/enums/booking_status.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../bloc/booking_bloc.dart';
import '../bloc/booking_event.dart';
import '../bloc/booking_state.dart';
import '../widgets/booking_status_widget.dart';
import '../widgets/cancellation_deadline_has_expired_widget.dart';

class BookingDetailsPage extends StatefulWidget {
  final String bookingId;

  const BookingDetailsPage({
    super.key,
    required this.bookingId,
  });

  @override
  State<BookingDetailsPage> createState() => _BookingDetailsPageState();
}

class _BookingDetailsPageState extends State<BookingDetailsPage> {
  @override
  void initState() {
    super.initState();
    _loadBookingDetails();
  }

  void _loadBookingDetails() {
    final authState = context.read<AuthBloc>().state;
    if (authState is AuthAuthenticated) {
      context.read<BookingBloc>().add(
        GetBookingDetailsEvent(
          bookingId: widget.bookingId,
          userId: authState.user.userId,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: BlocBuilder<BookingBloc, BookingState>(
        builder: (context, state) {
          if (state is BookingLoading) {
            return const Center(
              child: LoadingWidget(
                type: LoadingType.circular,
                message: 'جاري تحميل تفاصيل الحجز...',
              ),
            );
          }

          if (state is BookingError) {
            return Center(
              child: CustomErrorWidget(
                message: state.message,
                onRetry: _loadBookingDetails,
              ),
            );
          }

          if (state is BookingDetailsLoaded) {
            return _buildContent(state.booking);
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }

  PreferredSizeWidget _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      title: Text(
        'تفاصيل الحجز',
        style: AppTextStyles.heading3.copyWith(
          fontWeight: FontWeight.bold,
        ),
      ),
      actions: [
        IconButton(
          onPressed: _shareBooking,
          icon: const Icon(Icons.share),
        ),
        PopupMenuButton<String>(
          onSelected: _handleMenuAction,
          itemBuilder: (context) => [
            const PopupMenuItem(
              value: 'download',
              child: Row(
                children: [
                  Icon(Icons.download, size: 20),
                  SizedBox(width: 8),
                  Text('تحميل PDF'),
                ],
              ),
            ),
            const PopupMenuItem(
              value: 'report',
              child: Row(
                children: [
                  Icon(Icons.flag, size: 20),
                  SizedBox(width: 8),
                  Text('الإبلاغ عن مشكلة'),
                ],
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildContent(dynamic booking) {
    return SingleChildScrollView(
      child: Column(
        children: [
          _buildStatusCard(booking),
          _buildQRCode(booking),
          _buildPropertyCard(booking),
          _buildBookingInfo(booking),
          _buildGuestInfo(booking),
          if (booking.services.isNotEmpty) _buildServicesCard(booking),
          _buildPaymentInfo(booking),
          if (booking.canCancel) _buildCancellationPolicy(booking),
          _buildActions(booking),
          const SizedBox(height: AppDimensions.spacingXl),
        ],
      ),
    );
  }

  Widget _buildStatusCard(dynamic booking) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: _getStatusColor(booking.status).withOpacity(0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: _getStatusColor(booking.status)),
      ),
      child: Row(
        children: [
          Icon(
            _getStatusIcon(booking.status),
            color: _getStatusColor(booking.status),
            size: AppDimensions.iconLarge,
          ),
          const SizedBox(width: AppDimensions.spacingMd),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                BookingStatusWidget(status: booking.status),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  'رقم الحجز: ${booking.bookingNumber}',
                  style: AppTextStyles.bodyMedium.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  'تاريخ الحجز: ${DateFormat('dd/MM/yyyy').format(booking.bookingDate)}',
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

  Widget _buildQRCode(dynamic booking) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
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
      child: Column(
        children: [
          Text(
            'كود الحجز',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          QrImageView(
            data: booking.bookingNumber,
            version: QrVersions.auto,
            size: 150.0,
            backgroundColor: Colors.white,
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Text(
            'اعرض هذا الكود عند تسجيل الوصول',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPropertyCard(dynamic booking) {
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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'تفاصيل العقار',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Row(
            children: [
              if (booking.unitImages.isNotEmpty)
                ClipRRect(
                  borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                  child: Image.network(
                    booking.unitImages.first,
                    width: 80,
                    height: 80,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) => Container(
                      width: 80,
                      height: 80,
                      color: AppColors.shimmer,
                      child: Icon(
                        Icons.apartment,
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ),
                )
              else
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
                      booking.propertyName,
                      style: AppTextStyles.subtitle2.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    if (booking.unitName != null) ...[
                      const SizedBox(height: AppDimensions.spacingXs),
                      Text(
                        booking.unitName!,
                        style: AppTextStyles.bodyMedium,
                      ),
                    ],
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
                            booking.propertyAddress ?? 'العنوان غير متوفر',
                            style: AppTextStyles.caption.copyWith(
                              color: AppColors.textSecondary,
                            ),
                            maxLines: 2,
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
        ],
      ),
    );
  }

  Widget _buildBookingInfo(dynamic booking) {
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
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'معلومات الإقامة',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildInfoRow(
            icon: Icons.calendar_today,
            label: 'تاريخ الوصول',
            value: dateFormat.format(booking.checkInDate),
          ),
          const Divider(height: AppDimensions.spacingLg),
          _buildInfoRow(
            icon: Icons.calendar_today_outlined,
            label: 'تاريخ المغادرة',
            value: dateFormat.format(booking.checkOutDate),
          ),
          const Divider(height: AppDimensions.spacingLg),
          _buildInfoRow(
            icon: Icons.nights_stay,
            label: 'عدد الليالي',
            value: '${booking.numberOfNights} ${booking.numberOfNights == 1 ? 'ليلة' : 'ليالي'}',
          ),
        ],
      ),
    );
  }

  Widget _buildGuestInfo(dynamic booking) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
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
            'معلومات الضيوف',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildInfoRow(
            icon: Icons.person,
            label: 'اسم الضيف',
            value: booking.userName,
          ),
          const Divider(height: AppDimensions.spacingLg),
          _buildInfoRow(
            icon: Icons.people,
            label: 'عدد الضيوف',
            value: '${booking.totalGuests} ضيف (${booking.adultGuests} بالغ${booking.childGuests > 0 ? '، ${booking.childGuests} طفل' : ''})',
          ),
          if (booking.specialRequests != null) ...[
            const Divider(height: AppDimensions.spacingLg),
            _buildInfoRow(
              icon: Icons.note,
              label: 'طلبات خاصة',
              value: booking.specialRequests!,
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildServicesCard(dynamic booking) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
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
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          ...booking.services.map((service) => Padding(
            padding: const EdgeInsets.only(bottom: AppDimensions.spacingSm),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    '${service.serviceName} x${service.quantity}',
                    style: AppTextStyles.bodyMedium,
                  ),
                ),
                PriceWidget(
                  price: service.totalPrice,
                  currency: service.currency,
                  displayType: PriceDisplayType.compact,
                ),
              ],
            ),
          )).toList(),
        ],
      ),
    );
  }

  Widget _buildPaymentInfo(dynamic booking) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.primary.withOpacity(0.05),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.primary),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'معلومات الدفع',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
              color: AppColors.primary,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'المبلغ الإجمالي',
                style: AppTextStyles.bodyMedium,
              ),
              PriceWidget(
                price: booking.totalAmount,
                currency: booking.currency,
                displayType: PriceDisplayType.normal,
              ),
            ],
          ),
          if (booking.payments.isNotEmpty) ...[
            const Divider(height: AppDimensions.spacingLg),
            ...booking.payments.map((payment) => Padding(
              padding: const EdgeInsets.only(bottom: AppDimensions.spacingSm),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        payment.paymentMethod,
                        style: AppTextStyles.bodyMedium,
                      ),
                      Text(
                        DateFormat('dd/MM/yyyy').format(payment.paymentDate),
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                  Chip(
                    label: Text(
                      _getPaymentStatusText(payment.status),
                      style: AppTextStyles.caption.copyWith(
                        color: _getPaymentStatusColor(payment.status),
                      ),
                    ),
                    backgroundColor: _getPaymentStatusColor(payment.status).withOpacity(0.1),
                  ),
                ],
              ),
            )).toList(),
          ],
        ],
      ),
    );
  }

  Widget _buildCancellationPolicy(dynamic booking) {
    final now = DateTime.now();
    final cancellationDeadline = booking.checkInDate.subtract(const Duration(hours: 24));
    final canCancelFree = now.isBefore(cancellationDeadline);

    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: CancellationDeadlineHasExpiredWidget(
        hasExpired: !canCancelFree,
        deadline: cancellationDeadline,
      ),
    );
  }

  Widget _buildActions(dynamic booking) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        children: [
          if (booking.canModify)
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                onPressed: () => _modifyBooking(booking),
                icon: const Icon(Icons.edit),
                label: const Text('تعديل الحجز'),
                style: OutlinedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(
                    vertical: AppDimensions.paddingMedium,
                  ),
                ),
              ),
            ),
          if (booking.canCancel) ...[
            const SizedBox(height: AppDimensions.spacingMd),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: () => _cancelBooking(booking),
                icon: const Icon(Icons.cancel),
                label: const Text('إلغاء الحجز'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.error,
                  padding: const EdgeInsets.symmetric(
                    vertical: AppDimensions.paddingMedium,
                  ),
                ),
              ),
            ),
          ],
          if (booking.status == BookingStatus.completed && booking.canReview) ...[
            const SizedBox(height: AppDimensions.spacingMd),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: () => _writeReview(booking),
                icon: const Icon(Icons.rate_review),
                label: const Text('كتابة تقييم'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.secondary,
                  padding: const EdgeInsets.symmetric(
                    vertical: AppDimensions.paddingMedium,
                  ),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildInfoRow({
    required IconData icon,
    required String label,
    required String value,
  }) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(
          icon,
          size: AppDimensions.iconSmall,
          color: AppColors.textSecondary,
        ),
        const SizedBox(width: AppDimensions.spacingSm),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                value,
                style: AppTextStyles.bodyMedium.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Color _getStatusColor(BookingStatus status) {
    switch (status) {
      case BookingStatus.confirmed:
        return AppColors.success;
      case BookingStatus.pending:
        return AppColors.warning;
      case BookingStatus.cancelled:
        return AppColors.error;
      case BookingStatus.completed:
        return AppColors.info;
      case BookingStatus.checkedIn:
        return AppColors.primary;
    }
  }

  IconData _getStatusIcon(BookingStatus status) {
    switch (status) {
      case BookingStatus.confirmed:
        return Icons.check_circle;
      case BookingStatus.pending:
        return Icons.hourglass_empty;
      case BookingStatus.cancelled:
        return Icons.cancel;
      case BookingStatus.completed:
        return Icons.done_all;
      case BookingStatus.checkedIn:
        return Icons.login;
    }
  }

  String _getPaymentStatusText(dynamic status) {
    // Convert payment status to text
    return 'مدفوع';
  }

  Color _getPaymentStatusColor(dynamic status) {
    // Return color based on payment status
    return AppColors.success;
  }

  void _shareBooking() {
    // Implement share functionality
  }

  void _handleMenuAction(String action) {
    switch (action) {
      case 'download':
        // Download PDF
        break;
      case 'report':
        // Report issue
        break;
    }
  }

  void _modifyBooking(dynamic booking) {
    // Navigate to modify booking
  }

  void _cancelBooking(dynamic booking) {
    // Show cancel dialog
  }

  void _writeReview(dynamic booking) {
    context.push('/review/write', extra: {
      'bookingId': booking.id,
      'propertyId': booking.propertyId,
      'propertyName': booking.propertyName,
    });
  }
}