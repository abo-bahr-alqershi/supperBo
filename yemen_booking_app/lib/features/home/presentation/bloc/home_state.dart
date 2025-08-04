import 'package:equatable/equatable.dart';
import '../../domain/entities/home_config.dart';
import '../../domain/entities/home_section.dart';
import '../../../../core/models/paginated_result.dart';
import '../../domain/entities/featured_property.dart';
import '../../domain/entities/city_destination.dart';

abstract class HomeState extends Equatable {
  const HomeState();

  @override
  List<Object?> get props => [];
}

class HomeInitial extends HomeState {}

class HomeLoading extends HomeState {}

class HomeLoaded extends HomeState {
  final HomeConfig config;
  final List<HomeSection> sections;
  final PaginatedResult<FeaturedProperty>? featuredProperties;
  final PaginatedResult<FeaturedProperty>? nearbyProperties;
  final List<CityDestination>? popularDestinations;

  const HomeLoaded({
    required this.config,
    required this.sections,
    this.featuredProperties,
    this.nearbyProperties,
    this.popularDestinations,
  });

  @override
  List<Object?> get props => [config, sections, featuredProperties, nearbyProperties, popularDestinations];
}

class HomeError extends HomeState {
  final String message;

  const HomeError({required this.message});

  @override
  List<Object> get props => [message];
}
