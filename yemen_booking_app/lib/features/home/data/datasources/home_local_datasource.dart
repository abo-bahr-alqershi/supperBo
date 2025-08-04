import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../../core/error/exceptions.dart';
import '../models/home_config_model.dart';
import '../models/home_section_model.dart';

abstract class HomeLocalDataSource {
  Future<void> cacheHomeConfig(HomeConfigModel config);
  Future<HomeConfigModel?> getCachedHomeConfig();
  Future<void> cacheHomeSections(List<HomeSectionModel> sections);
  Future<List<HomeSectionModel>?> getCachedHomeSections();
  Future<void> clearHomeCache();
}

class HomeLocalDataSourceImpl implements HomeLocalDataSource {
  final SharedPreferences sharedPreferences;

  HomeLocalDataSourceImpl({required this.sharedPreferences});

  static const String _homeConfigKey = 'HOME_CONFIG';
  static const String _homeSectionsKey = 'HOME_SECTIONS';

  @override
  Future<void> cacheHomeConfig(HomeConfigModel config) async {
    try {
      final jsonString = json.encode(config.toJson());
      await sharedPreferences.setString(_homeConfigKey, jsonString);
    } catch (e) {
      throw CacheException('فشل حفظ إعدادات الصفحة الرئيسية');
    }
  }

  @override
  Future<HomeConfigModel?> getCachedHomeConfig() async {
    final jsonString = sharedPreferences.getString(_homeConfigKey);
    if (jsonString != null) {
      try {
        return HomeConfigModel.fromJson(json.decode(jsonString) as Map<String, dynamic>);
      } catch (e) {
        throw CacheException('فشل قراءة إعدادات الصفحة الرئيسية المحفوظة');
      }
    }
    return null;
  }

  @override
  Future<void> cacheHomeSections(List<HomeSectionModel> sections) async {
    try {
      final jsonString = json.encode(sections.map((s) => s.toJson()).toList());
      await sharedPreferences.setString(_homeSectionsKey, jsonString);
    } catch (e) {
      throw CacheException('فشل حفظ أقسام الصفحة الرئيسية');
    }
  }

  @override
  Future<List<HomeSectionModel>?> getCachedHomeSections() async {
    final jsonString = sharedPreferences.getString(_homeSectionsKey);
    if (jsonString != null) {
      try {
        final List<dynamic> jsonList = json.decode(jsonString);
        return jsonList.map((jsonItem) => HomeSectionModel.fromJson(jsonItem as Map<String, dynamic>)).toList();
      } catch (e) {
        throw CacheException('فشل قراءة أقسام الصفحة الرئيسية المحفوظة');
      }
    }
    return null;
  }

  @override
  Future<void> clearHomeCache() async {
    try {
      await sharedPreferences.remove(_homeConfigKey);
      await sharedPreferences.remove(_homeSectionsKey);
    } catch (e) {
      throw CacheException('فشل مسح بيانات الصفحة الرئيسية المحفوظة');
    }
  }
}
