// lib/features/home/presentation/bloc/home_bloc.dart

import 'dart:async';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:injectable/injectable.dart';

import '../../domain/entities/home_section.dart';
import '../../domain/entities/featured_property.dart';
import '../../domain/entities/city_destination.dart';
import '../../domain/usecases/get_home_config_usecase.dart';
import '../../domain/usecases/get_featured_properties_usecase.dart';
import '../../domain/usecases/get_popular_destinations_usecase.dart';
import '../../domain/usecases/refresh_home_sections_usecase.dart';

part 'home_event.dart';
part 'home_state.dart';

@injectable
class HomeBloc extends Bloc<HomeEvent, HomeState> {
  final GetHomeConfigUseCase _getHomeConfig;
  final GetFeaturedPropertiesUseCase _getFeaturedProperties;
  final GetPopularDestinationsUseCase _getPopularDestinations;
  final RefreshHomeSectionsUseCase _refreshHomeSections;

  Timer? _refreshTimer;
  static const Duration _autoRefreshDuration = Duration(minutes: 5);

  HomeBloc({
    required GetHomeConfigUseCase getHomeConfig,
    required GetFeaturedPropertiesUseCase getFeaturedProperties,
    required GetPopularDestinationsUseCase getPopularDestinations,
    required RefreshHomeSectionsUseCase refreshHomeSections,
  })  : _getHomeConfig = getHomeConfig,
        _getFeaturedProperties = getFeaturedProperties,
        _getPopularDestinations = getPopularDestinations,
        _refreshHomeSections = refreshHomeSections,
        super(const HomeInitial()) {
    on<LoadHomeData>(_onLoadHomeData);
    on<RefreshHome>(_onRefreshHome);
    on<LoadMoreSections>(_onLoadMoreSections);
    on<RetryLoadHome>(_onRetryLoadHome);
    on<UpdateSearchQuery>(_onUpdateSearchQuery);
    on<UpdateSelectedCity>(_onUpdateSelectedCity);
    on<UpdateDateRange>(_onUpdateDateRange);
    on<UpdateGuestCount>(_onUpdateGuestCount);
    on<ClearFilters>(_onClearFilters);
    on<StartAutoRefresh>(_onStartAutoRefresh);
    on<StopAutoRefresh>(_onStopAutoRefresh);
  }

  Future<void> _onLoadHomeData(
    LoadHomeData event,
    Emitter<HomeState> emit,
  ) async {
    emit(const HomeLoading());

    try {
      // Load all home data in parallel
      final results = await Future.wait([
        _getHomeConfig.call(),
        _getFeaturedProperties.call(),
        _getPopularDestinations.call(),
      ]);

      final config = results[0];
      final featuredProperties = results[1] as List<FeaturedProperty>;
      final destinations = results[2] as List<CityDestination>;

      // Load sections based on config
      final sections = await _loadSections(config);

      emit(HomeLoaded(
        sections: sections,
        featuredProperties: featuredProperties,
        destinations: destinations,
        searchQuery: '',
        selectedCity: null,
        checkInDate: null,
        checkOutDate: null,
        guestCount: 1,
        isRefreshing: false,
        hasReachedEnd: false,
        currentPage: 1,
      ));

      // Start auto-refresh if enabled
      if (event.enableAutoRefresh) {
        add(const StartAutoRefresh());
      }
    } catch (error) {
      emit(HomeError(
        message: _getErrorMessage(error),
        canRetry: true,
      ));
    }
  }

  Future<void> _onRefreshHome(
    RefreshHome event,
    Emitter<HomeState> emit,
  ) async {
    if (state is! HomeLoaded) return;

    final currentState = state as HomeLoaded;
    emit(currentState.copyWith(isRefreshing: true));

    try {
      final refreshedSections = await _refreshHomeSections.call();
      
      // Optionally refresh featured properties and destinations
      final featuredProperties = event.refreshAll 
          ? await _getFeaturedProperties.call()
          : currentState.featuredProperties;
      
      final destinations = event.refreshAll
          ? await _getPopularDestinations.call()
          : currentState.destinations;

      emit(currentState.copyWith(
        sections: refreshedSections,
        featuredProperties: featuredProperties,
        destinations: destinations,
        isRefreshing: false,
      ));
    } catch (error) {
      emit(currentState.copyWith(
        isRefreshing: false,
        lastError: _getErrorMessage(error),
      ));
    }
  }

