import 'package:flutter_bloc/flutter_bloc.dart';
import 'section_event.dart';
import 'section_state.dart';
import '../../../domain/usecases/get_home_sections_usecase.dart';

class SectionBloc extends Bloc<SectionEvent, SectionState> {
  final GetHomeSectionsUseCase getHomeSectionsUseCase;

  SectionBloc({required this.getHomeSectionsUseCase}) : super(SectionInitial()) {
    on<LoadSection>(_onLoadSection);
    on<RefreshSection>(_onRefreshSection);
  }

  Future<void> _onLoadSection(LoadSection event, Emitter<SectionState> emit) async {
    emit(SectionLoading(event.sectionId));
    final result = await getHomeSectionsUseCase(const GetHomeSectionsParams());
    await result.fold(
      (failure) async => emit(SectionError(sectionId: event.sectionId, message: failure.message)),
      (sections) async {
        final section = sections.firstWhere(
          (s) => s.id == event.sectionId,
          orElse: () => throw StateError('Section not found'),
        );
        emit(SectionLoaded(section));
      },
    );
  }

  Future<void> _onRefreshSection(RefreshSection event, Emitter<SectionState> emit) async {
    add(LoadSection(sectionId: event.sectionId));
  }
}