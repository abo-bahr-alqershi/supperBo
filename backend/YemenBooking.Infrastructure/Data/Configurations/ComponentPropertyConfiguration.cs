using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان ComponentProperty
/// ComponentProperty entity configuration
/// </summary>
public class ComponentPropertyConfiguration : IEntityTypeConfiguration<ComponentProperty>
{
    public void Configure(EntityTypeBuilder<ComponentProperty> builder)
    {
        builder.ToTable("ComponentProperties");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Id)
            .HasColumnName("PropertyId")
            .IsRequired();

        builder.Property(cp => cp.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(cp => cp.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(cp => cp.ComponentId)
            .IsRequired()
            .HasComment("Identifier of the home screen component");

        builder.Property(cp => cp.PropertyKey)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Unique key for the property");

        builder.Property(cp => cp.PropertyName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Display name of the property");

        builder.Property(cp => cp.PropertyType)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Type of the property (text, number, boolean, select, color, image)");

        builder.Property(cp => cp.Value)
            .HasMaxLength(500)
            .HasComment("Current value of the property");

        builder.Property(cp => cp.DefaultValue)
            .HasMaxLength(500)
            .HasComment("Default value of the property");

        builder.Property(cp => cp.IsRequired)
            .HasDefaultValue(false)
            .HasComment("Whether the property is required");

        builder.Property(cp => cp.ValidationRules)
            .HasColumnType("TEXT")
            .HasComment("JSON validation rules");

        builder.Property(cp => cp.Options)
            .HasColumnType("TEXT")
            .HasComment("JSON options for select type");

        builder.Property(cp => cp.HelpText)
            .HasMaxLength(500)
            .HasComment("Help text for the property");

        builder.Property(cp => cp.Order)
            .HasDefaultValue(0)
            .HasComment("Order of the property in the component");

        builder.Property(cp => cp.CreatedAt).IsRequired();
        builder.Property(cp => cp.UpdatedAt).IsRequired();

        builder.HasOne<HomeScreenComponent>()
            .WithMany(hsc => hsc.Properties)
            .HasForeignKey(cp => cp.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(cp => !cp.IsDeleted);
    }
}