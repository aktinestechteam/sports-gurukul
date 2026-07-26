using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyViewConfiguration : IEntityTypeConfiguration<AcademyView>
{
    public void Configure(EntityTypeBuilder<AcademyView> builder)
    {
        builder.ToTable("AcademyViews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedBy);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.AcademyId)
            .HasDatabaseName("IX_AcademyViews_AcademyId");

        builder.HasIndex(x => x.ViewedByUserId)
            .HasDatabaseName("IX_AcademyViews_ViewedByUserId");

        builder.HasIndex(x => new { x.AcademyId, x.ViewedAt })
            .HasDatabaseName("IX_AcademyViews_AcademyId_ViewedAt");

        builder.HasOne(x => x.Academy)
            .WithMany()
            .HasForeignKey(x => x.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
