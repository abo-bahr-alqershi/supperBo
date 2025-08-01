using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان HomeScreenTemplate
/// HomeScreenTemplate entity configuration
/// </summary>
public class HomeScreenTemplateConfiguration : IEntityTypeConfiguration<HomeScreenTemplate>
{
    public void Configure(EntityTypeBuilder<HomeScreenTemplate> builder)
    {
        builder.ToTable("HomeScreenTemplates");

        builder.HasKey(hst => hst.Id);

        builder.Property(hst => hst.Id)
            .HasColumnName("TemplateId")
            .IsRequired();

        builder.Property(hst => hst.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(hst => hst.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(hst => hst.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Name of the template");

        builder.Property(hst => hst.Description)
            .HasMaxLength(500)
            .HasComment("Description of the template");

        builder.Property(hst => hst.Version)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Version of the template");

        builder.Property(hst => hst.IsDefault)
            .HasDefaultValue(false)
            .HasComment("Whether the template is default");

        builder.Property(hst => hst.PublishedAt)
            .HasColumnType("datetime")
            .HasComment("Publication date of the template");

        builder.Property(hst => hst.PublishedBy)
            .HasComment("Identifier of the user who published the template");

        builder.Property(hst => hst.Platform)
            .HasMaxLength(50)
            .HasComment("Target platform of the template (iOS, Android, All)");

        builder.Property(hst => hst.TargetAudience)
            .HasMaxLength(50)
            .HasComment("Target audience of the template (Guest, User, Premium)");

        builder.Property(hst => hst.MetaData)
            .HasColumnType("TEXT")
            .HasComment("JSON metadata for additional settings");

        builder.Property(hst => hst.CreatedAt).IsRequired();
        builder.Property(hst => hst.UpdatedAt).IsRequired();

        builder.HasMany(hst => hst.Sections)
            .WithOne(hss => hss.Template)
            .HasForeignKey(hss => hss.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(hst => !hst.IsDeleted);
    }
}