using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AITokenUsageConfiguration : IEntityTypeConfiguration<AITokenUsage>
{
    public void Configure(EntityTypeBuilder<AITokenUsage> builder)
    {
        builder.ToTable("AITokenUsages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ModelName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ProviderName)
            .HasMaxLength(200);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 8);

        builder.Property(e => e.UserId)
            .HasMaxLength(100);

        builder.Property(e => e.SessionId)
            .HasMaxLength(200);

        builder.Property(e => e.RequestType)
            .HasMaxLength(100);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ConversationId)
            .HasDatabaseName("IX_AITokenUsages_ConversationId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_AITokenUsages_UserId");

        builder.HasIndex(e => e.ModelName)
            .HasDatabaseName("IX_AITokenUsages_ModelName");

        builder.HasIndex(e => e.ProviderName)
            .HasDatabaseName("IX_AITokenUsages_ProviderName");

        builder.HasOne(e => e.Conversation)
            .WithMany()
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Message)
            .WithMany()
            .HasForeignKey(e => e.MessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
