// lib/features/home/presentation/bloc/home_event.dart

part of 'home_bloc.dart';

abstract class HomeEvent extends Equatable {
  const HomeEvent();

  @override
  List<Object?> get props => [];
}

class LoadHomeData extends HomeEvent {
  final bool enableAutoRefresh;

  const LoadHomeData({this.enableAutoRefresh = true});

  @override
  List<Object?> get props => [enableAutoRefresh];
}

class RefreshHome extends HomeEvent {
  final bool refreshAll;

  const RefreshHome({this.refreshAll = true});

  @override
  List<Object?> get props => [refreshAll];
}

class LoadMoreSections extends HomeEvent {
  const LoadMoreSections();
}

class RetryLoadHome extends HomeEvent {
  final bool enableAutoRefresh;

  const RetryLoadHome({this.enableAutoRefresh = true});

  @override
  List<Object?> get props => [enableAutoRefresh];
}

class UpdateSearchQuery extends HomeEvent {
  final String query;

  const UpdateSearchQuery(this.query);

  @override
  List<Object?> get props => [query];
}

class UpdateSelectedCity extends HomeEvent {
  final String? city;

  const UpdateSelectedCity(this.city);

  @override
  List<Object?> get props => [city];
}

class UpdateDateRange extends HomeEvent {
  final DateTime? checkIn;
  final DateTime? checkOut;

  const UpdateDateRange({this.checkIn, this.checkOut});

  @override
  List<Object?> get props => [checkIn, checkOut];
}

class UpdateGuestCount extends HomeEvent {
  final int count;

  const UpdateGuestCount(this.count);

  @override
  List<Object?> get props => [count];
}

class ClearFilters extends HomeEvent {
  const ClearFilters();
}

class StartAutoRefresh extends HomeEvent {
  const StartAutoRefresh();
}

class StopAutoRefresh extends HomeEvent {
  const StopAutoRefresh();
}