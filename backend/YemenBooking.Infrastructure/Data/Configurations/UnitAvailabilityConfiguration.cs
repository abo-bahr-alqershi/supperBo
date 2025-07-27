using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان إتاحة الوحدة
/// Unit availability entity configuration
/// </summary>
public class UnitAvailabilityConfiguration : IEntityTypeConfiguration<UnitAvailability>
{
    public void Configure(EntityTypeBuilder<UnitAvailability> builder)
    {
        // Map to singular table name to match existing SQLite table
        builder.ToTable("UnitAvailability");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired();
        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(u => u.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(u => u.UnitId)
            .IsRequired();
        builder.Property(u => u.StartDate)
            .HasColumnType("datetime")
            .IsRequired();
        builder.Property(u => u.EndDate)
            .HasColumnType("datetime")
            .IsRequired();
        builder.Property(u => u.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(u => u.Reason)
            .HasMaxLength(50);
        builder.Property(u => u.Notes)
            .HasMaxLength(500);

        builder.HasIndex(u => new { u.UnitId, u.StartDate, u.EndDate });

        builder.HasOne<YemenBooking.Core.Entities.Unit>()
            .WithMany()
            .HasForeignKey(u => u.UnitId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // تكوين العلاقة مع كيان الحجز
        builder.Property(u => u.BookingId)
            .HasColumnName("BookingId")
            .IsRequired(false);
        builder.HasOne<YemenBooking.Core.Entities.Booking>()
            .WithMany(b => b.Availabilities)
            .HasForeignKey(u => u.BookingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
} 