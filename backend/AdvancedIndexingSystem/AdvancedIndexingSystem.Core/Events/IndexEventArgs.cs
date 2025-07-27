using AdvancedIndexingSystem.Core.Models;

namespace AdvancedIndexingSystem.Core.Events;

/// <summary>
/// معطيات حدث تغيير حالة الفهرس
/// Index status changed event arguments
/// </summary>
public class IndexStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// معرف الفهرس
    /// Index identifier
    /// </summary>
    public string IndexId { get; set; } = string.Empty;

    /// <summary>
    /// اسم الفهرس
    /// Index name
    /// </summary>
    public string IndexName { get; set; } = string.Empty;

    /// <summary>
    /// الحالة السابقة
    /// Previous status
    /// </summary>
    public IndexStatus PreviousStatus { get; set; }

    /// <summary>
    /// الحالة الجديدة
    /// New status
    /// </summary>
    public IndexStatus NewStatus { get; set; }

    /// <summary>
    /// سبب التغيير
    /// Change reason
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ التغيير
    /// Change timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// بيانات إضافية
    /// Additional data
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// منشئ معطيات حدث تغيير حالة الفهرس
    /// Constructor for index status changed event arguments
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index identifier</param>
    /// <param name="indexName">اسم الفهرس - Index name</param>
    /// <param name="previousStatus">الحالة السابقة - Previous status</param>
    /// <param name="newStatus">الحالة الجديدة - New status</param>
    /// <param name="reason">سبب التغيير - Change reason</param>
    public IndexStatusChangedEventArgs(string indexId, string indexName, IndexStatus previousStatus, IndexStatus newStatus, string reason)
    {
        IndexId = indexId;
        IndexName = indexName;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}

/// <summary>
/// معطيات حدث عنصر الفهرس
/// Index item event arguments
/// </summary>
/// <typeparam name="T">نوع العنصر - Item type</typeparam>
public class IndexItemEventArgs<T> : EventArgs
{
    /// <summary>
    /// معرف الفهرس
    /// Index identifier
    /// </summary>
    public string IndexId { get; set; } = string.Empty;

    /// <summary>
    /// اسم الفهرس
    /// Index name
    /// </summary>
    public string IndexName { get; set; } = string.Empty;

    /// <summary>
    /// معرف العنصر
    /// Item identifier
    /// </summary>
    public string ItemId { get; set; } = string.Empty;

    /// <summary>
    /// العنصر
    /// Item
    /// </summary>
    public T? Item { get; set; }

    /// <summary>
    /// العنصر السابق (في حالة التحديث)
    /// Previous item (for updates)
    /// </summary>
    public T? PreviousItem { get; set; }

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// وقت تنفيذ العملية (بالميلي ثانية)
    /// Operation execution time (milliseconds)
    /// </summary>
    public double ExecutionTimeMs { get; set; }

    /// <summary>
    /// الحقول المتأثرة (في حالة التحديث)
    /// Affected fields (for updates)
    /// </summary>
    public List<string> AffectedFields { get; set; } = new();

    /// <summary>
    /// بيانات إضافية
    /// Additional data
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// منشئ معطيات حدث عنصر الفهرس
    /// Constructor for index item event arguments
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index identifier</param>
    /// <param name="indexName">اسم الفهرس - Index name</param>
    /// <param name="itemId">معرف العنصر - Item identifier</param>
    /// <param name="item">العنصر - Item</param>
    /// <param name="operationType">نوع العملية - Operation type</param>
    public IndexItemEventArgs(string indexId, string indexName, string itemId, T? item, UpdateOperationType operationType)
    {
        IndexId = indexId;
        IndexName = indexName;
        ItemId = itemId;
        Item = item;
        OperationType = operationType;
    }

    /// <summary>
    /// منشئ معطيات حدث عنصر الفهرس مع العنصر السابق
    /// Constructor for index item event arguments with previous item
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index identifier</param>
    /// <param name="indexName">اسم الفهرس - Index name</param>
    /// <param name="itemId">معرف العنصر - Item identifier</param>
    /// <param name="item">العنصر - Item</param>
    /// <param name="previousItem">العنصر السابق - Previous item</param>
    /// <param name="operationType">نوع العملية - Operation type</param>
    public IndexItemEventArgs(string indexId, string indexName, string itemId, T? item, T? previousItem, UpdateOperationType operationType)
        : this(indexId, indexName, itemId, item, operationType)
    {
        PreviousItem = previousItem;
    }
}

