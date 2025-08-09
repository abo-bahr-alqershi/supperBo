import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class PriceRangeSliderWidget extends StatefulWidget {
  final double minPrice;
  final double maxPrice;
  final double minValue;
  final double maxValue;
  final String currency;
  final Function(double, double) onChanged;

  const PriceRangeSliderWidget({
    super.key,
    this.minPrice = 0,
    this.maxPrice = 100000,
    this.minValue = 0,
    this.maxValue = 100000,
    this.currency = 'YER',
    required this.onChanged,
  });

  @override
  State<PriceRangeSliderWidget> createState() => _PriceRangeSliderWidgetState();
}

class _PriceRangeSliderWidgetState extends State<PriceRangeSliderWidget> {
  late double _currentMinPrice;
  late double _currentMaxPrice;
  late TextEditingController _minController;
  late TextEditingController _maxController;

  @override
  void initState() {
    super.initState();
    _currentMinPrice = widget.minPrice;
    _currentMaxPrice = widget.maxPrice;
    _minController = TextEditingController(text: _formatPrice(_currentMinPrice));
    _maxController = TextEditingController(text: _formatPrice(_currentMaxPrice));
  }

  @override
  void dispose() {
    _minController.dispose();
    _maxController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildPriceInputs(),
        const SizedBox(height: AppDimensions.spacingLg),
        _buildSlider(),
        const SizedBox(height: AppDimensions.spacingMd),
        _buildQuickSelections(),
      ],
    );
  }

  Widget _buildPriceInputs() {
    return Row(
      children: [
        Expanded(
          child: _buildPriceInput(
            controller: _minController,
            label: 'الحد الأدنى',
            value: _currentMinPrice,
            onChanged: (value) {
              setState(() {
                _currentMinPrice = value.clamp(widget.minValue, _currentMaxPrice);
                widget.onChanged(_currentMinPrice, _currentMaxPrice);
              });
            },
          ),
        ),
        const SizedBox(width: AppDimensions.spacingMd),
        Container(
          padding: const EdgeInsets.all(AppDimensions.paddingSmall),
          decoration: BoxDecoration(
            color: AppColors.primary.withValues(alpha: 0.1),
            shape: BoxShape.circle,
          ),
          child: const Icon(
            Icons.remove_rounded,
            size: AppDimensions.iconSmall,
            color: AppColors.primary,
          ),
        ),
        const SizedBox(width: AppDimensions.spacingMd),
        Expanded(
          child: _buildPriceInput(
            controller: _maxController,
            label: 'الحد الأقصى',
            value: _currentMaxPrice,
            onChanged: (value) {
              setState(() {
                _currentMaxPrice = value.clamp(_currentMinPrice, widget.maxValue);
                widget.onChanged(_currentMinPrice, _currentMaxPrice);
              });
            },
          ),
        ),
      ],
    );
  }

  Widget _buildPriceInput({
    required TextEditingController controller,
    required String label,
    required double value,
    required Function(double) onChanged,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingXs),
        Container(
          decoration: BoxDecoration(
            color: AppColors.inputBackground,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            border: Border.all(
              color: AppColors.outline,
            ),
          ),
          child: TextField(
            controller: controller,
            keyboardType: TextInputType.number,
            decoration: InputDecoration(
              border: InputBorder.none,
              contentPadding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingMedium,
                vertical: AppDimensions.paddingSmall,
              ),
              suffixText: widget.currency,
              suffixStyle: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
            style: AppTextStyles.bodyMedium.copyWith(
              fontWeight: FontWeight.bold,
            ),
            onChanged: (text) {
              final number = double.tryParse(text.replaceAll(',', ''));
              if (number != null) {
                onChanged(number);
              }
            },
          ),
        ),
      ],
    );
  }

  Widget _buildSlider() {
    return Column(
      children: [
        SliderTheme(
          data: SliderThemeData(
            activeTrackColor: AppColors.primary,
            inactiveTrackColor: AppColors.primary.withValues(alpha: 0.2),
            thumbColor: AppColors.primary,
            overlayColor: AppColors.primary.withValues(alpha: 0.2),
            trackHeight: 4,
            thumbShape: const RoundSliderThumbShape(
              enabledThumbRadius: 8,
            ),
            overlayShape: const RoundSliderOverlayShape(
              overlayRadius: 16,
            ),
            rangeThumbShape: const RoundRangeSliderThumbShape(
              enabledThumbRadius: 8,
            ),
            rangeTrackShape: const RoundedRectRangeSliderTrackShape(),
          ),
          child: RangeSlider(
            values: RangeValues(_currentMinPrice, _currentMaxPrice),
            min: widget.minValue,
            max: widget.maxValue,
            onChanged: (values) {
              setState(() {
                _currentMinPrice = values.start;
                _currentMaxPrice = values.end;
                _minController.text = _formatPrice(_currentMinPrice);
                _maxController.text = _formatPrice(_currentMaxPrice);
              });
              widget.onChanged(_currentMinPrice, _currentMaxPrice);
            },
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingSmall,
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                _formatPrice(widget.minValue),
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
              Text(
                _formatPrice(widget.maxValue),
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildQuickSelections() {
    final quickRanges = [
      {'label': 'أقل من 10K', 'min': 0.0, 'max': 10000.0},
      {'label': '10K - 25K', 'min': 10000.0, 'max': 25000.0},
      {'label': '25K - 50K', 'min': 25000.0, 'max': 50000.0},
      {'label': '50K - 100K', 'min': 50000.0, 'max': 100000.0},
      {'label': 'أكثر من 100K', 'min': 100000.0, 'max': widget.maxValue},
    ];

    return Wrap(
      spacing: AppDimensions.spacingSm,
      runSpacing: AppDimensions.spacingSm,
      children: quickRanges.map((range) {
        final isSelected = _currentMinPrice == range['min'] && 
                          _currentMaxPrice == range['max'];
        
        return InkWell(
          onTap: () {
            setState(() {
              _currentMinPrice = range['min'] as double;
              _currentMaxPrice = range['max'] as double;
              _minController.text = _formatPrice(_currentMinPrice);
              _maxController.text = _formatPrice(_currentMaxPrice);
            });
            widget.onChanged(_currentMinPrice, _currentMaxPrice);
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
            child: Text(
              range['label'] as String,
              style: AppTextStyles.caption.copyWith(
                color: isSelected
                    ? AppColors.white
                    : AppColors.textPrimary,
                fontWeight: isSelected
                    ? FontWeight.bold
                    : FontWeight.normal,
              ),
            ),
          ),
        );
      }).toList(),
    );
  }

  String _formatPrice(double price) {
    if (price >= 1000000) {
      return '${(price / 1000000).toStringAsFixed(1)}M';
    } else if (price >= 1000) {
      return '${(price / 1000).toStringAsFixed(0)}K';
    }
    return price.toStringAsFixed(0);
  }
}