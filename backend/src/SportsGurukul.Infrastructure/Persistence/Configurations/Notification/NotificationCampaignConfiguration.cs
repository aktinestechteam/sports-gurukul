using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationCampaignConfiguration : IEntityTypeConfiguration<NotificationCampaign>
{
    public void Configure(EntityTypeBuilder<NotificationCampaign> builder)
    {
        builder.ToTable("NotificationCampaigns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.TargetCriteria)
            .HasMaxLength(4000);

        builder.Property(c => c.Metadata)
            .HasMaxLength(4000);

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_NotificationCampaigns_Status");

        builder.HasIndex(c => new { c.ChannelType, c.Status })
            .HasDatabaseName("IX_NotificationCampaigns_ChannelType_Status");

        builder.HasOne(c => c.Template)
            .WithMany()
            .HasForeignKey(c => c.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
