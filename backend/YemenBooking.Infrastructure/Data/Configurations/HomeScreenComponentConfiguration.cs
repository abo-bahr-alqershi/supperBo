using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان HomeScreenComponent
/// HomeScreenComponent entity configuration
/// </summary>
public class HomeScreenComponentConfiguration : IEntityTypeConfiguration<HomeScreenComponent>
{
    public void Configure(EntityTypeBuilder<HomeScreenComponent> builder)
    {
        builder.ToTable("HomeScreenComponents");

        builder.HasKey(hsc => hsc.Id);

        builder.Property(hsc => hsc.Id)
            .HasColumnName("ComponentId")
            .IsRequired();

        builder.Property(hsc => hsc.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(hsc => hsc.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(hsc => hsc.SectionId)
            .IsRequired()
            .HasComment("Identifier of the home screen section");

        builder.Property(hsc => hsc.ComponentType)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Type of the component (Banner, Carousel, CategoryGrid, etc.)");

        builder.Property(hsc => hsc.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Name of the component");

        builder.Property(hsc => hsc.Order)
            .HasDefaultValue(0)
            .HasComment("Order of the component in the section");

        builder.Property(hsc => hsc.IsVisible)
            .HasDefaultValue(true)
            .HasComment("Visibility status of the component");

        builder.Property(hsc => hsc.ColSpan)
            .HasDefaultValue(12)
            .HasComment("Column span of the component (1-12)");

        builder.Property(hsc => hsc.RowSpan)
            .HasDefaultValue(1)
            .HasComment("Row span of the component");

        builder.Property(hsc => hsc.Alignment)
            .HasMaxLength(20)
            .HasComment("Alignment of the component (left, center, right)");

        builder.Property(hsc => hsc.CustomClasses)
            .HasMaxLength(200)
            .HasComment("Custom CSS classes for the component");

        builder.Property(hsc => hsc.AnimationType)
            .HasMaxLength(50)
            .HasComment("Type of animation for the component");

        builder.Property(hsc => hsc.AnimationDuration)
            .HasDefaultValue(0)
            .HasComment("Duration of the animation in milliseconds");

        builder.Property(hsc => hsc.Conditions)
            .HasColumnType("TEXT")
            .HasComment("JSON conditions for component visibility or behavior");

        builder.Property(hsc => hsc.CreatedAt).IsRequired();
        builder.Property(hsc => hsc.UpdatedAt).IsRequired();

        builder.HasOne<HomeScreenSection>()
            .WithMany(hss => hss.Components)
            .HasForeignKey(hsc => hsc.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(hsc => hsc.Properties)
            .WithOne(cp => cp.Component)
            .HasForeignKey(cp => cp.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(hsc => hsc.Styles)
            .WithOne(cs => cs.Component)
            .HasForeignKey(cs => cs.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(hsc => hsc.Actions)
            .WithOne(ca => ca.Component)
            .HasForeignKey(ca => ca.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(hsc => hsc.DataSource)
            .WithOne(ds => ds.Component)
            .HasForeignKey<ComponentDataSource>(ds => ds.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(hsc => !hsc.IsDeleted);
    }
}