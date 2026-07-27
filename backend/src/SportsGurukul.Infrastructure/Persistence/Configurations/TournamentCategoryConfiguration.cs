using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentCategoryConfiguration : IEntityTypeConfiguration<TournamentCategory>
{
    public void Configure(EntityTypeBuilder<TournamentCategory> builder)
    {
        builder.ToTable("TournamentCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CategoryName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CategoryType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasIndex(c => c.TournamentId)
            .HasDatabaseName("IX_TournamentCategories_TournamentId");

        builder.HasIndex(c => c.CategoryType)
            .HasDatabaseName("IX_TournamentCategories_CategoryType");

        builder.HasOne(c => c.Tournament)
            .WithMany(t => t.Categories)
            .HasForeignKey(c => c.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
