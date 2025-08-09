import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class DynamicFieldsWidget extends StatefulWidget {
  final List<dynamic> fields;
  final Map<String, dynamic> values;
  final Function(Map<String, dynamic>) onChanged;

  const DynamicFieldsWidget({
    super.key,
    required this.fields,
    required this.values,
    required this.onChanged,
  });

  @override
  State<DynamicFieldsWidget> createState() => _DynamicFieldsWidgetState();
}

class _DynamicFieldsWidgetState extends State<DynamicFieldsWidget> {
  late Map<String, dynamic> _values;

  @override
  void initState() {
    super.initState();
    _values = Map.from(widget.values);
  }

  @override
  Widget build(BuildContext context) {
    if (widget.fields.isEmpty) {
      return _buildEmptyState();
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: widget.fields.map((field) {
        return Padding(
          padding: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
          child: _buildField(field),
        );
      }).toList(),
    );
  }

  Widget _buildEmptyState() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Center(
        child: Column(
          children: [
            Icon(
              Icons.filter_alt_off_rounded,
              size: AppDimensions.iconLarge,
              color: AppColors.textSecondary.withValues(alpha: 0.5),
            ),
            const SizedBox(height: AppDimensions.spacingSm),
            Text(
              'لا توجد فلاتر إضافية',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildField(dynamic field) {
    final fieldType = field['type'] ?? 'text';
    final fieldName = field['name'] ?? '';
    final fieldLabel = field['label'] ?? fieldName;
    final isRequired = field['required'] ?? false;

    switch (fieldType) {
      case 'text':
        return _buildTextField(field, fieldName, fieldLabel, isRequired);
      case 'number':
        return _buildNumberField(field, fieldName, fieldLabel, isRequired);
      case 'select':
        return _buildSelectField(field, fieldName, fieldLabel, isRequired);
      case 'multiselect':
        return _buildMultiSelectField(field, fieldName, fieldLabel, isRequired);
      case 'checkbox':
        return _buildCheckboxField(field, fieldName, fieldLabel);
      case 'radio':
        return _buildRadioField(field, fieldName, fieldLabel, isRequired);
      case 'range':
        return _buildRangeField(field, fieldName, fieldLabel);
      case 'date':
        return _buildDateField(field, fieldName, fieldLabel, isRequired);
      default:
        return const SizedBox.shrink();
    }
  }

  Widget _buildTextField(
    dynamic field,
    String name,
    String label,
    bool isRequired,
  ) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, isRequired),
        const SizedBox(height: AppDimensions.spacingXs),
        TextField(
          decoration: InputDecoration(
            hintText: field['placeholder'] ?? 'أدخل $label',
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            ),
          ),
          onChanged: (value) {
            setState(() {
              _values[name] = value;
            });
            widget.onChanged(_values);
          },
        ),
      ],
    );
  }

  Widget _buildNumberField(
    dynamic field,
    String name,
    String label,
    bool isRequired,
  ) {
    final min = field['min']?.toDouble() ?? 0.0;
    final max = field['max']?.toDouble() ?? 100.0;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, isRequired),
        const SizedBox(height: AppDimensions.spacingXs),
        Row(
          children: [
            Expanded(
              child: Slider(
                value: (_values[name] ?? min).toDouble(),
                min: min,
                max: max,
                onChanged: (value) {
                  setState(() {
                    _values[name] = value;
                  });
                  widget.onChanged(_values);
                },
                activeColor: AppColors.primary,
                inactiveColor: AppColors.primary.withValues(alpha: 0.2),
              ),
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
                (_values[name] ?? min).toStringAsFixed(0),
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.primary,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildSelectField(
    dynamic field,
    String name,
    String label,
    bool isRequired,
  ) {
    final options = field['options'] as List<dynamic>? ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, isRequired),
        const SizedBox(height: AppDimensions.spacingXs),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
          decoration: BoxDecoration(
            border: Border.all(color: AppColors.outline),
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          ),
          child: DropdownButtonHideUnderline(
            child: DropdownButton<String>(
              value: _values[name],
              hint: Text(
                'اختر $label',
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.textHint,
                ),
              ),
              isExpanded: true,
              icon: const Icon(Icons.keyboard_arrow_down_rounded),
              items: options.map<DropdownMenuItem<String>>((option) {
                return DropdownMenuItem<String>(
                  value: option['value'].toString(),
                  child: Text(option['label'].toString()),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _values[name] = value;
                });
                widget.onChanged(_values);
              },
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildMultiSelectField(
    dynamic field,
    String name,
    String label,
    bool isRequired,
  ) {
    final options = field['options'] as List<dynamic>? ?? [];
    final selectedValues = _values[name] as List<String>? ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, isRequired),
        const SizedBox(height: AppDimensions.spacingXs),
        Wrap(
          spacing: AppDimensions.spacingSm,
          runSpacing: AppDimensions.spacingSm,
          children: options.map((option) {
            final value = option['value'].toString();
            final optionLabel = option['label'].toString();
            final isSelected = selectedValues.contains(value);

            return FilterChip(
              label: Text(optionLabel),
              selected: isSelected,
              onSelected: (selected) {
                setState(() {
                  if (selected) {
                    selectedValues.add(value);
                  } else {
                    selectedValues.remove(value);
                  }
                  _values[name] = selectedValues;
                });
                widget.onChanged(_values);
              },
              selectedColor: AppColors.primary.withValues(alpha: 0.2),
              checkmarkColor: AppColors.primary,
              labelStyle: TextStyle(
                color: isSelected ? AppColors.primary : AppColors.textPrimary,
              ),
            );
          }).toList(),
        ),
      ],
    );
  }

  Widget _buildCheckboxField(
    dynamic field,
    String name,
    String label,
  ) {
    return CheckboxListTile(
      value: _values[name] ?? false,
      onChanged: (value) {
        setState(() {
          _values[name] = value;
        });
        widget.onChanged(_values);
      },
      title: Text(
        label,
        style: AppTextStyles.bodyMedium,
      ),
      subtitle: field['description'] != null
          ? Text(
              field['description'],
              style: AppTextStyles.caption.copyWith(
                color: AppColors.textSecondary,
              ),
            )
          : null,
      activeColor: AppColors.primary,
      contentPadding: EdgeInsets.zero,
    );
  }

  Widget _buildRadioField(
    dynamic field,
    String name,
    String label,
    bool isRequired,
  ) {
    final options = field['options'] as List<dynamic>? ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, isRequired),
        const SizedBox(height: AppDimensions.spacingXs),
        ...options.map((option) {
          final value = option['value'].toString();
          final optionLabel = option['label'].toString();

          return RadioListTile<String>(
            value: value,
            groupValue: _values[name],
            onChanged: (value) {
              setState(() {
                _values[name] = value;
              });
              widget.onChanged(_values);
            },
            title: Text(
              optionLabel,
              style: AppTextStyles.bodyMedium,
            ),
            activeColor: AppColors.primary,
            contentPadding: EdgeInsets.zero,
          );
        }),
      ],
    );
  }

  Widget _buildRangeField(
    dynamic field,
    String name,
    String label,
  ) {
    final min = field['min']?.toDouble() ?? 0.0;
    final max = field['max']?.toDouble() ?? 100.0;
    final currentRange = _values[name] as RangeValues? ??
        RangeValues(min, max);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, false),
        const SizedBox(height: AppDimensions.spacingXs),
        RangeSlider(
          values: currentRange,
          min: min,
          max: max,
          onChanged: (values) {
            setState(() {
              _values[name] = values;
            });
            widget.onChanged(_values);
          },
          activeColor: AppColors.primary,
          inactiveColor: AppColors.primary.withValues(alpha: 0.2),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingSmall),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                currentRange.start.toStringAsFixed(0),
                style: AppTextStyles.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
              Text(
                currentRange.end.toStringAsFixed(0),
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

  Widget _buildDateField(
    dynamic field,
    String name,
    String label,
    bool isRequired,
  ) {
    final selectedDate = _values[name] as DateTime?;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(label, isRequired),
        const SizedBox(height: AppDimensions.spacingXs),
        InkWell(
          onTap: () async {
            final date = await showDatePicker(
              context: context,
              initialDate: selectedDate ?? DateTime.now(),
              firstDate: DateTime.now(),
              lastDate: DateTime.now().add(const Duration(days: 365)),
            );
            
            if (date != null) {
              setState(() {
                _values[name] = date;
              });
              widget.onChanged(_values);
            }
          },
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          child: Container(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            decoration: BoxDecoration(
              border: Border.all(color: AppColors.outline),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            ),
            child: Row(
              children: [
                const Icon(
                  Icons.calendar_today_rounded,
                  color: AppColors.textSecondary,
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Text(
                  selectedDate != null
                      ? _formatDate(selectedDate)
                      : 'اختر التاريخ',
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: selectedDate != null
                        ? AppColors.textPrimary
                        : AppColors.textHint,
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildLabel(String label, bool isRequired) {
    return Row(
      children: [
        Text(
          label,
          style: AppTextStyles.bodyMedium.copyWith(
            fontWeight: FontWeight.w500,
          ),
        ),
        if (isRequired)
          Text(
            ' *',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.error,
            ),
          ),
      ],
    );
  }

  String _formatDate(DateTime date) {
    return '${date.day}/${date.month}/${date.year}';
  }
}