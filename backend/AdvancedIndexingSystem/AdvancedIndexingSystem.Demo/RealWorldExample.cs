using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Interfaces;

namespace AdvancedIndexingSystem.Demo;

/// <summary>
/// مثال عملي متكامل لاستخدام نظام الفهرسة
/// Complete real-world example of using the indexing system
/// </summary>
public class RealWorldExample
{
    private readonly IndexManager _indexManager;
    private IAdvancedIndex<PropertyItem>? _propertyIndex;

    public RealWorldExample()
    {
        _indexManager = new IndexManager("demo_indices");
    }

    /// <summary>
    /// تشغيل المثال الكامل
    /// Run complete example
    /// </summary>
    public async Task RunCompleteExampleAsync()
    {
        Console.WriteLine("🏠 مثال عملي متكامل - نظام فهرسة العقارات");
        Console.WriteLine("   Complete Real-World Example - Property Indexing System");
        Console.WriteLine("═══════════════════════════════════════════════════════════");

        try
        {
            // 1. إنشاء الفهرس
            await CreatePropertyIndexAsync();

            // 2. إضافة بيانات تجريبية
            await AddSamplePropertiesAsync();

            // 3. تجربة عمليات البحث المختلفة
            await PerformSearchOperationsAsync();

            // 4. تجربة عمليات التحديث
            await PerformUpdateOperationsAsync();

            // 5. حفظ وتحميل الفهرس
            await SaveAndLoadIndexAsync();

            // 6. عرض الإحصائيات
            await DisplayStatisticsAsync();

            Console.WriteLine("\n✅ تم إكمال المثال بنجاح!");
            Console.WriteLine("   Example completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ خطأ في تشغيل المثال: {ex.Message}");
            Console.WriteLine($"   Error running example: {ex.Message}");
        }
        finally
        {
            _indexManager.Dispose();
        }
    }

    /// <summary>
    /// إنشاء فهرس العقارات
    /// Create property index
    /// </summary>
    private async Task CreatePropertyIndexAsync()
    {
        Console.WriteLine("\n📋 إنشاء فهرس العقارات...");
        Console.WriteLine("   Creating property index...");

        var configuration = new IndexConfiguration
        {
            IndexId = "property-index-001",
            IndexName = "PropertyIndex",
            IndexType = IndexType.CustomIndex,
            Priority = IndexPriority.High,
            IndexedFields = new List<string>
            {
                "city", "district", "price", "bedrooms", "bathrooms", 
                "area", "propertytype", "heatingtype", "amenities", 
                "isavailable", "status", "rating"
            },
            DynamicFields = new List<DynamicFieldConfiguration>
            {
                new()
                {
                    FieldName = "bedrooms",
                    DataType = FieldDataType.Number,
                    IsRequired = false,
                    IsSearchable = true,
                    IsSortable = true,
                    ValidationRules = new ValidationRules
                    {
                        MinValue = 0,
                        MaxValue = 10,
                        ErrorMessage = "عدد غرف النوم يجب أن يكون بين 0 و 10"
                    }
                },
                new()
                {
                    FieldName = "price",
                    DataType = FieldDataType.Number,
                    IsRequired = true,
                    IsSearchable = true,
                    IsSortable = true,
                    ValidationRules = new ValidationRules
                    {
                        MinValue = 100,
                        MaxValue = 1000000,
                        ErrorMessage = "السعر يجب أن يكون بين 100 و 1,000,000"
                    }
                },
                new()
                {
                    FieldName = "heatingtype",
                    DataType = FieldDataType.Select,
                    IsRequired = false,
                    IsSearchable = true,
                    IsSortable = false,
                    AllowedValues = new List<string> { "central", "individual", "gas", "electric", "none" },
                    DefaultValue = "none"
                }
            }
        };

        _propertyIndex = await _indexManager.CreateIndexAsync<PropertyItem>(configuration);

        if (_propertyIndex != null)
        {
            Console.WriteLine("✅ تم إنشاء فهرس العقارات بنجاح");
            Console.WriteLine("   Property index created successfully");
        }
        else
        {
            throw new Exception("فشل في إنشاء فهرس العقارات - Failed to create property index");
        }
    }

    /// <summary>
    /// إضافة بيانات تجريبية
    /// Add sample properties
    /// </summary>
    private async Task AddSamplePropertiesAsync()
    {
        Console.WriteLine("\n📊 إضافة بيانات تجريبية...");
        Console.WriteLine("   Adding sample data...");

        var sampleProperties = GenerateSampleProperties();
        var addedCount = 0;

        foreach (var property in sampleProperties)
        {
            if (await _propertyIndex!.AddItemAsync(property.Id, property))
            {
                addedCount++;
            }
        }

        Console.WriteLine($"✅ تم إضافة {addedCount} عقار من أصل {sampleProperties.Count}");
        Console.WriteLine($"   Added {addedCount} properties out of {sampleProperties.Count}");
    }

