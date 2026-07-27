using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AttendanceStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.Remarks)
            .HasMaxLength(500);

        builder.HasIndex(a => a.SessionId)
            .HasDatabaseName("IX_Attendances_SessionId");

        builder.HasIndex(a => a.AthleteId)
            .HasDatabaseName("IX_Attendances_AthleteId");

        builder.HasIndex(a => a.AttendanceStatus)
            .HasDatabaseName("IX_Attendances_AttendanceStatus");

        builder.HasIndex(a => new { a.SessionId, a.AthleteId })
            .IsUnique()
            .HasDatabaseName("IX_Attendances_SessionId_AthleteId");

        builder.HasIndex(a => new { a.SessionId, a.AttendanceStatus })
            .HasDatabaseName("IX_Attendances_SessionId_Status");

        builder.HasOne(a => a.Session)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Athlete)
            .WithMany()
            .HasForeignKey(a => a.AthleteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
