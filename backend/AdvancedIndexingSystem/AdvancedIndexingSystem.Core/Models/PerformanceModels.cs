using Newtonsoft.Json;

namespace AdvancedIndexingSystem.Core.Models;

/// <summary>
/// معايير الأداء للفهرس
/// Index performance metrics
/// </summary>
public class PerformanceMetrics
{
    /// <summary>
    /// معرف الفهرس
    /// Index identifier
    /// </summary>
    [JsonProperty("index_id")]
    public string IndexId { get; set; } = string.Empty;

    /// <summary>
    /// اسم الفهرس
    /// Index name
    /// </summary>
    [JsonProperty("index_name")]
    public string IndexName { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ إنشاء الفهرس
    /// Index creation timestamp
    /// </summary>
    [JsonProperty("index_created_at")]
    public DateTime IndexCreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// وقت آخر عملية
    /// Last operation time
    /// </summary>
    [JsonProperty("last_operation_time")]
    public TimeSpan LastOperationTime { get; set; }

    /// <summary>
    /// عدد العناصر الحالي
    /// Current item count
    /// </summary>
    [JsonProperty("current_item_count")]
    public int CurrentItemCount { get; set; }

    /// <summary>
    /// عدد العناصر المفهرسة
    /// Number of indexed items
    /// </summary>
    [JsonProperty("total_items")]
    public int TotalItems { get; set; }

    /// <summary>
    /// عدد العناصر المفهرسة (اختصار للوصول السريع)
    /// Indexed items count (shortcut for quick access)
    /// </summary>
    [JsonProperty("indexed_items_count")]
    public int IndexedItemsCount 
    { 
        get => TotalItems; 
        set => TotalItems = value; 
    }

    /// <summary>
    /// عمليات البحث في الثانية
    /// Searches per second
    /// </summary>
    [JsonProperty("searches_per_second")]
    public double SearchesPerSecond { get; set; }

    /// <summary>
    /// حجم الفهرس بالبايت
    /// Index size in bytes
    /// </summary>
    [JsonProperty("index_size_bytes")]
    public long IndexSizeBytes { get; set; }

    /// <summary>
    /// متوسط وقت البحث بالميلي ثانية
    /// Average search time in milliseconds
    /// </summary>
    [JsonProperty("average_search_time_ms")]
    public double AverageSearchTimeMs { get; set; }

    /// <summary>
    /// متوسط وقت البحث (اختصار للوصول السريع)
    /// Average search time (shortcut for quick access)
    /// </summary>
    [JsonProperty("average_search_time")]
    public double AverageSearchTime 
    { 
        get => AverageSearchTimeMs; 
        set => AverageSearchTimeMs = value; 
    }

    /// <summary>
    /// متوسط وقت الإدراج بالميلي ثانية
    /// Average insertion time in milliseconds
    /// </summary>
    [JsonProperty("average_insertion_time_ms")]
    public double AverageInsertionTimeMs { get; set; }

    /// <summary>
    /// متوسط وقت التحديث بالميلي ثانية
    /// Average update time in milliseconds
    /// </summary>
    [JsonProperty("average_update_time_ms")]
    public double AverageUpdateTimeMs { get; set; }

    /// <summary>
    /// متوسط وقت الحذف بالميلي ثانية
    /// Average deletion time in milliseconds
    /// </summary>
    [JsonProperty("average_deletion_time_ms")]
    public double AverageDeletionTimeMs { get; set; }

    /// <summary>
    /// إجمالي عمليات الإنشاء
    /// Total create operations
    /// </summary>
    [JsonProperty("total_create_operations")]
    public long TotalCreateOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات الإضافة
    /// Total add operations
    /// </summary>
    [JsonProperty("total_add_operations")]
    public long TotalAddOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات التحديث
    /// Total update operations
    /// </summary>
    [JsonProperty("total_update_operations")]
    public long TotalUpdateOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات الحذف
    /// Total remove operations
    /// </summary>
    [JsonProperty("total_remove_operations")]
    public long TotalRemoveOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات البحث
    /// Total search operations
    /// </summary>
    [JsonProperty("total_search_operations")]
    public long TotalSearchOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات الحفظ
    /// Total save operations
    /// </summary>
    [JsonProperty("total_save_operations")]
    public long TotalSaveOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات التحميل
    /// Total load operations
    /// </summary>
    [JsonProperty("total_load_operations")]
    public long TotalLoadOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات إعادة البناء
    /// Total rebuild operations
    /// </summary>
    [JsonProperty("total_rebuild_operations")]
    public long TotalRebuildOperations { get; set; }

    /// <summary>
    /// إجمالي عمليات المسح
    /// Total clear operations
    /// </summary>
    [JsonProperty("total_clear_operations")]
    public long TotalClearOperations { get; set; }

    /// <summary>
    /// عدد عمليات البحث المنفذة
    /// Number of search operations executed
    /// </summary>
    [JsonProperty("search_operations_count")]
    public long SearchOperationsCount { get; set; }

    /// <summary>
    /// عدد عمليات الإدراج المنفذة
    /// Number of insertion operations executed
    /// </summary>
    [JsonProperty("insertion_operations_count")]
    public long InsertionOperationsCount { get; set; }

    /// <summary>
    /// عدد عمليات التحديث المنفذة
    /// Number of update operations executed
    /// </summary>
    [JsonProperty("update_operations_count")]
    public long UpdateOperationsCount { get; set; }

    /// <summary>
    /// عدد عمليات الحذف المنفذة
    /// Number of deletion operations executed
    /// </summary>
    [JsonProperty("deletion_operations_count")]
    public long DeletionOperationsCount { get; set; }

    /// <summary>
    /// معدل نجاح العمليات
    /// Operation success rate
    /// </summary>
    [JsonProperty("success_rate")]
    public double SuccessRate { get; set; }

    /// <summary>
    /// استهلاك الذاكرة بالميجابايت
    /// Memory usage in megabytes
    /// </summary>
    [JsonProperty("memory_usage_mb")]
    public double MemoryUsageMb { get; set; }

    /// <summary>
    /// معدل الإنتاجية (عمليات في الثانية)
    /// Throughput rate (operations per second)
    /// </summary>
    [JsonProperty("throughput_ops_per_second")]
    public double ThroughputOpsPerSecond { get; set; }

    /// <summary>
    /// نسبة كفاءة الفهرس
    /// Index efficiency ratio
    /// </summary>
    [JsonProperty("efficiency_ratio")]
    public double EfficiencyRatio { get; set; }

    /// <summary>
    /// عدد مرات إعادة بناء الفهرس
    /// Number of index rebuilds
    /// </summary>
    [JsonProperty("rebuild_count")]
    public int RebuildCount { get; set; }

    /// <summary>
    /// تاريخ آخر إعادة بناء
    /// Last rebuild timestamp
    /// </summary>
    [JsonProperty("last_rebuild_time")]
    public DateTime? LastRebuildTime { get; set; }

    /// <summary>
    /// تاريخ بداية جمع الإحصائيات
    /// Statistics collection start time
    /// </summary>
    [JsonProperty("statistics_start_time")]
    public DateTime StatisticsStartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تاريخ آخر تحديث للإحصائيات
    /// Last statistics update time
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// إحصائيات الحقول
    /// Field statistics
    /// </summary>
    [JsonProperty("field_statistics")]
    public Dictionary<string, FieldStatistics> FieldStatistics { get; set; } = new();

    /// <summary>
    /// إحصائيات التخزين المؤقت
    /// Cache statistics
    /// </summary>
    [JsonProperty("cache_statistics")]
    public CacheStatistics CacheStatistics { get; set; } = new();

    /// <summary>
    /// تفاصيل الأداء الإضافية
    /// Additional performance details
    /// </summary>
    [JsonProperty("additional_details")]
    public Dictionary<string, object> AdditionalDetails { get; set; } = new();

    /// <summary>
    /// حساب الإحصائيات المشتقة
    /// Calculate derived statistics
    /// </summary>
    public void CalculateDerivedStatistics()
    {
        // Calculate success rate
        var totalOperations = SearchOperationsCount + InsertionOperationsCount + UpdateOperationsCount + DeletionOperationsCount;
        if (totalOperations > 0)
        {
            // Assuming all recorded operations were successful for now
            // In a real implementation, you'd track failed operations separately
            SuccessRate = 1.0; // 100% for successful operations only
        }

        // Calculate throughput
        var timeSinceStart = (DateTime.UtcNow - StatisticsStartTime).TotalSeconds;
        if (timeSinceStart > 0)
        {
            ThroughputOpsPerSecond = totalOperations / timeSinceStart;
        }

        // Calculate efficiency ratio (items processed per MB of memory)
        if (MemoryUsageMb > 0)
        {
            EfficiencyRatio = TotalItems / MemoryUsageMb;
        }

        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// إعادة تعيين الإحصائيات
    /// Reset statistics
    /// </summary>
    public void Reset()
    {
        SearchOperationsCount = 0;
        InsertionOperationsCount = 0;
        UpdateOperationsCount = 0;
        DeletionOperationsCount = 0;
        AverageSearchTimeMs = 0;
        AverageInsertionTimeMs = 0;
        AverageUpdateTimeMs = 0;
        AverageDeletionTimeMs = 0;
        SuccessRate = 0;
        ThroughputOpsPerSecond = 0;
        EfficiencyRatio = 0;
        RebuildCount = 0;
        LastRebuildTime = null;
        StatisticsStartTime = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
        FieldStatistics.Clear();
        CacheStatistics = new CacheStatistics();
        AdditionalDetails.Clear();
    }
}

/// <summary>
/// إحصائيات الحقل
/// Field statistics
/// </summary>
public class FieldStatistics
{
    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    [JsonProperty("field_name")]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// نوع بيانات الحقل
    /// Field data type
    /// </summary>
    [JsonProperty("data_type")]
    public FieldDataType DataType { get; set; }

    /// <summary>
    /// عدد القيم الفريدة
    /// Number of unique values
    /// </summary>
    [JsonProperty("unique_values_count")]
    public int UniqueValuesCount { get; set; }

    /// <summary>
    /// عدد القيم الفارغة
    /// Number of null values
    /// </summary>
    [JsonProperty("null_values_count")]
    public int NullValuesCount { get; set; }

    /// <summary>
    /// متوسط طول القيمة (للنصوص)
    /// Average value length (for text fields)
    /// </summary>
    [JsonProperty("average_value_length")]
    public double AverageValueLength { get; set; }

    /// <summary>
    /// أقل قيمة
    /// Minimum value
    /// </summary>
    [JsonProperty("min_value")]
    public object? MinValue { get; set; }

    /// <summary>
    /// أكبر قيمة
    /// Maximum value
    /// </summary>
    [JsonProperty("max_value")]
    public object? MaxValue { get; set; }

    /// <summary>
    /// القيم الأكثر شيوعاً
    /// Most common values
    /// </summary>
    [JsonProperty("most_common_values")]
    public Dictionary<object, int> MostCommonValues { get; set; } = new();

    /// <summary>
    /// عدد مرات البحث في هذا الحقل
    /// Number of searches on this field
    /// </summary>
    [JsonProperty("search_count")]
    public long SearchCount { get; set; }

    /// <summary>
    /// متوسط وقت البحث في هذا الحقل
    /// Average search time for this field
    /// </summary>
    [JsonProperty("average_search_time_ms")]
    public double AverageSearchTimeMs { get; set; }

    /// <summary>
    /// هل الحقل مفهرس
    /// Is field indexed
    /// </summary>
    [JsonProperty("is_indexed")]
    public bool IsIndexed { get; set; }

    /// <summary>
    /// حجم فهرس الحقل بالبايت
    /// Field index size in bytes
    /// </summary>
    [JsonProperty("index_size_bytes")]
    public long IndexSizeBytes { get; set; }
}

/// <summary>
/// إحصائيات التخزين المؤقت
/// Cache statistics
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// حجم التخزين المؤقت بالبايت
    /// Cache size in bytes
    /// </summary>
    [JsonProperty("cache_size_bytes")]
    public long CacheSizeBytes { get; set; }

    /// <summary>
    /// عدد العناصر في التخزين المؤقت
    /// Number of cached items
    /// </summary>
    [JsonProperty("cached_items_count")]
    public int CachedItemsCount { get; set; }

    /// <summary>
    /// عدد مرات النجاح في الوصول للتخزين المؤقت
    /// Number of cache hits
    /// </summary>
    [JsonProperty("cache_hits")]
    public long CacheHits { get; set; }

    /// <summary>
    /// عدد مرات الفشل في الوصول للتخزين المؤقت
    /// Number of cache misses
    /// </summary>
    [JsonProperty("cache_misses")]
    public long CacheMisses { get; set; }

    /// <summary>
    /// نسبة نجاح التخزين المؤقت
    /// Cache hit ratio
    /// </summary>
    [JsonProperty("cache_hit_ratio")]
    public double CacheHitRatio
    {
        get
        {
            var totalRequests = CacheHits + CacheMisses;
            return totalRequests > 0 ? (double)CacheHits / totalRequests : 0.0;
        }
    }

    /// <summary>
    /// عدد مرات إخلاء التخزين المؤقت
    /// Number of cache evictions
    /// </summary>
    [JsonProperty("cache_evictions")]
    public long CacheEvictions { get; set; }

    /// <summary>
    /// متوسط وقت الوصول للتخزين المؤقت
    /// Average cache access time
    /// </summary>
    [JsonProperty("average_cache_access_time_ms")]
    public double AverageCacheAccessTimeMs { get; set; }

    /// <summary>
    /// تاريخ آخر تنظيف للتخزين المؤقت
    /// Last cache cleanup time
    /// </summary>
    [JsonProperty("last_cleanup_time")]
    public DateTime? LastCleanupTime { get; set; }
}

/// <summary>
/// تقرير الأداء المفصل
/// Detailed performance report
/// </summary>
public class PerformanceReport
{
    /// <summary>
    /// معرف التقرير
    /// Report identifier
    /// </summary>
    [JsonProperty("report_id")]
    public string ReportId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// تاريخ إنشاء التقرير
    /// Report generation time
    /// </summary>
    [JsonProperty("generated_at")]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// فترة التقرير (من)
    /// Report period start
    /// </summary>
    [JsonProperty("period_start")]
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// فترة التقرير (إلى)
    /// Report period end
    /// </summary>
    [JsonProperty("period_end")]
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// معايير الأداء
    /// Performance metrics
    /// </summary>
    [JsonProperty("metrics")]
    public PerformanceMetrics Metrics { get; set; } = new();

