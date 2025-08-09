// lib/features/home/presentation/widgets/property_categories_widget.dart

import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class PropertyCategory {
  final String id;
  final String name;
  final String icon;
  final Color color;
  final int count;

  PropertyCategory({
    required this.id,
    required this.name,
    required this.icon,
    required this.color,
    required this.count,
  });
}

class PropertyCategoriesWidget extends StatefulWidget {
  final Function(PropertyCategory) onCategoryTap;

  const PropertyCategoriesWidget({
    super.key,
    required this.onCategoryTap,
  });

  @override
  State<PropertyCategoriesWidget> createState() => 
      _PropertyCategoriesWidgetState();
}

class _PropertyCategoriesWidgetState extends State<PropertyCategoriesWidget> 
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late List<Animation<double>> _scaleAnimations;

  final List<PropertyCategory> _categories = [
    PropertyCategory(
      id: 'hotels',
      name: 'فنادق',
      icon: '🏨',
      color: const Color(0xFF6366F1),
      count: 234,
    ),
    PropertyCategory(
      id: 'apartments',
      name: 'شقق',
      icon: '🏢',
      color: const Color(0xFF8B5CF6),
      count: 456,
    ),
    PropertyCategory(
      id: 'villas',
      name: 'فلل',
      icon: '🏡',
      color: const Color(0xFFEC4899),
      count: 123,
    ),
    PropertyCategory(
      id: 'chalets',
      name: 'شاليهات',
      icon: '🏖️',
      color: const Color(0xFFF59E0B),
      count: 89,
    ),
    PropertyCategory(
      id: 'farms',
      name: 'مزارع',
      icon: '🌳',
      color: const Color(0xFF10B981),
      count: 67,
    ),
    PropertyCategory(
      id: 'camps',
      name: 'مخيمات',
      icon: '⛺',
      color: const Color(0xFFF97316),
      count: 45,
    ),
  ];

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _scaleAnimations = List.generate(
      _categories.length,
      (index) => Tween<double>(
        begin: 0.0,
        end: 1.0,
      ).animate(
        CurvedAnimation(
          parent: _animationController,
          curve: Interval(
            index * 0.1,
            0.5 + index * 0.1,
            curve: Curves.elasticOut,
          ),
        ),
      ),
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
    return Container(
      height: 120,
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMedium,
      ),
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        physics: const BouncingScrollPhysics(),
        itemCount: _categories.length,
        itemBuilder: (context, index) {
          return ScaleTransition(
            scale: _scaleAnimations[index],
            child: _buildCategoryCard(_categories[index]),
          );
        },
      ),
    );
  }

  Widget _buildCategoryCard(PropertyCategory category) {
    return GestureDetector(
      onTap: () => widget.onCategoryTap(category),
      child: Container(
        width: 100,
        margin: const EdgeInsets.only(left: AppDimensions.spacingSm),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // Icon Container
            Container(
              width: 64,
              height: 64,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    category.color.withValues(alpha: 0.1),
                    category.color.withValues(alpha: 0.2),
                  ],
                ),
                borderRadius: BorderRadius.circular(16),
                border: Border.all(
                  color: category.color.withValues(alpha: 0.2),
                  width: 1,
                ),
              ),
              child: Stack(
                alignment: Alignment.center,
                children: [
                  Text(
                    category.icon,
                    style: const TextStyle(fontSize: 28),
                  ),
                  // Count Badge
                  Positioned(
                    top: 4,
                    right: 4,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 6,
                        vertical: 2,
                      ),
                      decoration: BoxDecoration(
                        color: category.color,
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: Text(
                        '${category.count}',
                        style: AppTextStyles.caption.copyWith(
                          color: Colors.white,
                          fontSize: 10,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 8),
            // Category Name
            Text(
              category.name,
              style: AppTextStyles.bodyMedium.copyWith(
                fontWeight: FontWeight.w600,
              ),
              textAlign: TextAlign.center,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
      ),
    );
  }
}