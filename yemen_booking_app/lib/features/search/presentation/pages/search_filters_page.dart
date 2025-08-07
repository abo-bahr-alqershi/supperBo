import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../bloc/search_bloc.dart';
import '../bloc/search_state.dart';
import '../widgets/price_range_slider_widget.dart';
import '../widgets/dynamic_fields_widget.dart';

class SearchFiltersPage extends StatefulWidget {
  final Map<String, dynamic>? initialFilters;

  const SearchFiltersPage({
    super.key,
    this.initialFilters,
  });

  @override
  State<SearchFiltersPage> createState() => _SearchFiltersPageState();
}

class _SearchFiltersPageState extends State<SearchFiltersPage> {
  late Map<String, dynamic> _filters;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _filters = widget.initialFilters ?? {};
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: BlocBuilder<SearchBloc, SearchState>(
        builder: (context, state) {
          if (state is SearchFiltersLoaded) {
            return _buildFiltersContent(state.filters);
          } else if (state is SearchFiltersLoading) {
            return const Center(child: CircularProgressIndicator());
          }
          return _buildFiltersContent(null);
        },
      ),
      bottomNavigationBar: _buildBottomBar(),
    );
  }

  PreferredSizeWidget _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      leading: IconButton(
        onPressed: () => Navigator.pop(context),
        icon: const Icon(Icons.close_rounded),
      ),
      title: Text(
        'الفلاتر والترتيب',
        style: AppTextStyles.heading3,
      ),
      actions: [
        TextButton(
          onPressed: _resetFilters,
          child: Text(
            'إعادة تعيين',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.primary,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildFiltersContent(dynamic filters) {
    return CustomScrollView(
      controller: _scrollController,
      slivers: [
        SliverToBoxAdapter(
          child: Column(
            children: [
              _buildDateRangeSection(),
              _buildDivider(),
              _buildGuestsSection(),
              _buildDivider(),
              _buildPropertyTypeSection(),
              _buildDivider(),
              _buildPriceRangeSection(),
              _buildDivider(),
              _buildStarRatingSection(),
              _buildDivider(),
              _buildAmenitiesSection(),
              _buildDivider(),
              _buildServicesSection(),
              _buildDivider(),
              _buildDynamicFieldsSection(),
              const SizedBox(height: AppDimensions.spacingXl),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildDateRangeSection() {
    return _FilterSection(
      title: 'تاريخ الإقامة',
      icon: Icons.calendar_today_rounded,
      child: Column(
        children: [
          _buildDateSelector(
            label: 'تسجيل الدخول',
            date: _filters['checkIn'],
            onTap: () => _selectDate('checkIn'),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildDateSelector(
            label: 'تسجيل الخروج',
            date: _filters['checkOut'],
            onTap: () => _selectDate('checkOut'),
          ),
        ],
      ),
    );
  }

  Widget _buildDateSelector({
    required String label,
    DateTime? date,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(
            color: date != null ? AppColors.primary : AppColors.outline,
            width: date != null ? 2 : 1,
          ),
        ),
        child: Row(
          children: [
            Icon(
              Icons.calendar_month_rounded,
              color: date != null ? AppColors.primary : AppColors.textSecondary,
            ),
            const SizedBox(width: AppDimensions.spacingMd),
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
                  const SizedBox(height: AppDimensions.spacingXs),
                  Text(
                    date != null
                        ? _formatDate(date)
                        : 'اختر التاريخ',
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: date != null
                          ? AppColors.textPrimary
                          : AppColors.textSecondary,
                      fontWeight: date != null ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                ],
              ),
            ),
            if (date != null)
              IconButton(
                onPressed: () {
                  setState(() {
                    _filters.remove(label == 'تسجيل الدخول' ? 'checkIn' : 'checkOut');
                  });
                },
                icon: const Icon(
                  Icons.clear_rounded,
                  size: AppDimensions.iconSmall,
                ),
                color: AppColors.textSecondary,
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildGuestsSection() {
    final guestsCount = _filters['guestsCount'] ?? 1;
    
    return _FilterSection(
      title: 'عدد الضيوف',
      icon: Icons.people_rounded,
      child: Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(color: AppColors.outline),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              '$guestsCount ${guestsCount == 1 ? 'ضيف' : 'ضيوف'}',
              style: AppTextStyles.bodyLarge.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            Row(
              children: [
                _buildCounterButton(
                  icon: Icons.remove_rounded,
                  onPressed: guestsCount > 1
                      ? () {
                          setState(() {
                            _filters['guestsCount'] = guestsCount - 1;
                          });
                        }
                      : null,
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingMedium,
                    vertical: AppDimensions.paddingSmall,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.primary.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                  ),
                  child: Text(
                    guestsCount.toString(),
                    style: AppTextStyles.bodyLarge.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                _buildCounterButton(
                  icon: Icons.add_rounded,
                  onPressed: () {
                    setState(() {
                      _filters['guestsCount'] = guestsCount + 1;
                    });
                  },
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCounterButton({
    required IconData icon,
    VoidCallback? onPressed,
  }) {
    return Material(
      color: onPressed != null
          ? AppColors.primary
          : AppColors.textSecondary.withOpacity(0.3),
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
      child: InkWell(
        onTap: onPressed,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        child: Container(
          padding: const EdgeInsets.all(AppDimensions.paddingSmall),
          child: Icon(
            icon,
            size: AppDimensions.iconSmall,
            color: AppColors.white,
          ),
        ),
      ),
    );
  }

  Widget _buildPropertyTypeSection() {
    final propertyTypes = [
      {'id': '1', 'name': 'فندق', 'icon': Icons.hotel_rounded},
      {'id': '2', 'name': 'شقة', 'icon': Icons.apartment_rounded},
      {'id': '3', 'name': 'فيلا', 'icon': Icons.villa_rounded},
      {'id': '4', 'name': 'منتجع', 'icon': Icons.beach_access_rounded},
    ];

    return _FilterSection(
      title: 'نوع العقار',
      icon: Icons.home_rounded,
      child: Wrap(
        spacing: AppDimensions.spacingSm,
        runSpacing: AppDimensions.spacingSm,
        children: propertyTypes.map((type) {
          final isSelected = _filters['propertyTypeId'] == type['id'];
          
          return InkWell(
            onTap: () {
              setState(() {
                if (isSelected) {
                  _filters.remove('propertyTypeId');
                } else {
                  _filters['propertyTypeId'] = type['id'];
                }
              });
            },
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              decoration: BoxDecoration(
                color: isSelected
                    ? AppColors.primary
                    : AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
                border: Border.all(
                  color: isSelected
                      ? AppColors.primary
                      : AppColors.outline,
                ),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(
                    type['icon'] as IconData,
                    size: AppDimensions.iconSmall,
                    color: isSelected
                        ? AppColors.white
                        : AppColors.textSecondary,
                  ),
                  const SizedBox(width: AppDimensions.spacingXs),
                  Text(
                    type['name'] as String,
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: isSelected
                          ? AppColors.white
                          : AppColors.textPrimary,
                      fontWeight: isSelected
                          ? FontWeight.bold
                          : FontWeight.normal,
                    ),
                  ),
                ],
              ),
            ),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildPriceRangeSection() {
    return _FilterSection(
      title: 'نطاق السعر',
      icon: Icons.attach_money_rounded,
      child: PriceRangeSliderWidget(
        minPrice: _filters['minPrice']?.toDouble() ?? 0,
        maxPrice: _filters['maxPrice']?.toDouble() ?? 100000,
        onChanged: (min, max) {
          setState(() {
            _filters['minPrice'] = min;
            _filters['maxPrice'] = max;
          });
        },
      ),
    );
  }

  Widget _buildStarRatingSection() {
    return _FilterSection(
      title: 'تقييم النجوم',
      icon: Icons.star_rounded,
      child: Wrap(
        spacing: AppDimensions.spacingSm,
        children: List.generate(5, (index) {
          final rating = 5 - index;
          final isSelected = _filters['minStarRating'] == rating;
          
          return InkWell(
            onTap: () {
              setState(() {
                if (isSelected) {
                  _filters.remove('minStarRating');
                } else {
                  _filters['minStarRating'] = rating;
                }
              });
            },
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              decoration: BoxDecoration(
                color: isSelected
                    ? AppColors.ratingStar.withOpacity(0.2)
                    : AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
                border: Border.all(
                  color: isSelected
                      ? AppColors.ratingStar
                      : AppColors.outline,
                ),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  ...List.generate(rating, (i) {
                    return Icon(
                      Icons.star_rounded,
                      size: AppDimensions.iconSmall,
                      color: AppColors.ratingStar,
                    );
                  }),
                  if (rating < 5) ...[
                    const SizedBox(width: AppDimensions.spacingXs),
                    Text(
                      'فما فوق',
                      style: AppTextStyles.caption,
                    ),
                  ],
                ],
              ),
            ),
          );
        }),
      ),
    );
  }

  Widget _buildAmenitiesSection() {
    final amenities = [
      {'id': '1', 'name': 'واي فاي', 'icon': Icons.wifi_rounded},
      {'id': '2', 'name': 'موقف سيارات', 'icon': Icons.local_parking_rounded},
      {'id': '3', 'name': 'مسبح', 'icon': Icons.pool_rounded},
      {'id': '4', 'name': 'مطعم', 'icon': Icons.restaurant_rounded},
      {'id': '5', 'name': 'صالة رياضية', 'icon': Icons.fitness_center_rounded},
      {'id': '6', 'name': 'سبا', 'icon': Icons.spa_rounded},
      {'id': '7', 'name': 'غرفة اجتماعات', 'icon': Icons.meeting_room_rounded},
      {'id': '8', 'name': 'مكيف', 'icon': Icons.ac_unit_rounded},
    ];

    final selectedAmenities = _filters['requiredAmenities'] ?? <String>[];

    return _FilterSection(
      title: 'المرافق',
      icon: Icons.apps_rounded,
      expandable: true,
      child: Wrap(
        spacing: AppDimensions.spacingSm,
        runSpacing: AppDimensions.spacingSm,
        children: amenities.map((amenity) {
          final isSelected = selectedAmenities.contains(amenity['id']);
          
          return InkWell(
            onTap: () {
              setState(() {
                if (isSelected) {
                  selectedAmenities.remove(amenity['id']);
                } else {
                  selectedAmenities.add(amenity['id'] as String);
                }
                _filters['requiredAmenities'] = selectedAmenities;
              });
            },
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            child: Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              decoration: BoxDecoration(
                color: isSelected
                    ? AppColors.primary.withOpacity(0.1)
                    : AppColors.surface,
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
                border: Border.all(
                  color: isSelected
                      ? AppColors.primary
                      : AppColors.outline,
                ),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(
                    amenity['icon'] as IconData,
                    size: AppDimensions.iconSmall,
                    color: isSelected
                        ? AppColors.primary
                        : AppColors.textSecondary,
                  ),
                  const SizedBox(width: AppDimensions.spacingXs),
                  Text(
                    amenity['name'] as String,
                    style: AppTextStyles.bodySmall.copyWith(
                      color: isSelected
                          ? AppColors.primary
                          : AppColors.textPrimary,
                      fontWeight: isSelected
                          ? FontWeight.bold
                          : FontWeight.normal,
                    ),
                  ),
                ],
              ),
            ),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildServicesSection() {
    final services = [
      {'id': '1', 'name': 'إفطار مجاني'},
      {'id': '2', 'name': 'خدمة الغرف'},
      {'id': '3', 'name': 'غسيل الملابس'},
      {'id': '4', 'name': 'النقل من/إلى المطار'},
      {'id': '5', 'name': 'خدمة الكونسيرج'},
    ];

    final selectedServices = _filters['serviceIds'] ?? <String>[];

    return _FilterSection(
      title: 'الخدمات',
      icon: Icons.room_service_rounded,
      expandable: true,
      child: Column(
        children: services.map((service) {
          final isSelected = selectedServices.contains(service['id']);
          
          return CheckboxListTile(
            value: isSelected,
            onChanged: (value) {
              setState(() {
                if (value == true) {
                  selectedServices.add(service['id'] as String);
                } else {
                  selectedServices.remove(service['id']);
                }
                _filters['serviceIds'] = selectedServices;
              });
            },
            title: Text(
              service['name'] as String,
              style: AppTextStyles.bodyMedium,
            ),
            activeColor: AppColors.primary,
            dense: true,
            contentPadding: EdgeInsets.zero,
          );
        }).toList(),
      ),
    );
  }

  Widget _buildDynamicFieldsSection() {
    return _FilterSection(
      title: 'فلاتر إضافية',
      icon: Icons.tune_rounded,
      expandable: true,
      child: DynamicFieldsWidget(
        fields: [],
        values: _filters['dynamicFieldFilters'] ?? {},
        onChanged: (values) {
          setState(() {
            _filters['dynamicFieldFilters'] = values;
          });
        },
      ),
    );
  }

  Widget _buildDivider() {
    return const Divider(
      height: 1,
      thickness: 1,
      color: AppColors.divider,
    );
  }

  Widget _buildBottomBar() {
    return Container(
      padding: EdgeInsets.only(
        left: AppDimensions.paddingMedium,
        right: AppDimensions.paddingMedium,
        top: AppDimensions.paddingMedium,
        bottom: MediaQuery.of(context).padding.bottom + AppDimensions.paddingMedium,
      ),
      decoration: BoxDecoration(
        color: AppColors.surface,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurMedium,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Row(
        children: [
          Expanded(
            child: OutlinedButton(
              onPressed: _resetFilters,
              child: const Text('إعادة تعيين'),
            ),
          ),
          const SizedBox(width: AppDimensions.spacingMd),
          Expanded(
            flex: 2,
            child: ElevatedButton(
              onPressed: () {
                Navigator.pop(context, _filters);
              },
              child: Text(
                'تطبيق الفلاتر${_getFiltersCount()}',
              ),
            ),
          ),
        ],
      ),
    );
  }

  String _getFiltersCount() {
    int count = 0;
    
    if (_filters['checkIn'] != null) count++;
    if (_filters['checkOut'] != null) count++;
    if (_filters['guestsCount'] != null && _filters['guestsCount'] != 1) count++;
    if (_filters['propertyTypeId'] != null) count++;
    if (_filters['minPrice'] != null) count++;
    if (_filters['maxPrice'] != null) count++;
    if (_filters['minStarRating'] != null) count++;
    if (_filters['requiredAmenities']?.isNotEmpty ?? false) count++;
    if (_filters['serviceIds']?.isNotEmpty ?? false) count++;
    if (_filters['dynamicFieldFilters']?.isNotEmpty ?? false) count++;
    
    return count > 0 ? ' ($count)' : '';
  }

  void _resetFilters() {
    setState(() {
      _filters = {};
    });
  }

  Future<void> _selectDate(String field) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: _filters[field] ?? DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: AppColors.primary,
              onPrimary: AppColors.white,
              onSurface: AppColors.textPrimary,
            ),
          ),
          child: child!,
        );
      },
    );
    
    if (picked != null) {
      setState(() {
        _filters[field] = picked;
      });
    }
  }

  String _formatDate(DateTime date) {
    final months = [
      'يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو',
      'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'
    ];
    
    return '${date.day} ${months[date.month - 1]} ${date.year}';
  }
}

class _FilterSection extends StatefulWidget {
  final String title;
  final IconData icon;
  final Widget child;
  final bool expandable;

  const _FilterSection({
    required this.title,
    required this.icon,
    required this.child,
    this.expandable = false,
  });

  @override
  State<_FilterSection> createState() => _FilterSectionState();
}

class _FilterSectionState extends State<_FilterSection> {
  bool _isExpanded = true;

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.white,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          InkWell(
            onTap: widget.expandable
                ? () {
                    setState(() {
                      _isExpanded = !_isExpanded;
                    });
                  }
                : null,
            child: Padding(
              padding: const EdgeInsets.all(AppDimensions.paddingMedium),
              child: Row(
                children: [
                  Icon(
                    widget.icon,
                    color: AppColors.primary,
                    size: AppDimensions.iconMedium,
                  ),
                  const SizedBox(width: AppDimensions.spacingSm),
                  Expanded(
                    child: Text(
                      widget.title,
                      style: AppTextStyles.subtitle1.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  if (widget.expandable)
                    Icon(
                      _isExpanded
                          ? Icons.keyboard_arrow_up_rounded
                          : Icons.keyboard_arrow_down_rounded,
                      color: AppColors.textSecondary,
                    ),
                ],
              ),
            ),
          ),
          if (_isExpanded)
            Padding(
              padding: const EdgeInsets.fromLTRB(
                AppDimensions.paddingMedium,
                0,
                AppDimensions.paddingMedium,
                AppDimensions.paddingMedium,
              ),
              child: widget.child,
            ),
        ],
      ),
    );
  }
}