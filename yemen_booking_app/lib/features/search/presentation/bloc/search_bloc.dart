import 'dart:async';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:yemen_booking_app/core/usecases/usecase.dart';
import 'dart:convert';
import '../../../../core/models/paginated_result.dart';
import '../../domain/entities/search_result.dart';
import '../../domain/usecases/get_search_filters_usecase.dart';
import '../../domain/usecases/get_search_suggestions_usecase.dart';
import '../../domain/usecases/search_properties_usecase.dart';
import '../../domain/repositories/search_repository.dart';
import 'search_event.dart';
import 'search_state.dart';

class SearchBloc extends Bloc<SearchEvent, SearchState> {
  final SearchPropertiesUseCase searchPropertiesUseCase;
  final GetSearchFiltersUseCase getSearchFiltersUseCase;
  final GetSearchSuggestionsUseCase getSearchSuggestionsUseCase;
  final SearchRepository searchRepository;
  final SharedPreferences sharedPreferences;

  static const String _recentSearchesKey = 'recent_searches';
  static const String _savedSearchesKey = 'saved_searches';
  static const int _maxRecentSearches = 10;

  Map<String, dynamic> _currentFilters = {};
  PaginatedResult<SearchResult>? _currentSearchResults;

  SearchBloc({
    required this.searchPropertiesUseCase,
    required this.getSearchFiltersUseCase,
    required this.getSearchSuggestionsUseCase,
    required this.searchRepository,
    required this.sharedPreferences,
  }) : super(const SearchCombinedState()) {
    on<SearchPropertiesEvent>(_onSearchProperties);
    on<LoadMoreSearchResultsEvent>(_onLoadMoreSearchResults);
    on<GetSearchFiltersEvent>(_onGetSearchFilters);
    on<GetSearchSuggestionsEvent>(_onGetSearchSuggestions);
    on<ClearSearchSuggestionsEvent>(_onClearSearchSuggestions);
    on<GetRecommendedPropertiesEvent>(_onGetRecommendedProperties);
    on<GetPopularDestinationsEvent>(_onGetPopularDestinations);
    on<UpdateSearchFiltersEvent>(_onUpdateSearchFilters);
    on<ClearSearchResultsEvent>(_onClearSearchResults);
    on<AddToRecentSearchesEvent>(_onAddToRecentSearches);
    on<LoadRecentSearchesEvent>(_onLoadRecentSearches);
    on<ClearRecentSearchesEvent>(_onClearRecentSearches);
    on<SaveSearchEvent>(_onSaveSearch);
    on<LoadSavedSearchesEvent>(_onLoadSavedSearches);
    on<DeleteSavedSearchEvent>(_onDeleteSavedSearch);
    on<ApplySavedSearchEvent>(_onApplySavedSearch);
  }

  void _onSearchProperties(
    SearchPropertiesEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;

    if (event.isNewSearch) {
      emit(currentState.copyWith(
        searchResultsState: const SearchLoading(),
      ));
      _currentSearchResults = null;
    } else {
      emit(currentState.copyWith(
        searchResultsState: SearchLoadingMore(
          currentResults: _currentSearchResults!,
        ),
      ));
    }

    final params = SearchPropertiesParams(
      searchTerm: event.searchTerm,
      city: event.city,
      propertyTypeId: event.propertyTypeId,
      minPrice: event.minPrice,
      maxPrice: event.maxPrice,
      minStarRating: event.minStarRating,
      requiredAmenities: event.requiredAmenities,
      unitTypeId: event.unitTypeId,
      serviceIds: event.serviceIds,
      dynamicFieldFilters: event.dynamicFieldFilters,
      checkIn: event.checkIn,
      checkOut: event.checkOut,
      guestsCount: event.guestsCount,
      latitude: event.latitude,
      longitude: event.longitude,
      radiusKm: event.radiusKm,
      sortBy: event.sortBy,
      pageNumber: event.pageNumber,
      pageSize: event.pageSize,
    );

    final result = await searchPropertiesUseCase(params);

    result.fold(
      (failure) {
        emit(currentState.copyWith(
          searchResultsState: SearchError(message: failure.message),
        ));
      },
      (paginatedResult) {
        if (event.isNewSearch) {
          _currentSearchResults = paginatedResult;
          _currentFilters = _buildFiltersMap(event);
        } else {
          // Append new results to existing ones
          final updatedItems = [
            ..._currentSearchResults!.items,
            ...paginatedResult.items,
          ];
          _currentSearchResults = PaginatedResult(
            items: updatedItems,
            pageNumber: paginatedResult.pageNumber,
            pageSize: paginatedResult.pageSize,
            totalCount: paginatedResult.totalCount,
            metadata: paginatedResult.metadata,
          );
        }

        final hasReachedMax = _currentSearchResults!.items.length >= 
            _currentSearchResults!.totalCount;

        emit(currentState.copyWith(
          searchResultsState: SearchSuccess(
            searchResults: _currentSearchResults!,
            currentFilters: _currentFilters,
            hasReachedMax: hasReachedMax,
          ),
        ));

        // Add to recent searches if it's a new search with query
        if (event.isNewSearch && event.searchTerm != null && event.searchTerm!.isNotEmpty) {
          add(AddToRecentSearchesEvent(suggestion: event.searchTerm!));
        }
      },
    );
  }

