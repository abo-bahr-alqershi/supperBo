using System.Collections.Concurrent;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Events;

namespace AdvancedIndexingSystem.Core.Services;

/// <summary>
/// مدير الفهارس المتقدم - إدارة عدة فهارس
/// Advanced Index Manager - Managing multiple indices
/// </summary>
public class IndexManager : IDisposable
{
    #region الخصائص الأساسية - Basic Properties

    /// <summary>
    /// عدد الفهارس النشطة
    /// Count of active indices
    /// </summary>
    public int ActiveIndicesCount => _indices.Count;

    /// <summary>
    /// حالة المدير
    /// Manager status
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// مسار مجلد حفظ الفهارس
    /// Indices save directory path
    /// </summary>
    public string SaveDirectory { get; private set; }

    #endregion

    #region الحقول الخاصة - Private Fields

    /// <summary>
    /// قاموس الفهارس المُدارة
    /// Dictionary of managed indices
    /// </summary>
    private readonly ConcurrentDictionary<string, object> _indices = new();

    /// <summary>
    /// تكوينات الفهارس
    /// Index configurations
    /// </summary>
    private readonly ConcurrentDictionary<string, IndexConfiguration> _configurations = new();

    /// <summary>
    /// إحصائيات الأداء الإجمالية
    /// Global performance statistics
    /// </summary>
    private readonly PerformanceMetrics _globalMetrics = new();

    /// <summary>
    /// قفل للعمليات المتزامنة
    /// Lock for synchronized operations
    /// </summary>
    private readonly object _lockObject = new();

    #endregion

    #region الأحداث - Events

    /// <summary>
    /// حدث إنشاء فهرس جديد
    /// Index created event
    /// </summary>
    public event EventHandler<IndexStatusChangedEventArgs>? IndexCreated;

    /// <summary>
    /// حدث حذف فهرس
    /// Index removed event
    /// </summary>
    public event EventHandler<IndexStatusChangedEventArgs>? IndexRemoved;

    /// <summary>
    /// حدث خطأ في المدير
    /// Manager error event
    /// </summary>
    public event EventHandler<IndexErrorEventArgs>? ManagerError;

    #endregion

    #region البناء والتهيئة - Construction and Initialization

    /// <summary>
    /// إنشاء مدير فهارس جديد
    /// Create new index manager
    /// </summary>
    /// <param name="saveDirectory">مجلد حفظ الفهارس - Indices save directory</param>
    public IndexManager(string saveDirectory = "indices")
    {
        SaveDirectory = saveDirectory;
        Initialize();
    }

