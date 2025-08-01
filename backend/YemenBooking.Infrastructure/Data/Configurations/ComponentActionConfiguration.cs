using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان ComponentAction
/// ComponentAction entity configuration
/// </summary>
public class ComponentActionConfiguration : IEntityTypeConfiguration<ComponentAction>
{
    public void Configure(EntityTypeBuilder<ComponentAction> builder)
    {
        builder.ToTable("ComponentActions");

        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.Id)
            .HasColumnName("ActionId")
            .IsRequired();

        builder.Property(ca => ca.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(ca => ca.DeletedAt)
            .HasColumnType("datetime");

        builder.Property(ca => ca.ComponentId)
            .IsRequired()
            .HasComment("Identifier of the home screen component");

        builder.Property(ca => ca.ActionType)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Type of action (Navigate, OpenModal, CallAPI, Share)");

        builder.Property(ca => ca.ActionTrigger)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Trigger of action (Click, LongPress, Swipe)");

        builder.Property(ca => ca.ActionTarget)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Target of action (Screen name, URL, API endpoint)");

        builder.Property(ca => ca.ActionParams)
            .HasColumnType("TEXT")
            .HasComment("JSON parameters for the action");

        builder.Property(ca => ca.Conditions)
            .HasColumnType("TEXT")
            .HasComment("JSON conditions for the action");

        builder.Property(ca => ca.RequiresAuth)
            .IsRequired()
            .HasComment("Whether authentication is required for the action");

        builder.Property(ca => ca.AnimationType)
            .HasMaxLength(50)
            .HasComment("Type of animation for the action");

        builder.Property(ca => ca.Priority)
            .HasDefaultValue(0)
            .HasComment("Priority of the action");

        builder.Property(ca => ca.CreatedAt).IsRequired();
        builder.Property(ca => ca.UpdatedAt).IsRequired();

        builder.HasOne<HomeScreenComponent>()
            .WithMany(hsc => hsc.Actions)
            .HasForeignKey(ca => ca.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ca => !ca.IsDeleted);
    }
}