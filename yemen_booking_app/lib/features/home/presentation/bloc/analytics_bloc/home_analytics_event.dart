// lib/features/home/presentation/bloc/analytics_bloc/home_analytics_event.dart

part of 'home_analytics_bloc.dart';

abstract class HomeAnalyticsEvent extends Equatable {
  const HomeAnalyticsEvent();

  @override
  List<Object?> get props => [];
}

class TrackScreenView extends HomeAnalyticsEvent {
  final String screenName;
  final Map<String, dynamic> properties;

  const TrackScreenView({
    required this.screenName,
    this.properties = const {},
  });

  @override
  List<Object?> get props => [screenName, properties];
}

class TrackSectionView extends HomeAnalyticsEvent {
  final String sectionId;
  final String sectionType;
  final int position;

  const TrackSectionView({
    required this.sectionId,
    required this.sectionType,
    required this.position,
  });

  @override
  List<Object?> get props => [sectionId, sectionType, position];
}

class TrackItemClick extends HomeAnalyticsEvent {
  final String sectionId;
  final String itemId;
  final String itemType;
  final int position;
  final Map<String, dynamic> metadata;

  const TrackItemClick({
    required this.sectionId,
    required this.itemId,
    required this.itemType,
    required this.position,
    this.metadata = const {},
  });

  @override
  List<Object?> get props => [sectionId, itemId, itemType, position, metadata];
}

class TrackSearchPerformed extends HomeAnalyticsEvent {
  final String query;
  final Map<String, dynamic> filters;
  final int resultsCount;

  const TrackSearchPerformed({
    required this.query,
    required this.filters,
    required this.resultsCount,
  });

  @override
  List<Object?> get props => [query, filters, resultsCount];
}

class TrackFilterApplied extends HomeAnalyticsEvent {
  final String filterType;
  final dynamic filterValue;

  const TrackFilterApplied({
    required this.filterType,
    required this.filterValue,
  });

  @override
  List<Object?> get props => [filterType, filterValue];
}

class TrackScrollDepth extends HomeAnalyticsEvent {
  final double depthPercentage;
  final int sectionsViewed;

  const TrackScrollDepth({
    required this.depthPercentage,
    required this.sectionsViewed,
  });

  @override
  List<Object?> get props => [depthPercentage, sectionsViewed];
}

class GetAnalyticsSummary extends HomeAnalyticsEvent {
  const GetAnalyticsSummary();
}