  void _onLoadMoreSearchResults(
    LoadMoreSearchResultsEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    if (currentState.searchResultsState is SearchSuccess) {
      final successState = currentState.searchResultsState as SearchSuccess;
      
      if (!successState.hasReachedMax) {
        final nextPage = successState.searchResults.pageNumber + 1;
        
        add(SearchPropertiesEvent(
          searchTerm: _currentFilters['searchTerm'] as String?,
          city: _currentFilters['city'] as String?,
          propertyTypeId: _currentFilters['propertyTypeId'] as String?,
          minPrice: _currentFilters['minPrice'] as double?,
          maxPrice: _currentFilters['maxPrice'] as double?,
          minStarRating: _currentFilters['minStarRating'] as int?,
          requiredAmenities: _currentFilters['requiredAmenities'] as List<String>?,
          unitTypeId: _currentFilters['unitTypeId'] as String?,
          serviceIds: _currentFilters['serviceIds'] as List<String>?,
          dynamicFieldFilters: _currentFilters['dynamicFieldFilters'] as Map<String, dynamic>?,
          checkIn: _currentFilters['checkIn'] as DateTime?,
          checkOut: _currentFilters['checkOut'] as DateTime?,
          guestsCount: _currentFilters['guestsCount'] as int?,
          latitude: _currentFilters['latitude'] as double?,
          longitude: _currentFilters['longitude'] as double?,
          radiusKm: _currentFilters['radiusKm'] as double?,
          sortBy: _currentFilters['sortBy'] as String?,
          pageNumber: nextPage,
          pageSize: successState.searchResults.pageSize,
          isNewSearch: false,
        ));
      }
    }
  }

  void _onGetSearchFilters(
    GetSearchFiltersEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    emit(currentState.copyWith(
      filtersState: const SearchFiltersLoading(),
    ));

    final result = await getSearchFiltersUseCase(NoParams());

    result.fold(
      (failure) {
        emit(currentState.copyWith(
          filtersState: SearchFiltersError(message: failure.message),
        ));
      },
      (filters) {
        emit(currentState.copyWith(
          filtersState: SearchFiltersLoaded(filters: filters),
        ));
      },
    );
  }

  void _onGetSearchSuggestions(
    GetSearchSuggestionsEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    
    if (event.query.isEmpty) {
      emit(currentState.copyWith(
        suggestionsState: const SearchSuggestionsLoaded(suggestions: []),
      ));
      return;
    }

    emit(currentState.copyWith(
      suggestionsState: const SearchSuggestionsLoading(),
    ));

    final params = SearchSuggestionsParams(
      query: event.query,
      limit: event.limit,
    );

    final result = await getSearchSuggestionsUseCase(params);

    result.fold(
      (failure) {
        emit(currentState.copyWith(
          suggestionsState: SearchSuggestionsError(message: failure.message),
        ));
      },
      (suggestions) {
        emit(currentState.copyWith(
          suggestionsState: SearchSuggestionsLoaded(suggestions: suggestions),
        ));
      },
    );
  }

  void _onClearSearchSuggestions(
    ClearSearchSuggestionsEvent event,
    Emitter<SearchState> emit,
  ) {
    final currentState = state as SearchCombinedState;
    emit(currentState.copyWith(
      suggestionsState: const SearchSuggestionsLoaded(suggestions: []),
    ));
  }

