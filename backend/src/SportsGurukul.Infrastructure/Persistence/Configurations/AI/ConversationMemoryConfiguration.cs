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

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MemoryType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Key)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(m => m.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(m => m.ConversationId)
            .HasDatabaseName("IX_ConversationMemories_ConversationId");

        builder.HasIndex(m => new { m.ConversationId, m.MemoryType, m.Key })
            .IsUnique()
            .HasDatabaseName("IX_ConversationMemories_Conversation_Type_Key");

        builder.HasIndex(m => m.ExpiresAt)
            .HasDatabaseName("IX_ConversationMemories_ExpiresAt");

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Memories)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
