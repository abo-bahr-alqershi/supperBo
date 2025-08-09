import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../../../core/widgets/price_widget.dart';
import '../../../../injection_container.dart';
import '../../domain/entities/unit.dart';
import '../bloc/property_bloc.dart';
import '../bloc/property_event.dart';
import '../bloc/property_state.dart';

class PropertyUnitsPage extends StatefulWidget {
  final String propertyId;
  final String propertyName;
  final DateTime? checkInDate;
  final DateTime? checkOutDate;
  final int guestsCount;

  const PropertyUnitsPage({
    super.key,
    required this.propertyId,
    required this.propertyName,
    this.checkInDate,
    this.checkOutDate,
    this.guestsCount = 1,
  });

  @override
  State<PropertyUnitsPage> createState() => _PropertyUnitsPageState();
}

class _PropertyUnitsPageState extends State<PropertyUnitsPage> {
  late DateTime _checkInDate;
  late DateTime _checkOutDate;
  late int _guestsCount;
  String? _selectedUnitId;

  @override
  void initState() {
    super.initState();
    _checkInDate = widget.checkInDate ?? DateTime.now();
    _checkOutDate = widget.checkOutDate ?? DateTime.now().add(const Duration(days: 1));
    _guestsCount = widget.guestsCount;
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => sl<PropertyBloc>()
        ..add(GetPropertyUnitsEvent(
          propertyId: widget.propertyId,
          checkInDate: _checkInDate,
          checkOutDate: _checkOutDate,
          guestsCount: _guestsCount,
        )),
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: _buildAppBar(),
        body: Column(
          children: [
            _buildDateSelector(),
            Expanded(
              child: BlocBuilder<PropertyBloc, PropertyState>(
                builder: (context, state) {
                  if (state is PropertyUnitsLoading) {
                    return const Center(
                      child: LoadingWidget(
                        type: LoadingType.circular,
                        message: 'جاري البحث عن الوحدات المتاحة...',
                      ),
                    );
                  }

                  if (state is PropertyError) {
                    return Center(
                      child: CustomErrorWidget(
                        message: state.message,
                        onRetry: () => _loadUnits(context),
                      ),
                    );
                  }

                  if (state is PropertyUnitsLoaded) {
                    if (state.units.isEmpty) {
                      return EmptyWidget(
                        message: 'لا توجد وحدات متاحة في التواريخ المحددة',
                        actionWidget: ElevatedButton.icon(
                          onPressed: () => _showDatePicker(context),
                          icon: const Icon(Icons.calendar_today),
                          label: const Text('تغيير التواريخ'),
                        ),
                      );
                    }

                    return RefreshIndicator(
                      onRefresh: () async => _loadUnits(context),
                      child: ListView.separated(
                        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
                        itemCount: state.units.length,
                        separatorBuilder: (context, index) => const SizedBox(
                          height: AppDimensions.spacingMd,
                        ),
                        itemBuilder: (context, index) {
                          final unit = state.units[index];
                          return _buildUnitCard(context, unit, state.selectedUnitId == unit.id);
                        },
                      ),
                    );
                  }

                  return const SizedBox.shrink();
                },
              ),
            ),
          ],
        ),
        bottomNavigationBar: _buildBottomBar(),
      ),
    );
  }

  AppBar _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'الوحدات المتاحة',
            style: AppTextStyles.heading3,
          ),
          Text(
            widget.propertyName,
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
      actions: [
        IconButton(
          onPressed: () => _showFilterDialog(context),
          icon: const Icon(Icons.filter_list),
        ),
      ],
    );
  }

  Widget _buildDateSelector() {
    final nights = _checkOutDate.difference(_checkInDate).inDays;

    return Container(
      color: AppColors.surface,
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: _buildDateCard(
                  title: 'تسجيل الدخول',
                  date: _checkInDate,
                  icon: Icons.login,
                  onTap: () => _selectCheckInDate(context),
                ),
              ),
              Container(
                margin: const EdgeInsets.symmetric(horizontal: AppDimensions.spacingMd),
                child: Column(
                  children: [
                    const Icon(
                      Icons.nights_stay,
                      color: AppColors.primary,
                      size: 20,
                    ),
                    const SizedBox(height: AppDimensions.spacingXs),
                    Text(
                      '$nights',
                      style: AppTextStyles.subtitle2.copyWith(
                        color: AppColors.primary,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    Text(
                      nights == 1 ? 'ليلة' : 'ليالي',
                      style: AppTextStyles.caption.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: _buildDateCard(
                  title: 'تسجيل الخروج',
                  date: _checkOutDate,
                  icon: Icons.logout,
                  onTap: () => _selectCheckOutDate(context),
                ),
              ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildGuestsSelector(),
        ],
      ),
    );
  }

  Widget _buildDateCard({
    required String title,
    required DateTime date,
    required IconData icon,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.border),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon, size: 16, color: AppColors.textSecondary),
                const SizedBox(width: AppDimensions.spacingXs),
                Text(
                  title,
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppDimensions.spacingXs),
            Text(
              _formatDate(date),
              style: AppTextStyles.bodyMedium.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildGuestsSelector() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.border),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Row(
        children: [
          const Icon(Icons.people_outline, color: AppColors.textSecondary),
          const SizedBox(width: AppDimensions.spacingMd),
          const Expanded(
            child: Text(
              'عدد الضيوف',
              style: AppTextStyles.bodyMedium,
            ),
          ),
          IconButton(
            onPressed: _guestsCount > 1
                ? () {
                    setState(() {
                      _guestsCount--;
                    });
                    _loadUnits(context);
                  }
                : null,
            icon: const Icon(Icons.remove_circle_outline),
            color: AppColors.primary,
          ),
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
              vertical: AppDimensions.paddingSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
            ),
            child: Text(
              _guestsCount.toString(),
              style: AppTextStyles.subtitle1.copyWith(
                color: AppColors.primary,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          IconButton(
            onPressed: () {
              setState(() {
                _guestsCount++;
              });
              _loadUnits(context);
            },
            icon: const Icon(Icons.add_circle_outline),
            color: AppColors.primary,
          ),
        ],
      ),
    );
  }

  Widget _buildUnitCard(BuildContext context, Unit unit, bool isSelected) {
    return Card(
      elevation: isSelected ? 4 : 2,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        side: BorderSide(
          color: isSelected ? AppColors.primary : AppColors.transparent,
          width: 2,
        ),
      ),
      child: InkWell(
        onTap: () {
          setState(() {
            _selectedUnitId = unit.id;
          });
          context.read<PropertyBloc>().add(SelectUnitEvent(unitId: unit.id));
        },
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        child: Column(
          children: [
            if (unit.images.isNotEmpty)
              Container(
                height: 180,
                decoration: BoxDecoration(
                  borderRadius: const BorderRadius.vertical(
                    top: Radius.circular(AppDimensions.borderRadiusLg),
                  ),
                  image: DecorationImage(
                    image: NetworkImage(unit.images.first.url),
                    fit: BoxFit.cover,
                  ),
                ),
                child: Stack(
                  children: [
                    if (!unit.isAvailable)
                      Container(
                        decoration: BoxDecoration(
                          color: AppColors.black.withValues(alpha: 0.5),
                          borderRadius: const BorderRadius.vertical(
                            top: Radius.circular(AppDimensions.borderRadiusLg),
                          ),
                        ),
                        child: Center(
                          child: Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: AppDimensions.paddingMedium,
                              vertical: AppDimensions.paddingSmall,
                            ),
                            decoration: BoxDecoration(
                              color: AppColors.error,
                              borderRadius: BorderRadius.circular(
                                AppDimensions.borderRadiusMd,
                              ),
                            ),
                            child: Text(
                              'غير متاح',
                              style: AppTextStyles.bodyMedium.copyWith(
                                color: AppColors.white,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ),
                      ),
                    if (isSelected)
                      Positioned(
                        top: AppDimensions.paddingMedium,
                        right: AppDimensions.paddingMedium,
                        child: Container(
                          padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                          decoration: const BoxDecoration(
                            color: AppColors.primary,
                            shape: BoxShape.circle,
                          ),
                          child: const Icon(
                            Icons.check,
                            color: AppColors.white,
                            size: 20,
                          ),
                        ),
                      ),
                  ],
                ),
              ),
            Padding(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          unit.name,
                          style: AppTextStyles.subtitle1.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppDimensions.paddingSmall,
                          vertical: AppDimensions.paddingXSmall,
                        ),
                        decoration: BoxDecoration(
                          color: AppColors.primary.withValues(alpha: 0.1),
                          borderRadius: BorderRadius.circular(
                            AppDimensions.borderRadiusXs,
                          ),
                        ),
                        child: Text(
                          unit.unitTypeName,
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.primary,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: AppDimensions.spacingSm),
                  if (unit.customFeatures.isNotEmpty) ...[
                    Text(
                      unit.customFeatures,
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppColors.textSecondary,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: AppDimensions.spacingMd),
                  ],
                  _buildUnitFeatures(unit),
                  const SizedBox(height: AppDimensions.spacingMd),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      PriceWidget(
                        price: unit.basePrice.amount,
                        currency: unit.basePrice.currency,
                        period: _getPricingPeriod(unit.pricingMethod),
                        displayType: PriceDisplayType.normal,
                      ),
                      if (unit.isAvailable)
                        ElevatedButton(
                          onPressed: () => _selectUnit(context, unit),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: isSelected
                                ? AppColors.success
                                : AppColors.primary,
                            padding: const EdgeInsets.symmetric(
                              horizontal: AppDimensions.paddingMedium,
                              vertical: AppDimensions.paddingSmall,
                            ),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(
                                AppDimensions.borderRadiusSm,
                              ),
                            ),
                          ),
                          child: Text(
                            isSelected ? 'محدد' : 'اختيار',
                            style: AppTextStyles.button.copyWith(
                              color: AppColors.white,
                            ),
                          ),
                        ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildUnitFeatures(Unit unit) {
    if (unit.dynamicFields.isEmpty) return const SizedBox.shrink();

    return Wrap(
      spacing: AppDimensions.spacingSm,
      runSpacing: AppDimensions.spacingSm,
      children: unit.dynamicFields
          .expand((group) => group.fieldValues)
          .take(5)
          .map((field) {
        return Container(
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingSmall,
            vertical: AppDimensions.paddingXSmall,
          ),
          decoration: BoxDecoration(
            color: AppColors.background,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                _getFieldIcon(field.fieldName),
                size: 14,
                color: AppColors.textSecondary,
              ),
              const SizedBox(width: AppDimensions.spacingXs),
              Text(
                '${field.displayName}: ${field.value}',
                style: AppTextStyles.caption,
              ),
            ],
          ),
        );
      }).toList(),
    );
  }

  Widget _buildBottomBar() {
    return BlocBuilder<PropertyBloc, PropertyState>(
      builder: (context, state) {
        if (state is PropertyUnitsLoaded && _selectedUnitId != null) {
          final selectedUnit = state.units.firstWhere(
            (unit) => unit.id == _selectedUnitId,
          );

          final nights = _checkOutDate.difference(_checkInDate).inDays;
          final totalPrice = selectedUnit.basePrice.amount * nights;

          return Container(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            decoration: const BoxDecoration(
              color: AppColors.surface,
              boxShadow: [
                BoxShadow(
                  color: AppColors.shadow,
                  blurRadius: AppDimensions.blurLarge,
                  offset: Offset(0, -2),
                ),
              ],
            ),
            child: SafeArea(
              top: false,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'الوحدة المحددة',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                      Text(
                        selectedUnit.name,
                        style: AppTextStyles.bodyMedium.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: AppDimensions.spacingSm),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'المجموع ($nights ليالي)',
                        style: AppTextStyles.bodyMedium,
                      ),
                      PriceWidget(
                        price: totalPrice,
                        currency: selectedUnit.basePrice.currency,
                        displayType: PriceDisplayType.normal,
                      ),
                    ],
                  ),
                  const SizedBox(height: AppDimensions.spacingMd),
                  ElevatedButton(
                    onPressed: () => _proceedToBooking(context, selectedUnit),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      minimumSize: const Size(double.infinity, 48),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(
                          AppDimensions.borderRadiusMd,
                        ),
                      ),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.calendar_today, size: 20),
                        const SizedBox(width: AppDimensions.spacingSm),
                        Text(
                          'متابعة الحجز',
                          style: AppTextStyles.button.copyWith(
                            color: AppColors.white,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
        }

        return const SizedBox.shrink();
      },
    );
  }

  IconData _getFieldIcon(String fieldName) {
    switch (fieldName.toLowerCase()) {
      case 'beds':
      case 'أسرة':
        return Icons.bed;
      case 'bathrooms':
      case 'حمامات':
        return Icons.bathroom;
      case 'area':
      case 'المساحة':
        return Icons.square_foot;
      case 'floor':
      case 'الطابق':
        return Icons.stairs;
      case 'view':
      case 'الإطلالة':
        return Icons.landscape;
      default:
        return Icons.info_outline;
    }
  }

  String _getPricingPeriod(PricingMethod method) {
    switch (method) {
      case PricingMethod.hourly:
        return 'ساعة';
      case PricingMethod.daily:
        return 'ليلة';
      case PricingMethod.weekly:
        return 'أسبوع';
      case PricingMethod.monthly:
        return 'شهر';
    }
  }

  String _formatDate(DateTime date) {
    final months = [
      'يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو',
      'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'
    ];
    return '${date.day} ${months[date.month - 1]}';
  }

  void _loadUnits(BuildContext context) {
    context.read<PropertyBloc>().add(GetPropertyUnitsEvent(
      propertyId: widget.propertyId,
      checkInDate: _checkInDate,
      checkOutDate: _checkOutDate,
      guestsCount: _guestsCount,
    ));
  }

  void _selectCheckInDate(BuildContext context) async {
    final date = await showDatePicker(
      context: context,
      initialDate: _checkInDate,
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );

    if (!mounted) return;

    if (date != null) {
      setState(() {
        _checkInDate = date;
        if (_checkOutDate.isBefore(_checkInDate)) {
          _checkOutDate = _checkInDate.add(const Duration(days: 1));
        }
      });
      if (!mounted) return;
      _loadUnits(context);
    }
  }

  void _selectCheckOutDate(BuildContext context) async {
    final date = await showDatePicker(
      context: context,
      initialDate: _checkOutDate,
      firstDate: _checkInDate.add(const Duration(days: 1)),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );

    if (!mounted) return;

    if (date != null) {
      setState(() {
        _checkOutDate = date;
      });
      if (!mounted) return;
      _loadUnits(context);
    }
  }

  void _showDatePicker(BuildContext context) {
    _selectCheckInDate(context);
  }

  void _showFilterDialog(BuildContext context) {
    // Show filter dialog
  }

  void _selectUnit(BuildContext context, Unit unit) {
    setState(() {
      _selectedUnitId = unit.id;
    });
    context.read<PropertyBloc>().add(SelectUnitEvent(unitId: unit.id));
  }

  void _proceedToBooking(BuildContext context, Unit unit) {
    // Navigate to booking page with selected unit
  }
}