import 'package:flutter/material.dart';
import '../domain/entities/home_section.dart';
import 'sections/listings/horizontal_property_list.dart';

class FeaturedPropertiesWidget extends StatelessWidget {
  final HomeSection section;
  const FeaturedPropertiesWidget({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    return HorizontalPropertyList(section: section);
  }
}