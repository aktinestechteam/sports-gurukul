using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class SessionScheduleConfiguration : IEntityTypeConfiguration<SessionSchedule>
{
    public void Configure(EntityTypeBuilder<SessionSchedule> builder)
    {
        builder.ToTable("SessionSchedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.HasIndex(s => s.SessionId)
            .HasDatabaseName("IX_SessionSchedules_SessionId");

        builder.HasIndex(s => new { s.SessionId, s.DayOfWeek })
            .HasDatabaseName("IX_SessionSchedules_SessionId_DayOfWeek");

        builder.HasOne(s => s.Session)
            .WithMany(s => s.Schedules)
            .HasForeignKey(s => s.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
