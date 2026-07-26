using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyMembershipConfiguration : IEntityTypeConfiguration<AcademyMembership>
{
    public void Configure(EntityTypeBuilder<AcademyMembership> builder)
    {
        builder.ToTable("AcademyMemberships");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MembershipName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.Benefits)
            .HasMaxLength(2000);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(m => m.AcademyId)
            .HasDatabaseName("IX_AcademyMemberships_AcademyId");

        builder.HasIndex(m => m.Status)
            .HasDatabaseName("IX_AcademyMemberships_Status");

        builder.HasIndex(m => new { m.AcademyId, m.MembershipName })
            .IsUnique()
            .HasDatabaseName("IX_AcademyMemberships_AcademyId_Name");

        builder.HasOne(m => m.Academy)
            .WithMany(a => a.Memberships)
            .HasForeignKey(m => m.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
