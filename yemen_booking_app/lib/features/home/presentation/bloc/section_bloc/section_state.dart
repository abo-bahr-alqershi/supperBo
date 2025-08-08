// lib/features/home/presentation/bloc/section_bloc/section_state.dart

part of 'section_bloc.dart';

abstract class SectionState extends Equatable {
  const SectionState();

  @override
  List<Object?> get props => [];
}

class SectionInitial extends SectionState {
  const SectionInitial();
}

class SectionLoading extends SectionState {
  final String sectionId;

  const SectionLoading({required this.sectionId});

  @override
  List<Object?> get props => [sectionId];
}

class SectionLoaded extends SectionState {
  final HomeSection section;
  final dynamic data;
  final bool isVisible;
  final bool hasTrackedImpression;
  final bool isRefreshing;
  final bool isLoadingMore;
  final bool hasReachedEnd;
  final int currentPage;
  final String? lastError;

  const SectionLoaded({
    required this.section,
    required this.data,
    required this.isVisible,
    required this.hasTrackedImpression,
    this.isRefreshing = false,
    this.isLoadingMore = false,
    this.hasReachedEnd = false,
    this.currentPage = 1,
    this.lastError,
  });

  SectionLoaded copyWith({
    HomeSection? section,
    dynamic data,
    bool? isVisible,
    bool? hasTrackedImpression,
    bool? isRefreshing,
    bool? isLoadingMore,
    bool? hasReachedEnd,
    int? currentPage,
    String? lastError,
  }) {
    return SectionLoaded(
      section: section ?? this.section,
      data: data ?? this.data,
      isVisible: isVisible ?? this.isVisible,
      hasTrackedImpression: hasTrackedImpression ?? this.hasTrackedImpression,
      isRefreshing: isRefreshing ?? this.isRefreshing,
      isLoadingMore: isLoadingMore ?? this.isLoadingMore,
      hasReachedEnd: hasReachedEnd ?? this.hasReachedEnd,
      currentPage: currentPage ?? this.currentPage,
      lastError: lastError ?? this.lastError,
    );
  }

  @override
  List<Object?> get props => [
        section,
        data,
        isVisible,
        hasTrackedImpression,
        isRefreshing,
        isLoadingMore,
        hasReachedEnd,
        currentPage,
        lastError,
      ];
}

class SectionError extends SectionState {
  final String sectionId;
  final String message;

  const SectionError({
    required this.sectionId,
    required this.message,
  });

  @override
  List<Object?> get props => [sectionId, message];
}