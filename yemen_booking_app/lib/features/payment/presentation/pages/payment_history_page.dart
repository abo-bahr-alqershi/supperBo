/// features/payment/presentation/pages/payment_history_page.dart

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../../../core/enums/payment_method_enum.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../domain/entities/transaction.dart';
import '../bloc/payment_bloc.dart';
import '../bloc/payment_event.dart';
import '../bloc/payment_state.dart';
import '../widgets/transaction_item_widget.dart';

class PaymentHistoryPage extends StatefulWidget {
  const PaymentHistoryPage({super.key});

  @override
  State<PaymentHistoryPage> createState() => _PaymentHistoryPageState();
}

class _PaymentHistoryPageState extends State<PaymentHistoryPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final ScrollController _scrollController = ScrollController();
  
  // Filter states
  PaymentStatus? _selectedStatus;
  PaymentMethod? _selectedMethod;
  PaymentMethod? _selectedPaymentMethod;
  DateTimeRange? _selectedDateRange;
  DateTime? _fromDate;
  DateTime? _toDate;
  double? _minAmount;
  double? _maxAmount;
  bool _showDateFilter = false;
  int _currentPage = 1;
  
  // Statistics
  double _totalSpent = 0;
  int _totalTransactions = 0;
  double _averageTransaction = 0;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _scrollController.addListener(_onScroll);
    _loadPaymentHistory();
    _calculateStatistics();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent * 0.9) {
      _loadMoreTransactions();
    }
  }

  void _loadPaymentHistory() {
    final authState = context.read<AuthBloc>().state;
    if (authState is AuthAuthenticated) {
      context.read<PaymentBloc>().add(
        GetPaymentHistoryEvent(
          userId: authState.user.userId,
          status: _selectedStatus?.name,
          paymentMethod: _selectedMethod?.name,
          fromDate: _selectedDateRange?.start,
          toDate: _selectedDateRange?.end,
        ),
      );
    }
  }

  void _loadMoreTransactions() {
    final state = context.read<PaymentBloc>().state;
    if (state is PaymentHistoryLoaded && !state.isLoadingMore && state.hasMore) {
      final authState = context.read<AuthBloc>().state;
      if (authState is AuthAuthenticated) {
        context.read<PaymentBloc>().add(
          LoadMorePaymentHistoryEvent(
            userId: authState.user.userId,
            status: _selectedStatus?.name,
            paymentMethod: _selectedMethod?.name,
            fromDate: _selectedDateRange?.start,
            toDate: _selectedDateRange?.end,
          ),
        );
      }
    }
  }

  void _calculateStatistics() {
    final state = context.read<PaymentBloc>().state;
    if (state is PaymentHistoryLoaded) {
      _totalTransactions = state.totalCount;
      _totalSpent = state.transactions
          .where((t) => t.isSuccessful)
          .fold(0, (sum, t) => sum + t.totalAmount);
      _averageTransaction = _totalTransactions > 0 
          ? _totalSpent / _totalTransactions 
          : 0;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: NestedScrollView(
        headerSliverBuilder: (context, innerBoxIsScrolled) => [
          _buildSliverAppBar(),
          _buildStatisticsSection(),
          _buildFilterSection(),
        ],
        body: _buildTransactionsList(),
      ),
      floatingActionButton: _buildExportFAB(),
    );
  }

  Widget _buildSliverAppBar() {
    return SliverAppBar(
      expandedHeight: 200,
      pinned: true,
      backgroundColor: AppColors.primary,
      flexibleSpace: FlexibleSpaceBar(
        background: Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [
                AppColors.primary,
                AppColors.primaryDark,
              ],
            ),
          ),
          child: Stack(
            children: [
              Positioned(
                right: -50,
                top: 20,
                child: Container(
                  width: 150,
                  height: 150,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: AppColors.white.withValues(alpha: 0.1),
                  ),
                ),
              ),
              Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(
                      Icons.history,
                      size: AppDimensions.iconXLarge,
                      color: AppColors.white,
                    ),
                    const SizedBox(height: AppDimensions.spacingMd),
                    Text(
                      'سجل المدفوعات',
                      style: AppTextStyles.heading2.copyWith(
                        color: AppColors.white,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: AppDimensions.spacingSm),
                    Text(
                      'تتبع جميع معاملاتك المالية',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppColors.white.withValues(alpha: 0.8),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildStatisticsSection() {
    return SliverToBoxAdapter(
      child: Container(
        height: 120,
        margin: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Row(
          children: [
            Expanded(
              child: _buildStatCard(
                icon: Icons.account_balance_wallet,
                title: 'إجمالي المصروفات',
                value: '${_totalSpent.toStringAsFixed(0)} ريال',
                color: AppColors.primary,
              ),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            Expanded(
              child: _buildStatCard(
                icon: Icons.receipt_long,
                title: 'عدد المعاملات',
                value: _totalTransactions.toString(),
                color: AppColors.secondary,
              ),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            Expanded(
              child: _buildStatCard(
                icon: Icons.trending_up,
                title: 'متوسط المعاملة',
                value: '${_averageTransaction.toStringAsFixed(0)} ريال',
                color: AppColors.accent,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStatCard({
    required IconData icon,
    required String title,
    required String value,
    required Color color,
  }) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        boxShadow: [
          BoxShadow(
            color: color.withValues(alpha: 0.2),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Container(
            padding: const EdgeInsets.all(AppDimensions.paddingSmall),
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
            ),
            child: Icon(
              icon,
              color: color,
              size: AppDimensions.iconSmall,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            title,
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: AppDimensions.spacingXs),
          Text(
            value,
            style: AppTextStyles.subtitle2.copyWith(
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFilterSection() {
    return SliverToBoxAdapter(
      child: Container(
        height: 50,
        margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
        child: ListView(
          scrollDirection: Axis.horizontal,
          children: [
            _buildFilterChip(
              label: 'الكل',
              isSelected: _selectedStatus == null && _selectedMethod == null,
              onTap: () {
                setState(() {
                  _selectedStatus = null;
                  _selectedMethod = null;
                });
                _loadPaymentHistory();
              },
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            _buildFilterChip(
              label: 'ناجحة',
              isSelected: _selectedStatus == PaymentStatus.successful,
              onTap: () {
                setState(() {
                  _selectedStatus = PaymentStatus.successful;
                });
                _loadPaymentHistory();
              },
              color: AppColors.success,
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            _buildFilterChip(
              label: 'معلقة',
              isSelected: _selectedStatus == PaymentStatus.pending,
              onTap: () {
                setState(() {
                  _selectedStatus = PaymentStatus.pending;
                });
                _loadPaymentHistory();
              },
              color: AppColors.warning,
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            _buildFilterChip(
              label: 'فاشلة',
              isSelected: _selectedStatus == PaymentStatus.failed,
              onTap: () {
                setState(() {
                  _selectedStatus = PaymentStatus.failed;
                });
                _loadPaymentHistory();
              },
              color: AppColors.error,
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            _buildFilterChip(
              label: 'التاريخ',
              icon: Icons.calendar_today,
              isSelected: _selectedDateRange != null,
              onTap: _selectDateRange,
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            _buildFilterChip(
              label: 'طريقة الدفع',
              icon: Icons.payment,
              isSelected: _selectedMethod != null,
              onTap: _selectPaymentMethod,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFilterChip({
    required String label,
    IconData? icon,
    required bool isSelected,
    required VoidCallback onTap,
    Color? color,
  }) {
    return FilterChip(
      label: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) ...[
            Icon(
              icon,
              size: AppDimensions.iconSmall,
              color: isSelected ? AppColors.white : (color ?? AppColors.textSecondary),
            ),
            const SizedBox(width: AppDimensions.spacingXs),
          ],
          Text(
            label,
            style: AppTextStyles.bodyMedium.copyWith(
              color: isSelected ? AppColors.white : (color ?? AppColors.textPrimary),
              fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
            ),
          ),
        ],
      ),
      selected: isSelected,
      onSelected: (_) => onTap(),
      backgroundColor: AppColors.surface,
      selectedColor: color ?? AppColors.primary,
      checkmarkColor: AppColors.white,
      elevation: isSelected ? 4 : 0,
      shadowColor: (color ?? AppColors.primary).withValues(alpha: 0.3),
    );
  }

  Widget _buildTransactionsList() {
    return BlocBuilder<PaymentBloc, PaymentState>(
      builder: (context, state) {
        if (state is PaymentHistoryLoading) {
          return const Center(
            child: LoadingWidget(
              type: LoadingType.circular,
              message: 'جاري تحميل سجل المدفوعات...',
            ),
          );
        }

        if (state is PaymentError) {
          return Center(
            child: CustomErrorWidget(
              message: state.message,
              onRetry: _loadPaymentHistory,
            ),
          );
        }

        if (state is PaymentHistoryLoaded) {
          if (state.transactions.isEmpty) {
            return const Center(
              child: EmptyWidget(
                message: 'لا توجد معاملات مالية',
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: () async => _loadPaymentHistory(),
            child: ListView.builder(
              controller: _scrollController,
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              itemCount: state.transactions.length + (state.isLoadingMore ? 1 : 0),
              itemBuilder: (context, index) {
                if (index == state.transactions.length) {
                  return const Padding(
                    padding: EdgeInsets.all(AppDimensions.paddingMedium),
                    child: Center(
                      child: CircularProgressIndicator(),
                    ),
                  );
                }

                final transaction = state.transactions[index];
                return Padding(
                  padding: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
                  child: TransactionItemWidget(
                    transaction: transaction,
                    onTap: () => _showTransactionDetails(transaction),
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

  Widget _buildExportFAB() {
    return FloatingActionButton.extended(
      onPressed: _exportTransactions,
      backgroundColor: AppColors.primary,
      icon: const Icon(Icons.download, color: AppColors.white),
      label: Text(
        'تصدير',
        style: AppTextStyles.button.copyWith(color: AppColors.white),
      ),
    );
  }

  void _selectDateRange() async {
    final picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
      initialDateRange: _selectedDateRange,
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: AppColors.primary,
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null) {
      setState(() {
        _selectedDateRange = picked;
      });
      _loadPaymentHistory();
    }
  }

  void _selectPaymentMethod() {
    showModalBottomSheet(
      context: context,
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
                margin: const EdgeInsets.only(bottom: AppDimensions.spacingLg),
                decoration: BoxDecoration(
                  color: AppColors.divider,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),
            Text(
              'اختر طريقة الدفع',
              style: AppTextStyles.subtitle1.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingLg),
            ...PaymentMethod.values.map((method) => ListTile(
              leading: Icon(
                _getMethodIcon(method),
                color: _selectedMethod == method ? AppColors.primary : null,
              ),
              title: Text(method.displayNameAr),
              selected: _selectedMethod == method,
              onTap: () {
                setState(() {
                  _selectedMethod = method;
                });
                Navigator.pop(context);
                _loadPaymentHistory();
              },
            )).toList(),
            const SafeArea(child: SizedBox.shrink()),
          ],
        ),
      ),
    );
  }

  void _showTransactionDetails(Transaction transaction) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusLg),
        ),
      ),
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.7,
        minChildSize: 0.5,
        maxChildSize: 0.9,
        expand: false,
        builder: (context, scrollController) => SingleChildScrollView(
          controller: scrollController,
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Center(
                child: Container(
                  width: 40,
                  height: 4,
                  margin: const EdgeInsets.only(bottom: AppDimensions.spacingLg),
                  decoration: BoxDecoration(
                    color: AppColors.divider,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'تفاصيل المعاملة',
                    style: AppTextStyles.heading3.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  IconButton(
                    onPressed: () => Navigator.pop(context),
                    icon: const Icon(Icons.close),
                  ),
                ],
              ),
              const SizedBox(height: AppDimensions.spacingLg),
              _buildDetailItem('رقم المعاملة', transaction.transactionId ?? 'غير متوفر'),
              _buildDetailItem('رقم الحجز', transaction.bookingNumber),
              _buildDetailItem('اسم العقار', transaction.propertyName),
              _buildDetailItem('الوحدة', transaction.unitName),
              _buildDetailItem('المبلغ', '${transaction.amount.toStringAsFixed(2)} ${transaction.currency}'),
              _buildDetailItem('الرسوم', '${transaction.fees.toStringAsFixed(2)} ${transaction.currency}'),
              _buildDetailItem('الضرائب', '${transaction.taxes.toStringAsFixed(2)} ${transaction.currency}'),
              _buildDetailItem('المبلغ الإجمالي', '${transaction.totalAmount.toStringAsFixed(2)} ${transaction.currency}'),
              _buildDetailItem('طريقة الدفع', transaction.paymentMethod.displayNameAr),
              _buildDetailItem('الحالة', transaction.status.displayNameAr),
              _buildDetailItem('التاريخ', DateFormat('dd/MM/yyyy HH:mm').format(transaction.createdAt)),
              if (transaction.processedAt != null)
                _buildDetailItem('تاريخ المعالجة', DateFormat('dd/MM/yyyy HH:mm').format(transaction.processedAt!)),
              if (transaction.failureReason != null)
                _buildDetailItem('سبب الفشل', transaction.failureReason!, isError: true),
              const SizedBox(height: AppDimensions.spacingLg),
              if (transaction.canRefund)
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton.icon(
                    onPressed: () => _requestRefund(transaction),
                    icon: const Icon(Icons.replay),
                    label: const Text('طلب استرداد'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.warning,
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDetailItem(String label, String value, {bool isError = false}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppDimensions.spacingSm),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              label,
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: AppTextStyles.bodyMedium.copyWith(
                fontWeight: FontWeight.bold,
                color: isError ? AppColors.error : null,
              ),
            ),
          ),
        ],
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

  void _requestRefund(Transaction transaction) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        title: const Text('طلب استرداد'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text('هل أنت متأكد من طلب استرداد هذا المبلغ؟'),
            const SizedBox(height: AppDimensions.spacingMd),
            Container(
              padding: const EdgeInsets.all(AppDimensions.paddingSmall),
              decoration: BoxDecoration(
                color: AppColors.warning.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                border: Border.all(color: AppColors.warning),
              ),
              child: Row(
                children: [
                  const Icon(
                    Icons.info_outline,
                    color: AppColors.warning,
                    size: AppDimensions.iconSmall,
                  ),
                  const SizedBox(width: AppDimensions.spacingSm),
                  Expanded(
                    child: Text(
                      'قد يستغرق الاسترداد 3-5 أيام عمل',
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
              Navigator.pop(context);
              _processRefund(transaction);
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.warning,
            ),
            child: const Text('تأكيد الاسترداد'),
          ),
        ],
      ),
    );
  }

  void _processRefund(Transaction transaction) {
    final authState = context.read<AuthBloc>().state;
    if (authState is AuthAuthenticated) {
      context.read<PaymentBloc>().add(
        RefundPaymentEvent(
          transactionId: transaction.id,
          userId: authState.user.userId,
          amount: transaction.totalAmount,
          reason: 'طلب من المستخدم',
        ),
      );
    }
  }

  void _exportTransactions() {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: const Row(
          children: [
            CircularProgressIndicator(
              strokeWidth: 2,
              valueColor: AlwaysStoppedAnimation<Color>(AppColors.white),
            ),
            SizedBox(width: 16),
            Text('جاري تصدير البيانات...'),
          ],
        ),
        backgroundColor: AppColors.primary,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
      ),
    );
  }

  void _selectDate(bool isFromDate) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: isFromDate ? (_fromDate ?? DateTime.now()) : (_toDate ?? DateTime.now()),
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: AppColors.primary,
              onPrimary: AppColors.white,
              surface: AppColors.surface,
              onSurface: AppColors.textPrimary,
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null) {
      setState(() {
        if (isFromDate) {
          _fromDate = picked;
        } else {
          _toDate = picked;
        }
      });
    }
  }

  void _clearDateFilter() {
    setState(() {
      _fromDate = null;
      _toDate = null;
    });
  }

  void _applyDateFilter() {
    _currentPage = 1;
    _loadPaymentHistory();
  }

  Map<String, dynamic> _calculateSummary(List<Transaction> transactions) {
    if (transactions.isEmpty) {
      return {'total': 0.0, 'count': 0, 'lastDate': '--'};
    }

    final total = transactions
        .where((t) => t.status == PaymentStatus.successful)
        .fold<double>(0, (sum, t) => sum + t.amount);

    final lastDate = transactions.isNotEmpty
        ? DateFormat('dd/MM').format(transactions.first.createdAt)
        : '--';

    return {
      'total': total,
      'count': transactions.length,
      'lastDate': lastDate,
    };
  }
}