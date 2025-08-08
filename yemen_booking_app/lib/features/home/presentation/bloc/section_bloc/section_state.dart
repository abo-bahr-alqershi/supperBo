import 'package:equatable/equatable.dart';
import '../../../domain/entities/home_section.dart';

abstract class SectionState extends Equatable {
  const SectionState();
  @override
  List<Object?> get props => [];
}

class SectionInitial extends SectionState {}

class SectionLoading extends SectionState {
  final String sectionId;
  const SectionLoading(this.sectionId);
  @override
  List<Object> get props => [sectionId];
}

class SectionLoaded extends SectionState {
  final HomeSection section;
  const SectionLoaded(this.section);
  @override
  List<Object> get props => [section];
}

class SectionError extends SectionState {
  final String sectionId;
  final String message;
  const SectionError({required this.sectionId, required this.message});
  @override
  List<Object> get props => [sectionId, message];
}