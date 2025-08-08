// lib/features/home/presentation/bloc/analytics_bloc/home_analytics_state.dart

part of 'home_analytics_bloc.dart';

abstract class HomeAnalyticsState extends Equatable {
  const HomeAnalyticsState();

  @override
  List<Object?> get props => [];
}

class HomeAnalyticsInitial extends HomeAnalyticsState {
  const HomeAnalyticsInitial();
}

class HomeAnalyticsTracked extends HomeAnalyticsState {
  final String eventType;
  final DateTime timestamp;

  const HomeAnalyticsTracked({
    required this.eventType,
    required this.timestamp,
  });

  @override
  List<Object?> get props => [eventType, timestamp];
}

class HomeAnalyticsSummary extends HomeAnalyticsState {
  final AnalyticsSummary summary;

  const HomeAnalyticsSummary({required this.summary});

  @override
  List<Object?> get props => [summary];
}