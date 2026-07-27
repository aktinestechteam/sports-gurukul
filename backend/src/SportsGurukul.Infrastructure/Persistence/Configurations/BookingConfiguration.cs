using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.BookingType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(b => b.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.Property(b => b.ApprovalStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(b => b.Duration)
            .HasDefaultValue(0);

        builder.Property(b => b.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(b => b.BookingNumber)
            .IsUnique()
            .HasDatabaseName("IX_Bookings_BookingNumber");

        builder.HasIndex(b => b.AcademyId)
            .HasDatabaseName("IX_Bookings_AcademyId");

        builder.HasIndex(b => b.BranchId)
            .HasDatabaseName("IX_Bookings_BranchId");

        builder.HasIndex(b => b.FacilityId)
            .HasDatabaseName("IX_Bookings_FacilityId");

        builder.HasIndex(b => b.CoachId)
            .HasDatabaseName("IX_Bookings_CoachId");

        builder.HasIndex(b => b.AthleteId)
            .HasDatabaseName("IX_Bookings_AthleteId");

        builder.HasIndex(b => b.TrainingSessionId)
            .HasDatabaseName("IX_Bookings_TrainingSessionId");

        builder.HasIndex(b => b.BookingDate)
            .HasDatabaseName("IX_Bookings_BookingDate");

        builder.HasIndex(b => b.Status)
            .HasDatabaseName("IX_Bookings_Status");

        builder.HasIndex(b => b.BookingType)
            .HasDatabaseName("IX_Bookings_BookingType");

        builder.HasIndex(b => b.ApprovalStatus)
            .HasDatabaseName("IX_Bookings_ApprovalStatus");

        builder.HasIndex(b => new { b.AcademyId, b.BookingDate })
            .HasDatabaseName("IX_Bookings_AcademyId_BookingDate");

        builder.HasIndex(b => new { b.FacilityId, b.BookingDate })
            .HasDatabaseName("IX_Bookings_FacilityId_BookingDate");

        builder.HasIndex(b => new { b.CoachId, b.BookingDate })
            .HasDatabaseName("IX_Bookings_CoachId_BookingDate");

        builder.HasIndex(b => new { b.AthleteId, b.BookingDate })
            .HasDatabaseName("IX_Bookings_AthleteId_BookingDate");

        builder.HasIndex(b => new { b.Status, b.BookingDate })
            .HasDatabaseName("IX_Bookings_Status_BookingDate");

        builder.HasIndex(b => new { b.BookingType, b.Status })
            .HasDatabaseName("IX_Bookings_BookingType_Status");

        builder.HasOne(b => b.Academy)
            .WithMany()
            .HasForeignKey(b => b.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Branch)
            .WithMany()
            .HasForeignKey(b => b.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Facility)
            .WithMany()
            .HasForeignKey(b => b.FacilityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Coach)
            .WithMany()
            .HasForeignKey(b => b.CoachId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Athlete)
            .WithMany()
            .HasForeignKey(b => b.AthleteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.TrainingSession)
            .WithMany()
            .HasForeignKey(b => b.TrainingSessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Ignore(b => b.CreatedBy);
        builder.Ignore(b => b.UpdatedBy);

        builder.HasData(
            new Booking
            {
                Id = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                BookingNumber = "BK-20260727-SEED01",
                BookingType = BookingType.TrainingSession,
                Status = BookingStatus.Confirmed,
                Title = "Seed Training Booking",
                Description = "Seed booking for development and testing.",
                AcademyId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                BookingDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                Duration = 60,
                ApprovalStatus = BookingApprovalStatus.Approved,
                IsDeleted = false
            }
        );
    }
}
