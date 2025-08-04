import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/usecases/get_home_config_usecase.dart';
import '../../domain/usecases/get_home_sections_usecase.dart';
import '../../domain/usecases/get_sponsored_ads_usecase.dart';
import '../../domain/usecases/get_city_destinations_usecase.dart';
import '../../domain/usecases/record_ad_impression_usecase.dart';
import '../../domain/usecases/record_ad_click_usecase.dart';
import 'home_event.dart';
import 'home_state.dart';

class HomeBloc extends Bloc<HomeEvent, HomeState> {
  final GetHomeConfigUseCase getHomeConfigUseCase;
  final GetHomeSectionsUseCase getHomeSectionsUseCase;
  final GetSponsoredAdsUseCase getSponsoredAdsUseCase;
  final GetCityDestinationsUseCase getCityDestinationsUseCase;
  final RecordAdImpressionUseCase recordAdImpressionUseCase;
  final RecordAdClickUseCase recordAdClickUseCase;

  HomeBloc({
    required this.getHomeConfigUseCase,
    required this.getHomeSectionsUseCase,
    required this.getSponsoredAdsUseCase,
    required this.getCityDestinationsUseCase,
    required this.recordAdImpressionUseCase,
    required this.recordAdClickUseCase,
  }) : super(HomeInitial()) {
    on<LoadHomeData>(_onLoadHomeData);
    on<RefreshHome>(_onRefreshHome);
  }

  Future<void> _onLoadHomeData(LoadHomeData event, Emitter<HomeState> emit) async {
    emit(HomeLoading());
    final configResult = await getHomeConfigUseCase(const GetHomeConfigParams());
    await configResult.fold(
      (failure) async => emit(HomeError(message: failure.message)),
      (config) async {
        final sectionsResult = await getHomeSectionsUseCase(const GetHomeSectionsParams());
        await sectionsResult.fold(
          (failure) async => emit(HomeError(message: failure.message)),
          (sections) async {
            emit(HomeLoaded(config: config, sections: sections));
          },
        );
      },
    );
  }

  Future<void> _onRefreshHome(RefreshHome event, Emitter<HomeState> emit) async {
    add(const LoadHomeData(forceRefresh: true));
  }
}
