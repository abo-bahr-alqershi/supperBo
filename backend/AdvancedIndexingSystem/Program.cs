using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;
using System.Text.Json;

namespace AdvancedIndexingSystem;

/// <summary>
/// نموذج بيانات للاختبار
/// Test data model
/// </summary>
public class TestProperty
{
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public List<string> Amenities { get; set; } = new();
}

/// <summary>
/// برنامج اختبار النظام المحسن للذاكرة
/// Memory-optimized system test program
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== اختبار النظام المحسن للذاكرة ===");
        Console.WriteLine("=== Memory-Optimized System Test ===\n");

        try
        {
            await TestMemoryOptimizedIndexing();
            Console.WriteLine("\n✅ جميع الاختبارات نجحت!");
            Console.WriteLine("✅ All tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ فشل الاختبار: {ex.Message}");
            Console.WriteLine($"❌ Test failed: {ex.Message}");
        }
    }

    /// <summary>
    /// اختبار العمليات التدريجية
    /// Test incremental operations
    /// </summary>
    private static async Task TestMemoryOptimizedIndexing()
    {
        // إنشاء تكوين الفهرس
        var config = new IndexConfiguration<TestProperty>
        {
            Name = "TestPropertyIndex",
            Fields = new Dictionary<string, IndexFieldConfig>
            {
                ["City"] = new() { Type = IndexFieldType.String, IsFilterable = true },
                ["Price"] = new() { Type = IndexFieldType.Decimal, IsFilterable = true, IsRangeFilterable = true },
                ["Bedrooms"] = new() { Type = IndexFieldType.Integer, IsFilterable = true },
                ["Amenities"] = new() { Type = IndexFieldType.Array, IsFilterable = true }
            },
            MaxMemoryUsageMB = 50, // حد أقصى 50 ميجابايت
            EnableLazyLoading = true,
            EnableIncrementalUpdates = true
        };

        // إنشاء خدمة الفهرسة
        var indexService = new AdvancedIndexService<TestProperty>(config);

        Console.WriteLine("1. اختبار الإضافة التدريجية...");
        Console.WriteLine("1. Testing incremental addition...");

        // إضافة بيانات تدريجياً
        var properties = new[]
        {
            new TestProperty { Name = "Villa A", City = "صنعاء", Price = 1000, Bedrooms = 3, Amenities = new() { "مسبح", "حديقة" } },
            new TestProperty { Name = "Apartment B", City = "عدن", Price = 500, Bedrooms = 2, Amenities = new() { "مصعد", "أمان" } },
            new TestProperty { Name = "House C", City = "صنعاء", Price = 750, Bedrooms = 4, Amenities = new() { "حديقة", "جراج" } }
        };

        foreach (var property in properties)
        {
            await indexService.AddOrUpdateAsync(property.Name, property);
            Console.WriteLine($"   تمت إضافة: {property.Name}");
        }

        Console.WriteLine("\n2. اختبار الفلترة الذكية...");
        Console.WriteLine("2. Testing smart filtering...");

        // فلترة بالمدينة
        var sanaProperties = await indexService.FilterAsync(new Dictionary<string, object>
        {
            ["City"] = "صنعاء"
        });
        Console.WriteLine($"   العقارات في صنعاء: {sanaProperties.Count()}");

        // فلترة بالسعر
        var affordableProperties = await indexService.FilterAsync(new Dictionary<string, object>
        {
            ["Price"] = new { Min = 400, Max = 800 }
        });
        Console.WriteLine($"   العقارات في النطاق السعري: {affordableProperties.Count()}");

        Console.WriteLine("\n3. اختبار التحديث التدريجي...");
        Console.WriteLine("3. Testing incremental update...");

        // تحديث عقار
        var updatedProperty = properties[0];
        updatedProperty.Price = 1200;
        await indexService.AddOrUpdateAsync(updatedProperty.Name, updatedProperty);
        Console.WriteLine($"   تم تحديث: {updatedProperty.Name}");

        Console.WriteLine("\n4. اختبار الحذف...");
        Console.WriteLine("4. Testing deletion...");

        // حذف عقار
        await indexService.RemoveAsync("Apartment B");
        Console.WriteLine("   تم حذف: Apartment B");

        Console.WriteLine("\n5. اختبار الأداء والذاكرة...");
        Console.WriteLine("5. Testing performance and memory...");

        var stats = indexService.GetStatistics();
        Console.WriteLine($"   إجمالي العناصر: {stats.TotalItems}");
        Console.WriteLine($"   استخدام الذاكرة: {stats.MemoryUsageMB:F2} MB");
        Console.WriteLine($"   عدد الفهارس: {stats.IndexCount}");

        Console.WriteLine("\n6. اختبار البحث المركب...");
        Console.WriteLine("6. Testing complex search...");

        // بحث مركب
        var complexFilter = new Dictionary<string, object>
        {
            ["City"] = "صنعاء",
            ["Bedrooms"] = 3,
            ["Amenities"] = "حديقة"
        };

        var complexResults = await indexService.FilterAsync(complexFilter);
        Console.WriteLine($"   نتائج البحث المركب: {complexResults.Count()}");

        // تنظيف الموارد
        indexService.Dispose();
        Console.WriteLine("\n7. تم تنظيف الموارد بنجاح");
        Console.WriteLine("7. Resources cleaned up successfully");
    }
}