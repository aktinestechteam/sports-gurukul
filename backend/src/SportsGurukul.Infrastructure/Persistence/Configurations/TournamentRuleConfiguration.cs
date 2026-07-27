using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentRuleConfiguration : IEntityTypeConfiguration<TournamentRule>
{
    public void Configure(EntityTypeBuilder<TournamentRule> builder)
    {
        builder.ToTable("TournamentRules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RuleName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.RuleDescription)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(r => r.Category)
            .HasMaxLength(100);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        builder.HasIndex(r => r.TournamentId)
            .HasDatabaseName("IX_TournamentRules_TournamentId");

        builder.HasOne(r => r.Tournament)
            .WithMany(t => t.Rules_)
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
