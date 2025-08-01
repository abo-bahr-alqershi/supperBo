using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان HomeScreenSection
/// HomeScreenSection entity configuration
/// </summary>
public class HomeScreenSectionConfiguration : IEntityTypeConfiguration<HomeScreenSection>
{
    public void Configure(EntityTypeBuilder<HomeScreenSection> builder)
    {
        builder.ToTable("HomeScreenSections");

        builder.HasKey(hss => hss.Id);

        builder.Property(hss => hss.Id)
            .HasColumnName("SectionId")
            .IsRequired();

        builder.Property(hss => hss.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(hss => hss.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(hss => hss.TemplateId)
            .IsRequired()
            .HasComment("Identifier of the home screen template");

        builder.Property(hss => hss.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Name of the section");

        builder.Property(hss => hss.Title)
            .HasMaxLength(200)
            .HasComment("Title of the section");

        builder.Property(hss => hss.Subtitle)
            .HasMaxLength(200)
            .HasComment("Subtitle of the section");

        builder.Property(hss => hss.Order)
            .HasDefaultValue(0)
            .HasComment("Order of the section in the template");

        builder.Property(hss => hss.IsVisible)
            .HasDefaultValue(true)
            .HasComment("Visibility status of the section");

        builder.Property(hss => hss.BackgroundColor)
            .HasMaxLength(50)
            .HasComment("Background color of the section");

        builder.Property(hss => hss.BackgroundImage)
            .HasMaxLength(200)
            .HasComment("Background image URL for the section");

        builder.Property(hss => hss.Padding)
            .HasMaxLength(20)
            .HasComment("Padding of the section");

        builder.Property(hss => hss.Margin)
            .HasMaxLength(20)
            .HasComment("Margin of the section");

        builder.Property(hss => hss.MinHeight)
            .HasDefaultValue(0)
            .HasComment("Minimum height of the section");

        builder.Property(hss => hss.MaxHeight)
            .HasDefaultValue(0)
            .HasComment("Maximum height of the section");

        builder.Property(hss => hss.CustomStyles)
            .HasColumnType("TEXT")
            .HasComment("JSON for custom styles");

        builder.Property(hss => hss.Conditions)
            .HasColumnType("TEXT")
            .HasComment("JSON conditions for section visibility");

        builder.Property(hss => hss.CreatedAt).IsRequired();
        builder.Property(hss => hss.UpdatedAt).IsRequired();

        builder.HasOne<HomeScreenTemplate>()
            .WithMany(hst => hst.Sections)
            .HasForeignKey(hss => hss.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(hss => hss.Components)
            .WithOne(hsc => hsc.Section)
            .HasForeignKey(hsc => hsc.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(hss => !hss.IsDeleted);
    }
}