    /// <summary>
    /// التوصيات لتحسين الأداء
    /// Performance improvement recommendations
    /// </summary>
    [JsonProperty("recommendations")]
    public List<PerformanceRecommendation> Recommendations { get; set; } = new();

    /// <summary>
    /// مقارنة مع الفترة السابقة
    /// Comparison with previous period
    /// </summary>
    [JsonProperty("period_comparison")]
    public PerformanceComparison? PeriodComparison { get; set; }

    /// <summary>
    /// تحذيرات الأداء
    /// Performance warnings
    /// </summary>
    [JsonProperty("warnings")]
    public List<PerformanceWarning> Warnings { get; set; } = new();

    /// <summary>
    /// ملخص التقرير
    /// Report summary
    /// </summary>
    [JsonProperty("summary")]
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// توصية تحسين الأداء
/// Performance improvement recommendation
/// </summary>
public class PerformanceRecommendation
{
    /// <summary>
    /// نوع التوصية
    /// Recommendation type
    /// </summary>
    [JsonProperty("type")]
    public RecommendationType Type { get; set; }

    /// <summary>
    /// العنوان
    /// Title
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// الوصف
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// الأولوية
    /// Priority
    /// </summary>
    [JsonProperty("priority")]
    public RecommendationPriority Priority { get; set; }

