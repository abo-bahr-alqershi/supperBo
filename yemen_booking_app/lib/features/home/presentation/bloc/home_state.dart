// lib/features/home/presentation/bloc/home_state.dart

part of 'home_bloc.dart';

abstract class HomeState extends Equatable {
  const HomeState();

  @override
  List<Object?> get props => [];
}

class HomeInitial extends HomeState {
  const HomeInitial();
}

class HomeLoading extends HomeState {
  const HomeLoading();
}

class HomeLoaded extends HomeState {
  final List<HomeSection> sections;
  final List<FeaturedProperty> featuredProperties;
  final List<CityDestination> destinations;
  final String searchQuery;
  final String? selectedCity;
  final DateTime? checkInDate;
  final DateTime? checkOutDate;
  final int guestCount;
  final bool isRefreshing;
  final bool isLoadingMore;
  final bool hasReachedEnd;
  final int currentPage;
  final String? lastError;

  const HomeLoaded({
    required this.sections,
    required this.featuredProperties,
    required this.destinations,
    required this.searchQuery,
    this.selectedCity,
    this.checkInDate,
    this.checkOutDate,
    required this.guestCount,
    required this.isRefreshing,
    this.isLoadingMore = false,
    required this.hasReachedEnd,
    required this.currentPage,
    this.lastError,
  });

  HomeLoaded copyWith({
    List<HomeSection>? sections,
    List<FeaturedProperty>? featuredProperties,
    List<CityDestination>? destinations,
    String? searchQuery,
    String? selectedCity,
    DateTime? checkInDate,
    DateTime? checkOutDate,
    int? guestCount,
    bool? isRefreshing,
    bool? isLoadingMore,
    bool? hasReachedEnd,
    int? currentPage,
    String? lastError,
  }) {
    return HomeLoaded(
      sections: sections ?? this.sections,
      featuredProperties: featuredProperties ?? this.featuredProperties,
      destinations: destinations ?? this.destinations,
      searchQuery: searchQuery ?? this.searchQuery,
      selectedCity: selectedCity ?? this.selectedCity,
      checkInDate: checkInDate ?? this.checkInDate,
      checkOutDate: checkOutDate ?? this.checkOutDate,
      guestCount: guestCount ?? this.guestCount,
      isRefreshing: isRefreshing ?? this.isRefreshing,
      isLoadingMore: isLoadingMore ?? this.isLoadingMore,
      hasReachedEnd: hasReachedEnd ?? this.hasReachedEnd,
      currentPage: currentPage ?? this.currentPage,
      lastError: lastError ?? this.lastError,
    );
  }

  @override
  List<Object?> get props => [
        sections,
        featuredProperties,
        destinations,
        searchQuery,
        selectedCity,
        checkInDate,
        checkOutDate,
        guestCount,
        isRefreshing,
        isLoadingMore,
        hasReachedEnd,
        currentPage,
        lastError,
      ];
}

class HomeError extends HomeState {
  final String message;
  final bool canRetry;

  const HomeError({
    required this.message,
    this.canRetry = true,
  });

  @override
  List<Object?> get props => [message, canRetry];
}