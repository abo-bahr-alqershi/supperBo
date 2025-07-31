import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import 'package:yemen_booking_app/core/usecases/usecase.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/models/paginated_result.dart';
import '../entities/search_result.dart';
import '../repositories/search_repository.dart';

class SearchPropertiesUseCase implements UseCase<PaginatedResult<SearchResult>, SearchPropertiesParams> {
  final SearchRepository repository;

  SearchPropertiesUseCase(this.repository);

  @override
  Future<Either<Failure, PaginatedResult<SearchResult>>> call(SearchPropertiesParams params) async {
    return await repository.searchProperties(
      searchQuery: params.searchQuery,
      city: params.city,
      propertyType: params.propertyType,
      minPrice: params.minPrice,
      maxPrice: params.maxPrice,
      minRating: params.minRating,
      amenities: params.amenities,
      checkInDate: params.checkInDate,
      checkOutDate: params.checkOutDate,
      guests: params.guests,
      latitude: params.latitude,
      longitude: params.longitude,
      radiusKm: params.radiusKm,
      sortBy: params.sortBy,
      pageNumber: params.pageNumber,
      pageSize: params.pageSize,
    );
  }
}

class SearchPropertiesParams extends Equatable {
  final String? searchQuery;
  final String? city;
  final String? propertyType;
  final double? minPrice;
  final double? maxPrice;
  final int? minRating;
  final List<String>? amenities;
  final DateTime? checkInDate;
  final DateTime? checkOutDate;
  final int? guests;
  final double? latitude;
  final double? longitude;
  final double? radiusKm;
  final String? sortBy;
  final int pageNumber;
  final int pageSize;

  const SearchPropertiesParams({
    this.searchQuery,
    this.city,
    this.propertyType,
    this.minPrice,
    this.maxPrice,
    this.minRating,
    this.amenities,
    this.checkInDate,
    this.checkOutDate,
    this.guests,
    this.latitude,
    this.longitude,
    this.radiusKm,
    this.sortBy,
    this.pageNumber = 1,
    this.pageSize = 20,
  });

  @override
  List<Object?> get props => [
        searchQuery,
        city,
        propertyType,
        minPrice,
        maxPrice,
        minRating,
        amenities,
        checkInDate,
        checkOutDate,
        guests,
        latitude,
        longitude,
        radiusKm,
        sortBy,
        pageNumber,
        pageSize,
      ];
}