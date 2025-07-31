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
      searchQuery: event.searchQuery,
      city: event.city,
      propertyType: event.propertyType,
      minPrice: event.minPrice,
      maxPrice: event.maxPrice,
      minRating: event.minRating,
      amenities: event.amenities,
      checkInDate: event.checkInDate,
      checkOutDate: event.checkOutDate,
      guests: event.guests,
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
        if (event.isNewSearch && event.searchQuery != null && event.searchQuery!.isNotEmpty) {
          add(AddToRecentSearchesEvent(searchQuery: event.searchQuery!));
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
          searchQuery: _currentFilters['searchQuery'],
          city: _currentFilters['city'],
          propertyType: _currentFilters['propertyType'],
          minPrice: _currentFilters['minPrice'],
          maxPrice: _currentFilters['maxPrice'],
          minRating: _currentFilters['minRating'],
          amenities: _currentFilters['amenities'],
          checkInDate: _currentFilters['checkInDate'],
          checkOutDate: _currentFilters['checkOutDate'],
          guests: _currentFilters['guests'],
          latitude: _currentFilters['latitude'],
          longitude: _currentFilters['longitude'],
          radiusKm: _currentFilters['radiusKm'],
          sortBy: _currentFilters['sortBy'],
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
    recentSearches.remove(event.searchQuery);
    
    // Add to beginning
    recentSearches.insert(0, event.searchQuery);
    
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
      searchQuery: event.searchParams['searchQuery'],
      city: event.searchParams['city'],
      propertyType: event.searchParams['propertyType'],
      minPrice: event.searchParams['minPrice'],
      maxPrice: event.searchParams['maxPrice'],
      minRating: event.searchParams['minRating'],
      amenities: event.searchParams['amenities'] != null
          ? List<String>.from(event.searchParams['amenities'])
          : null,
      checkInDate: event.searchParams['checkInDate'] != null
          ? DateTime.parse(event.searchParams['checkInDate'])
          : null,
      checkOutDate: event.searchParams['checkOutDate'] != null
          ? DateTime.parse(event.searchParams['checkOutDate'])
          : null,
      guests: event.searchParams['guests'],
      latitude: event.searchParams['latitude'],
      longitude: event.searchParams['longitude'],
      radiusKm: event.searchParams['radiusKm'],
      sortBy: event.searchParams['sortBy'],
    ));
  }

  Map<String, dynamic> _buildFiltersMap(dynamic event) {
    final filters = <String, dynamic>{};
    
    if (event is SearchPropertiesEvent) {
      if (event.searchQuery != null) filters['searchQuery'] = event.searchQuery;
      if (event.city != null) filters['city'] = event.city;
      if (event.propertyType != null) filters['propertyType'] = event.propertyType;
      if (event.minPrice != null) filters['minPrice'] = event.minPrice;
      if (event.maxPrice != null) filters['maxPrice'] = event.maxPrice;
      if (event.minRating != null) filters['minRating'] = event.minRating;
      if (event.amenities != null) filters['amenities'] = event.amenities;
      if (event.checkInDate != null) filters['checkInDate'] = event.checkInDate;
      if (event.checkOutDate != null) filters['checkOutDate'] = event.checkOutDate;
      if (event.guests != null) filters['guests'] = event.guests;
      if (event.latitude != null) filters['latitude'] = event.latitude;
      if (event.longitude != null) filters['longitude'] = event.longitude;
      if (event.radiusKm != null) filters['radiusKm'] = event.radiusKm;
      if (event.sortBy != null) filters['sortBy'] = event.sortBy;
    } else if (event is UpdateSearchFiltersEvent) {
      if (event.city != null) filters['city'] = event.city;
      if (event.propertyType != null) filters['propertyType'] = event.propertyType;
      if (event.minPrice != null) filters['minPrice'] = event.minPrice;
      if (event.maxPrice != null) filters['maxPrice'] = event.maxPrice;
      if (event.minRating != null) filters['minRating'] = event.minRating;
      if (event.amenities != null) filters['amenities'] = event.amenities;
      if (event.checkInDate != null) filters['checkInDate'] = event.checkInDate;
      if (event.checkOutDate != null) filters['checkOutDate'] = event.checkOutDate;
      if (event.guests != null) filters['guests'] = event.guests;
      if (event.sortBy != null) filters['sortBy'] = event.sortBy;
    }
    
    return filters;
  }
}