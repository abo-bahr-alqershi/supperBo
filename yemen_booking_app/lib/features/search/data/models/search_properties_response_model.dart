import 'package:yemen_booking_app/features/search/data/models/search_result_model.dart';
import 'package:yemen_booking_app/features/search/data/models/search_filter_model.dart';
import 'package:yemen_booking_app/features/search/data/models/search_statistics_model.dart';

class SearchPropertiesResponseModel {
  final List<SearchResultModel> properties;
  final int totalCount;
  final int currentPage;
  final int pageSize;
  final int totalPages;
  final bool hasPreviousPage;
  final bool hasNextPage;
  final SearchFiltersModel appliedFilters;
  final int searchTimeMs;
  final SearchStatisticsModel statistics;

  const SearchPropertiesResponseModel({
    required this.properties,
    required this.totalCount,
    required this.currentPage,
    required this.pageSize,
    required this.totalPages,
    required this.hasPreviousPage,
    required this.hasNextPage,
    required this.appliedFilters,
    required this.searchTimeMs,
    required this.statistics,
  });

  factory SearchPropertiesResponseModel.fromJson(Map<String, dynamic> json) {
    return SearchPropertiesResponseModel(
      properties: (json['properties'] as List?)
              ?.map((e) => SearchResultModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          [],
      totalCount: json['total_count'] ?? 0,
      currentPage: json['current_page'] ?? 1,
      pageSize: json['page_size'] ?? 20,
      totalPages: json['total_pages'] ?? 0,
      hasPreviousPage: json['has_previous_page'] ?? false,
      hasNextPage: json['has_next_page'] ?? false,
      appliedFilters: SearchFiltersModel.fromJson(
          json['applied_filters'] as Map<String, dynamic>? ?? {}),
      searchTimeMs: json['search_time_ms'] ?? 0,
      statistics: SearchStatisticsModel.fromJson(
          json['statistics'] as Map<String, dynamic>? ?? {}),
    );
  }
}