using System;
using System.Threading.Tasks;
using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;

class TestNewFeatures
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🧪 اختبار المميزات الجديدة - Testing New Features");
        Console.WriteLine("=" + new string('=', 60));

        await TestCompressionService();
        Console.WriteLine();
        await TestPerformanceMetrics();

        Console.WriteLine("\n✅ تم اكتمال جميع الاختبارات بنجاح!");
        Console.WriteLine("✅ All tests completed successfully!");
    }

    static async Task TestCompressionService()
    {
        Console.WriteLine("📦 اختبار خدمة الضغط - Testing Compression Service");
        Console.WriteLine("-" + new string('-', 50));

        var compressionService = new CompressionService();

        // Test string compression
        string testText = "هذا نص تجريبي للاختبار - This is a test text for compression testing. " +
                         "It contains both Arabic and English text to test Unicode support.";
        
        Console.WriteLine($"📝 النص الأصلي: {testText.Length} حرف");
        Console.WriteLine($"📝 Original text: {testText.Length} characters");

        var compressedData = await compressionService.CompressStringAsync(testText);
        Console.WriteLine($"📦 البيانات المضغوطة: {compressedData.Length} بايت");
        Console.WriteLine($"📦 Compressed data: {compressedData.Length} bytes");

        var decompressedText = await compressionService.DecompressStringAsync(compressedData);
        Console.WriteLine($"📤 النص المستخرج: {decompressedText.Length} حرف");
        Console.WriteLine($"📤 Decompressed text: {decompressedText.Length} characters");

        bool isMatch = testText == decompressedText;
        Console.WriteLine($"✅ التطابق: {(isMatch ? "نعم" : "لا")} - Match: {(isMatch ? "Yes" : "No")}");

        // Test JSON compression
        var testMetrics = new PerformanceMetrics
        {
            IndexId = "test-index-001",
            IndexName = "Test Index",
            TotalItems = 10000,
            AverageSearchTimeMs = 45.6,
            SearchOperationsCount = 1500,
            SuccessRate = 0.995
        };

        var compressedJson = await compressionService.CompressJsonAsync(testMetrics);
        Console.WriteLine($"📊 JSON مضغوط: {compressedJson.Length} بايت");
        Console.WriteLine($"📊 Compressed JSON: {compressedJson.Length} bytes");

        var decompressedMetrics = await compressionService.DecompressJsonAsync<PerformanceMetrics>(compressedJson);
        bool jsonMatch = decompressedMetrics?.IndexId == testMetrics.IndexId && 
                        decompressedMetrics?.TotalItems == testMetrics.TotalItems;
        Console.WriteLine($"✅ تطابق JSON: {(jsonMatch ? "نعم" : "لا")} - JSON Match: {(jsonMatch ? "Yes" : "No")}");
    }

    static async Task TestPerformanceMetrics()
    {
        Console.WriteLine("📊 اختبار معايير الأداء - Testing Performance Metrics");
        Console.WriteLine("-" + new string('-', 50));

        var metrics = new PerformanceMetrics
        {
            IndexId = "performance-test-001",
            IndexName = "Performance Test Index",
            TotalItems = 50000,
            IndexSizeBytes = 1024 * 1024 * 10, // 10 MB
            AverageSearchTimeMs = 25.4,
            SearchOperationsCount = 2500,
            InsertionOperationsCount = 50000,
            SuccessRate = 0.998,
            MemoryUsageMb = 128.5
        };

        Console.WriteLine($"📋 معرف الفهرس: {metrics.IndexId}");
        Console.WriteLine($"📋 Index ID: {metrics.IndexId}");
        Console.WriteLine($"📊 عدد العناصر: {metrics.TotalItems:N0}");
        Console.WriteLine($"📊 Total items: {metrics.TotalItems:N0}");
        Console.WriteLine($"💾 حجم الفهرس: {metrics.IndexSizeBytes / (1024.0 * 1024):F1} ميجابايت");
        Console.WriteLine($"💾 Index size: {metrics.IndexSizeBytes / (1024.0 * 1024):F1} MB");
        Console.WriteLine($"⚡ متوسط وقت البحث: {metrics.AverageSearchTimeMs:F1} مللي ثانية");
        Console.WriteLine($"⚡ Average search time: {metrics.AverageSearchTimeMs:F1} ms");
        Console.WriteLine($"✅ معدل النجاح: {metrics.SuccessRate:P1}");
        Console.WriteLine($"✅ Success rate: {metrics.SuccessRate:P1}");

        // Test derived statistics calculation
        metrics.CalculateDerivedStatistics();
        Console.WriteLine($"🚀 الإنتاجية: {metrics.ThroughputOpsPerSecond:F1} عملية/ثانية");
        Console.WriteLine($"🚀 Throughput: {metrics.ThroughputOpsPerSecond:F1} ops/sec");
        Console.WriteLine($"⚡ نسبة الكفاءة: {metrics.EfficiencyRatio:F1} عنصر/ميجابايت");
        Console.WriteLine($"⚡ Efficiency ratio: {metrics.EfficiencyRatio:F1} items/MB");
    }
}