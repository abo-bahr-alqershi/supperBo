using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Events;
using Newtonsoft.Json;

namespace AdvancedIndexingSystem.Demo;

/// <summary>
/// برنامج العرض التوضيحي لنظام الفهرسة المتقدم
/// Advanced Indexing System Demo Program
/// </summary>
class Program
{
    /// <summary>
    /// نقطة الدخول الرئيسية للبرنامج
    /// Main entry point of the program
    /// </summary>
    /// <param name="args">معاملات سطر الأوامر - Command line arguments</param>
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("    نظام الفهرسة المتقدم والديناميكي - Advanced Dynamic Indexing System");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        try
        {
            await RunDemoAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ حدث خطأ في البرنامج: {ex.Message}");
            Console.WriteLine($"   Error occurred: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   التفاصيل: {ex.InnerException.Message}");
                Console.WriteLine($"   Details: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("اضغط أي مفتاح للخروج... Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// تشغيل العرض التوضيحي
    /// Run the demonstration
    /// </summary>
    static async Task RunDemoAsync()
    {
        Console.WriteLine("🚀 بدء العرض التوضيحي...");
        Console.WriteLine("   Starting demonstration...");
        Console.WriteLine();

        // عرض المفاهيم الأساسية
        await DemonstrateBasicConceptsAsync();
        
        // عرض إنشاء الفهارس
        await DemonstrateIndexCreationAsync();
        
        // عرض عمليات البحث
        await DemonstrateSearchOperationsAsync();
        
        // عرض الفهارس الديناميكية
        await DemonstrateDynamicFieldsAsync();
        
        // عرض الأداء والإحصائيات
        await DemonstratePerformanceMetricsAsync();

        Console.WriteLine("✅ انتهى العرض التوضيحي بنجاح!");
        Console.WriteLine("   Demonstration completed successfully!");

        // تشغيل المثال العملي المتكامل
        // Run complete real-world example
        Console.WriteLine("\n" + "=".PadRight(80, '='));
        Console.WriteLine("🚀 تشغيل المثال العملي المتكامل...");
        Console.WriteLine("   Running complete real-world example...");
        Console.WriteLine("=".PadRight(80, '='));

        try
        {
            var realWorldExample = new RealWorldExample();
            await realWorldExample.RunCompleteExampleAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ خطأ في المثال العملي: {ex.Message}");
            Console.WriteLine($"   Error in real-world example: {ex.Message}");
        }

        // تشغيل العرض التوضيحي الشامل للمميزات الجديدة
        // Run comprehensive demo of new features
        Console.WriteLine("\n" + "=".PadRight(80, '='));
        Console.WriteLine("🚀 تشغيل العرض التوضيحي الشامل للمميزات الجديدة...");
        Console.WriteLine("   Running comprehensive demo of new features...");
        Console.WriteLine("=".PadRight(80, '='));

        try
        {
            var comprehensiveDemo = new ComprehensiveDemo();
            await comprehensiveDemo.RunComprehensiveDemoAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ خطأ في العرض التوضيحي الشامل: {ex.Message}");
            Console.WriteLine($"   Error in comprehensive demo: {ex.Message}");
        }

        Console.WriteLine("\n" + "=".PadRight(80, '='));
        Console.WriteLine("🎉 تم إكمال جميع الأمثلة بنجاح!");
        Console.WriteLine("   All examples completed successfully!");
        Console.WriteLine("=".PadRight(80, '='));
    }

    /// <summary>
    /// عرض المفاهيم الأساسية
    /// Demonstrate basic concepts
    /// </summary>
    static async Task DemonstrateBasicConceptsAsync()
    {
        Console.WriteLine("📋 المفاهيم الأساسية - Basic Concepts");
        Console.WriteLine("━".PadRight(50, '━'));

        // عرض أنواع الفهارس المدعومة
        Console.WriteLine("🔍 أنواع الفهارس المدعومة - Supported Index Types:");
        foreach (IndexType indexType in Enum.GetValues<IndexType>())
        {
            Console.WriteLine($"   • {indexType} - {GetIndexTypeDescription(indexType)}");
        }

        Console.WriteLine();

        // عرض أنواع البيانات المدعومة
        Console.WriteLine("📊 أنواع البيانات المدعومة - Supported Data Types:");
        foreach (FieldDataType dataType in Enum.GetValues<FieldDataType>())
        {
            Console.WriteLine($"   • {dataType} - {GetDataTypeDescription(dataType)}");
        }

        Console.WriteLine();
        await Task.Delay(1000); // محاكاة وقت المعالجة
    }

    /// <summary>
    /// عرض إنشاء الفهارس
    /// Demonstrate index creation
    /// </summary>
    static async Task DemonstrateIndexCreationAsync()
    {
        Console.WriteLine("🏗️  إنشاء الفهارس - Index Creation");
        Console.WriteLine("━".PadRight(50, '━'));

        // إنشاء تكوين فهرس الأسعار
        var priceIndexConfig = new IndexConfiguration
        {
            IndexId = "price-index-001",
            IndexName = "PriceIndex",
            ArabicName = "فهرس الأسعار",
            Description = "فهرس لتصنيف العقارات حسب نطاقات الأسعار",
            IndexType = IndexType.PriceIndex,
            Priority = IndexPriority.High,
            Status = IndexStatus.Building,
            IndexedFields = new List<string> { "price", "currency", "priceRange" },
            CustomSettings = new Dictionary<string, object>
            {
                ["min_price"] = 0,
                ["max_price"] = 10000,
                ["currency"] = "USD",
                ["auto_generate_ranges"] = true,
                ["range_size"] = 500
            }
        };

        Console.WriteLine($"✅ تم إنشاء تكوين فهرس الأسعار:");
        Console.WriteLine($"   Created price index configuration:");
        Console.WriteLine($"   📝 المعرف: {priceIndexConfig.IndexId}");
        Console.WriteLine($"      ID: {priceIndexConfig.IndexId}");
        Console.WriteLine($"   📝 الاسم: {priceIndexConfig.ArabicName}");
        Console.WriteLine($"      Name: {priceIndexConfig.IndexName}");
        Console.WriteLine($"   📝 النوع: {priceIndexConfig.IndexType}");
        Console.WriteLine($"      Type: {priceIndexConfig.IndexType}");
        Console.WriteLine($"   📝 الأولوية: {priceIndexConfig.Priority}");
        Console.WriteLine($"      Priority: {priceIndexConfig.Priority}");

        Console.WriteLine();

        // إنشاء تكوين فهرس المدن
        var cityIndexConfig = new IndexConfiguration
        {
            IndexId = "city-index-001",
            IndexName = "CityIndex",
            ArabicName = "فهرس المدن",
            Description = "فهرس لتصنيف العقارات حسب المدن والمناطق",
            IndexType = IndexType.CityIndex,
            Priority = IndexPriority.Critical,
            Status = IndexStatus.Building,
            IndexedFields = new List<string> { "city", "region", "country" },
            CustomSettings = new Dictionary<string, object>
            {
                ["supported_cities"] = new List<string> { "صنعاء", "عدن", "تعز", "الحديدة", "إب" },
                ["auto_detect_regions"] = true,
                ["case_sensitive"] = false
            }
        };

        Console.WriteLine($"✅ تم إنشاء تكوين فهرس المدن:");
        Console.WriteLine($"   Created city index configuration:");
        Console.WriteLine($"   📝 المعرف: {cityIndexConfig.IndexId}");
        Console.WriteLine($"      ID: {cityIndexConfig.IndexId}");
        Console.WriteLine($"   📝 الاسم: {cityIndexConfig.ArabicName}");
        Console.WriteLine($"      Name: {cityIndexConfig.IndexName}");

        Console.WriteLine();
        await Task.Delay(1500);
    }

    /// <summary>
    /// عرض عمليات البحث
    /// Demonstrate search operations
    /// </summary>
    static async Task DemonstrateSearchOperationsAsync()
    {
        Console.WriteLine("🔍 عمليات البحث - Search Operations");
        Console.WriteLine("━".PadRight(50, '━'));

        // إنشاء طلب بحث بسيط
        var simpleSearchRequest = new SearchRequest
        {
            RequestId = Guid.NewGuid().ToString(),
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "city",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "صنعاء",
                    Weight = 1.0,
                    IsRequired = true
                }
            },
            PageNumber = 1,
            PageSize = 10,
            IncludeTotalCount = true,
            IncludeStatistics = true
        };

        Console.WriteLine("📋 طلب بحث بسيط - Simple Search Request:");
        Console.WriteLine($"   🔎 البحث عن: مدينة = 'صنعاء'");
        Console.WriteLine($"      Search for: city = 'صنعاء'");
        Console.WriteLine($"   📄 الصفحة: {simpleSearchRequest.PageNumber}");
        Console.WriteLine($"      Page: {simpleSearchRequest.PageNumber}");
        Console.WriteLine($"   📊 حجم الصفحة: {simpleSearchRequest.PageSize}");
        Console.WriteLine($"      Page size: {simpleSearchRequest.PageSize}");

        Console.WriteLine();

        // إنشاء طلب بحث متقدم
        var advancedSearchRequest = new SearchRequest
        {
            RequestId = Guid.NewGuid().ToString(),
            SearchCriteria = new List<SearchCriterion>
            {
                new SearchCriterion
                {
                    FieldName = "city",
                    CriterionType = SearchCriterionType.InList,
                    Values = new List<object> { "صنعاء", "عدن", "تعز" },
                    Weight = 1.5,
                    IsRequired = true
                },
                new SearchCriterion
                {
                    FieldName = "price",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 500,
                    MaxValue = 2000,
                    Weight = 1.0,
                    IsRequired = true
                },
                new SearchCriterion
                {
                    FieldName = "amenities",
                    CriterionType = SearchCriterionType.Contains,
                    Value = "wifi",
                    Weight = 0.5,
                    IsRequired = false
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new SortCriterion
                {
                    FieldName = "price",
                    Direction = SortDirection.Ascending,
                    Priority = 1,
                    DataType = FieldDataType.Number
                },
                new SortCriterion
                {
                    FieldName = "city",
                    Direction = SortDirection.Ascending,
                    Priority = 2,
                    DataType = FieldDataType.Text
                }
            },
            PageNumber = 1,
            PageSize = 20,
            TimeoutMs = 5000
        };

        Console.WriteLine("🔍 طلب بحث متقدم - Advanced Search Request:");
        Console.WriteLine($"   🏙️  المدن: صنعاء، عدن، تعز");
        Console.WriteLine($"      Cities: Sana'a, Aden, Taiz");
        Console.WriteLine($"   💰 نطاق السعر: 500 - 2000");
        Console.WriteLine($"      Price range: 500 - 2000");
        Console.WriteLine($"   📶 المرافق: واي فاي (اختياري)");
        Console.WriteLine($"      Amenities: WiFi (optional)");
        Console.WriteLine($"   📊 الترتيب: حسب السعر ثم المدينة");
        Console.WriteLine($"      Sort: by price then city");

        Console.WriteLine();

        // محاكاة نتيجة البحث
        var searchResult = new SearchResult<Dictionary<string, object>>
        {
            RequestId = advancedSearchRequest.RequestId,
            Items = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["id"] = "PROP-001",
                    ["title"] = "شقة مفروشة - حي الستين - صنعاء",
                    ["city"] = "صنعاء",
                    ["price"] = 800,
                    ["amenities"] = new List<string> { "wifi", "parking", "elevator" }
                },
                new Dictionary<string, object>
                {
                    ["id"] = "PROP-002", 
                    ["title"] = "بيت عائلي - كريتر - عدن",
                    ["city"] = "عدن",
                    ["price"] = 1200,
                    ["amenities"] = new List<string> { "wifi", "garden", "security" }
                }
            },
            TotalCount = 25,
            PageNumber = 1,
            PageSize = 20,
            TotalPages = 2,
            HasNextPage = true,
            ExecutionTimeMs = 12.5,
            IsSuccessful = true,
            UsedIndices = new List<string> { "city-index-001", "price-index-001", "amenity-index-001" }
        };

