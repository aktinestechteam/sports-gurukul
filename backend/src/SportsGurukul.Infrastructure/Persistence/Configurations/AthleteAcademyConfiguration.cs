using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AthleteAcademyConfiguration : IEntityTypeConfiguration<AthleteAcademy>
{
    public void Configure(EntityTypeBuilder<AthleteAcademy> builder)
    {
        builder.ToTable("AthleteAcademies");

        builder.HasKey(aa => aa.Id);

        builder.Property(aa => aa.RegisteredDate)
            .IsRequired();

        builder.HasIndex(aa => aa.AthleteId)
            .HasDatabaseName("IX_AthleteAcademies_AthleteId");

        builder.HasIndex(aa => aa.AcademyId)
            .HasDatabaseName("IX_AthleteAcademies_AcademyId");

        builder.HasIndex(aa => new { aa.AthleteId, aa.AcademyId })
            .IsUnique()
            .HasDatabaseName("IX_AthleteAcademies_AthleteId_AcademyId");

        builder.HasOne(aa => aa.Athlete)
            .WithMany()
            .HasForeignKey(aa => aa.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(aa => aa.Academy)
            .WithMany()
            .HasForeignKey(aa => aa.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(aa => aa.CreatedBy);
        builder.Ignore(aa => aa.UpdatedBy);
    }
}
