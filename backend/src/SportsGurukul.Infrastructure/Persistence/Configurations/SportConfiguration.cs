using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class SportConfiguration : IEntityTypeConfiguration<Sport>
{
    public void Configure(EntityTypeBuilder<Sport> builder)
    {
        builder.ToTable("Sports");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(100);

        builder.Property(s => s.Code)
            .HasMaxLength(20);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("IX_Sports_Name");

        builder.HasIndex(s => s.Code)
            .IsUnique()
            .HasDatabaseName("IX_Sports_Code");

        builder.HasIndex(s => s.SportCategoryId)
            .HasDatabaseName("IX_Sports_SportCategoryId");

        builder.HasOne(s => s.SportCategory)
            .WithMany(c => c.Sports)
            .HasForeignKey(s => s.SportCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);

        builder.HasData(
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000001"),
                Name = "Cricket",
                Code = "CRK",
                OlympicSport = false,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000002"),
                Name = "Football",
                Code = "FTB",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000003"),
                Name = "Badminton",
                Code = "BDM",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000002"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000004"),
                Name = "Tennis",
                Code = "TNS",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000002"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000005"),
                Name = "Table Tennis",
                Code = "TTP",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000002"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000006"),
                Name = "Athletics",
                Code = "ATH",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000003"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000007"),
                Name = "Chess",
                Code = "CHS",
                OlympicSport = false,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000003"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000008"),
                Name = "Swimming",
                Code = "SWM",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000005"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000009"),
                Name = "Basketball",
                Code = "BBL",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                IsDeleted = false
            },
            new Sport
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-00000000000a"),
                Name = "Volleyball",
                Code = "VLB",
                OlympicSport = true,
                SportCategoryId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                IsDeleted = false
            }
        );
    }
}
