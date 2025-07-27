using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Events;

namespace AdvancedIndexingSystem.Core.Interfaces;

/// <summary>
/// الواجهة الأساسية للفهرس المتقدم
/// Base interface for advanced index
/// </summary>
/// <typeparam name="T">نوع البيانات المفهرسة - Type of indexed data</typeparam>
public interface IAdvancedIndex<T> : IDisposable where T : class
{
    #region الخصائص - Properties

    /// <summary>
    /// تكوين الفهرس
    /// Index configuration
    /// </summary>
    IndexConfiguration Configuration { get; }

    /// <summary>
    /// حالة الفهرس الحالية
    /// Current index status
    /// </summary>
    IndexStatus Status { get; }

    /// <summary>
    /// عدد العناصر المفهرسة
    /// Count of indexed items
    /// </summary>
    int ItemCount { get; }

    /// <summary>
    /// حجم الفهرس بالبايت
    /// Index size in bytes
    /// </summary>
    long IndexSize { get; }

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last update timestamp
    /// </summary>
    DateTime LastUpdated { get; }

    #endregion

    #region العمليات الأساسية - Basic Operations

    /// <summary>
    /// إضافة عنصر جديد إلى الفهرس
    /// Add new item to index
    /// </summary>
    /// <param name="id">معرف العنصر - Item ID</param>
    /// <param name="item">العنصر المراد فهرسته - Item to index</param>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> AddItemAsync(string id, T item);

    /// <summary>
    /// تحديث عنصر موجود في الفهرس
    /// Update existing item in index
    /// </summary>
    /// <param name="id">معرف العنصر - Item ID</param>
    /// <param name="item">العنصر المحدث - Updated item</param>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> UpdateItemAsync(string id, T item);

    /// <summary>
    /// حذف عنصر من الفهرس
    /// Remove item from index
    /// </summary>
    /// <param name="id">معرف العنصر - Item ID</param>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> RemoveItemAsync(string id);

    /// <summary>
    /// مسح جميع العناصر من الفهرس
    /// Clear all items from index
    /// </summary>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> ClearAsync();

    #endregion

    #region عمليات البحث - Search Operations

    /// <summary>
    /// البحث في الفهرس
    /// Search in index
    /// </summary>
    /// <param name="searchRequest">طلب البحث - Search request</param>
    /// <returns>نتائج البحث - Search results</returns>
    Task<SearchResult<T>> SearchAsync(SearchRequest searchRequest);

    #endregion

    #region عمليات الحفظ والتحميل - Save and Load Operations

    /// <summary>
    /// حفظ الفهرس إلى ملف
    /// Save index to file
    /// </summary>
    /// <param name="filePath">مسار الملف - File path</param>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> SaveToFileAsync(string filePath);

    /// <summary>
    /// تحميل الفهرس من ملف
    /// Load index from file
    /// </summary>
    /// <param name="filePath">مسار الملف - File path</param>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> LoadFromFileAsync(string filePath);

    /// <summary>
    /// إعادة بناء الفهرس
    /// Rebuild index
    /// </summary>
    /// <returns>نتيجة العملية - Operation result</returns>
    Task<bool> RebuildIndexAsync();

    #endregion

    #region الإحصائيات - Statistics

    /// <summary>
    /// الحصول على إحصائيات الفهرس
    /// Get index statistics
    /// </summary>
    /// <returns>إحصائيات الفهرس - Index statistics</returns>
    PerformanceMetrics GetStatistics();

    #endregion

    #region الأحداث - Events

    /// <summary>
    /// حدث تغيير حالة الفهرس
    /// Index status changed event
    /// </summary>
    event EventHandler<IndexStatusChangedEventArgs>? StatusChanged;

    /// <summary>
    /// حدث إضافة عنصر
    /// Item added event
    /// </summary>
    event EventHandler<IndexItemEventArgs<T>>? ItemAdded;

    /// <summary>
    /// حدث تحديث عنصر
    /// Item updated event
    /// </summary>
    event EventHandler<IndexItemEventArgs<T>>? ItemUpdated;

    /// <summary>
    /// حدث حذف عنصر
    /// Item removed event
    /// </summary>
    event EventHandler<IndexItemEventArgs<T>>? ItemRemoved;

    /// <summary>
    /// حدث حدوث خطأ
    /// Error occurred event
    /// </summary>
    event EventHandler<IndexErrorEventArgs>? ErrorOccurred;

    #endregion
}