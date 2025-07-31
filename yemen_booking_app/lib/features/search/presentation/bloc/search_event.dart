import 'package:equatable/equatable.dart';

abstract class SearchEvent extends Equatable {
  const SearchEvent();

  @override
  List<Object?> get props => [];
}

class SearchPropertiesEvent extends SearchEvent {
  final String? searchQuery;
  final String? city;
  final String? propertyType;
  final double? minPrice;
  final double? maxPrice;
  final int? minRating;
  final List<String>? amenities;
  final DateTime? checkInDate;
  final DateTime? checkOutDate;
  final int? guests;
  final double? latitude;
  final double? longitude;
  final double? radiusKm;
  final String? sortBy;
  final int pageNumber;
  final int pageSize;
  final bool isNewSearch;

  const SearchPropertiesEvent({
    this.searchQuery,
    this.city,
    this.propertyType,
    this.minPrice,
    this.maxPrice,
    this.minRating,
    this.amenities,
    this.checkInDate,
    this.checkOutDate,
    this.guests,
    this.latitude,
    this.longitude,
    this.radiusKm,
    this.sortBy,
    this.pageNumber = 1,
    this.pageSize = 20,
    this.isNewSearch = true,
  });

  @override
  List<Object?> get props => [
        searchQuery,
        city,
        propertyType,
        minPrice,
        maxPrice,
        minRating,
        amenities,
        checkInDate,
        checkOutDate,
        guests,
        latitude,
        longitude,
        radiusKm,
        sortBy,
        pageNumber,
        pageSize,
        isNewSearch,
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

class ClearSearchSuggestionsEvent extends SearchEvent {
  const ClearSearchSuggestionsEvent();
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

class UpdateSearchFiltersEvent extends SearchEvent {
  final String? city;
  final String? propertyType;
  final double? minPrice;
  final double? maxPrice;
  final int? minRating;
  final List<String>? amenities;
  final DateTime? checkInDate;
  final DateTime? checkOutDate;
  final int? guests;
  final String? sortBy;

  const UpdateSearchFiltersEvent({
    this.city,
    this.propertyType,
    this.minPrice,
    this.maxPrice,
    this.minRating,
    this.amenities,
    this.checkInDate,
    this.checkOutDate,
    this.guests,
    this.sortBy,
  });

  @override
  List<Object?> get props => [
        city,
        propertyType,
        minPrice,
        maxPrice,
        minRating,
        amenities,
        checkInDate,
        checkOutDate,
        guests,
        sortBy,
      ];
}

class ClearSearchResultsEvent extends SearchEvent {
  const ClearSearchResultsEvent();
}

class AddToRecentSearchesEvent extends SearchEvent {
  final String searchQuery;

  const AddToRecentSearchesEvent({required this.searchQuery});

  @override
  List<Object> get props => [searchQuery];
}

class LoadRecentSearchesEvent extends SearchEvent {
  const LoadRecentSearchesEvent();
}

class ClearRecentSearchesEvent extends SearchEvent {
  const ClearRecentSearchesEvent();
}

class SaveSearchEvent extends SearchEvent {
  final String name;
  final Map<String, dynamic> searchParams;

  const SaveSearchEvent({
    required this.name,
    required this.searchParams,
  });

  @override
  List<Object> get props => [name, searchParams];
}

class LoadSavedSearchesEvent extends SearchEvent {
  const LoadSavedSearchesEvent();
}

class DeleteSavedSearchEvent extends SearchEvent {
  final String searchId;

  const DeleteSavedSearchEvent({required this.searchId});

  @override
  List<Object> get props => [searchId];
}

class ApplySavedSearchEvent extends SearchEvent {
  final Map<String, dynamic> searchParams;

  const ApplySavedSearchEvent({required this.searchParams});

  @override
  List<Object> get props => [searchParams];
}