using Newtonsoft.Json;

namespace AdvancedIndexingSystem.Core.Models;

/// <summary>
/// تكوين الفهرس الأساسي
/// Base index configuration
/// </summary>
public class IndexConfiguration
{
    /// <summary>
    /// معرف الفهرس الفريد
    /// Unique index identifier
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
    /// الاسم العربي للفهرس
    /// Arabic name of the index
    /// </summary>
    [JsonProperty("arabic_name")]
    public string ArabicName { get; set; } = string.Empty;

    /// <summary>
    /// وصف الفهرس
    /// Index description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// نوع الفهرس
    /// Index type
    /// </summary>
    [JsonProperty("index_type")]
    public IndexType IndexType { get; set; }

    /// <summary>
    /// أولوية الفهرس
    /// Index priority
    /// </summary>
    [JsonProperty("priority")]
    public IndexPriority Priority { get; set; } = IndexPriority.Medium;

    /// <summary>
    /// حالة الفهرس
    /// Index status
    /// </summary>
    [JsonProperty("status")]
    public IndexStatus Status { get; set; } = IndexStatus.Building;

    /// <summary>
    /// تاريخ الإنشاء
    /// Creation date
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last update date
    /// </summary>
    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// هل الفهرس مفعل
    /// Is index enabled
    /// </summary>
    [JsonProperty("is_enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// هل يتم التحديث التلقائي
    /// Auto update enabled
    /// </summary>
    [JsonProperty("auto_update")]
    public bool AutoUpdate { get; set; } = true;

    /// <summary>
    /// الحد الأقصى لعدد العناصر في الفهرس
    /// Maximum number of items in index
    /// </summary>
    [JsonProperty("max_items")]
    public int MaxItems { get; set; } = 1000000;

    /// <summary>
    /// إعدادات إضافية مخصصة
    /// Custom additional settings
    /// </summary>
    [JsonProperty("custom_settings")]
    public Dictionary<string, object> CustomSettings { get; set; } = new();

    /// <summary>
    /// قائمة الحقول المفهرسة
    /// List of indexed fields
    /// </summary>
    [JsonProperty("indexed_fields")]
    public List<string> IndexedFields { get; set; } = new();

    /// <summary>
    /// قائمة الحقول الديناميكية
    /// List of dynamic fields
    /// </summary>
    [JsonProperty("dynamic_fields")]
    public List<DynamicFieldConfiguration>? DynamicFields { get; set; }

    /// <summary>
    /// معايير الأداء
    /// Performance metrics
    /// </summary>
    [JsonProperty("performance_metrics")]
    public PerformanceMetrics PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// تكوين الحقل الديناميكي
/// Dynamic field configuration
/// </summary>
public class DynamicFieldConfiguration
{
    /// <summary>
    /// معرف الحقل الفريد
    /// Unique field identifier
    /// </summary>
    [JsonProperty("field_id")]
    public string FieldId { get; set; } = string.Empty;

    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    [JsonProperty("field_name")]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// الاسم العربي للحقل
    /// Arabic field name
    /// </summary>
    [JsonProperty("arabic_name")]
    public string ArabicName { get; set; } = string.Empty;

    /// <summary>
    /// نوع بيانات الحقل
    /// Field data type
    /// </summary>
    [JsonProperty("data_type")]
    public FieldDataType DataType { get; set; }

    /// <summary>
    /// هل الحقل مطلوب
    /// Is field required
    /// </summary>
    [JsonProperty("is_required")]
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// هل الحقل قابل للبحث
    /// Is field searchable
    /// </summary>
    [JsonProperty("is_searchable")]
    public bool IsSearchable { get; set; } = true;

    /// <summary>
    /// هل الحقل قابل للفرز
    /// Is field sortable
    /// </summary>
    [JsonProperty("is_sortable")]
    public bool IsSortable { get; set; } = false;

    /// <summary>
    /// القيم المسموحة (للحقول من نوع Select)
    /// Allowed values (for Select type fields)
    /// </summary>
    [JsonProperty("allowed_values")]
    public List<string> AllowedValues { get; set; } = new();

    /// <summary>
    /// القيمة الافتراضية
    /// Default value
    /// </summary>
    [JsonProperty("default_value")]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// قواعد التحقق
    /// Validation rules
    /// </summary>
    [JsonProperty("validation_rules")]
    public ValidationRules ValidationRules { get; set; } = new();

    /// <summary>
    /// إعدادات النطاق (للحقول الرقمية والتواريخ)
    /// Range settings (for numeric and date fields)
    /// </summary>
    [JsonProperty("range_settings")]
    public RangeSettings RangeSettings { get; set; } = new();
}

/// <summary>
/// قواعد التحقق
/// Validation rules
/// </summary>
public class ValidationRules
{
    /// <summary>
    /// الحد الأدنى للطول (للنصوص)
    /// Minimum length (for text)
    /// </summary>
    [JsonProperty("min_length")]
    public int? MinLength { get; set; }

    /// <summary>
    /// الحد الأقصى للطول (للنصوص)
    /// Maximum length (for text)
    /// </summary>
    [JsonProperty("max_length")]
    public int? MaxLength { get; set; }

    /// <summary>
    /// الحد الأدنى للقيمة (للأرقام)
    /// Minimum value (for numbers)
    /// </summary>
    [JsonProperty("min_value")]
    public double? MinValue { get; set; }

    /// <summary>
    /// الحد الأقصى للقيمة (للأرقام)
    /// Maximum value (for numbers)
    /// </summary>
    [JsonProperty("max_value")]
    public double? MaxValue { get; set; }

    /// <summary>
    /// نمط التعبير النمطي
    /// Regular expression pattern
    /// </summary>
    [JsonProperty("regex_pattern")]
    public string? RegexPattern { get; set; }

    /// <summary>
    /// رسالة خطأ مخصصة
    /// Custom error message
    /// </summary>
    [JsonProperty("error_message")]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// إعدادات النطاق
/// Range settings
/// </summary>
public class RangeSettings
{
    /// <summary>
    /// نطاقات مخصصة للفهرسة
    /// Custom ranges for indexing
    /// </summary>
    [JsonProperty("custom_ranges")]
    public List<RangeDefinition> CustomRanges { get; set; } = new();

    /// <summary>
    /// حجم النطاق التلقائي
    /// Automatic range size
    /// </summary>
    [JsonProperty("auto_range_size")]
    public double? AutoRangeSize { get; set; }

    /// <summary>
    /// هل يتم إنشاء النطاقات تلقائياً
    /// Auto generate ranges
    /// </summary>
    [JsonProperty("auto_generate_ranges")]
    public bool AutoGenerateRanges { get; set; } = true;
}

/// <summary>
/// تعريف النطاق
/// Range definition
/// </summary>
public class RangeDefinition
{
    /// <summary>
    /// اسم النطاق
    /// Range name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// القيمة الدنيا
    /// Minimum value
    /// </summary>
    [JsonProperty("min_value")]
    public object MinValue { get; set; } = null!;

    /// <summary>
    /// القيمة العليا
    /// Maximum value
    /// </summary>
    [JsonProperty("max_value")]
    public object MaxValue { get; set; } = null!;

    /// <summary>
    /// هل النطاق شامل للحد الأدنى
    /// Is minimum inclusive
    /// </summary>
    [JsonProperty("min_inclusive")]
    public bool MinInclusive { get; set; } = true;

    /// <summary>
    /// هل النطاق شامل للحد الأقصى
    /// Is maximum inclusive
    /// </summary>
    [JsonProperty("max_inclusive")]
    public bool MaxInclusive { get; set; } = false;
}