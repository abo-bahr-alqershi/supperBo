using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان ComponentStyle
/// ComponentStyle entity configuration
/// </summary>
public class ComponentStyleConfiguration : IEntityTypeConfiguration<ComponentStyle>
{
    public void Configure(EntityTypeBuilder<ComponentStyle> builder)
    {
        builder.ToTable("ComponentStyles");

        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Id)
            .HasColumnName("StyleId")
            .IsRequired();

        builder.Property(cs => cs.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(cs => cs.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(cs => cs.ComponentId)
            .IsRequired()
            .HasComment("Identifier of the home screen component");

        builder.Property(cs => cs.StyleKey)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Key of the style");

        builder.Property(cs => cs.StyleValue)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Value of the style");

        builder.Property(cs => cs.Unit)
            .HasMaxLength(20)
            .HasComment("Unit of measurement (px, %, em, rem)");

        builder.Property(cs => cs.IsImportant)
            .HasDefaultValue(false)
            .HasComment("Whether the style is marked as important");

        builder.Property(cs => cs.MediaQuery)
            .HasMaxLength(100)
            .HasComment("Media query for responsive styles");

        builder.Property(cs => cs.State)
            .HasMaxLength(20)
            .HasComment("State for which the style applies (hover, active, focus)");

        builder.Property(cs => cs.Platform)
            .HasMaxLength(20)
            .HasComment("Platform for which the style applies (iOS, Android, All)");

        builder.Property(cs => cs.CreatedAt).IsRequired();
        builder.Property(cs => cs.UpdatedAt).IsRequired();

        builder.HasOne<HomeScreenComponent>()
            .WithMany(hsc => hsc.Styles)
            .HasForeignKey(cs => cs.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(cs => !cs.IsDeleted);
    }
}