// lib/features/home/presentation/bloc/section_bloc/section_bloc.dart

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';

import '../../../domain/entities/home_section.dart';
import '../../../domain/usecases/get_section_data_usecase.dart';
import '../../../domain/usecases/track_section_impression_usecase.dart';
import '../../../domain/usecases/track_section_interaction_usecase.dart';

part 'section_event.dart';
part 'section_state.dart';

class SectionBloc extends Bloc<SectionEvent, SectionState> {
  final GetSectionDataUseCase _getSectionData;
  final TrackSectionImpressionUseCase _trackImpression;
  final TrackSectionInteractionUseCase _trackInteraction;

  SectionBloc({
    required GetSectionDataUseCase getSectionData,
    required TrackSectionImpressionUseCase trackImpression,
    required TrackSectionInteractionUseCase trackInteraction,
  })  : _getSectionData = getSectionData,
        _trackImpression = trackImpression,
        _trackInteraction = trackInteraction,
        super(const SectionInitial()) {
    on<LoadSectionData>(_onLoadSectionData);
    on<RefreshSection>(_onRefreshSection);
    on<LoadMoreSectionItems>(_onLoadMoreSectionItems);
    on<TrackSectionImpression>(_onTrackSectionImpression);
    on<TrackSectionInteraction>(_onTrackSectionInteraction);
    on<UpdateSectionVisibility>(_onUpdateSectionVisibility);
  }

  Future<void> _onLoadSectionData(
    LoadSectionData event,
    Emitter<SectionState> emit,
  ) async {
    emit(SectionLoading(sectionId: event.section.id));
    final result = await _getSectionData(GetSectionDataParams(sectionId: event.section.id));
    result.fold(
      (failure) => emit(SectionError(sectionId: event.section.id, message: failure.message)),
      (data) => emit(SectionLoaded(
        section: event.section,
        data: data,
        isVisible: true,
        hasTrackedImpression: false,
      )),
    );
  }

  Future<void> _onRefreshSection(
    RefreshSection event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    emit(currentState.copyWith(isRefreshing: true));
    final result = await _getSectionData(GetSectionDataParams(sectionId: currentState.section.id));
    emit(result.fold(
      (failure) => currentState.copyWith(isRefreshing: false, lastError: failure.message),
      (data) => currentState.copyWith(data: data, isRefreshing: false),
    ));
  }

  Future<void> _onLoadMoreSectionItems(
    LoadMoreSectionItems event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    if (currentState.hasReachedEnd || currentState.isLoadingMore) return;

    emit(currentState.copyWith(isLoadingMore: true));
    // Assuming pagination supported later; for now mark end
    emit(currentState.copyWith(hasReachedEnd: true, isLoadingMore: false));
  }

  Future<void> _onTrackSectionImpression(
    TrackSectionImpression event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    if (currentState.hasTrackedImpression) return;
    await _trackImpression(TrackSectionImpressionParams(sectionId: currentState.section.id));
    emit(currentState.copyWith(hasTrackedImpression: true));
  }

  Future<void> _onTrackSectionInteraction(
    TrackSectionInteraction event,
    Emitter<SectionState> emit,
  ) async {
    await _trackInteraction(TrackSectionInteractionParams(
      sectionId: event.sectionId,
      interactionType: event.interactionType,
      itemId: event.itemId,
      metadata: event.metadata,
    ));
  }

  void _onUpdateSectionVisibility(
    UpdateSectionVisibility event,
    Emitter<SectionState> emit,
  ) {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    emit(currentState.copyWith(isVisible: event.isVisible));

    // Track impression when section becomes visible
    if (event.isVisible && !currentState.hasTrackedImpression) {
      add(TrackSectionImpression(currentState.section.id));
    }
  }

  String _getErrorMessage(dynamic error) {
    if (error is Exception) {
      return error.toString();
    }
    return 'فشل تحميل المحتوى';
  }
}