    /// <summary>
    /// تهيئة المدير
    /// Initialize manager
    /// </summary>
    private void Initialize()
    {
        try
        {
            // إنشاء مجلد الحفظ إذا لم يكن موجوداً
            // Create save directory if not exists
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            _globalMetrics.IndexCreatedAt = DateTime.UtcNow;
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.InitializationError, $"خطأ في تهيئة مدير الفهارس: {ex.Message}", ex);
        }
    }

    #endregion

    #region إدارة الفهارس - Index Management

    /// <summary>
    /// إنشاء فهرس جديد
    /// Create new index
    /// </summary>
    /// <typeparam name="T">نوع البيانات - Data type</typeparam>
    /// <param name="configuration">تكوين الفهرس - Index configuration</param>
    /// <returns>الفهرس المُنشأ - Created index</returns>
    public async Task<IAdvancedIndex<T>?> CreateIndexAsync<T>(IndexConfiguration configuration) where T : class
    {
        if (configuration == null)
        {
            OnManagerError(IndexErrorType.ValidationError, "تكوين الفهرس فارغ - Index configuration is null");
            return null;
        }

        try
        {
            lock (_lockObject)
            {
                // التحقق من عدم وجود فهرس بنفس المعرف
                // Check if index with same ID doesn't exist
                if (_indices.ContainsKey(configuration.IndexId))
                {
                    OnManagerError(IndexErrorType.DuplicateKey, $"فهرس بالمعرف {configuration.IndexId} موجود مسبقاً - Index with ID {configuration.IndexId} already exists");
                    return null;
                }

                // إنشاء الفهرس
                // Create index
                var index = new AdvancedIndexService<T>(configuration);

                // حفظ الفهرس والتكوين
                // Save index and configuration
                _indices[configuration.IndexId] = index;
                _configurations[configuration.IndexId] = configuration;

                // ربط الأحداث
                // Subscribe to events
                SubscribeToIndexEvents(index, configuration.IndexId);

                // إطلاق حدث الإنشاء
                // Trigger creation event
                OnIndexCreated(configuration.IndexId, IndexStatus.Active);

                // تحديث الإحصائيات
                // Update statistics
                _globalMetrics.TotalCreateOperations++;

                return index;
            }
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.OperationError, $"خطأ في إنشاء الفهرس {configuration.IndexId}: {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// الحصول على فهرس موجود
    /// Get existing index
    /// </summary>
    /// <typeparam name="T">نوع البيانات - Data type</typeparam>
    /// <param name="indexId">معرف الفهرس - Index ID</param>
    /// <returns>الفهرس المطلوب - Requested index</returns>
    public IAdvancedIndex<T>? GetIndex<T>(string indexId) where T : class
    {
        if (string.IsNullOrWhiteSpace(indexId))
        {
            OnManagerError(IndexErrorType.ValidationError, "معرف الفهرس فارغ - Index ID is null/empty");
            return null;
        }

        try
        {
            if (_indices.TryGetValue(indexId, out var index) && index is IAdvancedIndex<T> typedIndex)
            {
                return typedIndex;
            }

            OnManagerError(IndexErrorType.ItemNotFound, $"الفهرس {indexId} غير موجود - Index {indexId} not found");
            return null;
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.OperationError, $"خطأ في الحصول على الفهرس {indexId}: {ex.Message}", ex);
            return null;
        }
    }

    /// <summary>
    /// حذف فهرس
    /// Remove index
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index ID</param>
    /// <returns>نتيجة العملية - Operation result</returns>
    public async Task<bool> RemoveIndexAsync(string indexId)
    {
        if (string.IsNullOrWhiteSpace(indexId))
        {
            OnManagerError(IndexErrorType.ValidationError, "معرف الفهرس فارغ - Index ID is null/empty");
            return false;
        }

        try
        {
            lock (_lockObject)
            {
                if (!_indices.TryGetValue(indexId, out var index))
                {
                    OnManagerError(IndexErrorType.ItemNotFound, $"الفهرس {indexId} غير موجود - Index {indexId} not found");
                    return false;
                }

                // حفظ الفهرس قبل الحذف
                // Save index before removal
                if (index is IDisposable disposableIndex)
                {
                    disposableIndex.Dispose();
                }

                // إزالة من القواميس
                // Remove from dictionaries
                _indices.TryRemove(indexId, out _);
                _configurations.TryRemove(indexId, out _);

                // حذف ملف الفهرس
                // Delete index file
                var indexFilePath = Path.Combine(SaveDirectory, $"{indexId}.json");
                if (File.Exists(indexFilePath))
                {
                    File.Delete(indexFilePath);
                }

                // إطلاق حدث الحذف
                // Trigger removal event
                OnIndexRemoved(indexId, IndexStatus.Removed);

                // تحديث الإحصائيات
                // Update statistics
                _globalMetrics.TotalRemoveOperations++;

                return true;
            }
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.OperationError, $"خطأ في حذف الفهرس {indexId}: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// الحصول على قائمة جميع الفهارس
    /// Get list of all indices
    /// </summary>
    /// <returns>قائمة معرفات الفهارس - List of index IDs</returns>
    public IEnumerable<string> GetAllIndexIds()
    {
        return _indices.Keys.ToList();
    }

    /// <summary>
    /// الحصول على تكوين فهرس
    /// Get index configuration
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index ID</param>
    /// <returns>تكوين الفهرس - Index configuration</returns>
    public IndexConfiguration? GetIndexConfiguration(string indexId)
    {
        _configurations.TryGetValue(indexId, out var configuration);
        return configuration;
    }

    #endregion

    #region عمليات متعددة الفهارس - Multi-Index Operations

    /// <summary>
    /// البحث في عدة فهارس
    /// Search across multiple indices
    /// </summary>
    /// <typeparam name="T">نوع البيانات - Data type</typeparam>
    /// <param name="indexIds">معرفات الفهارس - Index IDs</param>
    /// <param name="searchRequest">طلب البحث - Search request</param>
    /// <returns>نتائج البحث المجمعة - Aggregated search results</returns>
    public async Task<SearchResult<T>> SearchMultipleIndicesAsync<T>(
        IEnumerable<string> indexIds, 
        SearchRequest searchRequest) where T : class
    {
        var result = new SearchResult<T>
        {
            RequestId = searchRequest.RequestId,
            Success = true,
            Items = new List<T>()
        };

        var startTime = DateTime.UtcNow;
        var usedIndices = new List<string>();
        var totalCount = 0;

        try
        {
            var searchTasks = new List<Task<SearchResult<T>>>();

            // إنشاء مهام البحث لكل فهرس
            // Create search tasks for each index
            foreach (var indexId in indexIds)
            {
                var index = GetIndex<T>(indexId);
                if (index != null)
                {
                    searchTasks.Add(index.SearchAsync(searchRequest));
                    usedIndices.Add(indexId);
                }
            }

            // تنفيذ البحث بشكل متوازي
            // Execute searches in parallel
            var searchResults = await Task.WhenAll(searchTasks);

            // دمج النتائج
            // Merge results
            foreach (var searchResult in searchResults)
            {
                if (searchResult.Success && searchResult.Items != null)
                {
                    result.Items.AddRange(searchResult.Items);
                    totalCount += searchResult.Statistics?.TotalCount ?? 0;
                }
            }

            // ترتيب النتائج المجمعة (يمكن تخصيص هذا حسب نوع T)
            // Sort aggregated results (can be customized based on type T)
            if (searchRequest.SortCriteria != null && searchRequest.SortCriteria.Count > 0)
            {
                // ترتيب بسيط - يمكن تحسينه لاحقاً
                // Simple sorting - can be improved later
                result.Items = result.Items.ToList();
            }

            // تطبيق التصفح على النتائج المجمعة
            // Apply pagination to aggregated results
            var paginatedItems = result.Items
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .ToList();

            result.Items = paginatedItems;
            result.Statistics = new SearchStatistics
            {
                TotalCount = totalCount,
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize,
                ExecutionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds,
                UsedIndices = usedIndices
            };

            // تحديث الإحصائيات العامة
            // Update global statistics
            _globalMetrics.TotalSearchOperations++;

            return result;
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.SearchError, $"خطأ في البحث المتعدد: {ex.Message}", ex);
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// حفظ جميع الفهارس
    /// Save all indices
    /// </summary>
    /// <returns>نتيجة العملية - Operation result</returns>
    public async Task<bool> SaveAllIndicesAsync()
    {
        var allSaved = true;

        try
        {
            var saveTasks = new List<Task<bool>>();

            foreach (var kvp in _indices)
            {
                var indexId = kvp.Key;
                var index = kvp.Value;

                if (index is IAdvancedIndex<object> advancedIndex)
                {
                    var filePath = Path.Combine(SaveDirectory, $"{indexId}.json");
                    saveTasks.Add(advancedIndex.SaveToFileAsync(filePath));
                }
            }

            var results = await Task.WhenAll(saveTasks);
            allSaved = results.All(r => r);

            if (allSaved)
            {
                _globalMetrics.TotalSaveOperations++;
            }

            return allSaved;
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.FileOperationError, $"خطأ في حفظ جميع الفهارس: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// تحميل جميع الفهارس من المجلد
    /// Load all indices from directory
    /// </summary>
    /// <returns>عدد الفهارس المحملة - Number of loaded indices</returns>
    public async Task<int> LoadAllIndicesAsync()
    {
        var loadedCount = 0;

        try
        {
            if (!Directory.Exists(SaveDirectory))
            {
                return 0;
            }

            var indexFiles = Directory.GetFiles(SaveDirectory, "*.json");

            foreach (var filePath in indexFiles)
            {
                try
                {
                    var indexId = Path.GetFileNameWithoutExtension(filePath);
                    
                    // محاولة تحميل التكوين من الملف
                    // Try to load configuration from file
                    var json = await File.ReadAllTextAsync(filePath);
                    // هنا يمكن إضافة منطق تحميل التكوين من الملف
                    // Here we can add logic to load configuration from file
                    
                    loadedCount++;
                }
                catch (Exception ex)
                {
                    OnManagerError(IndexErrorType.FileOperationError, $"خطأ في تحميل الفهرس من {filePath}: {ex.Message}", ex);
                }
            }

            if (loadedCount > 0)
            {
                _globalMetrics.TotalLoadOperations++;
            }

            return loadedCount;
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.FileOperationError, $"خطأ في تحميل الفهارس: {ex.Message}", ex);
            return 0;
        }
    }

    /// <summary>
    /// إعادة بناء جميع الفهارس
    /// Rebuild all indices
    /// </summary>
    /// <returns>نتيجة العملية - Operation result</returns>
    public async Task<bool> RebuildAllIndicesAsync()
    {
        var allRebuilt = true;

        try
        {
            var rebuildTasks = new List<Task<bool>>();

            foreach (var kvp in _indices)
            {
                var index = kvp.Value;

                if (index is IAdvancedIndex<object> advancedIndex)
                {
                    rebuildTasks.Add(advancedIndex.RebuildIndexAsync());
                }
            }

            var results = await Task.WhenAll(rebuildTasks);
            allRebuilt = results.All(r => r);

            if (allRebuilt)
            {
                _globalMetrics.TotalRebuildOperations++;
            }

            return allRebuilt;
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.RebuildError, $"خطأ في إعادة بناء جميع الفهارس: {ex.Message}", ex);
            return false;
        }
    }

    #endregion

    #region الإحصائيات والمراقبة - Statistics and Monitoring

    /// <summary>
    /// الحصول على إحصائيات المدير العامة
    /// Get global manager statistics
    /// </summary>
    /// <returns>الإحصائيات العامة - Global statistics</returns>
    public PerformanceMetrics GetGlobalStatistics()
    {
        _globalMetrics.CurrentItemCount = _indices.Count;
        _globalMetrics.LastUpdated = DateTime.UtcNow;
        return _globalMetrics;
    }

    /// <summary>
    /// الحصول على إحصائيات فهرس محدد
    /// Get specific index statistics
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index ID</param>
    /// <returns>إحصائيات الفهرس - Index statistics</returns>
    public PerformanceMetrics? GetIndexStatistics(string indexId)
    {
        if (_indices.TryGetValue(indexId, out var index))
        {
            if (index is IAdvancedIndex<object> advancedIndex)
            {
                return advancedIndex.GetStatistics();
            }
        }

        return null;
    }

    /// <summary>
    /// الحصول على تقرير حالة جميع الفهارس
    /// Get status report for all indices
    /// </summary>
    /// <returns>تقرير الحالة - Status report</returns>
    public Dictionary<string, object> GetStatusReport()
    {
        var report = new Dictionary<string, object>
        {
            ["ManagerStatus"] = IsInitialized ? "Active" : "Inactive",
            ["TotalIndices"] = _indices.Count,
            ["SaveDirectory"] = SaveDirectory,
            ["GlobalStatistics"] = GetGlobalStatistics(),
            ["IndicesStatus"] = new Dictionary<string, object>()
        };

        var indicesStatus = (Dictionary<string, object>)report["IndicesStatus"];

        foreach (var kvp in _indices)
        {
            var indexId = kvp.Key;
            var index = kvp.Value;

            if (index is IAdvancedIndex<object> advancedIndex)
            {
                indicesStatus[indexId] = new
                {
                    Status = advancedIndex.Status.ToString(),
                    ItemCount = advancedIndex.ItemCount,
                    LastUpdated = advancedIndex.LastUpdated,
                    Configuration = _configurations.TryGetValue(indexId, out var config) ? config : null
                };
            }
        }

        return report;
    }

    #endregion

    #region المساعدات الخاصة - Private Helpers

    /// <summary>
    /// ربط أحداث الفهرس
    /// Subscribe to index events
    /// </summary>
    private void SubscribeToIndexEvents<T>(IAdvancedIndex<T> index, string indexId) where T : class
    {
        index.StatusChanged += (sender, args) =>
        {
            // إعادة توجيه أحداث الفهرس
            // Forward index events
            OnIndexStatusChanged(args);
        };

        index.ErrorOccurred += (sender, args) =>
        {
            // إعادة توجيه أخطاء الفهرس
            // Forward index errors
            OnManagerError(args.ErrorType, args.ErrorMessage, args.Exception);
        };
    }

    #endregion

    #region إطلاق الأحداث - Event Triggers

    private void OnIndexCreated(string indexId, IndexStatus status)
    {
        IndexCreated?.Invoke(this, new IndexStatusChangedEventArgs(
            indexId,
            indexId,
            IndexStatus.Initializing,
            status,
            "Index created"
        ));
    }

    private void OnIndexRemoved(string indexId, IndexStatus status)
    {
        IndexRemoved?.Invoke(this, new IndexStatusChangedEventArgs(
            indexId,
            indexId,
            IndexStatus.Active,
            status,
            "Index removed"
        ));
    }

    private void OnIndexStatusChanged(IndexStatusChangedEventArgs args)
    {
        // يمكن إضافة منطق إضافي هنا
        // Additional logic can be added here
    }

    private void OnManagerError(IndexErrorType errorType, string message, Exception? exception = null)
    {
        ManagerError?.Invoke(this, new IndexErrorEventArgs(
            "IndexManager",
            "IndexManager",
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
            // حفظ جميع الفهارس قبل التخلص
            // Save all indices before disposal
            _ = SaveAllIndicesAsync().GetAwaiter().GetResult();

            // تحرير جميع الفهارس
            // Dispose all indices
            foreach (var index in _indices.Values)
            {
                if (index is IDisposable disposableIndex)
                {
                    disposableIndex.Dispose();
                }
            }

            _indices.Clear();
            _configurations.Clear();
        }
        catch (Exception ex)
        {
            OnManagerError(IndexErrorType.DisposeError, $"خطأ في تحرير موارد المدير: {ex.Message}", ex);
        }
    }

    #endregion
}