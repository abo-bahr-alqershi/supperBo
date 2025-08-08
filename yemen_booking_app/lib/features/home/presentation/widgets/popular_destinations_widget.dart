import 'package:flutter/material.dart';
import '../domain/entities/home_section.dart';
import 'sections/destinations/city_cards_grid.dart';

class PopularDestinationsWidget extends StatelessWidget {
  final HomeSection section;
  const PopularDestinationsWidget({super.key, required this.section});

  @override
  Widget build(BuildContext context) {
    return CityCardsGrid(section: section);
  }
}