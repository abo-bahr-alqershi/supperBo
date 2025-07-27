using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Globalization;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;

namespace AdvancedIndexingSystem.Core.Services;

/// <summary>
/// خدمة البحث المتقدم والفلترة
/// Advanced Search and Filtering Service
/// </summary>
/// <typeparam name="T">نوع البيانات المفهرسة - Type of indexed data</typeparam>
public class SearchService<T> where T : class
{
    private readonly IAdvancedIndex<T> _index;
    private readonly ConcurrentDictionary<string, object> _searchCache = new();
    private readonly object _cacheLock = new();

    public SearchService(IAdvancedIndex<T> index)
    {
        _index = index ?? throw new ArgumentNullException(nameof(index));
    }

    /// <summary>
    /// تنفيذ البحث المتقدم مع الفلترة
    /// Execute advanced search with filtering
    /// </summary>
    public async Task<SearchResult<T>> ExecuteSearchAsync(SearchRequest searchRequest)
    {
        var startTime = DateTime.UtcNow;
        var searchResult = new SearchResult<T>
        {
            RequestId = searchRequest.RequestId,
            PageNumber = searchRequest.PageNumber,
            PageSize = searchRequest.PageSize
        };

        try
        {
            // Check cache first
            var cacheKey = GenerateCacheKey(searchRequest);
            if (_searchCache.TryGetValue(cacheKey, out var cachedResult))
            {
                return (SearchResult<T>)cachedResult;
            }

            // Get all items from index
            var allItems = await GetAllIndexedItemsAsync();
            
            // Apply search criteria
            var filteredItems = ApplySearchCriteria(allItems, searchRequest.SearchCriteria);
            
            // Apply sorting
            var sortedItems = ApplySorting(filteredItems, searchRequest.SortCriteria);
            
            // Apply pagination
            var paginatedItems = ApplyPagination(sortedItems, searchRequest.PageNumber, searchRequest.PageSize);
            
            // Build result
            searchResult.Items = paginatedItems.ToList();
            searchResult.TotalCount = filteredItems.Count();
            searchResult.TotalPages = (int)Math.Ceiling((double)searchResult.TotalCount / searchRequest.PageSize);
            searchResult.HasPreviousPage = searchRequest.PageNumber > 1;
            searchResult.HasNextPage = searchRequest.PageNumber < searchResult.TotalPages;
            searchResult.ExecutionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            searchResult.IsSuccessful = true;

            // Generate statistics if requested
            if (searchRequest.IncludeStatistics)
            {
                searchResult.SearchStatistics = GenerateSearchStatistics(allItems.Count(), filteredItems.Count(), searchResult.ExecutionTimeMs);
            }

            // Cache the result
            CacheSearchResult(cacheKey, searchResult);

            return searchResult;
        }
        catch (Exception ex)
        {
            searchResult.IsSuccessful = false;
            searchResult.ErrorMessage = ex.Message;
            searchResult.ExecutionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            return searchResult;
        }
    }

    /// <summary>
    /// تطبيق معايير البحث
    /// Apply search criteria
    /// </summary>
    private IEnumerable<T> ApplySearchCriteria(IEnumerable<T> items, List<SearchCriterion> criteria)
    {
        if (criteria == null || !criteria.Any())
            return items;

        var result = items;

        foreach (var criterion in criteria)
        {
            result = ApplySingleCriterion(result, criterion);
        }

        return result;
    }

