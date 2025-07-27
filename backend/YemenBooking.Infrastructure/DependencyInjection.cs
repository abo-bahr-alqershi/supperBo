using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Infrastructure.Services;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Services;
using AdvancedIndexingSystem.Core.Models;

namespace YemenBooking.Infrastructure;

/// <summary>
/// إعداد حقن التبعيات للبنية التحتية
/// Infrastructure dependency injection setup
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// إضافة خدمات البنية التحتية
    /// Add infrastructure services
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // إضافة خدمات الفهرسة المتقدمة
        AddIndexingServices(services);

        // إضافة خدمات أخرى
        services.AddScoped<IIndexingService, IndexingService>();

        return services;
    }

    /// <summary>
    /// إضافة خدمات الفهرسة المتقدمة
    /// Add advanced indexing services
    /// </summary>
    private static void AddIndexingServices(IServiceCollection services)
    {
        // إعداد الفهارس الأساسية
        services.AddSingleton<IAdvancedIndex<PropertyIndexItem>>(provider =>
        {
            var config = new IndexConfiguration
            {
                IndexId = "properties",
                IndexName = "Properties Index",
                Description = "فهرس العقارات",
                IndexType = IndexType.Hash,
                MaxItems = 100000
            };
            return new AdvancedIndexService<PropertyIndexItem>(config);
        });

        services.AddSingleton<IAdvancedIndex<UnitIndexItem>>(provider =>
        {
            var config = new IndexConfiguration
            {
                IndexId = "units",
                IndexName = "Units Index",
                Description = "فهرس الوحدات",
                IndexType = IndexType.Hash,
                MaxItems = 500000
            };
            return new AdvancedIndexService<UnitIndexItem>(config);
        });

        services.AddSingleton<IAdvancedIndex<PricingIndexItem>>(provider =>
        {
            var config = new IndexConfiguration
            {
                IndexId = "pricing",
                IndexName = "Pricing Index",
                Description = "فهرس التسعير",
                IndexType = IndexType.PriceIndex,
                MaxItems = 1000000
            };
            return new AdvancedIndexService<PricingIndexItem>(config);
        });

        services.AddSingleton<IAdvancedIndex<AvailabilityIndexItem>>(provider =>
        {
            var config = new IndexConfiguration
            {
                IndexId = "availability",
                IndexName = "Availability Index",
                Description = "فهرس الإتاحة",
                IndexType = IndexType.Dynamic,
                MaxItems = 2000000
            };
            return new AdvancedIndexService<AvailabilityIndexItem>(config);
        });

        services.AddSingleton<IAdvancedIndex<CityIndexItem>>(provider =>
        {
            var config = new IndexConfiguration
            {
                IndexId = "cities",
                IndexName = "Cities Index",
                Description = "فهرس المدن",
                IndexType = IndexType.CityIndex,
                MaxItems = 10000
            };
            return new AdvancedIndexService<CityIndexItem>(config);
        });

        services.AddSingleton<IAdvancedIndex<AmenityIndexItem>>(provider =>
        {
            var config = new IndexConfiguration
            {
                IndexId = "amenities",
                IndexName = "Amenities Index",
                Description = "فهرس المرافق",
                IndexType = IndexType.AmenityIndex,
                MaxItems = 50000
            };
            return new AdvancedIndexService<AmenityIndexItem>(config);
        });

        // إضافة خدمة الفهرسة المخصصة لنظام حجز العقارات اليمنية
        services.AddSingleton<IYemenBookingIndexService, YemenBookingIndexService>();
    }
}