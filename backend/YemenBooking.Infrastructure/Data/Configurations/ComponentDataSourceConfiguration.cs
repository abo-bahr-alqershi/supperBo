using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان ComponentDataSource
/// ComponentDataSource entity configuration
/// </summary>
public class ComponentDataSourceConfiguration : IEntityTypeConfiguration<ComponentDataSource>
{
    public void Configure(EntityTypeBuilder<ComponentDataSource> builder)
    {
        builder.ToTable("ComponentDataSources");

        builder.HasKey(cds => cds.Id);

        builder.Property(cds => cds.Id)
            .HasColumnName("DataSourceId")
            .IsRequired();

        builder.Property(cds => cds.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(cds => cds.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(cds => cds.ComponentId)
            .IsRequired()
            .HasComment("Identifier of the home screen component");

        builder.Property(cds => cds.SourceType)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Type of data source (Static, API, Database, Cache)");

        builder.Property(cds => cds.DataEndpoint)
            .HasMaxLength(200)
            .HasComment("Data endpoint URL or path");

        builder.Property(cds => cds.HttpMethod)
            .HasMaxLength(10)
            .HasComment("HTTP method for the data request");

        builder.Property(cds => cds.Headers)
            .HasColumnType("TEXT")
            .HasComment("JSON headers for the request");

        builder.Property(cds => cds.QueryParams)
            .HasColumnType("TEXT")
            .HasComment("JSON query parameters for the request");

        builder.Property(cds => cds.RequestBody)
            .HasColumnType("TEXT")
            .HasComment("JSON request body");

        builder.Property(cds => cds.DataMapping)
            .HasColumnType("TEXT")
            .HasComment("JSON field mapping");

        builder.Property(cds => cds.CacheKey)
            .HasMaxLength(100)
            .HasComment("Cache key for the data source");

        builder.Property(cds => cds.CacheDuration)
            .HasDefaultValue(0)
            .HasComment("Cache duration in minutes");

        builder.Property(cds => cds.RefreshTrigger)
            .HasMaxLength(50)
            .HasComment("Trigger for cache refresh (OnLoad, OnFocus, Manual, Timer)");

        builder.Property(cds => cds.RefreshInterval)
            .HasDefaultValue(0)
            .HasComment("Refresh interval in seconds");

        builder.Property(cds => cds.ErrorHandling)
            .HasColumnType("TEXT")
            .HasComment("JSON error handling configuration");

        builder.Property(cds => cds.MockData)
            .HasColumnType("TEXT")
            .HasComment("JSON mock data for development");

        builder.Property(cds => cds.UseMockInDev)
            .HasDefaultValue(false)
            .HasComment("Flag indicating mock data usage in development");

        builder.Property(cds => cds.CreatedAt).IsRequired();
        builder.Property(cds => cds.UpdatedAt).IsRequired();

        builder.HasOne<HomeScreenComponent>()
            .WithOne(hsc => hsc.DataSource)
            .HasForeignKey<ComponentDataSource>(cds => cds.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(cds => !cds.IsDeleted);
    }
}