/// <summary>
/// معطيات حدث خطأ الفهرس
/// Index error event arguments
/// </summary>
public class IndexErrorEventArgs : EventArgs
{
    /// <summary>
    /// معرف الفهرس
    /// Index identifier
    /// </summary>
    public string IndexId { get; set; } = string.Empty;

    /// <summary>
    /// اسم الفهرس
    /// Index name
    /// </summary>
    public string IndexName { get; set; } = string.Empty;

    /// <summary>
    /// رسالة الخطأ
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// الاستثناء
    /// Exception
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// نوع الخطأ
    /// Error type
    /// </summary>
    public IndexErrorType ErrorType { get; set; }

    /// <summary>
    /// مستوى الخطورة
    /// Severity level
    /// </summary>
    public ErrorSeverity Severity { get; set; }

    /// <summary>
    /// العملية التي تسببت في الخطأ
    /// Operation that caused the error
    /// </summary>
    public string? Operation { get; set; }

    /// <summary>
    /// معرف العنصر المتأثر (إن وجد)
    /// Affected item identifier (if any)
    /// </summary>
    public string? AffectedItemId { get; set; }

    /// <summary>
    /// تاريخ حدوث الخطأ
    /// Error timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// معلومات إضافية عن الخطأ
    /// Additional error information
    /// </summary>
    public Dictionary<string, object> AdditionalInfo { get; set; } = new();

    /// <summary>
    /// هل يمكن إعادة المحاولة
    /// Is retryable
    /// </summary>
    public bool IsRetryable { get; set; }

    /// <summary>
    /// عدد المحاولات المتبقية
    /// Remaining retry attempts
    /// </summary>
    public int RemainingRetries { get; set; }

    /// <summary>
    /// منشئ معطيات حدث خطأ الفهرس
    /// Constructor for index error event arguments
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index identifier</param>
    /// <param name="indexName">اسم الفهرس - Index name</param>
    /// <param name="errorMessage">رسالة الخطأ - Error message</param>
    /// <param name="errorType">نوع الخطأ - Error type</param>
    /// <param name="severity">مستوى الخطورة - Severity level</param>
    public IndexErrorEventArgs(string indexId, string indexName, string errorMessage, IndexErrorType errorType, ErrorSeverity severity)
    {
        IndexId = indexId;
        IndexName = indexName;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        Severity = severity;
    }

    /// <summary>
    /// منشئ معطيات حدث خطأ الفهرس مع الاستثناء
    /// Constructor for index error event arguments with exception
    /// </summary>
    /// <param name="indexId">معرف الفهرس - Index identifier</param>
    /// <param name="indexName">اسم الفهرس - Index name</param>
    /// <param name="errorMessage">رسالة الخطأ - Error message</param>
    /// <param name="exception">الاستثناء - Exception</param>
    /// <param name="errorType">نوع الخطأ - Error type</param>
    /// <param name="severity">مستوى الخطورة - Severity level</param>
    public IndexErrorEventArgs(string indexId, string indexName, string errorMessage, Exception exception, IndexErrorType errorType, ErrorSeverity severity)
        : this(indexId, indexName, errorMessage, errorType, severity)
    {
        Exception = exception;
    }
}

/// <summary>
/// نوع خطأ الفهرس
/// Index error type
/// </summary>
public enum IndexErrorType
{
    /// <summary>
    /// خطأ في التهيئة - Initialization Error
    /// </summary>
    InitializationError,

    /// <summary>
    /// مفتاح مكرر - Duplicate Key
    /// </summary>
    DuplicateKey,

    /// <summary>
    /// عنصر غير موجود - Item Not Found
    /// </summary>
    ItemNotFound,

    /// <summary>
    /// خطأ في العملية - Operation Error
    /// </summary>
    OperationError,

    /// <summary>
    /// خطأ في عمليات الملفات - File Operation Error
    /// </summary>
    FileOperationError,

    /// <summary>
    /// خطأ في التخلص من الموارد - Dispose Error
    /// </summary>
    DisposeError,

    /// <summary>
    /// خطأ في الإضافة - Add Error
    /// </summary>
    AddError,

    /// <summary>
    /// خطأ في التحديث - Update Error
    /// </summary>
    UpdateError,

