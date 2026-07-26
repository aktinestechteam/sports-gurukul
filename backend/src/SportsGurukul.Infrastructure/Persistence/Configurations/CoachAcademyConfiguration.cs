using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachAcademyConfiguration : IEntityTypeConfiguration<CoachAcademy>
{
    public void Configure(EntityTypeBuilder<CoachAcademy> builder)
    {
        builder.ToTable("CoachAcademies");

        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.AssignedDate)
            .IsRequired();

        builder.HasIndex(ca => ca.CoachId)
            .HasDatabaseName("IX_CoachAcademies_CoachId");

        builder.HasIndex(ca => ca.AcademyId)
            .HasDatabaseName("IX_CoachAcademies_AcademyId");

        builder.HasIndex(ca => new { ca.CoachId, ca.AcademyId })
            .IsUnique()
            .HasDatabaseName("IX_CoachAcademies_CoachId_AcademyId");

        builder.HasOne(ca => ca.Coach)
            .WithMany()
            .HasForeignKey(ca => ca.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.Academy)
            .WithMany()
            .HasForeignKey(ca => ca.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(ca => ca.CreatedBy);
        builder.Ignore(ca => ca.UpdatedBy);
    }
}
