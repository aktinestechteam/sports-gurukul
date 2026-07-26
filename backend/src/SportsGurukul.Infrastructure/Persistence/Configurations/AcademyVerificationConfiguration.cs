using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyVerificationConfiguration : IEntityTypeConfiguration<AcademyVerification>
{
    public void Configure(EntityTypeBuilder<AcademyVerification> builder)
    {
        builder.ToTable("AcademyVerifications");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(v => v.Remarks)
            .HasMaxLength(1000);

        builder.Property(v => v.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(v => v.AcademyId)
            .IsUnique()
            .HasDatabaseName("IX_AcademyVerifications_AcademyId");

        builder.HasIndex(v => v.VerificationStatus)
            .HasDatabaseName("IX_AcademyVerifications_Status");

        builder.HasOne(v => v.Academy)
            .WithOne(a => a.Verification)
            .HasForeignKey<AcademyVerification>(v => v.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(v => v.CreatedBy);
        builder.Ignore(v => v.UpdatedBy);
    }
}
