using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyConfiguration : IEntityTypeConfiguration<Academy>
{
    public void Configure(EntityTypeBuilder<Academy> builder)
    {
        builder.ToTable("Academies");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AcademyCode)
            .HasMaxLength(50);

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.LegalName)
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(2000);

        builder.Property(a => a.RegistrationNumber)
            .HasMaxLength(100);

        builder.Property(a => a.GSTNumber)
            .HasMaxLength(50);

        builder.Property(a => a.Website)
            .HasMaxLength(500);

        builder.Property(a => a.EstablishedDate)
            .HasColumnType("date")
            .HasConversion(
                v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null,
                v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null);

        builder.Property(a => a.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Phone)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.LogoUrl)
            .HasMaxLength(500);

        builder.Property(a => a.BannerUrl)
            .HasMaxLength(500);

        builder.Property(a => a.OwnedByUserId);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.AcademyType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(a => a.AcademyCode)
            .IsUnique()
            .HasDatabaseName("IX_Academies_AcademyCode");

        builder.HasIndex(a => a.Name)
            .HasDatabaseName("IX_Academies_Name");

        builder.HasIndex(a => a.Email)
            .IsUnique()
            .HasDatabaseName("IX_Academies_Email");

        builder.HasIndex(a => a.Phone)
            .HasDatabaseName("IX_Academies_Phone");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Academies_Status");

        builder.HasIndex(a => a.VerificationStatus)
            .HasDatabaseName("IX_Academies_VerificationStatus");

        builder.HasIndex(a => a.RegistrationNumber)
            .IsUnique()
            .HasDatabaseName("IX_Academies_RegistrationNumber")
            .HasFilter("\"RegistrationNumber\" IS NOT NULL");

        builder.HasIndex(a => new { a.Status, a.CreatedAt })
            .HasDatabaseName("IX_Academies_Status_CreatedAt");

        builder.HasIndex(a => a.OwnedByUserId)
            .HasDatabaseName("IX_Academies_OwnedByUserId");

        builder.HasOne(a => a.Contact)
            .WithOne(c => c.Academy)
            .HasForeignKey<AcademyContact>(c => c.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.OperatingHours)
            .WithOne(o => o.Academy)
            .HasForeignKey<AcademyOperatingHours>(o => o.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Verification)
            .WithOne(v => v.Academy)
            .HasForeignKey<AcademyVerification>(v => v.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);

        builder.HasData(
            new Academy
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                AcademyCode = "ACAD-SEED-001",
                Name = "Seed Academy",
                LegalName = "Seed Academy Pvt Ltd",
                Description = "Seed academy for development and testing.",
                Email = "academy.seed@sportsgurukul.com",
                Phone = "+910000000000",
                Status = AcademyStatus.Active,
                VerificationStatus = VerificationStatus.Verified,
                IsDeleted = false
            }
        );
    }
}
