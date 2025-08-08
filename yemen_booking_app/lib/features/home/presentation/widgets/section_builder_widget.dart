import 'package:flutter/material.dart';
import '../../domain/entities/home_section.dart';
import '../../../domain/entities/section_type.dart' as domain;
import 'sections/base/section_container.dart';
import 'sections/base/section_header.dart';
import 'sections/listings/horizontal_property_list.dart';
import 'sections/listings/vertical_property_grid.dart';
import 'sections/destinations/city_cards_grid.dart';
import 'sections/sponsored/featured_property_ad_widget.dart';

class SectionBuilderWidget extends StatelessWidget {
  final HomeSection section;
  const SectionBuilderWidget({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    final header = SectionHeader(title: section.displayTitle, subtitle: section.subtitle);
    Widget content;
    switch (section.sectionType) {
      case domain.SectionType.horizontalPropertyList:
        content = HorizontalPropertyList(section: section);
        break;
      case domain.SectionType.verticalPropertyGrid:
        content = VerticalPropertyGrid(section: section);
        break;
      case domain.SectionType.exploreCities:
        content = CityCardsGrid(section: section);
        break;
      case domain.SectionType.featuredPropertyAd:
        content = FeaturedPropertyAdWidget(section: section);
        break;
      default:
        content = const SizedBox.shrink();
    }
    return SectionContainer(header: header, child: content);
  }
}