    /// <summary>
    /// تجربة عمليات البحث المختلفة
    /// Perform various search operations
    /// </summary>
    private async Task PerformSearchOperationsAsync()
    {
        Console.WriteLine("\n🔍 تجربة عمليات البحث...");
        Console.WriteLine("   Testing search operations...");

        // بحث بسيط - البحث عن عقارات في صنعاء
        await SimpleSearchExample();

        // بحث متقدم - البحث بمعايير متعددة
        await AdvancedSearchExample();

        // بحث بالنطاق - البحث في نطاق سعري
        await RangeSearchExample();

        // بحث في المرافق
        await AmenitySearchExample();
    }

    /// <summary>
    /// مثال البحث البسيط
    /// Simple search example
    /// </summary>
    private async Task SimpleSearchExample()
    {
        Console.WriteLine("\n  🏙️ البحث البسيط - العقارات في صنعاء:");
        Console.WriteLine("     Simple Search - Properties in Sana'a:");

        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new()
                {
                    FieldName = "city",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = "Sana'a"
                }
            },
            PageNumber = 1,
            PageSize = 5
        };

        var result = await _propertyIndex!.SearchAsync(searchRequest);

        if (result.Success && result.Items != null)
        {
            Console.WriteLine($"     📋 النتائج: {result.Statistics?.TotalCount} عقار");
            Console.WriteLine($"        Results: {result.Statistics?.TotalCount} properties");
            Console.WriteLine($"     ⏱️ وقت التنفيذ: {result.Statistics?.ExecutionTimeMs:F2}ms");
            Console.WriteLine($"        Execution time: {result.Statistics?.ExecutionTimeMs:F2}ms");

            foreach (var property in result.Items.Take(3))
            {
                Console.WriteLine($"        • {property.Title} - ${property.Price:N0}");
            }
        }
    }

    /// <summary>
    /// مثال البحث المتقدم
    /// Advanced search example
    /// </summary>
    private async Task AdvancedSearchExample()
    {
        Console.WriteLine("\n  🔍 البحث المتقدم - عقارات متعددة المعايير:");
        Console.WriteLine("     Advanced Search - Multi-criteria properties:");

        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new()
                {
                    FieldName = "city",
                    CriterionType = SearchCriterionType.InList,
                    Values = new List<object> { "Sana'a", "Aden", "Taiz" }
                },
                new()
                {
                    FieldName = "bedrooms",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 2,
                    MaxValue = 4
                },
                new()
                {
                    FieldName = "isavailable",
                    CriterionType = SearchCriterionType.ExactMatch,
                    Value = true
                }
            },
            SortCriteria = new List<SortCriterion>
            {
                new()
                {
                    FieldName = "price",
                    Direction = SortDirection.Ascending
                }
            },
            PageNumber = 1,
            PageSize = 5
        };

        var result = await _propertyIndex!.SearchAsync(searchRequest);

        if (result.Success && result.Items != null)
        {
            Console.WriteLine($"     📋 النتائج: {result.Statistics?.TotalCount} عقار");
            Console.WriteLine($"        Results: {result.Statistics?.TotalCount} properties");
            Console.WriteLine($"     ⏱️ وقت التنفيذ: {result.Statistics?.ExecutionTimeMs:F2}ms");
            Console.WriteLine($"        Execution time: {result.Statistics?.ExecutionTimeMs:F2}ms");
        }
    }

    /// <summary>
    /// مثال البحث بالنطاق
    /// Range search example
    /// </summary>
    private async Task RangeSearchExample()
    {
        Console.WriteLine("\n  💰 البحث بالنطاق السعري (500-2000):");
        Console.WriteLine("     Price Range Search (500-2000):");

        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new()
                {
                    FieldName = "price",
                    CriterionType = SearchCriterionType.InRange,
                    MinValue = 500,
                    MaxValue = 2000
                }
            },
            PageNumber = 1,
            PageSize = 5
        };

        var result = await _propertyIndex!.SearchAsync(searchRequest);

        if (result.Success && result.Items != null)
        {
            Console.WriteLine($"     📋 النتائج: {result.Statistics?.TotalCount} عقار");
            Console.WriteLine($"        Results: {result.Statistics?.TotalCount} properties");

            foreach (var property in result.Items.Take(3))
            {
                Console.WriteLine($"        • {property.Title} - ${property.Price:N0} - {property.City}");
            }
        }
    }

    /// <summary>
    /// مثال البحث في المرافق
    /// Amenity search example
    /// </summary>
    private async Task AmenitySearchExample()
    {
        Console.WriteLine("\n  🏊 البحث في المرافق (مسبح):");
        Console.WriteLine("     Amenity Search (Pool):");

        var searchRequest = new SearchRequest
        {
            SearchCriteria = new List<SearchCriterion>
            {
                new()
                {
                    FieldName = "amenities",
                    CriterionType = SearchCriterionType.Contains,
                    Value = "pool"
                }
            },
            PageNumber = 1,
            PageSize = 5
        };

        var result = await _propertyIndex!.SearchAsync(searchRequest);

        if (result.Success && result.Items != null)
        {
            Console.WriteLine($"     📋 النتائج: {result.Statistics?.TotalCount} عقار");
            Console.WriteLine($"        Results: {result.Statistics?.TotalCount} properties");
        }
    }

    /// <summary>
    /// تجربة عمليات التحديث
    /// Perform update operations
    /// </summary>
    private async Task PerformUpdateOperationsAsync()
    {
        Console.WriteLine("\n🔄 تجربة عمليات التحديث...");
        Console.WriteLine("   Testing update operations...");

        // تحديث سعر عقار
        var propertyToUpdate = new PropertyItem
        {
            Id = "prop-001",
            Title = "Luxury Apartment - Updated",
            Price = 1800,
            City = "Sana'a",
            District = "Al-Sitteen",
            Bedrooms = 3,
            Bathrooms = 2,
            Area = 120,
            PropertyType = "apartment",
            HeatingType = "central",
            Amenities = new List<string> { "wifi", "parking", "elevator", "security" },
            IsAvailable = true,
            Status = PropertyStatus.Active,
            UpdatedAt = DateTime.UtcNow
        };

        var updateResult = await _propertyIndex!.UpdateItemAsync("prop-001", propertyToUpdate);

        if (updateResult)
        {
            Console.WriteLine("✅ تم تحديث العقار بنجاح");
            Console.WriteLine("   Property updated successfully");
        }

        // حذف عقار
        var deleteResult = await _propertyIndex!.RemoveItemAsync("prop-010");

        if (deleteResult)
        {
            Console.WriteLine("✅ تم حذف العقار بنجاح");
            Console.WriteLine("   Property deleted successfully");
        }
    }

    /// <summary>
    /// حفظ وتحميل الفهرس
    /// Save and load index
    /// </summary>
    private async Task SaveAndLoadIndexAsync()
    {
        Console.WriteLine("\n💾 تجربة حفظ وتحميل الفهرس...");
        Console.WriteLine("   Testing save and load operations...");

        // حفظ الفهرس
        var saveResult = await _propertyIndex!.SaveToFileAsync("demo_indices/property-index-backup.json");

        if (saveResult)
        {
            Console.WriteLine("✅ تم حفظ الفهرس بنجاح");
            Console.WriteLine("   Index saved successfully");
        }

        // إعادة بناء الفهرس
        var rebuildResult = await _propertyIndex!.RebuildIndexAsync();

        if (rebuildResult)
        {
            Console.WriteLine("✅ تم إعادة بناء الفهرس بنجاح");
            Console.WriteLine("   Index rebuilt successfully");
        }
    }

    /// <summary>
    /// عرض الإحصائيات
    /// Display statistics
    /// </summary>
    private async Task DisplayStatisticsAsync()
    {
        Console.WriteLine("\n📊 إحصائيات الأداء:");
        Console.WriteLine("   Performance Statistics:");

        var stats = _propertyIndex!.GetStatistics();
        var globalStats = _indexManager.GetGlobalStatistics();

        Console.WriteLine($"   📈 إحصائيات الفهرس:");
        Console.WriteLine($"      Index Statistics:");
        Console.WriteLine($"      • العناصر المفهرسة: {_propertyIndex.ItemCount}");
        Console.WriteLine($"        Indexed items: {_propertyIndex.ItemCount}");
        Console.WriteLine($"      • عمليات الإضافة: {stats.TotalAddOperations}");
        Console.WriteLine($"        Add operations: {stats.TotalAddOperations}");
        Console.WriteLine($"      • عمليات البحث: {stats.TotalSearchOperations}");
        Console.WriteLine($"        Search operations: {stats.TotalSearchOperations}");
        Console.WriteLine($"      • عمليات التحديث: {stats.TotalUpdateOperations}");
        Console.WriteLine($"        Update operations: {stats.TotalUpdateOperations}");
        Console.WriteLine($"      • متوسط وقت البحث: {stats.AverageSearchTime:F2}ms");
        Console.WriteLine($"        Average search time: {stats.AverageSearchTime:F2}ms");

        Console.WriteLine($"\n   🌐 إحصائيات المدير العامة:");
        Console.WriteLine($"      Global Manager Statistics:");
        Console.WriteLine($"      • عدد الفهارس النشطة: {_indexManager.ActiveIndicesCount}");
        Console.WriteLine($"        Active indices: {_indexManager.ActiveIndicesCount}");
        Console.WriteLine($"      • عمليات الإنشاء: {globalStats.TotalCreateOperations}");
        Console.WriteLine($"        Create operations: {globalStats.TotalCreateOperations}");
        Console.WriteLine($"      • عمليات البحث الإجمالية: {globalStats.TotalSearchOperations}");
        Console.WriteLine($"        Total search operations: {globalStats.TotalSearchOperations}");
    }

    /// <summary>
    /// توليد بيانات تجريبية
    /// Generate sample properties
    /// </summary>
    private List<PropertyItem> GenerateSampleProperties()
    {
        var properties = new List<PropertyItem>();
        var cities = new[] { "Sana'a", "Aden", "Taiz", "Hodeidah", "Ibb" };
        var propertyTypes = new[] { "apartment", "house", "villa", "studio" };
        var heatingTypes = new[] { "central", "individual", "gas", "electric", "none" };
        var amenities = new[] { "wifi", "parking", "elevator", "security", "pool", "gym", "garden" };

        var random = new Random(42); // Fixed seed for consistent results

        for (int i = 1; i <= 50; i++)
        {
            var city = cities[random.Next(cities.Length)];
            var propertyType = propertyTypes[random.Next(propertyTypes.Length)];
            var heatingType = heatingTypes[random.Next(heatingTypes.Length)];
            var selectedAmenities = amenities.OrderBy(x => random.Next()).Take(random.Next(2, 5)).ToList();

            var property = new PropertyItem
            {
                Id = $"prop-{i:D3}",
                Title = $"{propertyType.ToTitleCase()} in {city} - Unit {i}",
                Description = $"Beautiful {propertyType} located in {city} with modern amenities.",
                Price = random.Next(300, 5000),
                Currency = "USD",
                City = city,
                District = GetDistrictForCity(city, random),
                Bedrooms = random.Next(1, 6),
                Bathrooms = random.Next(1, 4),
                Area = random.Next(50, 300),
                PropertyType = propertyType,
                HeatingType = heatingType,
                Amenities = selectedAmenities,
                IsAvailable = random.NextDouble() > 0.2, // 80% available
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                UpdatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                Owner = new OwnerInfo
                {
                    Name = $"Owner {i}",
                    Phone = $"+967-{random.Next(100000000, 999999999)}",
                    Email = $"owner{i}@example.com",
                    IsVerified = random.NextDouble() > 0.3 // 70% verified
                },
                Location = new LocationInfo
                {
                    Latitude = 15.3694 + (random.NextDouble() - 0.5) * 0.1,
                    Longitude = 44.1910 + (random.NextDouble() - 0.5) * 0.1,
                    FullAddress = $"Street {random.Next(1, 100)}, {city}",
                    PostalCode = $"{random.Next(10000, 99999)}"
                },
                Rating = Math.Round(random.NextDouble() * 4 + 1, 1), // 1.0 to 5.0
                ReviewsCount = random.Next(0, 50),
                Tags = new List<string> { city.ToLower(), propertyType, "modern" },
                Status = PropertyStatus.Active,
                AdditionalInfo = new Dictionary<string, object>
                {
                    ["furnished"] = random.NextDouble() > 0.5,
                    ["petFriendly"] = random.NextDouble() > 0.7,
                    ["yearBuilt"] = random.Next(1990, 2024)
                }
            };

            properties.Add(property);
        }

        return properties;
    }

    /// <summary>
    /// الحصول على منطقة للمدينة
    /// Get district for city
    /// </summary>
    private string GetDistrictForCity(string city, Random random)
    {
        var districts = city switch
        {
            "Sana'a" => new[] { "Al-Sitteen", "Al-Hasaba", "Shu'ub", "Al-Thawra", "Al-Wahdah" },
            "Aden" => new[] { "Crater", "Al-Mualla", "Khormaksar", "Al-Mansura", "Dar Saad" },
            "Taiz" => new[] { "Al-Qahirah", "Salah", "Al-Mudhaffar", "Mawza", "Al-Shamayatayn" },
            "Hodeidah" => new[] { "Al-Mina", "Al-Hawk", "Al-Hali", "Bayt Al-Faqih", "Al-Luhayyah" },
            "Ibb" => new[] { "Al-Nadi", "Al-Mashannah", "Yarim", "Dhi As-Sufal", "Hubaysh" },
            _ => new[] { "Central", "North", "South", "East", "West" }
        };

        return districts[random.Next(districts.Length)];
    }
}

/// <summary>
/// امتداد لتحويل النص إلى عنوان
/// Extension for title case conversion
/// </summary>
public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input[1..].ToLower();
    }
}