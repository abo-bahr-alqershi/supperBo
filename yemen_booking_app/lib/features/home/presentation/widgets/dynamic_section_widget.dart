import 'package:flutter/material.dart';
import '../../../core/enums/section_type_enum.dart';
import '../../domain/entities/home_section.dart';

// Import section-specific widgets (to be implemented)
import 'sections/horizontal_property_list_widget.dart';
import 'sections/vertical_property_grid_widget.dart';
import 'sections/featured_property_ad_widget.dart';
// ... import other section widgets as needed

class DynamicSectionWidget extends StatelessWidget {
  final HomeSection section;

  const DynamicSectionWidget({Key? key, required this.section}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    if (!section.isVisible) return SizedBox.shrink();

    switch (section.type) {
      case SectionType.horizontalPropertyList:
        return HorizontalPropertyListWidget(section: section);
      case SectionType.verticalPropertyGrid:
        return VerticalPropertyGridWidget(section: section);
      case SectionType.singlePropertyAd:
      case SectionType.featuredPropertyAd:
        return FeaturedPropertyAdWidget(section: section);
      // TODO: handle other types: multiPropertyAd, offersCarousel, cityCardsGrid, etc.
      default:
        return SizedBox.shrink();
    }
  }
}