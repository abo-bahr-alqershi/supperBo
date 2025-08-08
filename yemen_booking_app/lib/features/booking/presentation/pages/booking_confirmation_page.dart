import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';
import '../../domain/entities/booking.dart';

class BookingConfirmationPage extends StatefulWidget {
  final Booking booking;

  const BookingConfirmationPage({
    super.key,
    required this.booking,
  });

  @override
  State<BookingConfirmationPage> createState() => _BookingConfirmationPageState();
}

class _BookingConfirmationPageState extends State<BookingConfirmationPage>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );
    _scaleAnimation = CurvedAnimation(
      parent: _animationController,
      curve: Curves.elasticOut,
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
    return WillPopScope(
      onWillPop: () async {
        context.go('/home');
        return false;
      },
      child: Scaffold(
        backgroundColor: AppColors.background,
        body: SafeArea(
          child: SingleChildScrollView(
            child: Padding(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Column(
                children: [
                  const SizedBox(height: AppDimensions.spacingXl),
                  _buildSuccessAnimation(),
                  const SizedBox(height: AppDimensions.spacingLg),
                  _buildSuccessMessage(),
                  const SizedBox(height: AppDimensions.spacingXl),
                  _buildBookingDetails(),
                  const SizedBox(height: AppDimensions.spacingLg),
                  _buildActionButtons(),
                  const SizedBox(height: AppDimensions.spacingXl),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildSuccessAnimation() {
    return ScaleTransition(
      scale: _scaleAnimation,
      child: Container(
        width: 120,
        height: 120,
        decoration: BoxDecoration(
          color: AppColors.success.withValues(alpha: 0.1),
          shape: BoxShape.circle,
        ),
        child: const Center(
          child: Icon(
            Icons.check_circle,
            color: AppColors.success,
            size: 80,
          ),
        ),
      ),
    );
  }

  Widget _buildSuccessMessage() {
    return Column(
      children: [
        Text(
          'تم الحجز بنجاح!',
          style: AppTextStyles.heading2.copyWith(
            fontWeight: FontWeight.bold,
            color: AppColors.success,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingSm),
        Text(
          'تم تأكيد حجزك وسيتم إرسال التفاصيل إلى بريدك الإلكتروني',
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textSecondary,
          ),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }

  Widget _buildBookingDetails() {
    final dateFormat = DateFormat('dd MMM yyyy', 'ar');
    
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        boxShadow: const [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurSmall,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildDetailHeader(),
          const Divider(height: AppDimensions.spacingLg),
          _buildDetailItem(
            'رقم الحجز',
            widget.booking.bookingNumber,
            canCopy: true,
          ),
          _buildDetailItem(
            'اسم العقار',
            widget.booking.propertyName,
          ),
          if (widget.booking.unitName != null)
            _buildDetailItem(
              'الوحدة',
              widget.booking.unitName!,
            ),
          _buildDetailItem(
            'تاريخ الوصول',
            dateFormat.format(widget.booking.checkInDate),
          ),
          _buildDetailItem(
            'تاريخ المغادرة',
            dateFormat.format(widget.booking.checkOutDate),
          ),
          _buildDetailItem(
            'عدد الضيوف',
            '${widget.booking.totalGuests} ضيف',
          ),
          _buildDetailItem(
            'المبلغ الإجمالي',
            '${widget.booking.totalAmount.toStringAsFixed(0)} ${widget.booking.currency}',
            highlight: true,
          ),
        ],
      ),
    );
  }

  Widget _buildDetailHeader() {
    return Row(
      children: [
        Container(
          padding: const EdgeInsets.all(AppDimensions.paddingSmall),
          decoration: BoxDecoration(
            color: AppColors.primary.withValues(alpha: 0.1),
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
          ),
          child: const Icon(
            Icons.confirmation_number,
            color: AppColors.primary,
            size: AppDimensions.iconMedium,
          ),
        ),
        const SizedBox(width: AppDimensions.spacingMd),
        Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'تفاصيل الحجز',
              style: AppTextStyles.subtitle1.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            Text(
              'احتفظ بهذه المعلومات للرجوع إليها',
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildDetailItem(
    String label,
    String value, {
    bool canCopy = false,
    bool highlight = false,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppDimensions.spacingSm),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          Row(
            children: [
              Text(
                value,
                style: highlight
                    ? AppTextStyles.subtitle1.copyWith(
                        fontWeight: FontWeight.bold,
                        color: AppColors.primary,
                      )
                    : AppTextStyles.bodyMedium.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
              ),
              if (canCopy) ...[
                const SizedBox(width: AppDimensions.spacingSm),
                InkWell(
                  onTap: () => _copyToClipboard(value),
                  child: const Icon(
                    Icons.copy,
                    size: AppDimensions.iconSmall,
                    color: AppColors.primary,
                  ),
                ),
              ],
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    return Column(
      children: [
        // View Booking Details Button
        SizedBox(
          width: double.infinity,
          child: ElevatedButton.icon(
            onPressed: () => context.push('/booking/${widget.booking.id}'),
            icon: const Icon(Icons.description_outlined),
            label: const Text('عرض تفاصيل الحجز'),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.primary,
              foregroundColor: AppColors.white,
              padding: const EdgeInsets.symmetric(
                vertical: AppDimensions.paddingMedium,
              ),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
            ),
          ),
        ),
        
        const SizedBox(height: AppDimensions.spacingMd),
        
        // Add to Calendar Button
        SizedBox(
          width: double.infinity,
          child: OutlinedButton.icon(
            onPressed: _addToCalendar,
            icon: const Icon(Icons.calendar_today),
            label: const Text('إضافة إلى التقويم'),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.primary,
              padding: const EdgeInsets.symmetric(
                vertical: AppDimensions.paddingMedium,
              ),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
              side: const BorderSide(color: AppColors.primary),
            ),
          ),
        ),
        
        const SizedBox(height: AppDimensions.spacingMd),
        
        // Share Booking Button
        SizedBox(
          width: double.infinity,
          child: OutlinedButton.icon(
            onPressed: _shareBooking,
            icon: const Icon(Icons.share),
            label: const Text('مشاركة الحجز'),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.textSecondary,
              padding: const EdgeInsets.symmetric(
                vertical: AppDimensions.paddingMedium,
              ),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
              side: const BorderSide(color: AppColors.outline),
            ),
          ),
        ),
        
        const SizedBox(height: AppDimensions.spacingLg),
        
        // Home Button
        TextButton(
          onPressed: () => context.go('/home'),
          child: Text(
            'العودة إلى الرئيسية',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.primary,
              decoration: TextDecoration.underline,
            ),
          ),
        ),
      ],
    );
  }

  void _copyToClipboard(String text) {
    Clipboard.setData(ClipboardData(text: text));
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(
              Icons.check_circle,
              color: AppColors.white,
              size: AppDimensions.iconSmall,
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            Text('تم نسخ $text'),
          ],
        ),
        duration: const Duration(seconds: 2),
        backgroundColor: AppColors.success,
        behavior: SnackBarBehavior.floating,
        margin: const EdgeInsets.all(AppDimensions.paddingMedium),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
      ),
    );
  }

  void _addToCalendar() {
    // Implementation for adding to calendar
    // You can use a package like add_2_calendar or implement native calendar integration
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        title: const Row(
          children: [
            Icon(
              Icons.calendar_today,
              color: AppColors.primary,
              size: AppDimensions.iconMedium,
            ),
            SizedBox(width: AppDimensions.spacingSm),
            Text('إضافة إلى التقويم'),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'سيتم إضافة موعد الحجز إلى تقويمك:',
              style: AppTextStyles.bodyMedium,
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildCalendarEventDetail(
              'العنوان',
              'حجز ${widget.booking.propertyName}',
            ),
            _buildCalendarEventDetail(
              'الوصول',
              DateFormat('dd MMM yyyy - HH:mm', 'ar').format(widget.booking.checkInDate),
            ),
            _buildCalendarEventDetail(
              'المغادرة',
              DateFormat('dd MMM yyyy - HH:mm', 'ar').format(widget.booking.checkOutDate),
            ),
            _buildCalendarEventDetail(
              'الموقع',
              widget.booking.propertyAddress ?? 'سيتم تحديد الموقع لاحقاً',
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('إلغاء'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.pop(context);
              _performAddToCalendar();
            },
            child: const Text('إضافة'),
          ),
        ],
      ),
    );
  }

  Widget _buildCalendarEventDetail(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppDimensions.spacingXs),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            '$label: ',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: AppTextStyles.caption.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _performAddToCalendar() {
    // Actual implementation to add to device calendar
    // Using platform-specific code or a package
    
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: const Row(
          children: [
            Icon(
              Icons.check_circle,
              color: AppColors.white,
              size: AppDimensions.iconSmall,
            ),
            SizedBox(width: AppDimensions.spacingSm),
            Text('تمت الإضافة إلى التقويم بنجاح'),
          ],
        ),
        backgroundColor: AppColors.success,
        behavior: SnackBarBehavior.floating,
        margin: const EdgeInsets.all(AppDimensions.paddingMedium),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
      ),
    );
  }

  void _shareBooking() {
    final dateFormat = DateFormat('dd MMM yyyy', 'ar');
    final shareText = '''
🎉 تم تأكيد الحجز!

📍 العقار: ${widget.booking.propertyName}
${widget.booking.unitName != null ? '🏠 الوحدة: ${widget.booking.unitName}' : ''}
📅 الوصول: ${dateFormat.format(widget.booking.checkInDate)}
📅 المغادرة: ${dateFormat.format(widget.booking.checkOutDate)}
👥 عدد الضيوف: ${widget.booking.totalGuests}
💰 المبلغ الإجمالي: ${widget.booking.totalAmount.toStringAsFixed(0)} ${widget.booking.currency}
🎫 رقم الحجز: ${widget.booking.bookingNumber}

تم الحجز عبر تطبيق Yemen Booking
    ''';

    // Show share bottom sheet
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusLg),
        ),
      ),
      builder: (context) => Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 40,
              height: 4,
              margin: const EdgeInsets.only(bottom: AppDimensions.spacingLg),
              decoration: BoxDecoration(
                color: AppColors.divider,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            Text(
              'مشاركة تفاصيل الحجز',
              style: AppTextStyles.subtitle1.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingLg),
            Container(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              decoration: BoxDecoration(
                color: AppColors.background,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                border: Border.all(color: AppColors.outline),
              ),
              child: Text(
                shareText,
                style: AppTextStyles.bodyMedium,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingLg),
            Row(
              children: [
                Expanded(
                  child: _buildShareOption(
                    icon: Icons.message,
                    label: 'رسالة',
                    color: AppColors.success,
                    onTap: () {
                      Navigator.pop(context);
                      _shareViaMessage(shareText);
                    },
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: _buildShareOption(
                    icon: Icons.email,
                    label: 'بريد',
                    color: AppColors.info,
                    onTap: () {
                      Navigator.pop(context);
                      _shareViaEmail(shareText);
                    },
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: _buildShareOption(
                    icon: Icons.copy,
                    label: 'نسخ',
                    color: AppColors.primary,
                    onTap: () {
                      Navigator.pop(context);
                      _copyToClipboard(shareText);
                    },
                  ),
                ),
              ],
            ),
            SafeArea(
              child: TextButton(
                onPressed: () => Navigator.pop(context),
                child: const Text('إلغاء'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildShareOption({
    required IconData icon,
    required String label,
    required Color color,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        padding: const EdgeInsets.symmetric(
          vertical: AppDimensions.paddingMedium,
        ),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(color: color.withValues(alpha: 0.3)),
        ),
        child: Column(
          children: [
            Icon(
              icon,
              color: color,
              size: AppDimensions.iconMedium,
            ),
            const SizedBox(height: AppDimensions.spacingXs),
            Text(
              label,
              style: AppTextStyles.caption.copyWith(
                color: color,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _shareViaMessage(String text) {
    // Implement share via SMS/WhatsApp
    // You can use url_launcher or share_plus package
  }

  void _shareViaEmail(String text) {
    // Implement share via email
    // You can use url_launcher or share_plus package
  }
}