// lib/features/home/presentation/bloc/home_bloc.dart

import 'dart:async';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';

import '../../domain/entities/home_section.dart';
import '../../domain/entities/featured_property.dart';
import '../../domain/entities/city_destination.dart';
import '../../domain/usecases/get_home_config_usecase.dart';
import '../../domain/usecases/get_home_sections_usecase.dart';
import '../../domain/usecases/get_city_destinations_usecase.dart';
import '../../domain/usecases/get_sponsored_ads_usecase.dart';
import '../../domain/usecases/record_ad_impression_usecase.dart';
import '../../domain/usecases/record_ad_click_usecase.dart';
import '../../../../core/usecases/usecase.dart';

part 'home_event.dart';
part 'home_state.dart';

class HomeBloc extends Bloc<HomeEvent, HomeState> {
  final GetHomeConfigUseCase _getHomeConfigUseCase;
  final GetHomeSectionsUseCase _getHomeSectionsUseCase;
  final GetCityDestinationsUseCase _getCityDestinationsUseCase;
  final GetSponsoredAdsUseCase _getSponsoredAdsUseCase;
  final RecordAdImpressionUseCase _recordAdImpressionUseCase;
  final RecordAdClickUseCase _recordAdClickUseCase;

  Timer? _refreshTimer;
  static const Duration _autoRefreshDuration = Duration(minutes: 5);

  HomeBloc({
    required GetHomeConfigUseCase getHomeConfigUseCase,
    required GetHomeSectionsUseCase getHomeSectionsUseCase,
    required GetSponsoredAdsUseCase getSponsoredAdsUseCase,
    required GetCityDestinationsUseCase getCityDestinationsUseCase,
    required RecordAdImpressionUseCase recordAdImpressionUseCase,
    required RecordAdClickUseCase recordAdClickUseCase,
  })  : _getHomeConfigUseCase = getHomeConfigUseCase,
        _getHomeSectionsUseCase = getHomeSectionsUseCase,
        _getSponsoredAdsUseCase = getSponsoredAdsUseCase,
        _getCityDestinationsUseCase = getCityDestinationsUseCase,
        _recordAdImpressionUseCase = recordAdImpressionUseCase,
        _recordAdClickUseCase = recordAdClickUseCase,
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
      final configEither = await _getHomeConfigUseCase(const GetHomeConfigParams());
      final sectionsEither = await _getHomeSectionsUseCase(const GetHomeSectionsParams());
      final destinationsEither = await _getCityDestinationsUseCase(NoParams());

      // Extract values or throw
      final config = configEither.fold((failure) => throw failure, (r) => r);
      final sections = sectionsEither.fold((failure) => throw failure, (r) => r);
      final destinations = destinationsEither.fold((failure) => throw failure, (r) => r);

      // Optionally: we could also fetch sponsored ads via _getSponsoredAdsUseCase, but UI expects FeaturedProperty list.
      final List<FeaturedProperty> featuredProperties = const [];

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

      if (event.enableAutoRefresh) {
        add(const StartAutoRefresh());
      }
    } catch (error) {
      emit(const HomeError(
        message: 'فشل تحميل الصفحة الرئيسية، يرجى المحاولة لاحقاً',
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
      // Re-load sections (acts as refresh)
      final sectionsEither = await _getHomeSectionsUseCase(const GetHomeSectionsParams());
      final refreshedSections = sectionsEither.fold((f) => currentState.sections, (r) => r);

      // Optionally refresh destinations
      final destinations = event.refreshAll
          ? (await _getCityDestinationsUseCase(NoParams())).fold((f) => currentState.destinations, (r) => r)
          : currentState.destinations;

      emit(currentState.copyWith(
        sections: refreshedSections,
        destinations: destinations,
        isRefreshing: false,
      ));
    } catch (_) {
      emit(currentState.copyWith(
        isRefreshing: false,
        lastError: 'تعذر تحديث المحتوى حالياً',
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
      // Pagination not implemented on server yet for sections → keep as reached end
      emit(currentState.copyWith(
        hasReachedEnd: true,
        isLoadingMore: false,
      ));
    } catch (_) {
      emit(currentState.copyWith(
        isLoadingMore: false,
        lastError: 'تعذر تحميل المزيد من الأقسام',
      ));
    }
  }

  void _onRetryLoadHome(
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

  @override
  Future<void> close() {
    _refreshTimer?.cancel();
    return super.close();
  }
}