    /// <summary>
    /// التأثير المتوقع
    /// Expected impact
    /// </summary>
    [JsonProperty("expected_impact")]
    public string ExpectedImpact { get; set; } = string.Empty;

    /// <summary>
    /// خطوات التنفيذ
    /// Implementation steps
    /// </summary>
    [JsonProperty("implementation_steps")]
    public List<string> ImplementationSteps { get; set; } = new();
}

/// <summary>
/// مقارنة الأداء
/// Performance comparison
/// </summary>
public class PerformanceComparison
{
    /// <summary>
    /// تغيير متوسط وقت البحث
    /// Search time change percentage
    /// </summary>
    [JsonProperty("search_time_change_percent")]
    public double SearchTimeChangePercent { get; set; }

    /// <summary>
    /// تغيير الإنتاجية
    /// Throughput change percentage
    /// </summary>
    [JsonProperty("throughput_change_percent")]
    public double ThroughputChangePercent { get; set; }

    /// <summary>
    /// تغيير استهلاك الذاكرة
    /// Memory usage change percentage
    /// </summary>
    [JsonProperty("memory_usage_change_percent")]
    public double MemoryUsageChangePercent { get; set; }

    /// <summary>
    /// تغيير معدل النجاح
    /// Success rate change percentage
    /// </summary>
    [JsonProperty("success_rate_change_percent")]
    public double SuccessRateChangePercent { get; set; }

