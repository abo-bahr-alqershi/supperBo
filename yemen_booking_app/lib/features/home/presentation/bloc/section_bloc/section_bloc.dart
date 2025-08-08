// lib/features/home/presentation/bloc/section_bloc/section_bloc.dart

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:injectable/injectable.dart';

import '../../../../../core/enums/section_type_enum.dart';
import '../../../domain/entities/home_section.dart';
import '../../../domain/usecases/get_section_data_usecase.dart';
import '../../../domain/usecases/track_section_impression_usecase.dart';
import '../../../domain/usecases/track_section_interaction_usecase.dart';

part 'section_event.dart';
part 'section_state.dart';

@injectable
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

    try {
      final data = await _getSectionData.call(
        sectionId: event.section.id,
        sectionType: event.section.sectionType,
      );

      emit(SectionLoaded(
        section: event.section,
        data: data,
        isVisible: true,
        hasTrackedImpression: false,
      ));
    } catch (error) {
      emit(SectionError(
        sectionId: event.section.id,
        message: _getErrorMessage(error),
      ));
    }
  }

  Future<void> _onRefreshSection(
    RefreshSection event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    emit(currentState.copyWith(isRefreshing: true));

    try {
      final data = await _getSectionData.call(
        sectionId: currentState.section.id,
        sectionType: currentState.section.sectionType,
      );

      emit(currentState.copyWith(
        data: data,
        isRefreshing: false,
      ));
    } catch (error) {
      emit(currentState.copyWith(
        isRefreshing: false,
        lastError: _getErrorMessage(error),
      ));
    }
  }

  Future<void> _onLoadMoreSectionItems(
    LoadMoreSectionItems event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    if (currentState.hasReachedEnd || currentState.isLoadingMore) return;

    emit(currentState.copyWith(isLoadingMore: true));

    try {
      final moreData = await _getSectionData.call(
        sectionId: currentState.section.id,
        sectionType: currentState.section.sectionType,
        page: currentState.currentPage + 1,
      );

      if (moreData == null || moreData.isEmpty) {
        emit(currentState.copyWith(
          hasReachedEnd: true,
          isLoadingMore: false,
        ));
      } else {
        emit(currentState.copyWith(
          data: currentState.data?.appendData(moreData),
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

  Future<void> _onTrackSectionImpression(
    TrackSectionImpression event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;
    if (currentState.hasTrackedImpression) return;

    try {
      await _trackImpression.call(
        sectionId: currentState.section.id,
        sectionType: currentState.section.sectionType,
      );

      emit(currentState.copyWith(hasTrackedImpression: true));
    } catch (_) {
      // Silently fail for analytics
    }
  }

  Future<void> _onTrackSectionInteraction(
    TrackSectionInteraction event,
    Emitter<SectionState> emit,
  ) async {
    if (state is! SectionLoaded) return;

    final currentState = state as SectionLoaded;

    try {
      await _trackInteraction.call(
        sectionId: currentState.section.id,
        sectionType: currentState.section.sectionType,
        interactionType: event.interactionType,
        itemId: event.itemId,
        metadata: event.metadata,
      );
    } catch (_) {
      // Silently fail for analytics
    }
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