    /// <summary>
    /// تطبيق معيار بحث واحد
    /// Apply single search criterion
    /// </summary>
    private IEnumerable<T> ApplySingleCriterion(IEnumerable<T> items, SearchCriterion criterion)
    {
        return criterion.CriterionType switch
        {
            SearchCriterionType.ExactMatch => ApplyExactMatch(items, criterion),
            SearchCriterionType.Contains => ApplyContains(items, criterion),
            SearchCriterionType.StartsWith => ApplyStartsWith(items, criterion),
            SearchCriterionType.EndsWith => ApplyEndsWith(items, criterion),
            SearchCriterionType.GreaterThan => ApplyGreaterThan(items, criterion),
            SearchCriterionType.GreaterThanOrEqual => ApplyGreaterThanOrEqual(items, criterion),
            SearchCriterionType.LessThan => ApplyLessThan(items, criterion),
            SearchCriterionType.LessThanOrEqual => ApplyLessThanOrEqual(items, criterion),
            SearchCriterionType.InRange => ApplyInRange(items, criterion),
            SearchCriterionType.InList => ApplyInList(items, criterion),
            SearchCriterionType.NotInList => ApplyNotInList(items, criterion),
            SearchCriterionType.IsNull => ApplyIsNull(items, criterion),
            SearchCriterionType.IsNotNull => ApplyIsNotNull(items, criterion),
            SearchCriterionType.RegularExpression => ApplyRegularExpression(items, criterion),
            SearchCriterionType.FuzzySearch => ApplyFuzzySearch(items, criterion),
            _ => items
        };
    }

    #region Search Criterion Implementations

    private IEnumerable<T> ApplyExactMatch(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null && criterion.Value == null) return true;
            if (fieldValue == null || criterion.Value == null) return false;
            
