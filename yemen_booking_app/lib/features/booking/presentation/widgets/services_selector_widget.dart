import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/theme/app_colors.dart';
import 'package:yemen_booking_app/core/theme/app_dimensions.dart';
import 'package:yemen_booking_app/core/theme/app_text_styles.dart';

class ServicesSelectorWidget extends StatefulWidget {
  final String propertyId;
  final Function(List<Map<String, dynamic>>) onServicesChanged;

  const ServicesSelectorWidget({
    super.key,
    required this.propertyId,
    required this.onServicesChanged,
  });

  @override
  State<ServicesSelectorWidget> createState() => _ServicesSelectorWidgetState();
}

class _ServicesSelectorWidgetState extends State<ServicesSelectorWidget> {
  final List<Map<String, dynamic>> _services = [
    {
      'id': '1',
      'name': 'خدمة التنظيف اليومية',
      'price': 50.0,
      'description': 'تنظيف الغرفة وترتيبها يومياً',
      'icon': Icons.cleaning_services,
    },
    {
      'id': '2',
      'name': 'وجبة الإفطار',
      'price': 30.0,
      'description': 'وجبة إفطار كاملة',
      'icon': Icons.breakfast_dining,
    },
    {
      'id': '3',
      'name': 'خدمة النقل من/إلى المطار',
      'price': 100.0,
      'description': 'سيارة خاصة مع سائق',
      'icon': Icons.airport_shuttle,
    },
    {
      'id': '4',
      'name': 'موقف سيارة خاص',
      'price': 20.0,
      'description': 'موقف آمن للسيارة',
      'icon': Icons.local_parking,
    },
    {
      'id': '5',
      'name': 'واي فاي عالي السرعة',
      'price': 15.0,
      'description': 'انترنت فائق السرعة',
      'icon': Icons.wifi,
    },
  ];

  final Map<String, bool> _selectedServices = {};
  final Map<String, int> _serviceQuantities = {};

  @override
  void initState() {
    super.initState();
    for (var service in _services) {
      _selectedServices[service['id']] = false;
      _serviceQuantities[service['id']] = 1;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(color: AppColors.outline),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            child: Row(
              children: [
                const Icon(
                  Icons.room_service,
                  color: AppColors.primary,
                  size: AppDimensions.iconSmall,
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                Text(
                  'اختر الخدمات الإضافية (اختياري)',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          ),
          const Divider(height: 1),
          ..._services.map((service) => _buildServiceItem(service)),
        ],
      ),
    );
  }

  Widget _buildServiceItem(Map<String, dynamic> service) {
    final isSelected = _selectedServices[service['id']] ?? false;
    final quantity = _serviceQuantities[service['id']] ?? 1;

    return Column(
      children: [
        InkWell(
          onTap: () {
            setState(() {
              _selectedServices[service['id']] = !isSelected;
              _updateSelectedServices();
            });
          },
          child: Container(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            decoration: BoxDecoration(
              color: isSelected
                  ? AppColors.primary.withValues(alpha: 0.05)
                  : Colors.transparent,
            ),
            child: Row(
              children: [
                Container(
                  width: 24,
                  height: 24,
                  decoration: BoxDecoration(
                    color: isSelected ? AppColors.primary : Colors.transparent,
                    border: Border.all(
                      color: isSelected ? AppColors.primary : AppColors.outline,
                      width: 2,
                    ),
                    borderRadius: BorderRadius.circular(4),
                  ),
                  child: isSelected
                      ? const Icon(
                          Icons.check,
                          size: 16,
                          color: Colors.white,
                        )
                      : null,
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Container(
                  padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                  decoration: BoxDecoration(
                    color: AppColors.primary.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
                  ),
                  child: Icon(
                    service['icon'],
                    color: AppColors.primary,
                    size: AppDimensions.iconSmall,
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        service['name'],
                        style: AppTextStyles.bodyMedium.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        service['description'],
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Text(
                      '${service['price'].toStringAsFixed(0)} ريال',
                      style: AppTextStyles.bodyMedium.copyWith(
                        fontWeight: FontWeight.bold,
                        color: AppColors.primary,
                      ),
                    ),
                    if (isSelected) ...[
                      const SizedBox(height: AppDimensions.spacingXs),
                      Container(
                        decoration: BoxDecoration(
                          border: Border.all(color: AppColors.outline),
                          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            InkWell(
                              onTap: quantity > 1
                                  ? () {
                                      setState(() {
                                        _serviceQuantities[service['id']] = quantity - 1;
                                        _updateSelectedServices();
                                      });
                                    }
                                  : null,
                              child: Container(
                                padding: const EdgeInsets.all(2),
                                child: Icon(
                                  Icons.remove,
                                  size: 16,
                                  color: quantity > 1
                                      ? AppColors.primary
                                      : AppColors.disabled,
                                ),
                              ),
                            ),
                            Container(
                              constraints: const BoxConstraints(minWidth: 30),
                              padding: const EdgeInsets.symmetric(horizontal: 8),
                              child: Text(
                                quantity.toString(),
                                style: AppTextStyles.caption.copyWith(
                                  fontWeight: FontWeight.bold,
                                ),
                                textAlign: TextAlign.center,
                              ),
                            ),
                            InkWell(
                              onTap: () {
                                setState(() {
                                  _serviceQuantities[service['id']] = quantity + 1;
                                  _updateSelectedServices();
                                });
                              },
                              child: Container(
                                padding: const EdgeInsets.all(2),
                                child: const Icon(
                                  Icons.add,
                                  size: 16,
                                  color: AppColors.primary,
                                ),
                              ),
                            ),
                                                      ],
                        ),
                      ),
                    ],
                  ],
                ),
              ],
            ),
          ),
        ),
        const Divider(height: 1),
      ],
    );
  }

  void _updateSelectedServices() {
    final selectedList = <Map<String, dynamic>>[];
    
    _selectedServices.forEach((id, isSelected) {
      if (isSelected) {
        final service = _services.firstWhere((s) => s['id'] == id);
        selectedList.add({
          'id': id,
          'name': service['name'],
          'price': service['price'],
          'quantity': _serviceQuantities[id] ?? 1,
        });
      }
    });
    
    widget.onServicesChanged(selectedList);
  }
}