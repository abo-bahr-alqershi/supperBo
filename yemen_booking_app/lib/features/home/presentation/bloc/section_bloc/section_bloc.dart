// lib/features/home/presentation/bloc/section_bloc/section_bloc.dart

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';

import '../../../domain/entities/home_section.dart';

part 'section_event.dart';
part 'section_state.dart';

class SectionBloc extends Bloc<SectionEvent, SectionState> {
  SectionBloc() : super(const SectionInitial()) {
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
    // Minimal placeholder: pass-through any provided data; otherwise empty list
    emit(SectionLoaded(
      section: event.section,
      data: const [],
      isVisible: true,
      hasTrackedImpression: false,
    ));
  }

  Future<void> _onRefreshSection(
    RefreshSection event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    emit(currentState.copyWith(isRefreshing: true));
    emit(currentState.copyWith(isRefreshing: false));
  }

  Future<void> _onLoadMoreSectionItems(
    LoadMoreSectionItems event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    if (currentState.hasReachedEnd || currentState.isLoadingMore) return;

    emit(currentState.copyWith(isLoadingMore: true));
    emit(currentState.copyWith(
      hasReachedEnd: true,
      isLoadingMore: false,
    ));
  }

  Future<void> _onTrackSectionImpression(
    TrackSectionImpression event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    if (currentState.hasTrackedImpression) return;
    emit(currentState.copyWith(hasTrackedImpression: true));
  }

  Future<void> _onTrackSectionInteraction(
    TrackSectionInteraction event,
    Emitter<SectionState> emit,
  ) async {
    // No-op in minimal implementation
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