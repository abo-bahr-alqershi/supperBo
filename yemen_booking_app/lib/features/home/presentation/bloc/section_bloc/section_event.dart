import 'package:equatable/equatable.dart';

abstract class SectionEvent extends Equatable {
  const SectionEvent();
  @override
  List<Object?> get props => [];
}

class LoadSection extends SectionEvent {
  final String sectionId;
  final Map<String, dynamic>? params;
  const LoadSection({required this.sectionId, this.params});
  @override
  List<Object?> get props => [sectionId, params];
}

class RefreshSection extends SectionEvent {
  final String sectionId;
  const RefreshSection(this.sectionId);
  @override
  List<Object> get props => [sectionId];
}

class TrackSectionImpression extends SectionEvent {
  final String sectionId;
  final String itemId;
  const TrackSectionImpression({required this.sectionId, required this.itemId});
  @override
  List<Object> get props => [sectionId, itemId];
}

class TrackSectionInteraction extends SectionEvent {
  final String sectionId;
  final String itemId;
  final String interactionType;
  const TrackSectionInteraction({required this.sectionId, required this.itemId, required this.interactionType});
  @override
  List<Object> get props => [sectionId, itemId, interactionType];
}