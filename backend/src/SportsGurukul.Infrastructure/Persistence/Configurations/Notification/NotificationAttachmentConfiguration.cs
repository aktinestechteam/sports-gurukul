using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationAttachmentConfiguration : IEntityTypeConfiguration<NotificationAttachment>
{
    public void Configure(EntityTypeBuilder<NotificationAttachment> builder)
    {
        builder.ToTable("NotificationAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.FilePath)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.StorageType)
            .HasMaxLength(50)
            .HasDefaultValue("local");

        builder.HasIndex(a => a.NotificationId)
            .HasDatabaseName("IX_NotificationAttachments_NotificationId");

        builder.HasOne(a => a.Notification)
            .WithMany(n => n.Attachments)
            .HasForeignKey(a => a.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
