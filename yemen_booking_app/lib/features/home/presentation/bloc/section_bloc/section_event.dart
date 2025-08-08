// lib/features/home/presentation/bloc/section_bloc/section_event.dart

part of 'section_bloc.dart';

abstract class SectionEvent extends Equatable {
  const SectionEvent();

  @override
  List<Object?> get props => [];
}

class LoadSectionData extends SectionEvent {
  final HomeSection section;

  const LoadSectionData(this.section);

  @override
  List<Object?> get props => [section];
}

class RefreshSection extends SectionEvent {
  final String sectionId;

  const RefreshSection(this.sectionId);

  @override
  List<Object?> get props => [sectionId];
}

class LoadMoreSectionItems extends SectionEvent {
  final String sectionId;

  const LoadMoreSectionItems(this.sectionId);

  @override
  List<Object?> get props => [sectionId];
}

class TrackSectionImpression extends SectionEvent {
  final String sectionId;

  const TrackSectionImpression(this.sectionId);

  @override
  List<Object?> get props => [sectionId];
}

class TrackSectionInteraction extends SectionEvent {
  final String sectionId;
  final String interactionType;
  final String? itemId;
  final Map<String, dynamic>? metadata;

  const TrackSectionInteraction({
    required this.sectionId,
    required this.interactionType,
    this.itemId,
    this.metadata,
  });

  @override
  List<Object?> get props => [sectionId, interactionType, itemId, metadata];
}

class UpdateSectionVisibility extends SectionEvent {
  final String sectionId;
  final bool isVisible;

  const UpdateSectionVisibility({
    required this.sectionId,
    required this.isVisible,
  });

  @override
  List<Object?> get props => [sectionId, isVisible];
}