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

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActorType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.Action)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.EntityType)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(a => a.DetailsJson)
            .HasMaxLength(8000);

        builder.Property(a => a.BeforeJson)
            .HasMaxLength(8000);

        builder.Property(a => a.AfterJson)
            .HasMaxLength(8000);

        builder.Property(a => a.ChangedFieldsJson)
            .HasMaxLength(8000);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.CorrelationId)
            .HasMaxLength(100);

        builder.Property(a => a.Severity)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(a => new { a.EntityType, a.EntityId })
            .HasDatabaseName("IX_AIAuditLogs_EntityType_EntityId");

        builder.HasIndex(a => a.Action)
            .HasDatabaseName("IX_AIAuditLogs_Action");

        builder.HasIndex(a => a.ActorUserId)
            .HasDatabaseName("IX_AIAuditLogs_ActorUserId");

        builder.HasIndex(a => a.CorrelationId)
            .HasDatabaseName("IX_AIAuditLogs_CorrelationId");

        builder.HasIndex(a => a.Severity)
            .HasDatabaseName("IX_AIAuditLogs_Severity");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_AIAuditLogs_CreatedAt");

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
