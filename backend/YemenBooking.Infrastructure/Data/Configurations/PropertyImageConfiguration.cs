using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان صورة الكيان
/// Property Image entity configuration
/// </summary>
public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("PropertyImages");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
            .IsRequired()
            .HasComment("معرف الصورة الفريد");

        builder.Property(pi => pi.PropertyId)
            .IsRequired(false)
            .HasComment("معرف الكيان");

        builder.Property(pi => pi.Url)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("مسار الصورة");

        builder.Property(pi => pi.AltText)
            .HasMaxLength(500)
            .HasComment("وصف الصورة");

        builder.Property(pi => pi.IsMain)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("هل هي الصورة الرئيسية");

        builder.Property(pi => pi.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("حالة الحذف الناعم");

        builder.Property(pi => pi.DeletedAt)
            .HasComment("تاريخ الحذف");

        // تكوين الخصائص الأساسية من BaseEntity
        builder.Property(b => b.Id).HasColumnName("ImageId").IsRequired();
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAt).HasColumnType("datetime");
        
        // تكوين الخصائص الأخرى
        builder.Property(pi => pi.Name).IsRequired().HasMaxLength(100);
        builder.Property(pi => pi.Url).IsRequired().HasMaxLength(500);
        builder.Property(pi => pi.SizeBytes).IsRequired();
        builder.Property(pi => pi.Type).HasMaxLength(50);
        builder.Property(pi => pi.Category).IsRequired();
        builder.Property(pi => pi.Caption).HasMaxLength(200);
        builder.Property(pi => pi.AltText).HasMaxLength(200);
        builder.Property(pi => pi.Tags).HasColumnType("TEXT");
        builder.Property(pi => pi.Sizes).HasColumnType("TEXT");
        builder.Property(pi => pi.IsMain).HasDefaultValue(false);
        builder.Property(pi => pi.SortOrder).HasDefaultValue(0);
        builder.Property(pi => pi.Views).HasDefaultValue(0);
        builder.Property(pi => pi.Downloads).HasDefaultValue(0);
        builder.Property(pi => pi.UploadedAt).HasColumnType("datetime").IsRequired();
        builder.Property(pi => pi.DisplayOrder).HasDefaultValue(0);
        builder.Property(pi => pi.Status).IsRequired();
        builder.Property(pi => pi.IsMainImage).HasDefaultValue(false);
        
        // تكوين العلاقات
        builder.HasOne(pi => pi.Property)
               .WithMany(p => p.Images)
               .HasForeignKey(pi => pi.PropertyId)
               .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(pi => pi.Unit)
               .WithMany(u => u.Images)
               .HasForeignKey(pi => pi.UnitId)
               .OnDelete(DeleteBehavior.SetNull);
        
        // تكوين الفهرس
        builder.HasIndex(pi => pi.PropertyId);
        builder.HasIndex(pi => pi.UnitId);

        builder.HasQueryFilter(pi => !pi.IsDeleted);
    }
}