            return criterion.CaseSensitive
                ? fieldValue.ToString()!.Equals(criterion.Value.ToString(), StringComparison.Ordinal)
                : fieldValue.ToString()!.Equals(criterion.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        });
    }

    private IEnumerable<T> ApplyContains(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null || criterion.Value == null) return false;
            
            var comparison = criterion.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return fieldValue.ToString()!.Contains(criterion.Value.ToString()!, comparison);
        });
    }

    private IEnumerable<T> ApplyStartsWith(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null || criterion.Value == null) return false;
            
            var comparison = criterion.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return fieldValue.ToString()!.StartsWith(criterion.Value.ToString()!, comparison);
        });
    }

    private IEnumerable<T> ApplyEndsWith(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null || criterion.Value == null) return false;
            
            var comparison = criterion.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return fieldValue.ToString()!.EndsWith(criterion.Value.ToString()!, comparison);
        });
    }

    private IEnumerable<T> ApplyGreaterThan(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            return CompareValues(fieldValue, criterion.Value) > 0;
        });
    }

    private IEnumerable<T> ApplyGreaterThanOrEqual(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            return CompareValues(fieldValue, criterion.Value) >= 0;
        });
    }

    private IEnumerable<T> ApplyLessThan(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            return CompareValues(fieldValue, criterion.Value) < 0;
        });
    }

    private IEnumerable<T> ApplyLessThanOrEqual(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            return CompareValues(fieldValue, criterion.Value) <= 0;
        });
    }

    private IEnumerable<T> ApplyInRange(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null) return false;
            
            var minComparison = criterion.MinValue != null ? CompareValues(fieldValue, criterion.MinValue) : 1;
            var maxComparison = criterion.MaxValue != null ? CompareValues(fieldValue, criterion.MaxValue) : -1;
            
            return minComparison >= 0 && maxComparison <= 0;
        });
    }

    private IEnumerable<T> ApplyInList(IEnumerable<T> items, SearchCriterion criterion)
    {
        if (criterion.Values == null || !criterion.Values.Any()) return Enumerable.Empty<T>();
        
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null) return false;
            
            return criterion.Values.Any(value => 
            {
                if (criterion.CaseSensitive)
                    return fieldValue.ToString()!.Equals(value?.ToString(), StringComparison.Ordinal);
                else
                    return fieldValue.ToString()!.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase);
            });
        });
    }

    private IEnumerable<T> ApplyNotInList(IEnumerable<T> items, SearchCriterion criterion)
    {
        if (criterion.Values == null || !criterion.Values.Any()) return items;
        
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null) return true;
            
            return !criterion.Values.Any(value => 
            {
                if (criterion.CaseSensitive)
                    return fieldValue.ToString()!.Equals(value?.ToString(), StringComparison.Ordinal);
                else
                    return fieldValue.ToString()!.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase);
            });
        });
    }

    private IEnumerable<T> ApplyIsNull(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item => GetFieldValue(item, criterion.FieldName) == null);
    }

    private IEnumerable<T> ApplyIsNotNull(IEnumerable<T> items, SearchCriterion criterion)
    {
        return items.Where(item => GetFieldValue(item, criterion.FieldName) != null);
    }

    private IEnumerable<T> ApplyRegularExpression(IEnumerable<T> items, SearchCriterion criterion)
    {
        if (criterion.Value == null) return Enumerable.Empty<T>();
        
        try
        {
            var regex = new Regex(criterion.Value.ToString()!, 
                criterion.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
            
            return items.Where(item =>
            {
                var fieldValue = GetFieldValue(item, criterion.FieldName);
                return fieldValue != null && regex.IsMatch(fieldValue.ToString()!);
            });
        }
        catch (ArgumentException)
        {
            return Enumerable.Empty<T>();
        }
    }

    private IEnumerable<T> ApplyFuzzySearch(IEnumerable<T> items, SearchCriterion criterion)
    {
        if (criterion.Value == null) return Enumerable.Empty<T>();
        
        var searchTerm = criterion.Value.ToString()!;
        var threshold = 0.7; // Similarity threshold
        
        return items.Where(item =>
        {
            var fieldValue = GetFieldValue(item, criterion.FieldName);
            if (fieldValue == null) return false;
            
            var similarity = CalculateLevenshteinSimilarity(
                fieldValue.ToString()!, 
                searchTerm, 
                criterion.CaseSensitive);
            
            return similarity >= threshold;
        });
    }

    #endregion

    /// <summary>
    /// تطبيق الفرز
    /// Apply sorting
    /// </summary>
    private IOrderedEnumerable<T> ApplySorting(IEnumerable<T> items, List<SortCriterion> sortCriteria)
    {
        if (sortCriteria == null || !sortCriteria.Any())
        {
            return items.OrderBy(x => x.GetHashCode()); // Default sorting
        }

        var orderedCriteria = sortCriteria.OrderBy(c => c.Priority).ToList();
        var firstCriterion = orderedCriteria.First();
        
        IOrderedEnumerable<T> orderedItems;
        
        if (firstCriterion.Direction == SortDirection.Ascending)
        {
            orderedItems = items.OrderBy(item => GetFieldValue(item, firstCriterion.FieldName), 
                GetComparer(firstCriterion.DataType));
        }
        else
        {
            orderedItems = items.OrderByDescending(item => GetFieldValue(item, firstCriterion.FieldName), 
                GetComparer(firstCriterion.DataType));
        }

        // Apply additional sort criteria
        foreach (var criterion in orderedCriteria.Skip(1))
        {
            if (criterion.Direction == SortDirection.Ascending)
            {
                orderedItems = orderedItems.ThenBy(item => GetFieldValue(item, criterion.FieldName), 
                    GetComparer(criterion.DataType));
            }
            else
            {
                orderedItems = orderedItems.ThenByDescending(item => GetFieldValue(item, criterion.FieldName), 
                    GetComparer(criterion.DataType));
            }
        }

        return orderedItems;
    }

    /// <summary>
    /// تطبيق التصفح
    /// Apply pagination
    /// </summary>
    private IEnumerable<T> ApplyPagination(IEnumerable<T> items, int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;
        return items.Skip(skip).Take(pageSize);
    }

    #region Helper Methods

    /// <summary>
    /// الحصول على قيمة الحقل من العنصر
    /// Get field value from item
    /// </summary>
    private object? GetFieldValue(T item, string fieldName)
    {
        try
        {
            var property = typeof(T).GetProperty(fieldName);
            return property?.GetValue(item);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// مقارنة القيم
    /// Compare values
    /// </summary>
    private int CompareValues(object? value1, object? value2)
    {
        if (value1 == null && value2 == null) return 0;
        if (value1 == null) return -1;
        if (value2 == null) return 1;

        if (value1 is IComparable comparable1 && value2 is IComparable comparable2)
        {
            try
            {
                return comparable1.CompareTo(comparable2);
            }
            catch
            {
                return string.Compare(value1.ToString(), value2.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }

        return string.Compare(value1.ToString(), value2.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// الحصول على مقارن للنوع المحدد
    /// Get comparer for specific type
    /// </summary>
    private IComparer<object?> GetComparer(FieldDataType dataType)
    {
        return dataType switch
        {
            FieldDataType.Number => Comparer<object?>.Create((x, y) => 
            {
                if (double.TryParse(x?.ToString(), out var dx) && double.TryParse(y?.ToString(), out var dy))
                    return dx.CompareTo(dy);
                return CompareValues(x, y);
            }),
            FieldDataType.Date => Comparer<object?>.Create((x, y) => 
            {
                if (DateTime.TryParse(x?.ToString(), out var dx) && DateTime.TryParse(y?.ToString(), out var dy))
                    return dx.CompareTo(dy);
                return CompareValues(x, y);
            }),
            FieldDataType.Boolean => Comparer<object?>.Create((x, y) => 
            {
                if (bool.TryParse(x?.ToString(), out var bx) && bool.TryParse(y?.ToString(), out var by))
                    return bx.CompareTo(by);
                return CompareValues(x, y);
            }),
            _ => Comparer<object?>.Create(CompareValues)
        };
    }

    /// <summary>
    /// حساب التشابه باستخدام Levenshtein Distance
    /// Calculate similarity using Levenshtein Distance
    /// </summary>
    private double CalculateLevenshteinSimilarity(string source, string target, bool caseSensitive)
    {
        if (!caseSensitive)
        {
            source = source.ToLowerInvariant();
            target = target.ToLowerInvariant();
        }

        if (source == target) return 1.0;
        if (source.Length == 0 || target.Length == 0) return 0.0;

        var distance = CalculateLevenshteinDistance(source, target);
        var maxLength = Math.Max(source.Length, target.Length);
        
        return 1.0 - (double)distance / maxLength;
    }

    /// <summary>
    /// حساب Levenshtein Distance
    /// Calculate Levenshtein Distance
    /// </summary>
    private int CalculateLevenshteinDistance(string source, string target)
    {
        var matrix = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= target.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source.Length, target.Length];
    }

    /// <summary>
    /// الحصول على جميع العناصر المفهرسة
    /// Get all indexed items
    /// </summary>
    private async Task<IEnumerable<T>> GetAllIndexedItemsAsync()
    {
        // This would normally interact with the index service
        // For now, we'll use a placeholder implementation
        await Task.Delay(1); // Simulate async operation
        
        // In a real implementation, this would fetch from the index
        return new List<T>();
    }

    /// <summary>
    /// توليد مفتاح التخزين المؤقت
    /// Generate cache key
    /// </summary>
    private string GenerateCacheKey(SearchRequest request)
    {
        var keyData = new
        {
            request.SearchCriteria,
            request.SortCriteria,
            request.PageNumber,
            request.PageSize,
            request.TargetIndices
        };
        
        return $"search_{keyData.GetHashCode()}";
    }

    /// <summary>
    /// تخزين نتيجة البحث مؤقتاً
    /// Cache search result
    /// </summary>
    private void CacheSearchResult(string cacheKey, SearchResult<T> result)
    {
        lock (_cacheLock)
        {
            _searchCache.TryAdd(cacheKey, result);
            
            // Simple cache cleanup - remove oldest entries if cache gets too large
            if (_searchCache.Count > 1000)
            {
                var keysToRemove = _searchCache.Keys.Take(100).ToList();
                foreach (var key in keysToRemove)
                {
                    _searchCache.TryRemove(key, out _);
                }
            }
        }
    }

    /// <summary>
    /// توليد إحصائيات البحث
    /// Generate search statistics
    /// </summary>
    private SearchStatistics GenerateSearchStatistics(int totalExamined, int totalMatched, double executionTime)
    {
        return new SearchStatistics
        {
            IndicesUsedCount = 1,
            ItemsExamined = totalExamined,
            ItemsMatched = totalMatched,
            EfficiencyRatio = totalExamined > 0 ? (double)totalMatched / totalExamined : 0.0,
            PerIndexTime = new Dictionary<string, double> { { _index.Configuration.IndexName, executionTime } },
            PerformanceDetails = new Dictionary<string, object>
            {
                ["cache_hits"] = 0,
                ["cache_misses"] = 1,
                ["memory_used_mb"] = GC.GetTotalMemory(false) / 1024.0 / 1024.0
            }
        };
    }

    #endregion
}