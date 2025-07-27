using Newtonsoft.Json;

namespace AdvancedIndexingSystem.Core.Models;

/// <summary>
/// طلب البحث
/// Search request
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// معرف طلب البحث الفريد
    /// Unique search request identifier
    /// </summary>
    [JsonProperty("request_id")]
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// معايير البحث
    /// Search criteria
    /// </summary>
    [JsonProperty("search_criteria")]
    public List<SearchCriterion> SearchCriteria { get; set; } = new();

    /// <summary>
    /// معايير الفرز
    /// Sort criteria
    /// </summary>
    [JsonProperty("sort_criteria")]
    public List<SortCriterion> SortCriteria { get; set; } = new();

    /// <summary>
    /// رقم الصفحة (للبحث المقسم)
    /// Page number (for paginated search)
    /// </summary>
    [JsonProperty("page_number")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    [JsonProperty("page_size")]
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// هل يتم إرجاع العدد الإجمالي
    /// Include total count
    /// </summary>
    [JsonProperty("include_total_count")]
    public bool IncludeTotalCount { get; set; } = true;

    /// <summary>
    /// هل يتم إرجاع إحصائيات البحث
    /// Include search statistics
    /// </summary>
    [JsonProperty("include_statistics")]
    public bool IncludeStatistics { get; set; } = false;

    /// <summary>
    /// الحد الأقصى لوقت البحث (بالميلي ثانية)
    /// Maximum search time (milliseconds)
    /// </summary>
    [JsonProperty("timeout_ms")]
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// فهارس محددة للبحث فيها
    /// Specific indices to search in
    /// </summary>
    [JsonProperty("target_indices")]
    public List<string> TargetIndices { get; set; } = new();

    /// <summary>
    /// معايير إضافية مخصصة
    /// Additional custom criteria
    /// </summary>
    [JsonProperty("custom_criteria")]
    public Dictionary<string, object> CustomCriteria { get; set; } = new();

    /// <summary>
    /// تاريخ إنشاء الطلب
    /// Request creation time
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// استعلام البحث النصي البسيط
    /// Simple text search query
    /// </summary>
    [JsonProperty("query")]
    public string? Query { get; set; }

    /// <summary>
    /// فلاتر البحث المبسطة
    /// Simplified search filters
    /// </summary>
    [JsonProperty("filters")]
    public List<SearchFilter> Filters { get; set; } = new();

    /// <summary>
    /// ترتيب النتائج حسب
    /// Sort results by
    /// </summary>
    [JsonProperty("sort_by")]
    public string? SortBy { get; set; }

    /// <summary>
    /// اتجاه الترتيب
    /// Sort order direction
    /// </summary>
    [JsonProperty("sort_order")]
    public string? SortOrder { get; set; }
}

/// <summary>
/// فلتر البحث المبسط
/// Simplified search filter
/// </summary>
public class SearchFilter
{
    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// القيمة
    /// Value
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// المشغل
    /// Operator
    /// </summary>
    public string Operator { get; set; } = "equals";

    /// <summary>
    /// حساسية الأحرف
    /// Case sensitive
    /// </summary>
    public bool CaseSensitive { get; set; } = false;
}

/// <summary>
/// معيار البحث
/// Search criterion
/// </summary>
public class SearchCriterion
{
    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    [JsonProperty("field_name")]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// نوع المعيار
    /// Criterion type
    /// </summary>
    [JsonProperty("criterion_type")]
    public SearchCriterionType CriterionType { get; set; }

    /// <summary>
    /// القيمة المطلوب البحث عنها
    /// Search value
    /// </summary>
    [JsonProperty("value")]
    public object? Value { get; set; }

    /// <summary>
    /// قائمة القيم (للبحث المتعدد)
    /// List of values (for multi-value search)
    /// </summary>
    [JsonProperty("values")]
    public List<object> Values { get; set; } = new();

    /// <summary>
    /// القيمة الدنيا (للنطاقات)
    /// Minimum value (for ranges)
    /// </summary>
    [JsonProperty("min_value")]
    public object? MinValue { get; set; }

    /// <summary>
    /// القيمة العليا (للنطاقات)
    /// Maximum value (for ranges)
    /// </summary>
    [JsonProperty("max_value")]
    public object? MaxValue { get; set; }

    /// <summary>
    /// هل البحث حساس لحالة الأحرف
    /// Case sensitive search
    /// </summary>
    [JsonProperty("case_sensitive")]
    public bool CaseSensitive { get; set; } = false;

    /// <summary>
    /// وزن المعيار في النتيجة النهائية
    /// Criterion weight in final result
    /// </summary>
    [JsonProperty("weight")]
    public double Weight { get; set; } = 1.0;

