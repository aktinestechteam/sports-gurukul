using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingCertificateConfiguration : IEntityTypeConfiguration<TrainingCertificate>
{
    public void Configure(EntityTypeBuilder<TrainingCertificate> builder)
    {
        builder.ToTable("TrainingCertificates");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificateType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.CertificateNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.FileUrl)
            .HasMaxLength(500);

        builder.Property(c => c.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(c => c.EnrollmentId)
            .HasDatabaseName("IX_TrainingCertificates_EnrollmentId");

        builder.HasIndex(c => c.CertificateNumber)
            .IsUnique()
            .HasDatabaseName("IX_TrainingCertificates_CertificateNumber");

        builder.HasIndex(c => c.CertificateType)
            .HasDatabaseName("IX_TrainingCertificates_CertificateType");

        builder.HasOne(c => c.Enrollment)
            .WithMany(e => e.Certificates)
            .HasForeignKey(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
