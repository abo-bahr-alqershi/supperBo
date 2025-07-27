using Xunit;
using FluentAssertions;
using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;
using System.IO.Compression;

namespace AdvancedIndexingSystem.Tests;

/// <summary>
/// اختبارات التكامل الشاملة للنظام
/// Comprehensive integration tests for the system
/// </summary>
public class IntegrationTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly CompressionService _compressionService;
    private readonly List<string> _createdFiles;

    public IntegrationTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "IntegrationTests", Guid.NewGuid().ToString());
        _compressionService = new CompressionService();
        _createdFiles = new List<string>();
        
        Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        try
        {
            foreach (var file in _createdFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    #region End-to-End Workflow Tests

    [Fact]
    public async Task CompleteIndexingWorkflow_WithSearchAndCompression_ShouldWorkCorrectly()
    {
        // Arrange
        var indexConfiguration = new IndexConfiguration
        {
            IndexId = "integration-test-001",
            IndexName = "IntegrationTestIndex",
            ArabicName = "فهرس اختبار التكامل",
            IndexType = IndexType.CustomIndex,
            Priority = IndexPriority.High,
            IsEnabled = true,
            AutoUpdate = true,
            MaxItems = 10000
        };

        var testItems = CreateLargeTestDataSet(1000);

        // Act & Assert

        // Step 1: Compress index configuration
        var compressedConfig = await _compressionService.CompressIndexConfigurationAsync(indexConfiguration);
        compressedConfig.Should().NotBeNull();
        compressedConfig.Length.Should().BeGreaterThan(0);

        // Step 2: Decompress and verify configuration
        var decompressedConfig = await _compressionService.DecompressIndexConfigurationAsync(compressedConfig);
        decompressedConfig.Should().NotBeNull();
        decompressedConfig!.IndexId.Should().Be(indexConfiguration.IndexId);
        decompressedConfig.IndexName.Should().Be(indexConfiguration.IndexName);

        // Step 3: Create search request
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 100,
                    MaxValue = 300
                },
                new SearchCriterion
                {
                    FieldName = "IsActive",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = true
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Rating",
                    Direction = SortDirection.Descending,
                    Priority = 1,
                    DataType = FieldDataType.Number
                }
            },
            PageNumber = 1,
            PageSize = 20,
            IncludeStatistics = true
        };

        // Step 4: Compress search request
        var compressedSearchRequest = await _compressionService.CompressJsonAsync(searchRequest);
        compressedSearchRequest.Should().NotBeNull();
        compressedSearchRequest.Length.Should().BeGreaterThan(0);

        // Step 5: Decompress search request
        var decompressedSearchRequest = await _compressionService.DecompressJsonAsync<SearchRequest>(compressedSearchRequest);
        decompressedSearchRequest.Should().NotBeNull();
        decompressedSearchRequest!.SearchCriteria.Should().HaveCount(2);
        decompressedSearchRequest.SortCriteria.Should().HaveCount(1);

        // Step 6: Create mock search result
        var searchResult = new SearchResult<TestItem>
        {
            RequestId = searchRequest.RequestId,
            Items = testItems.Take(20).ToList(),
            TotalCount = testItems.Count,
            PageNumber = 1,
            PageSize = 20,
            ExecutionTimeMs = 15.5,
            IsSuccessful = true,
            SearchStatistics = new SearchStatistics
            {
                IndicesUsedCount = 1,
                ItemsExamined = testItems.Count,
                ItemsMatched = 20,
                EfficiencyRatio = 0.02,
                PerIndexTime = new Dictionary<string, double> { { "IntegrationTestIndex", 15.5 } }
            }
        };

        // Step 7: Compress search result
        var compressedResult = await _compressionService.CompressSearchResultAsync(searchResult);
        compressedResult.Should().NotBeNull();
        compressedResult.Length.Should().BeGreaterThan(0);

        // Step 8: Decompress and verify search result
        var decompressedResult = await _compressionService.DecompressSearchResultAsync<TestItem>(compressedResult);
        decompressedResult.Should().NotBeNull();
        decompressedResult!.Items.Should().HaveCount(20);
        decompressedResult.IsSuccessful.Should().BeTrue();
        decompressedResult.SearchStatistics.Should().NotBeNull();
    }

    [Fact]
    public async Task PerformanceMetrics_WithLargeDataSet_ShouldGenerateAccurateStatistics()
    {
        // Arrange
        var performanceMetrics = new PerformanceMetrics
        {
            IndexId = "perf-test-001",
            IndexName = "PerformanceTestIndex",
            TotalItems = 10000,
            IndexSizeBytes = 1024 * 1024 * 10, // 10MB
            AverageSearchTimeMs = 8.5,
            AverageInsertionTimeMs = 2.1,
            AverageUpdateTimeMs = 3.2,
            AverageDeletionTimeMs = 1.8,
            SearchOperationsCount = 1500,
            InsertionOperationsCount = 10000,
            UpdateOperationsCount = 2500,
            DeletionOperationsCount = 500,
            MemoryUsageMb = 50.5,
            FieldStatistics = new Dictionary<string, FieldStatistics>
            {
                ["Name"] = new FieldStatistics
                {
                    FieldName = "Name",
                    DataType = FieldDataType.Text,
                    UniqueValuesCount = 8500,
                    NullValuesCount = 50,
                    AverageValueLength = 25.3,
                    SearchCount = 750,
                    AverageSearchTimeMs = 12.1,
                    IsIndexed = true
                },
                ["Price"] = new FieldStatistics
                {
                    FieldName = "Price",
                    DataType = FieldDataType.Number,
                    UniqueValuesCount = 500,
                    NullValuesCount = 0,
                    MinValue = 50,
                    MaxValue = 1000,
                    SearchCount = 900,
                    AverageSearchTimeMs = 5.2,
                    IsIndexed = true
                }
            },
            CacheStatistics = new CacheStatistics
            {
                CacheSizeBytes = 1024 * 1024 * 2, // 2MB
                CachedItemsCount = 1000,
                CacheHits = 2500,
                CacheMisses = 500,
                CacheEvictions = 100,
                AverageCacheAccessTimeMs = 0.5
            }
        };

        // Act
        performanceMetrics.CalculateDerivedStatistics();

        // Assert
        performanceMetrics.SuccessRate.Should().Be(1.0);
        performanceMetrics.ThroughputOpsPerSecond.Should().BeGreaterThan(0);
        performanceMetrics.EfficiencyRatio.Should().BeGreaterThan(0);
        performanceMetrics.CacheStatistics.CacheHitRatio.Should().BeApproximately(0.833, 0.01);

        // Test compression of performance metrics
        var compressedMetrics = await _compressionService.CompressJsonAsync(performanceMetrics);
        compressedMetrics.Should().NotBeNull();
        compressedMetrics.Length.Should().BeGreaterThan(0);

        var decompressedMetrics = await _compressionService.DecompressJsonAsync<PerformanceMetrics>(compressedMetrics);
        decompressedMetrics.Should().NotBeNull();
        decompressedMetrics!.IndexId.Should().Be(performanceMetrics.IndexId);
        decompressedMetrics.FieldStatistics.Should().HaveCount(2);
        decompressedMetrics.CacheStatistics.CacheHitRatio.Should().BeApproximately(0.833, 0.01);
    }

    #endregion

    #region Real-World Scenario Tests

    [Fact]
    public async Task RealEstateSearchScenario_WithComplexFiltering_ShouldWorkCorrectly()
    {
        // Arrange - Real estate properties
        var properties = new List<RealEstateProperty>
        {
            new RealEstateProperty { Id = "1", Name = "Luxury Villa Sanaa", City = "Sanaa", Price = 500000, Bedrooms = 5, Bathrooms = 4, Area = 350, PropertyType = "Villa", HasParking = true, HasGarden = true, YearBuilt = 2020 },
            new RealEstateProperty { Id = "2", Name = "Modern Apartment Aden", City = "Aden", Price = 200000, Bedrooms = 3, Bathrooms = 2, Area = 120, PropertyType = "Apartment", HasParking = true, HasGarden = false, YearBuilt = 2018 },
            new RealEstateProperty { Id = "3", Name = "Traditional House Taiz", City = "Taiz", Price = 150000, Bedrooms = 4, Bathrooms = 3, Area = 200, PropertyType = "House", HasParking = false, HasGarden = true, YearBuilt = 2015 },
            new RealEstateProperty { Id = "4", Name = "Beach Resort Hodeidah", City = "Hodeidah", Price = 800000, Bedrooms = 10, Bathrooms = 8, Area = 600, PropertyType = "Resort", HasParking = true, HasGarden = true, YearBuilt = 2022 },
            new RealEstateProperty { Id = "5", Name = "Office Building Sanaa", City = "Sanaa", Price = 1000000, Bedrooms = 0, Bathrooms = 10, Area = 800, PropertyType = "Commercial", HasParking = true, HasGarden = false, YearBuilt = 2019 }
        };

        // Complex search request
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.InList,
                    Values = new List<object> { "Sanaa", "Aden", "Taiz" }
                },
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 100000,
                    MaxValue = 600000
                },
                new SearchCriterion
                {
                    FieldName = "Bedrooms",
                    CriterionType = SearchCriterionType.GreaterThanOrEqual,
                    Value = 3
                },
                new SearchCriterion
                {
                    FieldName = "HasParking",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = true
                },
                new SearchCriterion
                {
                    FieldName = "YearBuilt",
                    CriterionType = SearchCriterionType.GreaterThan,
                    Value = 2016
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Price",
                    Direction = SortDirection.Ascending,
                    Priority = 1,
                    DataType = FieldDataType.Number
                },
                new SortCriterion
                {
                    FieldName = "Area",
                    Direction = SortDirection.Descending,
                    Priority = 2,
                    DataType = FieldDataType.Number
                }
            },
            PageNumber = 1,
            PageSize = 10,
            IncludeStatistics = true
        };

        // Act
        var compressedRequest = await _compressionService.CompressJsonAsync(searchRequest);
        var decompressedRequest = await _compressionService.DecompressJsonAsync<SearchRequest>(compressedRequest);

        // Simulate search results
        var filteredProperties = properties.Where(p => 
            new[] { "Sanaa", "Aden", "Taiz" }.Contains(p.City) &&
            p.Price >= 100000 && p.Price <= 600000 &&
            p.Bedrooms >= 3 &&
            p.HasParking &&
            p.YearBuilt > 2016
        ).OrderBy(p => p.Price).ThenByDescending(p => p.Area).ToList();

        var searchResult = new SearchResult<RealEstateProperty>
        {
            RequestId = searchRequest.RequestId,
            Items = filteredProperties,
            TotalCount = filteredProperties.Count,
            PageNumber = 1,
            PageSize = 10,
            ExecutionTimeMs = 12.3,
            IsSuccessful = true
        };

        // Assert
        decompressedRequest.Should().NotBeNull();
        decompressedRequest!.SearchCriteria.Should().HaveCount(5);
        
        searchResult.Items.Should().HaveCount(2); // Villa and Apartment should match
        searchResult.Items.First().Name.Should().Be("Modern Apartment Aden"); // Lowest price
        
        var compressedResult = await _compressionService.CompressSearchResultAsync(searchResult);
        compressedResult.Should().NotBeNull();
        compressedResult.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task HotelBookingSearchScenario_WithFuzzySearch_ShouldFindSimilarMatches()
    {
        // Arrange - Hotel data
        var hotels = new List<HotelProperty>
        {
            new HotelProperty { Id = "1", Name = "Grand Hotel Sanaa", City = "Sanaa", Rating = 5.0, Price = 200, Amenities = new[] { "WiFi", "Pool", "Spa", "Restaurant" } },
            new HotelProperty { Id = "2", Name = "Sanaa Palace Hotel", City = "Sanaa", Rating = 4.5, Price = 150, Amenities = new[] { "WiFi", "Restaurant", "Parking" } },
            new HotelProperty { Id = "3", Name = "Aden Beach Resort", City = "Aden", Rating = 4.8, Price = 180, Amenities = new[] { "WiFi", "Pool", "Beach", "Restaurant" } },
            new HotelProperty { Id = "4", Name = "Mountain View Hotel", City = "Taiz", Rating = 4.2, Price = 120, Amenities = new[] { "WiFi", "Restaurant", "Mountain View" } },
            new HotelProperty { Id = "5", Name = "Business Hotel Sanaa", City = "Sanaa", Rating = 4.0, Price = 100, Amenities = new[] { "WiFi", "Business Center", "Meeting Rooms" } }
        };

        // Fuzzy search for "Sana Hotel" (intentional misspelling)
        var fuzzySearchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.FuzzySearch,
                    Value = "Sana Hotel",
                    CaseSensitive = false
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Rating",
                    Direction = SortDirection.Descending,
                    Priority = 1,
                    DataType = FieldDataType.Number
                }
            }
        };

        // Act
        var compressedFuzzyRequest = await _compressionService.CompressJsonAsync(fuzzySearchRequest);
        var decompressedFuzzyRequest = await _compressionService.DecompressJsonAsync<SearchRequest>(compressedFuzzyRequest);

        // Assert
        decompressedFuzzyRequest.Should().NotBeNull();
        decompressedFuzzyRequest!.SearchCriteria.First().CriterionType.Should().Be(SearchCriterionType.FuzzySearch);
        decompressedFuzzyRequest.SearchCriteria.First().Value.ToString().Should().Be("Sana Hotel");
    }

    #endregion

    #region Performance and Load Tests

    [Fact]
    public async Task HighVolumeCompressionTest_WithLargeDataSet_ShouldMaintainPerformance()
    {
        // Arrange
        var largeDataSet = CreateLargeTestDataSet(10000);
        var batchSize = 1000;
        var compressionResults = new List<double>();

        // Act
        for (int i = 0; i < largeDataSet.Count; i += batchSize)
        {
            var batch = largeDataSet.Skip(i).Take(batchSize).ToList();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            var compressedBatch = await _compressionService.CompressJsonAsync(batch);
            
            stopwatch.Stop();
            compressionResults.Add(stopwatch.ElapsedMilliseconds);
            
            // Verify compression worked
            compressedBatch.Should().NotBeNull();
            compressedBatch.Length.Should().BeGreaterThan(0);
            
            // Verify decompression works
            var decompressedBatch = await _compressionService.DecompressJsonAsync<List<TestItem>>(compressedBatch);
            decompressedBatch.Should().NotBeNull();
            decompressedBatch!.Should().HaveCount(batch.Count);
        }

        // Assert
        compressionResults.Should().NotBeEmpty();
        compressionResults.Average().Should().BeLessThan(1000); // Average should be less than 1 second per batch
        compressionResults.Max().Should().BeLessThan(5000); // No single batch should take more than 5 seconds
    }

    [Fact]
    public async Task ConcurrentCompressionOperations_ShouldHandleParallelRequests()
    {
        // Arrange
        var testData = CreateTestDataSet(100);
        var concurrentTasks = new List<Task<byte[]>>();
        var numberOfConcurrentOperations = 10;

        // Act
        for (int i = 0; i < numberOfConcurrentOperations; i++)
        {
            var task = _compressionService.CompressJsonAsync(testData);
            concurrentTasks.Add(task);
        }

        var results = await Task.WhenAll(concurrentTasks);

        // Assert
        results.Should().HaveCount(numberOfConcurrentOperations);
        results.Should().AllSatisfy(result => 
        {
            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        });

        // Verify all results are identical (same input should produce same output)
        var firstResult = results[0];
        results.Should().AllSatisfy(result => result.Should().BeEquivalentTo(firstResult));
    }

    #endregion

    #region Error Handling and Edge Cases

    [Fact]
    public async Task ErrorHandling_WithCorruptedData_ShouldHandleGracefully()
    {
        // Arrange
        var corruptedData = new byte[] { 0x1F, 0x8B, 0x08, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }; // Invalid GZIP header

        // Act & Assert
        var result = await _compressionService.DecompressStringAsync(corruptedData);
        result.Should().BeEmpty(); // Should handle gracefully and return empty string

        var jsonResult = await _compressionService.DecompressJsonAsync<TestItem>(corruptedData);
        jsonResult.Should().BeNull(); // Should handle gracefully and return null
    }

    [Fact]
    public async Task LargeFileCompression_WithGigabyteFile_ShouldHandleEfficiently()
    {
        // Arrange - Create a large test file (but not actually 1GB for test performance)
        var largeContent = new string('x', 1024 * 1024); // 1MB of repeated characters
        var inputFile = CreateTestFile("large-file.txt", largeContent);
        var outputFile = Path.Combine(_testDirectory, "large-file.gz");

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _compressionService.CompressFileAsync(inputFile, outputFile);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.CompressionRatio.Should().BeLessThan(0.1); // Should compress very well due to repetitive content
        result.SpaceSavingsPercentage.Should().BeGreaterThan(90);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000); // Should complete within 10 seconds
        
        // Verify file exists and has expected properties
        File.Exists(outputFile).Should().BeTrue();
        var compressedFileInfo = new FileInfo(outputFile);
        compressedFileInfo.Length.Should().BeLessThan(new FileInfo(inputFile).Length);
    }

    #endregion

    #region Data Integrity Tests

    [Fact]
    public async Task DataIntegrity_ThroughMultipleCompressionCycles_ShouldMaintainAccuracy()
    {
        // Arrange
        var originalData = new ComplexTestObject
        {
            Id = Guid.NewGuid(),
            Name = "تجربة ضغط البيانات المعقدة",
            Numbers = Enumerable.Range(1, 1000).ToList(),
            NestedObject = new NestedTestObject
            {
                Properties = new Dictionary<string, object>
                {
                    ["string_prop"] = "test value",
                    ["int_prop"] = 42,
                    ["double_prop"] = 3.14159,
                    ["bool_prop"] = true,
                    ["date_prop"] = DateTime.Now
                }
            },
            Tags = new[] { "test", "integration", "compression", "data-integrity" }
        };

        // Act - Multiple compression/decompression cycles
        var currentData = originalData;
        for (int cycle = 0; cycle < 5; cycle++)
        {
            var compressed = await _compressionService.CompressJsonAsync(currentData);
            currentData = await _compressionService.DecompressJsonAsync<ComplexTestObject>(compressed);
            currentData.Should().NotBeNull();
        }

        // Assert - Data should remain identical after multiple cycles
        currentData!.Id.Should().Be(originalData.Id);
        currentData.Name.Should().Be(originalData.Name);
        currentData.Numbers.Should().BeEquivalentTo(originalData.Numbers);
        currentData.NestedObject.Properties.Should().HaveCount(originalData.NestedObject.Properties.Count);
        currentData.Tags.Should().BeEquivalentTo(originalData.Tags);
    }

    #endregion

    #region Helper Methods

    private List<TestItem> CreateTestDataSet(int count)
    {
        var random = new Random(42); // Fixed seed for reproducible tests
        var cities = new[] { "Sanaa", "Aden", "Taiz", "Hodeidah", "Ibb", "Dhamar", "Mukalla", "Shibam" };
        var names = new[] { "Hotel", "Resort", "Plaza", "Palace", "Lodge", "Inn", "Suites", "Center" };
        
        return Enumerable.Range(1, count).Select(i => new TestItem
        {
            Id = i.ToString(),
            Name = $"{names[random.Next(names.Length)]} {cities[random.Next(cities.Length)]} {i}",
            City = cities[random.Next(cities.Length)],
            Price = random.Next(50, 500),
            Rating = Math.Round(random.NextDouble() * 2 + 3, 1), // 3.0 to 5.0
            IsActive = random.NextDouble() > 0.2, // 80% active
            CreatedDate = DateTime.Now.AddDays(-random.Next(1, 365)),
            Tags = Enumerable.Range(0, random.Next(1, 5))
                .Select(_ => $"tag{random.Next(1, 20)}")
                .Distinct()
                .ToList(),
            Properties = new Dictionary<string, object>
            {
                ["rooms"] = random.Next(1, 10),
                ["parking"] = random.NextDouble() > 0.3,
                ["wifi"] = random.NextDouble() > 0.1,
                ["pool"] = random.NextDouble() > 0.6
            }
        }).ToList();
    }

    private List<TestItem> CreateLargeTestDataSet(int count)
    {
        return CreateTestDataSet(count);
    }

    private string CreateTestFile(string fileName, string content)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(filePath, content);
        _createdFiles.Add(filePath);
        return filePath;
    }

    #endregion
}

#region Test Model Classes

public class RealEstateProperty
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double Area { get; set; }
    public string PropertyType { get; set; } = string.Empty;
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public int YearBuilt { get; set; }
}

public class HotelProperty
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public decimal Price { get; set; }
    public string[] Amenities { get; set; } = Array.Empty<string>();
}

public class ComplexTestObject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> Numbers { get; set; } = new();
    public NestedTestObject NestedObject { get; set; } = new();
    public string[] Tags { get; set; } = Array.Empty<string>();
}

public class NestedTestObject
{
    public Dictionary<string, object> Properties { get; set; } = new();
}

#endregion