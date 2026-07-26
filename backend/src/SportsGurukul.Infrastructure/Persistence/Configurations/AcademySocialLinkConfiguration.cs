using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademySocialLinkConfiguration : IEntityTypeConfiguration<AcademySocialLink>
{
    public void Configure(EntityTypeBuilder<AcademySocialLink> builder)
    {
        builder.ToTable("AcademySocialLinks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Platform)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(s => s.AcademyId)
            .HasDatabaseName("IX_AcademySocialLinks_AcademyId");

        builder.HasIndex(s => new { s.AcademyId, s.Platform })
            .IsUnique()
            .HasDatabaseName("IX_AcademySocialLinks_AcademyId_Platform");

        builder.HasOne(s => s.Academy)
            .WithMany()
            .HasForeignKey(s => s.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
