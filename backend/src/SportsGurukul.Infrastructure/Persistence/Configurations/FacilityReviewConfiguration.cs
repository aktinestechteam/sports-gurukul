using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityReviewConfiguration : IEntityTypeConfiguration<FacilityReview>
{
    public void Configure(EntityTypeBuilder<FacilityReview> builder)
    {
        builder.ToTable("FacilityReviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReviewText)
            .HasMaxLength(5000);

        builder.HasIndex(r => r.FacilityId)
            .HasDatabaseName("IX_FacilityReviews_FacilityId");

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_FacilityReviews_UserId");

        builder.HasIndex(r => new { r.FacilityId, r.UserId })
            .IsUnique()
            .HasDatabaseName("IX_FacilityReviews_FacilityId_UserId");

        builder.HasOne(r => r.Facility)
            .WithMany(f => f.Reviews)
            .HasForeignKey(r => r.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