    /// <summary>
    /// خطأ في الحذف - Remove Error
    /// </summary>
    RemoveError,

    /// <summary>
    /// خطأ في البحث - Search Error
    /// </summary>
    SearchError,

    /// <summary>
    /// خطأ في التحميل - Load Error
    /// </summary>
    LoadError,

    /// <summary>
    /// خطأ في الحفظ - Save Error
    /// </summary>
    SaveError,

    /// <summary>
    /// خطأ في إعادة البناء - Rebuild Error
    /// </summary>
    RebuildError,

    /// <summary>
    /// خطأ في التحسين - Optimization Error
    /// </summary>
    OptimizationError,

    /// <summary>
    /// خطأ في التحقق - Validation Error
    /// </summary>
    ValidationError,

    /// <summary>
    /// خطأ في التكوين - Configuration Error
    /// </summary>
    ConfigurationError,

    /// <summary>
    /// خطأ في الذاكرة - Memory Error
    /// </summary>
    MemoryError,

    /// <summary>
    /// خطأ في القرص - Disk Error
    /// </summary>
    DiskError,

    /// <summary>
    /// خطأ في الشبكة - Network Error
    /// </summary>
    NetworkError,

    /// <summary>
    /// خطأ عام - General Error
    /// </summary>
    GeneralError
}

/// <summary>
/// مستوى خطورة الخطأ
/// Error severity level
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// معلومات - Info
    /// </summary>
    Info,

    /// <summary>
    /// تحذير - Warning
    /// </summary>
    Warning,

    /// <summary>
    /// خطأ - Error
    /// </summary>
    Error,

    /// <summary>
    /// خطأ حرج - Critical
    /// </summary>
    Critical,

    /// <summary>
    /// خطأ كارثي - Fatal
    /// </summary>
    Fatal
}

/// <summary>
/// معطيات حدث تقدم العملية
/// Operation progress event arguments
/// </summary>
public class OperationProgressEventArgs : EventArgs
{
    /// <summary>
    /// معرف العملية
    /// Operation identifier
    /// </summary>
    public string OperationId { get; set; } = string.Empty;

    /// <summary>
    /// اسم العملية
    /// Operation name
    /// </summary>
    public string OperationName { get; set; } = string.Empty;

    /// <summary>
    /// معرف الفهرس
    /// Index identifier
    /// </summary>
    public string IndexId { get; set; } = string.Empty;

    /// <summary>
    /// نسبة التقدم (0-100)
    /// Progress percentage (0-100)
    /// </summary>
    public double ProgressPercentage { get; set; }

    /// <summary>
    /// العناصر المعالجة
    /// Items processed
    /// </summary>
    public int ItemsProcessed { get; set; }

    /// <summary>
    /// إجمالي العناصر
    /// Total items
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// الوقت المنقضي (بالميلي ثانية)
    /// Elapsed time (milliseconds)
    /// </summary>
    public double ElapsedTimeMs { get; set; }

    /// <summary>
    /// الوقت المتوقع للانتهاء (بالميلي ثانية)
    /// Estimated time to completion (milliseconds)
    /// </summary>
    public double EstimatedTimeToCompletionMs { get; set; }

    /// <summary>
    /// رسالة الحالة الحالية
    /// Current status message
    /// </summary>
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// العنصر الحالي قيد المعالجة
    /// Current item being processed
    /// </summary>
    public string? CurrentItem { get; set; }

    /// <summary>
    /// معدل المعالجة (عناصر في الثانية)
    /// Processing rate (items per second)
    /// </summary>
    public double ProcessingRate { get; set; }

    /// <summary>
    /// تاريخ التحديث
    /// Update timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// بيانات إضافية
    /// Additional data
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// منشئ معطيات حدث تقدم العملية
    /// Constructor for operation progress event arguments
    /// </summary>
    /// <param name="operationId">معرف العملية - Operation identifier</param>
    /// <param name="operationName">اسم العملية - Operation name</param>
    /// <param name="indexId">معرف الفهرس - Index identifier</param>
    /// <param name="itemsProcessed">العناصر المعالجة - Items processed</param>
    /// <param name="totalItems">إجمالي العناصر - Total items</param>
    public OperationProgressEventArgs(string operationId, string operationName, string indexId, int itemsProcessed, int totalItems)
    {
        OperationId = operationId;
        OperationName = operationName;
        IndexId = indexId;
        ItemsProcessed = itemsProcessed;
        TotalItems = totalItems;
        ProgressPercentage = totalItems > 0 ? (double)itemsProcessed / totalItems * 100 : 0;
    }
}

