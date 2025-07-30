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