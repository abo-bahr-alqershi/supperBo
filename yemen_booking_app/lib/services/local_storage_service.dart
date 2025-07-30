import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class LocalStorageService {
  static SharedPreferences? _instance;

  static Future<void> init() async {
    _instance = await SharedPreferences.getInstance();
  }

  static SharedPreferences get instance {
    if (_instance == null) {
      throw Exception('LocalStorageService not initialized. Call init() first.');
    }
    return _instance!;
  }

  // Basic operations
  static Future<bool> setString(String key, String value) async {
    return await instance.setString(key, value);
  }

  static String? getString(String key) {
    return instance.getString(key);
  }

  static Future<bool> setBool(String key, bool value) async {
    return await instance.setBool(key, value);
  }

  static bool? getBool(String key) {
    return instance.getBool(key);
  }

  static Future<bool> setInt(String key, int value) async {
    return await instance.setInt(key, value);
  }

  static int? getInt(String key) {
    return instance.getInt(key);
  }

  static Future<bool> setDouble(String key, double value) async {
    return await instance.setDouble(key, value);
  }

  static double? getDouble(String key) {
    return instance.getDouble(key);
  }

  static Future<bool> setStringList(String key, List<String> value) async {
    return await instance.setStringList(key, value);
  }

  static List<String>? getStringList(String key) {
    return instance.getStringList(key);
  }

  // JSON operations
  static Future<bool> setJson(String key, Map<String, dynamic> value) async {
    final jsonString = jsonEncode(value);
    return await setString(key, jsonString);
  }

  static Map<String, dynamic>? getJson(String key) {
    final jsonString = getString(key);
    if (jsonString != null) {
      try {
        return jsonDecode(jsonString) as Map<String, dynamic>;
      } catch (e) {
        return null;
      }
    }
    return null;
  }

  // Cache operations with expiry
  static Future<bool> setCachedData(
    String key, 
    dynamic data, {
    int? expiryInMilliseconds,
  }) async {
    final cacheData = {
      'data': data,
      'timestamp': DateTime.now().millisecondsSinceEpoch,
      'expiry': expiryInMilliseconds,
    };
    return await setJson(key, cacheData);
  }

  static dynamic getCachedData(String key) {
    final cacheData = getJson(key);
    if (cacheData != null) {
      final timestamp = cacheData['timestamp'] as int;
      final expiry = cacheData['expiry'] as int?;
      
      if (expiry != null) {
        final now = DateTime.now().millisecondsSinceEpoch;
        final expiryTime = timestamp + expiry;
        
        if (now > expiryTime) {
          // Data has expired
          remove(key);
          return null;
        }
      }
      
      return cacheData['data'];
    }
    return null;
  }

  // Utility operations
  static Future<bool> remove(String key) async {
    return await instance.remove(key);
  }

  static Future<bool> clear() async {
    return await instance.clear();
  }

  static bool containsKey(String key) {
    return instance.containsKey(key);
  }

  static Set<String> getKeys() {
    return instance.getKeys();
  }

  // Batch operations
  static Future<void> setMultiple(Map<String, dynamic> values) async {
    for (final entry in values.entries) {
      if (entry.value is String) {
        await setString(entry.key, entry.value);
      } else if (entry.value is bool) {
        await setBool(entry.key, entry.value);
      } else if (entry.value is int) {
        await setInt(entry.key, entry.value);
      } else if (entry.value is double) {
        await setDouble(entry.key, entry.value);
      } else if (entry.value is List<String>) {
        await setStringList(entry.key, entry.value);
      } else if (entry.value is Map<String, dynamic>) {
        await setJson(entry.key, entry.value);
      }
    }
  }

  static Map<String, dynamic> getMultiple(List<String> keys) {
    final result = <String, dynamic>{};
    for (final key in keys) {
      if (containsKey(key)) {
        result[key] = instance.get(key);
      }
    }
    return result;
  }
}