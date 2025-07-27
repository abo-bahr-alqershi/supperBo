using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان الكيان
/// Property entity configuration
/// </summary>
public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        // تعيين اسم الجدول
        // Set table name
        builder.ToTable("Properties");

        // تعيين المفتاح الأساسي
        // Set primary key
        builder.HasKey(p => p.Id);

        // تكوين الخصائص الأساسية من BaseEntity
        builder.Property(b => b.Id).HasColumnName("PropertyId").IsRequired();
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAt).HasColumnType("datetime");
        
        // تكوين الخصائص الأخرى
        builder.Property(p => p.OwnerId).IsRequired();
        builder.Property(p => p.TypeId).IsRequired();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.City).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Latitude).HasColumnType("decimal(9,6)");
        builder.Property(p => p.Longitude).HasColumnType("decimal(9,6)");
        builder.Property(p => p.StarRating).HasDefaultValue(0);
        builder.Property(p => p.Description).HasColumnType("TEXT");
        builder.Property(p => p.IsApproved).HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnType("datetime").IsRequired();
        builder.Property(p => p.BookingCount).HasDefaultValue(0);
        builder.Property(p => p.AverageRating).HasColumnType("decimal(5,2)").HasDefaultValue(0);
        
        // تكوين العلاقات
        builder.HasOne(p => p.Owner)
               .WithMany(o => o.Properties)
               .HasForeignKey(p => p.OwnerId);
        
        builder.HasOne(p => p.PropertyType)
               .WithMany(pt => pt.Properties)
               .HasForeignKey(p => p.TypeId);
        
        builder.HasMany(p => p.Units)
               .WithOne(u => u.Property)
               .HasForeignKey(u => u.PropertyId);
        
        builder.HasMany(p => p.Services)
               .WithOne(s => s.Property)
               .HasForeignKey(s => s.PropertyId);
        
        builder.HasMany(p => p.Policies)
               .WithOne(po => po.Property)
               .HasForeignKey(po => po.PropertyId);
        
        // Map Reviews relationship via PropertyId
        builder.HasMany(p => p.Reviews)
               .WithOne(r => r.Property)
               .HasForeignKey(r => r.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.Staff)
               .WithOne(s => s.Property)
               .HasForeignKey(s => s.PropertyId);
        
        builder.HasMany(p => p.Images)
               .WithOne(i => i.Property)
               .HasForeignKey(i => i.PropertyId);
        
        builder.HasMany(p => p.Amenities)
               .WithOne(pa => pa.Property)
               .HasForeignKey(pa => pa.PropertyId);
        
        // البلاغات المرتبطة بالكيان
        builder.HasMany(p => p.Reports)
               .WithOne(r => r.ReportedProperty)
               .HasForeignKey(r => r.ReportedPropertyId)
               .OnDelete(DeleteBehavior.SetNull);
        
        // تكوين الفهرس
        builder.HasIndex(p => new { p.Name, p.City });
    }
}
