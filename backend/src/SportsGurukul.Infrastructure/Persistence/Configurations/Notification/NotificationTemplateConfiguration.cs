using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.SubjectTemplate)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.BodyTemplate)
            .IsRequired();

        builder.Property(t => t.CurrentVersion)
            .HasDefaultValue(1);

        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_NotificationTemplates_Name");

        builder.HasIndex(t => new { t.ChannelType, t.IsActive })
            .HasDatabaseName("IX_NotificationTemplates_ChannelType_IsActive");

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