  void _onGetRecommendedProperties(
    GetRecommendedPropertiesEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    emit(currentState.copyWith(
      recommendedState: const RecommendedPropertiesLoading(),
    ));

    final result = await searchRepository.getRecommendedProperties(
      userId: event.userId,
      limit: event.limit,
    );

    result.fold(
      (failure) {
        emit(currentState.copyWith(
          recommendedState: RecommendedPropertiesError(message: failure.message),
        ));
      },
      (properties) {
        emit(currentState.copyWith(
          recommendedState: RecommendedPropertiesLoaded(properties: properties),
        ));
      },
    );
  }

  void _onGetPopularDestinations(
    GetPopularDestinationsEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    emit(currentState.copyWith(
      popularDestinationsState: const PopularDestinationsLoading(),
    ));

    final result = await searchRepository.getPopularDestinations(
      limit: event.limit,
    );

    result.fold(
      (failure) {
        emit(currentState.copyWith(
          popularDestinationsState: PopularDestinationsError(message: failure.message),
        ));
      },
      (destinations) {
        emit(currentState.copyWith(
          popularDestinationsState: PopularDestinationsLoaded(destinations: destinations),
        ));
      },
    );
  }

  void _onUpdateSearchFilters(
    UpdateSearchFiltersEvent event,
    Emitter<SearchState> emit,
  ) {
    _currentFilters = _buildFiltersMap(event);
  }

  void _onClearSearchResults(
    ClearSearchResultsEvent event,
    Emitter<SearchState> emit,
  ) {
    final currentState = state as SearchCombinedState;
    _currentSearchResults = null;
    _currentFilters = {};
    emit(currentState.copyWith(
      searchResultsState: const SearchInitial(),
    ));
  }

  void _onAddToRecentSearches(
    AddToRecentSearchesEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    final recentSearches = List<String>.from(currentState.recentSearches);
    
    // Remove if already exists
    recentSearches.remove(event.suggestion);
    
    // Add to beginning
    recentSearches.insert(0, event.suggestion);
    
    // Keep only max allowed
    if (recentSearches.length > _maxRecentSearches) {
      recentSearches.removeRange(_maxRecentSearches, recentSearches.length);
    }
    
    // Save to local storage
    await sharedPreferences.setStringList(_recentSearchesKey, recentSearches);
    
    emit(currentState.copyWith(recentSearches: recentSearches));
  }

  void _onLoadRecentSearches(
    LoadRecentSearchesEvent event,
    Emitter<SearchState> emit,
  ) {
    final currentState = state as SearchCombinedState;
    final recentSearches = sharedPreferences.getStringList(_recentSearchesKey) ?? [];
    emit(currentState.copyWith(recentSearches: recentSearches));
  }

  void _onClearRecentSearches(
    ClearRecentSearchesEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    await sharedPreferences.remove(_recentSearchesKey);
    emit(currentState.copyWith(recentSearches: []));
  }

  void _onSaveSearch(
    SaveSearchEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    final savedSearches = List<SavedSearch>.from(currentState.savedSearches);
    
    final newSearch = SavedSearch(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      name: event.name,
      searchParams: event.searchParams,
      createdAt: DateTime.now(),
    );
    
    savedSearches.add(newSearch);
    
    // Save to local storage
    final savedSearchesJson = savedSearches.map((search) => {
      'id': search.id,
      'name': search.name,
      'searchParams': search.searchParams,
      'createdAt': search.createdAt.toIso8601String(),
    }).toList();
    
    await sharedPreferences.setString(
      _savedSearchesKey,
      json.encode(savedSearchesJson),
    );
    
    emit(currentState.copyWith(savedSearches: savedSearches));
  }

  void _onLoadSavedSearches(
    LoadSavedSearchesEvent event,
    Emitter<SearchState> emit,
  ) {
    final currentState = state as SearchCombinedState;
    final savedSearchesJson = sharedPreferences.getString(_savedSearchesKey);
    
    if (savedSearchesJson != null) {
      final decoded = json.decode(savedSearchesJson) as List;
      final savedSearches = decoded.map((item) => SavedSearch(
        id: item['id'],
        name: item['name'],
        searchParams: Map<String, dynamic>.from(item['searchParams']),
        createdAt: DateTime.parse(item['createdAt']),
      )).toList();
      
      emit(currentState.copyWith(savedSearches: savedSearches));
    }
  }

  void _onDeleteSavedSearch(
    DeleteSavedSearchEvent event,
    Emitter<SearchState> emit,
  ) async {
    final currentState = state as SearchCombinedState;
    final savedSearches = currentState.savedSearches
        .where((search) => search.id != event.searchId)
        .toList();
    
    // Save updated list to local storage
    final savedSearchesJson = savedSearches.map((search) => {
      'id': search.id,
      'name': search.name,
      'searchParams': search.searchParams,
      'createdAt': search.createdAt.toIso8601String(),
    }).toList();
    
    await sharedPreferences.setString(
      _savedSearchesKey,
      json.encode(savedSearchesJson),
    );
    
    emit(currentState.copyWith(savedSearches: savedSearches));
  }

