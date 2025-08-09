// lib/features/home/presentation/widgets/search_bar_widget.dart

import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class SearchBarWidget extends StatefulWidget {
  final String initialQuery;
  final Function(String) onSearchChanged;
  final VoidCallback? onTap;
  final bool isExpanded;
  final bool showFilters;

  const SearchBarWidget({
    super.key,
    this.initialQuery = '',
    required this.onSearchChanged,
    this.onTap,
    this.isExpanded = false,
    this.showFilters = true,
  });

  @override
  State<SearchBarWidget> createState() => _SearchBarWidgetState();
}

class _SearchBarWidgetState extends State<SearchBarWidget> 
    with SingleTickerProviderStateMixin {
  late TextEditingController _controller;
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  final bool _hasFocus = false;

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: widget.initialQuery);
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 0.95,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    ));
  }

  @override
  void dispose() {
    _controller.dispose();
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTapDown: (_) => _animationController.forward(),
      onTapUp: (_) => _animationController.reverse(),
      onTapCancel: () => _animationController.reverse(),
      onTap: widget.onTap,
      child: ScaleTransition(
        scale: _scaleAnimation,
        child: Container(
          height: 56,
          decoration: BoxDecoration(
            color: widget.isExpanded 
                ? AppColors.white 
                : AppColors.background,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            border: Border.all(
              color: _hasFocus 
                  ? AppColors.primary 
                  : AppColors.border.withValues(alpha: 0.3),
              width: _hasFocus ? 2 : 1,
            ),
            boxShadow: [
              BoxShadow(
                color: AppColors.shadow.withValues(alpha: 0.08),
                blurRadius: 10,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: Row(
            children: [
              // Search Icon
              Padding(
                padding: const EdgeInsets.only(
                  left: AppDimensions.paddingMedium,
                  right: AppDimensions.paddingSmall,
                ),
                child: Icon(
                  Icons.search_rounded,
                  color: _hasFocus 
                      ? AppColors.primary 
                      : AppColors.textSecondary,
                  size: 24,
                ),
              ),
              
              // Search Field
              Expanded(
                child: TextField(
                  controller: _controller,
                  onChanged: widget.onSearchChanged,
                  onTap: widget.onTap,
                  style: AppTextStyles.bodyLarge,
                  decoration: InputDecoration(
                    hintText: 'ابحث عن المدينة أو العقار...',
                    hintStyle: AppTextStyles.bodyLarge.copyWith(
                      color: AppColors.textHint,
                    ),
                    border: InputBorder.none,
                    contentPadding: const EdgeInsets.symmetric(
                      vertical: AppDimensions.paddingMedium,
                    ),
                  ),
                  onSubmitted: (value) {
                    // Handle search submission
                  },
                ),
              ),
              
              // Clear Button
              if (_controller.text.isNotEmpty)
                IconButton(
                  icon: const Icon(
                    Icons.clear_rounded,
                    color: AppColors.textSecondary,
                    size: 20,
                  ),
                  onPressed: () {
                    setState(() {
                      _controller.clear();
                      widget.onSearchChanged('');
                    });
                  },
                ),
              
              // Filter Button
              if (widget.showFilters)
                Container(
                  margin: const EdgeInsets.only(
                    right: AppDimensions.paddingSmall,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.primary.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(
                      AppDimensions.borderRadiusMd,
                    ),
                  ),
                  child: IconButton(
                    icon: const Icon(
                      Icons.tune_rounded,
                      color: AppColors.primary,
                      size: 22,
                    ),
                    onPressed: () {
                      // Show filters
                    },
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}