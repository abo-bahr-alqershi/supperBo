using Xunit;
using FluentAssertions;
using System.IO.Compression;
using System.Text;
using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;

namespace AdvancedIndexingSystem.Tests;

/// <summary>
/// اختبارات شاملة لخدمة الضغط
/// Comprehensive tests for CompressionService
/// </summary>
public class CompressionServiceTests : IDisposable
{
    private readonly CompressionService _compressionService;
    private readonly string _testDirectory;
    private readonly List<string> _createdFiles;

    public CompressionServiceTests()
    {
        _compressionService = new CompressionService();
        _testDirectory = Path.Combine(Path.GetTempPath(), "CompressionTests", Guid.NewGuid().ToString());
        _createdFiles = new List<string>();
        
        // Create test directory
        Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        // Clean up created files and directories
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

    #region String Compression Tests

    [Fact]
    public async Task CompressStringAsync_WithValidText_ShouldCompressSuccessfully()
    {
        // Arrange
        var originalText = "This is a test string for compression. It should be compressed effectively using GZIP algorithm. " +
                          "We need to repeat this text multiple times to make it long enough for effective compression. " +
                          "This is a test string for compression. It should be compressed effectively using GZIP algorithm. " +
                          "We need to repeat this text multiple times to make it long enough for effective compression.";

        // Act
        var compressedData = await _compressionService.CompressStringAsync(originalText);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
        // For very short text, compression might not be effective, so we just check it's working
        if (originalText.Length > 200)
        {
            compressedData.Length.Should().BeLessThan(Encoding.UTF8.GetBytes(originalText).Length);
        }
    }

    [Fact]
    public async Task DecompressStringAsync_WithCompressedData_ShouldReturnOriginalText()
    {
        // Arrange
        var originalText = "This is a test string for compression and decompression testing.";
        var compressedData = await _compressionService.CompressStringAsync(originalText);

        // Act
        var decompressedText = await _compressionService.DecompressStringAsync(compressedData);

        // Assert
        decompressedText.Should().Be(originalText);
    }

    [Fact]
    public async Task CompressStringAsync_WithEmptyString_ShouldReturnEmptyArray()
    {
        // Arrange
        var emptyString = string.Empty;

        // Act
        var compressedData = await _compressionService.CompressStringAsync(emptyString);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().Be(0);
    }

    [Fact]
    public async Task CompressStringAsync_WithNullString_ShouldReturnEmptyArray()
    {
        // Arrange
        string? nullString = null;

        // Act
        var compressedData = await _compressionService.CompressStringAsync(nullString!);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().Be(0);
    }

    [Fact]
    public async Task CompressDecompressString_WithUnicodeText_ShouldPreserveEncoding()
    {
        // Arrange
        var unicodeText = "مرحبا بك في نظام الفهرسة المتقدم! Welcome to Advanced Indexing System! 🚀";

        // Act
        var compressedData = await _compressionService.CompressStringAsync(unicodeText);
        var decompressedText = await _compressionService.DecompressStringAsync(compressedData);

        // Assert
        decompressedText.Should().Be(unicodeText);
    }

    #endregion

    #region Byte Array Compression Tests

    [Fact]
    public async Task CompressBytesAsync_WithValidData_ShouldCompressSuccessfully()
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes("This is test data for byte compression testing. ".PadRight(1000, 'x'));

        // Act
        var compressedData = await _compressionService.CompressBytesAsync(originalData);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
        compressedData.Length.Should().BeLessThan(originalData.Length);
    }

    [Fact]
    public async Task DecompressBytesAsync_WithCompressedData_ShouldReturnOriginalData()
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes("Test data for byte compression and decompression.");
        var compressedData = await _compressionService.CompressBytesAsync(originalData);

        // Act
        var decompressedData = await _compressionService.DecompressBytesAsync(compressedData);

