using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان الدفع
/// Payment entity configuration
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .HasComment("معرف الدفع الفريد");

        builder.Property(p => p.BookingId)
            .IsRequired()
            .HasComment("معرف الحجز");

        // Money value object configuration
        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("Amount_Amount")
                .HasComment("مبلغ الدفع");

            money.Property(m => m.Currency)
                .HasMaxLength(3)
                .HasColumnName("Amount_Currency")
                .HasComment("عملة الدفع");
        });

        builder.Property(p => p.Method)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("طريقة الدفع");

        builder.Property(p => p.TransactionId)
            .HasMaxLength(100)
            .HasComment("معرف المعاملة");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("حالة الدفع");

        builder.Property(p => p.PaymentDate)
            .IsRequired()
            .HasColumnType("datetime")
            .HasComment("تاريخ الدفع");

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("حالة الحذف الناعم");

        builder.Property(p => p.DeletedAt)
            .HasComment("تاريخ الحذف");

        // Indexes
        builder.HasIndex(p => p.BookingId)
            .HasDatabaseName("IX_Payments_BookingId");

        builder.HasIndex(p => p.TransactionId)
            .IsUnique()
            .HasFilter("[TransactionId] IS NOT NULL")
            .HasDatabaseName("IX_Payments_TransactionId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Payments_Status");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Payments_IsDeleted");

        // Relationships
        builder.HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !p.IsDeleted);

        // تكوين الخصائص الأساسية من BaseEntity
        builder.Property(b => b.Id).HasColumnName("PaymentId").IsRequired();
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAt).HasColumnType("datetime");
    }
}
