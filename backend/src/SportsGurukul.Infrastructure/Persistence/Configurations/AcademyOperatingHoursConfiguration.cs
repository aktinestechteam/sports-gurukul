using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyOperatingHoursConfiguration : IEntityTypeConfiguration<AcademyOperatingHours>
{
    public void Configure(EntityTypeBuilder<AcademyOperatingHours> builder)
    {
        builder.ToTable("AcademyOperatingHours");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.HolidaySchedule)
            .HasMaxLength(2000);

        builder.HasIndex(o => o.AcademyId)
            .IsUnique()
            .HasDatabaseName("IX_AcademyOperatingHours_AcademyId");

        builder.HasOne(o => o.Academy)
            .WithOne(a => a.OperatingHours)
            .HasForeignKey<AcademyOperatingHours>(o => o.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(o => o.CreatedBy);
        builder.Ignore(o => o.UpdatedBy);
    }
}