    /// <summary>
    /// التقييم العام للتغيير
    /// Overall change assessment
    /// </summary>
    [JsonProperty("overall_assessment")]
    public PerformanceChangeAssessment OverallAssessment { get; set; }
}

/// <summary>
/// تحذير الأداء
/// Performance warning
/// </summary>
public class PerformanceWarning
{
    /// <summary>
    /// نوع التحذير
    /// Warning type
    /// </summary>
    [JsonProperty("type")]
    public WarningType Type { get; set; }

    /// <summary>
    /// الرسالة
    /// Message
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// مستوى الخطورة
    /// Severity level
    /// </summary>
    [JsonProperty("severity")]
    public WarningSeverity Severity { get; set; }

    /// <summary>
    /// القيمة الحالية
    /// Current value
    /// </summary>
    [JsonProperty("current_value")]
    public object? CurrentValue { get; set; }

    /// <summary>
    /// القيمة المستهدفة
    /// Target value
    /// </summary>
    [JsonProperty("target_value")]
    public object? TargetValue { get; set; }

    /// <summary>
    /// تاريخ حدوث التحذير
    /// Warning occurrence time
    /// </summary>
    [JsonProperty("occurred_at")]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

#region Enums

/// <summary>
/// نوع التوصية
/// Recommendation type
/// </summary>
public enum RecommendationType
{
    /// <summary>
    /// تحسين الفهرس
    /// Index optimization
    /// </summary>
    IndexOptimization,

