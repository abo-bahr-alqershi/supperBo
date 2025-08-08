// lib/features/home/presentation/bloc/analytics_bloc/home_analytics_bloc.dart

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:injectable/injectable.dart';

part 'home_analytics_event.dart';
part 'home_analytics_state.dart';

@injectable
class HomeAnalyticsBloc extends Bloc<HomeAnalyticsEvent, HomeAnalyticsState> {
  final Map<String, DateTime> _impressionTimestamps = {};
  final Map<String, int> _interactionCounts = {};
  final List<AnalyticsRecord> _analyticsRecords = [];

  HomeAnalyticsBloc() : super(const HomeAnalyticsInitial()) {
    on<TrackScreenView>(_onTrackScreenView);
    on<TrackSectionView>(_onTrackSectionView);
    on<TrackItemClick>(_onTrackItemClick);
    on<TrackSearchPerformed>(_onTrackSearchPerformed);
    on<TrackFilterApplied>(_onTrackFilterApplied);
    on<TrackScrollDepth>(_onTrackScrollDepth);
    on<GetAnalyticsSummary>(_onGetAnalyticsSummary);
  }

  void _onTrackScreenView(
    TrackScreenView event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    _analyticsRecords.add(AnalyticsRecord(
      type: 'screen_view',
      data: {
        'screen': event.screenName,
        'timestamp': DateTime.now().toIso8601String(),
        ...event.properties,
      },
    ));

    emit(HomeAnalyticsTracked(
      eventType: 'screen_view',
      timestamp: DateTime.now(),
    ));
  }

  void _onTrackSectionView(
    TrackSectionView event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    final key = '${event.sectionId}_${event.sectionType}';
    _impressionTimestamps[key] = DateTime.now();

    _analyticsRecords.add(AnalyticsRecord(
      type: 'section_view',
      data: {
        'section_id': event.sectionId,
        'section_type': event.sectionType,
        'position': event.position,
        'timestamp': DateTime.now().toIso8601String(),
      },
    ));

    emit(HomeAnalyticsTracked(
      eventType: 'section_view',
      timestamp: DateTime.now(),
    ));
  }

  void _onTrackItemClick(
    TrackItemClick event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    final key = '${event.sectionId}_${event.itemId}';
    _interactionCounts[key] = (_interactionCounts[key] ?? 0) + 1;

    _analyticsRecords.add(AnalyticsRecord(
      type: 'item_click',
      data: {
        'section_id': event.sectionId,
        'item_id': event.itemId,
        'item_type': event.itemType,
        'position': event.position,
        'timestamp': DateTime.now().toIso8601String(),
        ...event.metadata,
      },
    ));

    emit(HomeAnalyticsTracked(
      eventType: 'item_click',
      timestamp: DateTime.now(),
    ));
  }

  void _onTrackSearchPerformed(
    TrackSearchPerformed event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    _analyticsRecords.add(AnalyticsRecord(
      type: 'search_performed',
      data: {
        'query': event.query,
        'filters': event.filters,
        'results_count': event.resultsCount,
        'timestamp': DateTime.now().toIso8601String(),
      },
    ));

    emit(HomeAnalyticsTracked(
      eventType: 'search_performed',
      timestamp: DateTime.now(),
    ));
  }

  void _onTrackFilterApplied(
    TrackFilterApplied event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    _analyticsRecords.add(AnalyticsRecord(
      type: 'filter_applied',
      data: {
        'filter_type': event.filterType,
        'filter_value': event.filterValue,
        'timestamp': DateTime.now().toIso8601String(),
      },
    ));

    emit(HomeAnalyticsTracked(
      eventType: 'filter_applied',
      timestamp: DateTime.now(),
    ));
  }

  void _onTrackScrollDepth(
    TrackScrollDepth event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    _analyticsRecords.add(AnalyticsRecord(
      type: 'scroll_depth',
      data: {
        'depth_percentage': event.depthPercentage,
        'sections_viewed': event.sectionsViewed,
        'timestamp': DateTime.now().toIso8601String(),
      },
    ));

    emit(HomeAnalyticsTracked(
      eventType: 'scroll_depth',
      timestamp: DateTime.now(),
    ));
  }

  void _onGetAnalyticsSummary(
    GetAnalyticsSummary event,
    Emitter<HomeAnalyticsState> emit,
  ) {
    final summary = AnalyticsSummary(
      totalImpressions: _impressionTimestamps.length,
      totalInteractions: _interactionCounts.values.fold(0, (a, b) => a + b),
      totalEvents: _analyticsRecords.length,
      impressionTimestamps: Map.from(_impressionTimestamps),
      interactionCounts: Map.from(_interactionCounts),
      recentEvents: _analyticsRecords.take(10).toList(),
    );

    emit(HomeAnalyticsSummary(summary: summary));
  }
}

class AnalyticsRecord {
  final String type;
  final Map<String, dynamic> data;

  AnalyticsRecord({
    required this.type,
    required this.data,
  });
}

class AnalyticsSummary {
  final int totalImpressions;
  final int totalInteractions;
  final int totalEvents;
  final Map<String, DateTime> impressionTimestamps;
  final Map<String, int> interactionCounts;
  final List<AnalyticsRecord> recentEvents;

  AnalyticsSummary({
    required this.totalImpressions,
    required this.totalInteractions,
    required this.totalEvents,
    required this.impressionTimestamps,
    required this.interactionCounts,
    required this.recentEvents,
  });
}