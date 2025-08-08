import 'package:equatable/equatable.dart';

abstract class HomeAnalyticsEvent extends Equatable {
  const HomeAnalyticsEvent();
  @override
  List<Object?> get props => [];
}

class LogSectionImpression extends HomeAnalyticsEvent {
  final String sectionId;
  final String? itemId;
  const LogSectionImpression({required this.sectionId, this.itemId});
  @override
  List<Object?> get props => [sectionId, itemId];
}

class LogSectionInteraction extends HomeAnalyticsEvent {
  final String sectionId;
  final String? itemId;
  final String interactionType;
  final Map<String, dynamic>? metadata;
  const LogSectionInteraction({required this.sectionId, this.itemId, required this.interactionType, this.metadata});
  @override
  List<Object?> get props => [sectionId, itemId, interactionType, metadata];
}