    /// <summary>
    /// هل المعيار مطلوب
    /// Is criterion required
    /// </summary>
    [JsonProperty("is_required")]
    public bool IsRequired { get; set; } = true;
}

/// <summary>
/// أنواع معايير البحث
/// Search criterion types
/// </summary>
public enum SearchCriterionType
{
    /// <summary>
    /// مطابقة تامة - Exact Match
    /// </summary>
    ExactMatch,

    /// <summary>
    /// يحتوي على - Contains
    /// </summary>
    Contains,

    /// <summary>
    /// يبدأ بـ - Starts With
    /// </summary>
    StartsWith,

    /// <summary>
    /// ينتهي بـ - Ends With
    /// </summary>
    EndsWith,

    /// <summary>
    /// أكبر من - Greater Than
    /// </summary>
    GreaterThan,

    /// <summary>
    /// أكبر من أو يساوي - Greater Than Or Equal
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// أصغر من - Less Than
    /// </summary>
    LessThan,

    /// <summary>
    /// أصغر من أو يساوي - Less Than Or Equal
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// في النطاق - In Range
    /// </summary>
    InRange,

    /// <summary>
    /// في القائمة - In List
    /// </summary>
    InList,

    /// <summary>
    /// ليس في القائمة - Not In List
    /// </summary>
    NotInList,

    /// <summary>
    /// فارغ - Is Null
    /// </summary>
    IsNull,

    /// <summary>
    /// غير فارغ - Is Not Null
    /// </summary>
    IsNotNull,

    /// <summary>
    /// تعبير نمطي - Regular Expression
    /// </summary>
    RegularExpression,

    /// <summary>
    /// بحث ضبابي - Fuzzy Search
    /// </summary>
    FuzzySearch
}

