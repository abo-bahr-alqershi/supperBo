import '../../../../core/network/api_client.dart';
import '../../../../core/error/exceptions.dart';
import '../models/home_config_model.dart';
import '../models/home_section_model.dart';
import '../models/city_destination_model.dart';
import '../models/sponsored_ad_model.dart';
import '../models/section_data_model.dart';

abstract class HomeRemoteDataSource {
  Future<HomeConfigModel> getHomeConfig({String? version});
  Future<List<HomeSectionModel>> getHomeSections({String? userId});
  Future<List<SponsoredAdModel>> getSponsoredAds();
  Future<List<CityDestinationModel>> getCityDestinations();
  Future<void> recordAdImpression({required String adId, String? additionalData});
  Future<void> recordAdClick({required String adId, String? additionalData});
  Future<SectionDataModel?> getSectionData({required String sectionId});
  Future<void> recordSectionImpression({required String sectionId});
  Future<void> recordSectionInteraction({required String sectionId, required String interactionType, String? itemId, Map<String, dynamic>? metadata});
}

class HomeRemoteDataSourceImpl implements HomeRemoteDataSource {
  final ApiClient apiClient;

  HomeRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<HomeConfigModel> getHomeConfig({String? version}) async {
    try {
      final response = await apiClient.get('/api/client/home-sections/config', queryParameters: {'version': version});
      return HomeConfigModel.fromJson(response.data);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  @override
  Future<List<HomeSectionModel>> getHomeSections({String? userId}) async {
    try {
      final response = await apiClient.get('/api/client/home-sections/sections', queryParameters: {
        'userId': userId,
        'includeContent': true,
        'onlyActive': true,
        'language': 'ar',
      });
      final data = response.data as List;
      return data.map((item) => HomeSectionModel.fromJson(item)).toList();
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  @override
  Future<List<SponsoredAdModel>> getSponsoredAds() async {
    try {
      final response = await apiClient.get('/api/client/home-sections/sponsored-ads');
      final data = response.data as List;
      return data.map((item) => SponsoredAdModel.fromJson(item)).toList();
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  @override
  Future<List<CityDestinationModel>> getCityDestinations() async {
    try {
      final response = await apiClient.get('/api/client/home-sections/destinations');
      final data = response.data as List;
      return data.map((item) => CityDestinationModel.fromJson(item)).toList();
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> recordAdImpression({required String adId, String? additionalData}) async {
    try {
      await apiClient.post('/api/client/home-sections/sponsored-ads/$adId/impression', data: {'additionalData': additionalData});
    } catch (e) {
      // Fail silently
      print('Failed to record ad impression: $e');
    }
  }

  @override
  Future<void> recordAdClick({required String adId, String? additionalData}) async {
    try {
      await apiClient.post('/api/client/home-sections/sponsored-ads/$adId/click', data: {'additionalData': additionalData});
    } catch (e) {
      // Fail silently
      print('Failed to record ad click: $e');
    }
  }

  @override
  Future<SectionDataModel?> getSectionData({required String sectionId}) async {
    try {
      final response = await apiClient.get('/api/client/home-sections/sections/$sectionId/data');
      if (response.data == null) return null;
      return SectionDataModel.fromJson(response.data as Map<String, dynamic>);
    } catch (e) {
      throw ServerException(e.toString());
    }
  }

  @override
  Future<void> recordSectionImpression({required String sectionId}) async {
    try {
      await apiClient.post('/api/client/home-sections/sections/$sectionId/impression');
    } catch (e) {
      // Silent failure for analytics
    }
  }

  @override
  Future<void> recordSectionInteraction({required String sectionId, required String interactionType, String? itemId, Map<String, dynamic>? metadata}) async {
    try {
      await apiClient.post('/api/client/home-sections/sections/$sectionId/interaction', data: {
        'interactionType': interactionType,
        'itemId': itemId,
        'metadata': metadata,
      });
    } catch (e) {
      // Silent failure for analytics
    }
  }
}
