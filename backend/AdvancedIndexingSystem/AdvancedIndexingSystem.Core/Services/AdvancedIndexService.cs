using System.Collections.Concurrent;
using System.Text.Json;
using System.Reflection;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Events;

namespace AdvancedIndexingSystem.Core.Services;

/// <summary>
/// خدمة الفهرسة المتقدمة والديناميكية - محسنة للذاكرة
/// Advanced Dynamic Indexing Service - Memory Optimized
/// </summary>
/// <typeparam name="T">نوع البيانات المفهرسة - Type of indexed data</typeparam>
public class AdvancedIndexService<T> : IAdvancedIndex<T> where T : class
{
    #region الخصائص الأساسية - Basic Properties

    /// <summary>
    /// تكوين الفهرس
    /// Index configuration
    /// </summary>
    public IndexConfiguration Configuration { get; private set; }

    /// <summary>
    /// حالة الفهرس الحالية
    /// Current index status
    /// </summary>
    public IndexStatus Status { get; private set; } = IndexStatus.Initializing;

    /// <summary>
    /// عدد العناصر المفهرسة
    /// Count of indexed items
    /// </summary>
    public int ItemCount => _itemCount;

    /// <summary>
    /// حجم الفهرس بالبايت
    /// Index size in bytes
    /// </summary>
    public long IndexSize { get; private set; }

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last update timestamp
    /// </summary>
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    #endregion

    #region الحقول الخاصة - Private Fields

    /// <summary>
    /// عداد العناصر المفهرسة (لتجنب تحميل جميع العناصر للعد)
    /// Indexed items counter (to avoid loading all items for counting)
    /// </summary>
    private volatile int _itemCount = 0;

    /// <summary>
    /// مسارات ملفات الفهارس للوصول المباشر
    /// Index file paths for direct access
    /// </summary>
    private readonly ConcurrentDictionary<string, string> _indexFilePaths = new();

    /// <summary>
    /// كاش محدود للعناصر المستخدمة مؤخراً
    /// Limited cache for recently used items
    /// </summary>
    private readonly ConcurrentDictionary<string, (T Item, DateTime LastAccess)> _itemCache = new();

    /// <summary>
    /// كاش محدود لفهارس الحقول
    /// Limited cache for field indices
    /// </summary>
    private readonly ConcurrentDictionary<string, (Dictionary<object, HashSet<string>> Index, DateTime LastAccess)> _fieldIndexCache = new();

    /// <summary>
    /// إحصائيات الأداء
    /// Performance statistics
    /// </summary>
    private readonly PerformanceMetrics _performanceMetrics = new();

    /// <summary>
    /// قفل للعمليات المتزامنة
    /// Lock for synchronized operations
    /// </summary>
    private readonly object _lockObject = new();

    /// <summary>
    /// مسار حفظ الفهرس
    /// Index save path
    /// </summary>
    private string? _indexFilePath;

    /// <summary>
    /// حد الكاش الأقصى (عدد العناصر)
    /// Maximum cache limit (number of items)
    /// </summary>
    private const int MAX_CACHE_SIZE = 1000;

    /// <summary>
    /// مهلة انتهاء صلاحية الكاش
    /// Cache expiration timeout
    /// </summary>
    private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(10);

    #endregion

    #region الأحداث - Events

    public event EventHandler<IndexStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<IndexItemEventArgs<T>>? ItemAdded;
    public event EventHandler<IndexItemEventArgs<T>>? ItemUpdated;
    public event EventHandler<IndexItemEventArgs<T>>? ItemRemoved;
    public event EventHandler<IndexErrorEventArgs>? ErrorOccurred;

    #endregion

    #region البناء والتهيئة - Construction and Initialization

    /// <summary>
    /// إنشاء خدمة فهرسة جديدة
    /// Create new indexing service
    /// </summary>
    /// <param name="configuration">تكوين الفهرس - Index configuration</param>
    public AdvancedIndexService(IndexConfiguration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        InitializeIndex();
    }

