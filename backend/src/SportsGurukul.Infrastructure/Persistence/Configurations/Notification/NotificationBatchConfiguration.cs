using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationBatchConfiguration : IEntityTypeConfiguration<NotificationBatch>
{
    public void Configure(EntityTypeBuilder<NotificationBatch> builder)
    {
        builder.ToTable("NotificationBatches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Metadata)
            .HasMaxLength(4000);

        builder.HasIndex(b => b.Status)
            .HasDatabaseName("IX_NotificationBatches_Status");

        builder.HasIndex(b => b.CreatedAt)
            .HasDatabaseName("IX_NotificationBatches_CreatedAt");

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
