import 'package:equatable/equatable.dart';

abstract class SearchEvent extends Equatable {
  const SearchEvent();

  @override
  List<Object?> get props => [];
}

class SearchPropertiesEvent extends SearchEvent {
  final String? searchTerm;
  final String? city;
  final String? propertyTypeId;
  final double? minPrice;
  final double? maxPrice;
  final int? minStarRating;
  final List<String>? requiredAmenities;
  final String? unitTypeId;
  final List<String>? serviceIds;
  final Map<String, dynamic>? dynamicFieldFilters;
  final DateTime? checkIn;
  final DateTime? checkOut;
  final int? guestsCount;
  final double? latitude;
  final double? longitude;
  final double? radiusKm;
  final String? sortBy;
  final int pageNumber;
  final int pageSize;

  const SearchPropertiesEvent({
    this.searchTerm,
    this.city,
    this.propertyTypeId,
    this.minPrice,
    this.maxPrice,
    this.minStarRating,
    this.requiredAmenities,
    this.unitTypeId,
    this.serviceIds,
    this.dynamicFieldFilters,
    this.checkIn,
    this.checkOut,
    this.guestsCount,
    this.latitude,
    this.longitude,
    this.radiusKm,
    this.sortBy,
    this.pageNumber = 1,
    this.pageSize = 20,
  });

  @override
  List<Object?> get props => [
        searchTerm,
        city,
        propertyTypeId,
        minPrice,
        maxPrice,
        minStarRating,
        requiredAmenities,
        unitTypeId,
        serviceIds,
        dynamicFieldFilters,
        checkIn,
        checkOut,
        guestsCount,
        latitude,
        longitude,
        radiusKm,
        sortBy,
        pageNumber,
        pageSize,
      ];
}

class LoadMoreSearchResultsEvent extends SearchEvent {
  const LoadMoreSearchResultsEvent();
}

class GetSearchFiltersEvent extends SearchEvent {
  const GetSearchFiltersEvent();
}

class GetSearchSuggestionsEvent extends SearchEvent {
  final String query;
  final int limit;

  const GetSearchSuggestionsEvent({
    required this.query,
    this.limit = 10,
  });

  @override
  List<Object> get props => [query, limit];
}

class GetRecommendedPropertiesEvent extends SearchEvent {
  final String? userId;
  final int limit;

  const GetRecommendedPropertiesEvent({
    this.userId,
    this.limit = 10,
  });

  @override
  List<Object?> get props => [userId, limit];
}

class GetPopularDestinationsEvent extends SearchEvent {
  final int limit;

  const GetPopularDestinationsEvent({
    this.limit = 10,
  });

  @override
  List<Object> get props => [limit];
}

class ClearSearchEvent extends SearchEvent {
  const ClearSearchEvent();
}

class UpdateSearchFiltersEvent extends SearchEvent {
  final Map<String, dynamic> filters;

  const UpdateSearchFiltersEvent({required this.filters});

  @override
  List<Object> get props => [filters];
}

class ToggleViewModeEvent extends SearchEvent {
  const ToggleViewModeEvent();
}

class SaveSearchEvent extends SearchEvent {
  final String name;

  const SaveSearchEvent({required this.name});

  @override
  List<Object> get props => [name];
}

class LoadSavedSearchEvent extends SearchEvent {
  final String searchId;

  const LoadSavedSearchEvent({required this.searchId});

  @override
  List<Object> get props => [searchId];
}