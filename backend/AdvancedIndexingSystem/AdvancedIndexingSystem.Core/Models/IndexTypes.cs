using System.ComponentModel;

namespace AdvancedIndexingSystem.Core.Models;

/// <summary>
/// أنواع الفهارس المدعومة في النظام
/// Supported index types in the system
/// </summary>
public enum IndexType
{
    /// <summary>
    /// فهرس الأسعار - Price Index
    /// </summary>
    [Description("فهرس الأسعار")]
    PriceIndex,

    /// <summary>
    /// فهرس المدن - City Index
    /// </summary>
    [Description("فهرس المدن")]
    CityIndex,

    /// <summary>
    /// فهرس المرافق - Amenity Index
    /// </summary>
    [Description("فهرس المرافق")]
    AmenityIndex,

    /// <summary>
    /// فهرس الحقول الديناميكية - Dynamic Field Index
    /// </summary>
    [Description("فهرس الحقول الديناميكية")]
    DynamicFieldIndex,

    /// <summary>
    /// فهرس ديناميكي - Dynamic Index
    /// </summary>
    [Description("فهرس ديناميكي")]
    Dynamic,

    /// <summary>
    /// فهرس الهاش - Hash Index
    /// </summary>
    [Description("فهرس الهاش")]
    Hash,

    /// <summary>
    /// فهرس النصوص - Text Index
    /// </summary>
    [Description("فهرس النصوص")]
    TextIndex,

    /// <summary>
    /// فهرس التواريخ - Date Index
    /// </summary>
    [Description("فهرس التواريخ")]
    DateIndex,

    /// <summary>
    /// فهرس منطقي - Boolean Index
    /// </summary>
    [Description("فهرس منطقي")]
    BooleanIndex,

    /// <summary>
    /// فهرس مخصص - Custom Index
    /// </summary>
    [Description("فهرس مخصص")]
    CustomIndex
}

/// <summary>
/// أنواع البيانات المدعومة للحقول الديناميكية
/// Supported data types for dynamic fields
/// </summary>
public enum FieldDataType
{
    /// <summary>
    /// نص - Text
    /// </summary>
    [Description("نص")]
    Text,

    /// <summary>
    /// رقم - Number
    /// </summary>
    [Description("رقم")]
    Number,

    /// <summary>
    /// تاريخ - Date
    /// </summary>
    [Description("تاريخ")]
    Date,

    /// <summary>
    /// منطقي - Boolean
    /// </summary>
    [Description("منطقي")]
    Boolean,

    /// <summary>
    /// قائمة اختيار - Select List
    /// </summary>
    [Description("قائمة اختيار")]
    Select,

    /// <summary>
    /// قائمة متعددة الاختيار - Multi Select
    /// </summary>
    [Description("قائمة متعددة الاختيار")]
    MultiSelect,

    /// <summary>
    /// نطاق رقمي - Numeric Range
    /// </summary>
    [Description("نطاق رقمي")]
    NumericRange,

    /// <summary>
    /// نطاق تاريخ - Date Range
    /// </summary>
    [Description("نطاق تاريخ")]
    DateRange
}

/// <summary>
/// أولوية الفهرس
/// Index priority levels
/// </summary>
public enum IndexPriority
{
    /// <summary>
    /// منخفض - Low
    /// </summary>
    [Description("منخفض")]
    Low = 1,

    /// <summary>
    /// متوسط - Medium
    /// </summary>
    [Description("متوسط")]
    Medium = 2,

    /// <summary>
    /// عالي - High
    /// </summary>
    [Description("عالي")]
    High = 3,

    /// <summary>
    /// حرج - Critical
    /// </summary>
    [Description("حرج")]
    Critical = 4
}

/// <summary>
/// حالة الفهرس
/// Index status
/// </summary>
public enum IndexStatus
{
    /// <summary>
    /// جاري التهيئة - Initializing
    /// </summary>
    [Description("جاري التهيئة")]
    Initializing,

    /// <summary>
    /// جاري الإنشاء - Building
    /// </summary>
    [Description("جاري الإنشاء")]
    Building,

    /// <summary>
    /// نشط - Active
    /// </summary>
    [Description("نشط")]
    Active,

    /// <summary>
    /// معطل - Disabled
    /// </summary>
    [Description("معطل")]
    Disabled,

    /// <summary>
    /// خطأ - Error
    /// </summary>
    [Description("خطأ")]
    Error,

    /// <summary>
    /// جاري التحديث - Updating
    /// </summary>
    [Description("جاري التحديث")]
    Updating,

    /// <summary>
    /// جاري إعادة البناء - Rebuilding
    /// </summary>
    [Description("جاري إعادة البناء")]
    Rebuilding,

    /// <summary>
    /// محذوف - Removed
    /// </summary>
    [Description("محذوف")]
    Removed,

    /// <summary>
    /// تم التخلص منه - Disposed
    /// </summary>
    [Description("تم التخلص منه")]
    Disposed
}

/// <summary>
/// نوع عملية التحديث
/// Update operation type
/// </summary>
public enum UpdateOperationType
{
    /// <summary>
    /// إضافة - Add
    /// </summary>
    [Description("إضافة")]
    Add,

    /// <summary>
    /// تحديث - Update
    /// </summary>
    [Description("تحديث")]
    Update,

    /// <summary>
    /// حذف - Delete
    /// </summary>
    [Description("حذف")]
    Delete,

    /// <summary>
    /// إزالة - Remove
    /// </summary>
    [Description("إزالة")]
    Remove,

    /// <summary>
    /// إعادة بناء كامل - Full Rebuild
    /// </summary>
    [Description("إعادة بناء كامل")]
    FullRebuild,

    /// <summary>
    /// تحديث تدريجي - Incremental Update
    /// </summary>
    [Description("تحديث تدريجي")]
    IncrementalUpdate
}