  Future<void> _onLoadMoreSections(
    LoadMoreSections event,
    Emitter<HomeState> emit,
  ) async {
    if (state is! HomeLoaded) return;

    final currentState = state as HomeLoaded;
    if (currentState.hasReachedEnd || currentState.isLoadingMore) return;

    emit(currentState.copyWith(isLoadingMore: true));

    try {
      // Simulate loading more sections with pagination
      final moreSections = await _loadMoreSectionsFromApi(
        page: currentState.currentPage + 1,
      );

      if (moreSections.isEmpty) {
        emit(currentState.copyWith(
          hasReachedEnd: true,
          isLoadingMore: false,
        ));
      } else {
        emit(currentState.copyWith(
          sections: [...currentState.sections, ...moreSections],
          currentPage: currentState.currentPage + 1,
          isLoadingMore: false,
        ));
      }
    } catch (error) {
      emit(currentState.copyWith(
        isLoadingMore: false,
        lastError: _getErrorMessage(error),
      ));
    }
  }

  Future<void> _onRetryLoadHome(
    RetryLoadHome event,
    Emitter<HomeState> emit,
  ) async {
    add(LoadHomeData(enableAutoRefresh: event.enableAutoRefresh));
  }

  void _onUpdateSearchQuery(
    UpdateSearchQuery event,
    Emitter<HomeState> emit,
  ) {
    if (state is! HomeLoaded) return;
    
    final currentState = state as HomeLoaded;
    emit(currentState.copyWith(searchQuery: event.query));
  }

  void _onUpdateSelectedCity(
    UpdateSelectedCity event,
    Emitter<HomeState> emit,
  ) {
    if (state is! HomeLoaded) return;
    
    final currentState = state as HomeLoaded;
    emit(currentState.copyWith(selectedCity: event.city));
  }

  void _onUpdateDateRange(
    UpdateDateRange event,
    Emitter<HomeState> emit,
  ) {
    if (state is! HomeLoaded) return;
    
    final currentState = state as HomeLoaded;
    emit(currentState.copyWith(
      checkInDate: event.checkIn,
      checkOutDate: event.checkOut,
    ));
  }

  void _onUpdateGuestCount(
    UpdateGuestCount event,
    Emitter<HomeState> emit,
  ) {
    if (state is! HomeLoaded) return;
    
    final currentState = state as HomeLoaded;
    emit(currentState.copyWith(guestCount: event.count));
  }

  void _onClearFilters(
    ClearFilters event,
    Emitter<HomeState> emit,
  ) {
    if (state is! HomeLoaded) return;
    
    final currentState = state as HomeLoaded;
    emit(currentState.copyWith(
      searchQuery: '',
      selectedCity: null,
      checkInDate: null,
      checkOutDate: null,
      guestCount: 1,
    ));
  }

  void _onStartAutoRefresh(
    StartAutoRefresh event,
    Emitter<HomeState> emit,
  ) {
    _refreshTimer?.cancel();
    _refreshTimer = Timer.periodic(_autoRefreshDuration, (_) {
      if (!isClosed) {
        add(const RefreshHome(refreshAll: false));
      }
    });
  }

  void _onStopAutoRefresh(
    StopAutoRefresh event,
    Emitter<HomeState> emit,
  ) {
    _refreshTimer?.cancel();
    _refreshTimer = null;
  }

  // Helper methods
  Future<List<HomeSection>> _loadSections(dynamic config) async {
    // Implementation to load sections based on config
    // This would typically call a use case or repository
    return [];
  }

  Future<List<HomeSection>> _loadMoreSectionsFromApi({required int page}) async {
    // Implementation to load more sections with pagination
    return [];
  }

  String _getErrorMessage(dynamic error) {
    if (error is Exception) {
      return error.toString();
    }
    return 'حدث خطأ غير متوقع';
  }

  @override
  Future<void> close() {
    _refreshTimer?.cancel();
    return super.close();
  }
}