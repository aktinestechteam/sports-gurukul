using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyBranchConfiguration : IEntityTypeConfiguration<AcademyBranch>
{
    public void Configure(EntityTypeBuilder<AcademyBranch> builder)
    {
        builder.ToTable("AcademyBranches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BranchName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Address)
            .HasMaxLength(500);

        builder.Property(b => b.Country)
            .HasMaxLength(100);

        builder.Property(b => b.State)
            .HasMaxLength(100);

        builder.Property(b => b.City)
            .HasMaxLength(100);

        builder.Property(b => b.District)
            .HasMaxLength(100);

        builder.Property(b => b.PostalCode)
            .HasMaxLength(20);

        builder.Property(b => b.Latitude)
            .HasPrecision(10, 8);

        builder.Property(b => b.Longitude)
            .HasPrecision(11, 8);

        builder.HasIndex(b => b.AcademyId)
            .HasDatabaseName("IX_AcademyBranches_AcademyId");

        builder.HasIndex(b => new { b.AcademyId, b.BranchName })
            .IsUnique()
            .HasDatabaseName("IX_AcademyBranches_AcademyId_BranchName");

        builder.HasIndex(b => new { b.State, b.City })
            .HasDatabaseName("IX_AcademyBranches_State_City");

        builder.HasIndex(b => b.Country)
            .HasDatabaseName("IX_AcademyBranches_Country");

        builder.HasOne(b => b.Academy)
            .WithMany(a => a.Branches)
            .HasForeignKey(b => b.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.CreatedBy);
        builder.Ignore(b => b.UpdatedBy);
    }
}
