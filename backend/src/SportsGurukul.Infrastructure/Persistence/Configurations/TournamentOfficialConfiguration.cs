using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentOfficialConfiguration : IEntityTypeConfiguration<TournamentOfficial>
{
    public void Configure(EntityTypeBuilder<TournamentOfficial> builder)
    {
        builder.ToTable("TournamentOfficials");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OfficialName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(o => o.Email)
            .HasMaxLength(200);

        builder.Property(o => o.Phone)
            .HasMaxLength(20);

        builder.Property(o => o.RowVersion)
            .IsRowVersion();

        builder.HasIndex(o => o.TournamentId)
            .HasDatabaseName("IX_TournamentOfficials_TournamentId");

        builder.HasIndex(o => o.CoachId)
            .HasDatabaseName("IX_TournamentOfficials_CoachId");

        builder.HasIndex(o => o.Role)
            .HasDatabaseName("IX_TournamentOfficials_Role");

        builder.HasOne(o => o.Tournament)
            .WithMany(t => t.Officials)
            .HasForeignKey(o => o.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Coach)
            .WithMany()
            .HasForeignKey(o => o.CoachId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(o => o.CreatedBy);
        builder.Ignore(o => o.UpdatedBy);
    }
}