    /// <summary>
    /// تحسين الذاكرة
    /// Memory optimization
    /// </summary>
    MemoryOptimization,

    /// <summary>
    /// تحسين التخزين المؤقت
    /// Cache optimization
    /// </summary>
    CacheOptimization,

    /// <summary>
    /// تحسين الاستعلام
    /// Query optimization
    /// </summary>
    QueryOptimization,

    /// <summary>
    /// إعادة هيكلة البيانات
    /// Data restructuring
    /// </summary>
    DataRestructuring,

    /// <summary>
    /// تحسين التكوين
    /// Configuration optimization
    /// </summary>
    ConfigurationOptimization
}

/// <summary>
/// أولوية التوصية
/// Recommendation priority
/// </summary>
public enum RecommendationPriority
{
    /// <summary>
    /// منخفض
    /// Low
    /// </summary>
    Low = 1,

    /// <summary>
    /// متوسط
    /// Medium
    /// </summary>
    Medium = 2,

    /// <summary>
    /// عالي
    /// High
    /// </summary>
    High = 3,

    /// <summary>
    /// حرج
    /// Critical
    /// </summary>
    Critical = 4
}

/// <summary>
/// تقييم تغيير الأداء
/// Performance change assessment
/// </summary>
public enum PerformanceChangeAssessment
{
    /// <summary>
    /// تحسن كبير
    /// Significant improvement
    /// </summary>
    SignificantImprovement,

    /// <summary>
    /// تحسن طفيف
    /// Minor improvement
    /// </summary>
    MinorImprovement,

    /// <summary>
    /// لا يوجد تغيير
    /// No change
    /// </summary>
    NoChange,

    /// <summary>
    /// تراجع طفيف
    /// Minor degradation
    /// </summary>
    MinorDegradation,

    /// <summary>
    /// تراجع كبير
    /// Significant degradation
    /// </summary>
    SignificantDegradation
}

/// <summary>
/// نوع التحذير
/// Warning type
/// </summary>
public enum WarningType
{
    /// <summary>
    /// استهلاك ذاكرة عالي
    /// High memory usage
    /// </summary>
    HighMemoryUsage,

    /// <summary>
    /// بطء في البحث
    /// Slow search performance
    /// </summary>
    SlowSearchPerformance,

    /// <summary>
    /// انخفاض معدل النجاح
    /// Low success rate
    /// </summary>
    LowSuccessRate,

    /// <summary>
    /// فهرس كبير جداً
    /// Index too large
    /// </summary>
    IndexTooLarge,

    /// <summary>
    /// تخزين مؤقت غير فعال
    /// Ineffective caching
    /// </summary>
    IneffectiveCaching,

    /// <summary>
    /// حاجة لإعادة بناء الفهرس
    /// Index rebuild needed
    /// </summary>
    IndexRebuildNeeded
}

/// <summary>
/// مستوى خطورة التحذير
/// Warning severity level
/// </summary>
public enum WarningSeverity
{
    /// <summary>
    /// معلومات
    /// Information
    /// </summary>
    Info = 1,

    /// <summary>
    /// تحذير
    /// Warning
    /// </summary>
    Warning = 2,

    /// <summary>
    /// خطأ
    /// Error
    /// </summary>
    Error = 3,

    /// <summary>
    /// حرج
    /// Critical
    /// </summary>
    Critical = 4
}

#endregion