  void _onApplySavedSearch(
    ApplySavedSearchEvent event,
    Emitter<SearchState> emit,
  ) {
    add(SearchPropertiesEvent(
      searchTerm: event.searchParams['searchTerm'] as String?,
      city: event.searchParams['city'] as String?,
      propertyTypeId: event.searchParams['propertyTypeId'] as String?,
      minPrice: event.searchParams['minPrice'] as double?,
      maxPrice: event.searchParams['maxPrice'] as double?,
      minStarRating: event.searchParams['minStarRating'] as int?,
      requiredAmenities: event.searchParams['requiredAmenities'] != null
          ? List<String>.from(event.searchParams['requiredAmenities'])
          : null,
      unitTypeId: event.searchParams['unitTypeId'] as String?,
      serviceIds: event.searchParams['serviceIds'] != null
          ? List<String>.from(event.searchParams['serviceIds'])
          : null,
      dynamicFieldFilters: event.searchParams['dynamicFieldFilters'] != null
          ? Map<String, dynamic>.from(event.searchParams['dynamicFieldFilters'])
          : null,
      checkIn: event.searchParams['checkIn'] != null
          ? DateTime.parse(event.searchParams['checkIn'])
          : null,
      checkOut: event.searchParams['checkOut'] != null
          ? DateTime.parse(event.searchParams['checkOut'])
          : null,
      guestsCount: event.searchParams['guestsCount'] as int?,
      latitude: event.searchParams['latitude'] as double?,
      longitude: event.searchParams['longitude'] as double?,
      radiusKm: event.searchParams['radiusKm'] as double?,
      sortBy: event.searchParams['sortBy'] as String?,
    ));
  }

  Map<String, dynamic> _buildFiltersMap(dynamic event) {
    final filters = <String, dynamic>{};
    
    if (event is SearchPropertiesEvent) {
      if (event.searchTerm != null) filters['searchTerm'] = event.searchTerm;
      if (event.city != null) filters['city'] = event.city;
      if (event.propertyTypeId != null) filters['propertyTypeId'] = event.propertyTypeId;
      if (event.minPrice != null) filters['minPrice'] = event.minPrice;
      if (event.maxPrice != null) filters['maxPrice'] = event.maxPrice;
      if (event.minStarRating != null) filters['minStarRating'] = event.minStarRating;
      if (event.requiredAmenities != null) filters['requiredAmenities'] = event.requiredAmenities;
      if (event.unitTypeId != null) filters['unitTypeId'] = event.unitTypeId;
      if (event.serviceIds != null) filters['serviceIds'] = event.serviceIds;
      if (event.dynamicFieldFilters != null) filters['dynamicFieldFilters'] = event.dynamicFieldFilters;
      if (event.checkIn != null) filters['checkIn'] = event.checkIn;
      if (event.checkOut != null) filters['checkOut'] = event.checkOut;
      if (event.guestsCount != null) filters['guestsCount'] = event.guestsCount;
      if (event.latitude != null) filters['latitude'] = event.latitude;
      if (event.longitude != null) filters['longitude'] = event.longitude;
      if (event.radiusKm != null) filters['radiusKm'] = event.radiusKm;
      if (event.sortBy != null) filters['sortBy'] = event.sortBy;
    } else if (event is UpdateSearchFiltersEvent) {
      if (event.city != null) filters['city'] = event.city;
      if (event.propertyTypeId != null) filters['propertyTypeId'] = event.propertyTypeId;
      if (event.minPrice != null) filters['minPrice'] = event.minPrice;
      if (event.maxPrice != null) filters['maxPrice'] = event.maxPrice;
      if (event.minStarRating != null) filters['minStarRating'] = event.minStarRating;
      if (event.requiredAmenities != null) filters['requiredAmenities'] = event.requiredAmenities;
      if (event.checkIn != null) filters['checkIn'] = event.checkIn;
      if (event.checkOut != null) filters['checkOut'] = event.checkOut;
      if (event.guestsCount != null) filters['guestsCount'] = event.guestsCount;
      if (event.sortBy != null) filters['sortBy'] = event.sortBy;
    }
    
    return filters;
  }
}