using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class FeeStructureConfiguration : IEntityTypeConfiguration<FeeStructure>
{
    public void Configure(EntityTypeBuilder<FeeStructure> builder)
    {
        builder.ToTable("FeeStructures");

        builder.HasKey(fs => fs.Id);

        builder.Property(fs => fs.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(fs => fs.Description)
            .HasMaxLength(1000);

        builder.Property(fs => fs.Amount)
            .HasPrecision(18, 2);

        builder.Property(fs => fs.Frequency)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(fs => fs.SportId)
            .HasDatabaseName("IX_FeeStructures_SportId");

        builder.HasIndex(fs => fs.AcademyId)
            .HasDatabaseName("IX_FeeStructures_AcademyId");

        builder.HasIndex(fs => fs.FeeCategoryId)
            .HasDatabaseName("IX_FeeStructures_FeeCategoryId");

        builder.HasOne(fs => fs.Sport)
            .WithMany()
            .HasForeignKey(fs => fs.SportId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(fs => fs.Academy)
            .WithMany()
            .HasForeignKey(fs => fs.AcademyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(fs => fs.FeeCategory)
            .WithMany(fc => fc.FeeStructures)
            .HasForeignKey(fs => fs.FeeCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(fs => !fs.IsDeleted);

        builder.Ignore(fs => fs.CreatedBy);
        builder.Ignore(fs => fs.UpdatedBy);
    }
}
