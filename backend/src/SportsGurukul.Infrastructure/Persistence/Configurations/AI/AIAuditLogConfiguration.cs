using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AIAuditLogConfiguration : IEntityTypeConfiguration<AIAuditLog>
{
    public void Configure(EntityTypeBuilder<AIAuditLog> builder)
    {
        builder.ToTable("AIAuditLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Severity)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Action)
            .HasMaxLength(200);

        builder.Property(e => e.ActorId)
            .HasMaxLength(100);

        builder.Property(e => e.ActorType)
            .HasMaxLength(50);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.PreviousState)
            .HasMaxLength(8000);

        builder.Property(e => e.NewState)
            .HasMaxLength(8000);

        builder.Property(e => e.Message)
            .HasMaxLength(2000);

        builder.Property(e => e.Metadata)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.EntityType)
            .HasDatabaseName("IX_AIAuditLogs_EntityType");

        builder.HasIndex(e => e.EventType)
            .HasDatabaseName("IX_AIAuditLogs_EventType");

        builder.HasIndex(e => e.Severity)
            .HasDatabaseName("IX_AIAuditLogs_Severity");

        builder.HasIndex(e => e.ActorId)
            .HasDatabaseName("IX_AIAuditLogs_ActorId");

        builder.HasIndex(e => e.EntityId)
            .HasDatabaseName("IX_AIAuditLogs_EntityId");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