    /// <summary>
    /// تهيئة الفهرس
    /// Initialize index
    /// </summary>
    private void InitializeIndex()
    {
        try
        {
            // إنشاء مجلد الفهرس
            var indexDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "AdvancedIndexing", Configuration.IndexId);
            Directory.CreateDirectory(indexDirectory);

            // تهيئة مسارات ملفات الفهارس
            foreach (var field in Configuration.IndexedFields)
            {
                _indexFilePaths[field] = Path.Combine(indexDirectory, $"{field}_index.json");
            }

            // تهيئة الحقول الديناميكية
            if (Configuration.DynamicFields != null)
            {
                foreach (var dynamicField in Configuration.DynamicFields)
                {
                    _indexFilePaths[dynamicField.FieldName] = Path.Combine(indexDirectory, $"{dynamicField.FieldName}_dynamic_index.json");
                }
            }

            // تحميل عداد العناصر من ملف البيانات الوصفية
            LoadItemCountFromMetadata(indexDirectory);

            Status = IndexStatus.Active;
            OnStatusChanged(IndexStatus.Initializing, IndexStatus.Active);

            _performanceMetrics.IndexCreatedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;

            // بدء مهمة تنظيف الكاش
            _ = Task.Run(async () => await CacheCleanupTaskAsync());
        }
        catch (Exception ex)
        {
            Status = IndexStatus.Error;
            OnErrorOccurred(IndexErrorType.InitializationError, ex.Message, ex);
        }
    }

    #endregion

    #region عمليات إدارة البيانات المحسنة للذاكرة - Memory-Optimized Data Management

    /// <summary>
    /// إضافة عنصر جديد إلى الفهرس بطريقة محسنة للذاكرة
    /// Add new item to index in memory-optimized way
    /// </summary>
    public async Task<bool> AddItemAsync(string id, T item)
    {
        if (string.IsNullOrWhiteSpace(id) || item == null)
        {
            OnErrorOccurred(IndexErrorType.ValidationError, "معرف العنصر أو العنصر نفسه فارغ - Item ID or item is null/empty");
            return false;
        }

        var startTime = DateTime.UtcNow;

        try
        {
            // التحقق من وجود العنصر مسبقاً (دون تحميل جميع العناصر)
            if (await ItemExistsAsync(id))
            {
                OnErrorOccurred(IndexErrorType.DuplicateKey, $"العنصر بالمعرف {id} موجود مسبقاً - Item with ID {id} already exists");
                return false;
            }

            // حفظ العنصر مباشرة إلى الملف
            await SaveItemToFileAsync(id, item);

            // تحديث فهارس الحقول تدريجياً
            await UpdateFieldIndicesIncrementallyAsync(id, item, IndexOperation.Add);

            // إضافة إلى الكاش المحدود
            AddToCache(id, item);

            // تحديث العداد
            Interlocked.Increment(ref _itemCount);

            // تحديث الإحصائيات
            _performanceMetrics.TotalAddOperations++;
            _performanceMetrics.LastOperationTime = DateTime.UtcNow - startTime;
            LastUpdated = DateTime.UtcNow;

            // حفظ البيانات الوصفية
            await SaveMetadataAsync();

            OnItemAdded(id, item);
            return true;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.OperationError, $"خطأ في إضافة العنصر {id}: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// تحديث عنصر موجود في الفهرس بطريقة محسنة للذاكرة
    /// Update existing item in index in memory-optimized way
    /// </summary>
    public async Task<bool> UpdateItemAsync(string id, T item)
    {
        if (string.IsNullOrWhiteSpace(id) || item == null)
        {
            OnErrorOccurred(IndexErrorType.ValidationError, "معرف العنصر أو العنصر نفسه فارغ - Item ID or item is null/empty");
            return false;
        }

        var startTime = DateTime.UtcNow;

        try
        {
            // التحقق من وجود العنصر
            if (!await ItemExistsAsync(id))
            {
                OnErrorOccurred(IndexErrorType.ItemNotFound, $"العنصر بالمعرف {id} غير موجود - Item with ID {id} not found");
                return false;
            }

            // الحصول على العنصر القديم لإزالة فهرسته
            var oldItem = await GetItemAsync(id);
            if (oldItem != null)
            {
                // إزالة الفهرسة القديمة
                await UpdateFieldIndicesIncrementallyAsync(id, oldItem, IndexOperation.Remove);
            }

            // حفظ العنصر المحدث
            await SaveItemToFileAsync(id, item);

            // إعادة فهرسة الحقول
            await UpdateFieldIndicesIncrementallyAsync(id, item, IndexOperation.Add);

            // تحديث الكاش
            UpdateCache(id, item);

            // تحديث الإحصائيات
            _performanceMetrics.TotalUpdateOperations++;
            _performanceMetrics.LastOperationTime = DateTime.UtcNow - startTime;
            LastUpdated = DateTime.UtcNow;

            // حفظ البيانات الوصفية
            await SaveMetadataAsync();

            OnItemUpdated(id, item);
            return true;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.OperationError, $"خطأ في تحديث العنصر {id}: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// حذف عنصر من الفهرس بطريقة محسنة للذاكرة
    /// Remove item from index in memory-optimized way
    /// </summary>
    public async Task<bool> RemoveItemAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            OnErrorOccurred(IndexErrorType.ValidationError, "معرف العنصر فارغ - Item ID is null/empty");
            return false;
        }

        var startTime = DateTime.UtcNow;

        try
        {
            // الحصول على العنصر قبل الحذف
            var item = await GetItemAsync(id);
            if (item == null)
            {
                OnErrorOccurred(IndexErrorType.ItemNotFound, $"العنصر بالمعرف {id} غير موجود - Item with ID {id} not found");
                return false;
            }

            // إزالة من الفهارس
            await UpdateFieldIndicesIncrementallyAsync(id, item, IndexOperation.Remove);

            // حذف الملف
            await DeleteItemFileAsync(id);

            // إزالة من الكاش
            RemoveFromCache(id);

            // تحديث العداد
            Interlocked.Decrement(ref _itemCount);

            // تحديث الإحصائيات
            _performanceMetrics.TotalRemoveOperations++;
            _performanceMetrics.LastOperationTime = DateTime.UtcNow - startTime;
            LastUpdated = DateTime.UtcNow;

            // حفظ البيانات الوصفية
            await SaveMetadataAsync();

            OnItemRemoved(id, item);
            return true;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.OperationError, $"خطأ في حذف العنصر {id}: {ex.Message}", ex);
            return false;
        }
    }

    #endregion

    #region عمليات البحث المحسنة - Optimized Search Operations

    /// <summary>
    /// البحث في الفهرس بطريقة محسنة للذاكرة
    /// Search in index in memory-optimized way
    /// </summary>
    public async Task<SearchResult<T>> SearchAsync(SearchRequest searchRequest)
    {
        if (searchRequest == null)
        {
            return new SearchResult<T>
            {
                Success = false,
                ErrorMessage = "طلب البحث فارغ - Search request is null"
            };
        }

        var startTime = DateTime.UtcNow;
        var result = new SearchResult<T>
        {
            RequestId = searchRequest.RequestId,
            Success = true
        };

        try
        {
            // تطبيق معايير البحث باستخدام streaming
            var matchingIds = await ApplySearchCriteriaStreamingAsync(searchRequest.SearchCriteria);

            // تطبيق التصفح أولاً لتقليل عدد العناصر المحملة
            var paginatedIds = matchingIds.Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                                        .Take(searchRequest.PageSize)
                                        .ToList();

            // تحميل العناصر المطلوبة فقط
            var items = new List<T>();
            foreach (var id in paginatedIds)
            {
                var item = await GetItemAsync(id);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            // تطبيق الترتيب على العناصر المحملة
            if (searchRequest.SortCriteria != null && searchRequest.SortCriteria.Count > 0)
            {
                items = ApplySortingToItems(items, searchRequest.SortCriteria);
            }

            result.Items = items;
            result.Statistics = new SearchStatistics
            {
                TotalCount = matchingIds.Count(),
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize,
                ExecutionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds,
                UsedIndices = GetUsedIndices(searchRequest.SearchCriteria)
            };

            // تحديث إحصائيات الأداء
            _performanceMetrics.TotalSearchOperations++;
            _performanceMetrics.AverageSearchTime = UpdateAverageTime(_performanceMetrics.AverageSearchTime, 
                _performanceMetrics.TotalSearchOperations, result.Statistics.ExecutionTimeMs);

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.SearchError, $"خطأ في البحث: {ex.Message}", ex);
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    #endregion

    #region المساعدات المحسنة للذاكرة - Memory-Optimized Helpers

    /// <summary>
    /// التحقق من وجود العنصر دون تحميله
    /// Check if item exists without loading it
    /// </summary>
    private async Task<bool> ItemExistsAsync(string id)
    {
        // التحقق من الكاش أولاً
        if (_itemCache.ContainsKey(id))
        {
            return true;
        }

        // التحقق من وجود الملف
        var itemFilePath = GetItemFilePath(id);
        return File.Exists(itemFilePath);
    }

    /// <summary>
    /// الحصول على عنصر مع استخدام الكاش
    /// Get item with cache usage
    /// </summary>
    private async Task<T?> GetItemAsync(string id)
    {
        // التحقق من الكاش أولاً
        if (_itemCache.TryGetValue(id, out var cachedItem))
        {
            // تحديث وقت الوصول
            _itemCache[id] = (cachedItem.Item, DateTime.UtcNow);
            return cachedItem.Item;
        }

        // تحميل من الملف
        var itemFilePath = GetItemFilePath(id);
        if (!File.Exists(itemFilePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(itemFilePath);
            var item = JsonSerializer.Deserialize<T>(json);
            
            if (item != null)
            {
                // إضافة إلى الكاش
                AddToCache(id, item);
            }

            return item;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.FileOperationError, $"خطأ في تحميل العنصر {id}: {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// حفظ عنصر إلى ملف
    /// Save item to file
    /// </summary>
    private async Task SaveItemToFileAsync(string id, T item)
    {
        var itemFilePath = GetItemFilePath(id);
        var directory = Path.GetDirectoryName(itemFilePath);
        
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
        {
            WriteIndented = false // توفير المساحة
        });

        await File.WriteAllTextAsync(itemFilePath, json);
    }

    /// <summary>
    /// تحديث فهارس الحقول تدريجياً
    /// Update field indices incrementally
    /// </summary>
    private async Task UpdateFieldIndicesIncrementallyAsync(string id, T item, IndexOperation operation)
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var fieldName = property.Name.ToLowerInvariant();
            
            if (_indexFilePaths.ContainsKey(fieldName))
            {
                var value = property.GetValue(item);
                if (value != null)
                {
                    await UpdateFieldIndexFileAsync(fieldName, value, id, operation);
                }
            }
        }
    }

    /// <summary>
    /// تحديث ملف فهرس حقل محدد
    /// Update specific field index file
    /// </summary>
    private async Task UpdateFieldIndexFileAsync(string fieldName, object value, string id, IndexOperation operation)
    {
        var indexFilePath = _indexFilePaths[fieldName];
        
        // تحميل الفهرس من الكاش أو الملف
        var fieldIndex = await GetFieldIndexAsync(fieldName);

        // تطبيق العملية
        switch (operation)
        {
            case IndexOperation.Add:
                if (!fieldIndex.ContainsKey(value))
                {
                    fieldIndex[value] = new HashSet<string>();
                }
                fieldIndex[value].Add(id);
                break;

            case IndexOperation.Remove:
                if (fieldIndex.ContainsKey(value))
                {
                    fieldIndex[value].Remove(id);
                    if (fieldIndex[value].Count == 0)
                    {
                        fieldIndex.Remove(value);
                    }
                }
                break;
        }

        // حفظ الفهرس المحدث
        await SaveFieldIndexAsync(fieldName, fieldIndex);
    }

    /// <summary>
    /// الحصول على فهرس حقل مع استخدام الكاش
    /// Get field index with cache usage
    /// </summary>
    private async Task<Dictionary<object, HashSet<string>>> GetFieldIndexAsync(string fieldName)
    {
        // التحقق من الكاش أولاً
        if (_fieldIndexCache.TryGetValue(fieldName, out var cachedIndex))
        {
            // تحديث وقت الوصول
            _fieldIndexCache[fieldName] = (cachedIndex.Index, DateTime.UtcNow);
            return cachedIndex.Index;
        }

        // تحميل من الملف
        var indexFilePath = _indexFilePaths[fieldName];
        var fieldIndex = new Dictionary<object, HashSet<string>>();

        if (File.Exists(indexFilePath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(indexFilePath);
                var deserializedIndex = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                
                if (deserializedIndex != null)
                {
                    foreach (var kvp in deserializedIndex)
                    {
                        fieldIndex[kvp.Key] = new HashSet<string>(kvp.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(IndexErrorType.FileOperationError, $"خطأ في تحميل فهرس الحقل {fieldName}: {ex.Message}", ex);
            }
        }

        // إضافة إلى الكاش
        AddFieldIndexToCache(fieldName, fieldIndex);
        return fieldIndex;
    }

    /// <summary>
    /// حفظ فهرس حقل
    /// Save field index
    /// </summary>
    private async Task SaveFieldIndexAsync(string fieldName, Dictionary<object, HashSet<string>> fieldIndex)
    {
        var indexFilePath = _indexFilePaths[fieldName];
        var directory = Path.GetDirectoryName(indexFilePath);
        
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // تحويل إلى تنسيق قابل للتسلسل
        var serializableIndex = fieldIndex.ToDictionary(
            kvp => kvp.Key.ToString() ?? "",
            kvp => kvp.Value.ToList()
        );

        var json = JsonSerializer.Serialize(serializableIndex, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        await File.WriteAllTextAsync(indexFilePath, json);

        // تحديث الكاش
        UpdateFieldIndexCache(fieldName, fieldIndex);
    }

    /// <summary>
    /// تطبيق معايير البحث باستخدام streaming
    /// Apply search criteria using streaming
    /// </summary>
    private async Task<IEnumerable<string>> ApplySearchCriteriaStreamingAsync(List<SearchCriterion> criteria)
    {
        if (criteria == null || criteria.Count == 0)
        {
            return await GetAllItemIdsAsync();
        }

        IEnumerable<string>? result = null;

        foreach (var criterion in criteria)
        {
            var matchingIds = await GetMatchingIdsStreamingAsync(criterion);
            
            if (result == null)
            {
                result = matchingIds;
            }
            else
            {
                result = result.Intersect(matchingIds);
            }
        }

        return result ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// الحصول على المعرفات المطابقة باستخدام streaming
    /// Get matching IDs using streaming
    /// </summary>
    private async Task<IEnumerable<string>> GetMatchingIdsStreamingAsync(SearchCriterion criterion)
    {
        var fieldName = criterion.FieldName.ToLowerInvariant();

        if (!_indexFilePaths.ContainsKey(fieldName))
        {
            return Enumerable.Empty<string>();
        }

        var fieldIndex = await GetFieldIndexAsync(fieldName);
        var result = new HashSet<string>();

        switch (criterion.CriterionType)
        {
            case SearchCriterionType.ExactMatch:
                if (fieldIndex.ContainsKey(criterion.Value))
                {
                    result.UnionWith(fieldIndex[criterion.Value]);
                }
                break;

            case SearchCriterionType.Contains:
                foreach (var kvp in fieldIndex)
                {
                    if (kvp.Key.ToString()?.Contains(criterion.Value?.ToString() ?? "") == true)
                    {
                        result.UnionWith(kvp.Value);
                    }
                }
                break;

            case SearchCriterionType.InRange:
                if (criterion.MinValue != null && criterion.MaxValue != null)
                {
                    foreach (var kvp in fieldIndex)
                    {
                        if (IsInRange(kvp.Key, criterion.MinValue, criterion.MaxValue))
                        {
                            result.UnionWith(kvp.Value);
                        }
                    }
                }
                break;

            case SearchCriterionType.InList:
                if (criterion.Values != null)
                {
                    foreach (var value in criterion.Values)
                    {
                        if (fieldIndex.ContainsKey(value))
                        {
                            result.UnionWith(fieldIndex[value]);
                        }
                    }
                }
                break;
        }

        return result;
    }

    #endregion

    #region إدارة الكاش - Cache Management

    /// <summary>
    /// إضافة عنصر إلى الكاش
    /// Add item to cache
    /// </summary>
    private void AddToCache(string id, T item)
    {
        if (_itemCache.Count >= MAX_CACHE_SIZE)
        {
            CleanupCache();
        }

        _itemCache[id] = (item, DateTime.UtcNow);
    }

    /// <summary>
    /// تحديث عنصر في الكاش
    /// Update item in cache
    /// </summary>
    private void UpdateCache(string id, T item)
    {
        _itemCache[id] = (item, DateTime.UtcNow);
    }

    /// <summary>
    /// إزالة عنصر من الكاش
    /// Remove item from cache
    /// </summary>
    private void RemoveFromCache(string id)
    {
        _itemCache.TryRemove(id, out _);
    }

    /// <summary>
    /// إضافة فهرس حقل إلى الكاش
    /// Add field index to cache
    /// </summary>
    private void AddFieldIndexToCache(string fieldName, Dictionary<object, HashSet<string>> fieldIndex)
    {
        if (_fieldIndexCache.Count >= MAX_CACHE_SIZE)
        {
            CleanupFieldIndexCache();
        }

        _fieldIndexCache[fieldName] = (fieldIndex, DateTime.UtcNow);
    }

    /// <summary>
    /// تحديث فهرس حقل في الكاش
    /// Update field index in cache
    /// </summary>
    private void UpdateFieldIndexCache(string fieldName, Dictionary<object, HashSet<string>> fieldIndex)
    {
        _fieldIndexCache[fieldName] = (fieldIndex, DateTime.UtcNow);
    }

    /// <summary>
    /// تنظيف الكاش
    /// Cleanup cache
    /// </summary>
    private void CleanupCache()
    {
        var expiredItems = _itemCache
            .Where(kvp => DateTime.UtcNow - kvp.Value.LastAccess > _cacheTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredItems)
        {
            _itemCache.TryRemove(key, out _);
        }

        // إذا لم يكن هناك عناصر منتهية الصلاحية، احذف الأقدم
        if (expiredItems.Count == 0 && _itemCache.Count >= MAX_CACHE_SIZE)
        {
            var oldestKey = _itemCache
                .OrderBy(kvp => kvp.Value.LastAccess)
                .First().Key;
            
            _itemCache.TryRemove(oldestKey, out _);
        }
    }

    /// <summary>
    /// تنظيف كاش فهارس الحقول
    /// Cleanup field index cache
    /// </summary>
    private void CleanupFieldIndexCache()
    {
        var expiredIndices = _fieldIndexCache
            .Where(kvp => DateTime.UtcNow - kvp.Value.LastAccess > _cacheTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredIndices)
        {
            _fieldIndexCache.TryRemove(key, out _);
        }

        // إذا لم يكن هناك فهارس منتهية الصلاحية، احذف الأقدم
        if (expiredIndices.Count == 0 && _fieldIndexCache.Count >= MAX_CACHE_SIZE)
        {
            var oldestKey = _fieldIndexCache
                .OrderBy(kvp => kvp.Value.LastAccess)
                .First().Key;
            
            _fieldIndexCache.TryRemove(oldestKey, out _);
        }
    }

    /// <summary>
    /// مهمة تنظيف الكاش الدورية
    /// Periodic cache cleanup task
    /// </summary>
    private async Task CacheCleanupTaskAsync()
    {
        while (Status != IndexStatus.Disposed)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                CleanupCache();
                CleanupFieldIndexCache();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(IndexErrorType.OperationError, $"خطأ في تنظيف الكاش: {ex.Message}", ex);
            }
        }
    }

    #endregion

    #region مساعدات إضافية - Additional Helpers

    /// <summary>
    /// الحصول على مسار ملف العنصر
    /// Get item file path
    /// </summary>
    private string GetItemFilePath(string id)
    {
        var indexDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "AdvancedIndexing", Configuration.IndexId, "items");
        
        return Path.Combine(indexDirectory, $"{id}.json");
    }

    /// <summary>
    /// حذف ملف العنصر
    /// Delete item file
    /// </summary>
    private async Task DeleteItemFileAsync(string id)
    {
        var itemFilePath = GetItemFilePath(id);
        if (File.Exists(itemFilePath))
        {
            File.Delete(itemFilePath);
        }
        await Task.CompletedTask;
    }

    /// <summary>
    /// الحصول على جميع معرفات العناصر
    /// Get all item IDs
    /// </summary>
    private async Task<IEnumerable<string>> GetAllItemIdsAsync()
    {
        var itemsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "AdvancedIndexing", Configuration.IndexId, "items");

        if (!Directory.Exists(itemsDirectory))
        {
            return Enumerable.Empty<string>();
        }

        var files = Directory.GetFiles(itemsDirectory, "*.json");
        return files.Select(f => Path.GetFileNameWithoutExtension(f));
    }

    /// <summary>
    /// تحميل عداد العناصر من البيانات الوصفية
    /// Load item count from metadata
    /// </summary>
    private void LoadItemCountFromMetadata(string indexDirectory)
    {
        var metadataPath = Path.Combine(indexDirectory, "metadata.json");
        if (File.Exists(metadataPath))
        {
            try
            {
                var json = File.ReadAllText(metadataPath);
                var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (metadata != null && metadata.ContainsKey("ItemCount"))
                {
                    if (int.TryParse(metadata["ItemCount"].ToString(), out var count))
                    {
                        _itemCount = count;
                    }
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(IndexErrorType.FileOperationError, $"خطأ في تحميل البيانات الوصفية: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// حفظ البيانات الوصفية
    /// Save metadata
    /// </summary>
    private async Task SaveMetadataAsync()
    {
        var indexDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "AdvancedIndexing", Configuration.IndexId);

        var metadataPath = Path.Combine(indexDirectory, "metadata.json");
        var metadata = new Dictionary<string, object>
        {
            ["ItemCount"] = _itemCount,
            ["LastUpdated"] = LastUpdated,
            ["IndexSize"] = IndexSize,
            ["Configuration"] = Configuration
        };

        var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(metadataPath, json);
    }

    /// <summary>
    /// تطبيق الترتيب على العناصر
    /// Apply sorting to items
    /// </summary>
    private List<T> ApplySortingToItems(List<T> items, List<SortCriterion> sortCriteria)
    {
        if (sortCriteria == null || sortCriteria.Count == 0)
        {
            return items;
        }

        // تطبيق الترتيب حسب المعايير
        return items.OrderBy(item => GetSortValue(item, sortCriteria[0])).ToList();
    }

    /// <summary>
    /// الحصول على الفهارس المستخدمة
    /// Get used indices
    /// </summary>
    private List<string> GetUsedIndices(List<SearchCriterion> criteria)
    {
        return criteria.Select(c => $"{Configuration.IndexId}-{c.FieldName}").ToList();
    }

    /// <summary>
    /// تحديث متوسط الوقت
    /// Update average time
    /// </summary>
    private double UpdateAverageTime(double currentAverage, long operationCount, double newTime)
    {
        return ((currentAverage * (operationCount - 1)) + newTime) / operationCount;
    }

    /// <summary>
    /// التحقق من وجود القيمة في النطاق
    /// Check if value is in range
    /// </summary>
    private bool IsInRange(object value, object minValue, object maxValue)
    {
        try
        {
            if (value is IComparable comparable && minValue is IComparable min && maxValue is IComparable max)
            {
                return comparable.CompareTo(min) >= 0 && comparable.CompareTo(max) <= 0;
            }
        }
        catch
        {
            // تجاهل الأخطاء في المقارنة
        }
        return false;
    }

    /// <summary>
    /// الحصول على قيمة الترتيب
    /// Get sort value
    /// </summary>
    private object? GetSortValue(T item, SortCriterion sortCriterion)
    {
        var property = typeof(T).GetProperty(sortCriterion.FieldName);
        return property?.GetValue(item);
    }

    #endregion

    #region عمليات الحفظ والتحميل المحسنة - Optimized Save and Load Operations

    /// <summary>
    /// حفظ الفهرس إلى ملف
    /// Save index to file
    /// </summary>
    public async Task<bool> SaveToFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            OnErrorOccurred(IndexErrorType.ValidationError, "مسار الملف فارغ - File path is null/empty");
            return false;
        }

        try
        {
            // حفظ البيانات الوصفية فقط (ليس العناصر الفعلية)
            var indexData = new
            {
                Configuration,
                Metadata = new
                {
                    CreatedAt = _performanceMetrics.IndexCreatedAt,
                    LastUpdated,
                    ItemCount,
                    IndexSize,
                    Version = "2.0.0-MemoryOptimized"
                },
                Statistics = _performanceMetrics,
                IndexFilePaths = _indexFilePaths.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            var json = JsonSerializer.Serialize(indexData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await File.WriteAllTextAsync(filePath, json);
            _indexFilePath = filePath;

            _performanceMetrics.TotalSaveOperations++;
            return true;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.FileOperationError, $"خطأ في حفظ الملف: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// تحميل الفهرس من ملف
    /// Load index from file
    /// </summary>
    public async Task<bool> LoadFromFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            OnErrorOccurred(IndexErrorType.ValidationError, "مسار الملف غير صحيح أو الملف غير موجود - Invalid file path or file not found");
            return false;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var indexData = JsonSerializer.Deserialize<JsonElement>(json);

            // تحميل البيانات الوصفية فقط
            if (indexData.TryGetProperty("metadata", out var metadataElement))
            {
                if (metadataElement.TryGetProperty("itemCount", out var itemCountElement))
                {
                    _itemCount = itemCountElement.GetInt32();
                }
            }

            // تحميل مسارات الفهارس
            if (indexData.TryGetProperty("indexFilePaths", out var pathsElement))
            {
                foreach (var pathProperty in pathsElement.EnumerateObject())
                {
                    _indexFilePaths[pathProperty.Name] = pathProperty.Value.GetString() ?? "";
                }
            }

            _indexFilePath = filePath;
            _performanceMetrics.TotalLoadOperations++;
            
            return true;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.FileOperationError, $"خطأ في تحميل الملف: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// إعادة بناء الفهرس
    /// Rebuild index
    /// </summary>
    public async Task<bool> RebuildIndexAsync()
    {
        try
        {
            Status = IndexStatus.Rebuilding;
            OnStatusChanged(IndexStatus.Active, IndexStatus.Rebuilding);

            // مسح الكاش
            _itemCache.Clear();
            _fieldIndexCache.Clear();

            // إعادة بناء فهارس الحقول من ملفات العناصر
            var allItemIds = await GetAllItemIdsAsync();
            
            // مسح ملفات الفهارس الموجودة
            foreach (var indexFilePath in _indexFilePaths.Values)
            {
                if (File.Exists(indexFilePath))
                {
                    File.Delete(indexFilePath);
                }
            }

            // إعادة فهرسة جميع العناصر
            foreach (var itemId in allItemIds)
            {
                var item = await GetItemAsync(itemId);
                if (item != null)
                {
                    await UpdateFieldIndicesIncrementallyAsync(itemId, item, IndexOperation.Add);
                }
            }

            Status = IndexStatus.Active;
            OnStatusChanged(IndexStatus.Rebuilding, IndexStatus.Active);
            
            _performanceMetrics.TotalRebuildOperations++;
            LastUpdated = DateTime.UtcNow;

            await SaveMetadataAsync();
            return true;
        }
        catch (Exception ex)
        {
            Status = IndexStatus.Error;
            OnErrorOccurred(IndexErrorType.RebuildError, $"خطأ في إعادة بناء الفهرس: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// مسح جميع العناصر من الفهرس
    /// Clear all items from index
    /// </summary>
    public async Task<bool> ClearAsync()
    {
        try
        {
            // مسح الكاش
            _itemCache.Clear();
            _fieldIndexCache.Clear();

            // حذف جميع ملفات العناصر
            var itemsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "AdvancedIndexing", Configuration.IndexId, "items");

            if (Directory.Exists(itemsDirectory))
            {
                Directory.Delete(itemsDirectory, true);
            }

            // حذف ملفات الفهارس
            foreach (var indexFilePath in _indexFilePaths.Values)
            {
                if (File.Exists(indexFilePath))
                {
                    File.Delete(indexFilePath);
                }
            }

            // إعادة تعيين العداد
            _itemCount = 0;

            _performanceMetrics.TotalClearOperations++;
            LastUpdated = DateTime.UtcNow;

            await SaveMetadataAsync();
            OnStatusChanged(Status, IndexStatus.Active);
            return true;
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.OperationError, $"خطأ في مسح الفهرس: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// الحصول على إحصائيات الفهرس
    /// Get index statistics
    /// </summary>
    public PerformanceMetrics GetStatistics()
    {
        _performanceMetrics.CurrentItemCount = ItemCount;
        _performanceMetrics.IndexSizeBytes = IndexSize;
        _performanceMetrics.LastUpdated = LastUpdated;
        return _performanceMetrics;
    }

    #endregion

    #region إطلاق الأحداث - Event Triggers

    private void OnStatusChanged(IndexStatus oldStatus, IndexStatus newStatus)
    {
        StatusChanged?.Invoke(this, new IndexStatusChangedEventArgs(
            Configuration.IndexId,
            Configuration.IndexName ?? Configuration.IndexId,
            oldStatus,
            newStatus,
            "Status change triggered"
        ));
    }

    private void OnItemAdded(string id, T item)
    {
        ItemAdded?.Invoke(this, new IndexItemEventArgs<T>(
            Configuration.IndexId,
            Configuration.IndexName ?? Configuration.IndexId,
            id,
            item,
            UpdateOperationType.Add
        ));
    }

    private void OnItemUpdated(string id, T item)
    {
        ItemUpdated?.Invoke(this, new IndexItemEventArgs<T>(
            Configuration.IndexId,
            Configuration.IndexName ?? Configuration.IndexId,
            id,
            item,
            UpdateOperationType.Update
        ));
    }

    private void OnItemRemoved(string id, T item)
    {
        ItemRemoved?.Invoke(this, new IndexItemEventArgs<T>(
            Configuration.IndexId,
            Configuration.IndexName ?? Configuration.IndexId,
            id,
            item,
            UpdateOperationType.Remove
        ));
    }

    private void OnErrorOccurred(IndexErrorType errorType, string message, Exception? exception = null)
    {
        ErrorOccurred?.Invoke(this, new IndexErrorEventArgs(
            Configuration.IndexId,
            Configuration.IndexName ?? Configuration.IndexId,
            message,
            errorType,
            ErrorSeverity.Error
        )
        {
            Exception = exception
        });
    }

    #endregion

    #region التخلص من الموارد - Dispose

    /// <summary>
    /// تحرير الموارد
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        try
        {
            Status = IndexStatus.Disposed;

            if (!string.IsNullOrEmpty(_indexFilePath))
            {
                // حفظ تلقائي عند التخلص
                _ = SaveToFileAsync(_indexFilePath);
            }

            // حفظ البيانات الوصفية
            _ = SaveMetadataAsync();

            _itemCache.Clear();
            _fieldIndexCache.Clear();
        }
        catch (Exception ex)
        {
            OnErrorOccurred(IndexErrorType.DisposeError, $"خطأ في تحرير الموارد: {ex.Message}", ex);
        }
    }

    #endregion
}

/// <summary>
/// عملية الفهرسة
/// Index operation
/// </summary>
public enum IndexOperation
{
    Add,
    Remove,
    Update
}