/// <summary>
/// معيار الفرز
/// Sort criterion
/// </summary>
public class SortCriterion
{
    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    [JsonProperty("field_name")]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// اتجاه الفرز
    /// Sort direction
    /// </summary>
    [JsonProperty("direction")]
    public SortDirection Direction { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// أولوية الفرز
    /// Sort priority
    /// </summary>
    [JsonProperty("priority")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// نوع البيانات للفرز
    /// Data type for sorting
    /// </summary>
    [JsonProperty("data_type")]
    public FieldDataType DataType { get; set; } = FieldDataType.Text;
}

/// <summary>
/// اتجاه الفرز
/// Sort direction
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// تصاعدي - Ascending
    /// </summary>
    Ascending,

    /// <summary>
    /// تنازلي - Descending
    /// </summary>
    Descending
}

/// <summary>
/// نتيجة البحث
/// Search result
/// </summary>
public class SearchResult<T>
{
    /// <summary>
    /// معرف طلب البحث
    /// Search request identifier
    /// </summary>
    [JsonProperty("request_id")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// العناصر المطابقة
    /// Matching items
    /// </summary>
    [JsonProperty("items")]
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// العدد الإجمالي للنتائج
    /// Total count of results
    /// </summary>
    [JsonProperty("total_count")]
    public int TotalCount { get; set; }

    /// <summary>
    /// رقم الصفحة الحالية
    /// Current page number
    /// </summary>
    [JsonProperty("page_number")]
    public int PageNumber { get; set; }

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    [JsonProperty("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// العدد الإجمالي للصفحات
    /// Total number of pages
    /// </summary>
    [JsonProperty("total_pages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// هل توجد صفحة سابقة
    /// Has previous page
    /// </summary>
    [JsonProperty("has_previous_page")]
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// هل توجد صفحة تالية
    /// Has next page
    /// </summary>
    [JsonProperty("has_next_page")]
    public bool HasNextPage { get; set; }

    /// <summary>
    /// وقت تنفيذ البحث (بالميلي ثانية)
    /// Search execution time (milliseconds)
    /// </summary>
    [JsonProperty("execution_time_ms")]
    public double ExecutionTimeMs { get; set; }

    /// <summary>
    /// إحصائيات البحث
    /// Search statistics
    /// </summary>
    [JsonProperty("search_statistics")]
    public SearchStatistics? SearchStatistics { get; set; }

    /// <summary>
    /// الإحصائيات (اختصار للوصول السريع)
    /// Statistics (shortcut for quick access)
    /// </summary>
    [JsonIgnore]
    public SearchStatistics? Statistics 
    { 
        get => SearchStatistics; 
        set => SearchStatistics = value; 
    }

    /// <summary>
    /// الفهارس المستخدمة في البحث
    /// Indices used in search
    /// </summary>
    [JsonProperty("used_indices")]
    public List<string> UsedIndices { get; set; } = new();

    /// <summary>
    /// رسائل التحذير
    /// Warning messages
    /// </summary>
    [JsonProperty("warnings")]
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// هل البحث ناجح
    /// Is search successful
    /// </summary>
    [JsonProperty("is_successful")]
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// النجاح (اختصار للوصول السريع)
    /// Success (shortcut for quick access)
    /// </summary>
    [JsonIgnore]
    public bool Success 
    { 
        get => IsSuccessful; 
        set => IsSuccessful = value; 
    }

    /// <summary>
    /// رسالة الخطأ (في حالة الفشل)
    /// Error message (if failed)
    /// </summary>
    [JsonProperty("error_message")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// تاريخ تنفيذ البحث
    /// Search execution time
    /// </summary>
    [JsonProperty("executed_at")]
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// إحصائيات البحث
/// Search statistics
/// </summary>
public class SearchStatistics
{
    /// <summary>
    /// العدد الإجمالي للنتائج
    /// Total count of results
    /// </summary>
    [JsonProperty("total_count")]
    public int TotalCount { get; set; }

    /// <summary>
    /// رقم الصفحة الحالية
    /// Current page number
    /// </summary>
    [JsonProperty("page_number")]
    public int PageNumber { get; set; }

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    [JsonProperty("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// وقت التنفيذ بالميلي ثانية
    /// Execution time in milliseconds
    /// </summary>
    [JsonProperty("execution_time_ms")]
    public double ExecutionTimeMs { get; set; }

    /// <summary>
    /// الفهارس المستخدمة
    /// Used indices
    /// </summary>
    [JsonProperty("used_indices")]
    public List<string> UsedIndices { get; set; } = new();

    /// <summary>
    /// عدد الفهارس المستخدمة
    /// Number of indices used
    /// </summary>
    [JsonProperty("indices_used_count")]
    public int IndicesUsedCount { get; set; }

    /// <summary>
    /// عدد العناصر المفحوصة
    /// Number of items examined
    /// </summary>
    [JsonProperty("items_examined")]
    public int ItemsExamined { get; set; }

    /// <summary>
    /// عدد العناصر المطابقة
    /// Number of items matched
    /// </summary>
    [JsonProperty("items_matched")]
    public int ItemsMatched { get; set; }

    /// <summary>
    /// نسبة الكفاءة
    /// Efficiency ratio
    /// </summary>
    [JsonProperty("efficiency_ratio")]
    public double EfficiencyRatio { get; set; }

    /// <summary>
    /// وقت البحث في كل فهرس
    /// Search time per index
    /// </summary>
    [JsonProperty("per_index_time")]
    public Dictionary<string, double> PerIndexTime { get; set; } = new();

    /// <summary>
    /// تفاصيل الأداء
    /// Performance details
    /// </summary>
    [JsonProperty("performance_details")]
    public Dictionary<string, object> PerformanceDetails { get; set; } = new();
}

/// <summary>
/// عنصر نتيجة البحث
/// Search result item
/// </summary>
public class SearchResultItem
{
    /// <summary>
    /// معرف العنصر
    /// Item identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// بيانات العنصر
    /// Item data
    /// </summary>
    [JsonProperty("data")]
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// العنصر (اختصار للوصول السريع)
    /// Item (shortcut for quick access)
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, object> Item 
    { 
        get => Data; 
        set => Data = value; 
    }

    /// <summary>
    /// نقاط المطابقة
    /// Match score
    /// </summary>
    [JsonProperty("match_score")]
    public double MatchScore { get; set; }

    /// <summary>
    /// النقاط (اختصار للوصول السريع)
    /// Score (shortcut for quick access)
    /// </summary>
    [JsonIgnore]
    public double Score 
    { 
        get => MatchScore; 
        set => MatchScore = value; 
    }

    /// <summary>
    /// الحقول المطابقة
    /// Matched fields
    /// </summary>
    [JsonProperty("matched_fields")]
    public List<string> MatchedFields { get; set; } = new();

    /// <summary>
    /// تفاصيل المطابقة
    /// Match details
    /// </summary>
    [JsonProperty("match_details")]
    public Dictionary<string, object> MatchDetails { get; set; } = new();

    /// <summary>
    /// الفهرس المصدر
    /// Source index
    /// </summary>
    [JsonProperty("source_index")]
    public string SourceIndex { get; set; } = string.Empty;

    /// <summary>
    /// ترتيب العنصر في النتائج
    /// Item rank in results
    /// </summary>
    [JsonProperty("rank")]
    public int Rank { get; set; }
}