import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../../data/models/section_data_model.dart';
import '../repositories/home_repository.dart';

class GetSectionDataUseCase implements UseCase<SectionDataModel?, GetSectionDataParams> {
  final HomeRepository repository;

  GetSectionDataUseCase(this.repository);

  @override
  Future<Either<Failure, SectionDataModel?>> call(GetSectionDataParams params) async {
    return await repository.getSectionData(sectionId: params.sectionId);
  }
}

class GetSectionDataParams extends Equatable {
  final String sectionId;

  const GetSectionDataParams({required this.sectionId});

  @override
  List<Object?> get props => [sectionId];
}