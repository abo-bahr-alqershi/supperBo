import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../../../core/enums/booking_status.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../bloc/booking_bloc.dart';
import '../bloc/booking_event.dart';
import '../bloc/booking_state.dart';
import '../widgets/booking_card_widget.dart';

class MyBookingsPage extends StatefulWidget {
  const MyBookingsPage({super.key});

  @override
  State<MyBookingsPage> createState() => _MyBookingsPageState();
}

class _MyBookingsPageState extends State<MyBookingsPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final ScrollController _scrollController = ScrollController();
  BookingStatus? _selectedStatus;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 4, vsync: this);
    _tabController.addListener(_onTabChanged);
    _scrollController.addListener(_onScroll);
    _loadBookings();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  void _onTabChanged() {
    if (!_tabController.indexIsChanging) {
      setState(() {
        switch (_tabController.index) {
          case 0:
            _selectedStatus = null;
            break;
          case 1:
            _selectedStatus = BookingStatus.confirmed;
            break;
          case 2:
            _selectedStatus = BookingStatus.pending;
            break;
          case 3:
            _selectedStatus = BookingStatus.completed;
            break;
        }
      });
      _loadBookings();
    }
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent * 0.9) {
      _loadMoreBookings();
    }
  }

  void _loadBookings() {
    final authState = context.read<AuthBloc>().state;
    if (authState is AuthAuthenticated) {
      context.read<BookingBloc>().add(
        GetUserBookingsEvent(
          userId: authState.user.userId,
          status: _selectedStatus?.toString().split('.').last,
          pageNumber: 1,
          pageSize: 10,
        ),
      );
    }
  }

  void _loadMoreBookings() {
    final state = context.read<BookingBloc>().state;
    if (state is UserBookingsLoaded && !state.isLoadingMore && state.hasMore) {
      final authState = context.read<AuthBloc>().state;
      if (authState is AuthAuthenticated) {
        context.read<BookingBloc>().add(
          GetUserBookingsEvent(
            userId: authState.user.userId,
            status: _selectedStatus?.toString().split('.').last,
            pageNumber: state.currentPage + 1,
            pageSize: 10,
            loadMore: true,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: _buildBody(),
    );
  }

  PreferredSizeWidget _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      title: Text(
        'حجوزاتي',
        style: AppTextStyles.heading3.copyWith(
          fontWeight: FontWeight.bold,
        ),
      ),
      actions: [
        IconButton(
          onPressed: _showFilterDialog,
          icon: const Icon(Icons.filter_list),
        ),
      ],
      bottom: TabBar(
        controller: _tabController,
        isScrollable: false,
        labelColor: AppColors.primary,
        unselectedLabelColor: AppColors.textSecondary,
        indicatorColor: AppColors.primary,
        indicatorWeight: 3,
        labelStyle: AppTextStyles.bodyMedium.copyWith(
          fontWeight: FontWeight.bold,
        ),
        tabs: const [
          Tab(text: 'الكل'),
          Tab(text: 'مؤكدة'),
          Tab(text: 'معلقة'),
          Tab(text: 'مكتملة'),
        ],
      ),
    );
  }

  Widget _buildBody() {
    return BlocBuilder<BookingBloc, BookingState>(
      builder: (context, state) {
        if (state is BookingLoading && state is! UserBookingsLoaded) {
          return const Center(
            child: LoadingWidget(
              type: LoadingType.circular,
              message: 'جاري تحميل الحجوزات...',
            ),
          );
        }

        if (state is BookingError) {
          return Center(
            child: CustomErrorWidget(
              message: state.message,
              onRetry: _loadBookings,
            ),
          );
        }

        if (state is UserBookingsLoaded) {
          if (state.bookings.isEmpty) {
            return _buildEmptyState();
          }

          return RefreshIndicator(
            onRefresh: () async => _loadBookings(),
            child: ListView.builder(
              controller: _scrollController,
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              itemCount: state.bookings.length + (state.isLoadingMore ? 1 : 0),
              itemBuilder: (context, index) {
                if (index == state.bookings.length) {
                  return const Padding(
                    padding: EdgeInsets.all(AppDimensions.paddingMedium),
                    child: Center(
                      child: CircularProgressIndicator(),
                    ),
                  );
                }

                final booking = state.bookings[index];
                return Padding(
                  padding: const EdgeInsets.only(
                    bottom: AppDimensions.spacingMd,
                  ),
                  child: BookingCardWidget(
                    booking: booking,
                    onTap: () => _navigateToDetails(booking.id),
                    onCancel: booking.canCancel
                        ? () => _showCancelDialog(booking)
                        : null,
                    onReview: booking.canReview
                        ? () => _navigateToReview(booking)
                        : null,
                  ),
                );
              },
            ),
          );
        }

        return const SizedBox.shrink();
      },
    );
  }

  Widget _buildEmptyState() {
    String message;
    switch (_selectedStatus) {
      case BookingStatus.confirmed:
        message = 'لا توجد حجوزات مؤكدة';
        break;
      case BookingStatus.pending:
        message = 'لا توجد حجوزات معلقة';
        break;
      case BookingStatus.completed:
        message = 'لا توجد حجوزات مكتملة';
        break;
      default:
        message = 'لا توجد حجوزات';
    }

    return Center(
      child: EmptyWidget(
        message: message,
        actionWidget: ElevatedButton.icon(
          onPressed: () => context.push('/search'),
          icon: const Icon(Icons.search),
          label: const Text('ابحث عن عقارات'),
          style: ElevatedButton.styleFrom(
            backgroundColor: AppColors.primary,
            foregroundColor: AppColors.white,
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingLarge,
              vertical: AppDimensions.paddingMedium,
            ),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            ),
          ),
        ),
      ),
    );
  }

  void _navigateToDetails(String bookingId) {
    context.push('/booking/$bookingId');
  }

  void _navigateToReview(dynamic booking) {
    context.push('/review/write', extra: {
      'bookingId': booking.id,
      'propertyId': booking.propertyId,
      'propertyName': booking.propertyName,
    });
  }

  void _showCancelDialog(dynamic booking) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('إلغاء الحجز'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'هل أنت متأكد من إلغاء هذا الحجز؟',
              style: AppTextStyles.bodyMedium,
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Container(
              padding: const EdgeInsets.all(AppDimensions.paddingSmall),
              decoration: BoxDecoration(
                color: AppColors.warning.withOpacity(0.1),
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                border: Border.all(color: AppColors.warning),
              ),
              child: Row(
                children: [
                  Icon(
                    Icons.info_outline,
                    color: AppColors.warning,
                    size: AppDimensions.iconSmall,
                  ),
                  const SizedBox(width: AppDimensions.spacingSm),
                  Expanded(
                    child: Text(
                      'قد يتم تطبيق رسوم إلغاء حسب سياسة الإلغاء',
                      style: AppTextStyles.caption.copyWith(
                        color: AppColors.warning,
                      ),
                    ),
                  ),
                ],
              ),
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
              _cancelBooking(booking);
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.error,
            ),
            child: const Text('تأكيد الإلغاء'),
          ),
        ],
      ),
    );
  }

  void _cancelBooking(dynamic booking) {
    final authState = context.read<AuthBloc>().state;
    if (authState is AuthAuthenticated) {
      context.read<BookingBloc>().add(
        CancelBookingEvent(
          bookingId: booking.id,
          userId: authState.user.userId,
          reason: 'إلغاء من قبل المستخدم',
        ),
      );
    }
  }

  void _showFilterDialog() {
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
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: AppColors.divider,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),
            const SizedBox(height: AppDimensions.spacingLg),
            Text(
              'تصفية الحجوزات',
              style: AppTextStyles.heading3.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingLg),
            // Add filter options here
            SafeArea(
              child: SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.pop(context);
                    _loadBookings();
                  },
                  child: const Text('تطبيق'),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}