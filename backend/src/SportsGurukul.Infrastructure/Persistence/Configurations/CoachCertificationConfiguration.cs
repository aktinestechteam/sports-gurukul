using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachCertificationConfiguration : IEntityTypeConfiguration<CoachCertification>
{
    public void Configure(EntityTypeBuilder<CoachCertification> builder)
    {
        builder.ToTable("CoachCertifications");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificationName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.IssuingAuthority)
            .HasMaxLength(200);

        builder.Property(c => c.CertificateNumber)
            .HasMaxLength(100);

        builder.Property(c => c.CertificateUrl)
            .HasMaxLength(500);

        builder.Property(c => c.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasIndex(c => c.CoachId)
            .HasDatabaseName("IX_CoachCertifications_CoachId");

        builder.HasIndex(c => c.VerificationStatus)
            .HasDatabaseName("IX_CoachCertifications_VerificationStatus");

        builder.HasIndex(c => new { c.CoachId, c.CertificationName })
            .HasDatabaseName("IX_CoachCertifications_CoachId_Name");

        builder.HasOne(c => c.Coach)
            .WithMany(co => co.Certifications)
            .HasForeignKey(c => c.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);

        builder.HasData(
            new CoachCertification
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000001"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                CertificationName = "BCCI Level A Coaching",
                IssuingAuthority = "Board of Control for Cricket in India",
                CertificateNumber = "BCCI-LA-2024-001",
                IssueDate = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2027, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                VerificationStatus = VerificationStatus.Verified,
                IsDeleted = false
            }
        );
    }
}