        Console.WriteLine("📊 نتائج البحث - Search Results:");
        Console.WriteLine($"   ✅ العدد الإجمالي: {searchResult.TotalCount}");
        Console.WriteLine($"      Total count: {searchResult.TotalCount}");
        Console.WriteLine($"   ⏱️  وقت التنفيذ: {searchResult.ExecutionTimeMs} مللي ثانية");
        Console.WriteLine($"      Execution time: {searchResult.ExecutionTimeMs} ms");
        Console.WriteLine($"   🔗 الفهارس المستخدمة: {string.Join(", ", searchResult.UsedIndices)}");
        Console.WriteLine($"      Used indices: {string.Join(", ", searchResult.UsedIndices)}");

        Console.WriteLine();
        Console.WriteLine("   📋 عينة من النتائج - Sample Results:");
        foreach (var item in searchResult.Items.Take(2))
        {
            Console.WriteLine($"   • {item["title"]} - {item["city"]} - ${item["price"]}");
        }

        Console.WriteLine();
        await Task.Delay(2000);
    }

    /// <summary>
    /// عرض الحقول الديناميكية
    /// Demonstrate dynamic fields
    /// </summary>
    static async Task DemonstrateDynamicFieldsAsync()
    {
        Console.WriteLine("⚡ الحقول الديناميكية - Dynamic Fields");
        Console.WriteLine("━".PadRight(50, '━'));

        // إنشاء تكوين حقل ديناميكي رقمي
        var bedroomsField = new DynamicFieldConfiguration
        {
            FieldId = "field-bedrooms-001",
            FieldName = "bedrooms",
            ArabicName = "عدد غرف النوم",
            DataType = FieldDataType.Number,
            IsRequired = false,
            IsSearchable = true,
            IsSortable = true,
            ValidationRules = new ValidationRules
            {
                MinValue = 1,
                MaxValue = 10,
                ErrorMessage = "عدد غرف النوم يجب أن يكون بين 1 و 10"
            },
            RangeSettings = new RangeSettings
            {
                AutoGenerateRanges = true,
                AutoRangeSize = 1,
                CustomRanges = new List<RangeDefinition>
                {
                    new RangeDefinition { Name = "1-2", MinValue = 1, MaxValue = 2, MaxInclusive = true },
                    new RangeDefinition { Name = "3-4", MinValue = 3, MaxValue = 4, MaxInclusive = true },
                    new RangeDefinition { Name = "5+", MinValue = 5, MaxValue = 10, MaxInclusive = true }
                }
            }
        };

        Console.WriteLine("🔢 حقل ديناميكي رقمي - Numeric Dynamic Field:");
        Console.WriteLine($"   📝 الاسم: {bedroomsField.ArabicName} ({bedroomsField.FieldName})");
        Console.WriteLine($"      Name: {bedroomsField.ArabicName} ({bedroomsField.FieldName})");
        Console.WriteLine($"   📊 النوع: {bedroomsField.DataType}");
        Console.WriteLine($"      Type: {bedroomsField.DataType}");
        Console.WriteLine($"   🔍 قابل للبحث: {(bedroomsField.IsSearchable ? "نعم" : "لا")}");
        Console.WriteLine($"      Searchable: {(bedroomsField.IsSearchable ? "Yes" : "No")}");
        Console.WriteLine($"   📈 قابل للفرز: {(bedroomsField.IsSortable ? "نعم" : "لا")}");
        Console.WriteLine($"      Sortable: {(bedroomsField.IsSortable ? "Yes" : "No")}");
        Console.WriteLine($"   📏 النطاق: {bedroomsField.ValidationRules.MinValue} - {bedroomsField.ValidationRules.MaxValue}");
        Console.WriteLine($"      Range: {bedroomsField.ValidationRules.MinValue} - {bedroomsField.ValidationRules.MaxValue}");

        Console.WriteLine();

        // إنشاء تكوين حقل ديناميكي قائمة اختيار
        var heatingTypeField = new DynamicFieldConfiguration
        {
            FieldId = "field-heating-002",
            FieldName = "heating_type",
            ArabicName = "نوع التدفئة",
            DataType = FieldDataType.Select,
            IsRequired = false,
            IsSearchable = true,
            IsSortable = false,
            AllowedValues = new List<string> { "مركزية", "فردية", "غاز", "كهربائية", "بدون" },
            DefaultValue = "بدون"
        };

        Console.WriteLine("📋 حقل ديناميكي قائمة اختيار - Select Dynamic Field:");
        Console.WriteLine($"   📝 الاسم: {heatingTypeField.ArabicName} ({heatingTypeField.FieldName})");
        Console.WriteLine($"      Name: {heatingTypeField.ArabicName} ({heatingTypeField.FieldName})");
        Console.WriteLine($"   📊 النوع: {heatingTypeField.DataType}");
        Console.WriteLine($"      Type: {heatingTypeField.DataType}");
        Console.WriteLine($"   📋 الخيارات المتاحة:");
        Console.WriteLine($"      Available options:");
        foreach (var option in heatingTypeField.AllowedValues)
        {
            Console.WriteLine($"      • {option}");
        }
        Console.WriteLine($"   🔧 القيمة الافتراضية: {heatingTypeField.DefaultValue}");
        Console.WriteLine($"      Default value: {heatingTypeField.DefaultValue}");

        Console.WriteLine();
        await Task.Delay(1500);
    }

    /// <summary>
    /// عرض معايير الأداء والإحصائيات
    /// Demonstrate performance metrics and statistics
    /// </summary>
    static async Task DemonstratePerformanceMetricsAsync()
    {
        Console.WriteLine("📊 معايير الأداء والإحصائيات - Performance Metrics & Statistics");
        Console.WriteLine("━".PadRight(70, '━'));

        // إنشاء معايير أداء تجريبية
        var performanceMetrics = new PerformanceMetrics
        {
            AverageSearchTimeMs = 8.5,
            AverageUpdateTimeMs = 3.2,
            IndexSizeBytes = 2560000, // 2.5 MB
            IndexedItemsCount = 1500,
            SuccessRate = 0.998,
            SearchesPerSecond = 125.7,
            LastRebuildTime = DateTime.UtcNow
        };

        Console.WriteLine("⚡ معايير الأداء - Performance Metrics:");
        Console.WriteLine($"   🔍 متوسط وقت البحث: {performanceMetrics.AverageSearchTimeMs} مللي ثانية");
        Console.WriteLine($"      Average search time: {performanceMetrics.AverageSearchTimeMs} ms");
        Console.WriteLine($"   🔄 متوسط وقت التحديث: {performanceMetrics.AverageUpdateTimeMs} مللي ثانية");
        Console.WriteLine($"      Average update time: {performanceMetrics.AverageUpdateTimeMs} ms");
        Console.WriteLine($"   💾 حجم الفهرس: {performanceMetrics.IndexSizeBytes / 1024.0 / 1024.0:F2} ميجابايت");
        Console.WriteLine($"      Index size: {performanceMetrics.IndexSizeBytes / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"   📊 عدد العناصر المفهرسة: {performanceMetrics.IndexedItemsCount:N0}");
        Console.WriteLine($"      Indexed items count: {performanceMetrics.IndexedItemsCount:N0}");
        Console.WriteLine($"   ✅ معدل النجاح: {performanceMetrics.SuccessRate:P2}");
        Console.WriteLine($"      Success rate: {performanceMetrics.SuccessRate:P2}");
        Console.WriteLine($"   🚀 عمليات البحث في الثانية: {performanceMetrics.SearchesPerSecond:F1}");
        Console.WriteLine($"      Searches per second: {performanceMetrics.SearchesPerSecond:F1}");

        Console.WriteLine();

        // إنشاء إحصائيات تجريبية
        var indexStatistics = new PerformanceMetrics
        {
            IndexId = "comprehensive-index-001",
            TotalItems = 1500,
            IndexSizeBytes = 2560000,
            AverageSearchTimeMs = 12.5,
            TotalSearchOperations = 12500,
            TotalAddOperations = 1500,
            TotalUpdateOperations = 450,
            TotalRemoveOperations = 25
        };

        Console.WriteLine("📈 إحصائيات الفهرس - Index Statistics:");
        Console.WriteLine($"   🏗️  إجمالي العناصر: {indexStatistics.TotalItems:N0}");
        Console.WriteLine($"      Total items: {indexStatistics.TotalItems:N0}");
        Console.WriteLine($"   📊 حجم البيانات: {indexStatistics.IndexSizeBytes / 1024.0 / 1024.0:F2} ميجابايت");
        Console.WriteLine($"      Data size: {indexStatistics.IndexSizeBytes / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"   🔄 متوسط وقت البحث: {indexStatistics.AverageSearchTimeMs:F2} مللي ثانية");
        Console.WriteLine($"      Average search time: {indexStatistics.AverageSearchTimeMs:F2} ms");

        Console.WriteLine();
        Console.WriteLine("   🔄 إحصائيات العمليات - Operation Statistics:");
        Console.WriteLine($"      • عمليات الإضافة: {indexStatistics.TotalAddOperations:N0}");
        Console.WriteLine($"        Add operations: {indexStatistics.TotalAddOperations:N0}");
        Console.WriteLine($"      • عمليات التحديث: {indexStatistics.TotalUpdateOperations:N0}");
        Console.WriteLine($"        Update operations: {indexStatistics.TotalUpdateOperations:N0}");
        Console.WriteLine($"      • عمليات البحث: {indexStatistics.TotalSearchOperations:N0}");
        Console.WriteLine($"        Search operations: {indexStatistics.TotalSearchOperations:N0}");

        Console.WriteLine();
        await Task.Delay(2000);
    }

    /// <summary>
    /// الحصول على وصف نوع الفهرس
    /// Get index type description
    /// </summary>
    /// <param name="indexType">نوع الفهرس - Index type</param>
    /// <returns>الوصف - Description</returns>
    static string GetIndexTypeDescription(IndexType indexType)
    {
        return indexType switch
        {
            IndexType.PriceIndex => "فهرس الأسعار",
            IndexType.CityIndex => "فهرس المدن",
            IndexType.AmenityIndex => "فهرس المرافق",
            IndexType.DynamicFieldIndex => "فهرس الحقول الديناميكية",
            IndexType.TextIndex => "فهرس النصوص",
            IndexType.DateIndex => "فهرس التواريخ",
            IndexType.BooleanIndex => "فهرس منطقي",
            IndexType.CustomIndex => "فهرس مخصص",
            _ => "غير معروف"
        };
    }

    /// <summary>
    /// الحصول على وصف نوع البيانات
    /// Get data type description
    /// </summary>
    /// <param name="dataType">نوع البيانات - Data type</param>
    /// <returns>الوصف - Description</returns>
    static string GetDataTypeDescription(FieldDataType dataType)
    {
        return dataType switch
        {
            FieldDataType.Text => "نص",
            FieldDataType.Number => "رقم",
            FieldDataType.Date => "تاريخ",
            FieldDataType.Boolean => "منطقي",
            FieldDataType.Select => "قائمة اختيار",
            FieldDataType.MultiSelect => "قائمة متعددة الاختيار",
            FieldDataType.NumericRange => "نطاق رقمي",
            FieldDataType.DateRange => "نطاق تاريخ",
            _ => "غير معروف"
        };
    }
}