        // Assert
        decompressedData.Should().BeEquivalentTo(originalData);
    }

    [Fact]
    public async Task CompressBytesAsync_WithEmptyArray_ShouldReturnEmptyArray()
    {
        // Arrange
        var emptyData = Array.Empty<byte>();

        // Act
        var compressedData = await _compressionService.CompressBytesAsync(emptyData);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().Be(0);
    }

    [Theory]
    [InlineData(CompressionLevel.Fastest)]
    [InlineData(CompressionLevel.Optimal)]
    [InlineData(CompressionLevel.SmallestSize)]
    public async Task CompressBytesAsync_WithDifferentCompressionLevels_ShouldWork(CompressionLevel level)
    {
        // Arrange
        var testData = Encoding.UTF8.GetBytes("Test data ".PadRight(500, 'x'));

        // Act
        var compressedData = await _compressionService.CompressBytesAsync(testData, level);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
    }

    #endregion

    #region File Compression Tests

    [Fact]
    public async Task CompressFileAsync_WithValidFile_ShouldCreateCompressedFile()
    {
        // Arrange
        var inputFile = CreateTestFile("test-input.txt", "This is test content for file compression testing. ".PadRight(1000, 'x'));
        var outputFile = Path.Combine(_testDirectory, "compressed.gz");

        // Act
        var result = await _compressionService.CompressFileAsync(inputFile, outputFile);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.OriginalSize.Should().BeGreaterThan(0);
        result.CompressedSize.Should().BeGreaterThan(0);
        result.CompressedSize.Should().BeLessThan(result.OriginalSize);
        result.CompressionRatio.Should().BeLessThan(1.0);
        result.SpaceSaved.Should().BeGreaterThan(0);
        result.CompressionTimeMs.Should().BeGreaterThan(0);
        File.Exists(outputFile).Should().BeTrue();
    }

    [Fact]
    public async Task DecompressFileAsync_WithValidCompressedFile_ShouldRestoreOriginalFile()
    {
        // Arrange
        var originalContent = "This is test content for file compression and decompression testing.";
        var inputFile = CreateTestFile("original.txt", originalContent);
        var compressedFile = Path.Combine(_testDirectory, "compressed.gz");
        var decompressedFile = Path.Combine(_testDirectory, "decompressed.txt");

        await _compressionService.CompressFileAsync(inputFile, compressedFile);

        // Act
        var result = await _compressionService.DecompressFileAsync(compressedFile, decompressedFile);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.CompressedSize.Should().BeGreaterThan(0);
        result.DecompressedSize.Should().BeGreaterThan(0);
        result.DecompressionTimeMs.Should().BeGreaterThan(0);
        File.Exists(decompressedFile).Should().BeTrue();

        var decompressedContent = await File.ReadAllTextAsync(decompressedFile);
        decompressedContent.Should().Be(originalContent);
    }

    [Fact]
    public async Task CompressFileAsync_WithNonExistentFile_ShouldReturnFailureResult()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "non-existent.txt");
        var outputFile = Path.Combine(_testDirectory, "output.gz");

        // Act
        var result = await _compressionService.CompressFileAsync(nonExistentFile, outputFile);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DecompressFileAsync_WithNonExistentFile_ShouldReturnFailureResult()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "non-existent.gz");
        var outputFile = Path.Combine(_testDirectory, "output.txt");

        // Act
        var result = await _compressionService.DecompressFileAsync(nonExistentFile, outputFile);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region JSON Compression Tests

    [Fact]
    public async Task CompressJsonAsync_WithValidObject_ShouldCompressSuccessfully()
    {
        // Arrange
        var testObject = new TestItem
        {
            Id = "test-123",
            Name = "Test Hotel",
            City = "Sanaa",
            Price = 150.50m,
            Rating = 4.5,
            IsActive = true,
            CreatedDate = DateTime.Now,
            Tags = new List<string> { "hotel", "business", "wifi" },
            Properties = new Dictionary<string, object>
            {
                ["rooms"] = 50,
                ["parking"] = true,
                ["pool"] = false
            }
        };

        // Act
        var compressedData = await _compressionService.CompressJsonAsync(testObject);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DecompressJsonAsync_WithCompressedData_ShouldReturnOriginalObject()
    {
        // Arrange
        var originalObject = new TestItem
        {
            Id = "test-456",
            Name = "Test Resort",
            City = "Aden",
            Price = 200.75m,
            Rating = 4.8,
            IsActive = true,
            CreatedDate = new DateTime(2024, 1, 15),
            Tags = new List<string> { "resort", "beach", "spa" }
        };

        var compressedData = await _compressionService.CompressJsonAsync(originalObject);

        // Act
        var decompressedObject = await _compressionService.DecompressJsonAsync<TestItem>(compressedData);

        // Assert
        decompressedObject.Should().NotBeNull();
        decompressedObject!.Id.Should().Be(originalObject.Id);
        decompressedObject.Name.Should().Be(originalObject.Name);
        decompressedObject.City.Should().Be(originalObject.City);
        decompressedObject.Price.Should().Be(originalObject.Price);
        decompressedObject.Rating.Should().Be(originalObject.Rating);
        decompressedObject.IsActive.Should().Be(originalObject.IsActive);
        decompressedObject.Tags.Should().BeEquivalentTo(originalObject.Tags);
    }

    [Fact]
    public async Task CompressJsonAsync_WithNullObject_ShouldReturnEmptyArray()
    {
        // Arrange
        TestItem? nullObject = null;

        // Act
        var compressedData = await _compressionService.CompressJsonAsync(nullObject);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().Be(0);
    }

    #endregion

    #region Index-Specific Compression Tests

    [Fact]
    public async Task CompressIndexConfigurationAsync_WithValidConfiguration_ShouldCompressSuccessfully()
    {
        // Arrange
        var configuration = new IndexConfiguration
        {
            IndexId = "test-index-001",
            IndexName = "TestIndex",
            ArabicName = "فهرس الاختبار",
            Description = "Test index for compression testing",
            IndexType = IndexType.CustomIndex,
            Priority = IndexPriority.High,
            Status = IndexStatus.Active,
            IsEnabled = true,
            AutoUpdate = true,
            MaxItems = 100000,
            CustomSettings = new Dictionary<string, object>
            {
                ["cache_size"] = 10000,
                ["batch_size"] = 1000,
                ["compression_enabled"] = true
            }
        };

        // Act
        var compressedData = await _compressionService.CompressIndexConfigurationAsync(configuration);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DecompressIndexConfigurationAsync_WithCompressedData_ShouldReturnOriginalConfiguration()
    {
        // Arrange
        var originalConfiguration = new IndexConfiguration
        {
            IndexId = "test-index-002",
            IndexName = "TestIndex2",
            ArabicName = "فهرس الاختبار الثاني",
            IndexType = IndexType.PriceIndex,
            Priority = IndexPriority.Medium
        };

        var compressedData = await _compressionService.CompressIndexConfigurationAsync(originalConfiguration);

        // Act
        var decompressedConfiguration = await _compressionService.DecompressIndexConfigurationAsync(compressedData);

        // Assert
        decompressedConfiguration.Should().NotBeNull();
        decompressedConfiguration!.IndexId.Should().Be(originalConfiguration.IndexId);
        decompressedConfiguration.IndexName.Should().Be(originalConfiguration.IndexName);
        decompressedConfiguration.ArabicName.Should().Be(originalConfiguration.ArabicName);
        decompressedConfiguration.IndexType.Should().Be(originalConfiguration.IndexType);
        decompressedConfiguration.Priority.Should().Be(originalConfiguration.Priority);
    }

    [Fact]
    public async Task CompressSearchResultAsync_WithValidSearchResult_ShouldCompressSuccessfully()
    {
        // Arrange
        var searchResult = new SearchResult<TestItem>
        {
            RequestId = "search-123",
            Items = new List<TestItem>
            {
                new TestItem { Id = "1", Name = "Hotel A", City = "Sanaa", Price = 150 },
                new TestItem { Id = "2", Name = "Hotel B", City = "Aden", Price = 200 }
            },
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10,
            ExecutionTimeMs = 15.5,
            IsSuccessful = true
        };

        // Act
        var compressedData = await _compressionService.CompressSearchResultAsync(searchResult);

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
    }

    #endregion

    #region Batch Compression Tests

    [Fact]
    public async Task CompressFilesAsync_WithMultipleFiles_ShouldCompressAllFiles()
    {
        // Arrange
        var file1 = CreateTestFile("file1.txt", "Content of file 1 ".PadRight(500, 'x'));
        var file2 = CreateTestFile("file2.txt", "Content of file 2 ".PadRight(600, 'y'));
        var file3 = CreateTestFile("file3.txt", "Content of file 3 ".PadRight(700, 'z'));
        
        var filePaths = new[] { file1, file2, file3 };
        var outputDirectory = Path.Combine(_testDirectory, "compressed");

        // Act
        var results = await _compressionService.CompressFilesAsync(filePaths, outputDirectory);

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(3);
        results.All(r => r.IsSuccessful).Should().BeTrue();
        results.All(r => r.CompressedSize < r.OriginalSize).Should().BeTrue();
    }

    [Fact]
    public async Task DecompressFilesAsync_WithMultipleCompressedFiles_ShouldDecompressAllFiles()
    {
        // Arrange
        var file1 = CreateTestFile("original1.txt", "Original content 1");
        var file2 = CreateTestFile("original2.txt", "Original content 2");
        
        var compressedDir = Path.Combine(_testDirectory, "compressed");
        var decompressedDir = Path.Combine(_testDirectory, "decompressed");
        Directory.CreateDirectory(compressedDir);
        Directory.CreateDirectory(decompressedDir);

        // Compress files first
        await _compressionService.CompressFilesAsync(new[] { file1, file2 }, compressedDir);
        
        var compressedFiles = Directory.GetFiles(compressedDir, "*.gz");

        // Act
        var results = await _compressionService.DecompressFilesAsync(compressedFiles, decompressedDir);

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(2);
        results.All(r => r.IsSuccessful).Should().BeTrue();
        Directory.GetFiles(decompressedDir).Should().HaveCount(2);
    }

    #endregion

    #region Utility Methods Tests

    [Theory]
    [InlineData(1000, 500, 0.5)]
    [InlineData(2000, 1000, 0.5)]
    [InlineData(100, 75, 0.75)]
    public void CalculateCompressionRatio_WithValidSizes_ShouldReturnCorrectRatio(long originalSize, long compressedSize, double expectedRatio)
    {
        // Act
        var ratio = CompressionService.CalculateCompressionRatio(originalSize, compressedSize);

        // Assert
        ratio.Should().BeApproximately(expectedRatio, 0.001);
    }

    [Theory]
    [InlineData(1000, 500, 50.0)]
    [InlineData(2000, 1000, 50.0)]
    [InlineData(100, 25, 75.0)]
    public void CalculateSpaceSavingsPercentage_WithValidSizes_ShouldReturnCorrectPercentage(long originalSize, long compressedSize, double expectedPercentage)
    {
        // Act
        var percentage = CompressionService.CalculateSpaceSavingsPercentage(originalSize, compressedSize);

        // Assert
        percentage.Should().BeApproximately(expectedPercentage, 0.001);
    }

    [Fact]
    public async Task ValidateCompressedFileAsync_WithValidCompressedFile_ShouldReturnTrue()
    {
        // Arrange
        var testFile = CreateTestFile("test.txt", "Test content for validation");
        var compressedFile = Path.Combine(_testDirectory, "test.gz");
        await _compressionService.CompressFileAsync(testFile, compressedFile);

        // Act
        var isValid = await _compressionService.ValidateCompressedFileAsync(compressedFile);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateCompressedFileAsync_WithNonExistentFile_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "non-existent.gz");

        // Act
        var isValid = await _compressionService.ValidateCompressedFileAsync(nonExistentFile);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateCompressedFileAsync_WithInvalidFile_ShouldReturnFalse()
    {
        // Arrange
        var invalidFile = CreateTestFile("invalid.gz", "This is not a valid GZIP file");

        // Act
        var isValid = await _compressionService.ValidateCompressedFileAsync(invalidFile);

        // Assert
        isValid.Should().BeFalse();
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithDefaultParameters_ShouldCreateInstance()
    {
        // Act
        var service = new CompressionService();

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithCustomParameters_ShouldCreateInstance()
    {
        // Act
        var service = new CompressionService(CompressionLevel.Fastest, Encoding.ASCII);

        // Assert
        service.Should().NotBeNull();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task CompressStringAsync_WithLargeString_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var largeString = new string('x', 1000000); // 1MB string
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var compressedData = await _compressionService.CompressStringAsync(largeString);
        stopwatch.Stop();

        // Assert
        compressedData.Should().NotBeNull();
        compressedData.Length.Should().BeGreaterThan(0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete within 5 seconds
    }

    #endregion

    #region Helper Methods

    private string CreateTestFile(string fileName, string content)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(filePath, content);
        _createdFiles.Add(filePath);
        return filePath;
    }

    #endregion
}