using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;
using System.Text.Json;

namespace AdvancedIndexingSystem.Tests;

/// <summary>
/// اختبار النظام المحسن للذاكرة
/// Memory-optimized system test
/// </summary>
public class TestMemoryOptimizedIndexing
{
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
    /// اختبار العمليات التدريجية
    /// Test incremental operations
    /// </summary>
    public static async Task TestIncrementalOperations()
    {
        Console.WriteLine("🚀 بدء اختبار العمليات التدريجية المحسنة للذاكرة...");
        Console.WriteLine("🚀 Starting memory-optimized incremental operations test...");

        // إنشاء تكوين الفهرس
        var config = new IndexConfiguration
        {
            IndexId = "test-properties",
            IndexName = "Test Properties Index",
            IndexedFields = new List<string> { "name", "city", "price", "bedrooms" },
            IsEnabled = true,
            AutoUpdate = true
        };

        // إنشاء خدمة الفهرسة
        var indexService = new AdvancedIndexService<TestProperty>(config);

        // إضافة عقارات للاختبار
        var properties = new List<(string Id, TestProperty Property)>
        {
            ("prop1", new TestProperty { Name = "شقة مفروشة", City = "صنعاء", Price = 800, Bedrooms = 2, Amenities = new List<string> { "wifi", "parking" } }),
            ("prop2", new TestProperty { Name = "فيلا عائلية", City = "عدن", Price = 1500, Bedrooms = 4, Amenities = new List<string> { "pool", "garden" } }),
            ("prop3", new TestProperty { Name = "استوديو حديث", City = "تعز", Price = 600, Bedrooms = 1, Amenities = new List<string> { "wifi", "elevator" } }),
            ("prop4", new TestProperty { Name = "شقة تجارية", City = "صنعاء", Price = 1200, Bedrooms = 3, Amenities = new List<string> { "parking", "security" } }),
            ("prop5", new TestProperty { Name = "بيت شعبي", City = "إب", Price = 400, Bedrooms = 2, Amenities = new List<string> { "garden" } })
        };

        Console.WriteLine($"📝 إضافة {properties.Count} عقار إلى الفهرس...");
        Console.WriteLine($"📝 Adding {properties.Count} properties to index...");

        // إضافة العقارات تدريجياً
        foreach (var (id, property) in properties)
        {
            var success = await indexService.AddItemAsync(id, property);
            Console.WriteLine($"   ✅ تم إضافة العقار {id}: {property.Name} - {success}");
            Console.WriteLine($"   ✅ Added property {id}: {property.Name} - {success}");
        }

        Console.WriteLine($"📊 إجمالي العناصر في الفهرس: {indexService.ItemCount}");
        Console.WriteLine($"📊 Total items in index: {indexService.ItemCount}");

        // اختبار البحث
        Console.WriteLine("\n🔍 اختبار البحث...");
        Console.WriteLine("🔍 Testing search...");

        var searchRequest = new SearchRequest
        {
            RequestId = Guid.NewGuid().ToString(),
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "صنعاء"
                }
            },
            PageNumber = 1,
            PageSize = 10
        };

        var searchResult = await indexService.SearchAsync(searchRequest);
        Console.WriteLine($"   🎯 نتائج البحث عن 'صنعاء': {searchResult.Items?.Count ?? 0} عقار");
        Console.WriteLine($"   🎯 Search results for 'صنعاء': {searchResult.Items?.Count ?? 0} properties");

        if (searchResult.Items != null)
        {
            foreach (var item in searchResult.Items)
            {
                Console.WriteLine($"      - {item.Name} - {item.Price} ريال");
            }
        }

        // اختبار التحديث
        Console.WriteLine("\n✏️ اختبار تحديث العقار...");
        Console.WriteLine("✏️ Testing property update...");

        var updatedProperty = new TestProperty 
        { 
            Name = "شقة مفروشة محدثة", 
            City = "صنعاء", 
            Price = 900, 
            Bedrooms = 2, 
            Amenities = new List<string> { "wifi", "parking", "air_conditioning" } 
        };

        var updateSuccess = await indexService.UpdateItemAsync("prop1", updatedProperty);
        Console.WriteLine($"   ✅ تم تحديث العقار prop1: {updateSuccess}");
        Console.WriteLine($"   ✅ Updated property prop1: {updateSuccess}");

        // اختبار الحذف
        Console.WriteLine("\n🗑️ اختبار حذف العقار...");
        Console.WriteLine("🗑️ Testing property deletion...");

        var deleteSuccess = await indexService.RemoveItemAsync("prop5");
        Console.WriteLine($"   ✅ تم حذف العقار prop5: {deleteSuccess}");
        Console.WriteLine($"   ✅ Deleted property prop5: {deleteSuccess}");

        Console.WriteLine($"📊 إجمالي العناصر بعد الحذف: {indexService.ItemCount}");
        Console.WriteLine($"📊 Total items after deletion: {indexService.ItemCount}");

        // اختبار الحفظ والتحميل
        Console.WriteLine("\n💾 اختبار حفظ وتحميل الفهرس...");
        Console.WriteLine("💾 Testing index save and load...");

        var tempFile = Path.GetTempFileName();
        var saveSuccess = await indexService.SaveToFileAsync(tempFile);
        Console.WriteLine($"   ✅ تم حفظ الفهرس: {saveSuccess}");
        Console.WriteLine($"   ✅ Index saved: {saveSuccess}");

        // إنشاء فهرس جديد وتحميل البيانات
        var newIndexService = new AdvancedIndexService<TestProperty>(config);
        var loadSuccess = await newIndexService.LoadFromFileAsync(tempFile);
        Console.WriteLine($"   ✅ تم تحميل الفهرس: {loadSuccess}");
        Console.WriteLine($"   ✅ Index loaded: {loadSuccess}");

        Console.WriteLine($"📊 عدد العناصر في الفهرس المحمل: {newIndexService.ItemCount}");
        Console.WriteLine($"📊 Items in loaded index: {newIndexService.ItemCount}");

        // تنظيف الملف المؤقت
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }

        // إحصائيات الأداء
        Console.WriteLine("\n📈 إحصائيات الأداء:");
        Console.WriteLine("📈 Performance statistics:");

        var stats = indexService.GetStatistics();
        Console.WriteLine($"   - عمليات الإضافة: {stats.TotalAddOperations}");
        Console.WriteLine($"   - عمليات التحديث: {stats.TotalUpdateOperations}");
        Console.WriteLine($"   - عمليات الحذف: {stats.TotalRemoveOperations}");
        Console.WriteLine($"   - عمليات البحث: {stats.TotalSearchOperations}");
        Console.WriteLine($"   - Add operations: {stats.TotalAddOperations}");
        Console.WriteLine($"   - Update operations: {stats.TotalUpdateOperations}");
        Console.WriteLine($"   - Remove operations: {stats.TotalRemoveOperations}");
        Console.WriteLine($"   - Search operations: {stats.TotalSearchOperations}");

        // تحرير الموارد
        indexService.Dispose();
        newIndexService.Dispose();

        Console.WriteLine("\n✅ تم الانتهاء من جميع الاختبارات بنجاح!");
        Console.WriteLine("✅ All tests completed successfully!");
    }

    /// <summary>
    /// اختبار الأداء والذاكرة
    /// Test performance and memory
    /// </summary>
    public static async Task TestPerformanceAndMemory()
    {
        Console.WriteLine("\n🚀 بدء اختبار الأداء والذاكرة...");
        Console.WriteLine("🚀 Starting performance and memory test...");

        var config = new IndexConfiguration
        {
            IndexId = "performance-test",
            IndexName = "Performance Test Index",
            IndexedFields = new List<string> { "name", "city", "price", "bedrooms" },
            IsEnabled = true,
            AutoUpdate = true
        };

        var indexService = new AdvancedIndexService<TestProperty>(config);

        // قياس الذاكرة قبل البدء
        var initialMemory = GC.GetTotalMemory(true);
        Console.WriteLine($"📊 الذاكرة المستخدمة في البداية: {initialMemory / 1024 / 1024:F2} MB");
        Console.WriteLine($"📊 Initial memory usage: {initialMemory / 1024 / 1024:F2} MB");

        // إضافة عدد كبير من العناصر
        var itemCount = 1000;
        Console.WriteLine($"📝 إضافة {itemCount} عنصر...");
        Console.WriteLine($"📝 Adding {itemCount} items...");

        var startTime = DateTime.UtcNow;

        for (int i = 0; i < itemCount; i++)
        {
            var property = new TestProperty
            {
                Name = $"عقار رقم {i}",
                City = i % 2 == 0 ? "صنعاء" : "عدن",
                Price = 500 + (i % 1000),
                Bedrooms = 1 + (i % 4),
                Amenities = new List<string> { "wifi", "parking" }
            };

            await indexService.AddItemAsync($"prop{i}", property);

            if (i % 100 == 0)
            {
                var currentMemory = GC.GetTotalMemory(false);
                Console.WriteLine($"   📊 تم إضافة {i} عنصر - الذاكرة: {currentMemory / 1024 / 1024:F2} MB");
                Console.WriteLine($"   📊 Added {i} items - Memory: {currentMemory / 1024 / 1024:F2} MB");
            }
        }

        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        Console.WriteLine($"⏱️ وقت إضافة {itemCount} عنصر: {duration.TotalSeconds:F2} ثانية");
        Console.WriteLine($"⏱️ Time to add {itemCount} items: {duration.TotalSeconds:F2} seconds");

        // قياس الذاكرة بعد الإضافة
        var finalMemory = GC.GetTotalMemory(true);
        Console.WriteLine($"📊 الذاكرة المستخدمة نهائياً: {finalMemory / 1024 / 1024:F2} MB");
        Console.WriteLine($"📊 Final memory usage: {finalMemory / 1024 / 1024:F2} MB");
        Console.WriteLine($"📊 زيادة الذاكرة: {(finalMemory - initialMemory) / 1024 / 1024:F2} MB");
        Console.WriteLine($"📊 Memory increase: {(finalMemory - initialMemory) / 1024 / 1024:F2} MB");

        // اختبار سرعة البحث
        Console.WriteLine("\n🔍 اختبار سرعة البحث...");
        Console.WriteLine("🔍 Testing search speed...");

        var searchStartTime = DateTime.UtcNow;

        var searchRequest = new SearchRequest
        {
            RequestId = Guid.NewGuid().ToString(),
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "City",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "صنعاء"
                }
            },
            PageNumber = 1,
            PageSize = 10
        };

        var searchResult = await indexService.SearchAsync(searchRequest);
        var searchEndTime = DateTime.UtcNow;
        var searchDuration = searchEndTime - searchStartTime;

        Console.WriteLine($"⏱️ وقت البحث: {searchDuration.TotalMilliseconds:F2} مللي ثانية");
        Console.WriteLine($"⏱️ Search time: {searchDuration.TotalMilliseconds:F2} milliseconds");
        Console.WriteLine($"🎯 عدد النتائج: {searchResult.Items?.Count ?? 0}");
        Console.WriteLine($"🎯 Results count: {searchResult.Items?.Count ?? 0}");

        // تحرير الموارد
        indexService.Dispose();

        Console.WriteLine("\n✅ تم الانتهاء من اختبار الأداء!");
        Console.WriteLine("✅ Performance test completed!");
    }

    /// <summary>
    /// تشغيل جميع الاختبارات
    /// Run all tests
    /// </summary>
    public static async Task RunAllTests()
    {
        Console.WriteLine("🎯 بدء تشغيل جميع اختبارات النظام المحسن للذاكرة");
        Console.WriteLine("🎯 Starting all memory-optimized system tests");
        Console.WriteLine("=" * 60);

        try
        {
            await TestIncrementalOperations();
            await TestPerformanceAndMemory();

            Console.WriteLine("\n" + "=" * 60);
            Console.WriteLine("🎉 تم إنجاز جميع الاختبارات بنجاح!");
            Console.WriteLine("🎉 All tests completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ خطأ في الاختبارات: {ex.Message}");
            Console.WriteLine($"❌ Test error: {ex.Message}");
            Console.WriteLine($"📍 تفاصيل الخطأ: {ex.StackTrace}");
            Console.WriteLine($"📍 Error details: {ex.StackTrace}");
        }
    }
}

/// <summary>
/// نقطة الدخول الرئيسية للاختبارات
/// Main entry point for tests
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        await TestMemoryOptimizedIndexing.RunAllTests();
    }
}