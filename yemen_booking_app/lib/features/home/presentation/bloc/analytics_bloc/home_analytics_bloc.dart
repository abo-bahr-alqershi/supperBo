import 'package:flutter_bloc/flutter_bloc.dart';
import 'home_analytics_event.dart';
import 'home_analytics_state.dart';
import '../../../domain/usecases/track_section_impression_usecase.dart';
import '../../../domain/usecases/track_section_interaction_usecase.dart';

class HomeAnalyticsBloc extends Bloc<HomeAnalyticsEvent, HomeAnalyticsState> {
  final TrackSectionImpressionUseCase trackImpression;
  final TrackSectionInteractionUseCase trackInteraction;

  HomeAnalyticsBloc({required this.trackImpression, required this.trackInteraction}) : super(HomeAnalyticsIdle()) {
    on<LogSectionImpression>(_onLogImpression);
    on<LogSectionInteraction>(_onLogInteraction);
  }

  Future<void> _onLogImpression(LogSectionImpression event, Emitter<HomeAnalyticsState> emit) async {
    emit(HomeAnalyticsLogging());
    try {
      await trackImpression(TrackSectionImpressionParams(sectionId: event.sectionId, itemId: event.itemId));
      emit(HomeAnalyticsIdle());
    } catch (e) {
      emit(HomeAnalyticsError(e.toString()));
    }
  }

  Future<void> _onLogInteraction(LogSectionInteraction event, Emitter<HomeAnalyticsState> emit) async {
    emit(HomeAnalyticsLogging());
    try {
      await trackInteraction(TrackSectionInteractionParams(
        sectionId: event.sectionId,
        itemId: event.itemId,
        interactionType: event.interactionType,
        metadata: event.metadata,
      ));
      emit(HomeAnalyticsIdle());
    } catch (e) {
      emit(HomeAnalyticsError(e.toString()));
    }
  }
}