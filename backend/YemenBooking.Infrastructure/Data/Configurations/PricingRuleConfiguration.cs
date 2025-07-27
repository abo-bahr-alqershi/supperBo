using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان قواعد التسعير
/// Pricing rule entity configuration
/// </summary>
public class PricingRuleConfiguration : IEntityTypeConfiguration<PricingRule>
{
    public void Configure(EntityTypeBuilder<PricingRule> builder)
    {
        builder.ToTable("PricingRules");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("PricingRuleId")
            .IsRequired();
        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(p => p.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(p => p.UnitId)
            .IsRequired();
        builder.Property(p => p.PriceType)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.StartDate)
            .HasColumnType("datetime")
            .IsRequired();
        builder.Property(p => p.EndDate)
            .HasColumnType("datetime")
            .IsRequired();
        builder.Property(p => p.StartTime)
            .HasColumnType("time");
        builder.Property(p => p.EndTime)
            .HasColumnType("time");
        builder.Property(p => p.PriceAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        // Currency for price
        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);
        builder.Property(p => p.PricingTier)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(p => p.PercentageChange)
            .HasColumnType("decimal(5,2)");
        builder.Property(p => p.MinPrice)
            .HasColumnType("decimal(18,2)");
        builder.Property(p => p.MaxPrice)
            .HasColumnType("decimal(18,2)");
        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.HasIndex(p => new { p.UnitId, p.StartDate, p.EndDate });

        builder.HasOne<YemenBooking.Core.Entities.Unit>()
            .WithMany()
            .HasForeignKey(p => p.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
} 