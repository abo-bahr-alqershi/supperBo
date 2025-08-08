import 'package:flutter/material.dart';

class PropertyCategoriesWidget extends StatelessWidget {
  const PropertyCategoriesWidget({super.key});

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: List.generate(6, (index) {
        return Chip(
          label: Text('فئة ${index + 1}'),
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
        );
      }),
    );
  }
}