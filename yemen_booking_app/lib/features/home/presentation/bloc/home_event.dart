import 'package:equatable/equatable.dart';

abstract class HomeEvent extends Equatable {
  const HomeEvent();

  @override
  List<Object?> get props => [];
}

class LoadHomeData extends HomeEvent {
  final bool forceRefresh;

  const LoadHomeData({this.forceRefresh = false});

  @override
  List<Object> get props => [forceRefresh];
}

class LoadSectionData extends HomeEvent {
  final String sectionId;
  final Map<String, dynamic>? params;

  const LoadSectionData({required this.sectionId, this.params});

  @override
  List<Object?> get props => [sectionId, params];
}

class RefreshHome extends HomeEvent {}

class TrackImpression extends HomeEvent {
  final String sectionId;
  final String itemId;

  const TrackImpression({required this.sectionId, required this.itemId});

  @override
  List<Object> get props => [sectionId, itemId];
}

class TrackInteraction extends HomeEvent {
  final String sectionId;
  final String itemId;
  final String interactionType;

  const TrackInteraction({required this.sectionId, required this.itemId, required this.interactionType});

  @override
  List<Object> get props => [sectionId, itemId, interactionType];
}
