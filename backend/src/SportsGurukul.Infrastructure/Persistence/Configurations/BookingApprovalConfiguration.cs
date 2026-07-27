using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingApprovalConfiguration : IEntityTypeConfiguration<BookingApproval>
{
    public void Configure(EntityTypeBuilder<BookingApproval> builder)
    {
        builder.ToTable("BookingApprovals");

        builder.HasKey(ba => ba.Id);

        builder.Property(ba => ba.ApprovalStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(ba => ba.Comments)
            .HasMaxLength(1000);

        builder.Property(ba => ba.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(ba => ba.BookingId)
            .HasDatabaseName("IX_BookingApprovals_BookingId");

        builder.HasIndex(ba => ba.ApprovalStatus)
            .HasDatabaseName("IX_BookingApprovals_ApprovalStatus");

        builder.HasIndex(ba => ba.ApproverUserId)
            .HasDatabaseName("IX_BookingApprovals_ApproverUserId");

        builder.HasIndex(ba => ba.EscalationLevel)
            .HasDatabaseName("IX_BookingApprovals_EscalationLevel");

        builder.HasIndex(ba => new { ba.BookingId, ba.ApprovalStatus })
            .HasDatabaseName("IX_BookingApprovals_BookingId_Status");

        builder.HasOne(ba => ba.Booking)
            .WithMany(b => b.Approvals)
            .HasForeignKey(ba => ba.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ba => !ba.IsDeleted);

        builder.Ignore(ba => ba.CreatedBy);
        builder.Ignore(ba => ba.UpdatedBy);

        builder.HasData(
            new BookingApproval
            {
                Id = Guid.Parse("b6000000-0000-0000-0000-000000000001"),
                BookingId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                ApprovalStatus = BookingApprovalStatus.Approved,
                EscalationLevel = 0,
                IsDeleted = false
            }
        );
    }
}
