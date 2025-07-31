class SearchStatisticsModel {
  final Map<String, int> propertiesByType;
  final Map<String, int> propertiesByCity;
  final PriceRangeStatsModel priceRange;
  final double averageRating;
  final int availableCount;
  final int totalCount;

  const SearchStatisticsModel({
    required this.propertiesByType,
    required this.propertiesByCity,
    required this.priceRange,
    required this.averageRating,
    required this.availableCount,
    required this.totalCount,
  });

  factory SearchStatisticsModel.fromJson(Map<String, dynamic> json) {
    return SearchStatisticsModel(
      propertiesByType: Map<String, int>.from(json['properties_by_type'] ?? {}),
      propertiesByCity: Map<String, int>.from(json['properties_by_city'] ?? {}),
      priceRange: PriceRangeStatsModel.fromJson(
        json['price_range'] as Map<String, dynamic>? ?? {},
      ),
      averageRating: (json['average_rating'] ?? 0).toDouble(),
      availableCount: json['available_count'] ?? 0,
      totalCount: json['total_count'] ?? 0,
    );
  }
}

class PriceRangeStatsModel {
  final double min;
  final double max;
  final String currency;

  const PriceRangeStatsModel({
    required this.min,
    required this.max,
    required this.currency,
  });

  factory PriceRangeStatsModel.fromJson(Map<String, dynamic> json) {
    return PriceRangeStatsModel(
      min: (json['min'] ?? 0).toDouble(),
      max: (json['max'] ?? 0).toDouble(),
      currency: json['currency'] ?? 'YER',
    );
  }
}