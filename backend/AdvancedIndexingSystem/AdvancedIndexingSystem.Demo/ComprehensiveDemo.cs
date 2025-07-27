using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Services;
using System.IO.Compression;
using System.Text.Json;

namespace AdvancedIndexingSystem.Demo;

/// <summary>
/// عرض توضيحي شامل للمميزات الجديدة
/// Comprehensive demo of new features
/// </summary>
public class ComprehensiveDemo
{
    private readonly CompressionService _compressionService;
    private readonly string _demoDirectory;

    public ComprehensiveDemo()
    {
        _compressionService = new CompressionService();
        _demoDirectory = Path.Combine(Path.GetTempPath(), "AdvancedIndexingDemo", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_demoDirectory);
    }

    /// <summary>
    /// تشغيل العرض التوضيحي الشامل
    /// Run comprehensive demonstration
    /// </summary>
    public async Task RunComprehensiveDemoAsync()
    {
        Console.WriteLine("🚀 بدء العرض التوضيحي الشامل للمميزات الجديدة");
        Console.WriteLine("   Starting comprehensive demonstration of new features");
        Console.WriteLine("=".PadRight(80, '='));

        try
        {
            await DemonstrateSearchAndFilteringAsync();
            await DemonstrateCompressionFeaturesAsync();
            await DemonstratePerformanceMetricsAsync();
            await DemonstrateRealWorldScenariosAsync();
            await DemonstrateAdvancedFeaturesAsync();

            Console.WriteLine("\n🎉 تم إكمال العرض التوضيحي الشامل بنجاح!");
            Console.WriteLine("   Comprehensive demonstration completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ خطأ في العرض التوضيحي: {ex.Message}");
            Console.WriteLine($"   Demo error: {ex.Message}");
        }
        finally
        {
            // Cleanup
            try
            {
                if (Directory.Exists(_demoDirectory))
                {
                    Directory.Delete(_demoDirectory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// عرض ميزات البحث والفلترة
    /// Demonstrate search and filtering features
    /// </summary>
    private async Task DemonstrateSearchAndFilteringAsync()
    {
        Console.WriteLine("\n📋 عرض ميزات البحث والفلترة المتقدمة");
        Console.WriteLine("   Demonstrating Advanced Search and Filtering Features");
        Console.WriteLine("-".PadRight(60, '-'));

        // Create sample hotel data
        var hotels = CreateSampleHotelData();
        Console.WriteLine($"✅ تم إنشاء {hotels.Count} فندق تجريبي");
        Console.WriteLine($"   Created {hotels.Count} sample hotels");

        // Demonstrate different search criteria
        await DemonstrateExactMatchSearchAsync(hotels);
        await DemonstrateRangeSearchAsync(hotels);
        await DemonstrateContainsSearchAsync(hotels);
        await DemonstrateFuzzySearchAsync(hotels);
        await DemonstrateComplexMultiCriteriaSearchAsync(hotels);
        await DemonstrateSortingAndPaginationAsync(hotels);
    }

    /// <summary>
    /// عرض البحث بالمطابقة التامة
    /// Demonstrate exact match search
    /// </summary>
    private async Task DemonstrateExactMatchSearchAsync(List<Hotel> hotels)
    {
        Console.WriteLine("\n🔍 البحث بالمطابقة التامة - Exact Match Search");
        
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

        // Simulate search execution time
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filteredHotels = hotels.Where(h => h.City == "Sanaa").ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🏨 العثور على {filteredHotels.Count} فندق في صنعاء");
        Console.WriteLine($"   🏨 Found {filteredHotels.Count} hotels in Sanaa");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");
        Console.WriteLine($"   ⏱️ Search time: {stopwatch.ElapsedMilliseconds} ms");

        foreach (var hotel in filteredHotels.Take(3))
        {
            Console.WriteLine($"      • {hotel.Name} - {hotel.Rating}⭐ - ${hotel.PricePerNight}/ليلة");
        }
    }

    /// <summary>
    /// عرض البحث بالنطاق
    /// Demonstrate range search
    /// </summary>
    private async Task DemonstrateRangeSearchAsync(List<Hotel> hotels)
    {
        Console.WriteLine("\n🔍 البحث بالنطاق - Range Search");
        
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "PricePerNight",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 100,
                    MaxValue = 300
                }
            }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filteredHotels = hotels.Where(h => h.PricePerNight >= 100 && h.PricePerNight <= 300).ToList();
        stopwatch.Stop();

        Console.WriteLine($"   💰 العثور على {filteredHotels.Count} فندق في النطاق السعري $100-$300");
        Console.WriteLine($"   💰 Found {filteredHotels.Count} hotels in price range $100-$300");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        var avgPrice = filteredHotels.Average(h => h.PricePerNight);
        Console.WriteLine($"   📊 متوسط السعر: ${avgPrice:F2}");
        Console.WriteLine($"   📊 Average price: ${avgPrice:F2}");
    }

    /// <summary>
    /// عرض البحث بالاحتواء
    /// Demonstrate contains search
    /// </summary>
    private async Task DemonstrateContainsSearchAsync(List<Hotel> hotels)
    {
        Console.WriteLine("\n🔍 البحث بالاحتواء - Contains Search");
        
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Name",
                    CriterionType = SearchCriterionType.Contains,
                    Value = "Palace",
                    CaseSensitive = false
                }
            }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filteredHotels = hotels.Where(h => h.Name.Contains("Palace", StringComparison.OrdinalIgnoreCase)).ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🏰 العثور على {filteredHotels.Count} فندق يحتوي على 'Palace'");
        Console.WriteLine($"   🏰 Found {filteredHotels.Count} hotels containing 'Palace'");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        foreach (var hotel in filteredHotels)
        {
            Console.WriteLine($"      • {hotel.Name} - {hotel.City}");
        }
    }

    /// <summary>
    /// عرض البحث الضبابي
    /// Demonstrate fuzzy search
    /// </summary>
    private async Task DemonstrateFuzzySearchAsync(List<Hotel> hotels)
    {
        Console.WriteLine("\n🔍 البحث الضبابي - Fuzzy Search");
        
        var searchTerm = "Hotell"; // Intentional typo
        Console.WriteLine($"   🔤 البحث عن: '{searchTerm}' (خطأ مقصود)");
        Console.WriteLine($"   🔤 Searching for: '{searchTerm}' (intentional typo)");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var fuzzyMatches = hotels.Where(h => 
            CalculateLevenshteinSimilarity(h.Name.ToLower(), searchTerm.ToLower()) >= 0.6
        ).ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🎯 العثور على {fuzzyMatches.Count} مطابقة تقريبية");
        Console.WriteLine($"   🎯 Found {fuzzyMatches.Count} fuzzy matches");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        foreach (var hotel in fuzzyMatches.Take(3))
        {
            var similarity = CalculateLevenshteinSimilarity(hotel.Name.ToLower(), searchTerm.ToLower());
            Console.WriteLine($"      • {hotel.Name} - التشابه: {similarity:P1}");
            Console.WriteLine($"        Similarity: {similarity:P1}");
        }
    }

    /// <summary>
    /// عرض البحث متعدد المعايير
    /// Demonstrate complex multi-criteria search
    /// </summary>
    private async Task DemonstrateComplexMultiCriteriaSearchAsync(List<Hotel> hotels)
    {
        Console.WriteLine("\n🔍 البحث متعدد المعايير - Complex Multi-Criteria Search");
        
        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Rating",
                    CriterionType = SearchCriterionType.GreaterThanOrEqual,
                    Value = 4.0
                },
                new SearchCriterion
                {
                    FieldName = "PricePerNight",
                    CriterionType = SearchCriterionType.LessThan,
                    Value = 250
                },
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.InList,
                    Values = new List<object> { "Sanaa", "Aden", "Taiz" }
                },
                new SearchCriterion
                {
                    FieldName = "HasWifi",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = true
                }
            }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filteredHotels = hotels.Where(h => 
            h.Rating >= 4.0 &&
            h.PricePerNight < 250 &&
            new[] { "Sanaa", "Aden", "Taiz" }.Contains(h.City) &&
            h.HasWifi
        ).ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🎯 المعايير: تقييم ≥ 4.0، سعر < $250، مدن محددة، واي فاي");
        Console.WriteLine($"   🎯 Criteria: Rating ≥ 4.0, Price < $250, Specific cities, WiFi");
        Console.WriteLine($"   ✅ العثور على {filteredHotels.Count} فندق يطابق جميع المعايير");
        Console.WriteLine($"   ✅ Found {filteredHotels.Count} hotels matching all criteria");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        foreach (var hotel in filteredHotels.Take(3))
        {
            Console.WriteLine($"      • {hotel.Name} - {hotel.City} - {hotel.Rating}⭐ - ${hotel.PricePerNight}");
        }
    }

    /// <summary>
    /// عرض الفرز والتصفح
    /// Demonstrate sorting and pagination
    /// </summary>
    private async Task DemonstrateSortingAndPaginationAsync(List<Hotel> hotels)
    {
        Console.WriteLine("\n🔍 الفرز والتصفح - Sorting and Pagination");
        
        var searchRequest = new SearchRequest
        {
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Rating",
                    Direction = SortDirection.Descending,
                    Priority = 1,
                    DataType = FieldDataType.Number
                },
                new SortCriterion
                {
                    FieldName = "PricePerNight",
                    Direction = SortDirection.Ascending,
                    Priority = 2,
                    DataType = FieldDataType.Number
                }
            },
            PageNumber = 1,
            PageSize = 5
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var sortedHotels = hotels
            .OrderByDescending(h => h.Rating)
            .ThenBy(h => h.PricePerNight)
            .Take(5)
            .ToList();
        stopwatch.Stop();

        Console.WriteLine($"   📊 فرز حسب: التقييم (تنازلي) ثم السعر (تصاعدي)");
        Console.WriteLine($"   📊 Sorted by: Rating (desc) then Price (asc)");
        Console.WriteLine($"   📄 الصفحة 1، حجم الصفحة: 5");
        Console.WriteLine($"   📄 Page 1, Page size: 5");
        Console.WriteLine($"   ⏱️ وقت المعالجة: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        Console.WriteLine("   🏆 أفضل الفنادق:");
        Console.WriteLine("   🏆 Top hotels:");
        for (int i = 0; i < sortedHotels.Count; i++)
        {
            var hotel = sortedHotels[i];
            Console.WriteLine($"      {i + 1}. {hotel.Name} - {hotel.Rating}⭐ - ${hotel.PricePerNight}");
        }
    }

    /// <summary>
    /// عرض ميزات الضغط
    /// Demonstrate compression features
    /// </summary>
    private async Task DemonstrateCompressionFeaturesAsync()
    {
        Console.WriteLine("\n📦 عرض ميزات الضغط والفتح باستخدام GZIP");
        Console.WriteLine("   Demonstrating GZIP Compression and Decompression Features");
        Console.WriteLine("-".PadRight(60, '-'));

        await DemonstrateStringCompressionAsync();
        await DemonstrateFileCompressionAsync();
        await DemonstrateJsonCompressionAsync();
        await DemonstrateBatchCompressionAsync();
        await DemonstrateIndexSpecificCompressionAsync();
    }

    /// <summary>
    /// عرض ضغط النصوص
    /// Demonstrate string compression
    /// </summary>
    private async Task DemonstrateStringCompressionAsync()
    {
        Console.WriteLine("\n📝 ضغط النصوص - String Compression");
        
        var originalText = @"
        مرحباً بكم في نظام الفهرسة المتقدم والديناميكي!
        هذا النظام يوفر إمكانيات بحث وفهرسة متقدمة مع دعم كامل للغة العربية.
        يمكنكم البحث والفلترة والفرز بطرق متعددة ومرنة.
        النظام يدعم أيضاً ضغط البيانات لتوفير مساحة التخزين وتحسين الأداء.
        
        Welcome to the Advanced Dynamic Indexing System!
        This system provides advanced search and indexing capabilities with full Arabic language support.
        You can search, filter, and sort in multiple flexible ways.
        The system also supports data compression to save storage space and improve performance.
        ".Trim();

        Console.WriteLine($"   📄 النص الأصلي: {originalText.Length} حرف");
        Console.WriteLine($"   📄 Original text: {originalText.Length} characters");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var compressedData = await _compressionService.CompressStringAsync(originalText);
        var compressionTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        var decompressedText = await _compressionService.DecompressStringAsync(compressedData);
        var decompressionTime = stopwatch.ElapsedMilliseconds;

        var originalBytes = System.Text.Encoding.UTF8.GetBytes(originalText).Length;
        var compressionRatio = (double)compressedData.Length / originalBytes;
        var spaceSavings = ((double)(originalBytes - compressedData.Length) / originalBytes) * 100;

        Console.WriteLine($"   📦 البيانات المضغوطة: {compressedData.Length} بايت");
        Console.WriteLine($"   📦 Compressed data: {compressedData.Length} bytes");
        Console.WriteLine($"   📊 نسبة الضغط: {compressionRatio:P1}");
        Console.WriteLine($"   📊 Compression ratio: {compressionRatio:P1}");
        Console.WriteLine($"   💾 توفير المساحة: {spaceSavings:F1}%");
        Console.WriteLine($"   💾 Space savings: {spaceSavings:F1}%");
        Console.WriteLine($"   ⏱️ وقت الضغط: {compressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Compression time: {compressionTime} ms");
        Console.WriteLine($"   ⏱️ وقت فتح الضغط: {decompressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Decompression time: {decompressionTime} ms");
        Console.WriteLine($"   ✅ تطابق البيانات: {originalText == decompressedText}");
        Console.WriteLine($"   ✅ Data integrity: {originalText == decompressedText}");
    }

    /// <summary>
    /// عرض ضغط الملفات
    /// Demonstrate file compression
    /// </summary>
    private async Task DemonstrateFileCompressionAsync()
    {
        Console.WriteLine("\n📁 ضغط الملفات - File Compression");
        
        // Create a test file
        var testContent = string.Join("\n", Enumerable.Range(1, 1000).Select(i => 
            $"Line {i}: This is a test line with some repeated content to demonstrate compression efficiency."));
        
        var inputFile = Path.Combine(_demoDirectory, "test-input.txt");
        var compressedFile = Path.Combine(_demoDirectory, "test-compressed.gz");
        var decompressedFile = Path.Combine(_demoDirectory, "test-decompressed.txt");

        await File.WriteAllTextAsync(inputFile, testContent);
        var originalSize = new FileInfo(inputFile).Length;

        Console.WriteLine($"   📄 ملف الاختبار: {originalSize:N0} بايت");
        Console.WriteLine($"   📄 Test file: {originalSize:N0} bytes");

        // Compress file
        var compressionResult = await _compressionService.CompressFileAsync(inputFile, compressedFile);
        
        if (compressionResult.IsSuccessful)
        {
            Console.WriteLine($"   📦 الملف المضغوط: {compressionResult.CompressedSize:N0} بايت");
            Console.WriteLine($"   📦 Compressed file: {compressionResult.CompressedSize:N0} bytes");
            Console.WriteLine($"   📊 نسبة الضغط: {compressionResult.CompressionRatio:P1}");
            Console.WriteLine($"   📊 Compression ratio: {compressionResult.CompressionRatio:P1}");
            Console.WriteLine($"   💾 توفير المساحة: {compressionResult.SpaceSavingsPercentage:F1}%");
            Console.WriteLine($"   💾 Space savings: {compressionResult.SpaceSavingsPercentage:F1}%");
            Console.WriteLine($"   ⏱️ وقت الضغط: {compressionResult.CompressionTimeMs:F1} مللي ثانية");
            Console.WriteLine($"   ⏱️ Compression time: {compressionResult.CompressionTimeMs:F1} ms");

            // Decompress file
            var decompressionResult = await _compressionService.DecompressFileAsync(compressedFile, decompressedFile);
            
            if (decompressionResult.IsSuccessful)
            {
                var decompressedContent = await File.ReadAllTextAsync(decompressedFile);
                var isIdentical = testContent == decompressedContent;
                
                Console.WriteLine($"   📄 الملف المفتوح: {decompressionResult.DecompressedSize:N0} بايت");
                Console.WriteLine($"   📄 Decompressed file: {decompressionResult.DecompressedSize:N0} bytes");
                Console.WriteLine($"   ⏱️ وقت فتح الضغط: {decompressionResult.DecompressionTimeMs:F1} مللي ثانية");
                Console.WriteLine($"   ⏱️ Decompression time: {decompressionResult.DecompressionTimeMs:F1} ms");
                Console.WriteLine($"   ✅ تطابق المحتوى: {isIdentical}");
                Console.WriteLine($"   ✅ Content integrity: {isIdentical}");
            }
        }
    }

    /// <summary>
    /// عرض ضغط JSON
    /// Demonstrate JSON compression
    /// </summary>
    private async Task DemonstrateJsonCompressionAsync()
    {
        Console.WriteLine("\n📋 ضغط البيانات JSON - JSON Data Compression");
        
        var sampleData = CreateSampleHotelData();
        Console.WriteLine($"   📊 بيانات الاختبار: {sampleData.Count} فندق");
        Console.WriteLine($"   📊 Test data: {sampleData.Count} hotels");

        var jsonString = JsonSerializer.Serialize(sampleData, new JsonSerializerOptions { WriteIndented = true });
        var originalJsonSize = System.Text.Encoding.UTF8.GetBytes(jsonString).Length;

        Console.WriteLine($"   📄 حجم JSON الأصلي: {originalJsonSize:N0} بايت");
        Console.WriteLine($"   📄 Original JSON size: {originalJsonSize:N0} bytes");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var compressedJson = await _compressionService.CompressJsonAsync(sampleData);
        var compressionTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        var decompressedData = await _compressionService.DecompressJsonAsync<List<Hotel>>(compressedJson);
        var decompressionTime = stopwatch.ElapsedMilliseconds;

        var compressionRatio = (double)compressedJson.Length / originalJsonSize;
        var spaceSavings = ((double)(originalJsonSize - compressedJson.Length) / originalJsonSize) * 100;

        Console.WriteLine($"   📦 البيانات المضغوطة: {compressedJson.Length:N0} بايت");
        Console.WriteLine($"   📦 Compressed data: {compressedJson.Length:N0} bytes");
        Console.WriteLine($"   📊 نسبة الضغط: {compressionRatio:P1}");
        Console.WriteLine($"   📊 Compression ratio: {compressionRatio:P1}");
        Console.WriteLine($"   💾 توفير المساحة: {spaceSavings:F1}%");
        Console.WriteLine($"   💾 Space savings: {spaceSavings:F1}%");
        Console.WriteLine($"   ⏱️ وقت الضغط: {compressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Compression time: {compressionTime} ms");
        Console.WriteLine($"   ⏱️ وقت فتح الضغط: {decompressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Decompression time: {decompressionTime} ms");
        Console.WriteLine($"   ✅ تطابق البيانات: {decompressedData?.Count == sampleData.Count}");
        Console.WriteLine($"   ✅ Data integrity: {decompressedData?.Count == sampleData.Count}");
    }

    /// <summary>
    /// عرض الضغط المتعدد
    /// Demonstrate batch compression
    /// </summary>
    private async Task DemonstrateBatchCompressionAsync()
    {
        Console.WriteLine("\n📦 الضغط المتعدد - Batch Compression");
        
        // Create multiple test files
        var files = new List<string>();
        for (int i = 1; i <= 3; i++)
        {
            var fileName = Path.Combine(_demoDirectory, $"batch-file-{i}.txt");
            var content = $"Batch file {i} content: " + string.Join(" ", Enumerable.Range(1, 100).Select(j => $"word{j}"));
            await File.WriteAllTextAsync(fileName, content);
            files.Add(fileName);
        }

        var compressedDir = Path.Combine(_demoDirectory, "compressed");
        Directory.CreateDirectory(compressedDir);

        Console.WriteLine($"   📁 ضغط {files.Count} ملفات...");
        Console.WriteLine($"   📁 Compressing {files.Count} files...");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = await _compressionService.CompressFilesAsync(files, compressedDir);
        stopwatch.Stop();

        var totalOriginalSize = results.Sum(r => r.OriginalSize);
        var totalCompressedSize = results.Sum(r => r.CompressedSize);
        var avgCompressionRatio = results.Average(r => r.CompressionRatio);
        var avgSpaceSavings = results.Average(r => r.SpaceSavingsPercentage);

        Console.WriteLine($"   ✅ تم ضغط {results.Count(r => r.IsSuccessful)} ملفات بنجاح");
        Console.WriteLine($"   ✅ Successfully compressed {results.Count(r => r.IsSuccessful)} files");
        Console.WriteLine($"   📊 الحجم الأصلي الإجمالي: {totalOriginalSize:N0} بايت");
        Console.WriteLine($"   📊 Total original size: {totalOriginalSize:N0} bytes");
        Console.WriteLine($"   📦 الحجم المضغوط الإجمالي: {totalCompressedSize:N0} بايت");
        Console.WriteLine($"   📦 Total compressed size: {totalCompressedSize:N0} bytes");
        Console.WriteLine($"   📊 متوسط نسبة الضغط: {avgCompressionRatio:P1}");
        Console.WriteLine($"   📊 Average compression ratio: {avgCompressionRatio:P1}");
        Console.WriteLine($"   💾 متوسط توفير المساحة: {avgSpaceSavings:F1}%");
        Console.WriteLine($"   💾 Average space savings: {avgSpaceSavings:F1}%");
        Console.WriteLine($"   ⏱️ إجمالي وقت الضغط: {stopwatch.ElapsedMilliseconds} مللي ثانية");
        Console.WriteLine($"   ⏱️ Total compression time: {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// عرض ضغط خاص بالفهرسة
    /// Demonstrate index-specific compression
    /// </summary>
    private async Task DemonstrateIndexSpecificCompressionAsync()
    {
        Console.WriteLine("\n🗂️ ضغط بيانات الفهرسة - Index-Specific Compression");
        
        // Create sample index configuration
        var indexConfig = new IndexConfiguration
        {
            IndexId = "demo-hotel-index",
            IndexName = "HotelSearchIndex",
            ArabicName = "فهرس البحث في الفنادق",
            Description = "Advanced hotel search index with dynamic fields",
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
                ["compression_enabled"] = true,
                ["parallel_processing"] = true,
                ["fuzzy_search_threshold"] = 0.7
            }
        };

        Console.WriteLine($"   🗂️ ضغط تكوين الفهرس: {indexConfig.IndexName}");
        Console.WriteLine($"   🗂️ Compressing index configuration: {indexConfig.IndexName}");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var compressedConfig = await _compressionService.CompressIndexConfigurationAsync(indexConfig);
        var compressionTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        var decompressedConfig = await _compressionService.DecompressIndexConfigurationAsync(compressedConfig);
        var decompressionTime = stopwatch.ElapsedMilliseconds;

        var originalConfigSize = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(indexConfig)).Length;
        var compressionRatio = (double)compressedConfig.Length / originalConfigSize;

        Console.WriteLine($"   📄 حجم التكوين الأصلي: {originalConfigSize:N0} بايت");
        Console.WriteLine($"   📄 Original config size: {originalConfigSize:N0} bytes");
        Console.WriteLine($"   📦 حجم التكوين المضغوط: {compressedConfig.Length:N0} بايت");
        Console.WriteLine($"   📦 Compressed config size: {compressedConfig.Length:N0} bytes");
        Console.WriteLine($"   📊 نسبة الضغط: {compressionRatio:P1}");
        Console.WriteLine($"   📊 Compression ratio: {compressionRatio:P1}");
        Console.WriteLine($"   ⏱️ وقت الضغط: {compressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Compression time: {compressionTime} ms");
        Console.WriteLine($"   ⏱️ وقت فتح الضغط: {decompressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Decompression time: {decompressionTime} ms");
        Console.WriteLine($"   ✅ تطابق التكوين: {decompressedConfig?.IndexId == indexConfig.IndexId}");
        Console.WriteLine($"   ✅ Config integrity: {decompressedConfig?.IndexId == indexConfig.IndexId}");

        // Create and compress search results
        var searchResults = new SearchResult<Hotel>
        {
            RequestId = Guid.NewGuid().ToString(),
            Items = CreateSampleHotelData().Take(10).ToList(),
            TotalCount = 150,
            PageNumber = 1,
            PageSize = 10,
            TotalPages = 15,
            HasPreviousPage = false,
            HasNextPage = true,
            ExecutionTimeMs = 12.5,
            IsSuccessful = true,
            SearchStatistics = new SearchStatistics
            {
                IndicesUsedCount = 1,
                ItemsExamined = 150,
                ItemsMatched = 10,
                EfficiencyRatio = 0.067,
                PerIndexTime = new Dictionary<string, double> { { "HotelSearchIndex", 12.5 } },
                PerformanceDetails = new Dictionary<string, object>
                {
                    ["cache_hits"] = 5,
                    ["cache_misses"] = 1,
                    ["memory_used_mb"] = 2.3
                }
            }
        };

        Console.WriteLine($"\n   🔍 ضغط نتائج البحث: {searchResults.Items.Count} عنصر");
        Console.WriteLine($"   🔍 Compressing search results: {searchResults.Items.Count} items");

        var compressedResults = await _compressionService.CompressSearchResultAsync(searchResults);
        var decompressedResults = await _compressionService.DecompressSearchResultAsync<Hotel>(compressedResults);

        var originalResultsSize = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(searchResults)).Length;
        var resultsCompressionRatio = (double)compressedResults.Length / originalResultsSize;

        Console.WriteLine($"   📊 حجم النتائج الأصلية: {originalResultsSize:N0} بايت");
        Console.WriteLine($"   📊 Original results size: {originalResultsSize:N0} bytes");
        Console.WriteLine($"   📦 حجم النتائج المضغوطة: {compressedResults.Length:N0} بايت");
        Console.WriteLine($"   📦 Compressed results size: {compressedResults.Length:N0} bytes");
        Console.WriteLine($"   📊 نسبة الضغط: {resultsCompressionRatio:P1}");
        Console.WriteLine($"   📊 Compression ratio: {resultsCompressionRatio:P1}");
        Console.WriteLine($"   ✅ تطابق النتائج: {decompressedResults?.Items.Count == searchResults.Items.Count}");
        Console.WriteLine($"   ✅ Results integrity: {decompressedResults?.Items.Count == searchResults.Items.Count}");
    }

    /// <summary>
    /// عرض معايير الأداء
    /// Demonstrate performance metrics
    /// </summary>
    private async Task DemonstratePerformanceMetricsAsync()
    {
        Console.WriteLine("\n📊 عرض معايير الأداء والإحصائيات");
        Console.WriteLine("   Demonstrating Performance Metrics and Statistics");
        Console.WriteLine("-".PadRight(60, '-'));

        var performanceMetrics = new PerformanceMetrics
        {
            IndexId = "demo-performance-index",
            IndexName = "DemoPerformanceIndex",
            TotalItems = 50000,
            IndexSizeBytes = 1024 * 1024 * 25, // 25MB
            AverageSearchTimeMs = 7.2,
            AverageInsertionTimeMs = 1.8,
            AverageUpdateTimeMs = 2.4,
            AverageDeletionTimeMs = 1.2,
            SearchOperationsCount = 2500,
            InsertionOperationsCount = 50000,
            UpdateOperationsCount = 5000,
            DeletionOperationsCount = 1000,
            MemoryUsageMb = 75.3,
            FieldStatistics = new Dictionary<string, FieldStatistics>
            {
                ["Name"] = new FieldStatistics
                {
                    FieldName = "Name",
                    DataType = FieldDataType.Text,
                    UniqueValuesCount = 48500,
                    NullValuesCount = 25,
                    AverageValueLength = 28.5,
                    SearchCount = 1200,
                    AverageSearchTimeMs = 9.1,
                    IsIndexed = true,
                    IndexSizeBytes = 1024 * 1024 * 5 // 5MB
                },
                ["City"] = new FieldStatistics
                {
                    FieldName = "City",
                    DataType = FieldDataType.Text,
                    UniqueValuesCount = 15,
                    NullValuesCount = 0,
                    AverageValueLength = 8.2,
                    SearchCount = 800,
                    AverageSearchTimeMs = 3.5,
                    IsIndexed = true,
                    IndexSizeBytes = 1024 * 512 // 512KB
                },
                ["Price"] = new FieldStatistics
                {
                    FieldName = "Price",
                    DataType = FieldDataType.Number,
                    UniqueValuesCount = 450,
                    NullValuesCount = 0,
                    MinValue = 50,
                    MaxValue = 800,
                    SearchCount = 1500,
                    AverageSearchTimeMs = 4.8,
                    IsIndexed = true,
                    IndexSizeBytes = 1024 * 256 // 256KB
                }
            },
            CacheStatistics = new CacheStatistics
            {
                CacheSizeBytes = 1024 * 1024 * 10, // 10MB
                CachedItemsCount = 2500,
                CacheHits = 8500,
                CacheMisses = 1500,
                CacheEvictions = 200,
                AverageCacheAccessTimeMs = 0.3,
                LastCleanupTime = DateTime.Now.AddHours(-2)
            }
        };

        Console.WriteLine($"📈 معايير الأداء للفهرس: {performanceMetrics.IndexName}");
        Console.WriteLine($"📈 Performance metrics for index: {performanceMetrics.IndexName}");

        // Calculate derived statistics
        performanceMetrics.CalculateDerivedStatistics();

        Console.WriteLine($"\n🗂️ معلومات الفهرس الأساسية:");
        Console.WriteLine($"🗂️ Basic Index Information:");
        Console.WriteLine($"   📊 إجمالي العناصر: {performanceMetrics.TotalItems:N0}");
        Console.WriteLine($"   📊 Total items: {performanceMetrics.TotalItems:N0}");
        Console.WriteLine($"   💾 حجم الفهرس: {performanceMetrics.IndexSizeBytes / (1024.0 * 1024):F1} ميجابايت");
        Console.WriteLine($"   💾 Index size: {performanceMetrics.IndexSizeBytes / (1024.0 * 1024):F1} MB");
        Console.WriteLine($"   🧠 استهلاك الذاكرة: {performanceMetrics.MemoryUsageMb:F1} ميجابايت");
        Console.WriteLine($"   🧠 Memory usage: {performanceMetrics.MemoryUsageMb:F1} MB");

        Console.WriteLine($"\n⏱️ أوقات العمليات (متوسط):");
        Console.WriteLine($"⏱️ Operation Times (Average):");
        Console.WriteLine($"   🔍 البحث: {performanceMetrics.AverageSearchTimeMs:F1} مللي ثانية");
        Console.WriteLine($"   🔍 Search: {performanceMetrics.AverageSearchTimeMs:F1} ms");
        Console.WriteLine($"   ➕ الإدراج: {performanceMetrics.AverageInsertionTimeMs:F1} مللي ثانية");
        Console.WriteLine($"   ➕ Insert: {performanceMetrics.AverageInsertionTimeMs:F1} ms");
        Console.WriteLine($"   ✏️ التحديث: {performanceMetrics.AverageUpdateTimeMs:F1} مللي ثانية");
        Console.WriteLine($"   ✏️ Update: {performanceMetrics.AverageUpdateTimeMs:F1} ms");
        Console.WriteLine($"   🗑️ الحذف: {performanceMetrics.AverageDeletionTimeMs:F1} مللي ثانية");
        Console.WriteLine($"   🗑️ Delete: {performanceMetrics.AverageDeletionTimeMs:F1} ms");

        Console.WriteLine($"\n📊 إحصائيات العمليات:");
        Console.WriteLine($"📊 Operation Statistics:");
        Console.WriteLine($"   🔍 عمليات البحث: {performanceMetrics.SearchOperationsCount:N0}");
        Console.WriteLine($"   🔍 Search operations: {performanceMetrics.SearchOperationsCount:N0}");
        Console.WriteLine($"   ➕ عمليات الإدراج: {performanceMetrics.InsertionOperationsCount:N0}");
        Console.WriteLine($"   ➕ Insert operations: {performanceMetrics.InsertionOperationsCount:N0}");
        Console.WriteLine($"   ✏️ عمليات التحديث: {performanceMetrics.UpdateOperationsCount:N0}");
        Console.WriteLine($"   ✏️ Update operations: {performanceMetrics.UpdateOperationsCount:N0}");
        Console.WriteLine($"   🗑️ عمليات الحذف: {performanceMetrics.DeletionOperationsCount:N0}");
        Console.WriteLine($"   🗑️ Delete operations: {performanceMetrics.DeletionOperationsCount:N0}");

        Console.WriteLine($"\n🎯 معايير الأداء المحسوبة:");
        Console.WriteLine($"🎯 Calculated Performance Metrics:");
        Console.WriteLine($"   ✅ معدل النجاح: {performanceMetrics.SuccessRate:P1}");
        Console.WriteLine($"   ✅ Success rate: {performanceMetrics.SuccessRate:P1}");
        Console.WriteLine($"   🚀 الإنتاجية: {performanceMetrics.ThroughputOpsPerSecond:F1} عملية/ثانية");
        Console.WriteLine($"   🚀 Throughput: {performanceMetrics.ThroughputOpsPerSecond:F1} ops/sec");
        Console.WriteLine($"   ⚡ نسبة الكفاءة: {performanceMetrics.EfficiencyRatio:F1} عنصر/ميجابايت");
        Console.WriteLine($"   ⚡ Efficiency ratio: {performanceMetrics.EfficiencyRatio:F1} items/MB");

        Console.WriteLine($"\n💾 إحصائيات التخزين المؤقت:");
        Console.WriteLine($"💾 Cache Statistics:");
        Console.WriteLine($"   📦 حجم التخزين المؤقت: {performanceMetrics.CacheStatistics.CacheSizeBytes / (1024.0 * 1024):F1} ميجابايت");
        Console.WriteLine($"   📦 Cache size: {performanceMetrics.CacheStatistics.CacheSizeBytes / (1024.0 * 1024):F1} MB");
        Console.WriteLine($"   📊 العناصر المخزنة: {performanceMetrics.CacheStatistics.CachedItemsCount:N0}");
        Console.WriteLine($"   📊 Cached items: {performanceMetrics.CacheStatistics.CachedItemsCount:N0}");
        Console.WriteLine($"   🎯 نسبة نجاح التخزين المؤقت: {performanceMetrics.CacheStatistics.CacheHitRatio:P1}");
        Console.WriteLine($"   🎯 Cache hit ratio: {performanceMetrics.CacheStatistics.CacheHitRatio:P1}");
        Console.WriteLine($"   ✅ النجاحات: {performanceMetrics.CacheStatistics.CacheHits:N0}");
        Console.WriteLine($"   ✅ Cache hits: {performanceMetrics.CacheStatistics.CacheHits:N0}");
        Console.WriteLine($"   ❌ الإخفاقات: {performanceMetrics.CacheStatistics.CacheMisses:N0}");
        Console.WriteLine($"   ❌ Cache misses: {performanceMetrics.CacheStatistics.CacheMisses:N0}");

        Console.WriteLine($"\n📋 إحصائيات الحقول:");
        Console.WriteLine($"📋 Field Statistics:");
        foreach (var fieldStat in performanceMetrics.FieldStatistics.Take(3))
        {
            Console.WriteLine($"   🏷️ الحقل: {fieldStat.Key}");
            Console.WriteLine($"   🏷️ Field: {fieldStat.Key}");
            Console.WriteLine($"      📊 القيم الفريدة: {fieldStat.Value.UniqueValuesCount:N0}");
            Console.WriteLine($"      📊 Unique values: {fieldStat.Value.UniqueValuesCount:N0}");
            Console.WriteLine($"      🔍 عمليات البحث: {fieldStat.Value.SearchCount:N0}");
            Console.WriteLine($"      🔍 Search count: {fieldStat.Value.SearchCount:N0}");
            Console.WriteLine($"      ⏱️ متوسط وقت البحث: {fieldStat.Value.AverageSearchTimeMs:F1} مللي ثانية");
            Console.WriteLine($"      ⏱️ Avg search time: {fieldStat.Value.AverageSearchTimeMs:F1} ms");
            Console.WriteLine($"      💾 حجم الفهرس: {fieldStat.Value.IndexSizeBytes / 1024.0:F1} كيلوبايت");
            Console.WriteLine($"      💾 Index size: {fieldStat.Value.IndexSizeBytes / 1024.0:F1} KB");
        }

        // Test compression of performance metrics
        Console.WriteLine($"\n📦 ضغط معايير الأداء:");
        Console.WriteLine($"📦 Compressing Performance Metrics:");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var compressedMetrics = await _compressionService.CompressJsonAsync(performanceMetrics);
        var compressionTime = stopwatch.ElapsedMilliseconds;

        var originalMetricsSize = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(performanceMetrics)).Length;
        var metricsCompressionRatio = (double)compressedMetrics.Length / originalMetricsSize;

        Console.WriteLine($"   📄 الحجم الأصلي: {originalMetricsSize:N0} بايت");
        Console.WriteLine($"   📄 Original size: {originalMetricsSize:N0} bytes");
        Console.WriteLine($"   📦 الحجم المضغوط: {compressedMetrics.Length:N0} بايت");
        Console.WriteLine($"   📦 Compressed size: {compressedMetrics.Length:N0} bytes");
        Console.WriteLine($"   📊 نسبة الضغط: {metricsCompressionRatio:P1}");
        Console.WriteLine($"   📊 Compression ratio: {metricsCompressionRatio:P1}");
        Console.WriteLine($"   ⏱️ وقت الضغط: {compressionTime} مللي ثانية");
        Console.WriteLine($"   ⏱️ Compression time: {compressionTime} ms");
    }

    /// <summary>
    /// عرض سيناريوهات واقعية
    /// Demonstrate real-world scenarios
    /// </summary>
    private async Task DemonstrateRealWorldScenariosAsync()
    {
        Console.WriteLine("\n🌍 عرض السيناريوهات الواقعية");
        Console.WriteLine("   Demonstrating Real-World Scenarios");
        Console.WriteLine("-".PadRight(60, '-'));

        await DemonstrateHotelBookingScenarioAsync();
        await DemonstrateRealEstateSearchScenarioAsync();
        await DemonstrateECommerceSearchScenarioAsync();
    }

    /// <summary>
    /// سيناريو حجز الفنادق
    /// Hotel booking scenario
    /// </summary>
    private async Task DemonstrateHotelBookingScenarioAsync()
    {
        Console.WriteLine("\n🏨 سيناريو حجز الفنادق - Hotel Booking Scenario");
        
        var hotels = CreateSampleHotelData();
        
        // Simulate a user searching for hotels
        Console.WriteLine("   👤 المستخدم يبحث عن فندق في صنعاء، تقييم 4+ نجوم، سعر أقل من $200");
        Console.WriteLine("   👤 User searching for hotel in Sanaa, 4+ stars, price under $200");

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
                    FieldName = "Rating",
                    CriterionType = SearchCriterionType.GreaterThanOrEqual,
                    Value = 4.0
                },
                new SearchCriterion
                {
                    FieldName = "PricePerNight",
                    CriterionType = SearchCriterionType.LessThan,
                    Value = 200
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Rating",
                    Direction = SortDirection.Descending,
                    Priority = 1
                },
                new SortCriterion
                {
                    FieldName = "PricePerNight",
                    Direction = SortDirection.Ascending,
                    Priority = 2
                }
            },
            PageSize = 5
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = hotels.Where(h => 
            h.City == "Sanaa" && 
            h.Rating >= 4.0 && 
            h.PricePerNight < 200
        ).OrderByDescending(h => h.Rating)
         .ThenBy(h => h.PricePerNight)
         .Take(5)
         .ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🔍 العثور على {results.Count} فندق يطابق المعايير");
        Console.WriteLine($"   🔍 Found {results.Count} hotels matching criteria");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");
        Console.WriteLine($"   ⏱️ Search time: {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("   🏆 أفضل النتائج:");
        Console.WriteLine("   🏆 Top results:");
        foreach (var hotel in results)
        {
            Console.WriteLine($"      • {hotel.Name} - {hotel.Rating}⭐ - ${hotel.PricePerNight}/ليلة");
            Console.WriteLine($"        {(hotel.HasWifi ? "📶 واي فاي" : "")} {(hotel.HasPool ? "🏊 مسبح" : "")} {(hotel.HasSpa ? "🧘 سبا" : "")}");
        }

        // Compress and store search results
        var searchResult = new SearchResult<Hotel>
        {
            RequestId = Guid.NewGuid().ToString(),
            Items = results,
            TotalCount = results.Count,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            IsSuccessful = true
        };

        var compressedResults = await _compressionService.CompressSearchResultAsync(searchResult);
        Console.WriteLine($"   📦 تم ضغط النتائج: {compressedResults.Length} بايت");
        Console.WriteLine($"   📦 Results compressed: {compressedResults.Length} bytes");
    }

    /// <summary>
    /// سيناريو البحث العقاري
    /// Real estate search scenario
    /// </summary>
    private async Task DemonstrateRealEstateSearchScenarioAsync()
    {
        Console.WriteLine("\n🏠 سيناريو البحث العقاري - Real Estate Search Scenario");
        
        var properties = CreateSampleRealEstateData();
        
        Console.WriteLine("   👤 المستخدم يبحث عن عقار: 3+ غرف نوم، موقف سيارات، في صنعاء أو عدن");
        Console.WriteLine("   👤 User searching for property: 3+ bedrooms, parking, in Sanaa or Aden");

        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
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
                    FieldName = "City",
                    CriterionType = SearchCriterionType.InList,
                    Values = new List<object> { "Sanaa", "Aden" }
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Price",
                    Direction = SortDirection.Ascending,
                    Priority = 1
                }
            }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = properties.Where(p => 
            p.Bedrooms >= 3 && 
            p.HasParking && 
            (p.City == "Sanaa" || p.City == "Aden")
        ).OrderBy(p => p.Price).ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🔍 العثور على {results.Count} عقار يطابق المعايير");
        Console.WriteLine($"   🔍 Found {results.Count} properties matching criteria");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        Console.WriteLine("   🏆 أفضل العقارات:");
        Console.WriteLine("   🏆 Top properties:");
        foreach (var property in results.Take(3))
        {
            Console.WriteLine($"      • {property.Name} - {property.City}");
            Console.WriteLine($"        💰 ${property.Price:N0} - 🛏️ {property.Bedrooms} غرف - 🚗 موقف");
            Console.WriteLine($"        💰 ${property.Price:N0} - 🛏️ {property.Bedrooms} beds - 🚗 Parking");
        }
    }

    /// <summary>
    /// سيناريو البحث في التجارة الإلكترونية
    /// E-commerce search scenario
    /// </summary>
    private async Task DemonstrateECommerceSearchScenarioAsync()
    {
        Console.WriteLine("\n🛒 سيناريو البحث في التجارة الإلكترونية - E-commerce Search Scenario");
        
        var products = CreateSampleProductData();
        
        Console.WriteLine("   👤 المستخدم يبحث عن: منتجات إلكترونية، سعر $100-$500، تقييم 4+ نجوم");
        Console.WriteLine("   👤 User searching for: Electronics, price $100-$500, 4+ star rating");

        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "Category",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "Electronics"
                },
                new SearchCriterion
                {
                    FieldName = "Price",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 100,
                    MaxValue = 500
                },
                new SearchCriterion
                {
                    FieldName = "Rating",
                    CriterionType = SearchCriterionType.GreaterThanOrEqual,
                    Value = 4.0
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "Rating",
                    Direction = SortDirection.Descending,
                    Priority = 1
                },
                new SortCriterion
                {
                    FieldName = "Price",
                    Direction = SortDirection.Ascending,
                    Priority = 2
                }
            }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = products.Where(p => 
            p.Category == "Electronics" && 
            p.Price >= 100 && p.Price <= 500 && 
            p.Rating >= 4.0
        ).OrderByDescending(p => p.Rating)
         .ThenBy(p => p.Price)
         .ToList();
        stopwatch.Stop();

        Console.WriteLine($"   🔍 العثور على {results.Count} منتج يطابق المعايير");
        Console.WriteLine($"   🔍 Found {results.Count} products matching criteria");
        Console.WriteLine($"   ⏱️ وقت البحث: {stopwatch.ElapsedMilliseconds} مللي ثانية");

        Console.WriteLine("   🏆 أفضل المنتجات:");
        Console.WriteLine("   🏆 Top products:");
        foreach (var product in results.Take(3))
        {
            Console.WriteLine($"      • {product.Name}");
            Console.WriteLine($"        💰 ${product.Price} - ⭐ {product.Rating} - 📦 {product.Stock} متوفر");
            Console.WriteLine($"        💰 ${product.Price} - ⭐ {product.Rating} - 📦 {product.Stock} in stock");
        }
    }

    /// <summary>
    /// عرض المميزات المتقدمة
    /// Demonstrate advanced features
    /// </summary>
    private async Task DemonstrateAdvancedFeaturesAsync()
    {
        Console.WriteLine("\n🚀 عرض المميزات المتقدمة");
        Console.WriteLine("   Demonstrating Advanced Features");
        Console.WriteLine("-".PadRight(60, '-'));

        await DemonstratePerformanceOptimizationAsync();
        await DemonstrateDataIntegrityAsync();
        await DemonstrateConcurrencyAsync();
    }

    /// <summary>
    /// عرض تحسين الأداء
    /// Demonstrate performance optimization
    /// </summary>
    private async Task DemonstratePerformanceOptimizationAsync()
    {
        Console.WriteLine("\n⚡ تحسين الأداء - Performance Optimization");
        
        var largeDataSet = Enumerable.Range(1, 10000).Select(i => new Hotel
        {
            Id = i.ToString(),
            Name = $"Hotel {i}",
            City = new[] { "Sanaa", "Aden", "Taiz", "Hodeidah" }[i % 4],
            Rating = 3.0 + (i % 3),
            PricePerNight = 50 + (i % 300),
            HasWifi = i % 2 == 0,
            HasPool = i % 3 == 0,
            HasSpa = i % 5 == 0
        }).ToList();

        Console.WriteLine($"   📊 بيانات الاختبار: {largeDataSet.Count:N0} فندق");
        Console.WriteLine($"   📊 Test data: {largeDataSet.Count:N0} hotels");

        // Test search performance
        var searchTimes = new List<double>();
        for (int i = 0; i < 5; i++)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var results = largeDataSet.Where(h => 
                h.City == "Sanaa" && 
                h.Rating >= 4.0 && 
                h.PricePerNight <= 200
            ).OrderByDescending(h => h.Rating).Take(10).ToList();
            stopwatch.Stop();
            searchTimes.Add(stopwatch.ElapsedMilliseconds);
        }

        var avgSearchTime = searchTimes.Average();
        var minSearchTime = searchTimes.Min();
        var maxSearchTime = searchTimes.Max();

        Console.WriteLine($"   🔍 نتائج اختبار الأداء (5 تشغيلات):");
        Console.WriteLine($"   🔍 Performance test results (5 runs):");
        Console.WriteLine($"      ⏱️ متوسط الوقت: {avgSearchTime:F1} مللي ثانية");
        Console.WriteLine($"      ⏱️ Average time: {avgSearchTime:F1} ms");
        Console.WriteLine($"      🚀 أسرع وقت: {minSearchTime:F1} مللي ثانية");
        Console.WriteLine($"      🚀 Fastest time: {minSearchTime:F1} ms");
        Console.WriteLine($"      🐌 أبطأ وقت: {maxSearchTime:F1} مللي ثانية");
        Console.WriteLine($"      🐌 Slowest time: {maxSearchTime:F1} ms");

        // Test compression performance
        var compressionStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var compressedData = await _compressionService.CompressJsonAsync(largeDataSet);
        compressionStopwatch.Stop();

        var originalSize = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(largeDataSet)).Length;
        var compressionRatio = (double)compressedData.Length / originalSize;

        Console.WriteLine($"\n   📦 أداء الضغط:");
        Console.WriteLine($"   📦 Compression performance:");
        Console.WriteLine($"      📄 الحجم الأصلي: {originalSize / (1024.0 * 1024):F1} ميجابايت");
        Console.WriteLine($"      📄 Original size: {originalSize / (1024.0 * 1024):F1} MB");
        Console.WriteLine($"      📦 الحجم المضغوط: {compressedData.Length / (1024.0 * 1024):F1} ميجابايت");
        Console.WriteLine($"      📦 Compressed size: {compressedData.Length / (1024.0 * 1024):F1} MB");
        Console.WriteLine($"      📊 نسبة الضغط: {compressionRatio:P1}");
        Console.WriteLine($"      📊 Compression ratio: {compressionRatio:P1}");
        Console.WriteLine($"      ⏱️ وقت الضغط: {compressionStopwatch.ElapsedMilliseconds} مللي ثانية");
        Console.WriteLine($"      ⏱️ Compression time: {compressionStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"      🚀 سرعة الضغط: {originalSize / (1024.0 * compressionStopwatch.ElapsedMilliseconds):F1} كيلوبايت/مللي ثانية");
        Console.WriteLine($"      🚀 Compression speed: {originalSize / (1024.0 * compressionStopwatch.ElapsedMilliseconds):F1} KB/ms");
    }

    /// <summary>
    /// عرض سلامة البيانات
    /// Demonstrate data integrity
    /// </summary>
    private async Task DemonstrateDataIntegrityAsync()
    {
        Console.WriteLine("\n🔒 سلامة البيانات - Data Integrity");
        
        var originalData = new ComplexTestData
        {
            Id = Guid.NewGuid(),
            Name = "اختبار سلامة البيانات المعقدة",
            Numbers = Enumerable.Range(1, 100).ToList(),
            Dictionary = new Dictionary<string, object>
            {
                ["string_value"] = "نص تجريبي",
                ["number_value"] = 42.5,
                ["boolean_value"] = true,
                ["date_value"] = DateTime.Now
            },
            NestedData = new NestedData
            {
                Values = new[] { "قيمة1", "قيمة2", "قيمة3" }
            }
        };

        Console.WriteLine($"   📋 البيانات الأصلية:");
        Console.WriteLine($"   📋 Original data:");
        Console.WriteLine($"      🆔 المعرف: {originalData.Id}");
        Console.WriteLine($"      🆔 ID: {originalData.Id}");
        Console.WriteLine($"      📝 الاسم: {originalData.Name}");
        Console.WriteLine($"      📝 Name: {originalData.Name}");
        Console.WriteLine($"      📊 عدد الأرقام: {originalData.Numbers.Count}");
        Console.WriteLine($"      📊 Numbers count: {originalData.Numbers.Count}");
        Console.WriteLine($"      📚 عدد عناصر القاموس: {originalData.Dictionary.Count}");
        Console.WriteLine($"      📚 Dictionary items: {originalData.Dictionary.Count}");

        // Multiple compression/decompression cycles
        var currentData = originalData;
        var cycles = 5;
        
        Console.WriteLine($"\n   🔄 تنفيذ {cycles} دورات ضغط/فتح:");
        Console.WriteLine($"   🔄 Performing {cycles} compression/decompression cycles:");

        for (int i = 1; i <= cycles; i++)
        {
            var cycleStopwatch = System.Diagnostics.Stopwatch.StartNew();
            var compressed = await _compressionService.CompressJsonAsync(currentData);
            currentData = await _compressionService.DecompressJsonAsync<ComplexTestData>(compressed);
            cycleStopwatch.Stop();

            Console.WriteLine($"      🔄 الدورة {i}: {cycleStopwatch.ElapsedMilliseconds} مللي ثانية");
            Console.WriteLine($"      🔄 Cycle {i}: {cycleStopwatch.ElapsedMilliseconds} ms");
        }

        // Verify data integrity
        var isIntact = currentData != null &&
                      currentData.Id == originalData.Id &&
                      currentData.Name == originalData.Name &&
                      currentData.Numbers.SequenceEqual(originalData.Numbers) &&
                      currentData.Dictionary.Count == originalData.Dictionary.Count &&
                      currentData.NestedData.Values.SequenceEqual(originalData.NestedData.Values);

        Console.WriteLine($"\n   ✅ سلامة البيانات بعد {cycles} دورات: {(isIntact ? "محفوظة" : "تالفة")}");
        Console.WriteLine($"   ✅ Data integrity after {cycles} cycles: {(isIntact ? "Preserved" : "Corrupted")}");
        
        if (isIntact)
        {
            Console.WriteLine($"      🎯 جميع البيانات تطابق النسخة الأصلية");
            Console.WriteLine($"      🎯 All data matches the original");
        }
    }

    /// <summary>
    /// عرض المعالجة المتزامنة
    /// Demonstrate concurrency
    /// </summary>
    private async Task DemonstrateConcurrencyAsync()
    {
        Console.WriteLine("\n🔄 المعالجة المتزامنة - Concurrency");
        
        var testData = CreateSampleHotelData();
        var concurrentOperations = 10;
        
        Console.WriteLine($"   🚀 تنفيذ {concurrentOperations} عمليات ضغط متزامنة:");
        Console.WriteLine($"   🚀 Performing {concurrentOperations} concurrent compression operations:");

        var tasks = new List<Task<(int taskId, double timeMs, int compressedSize)>>();
        
        for (int i = 1; i <= concurrentOperations; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var compressed = await _compressionService.CompressJsonAsync(testData);
                stopwatch.Stop();
                return (taskId, stopwatch.Elapsed.TotalMilliseconds, compressed.Length);
            }));
        }

        var results = await Task.WhenAll(tasks);
        
        var totalTime = results.Max(r => r.timeMs);
        var avgTime = results.Average(r => r.timeMs);
        var minTime = results.Min(r => r.timeMs);
        var maxTime = results.Max(r => r.timeMs);
        var allSameSize = results.All(r => r.compressedSize == results[0].compressedSize);

        Console.WriteLine($"   📊 نتائج العمليات المتزامنة:");
        Console.WriteLine($"   📊 Concurrent operations results:");
        Console.WriteLine($"      ⏱️ إجمالي الوقت: {totalTime:F1} مللي ثانية");
        Console.WriteLine($"      ⏱️ Total time: {totalTime:F1} ms");
        Console.WriteLine($"      📊 متوسط وقت العملية: {avgTime:F1} مللي ثانية");
        Console.WriteLine($"      📊 Average operation time: {avgTime:F1} ms");
        Console.WriteLine($"      🚀 أسرع عملية: {minTime:F1} مللي ثانية");
        Console.WriteLine($"      🚀 Fastest operation: {minTime:F1} ms");
        Console.WriteLine($"      🐌 أبطأ عملية: {maxTime:F1} مللي ثانية");
        Console.WriteLine($"      🐌 Slowest operation: {maxTime:F1} ms");
        Console.WriteLine($"      🎯 تطابق النتائج: {(allSameSize ? "نعم" : "لا")}");
        Console.WriteLine($"      🎯 Results consistency: {(allSameSize ? "Yes" : "No")}");
        Console.WriteLine($"      📦 حجم البيانات المضغوطة: {results[0].compressedSize:N0} بايت");
        Console.WriteLine($"      📦 Compressed data size: {results[0].compressedSize:N0} bytes");

        // Calculate throughput
        var totalOperations = concurrentOperations;
        var throughput = totalOperations / (totalTime / 1000.0); // operations per second
        
        Console.WriteLine($"      🚀 الإنتاجية: {throughput:F1} عملية/ثانية");
        Console.WriteLine($"      🚀 Throughput: {throughput:F1} operations/second");
    }

    #region Helper Methods and Data Creation

    /// <summary>
    /// إنشاء بيانات فنادق تجريبية
    /// Create sample hotel data
    /// </summary>
    private List<Hotel> CreateSampleHotelData()
    {
        var random = new Random(42); // Fixed seed for reproducible results
        var cities = new[] { "Sanaa", "Aden", "Taiz", "Hodeidah", "Ibb", "Dhamar" };
        var hotelTypes = new[] { "Hotel", "Resort", "Palace", "Plaza", "Lodge", "Inn", "Suites" };
        
        return Enumerable.Range(1, 50).Select(i => new Hotel
        {
            Id = i.ToString(),
            Name = $"{hotelTypes[random.Next(hotelTypes.Length)]} {cities[random.Next(cities.Length)]} {i}",
            City = cities[random.Next(cities.Length)],
            Rating = Math.Round(2.5 + random.NextDouble() * 2.5, 1), // 2.5 to 5.0
            PricePerNight = 50 + random.Next(350), // $50 to $400
            HasWifi = random.NextDouble() > 0.1, // 90% have WiFi
            HasPool = random.NextDouble() > 0.6, // 40% have pool
            HasSpa = random.NextDouble() > 0.8,  // 20% have spa
            HasParking = random.NextDouble() > 0.3, // 70% have parking
            HasRestaurant = random.NextDouble() > 0.4, // 60% have restaurant
            RoomCount = 20 + random.Next(180), // 20 to 200 rooms
            CheckInTime = "14:00",
            CheckOutTime = "12:00"
        }).ToList();
    }

    /// <summary>
    /// إنشاء بيانات عقارات تجريبية
    /// Create sample real estate data
    /// </summary>
    private List<RealEstateProperty> CreateSampleRealEstateData()
    {
        var random = new Random(42);
        var cities = new[] { "Sanaa", "Aden", "Taiz", "Hodeidah", "Ibb" };
        var propertyTypes = new[] { "Apartment", "Villa", "House", "Townhouse", "Penthouse" };
        
        return Enumerable.Range(1, 30).Select(i => new RealEstateProperty
        {
            Id = i.ToString(),
            Name = $"{propertyTypes[random.Next(propertyTypes.Length)]} in {cities[random.Next(cities.Length)]} {i}",
            City = cities[random.Next(cities.Length)],
            Price = 50000 + random.Next(950000), // $50K to $1M
            Bedrooms = 1 + random.Next(5), // 1 to 5 bedrooms
            Bathrooms = 1 + random.Next(4), // 1 to 4 bathrooms
            Area = 80 + random.Next(420), // 80 to 500 sqm
            HasParking = random.NextDouble() > 0.3, // 70% have parking
            HasGarden = random.NextDouble() > 0.6, // 40% have garden
            YearBuilt = 1990 + random.Next(34), // 1990 to 2024
            PropertyType = propertyTypes[random.Next(propertyTypes.Length)]
        }).ToList();
    }

    /// <summary>
    /// إنشاء بيانات منتجات تجريبية
    /// Create sample product data
    /// </summary>
    private List<Product> CreateSampleProductData()
    {
        var random = new Random(42);
        var categories = new[] { "Electronics", "Clothing", "Books", "Home", "Sports", "Beauty" };
        var brands = new[] { "Samsung", "Apple", "Nike", "Adidas", "Sony", "LG", "Canon", "HP" };
        
        return Enumerable.Range(1, 40).Select(i => new Product
        {
            Id = i.ToString(),
            Name = $"Product {i} {brands[random.Next(brands.Length)]}",
            Category = categories[random.Next(categories.Length)],
            Price = 10 + random.Next(990), // $10 to $1000
            Rating = Math.Round(2.0 + random.NextDouble() * 3.0, 1), // 2.0 to 5.0
            Stock = random.Next(100),
            Brand = brands[random.Next(brands.Length)],
            IsAvailable = random.NextDouble() > 0.1, // 90% available
            Weight = 0.1 + random.NextDouble() * 5.0, // 0.1 to 5.1 kg
            Dimensions = $"{10 + random.Next(40)}x{10 + random.Next(40)}x{5 + random.Next(20)} cm"
        }).ToList();
    }

    /// <summary>
    /// حساب التشابه باستخدام Levenshtein Distance
    /// Calculate similarity using Levenshtein Distance
    /// </summary>
    private double CalculateLevenshteinSimilarity(string source, string target)
    {
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

    #endregion
}

#region Model Classes

public class Hotel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public decimal PricePerNight { get; set; }
    public bool HasWifi { get; set; }
    public bool HasPool { get; set; }
    public bool HasSpa { get; set; }
    public bool HasParking { get; set; }
    public bool HasRestaurant { get; set; }
    public int RoomCount { get; set; }
    public string CheckInTime { get; set; } = string.Empty;
    public string CheckOutTime { get; set; } = string.Empty;
}

public class RealEstateProperty
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double Area { get; set; }
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public int YearBuilt { get; set; }
    public string PropertyType { get; set; } = string.Empty;
}

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int Stock { get; set; }
    public string Brand { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public double Weight { get; set; }
    public string Dimensions { get; set; } = string.Empty;
}

public class ComplexTestData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> Numbers { get; set; } = new();
    public Dictionary<string, object> Dictionary { get; set; } = new();
    public NestedData NestedData { get; set; } = new();
}

public class NestedData
{
    public string[] Values { get; set; } = Array.Empty<string>();
}

#endregion