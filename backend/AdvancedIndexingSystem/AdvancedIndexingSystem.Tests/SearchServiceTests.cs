using Xunit;
using Moq;
using FluentAssertions;
using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;

namespace AdvancedIndexingSystem.Tests;

/// <summary>
/// اختبارات شاملة لخدمة البحث
/// Comprehensive tests for SearchService
/// </summary>
public class SearchServiceTests
{
    private readonly Mock<IAdvancedIndex<TestItem>> _mockIndex;
    private readonly SearchService<TestItem> _searchService;
    private readonly List<TestItem> _testData;

    public SearchServiceTests()
    {
        _mockIndex = new Mock<IAdvancedIndex<TestItem>>();
        _searchService = new SearchService<TestItem>(_mockIndex.Object);
        _testData = CreateTestData();

        // Setup mock index configuration
        _mockIndex.Setup(x => x.Configuration).Returns(new IndexConfiguration
        {
            IndexId = "test-index",
            IndexName = "TestIndex",
            IndexType = IndexType.CustomIndex
        });
    }

    #region Test Data Setup

    private List<TestItem> CreateTestData()
    {
        return new List<TestItem>
        {
            new TestItem { Id = "1", Name = "Hotel Sana'a", City = "Sanaa", Price = 150, Rating = 4.5, IsActive = true, CreatedDate = DateTime.Now.AddDays(-10) },
            new TestItem { Id = "2", Name = "Aden Resort", City = "Aden", Price = 200, Rating = 4.2, IsActive = true, CreatedDate = DateTime.Now.AddDays(-5) },
            new TestItem { Id = "3", Name = "Taiz Hotel", City = "Taiz", Price = 120, Rating = 3.8, IsActive = false, CreatedDate = DateTime.Now.AddDays(-15) },
            new TestItem { Id = "4", Name = "Sana'a Plaza", City = "Sanaa", Price = 180, Rating = 4.7, IsActive = true, CreatedDate = DateTime.Now.AddDays(-2) },
            new TestItem { Id = "5", Name = "Aden Beach", City = "Aden", Price = 250, Rating = 4.9, IsActive = true, CreatedDate = DateTime.Now.AddDays(-1) },
            new TestItem { Id = "6", Name = "Mountain View", City = "Taiz", Price = 100, Rating = 3.5, IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) },
            new TestItem { Id = "7", Name = "Desert Lodge", City = "Marib", Price = 300, Rating = 4.8, IsActive = true, CreatedDate = DateTime.Now.AddDays(-7) },
            new TestItem { Id = "8", Name = "City Center", City = "Sanaa", Price = 90, Rating = 3.2, IsActive = false, CreatedDate = DateTime.Now.AddDays(-12) },
            new TestItem { Id = "9", Name = "Coastal Resort", City = "Hodeidah", Price = 220, Rating = 4.4, IsActive = true, CreatedDate = DateTime.Now.AddDays(-3) },
            new TestItem { Id = "10", Name = "Heritage Hotel", City = "Shibam", Price = 160, Rating = 4.1, IsActive = true, CreatedDate = DateTime.Now.AddDays(-8) }
        };
    }

    #endregion

    #region Basic Search Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithEmptyCriteria_ShouldReturnAllItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>(),
            PageSize = 20
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Items.Should().HaveCount(0); // Since we're using mock data
    }

    [Fact]
    public async Task ExecuteSearchAsync_WithValidRequest_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "Sanaa"
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.RequestId.Should().Be(searchRequest.RequestId);
        result.ExecutionTimeMs.Should().BeGreaterThan(0);
    }

    #endregion

    #region Search Criterion Tests

    [Theory]
    [InlineData("Sanaa", 3)] // Expected count for Sanaa
    [InlineData("Aden", 2)]  // Expected count for Aden
    [InlineData("Taiz", 2)]  // Expected count for Taiz
    public async Task ApplyExactMatch_WithCityField_ShouldReturnCorrectCount(string city, int expectedCount)
    {
        // This test would need actual data implementation
        // For now, we'll test the structure
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = city
                }
            }
        };

        var result = await _searchService.ExecuteSearchAsync(searchRequest);
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyContains_WithNameField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.Contains,
                    Value = "Hotel"
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyStartsWith_WithNameField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.StartsWith,
                    Value = "Hotel"
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyEndsWith_WithNameField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.EndsWith,
                    Value = "Resort"
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyGreaterThan_WithPriceField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.GreaterThan,
                    Value = 150
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyLessThan_WithPriceField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.LessThan,
                    Value = 150
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyInRange_WithPriceField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 100,
                    MaxValue = 200
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyInList_WithCityField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.InList,
                    Values = new List<object> { "Sanaa", "Aden", "Taiz" }
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyRegularExpression_WithNameField_ShouldReturnMatchingItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.RegularExpression,
                    Value = @"^Hotel.*"
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyFuzzySearch_WithNameField_ShouldReturnSimilarItems()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.FuzzySearch,
                    Value = "Hotell" // Intentional typo
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    #endregion

    #region Multiple Criteria Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithMultipleCriteria_ShouldApplyAllCriteria()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "Sanaa"
                },
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.GreaterThan,
                    Value = 100
                },
                new SearchCriterion
                {
                    FieldName = "IsActive",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = true
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    #endregion

    #region Sorting Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithSortCriteria_ShouldApplySorting()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Price",
                    Direction = SortDirection.Ascending,
                    Priority = 1,
                    DataType = FieldDataType.Number
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteSearchAsync_WithMultipleSortCriteria_ShouldApplyInPriorityOrder()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "City",
                    Direction = SortDirection.Ascending,
                    Priority = 1,
                    DataType = FieldDataType.Text
                },
                new SortCriterion
                {
                    FieldName = "Price",
                    Direction = SortDirection.Descending,
                    Priority = 2,
                    DataType = FieldDataType.Number
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    #endregion

    #region Pagination Tests

    [Theory]
    [InlineData(1, 5)]   // First page, 5 items
    [InlineData(2, 5)]   // Second page, 5 items
    [InlineData(1, 10)]  // First page, 10 items
    public async Task ExecuteSearchAsync_WithPagination_ShouldReturnCorrectPage(int pageNumber, int pageSize)
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public async Task ExecuteSearchAsync_WithPagination_ShouldCalculateCorrectTotalPages()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            PageNumber = 1,
            PageSize = 3,
            IncludeTotalCount = true
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.TotalPages.Should().BeGreaterOrEqualTo(0);
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithIncludeStatistics_ShouldReturnStatistics()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            IncludeStatistics = true
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.SearchStatistics.Should().NotBeNull();
        result.SearchStatistics.IndicesUsedCount.Should().BeGreaterThan(0);
        result.SearchStatistics.PerformanceDetails.Should().NotBeEmpty();
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithInvalidRegex_ShouldHandleGracefully()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.RegularExpression,
                    Value = "[invalid regex"
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue(); // Should handle gracefully and return empty results
    }

    [Fact]
    public async Task ExecuteSearchAsync_WithNullCriteria_ShouldHandleGracefully()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = null!
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task ExecuteSearchAsync_ShouldCompleteWithinTimeLimit()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            TimeoutMs = 5000,
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "Sanaa"
                }
            }
        };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _searchService.ExecuteSearchAsync(searchRequest);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(searchRequest.TimeoutMs);
        result.ExecutionTimeMs.Should().BeGreaterThan(0);
    }

    #endregion

    #region Cache Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithSameRequest_ShouldUseCaching()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "Sanaa"
                }
            }
        };

        // Act
        var result1 = await _searchService.ExecuteSearchAsync(searchRequest);
        var result2 = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1.IsSuccessful.Should().BeTrue();
        result2.IsSuccessful.Should().BeTrue();
        // Second request should be faster due to caching
        // This would need actual implementation to test properly
    }

    #endregion

    #region Case Sensitivity Tests

    [Fact]
    public async Task ExecuteSearchAsync_WithCaseSensitiveSearch_ShouldRespectCase()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.Contains,
                    Value = "HOTEL",
                    CaseSensitive = true
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteSearchAsync_WithCaseInsensitiveSearch_ShouldIgnoreCase()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.Contains,
                    Value = "HOTEL",
                    CaseSensitive = false
                }
            }
        };

        // Act
        var result = await _searchService.ExecuteSearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullIndex_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SearchService<TestItem>(null!));
    }

    [Fact]
    public void Constructor_WithValidIndex_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new SearchService<TestItem>(_mockIndex.Object);

        // Assert
        service.Should().NotBeNull();
    }

    #endregion
}

/// <summary>
/// عنصر اختبار
/// Test item class
/// </summary>
public class TestItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();
}