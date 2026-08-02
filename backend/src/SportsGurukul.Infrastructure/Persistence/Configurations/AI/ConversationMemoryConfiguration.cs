using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class ConversationMemoryConfiguration : IEntityTypeConfiguration<ConversationMemory>
{
    public void Configure(EntityTypeBuilder<ConversationMemory> builder)
    {
        builder.ToTable("ConversationMemories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Importance)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.Summary)
            .HasMaxLength(2000);

        builder.Property(e => e.Keywords)
            .HasMaxLength(1000);

        builder.Property(e => e.Context)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ConversationId)
            .HasDatabaseName("IX_ConversationMemories_ConversationId");

        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_ConversationMemories_Type");

        builder.HasIndex(e => e.Importance)
            .HasDatabaseName("IX_ConversationMemories_Importance");

        builder.HasOne(e => e.Conversation)
            .WithMany(c => c.Memories)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