/// <summary>
/// معطيات حدث فهرسة العقار
/// Property indexing event arguments
/// </summary>
public class PropertyIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف العقار
    /// Property identifier
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// اسم العقار
    /// Property name
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// المدينة
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// نوع العقار
    /// Property type
    /// </summary>
    public string PropertyType { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// الحقول المتأثرة
    /// Affected fields
    /// </summary>
    public List<string> AffectedFields { get; set; } = new();

    /// <summary>
    /// البيانات الإضافية للفهرسة
    /// Additional indexing data
    /// </summary>
    public Dictionary<string, object> IndexingData { get; set; } = new();

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ أحداث فهرسة العقار
    /// Constructor for property indexing event arguments
    /// </summary>
    /// <param name="propertyId">معرف العقار</param>
    /// <param name="propertyName">اسم العقار</param>
    /// <param name="city">المدينة</param>
    /// <param name="propertyType">نوع العقار</param>
    /// <param name="operationType">نوع العملية</param>
    public PropertyIndexingEventArgs(Guid propertyId, string propertyName, string city, string propertyType, UpdateOperationType operationType)
    {
        PropertyId = propertyId;
        PropertyName = propertyName;
        City = city;
        PropertyType = propertyType;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة الوحدة
/// Unit indexing event arguments
/// </summary>
public class UnitIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// معرف العقار المرتبط
    /// Associated property identifier
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// اسم الوحدة
    /// Unit name
    /// </summary>
    public string UnitName { get; set; } = string.Empty;

    /// <summary>
    /// نوع الوحدة
    /// Unit type
    /// </summary>
    public string UnitType { get; set; } = string.Empty;

    /// <summary>
    /// السعر الأساسي
    /// Base price
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// الحقول الديناميكية المحدثة
    /// Updated dynamic fields
    /// </summary>
    public Dictionary<string, object> DynamicFields { get; set; } = new();

    /// <summary>
    /// قواعد التسعير المرتبطة
    /// Associated pricing rules
    /// </summary>
    public List<object> PricingRules { get; set; } = new();

    /// <summary>
    /// الإتاحة المرتبطة
    /// Associated availability
    /// </summary>
    public List<object> Availability { get; set; } = new();

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ أحداث فهرسة الوحدة
    /// Constructor for unit indexing event arguments
    /// </summary>
    public UnitIndexingEventArgs(Guid unitId, Guid propertyId, string unitName, string unitType, decimal basePrice, string currency, UpdateOperationType operationType)
    {
        UnitId = unitId;
        PropertyId = propertyId;
        UnitName = unitName;
        UnitType = unitType;
        BasePrice = basePrice;
        Currency = currency;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة التسعير
/// Pricing indexing event arguments
/// </summary>
public class PricingIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف قاعدة التسعير
    /// Pricing rule identifier
    /// </summary>
    public Guid PricingRuleId { get; set; }

    /// <summary>
    /// معرف الوحدة المرتبطة
    /// Associated unit identifier
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// نوع السعر
    /// Price type
    /// </summary>
    public string PriceType { get; set; } = string.Empty;

    /// <summary>
    /// مبلغ السعر
    /// Price amount
    /// </summary>
    public decimal PriceAmount { get; set; }

    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ أحداث فهرسة التسعير
    /// Constructor for pricing indexing event arguments
    /// </summary>
    public PricingIndexingEventArgs(Guid pricingRuleId, Guid unitId, string priceType, decimal priceAmount, string currency, DateTime startDate, DateTime endDate, UpdateOperationType operationType)
    {
        PricingRuleId = pricingRuleId;
        UnitId = unitId;
        PriceType = priceType;
        PriceAmount = priceAmount;
        Currency = currency;
        StartDate = startDate;
        EndDate = endDate;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة الإتاحة
/// Availability indexing event arguments
/// </summary>
public class AvailabilityIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف الإتاحة
    /// Availability identifier
    /// </summary>
    public Guid AvailabilityId { get; set; }

    /// <summary>
    /// معرف الوحدة المرتبطة
    /// Associated unit identifier
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// تاريخ البداية
    /// Start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// حالة الإتاحة
    /// Availability status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ أحداث فهرسة الإتاحة
    /// Constructor for availability indexing event arguments
    /// </summary>
    public AvailabilityIndexingEventArgs(Guid availabilityId, Guid unitId, DateTime startDate, DateTime endDate, string status, UpdateOperationType operationType)
    {
        AvailabilityId = availabilityId;
        UnitId = unitId;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        OperationType = operationType;
    }
}



/// <summary>
/// معطيات حدث فهرسة الحقول الديناميكية
/// Dynamic field indexing event arguments
/// </summary>
public class DynamicFieldIndexingEventArgs : EventArgs
{
    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// قيمة الحقل
    /// Field value
    /// </summary>
    public string FieldValue { get; set; } = string.Empty;

    /// <summary>
    /// نوع الحقل
    /// Field type
    /// </summary>
    public string FieldType { get; set; } = string.Empty;

    /// <summary>
    /// معرف الوحدة المرتبطة
    /// Associated unit identifier
    /// </summary>
    public string UnitId { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ أحداث فهرسة الحقول الديناميكية
    /// Constructor for dynamic field indexing event arguments
    /// </summary>
    public DynamicFieldIndexingEventArgs(string fieldName, string fieldValue, string fieldType, string unitId, UpdateOperationType operationType)
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
        FieldType = fieldType;
        UnitId = unitId;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة نوع العقار
/// Property type indexing event arguments
/// </summary>
public class PropertyTypeIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف نوع العقار
    /// Property type identifier
    /// </summary>
    public Guid PropertyTypeId { get; set; }

    /// <summary>
    /// اسم نوع العقار
    /// Property type name
    /// </summary>
    public string PropertyTypeName { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ معطيات حدث فهرسة نوع العقار
    /// Property type indexing event arguments constructor
    /// </summary>
    public PropertyTypeIndexingEventArgs(Guid propertyTypeId, string propertyTypeName, UpdateOperationType operationType)
    {
        PropertyTypeId = propertyTypeId;
        PropertyTypeName = propertyTypeName;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة نوع الوحدة
/// Unit type indexing event arguments
/// </summary>
public class UnitTypeIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type identifier
    /// </summary>
    public Guid UnitTypeId { get; set; }

    /// <summary>
    /// اسم نوع الوحدة
    /// Unit type name
    /// </summary>
    public string UnitTypeName { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ معطيات حدث فهرسة نوع الوحدة
    /// Unit type indexing event arguments constructor
    /// </summary>
    public UnitTypeIndexingEventArgs(Guid unitTypeId, string unitTypeName, UpdateOperationType operationType)
    {
        UnitTypeId = unitTypeId;
        UnitTypeName = unitTypeName;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة المدينة
/// City indexing event arguments
/// </summary>
public class CityIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف المدينة
    /// City identifier
    /// </summary>
    public Guid CityId { get; set; }

    /// <summary>
    /// اسم المدينة
    /// City name
    /// </summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ معطيات حدث فهرسة المدينة
    /// City indexing event arguments constructor
    /// </summary>
    public CityIndexingEventArgs(Guid cityId, string cityName, UpdateOperationType operationType)
    {
        CityId = cityId;
        CityName = cityName;
        OperationType = operationType;
    }
}

/// <summary>
/// معطيات حدث فهرسة ربط المرفق بالعقار
/// Facility property indexing event arguments
/// </summary>
public class FacilityPropertyIndexingEventArgs : EventArgs
{
    /// <summary>
    /// معرف العقار
    /// Property identifier
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// معرف المرفق
    /// Facility identifier
    /// </summary>
    public Guid FacilityId { get; set; }

    /// <summary>
    /// اسم المرفق
    /// Facility name
    /// </summary>
    public string FacilityName { get; set; } = string.Empty;

    /// <summary>
    /// نوع العملية
    /// Operation type
    /// </summary>
    public UpdateOperationType OperationType { get; set; }

    /// <summary>
    /// تاريخ العملية
    /// Operation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// منشئ معطيات حدث فهرسة ربط المرفق بالعقار
    /// Facility property indexing event arguments constructor
    /// </summary>
    public FacilityPropertyIndexingEventArgs(Guid propertyId, Guid facilityId, string facilityName, UpdateOperationType operationType)
    {
        PropertyId = propertyId;
        FacilityId = facilityId;
        FacilityName = facilityName;
        OperationType = operationType;
    }
}