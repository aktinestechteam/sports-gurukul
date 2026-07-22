using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class ContactInformationConfiguration : IEntityTypeConfiguration<ContactInformation>
{
    public void Configure(EntityTypeBuilder<ContactInformation> builder)
    {
        builder.ToTable("ContactInformation");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.PrimaryPhoneCountryCode)
            .HasMaxLength(10);

        builder.Property(ci => ci.PrimaryPhoneNumber)
            .HasMaxLength(20);

        builder.Property(ci => ci.SecondaryPhoneCountryCode)
            .HasMaxLength(10);

        builder.Property(ci => ci.SecondaryPhoneNumber)
            .HasMaxLength(20);

        builder.Property(ci => ci.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(ci => ci.FacebookUrl)
            .HasMaxLength(500);

        builder.Property(ci => ci.TwitterUrl)
            .HasMaxLength(500);

        builder.Property(ci => ci.InstagramUrl)
            .HasMaxLength(500);

        builder.Property(ci => ci.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(ci => ci.YouTubeUrl)
            .HasMaxLength(500);

        builder.HasIndex(ci => ci.UserProfileId)
            .IsUnique()
            .HasDatabaseName("IX_ContactInformation_UserProfileId");

        builder.HasIndex(ci => ci.PrimaryPhoneNumber)
            .HasDatabaseName("IX_ContactInformation_PrimaryPhoneNumber");

        builder.HasIndex(ci => ci.SecondaryPhoneNumber)
            .HasDatabaseName("IX_ContactInformation_SecondaryPhoneNumber");

        builder.HasOne(ci => ci.UserProfile)
            .WithOne(up => up.ContactInformation)
            .HasForeignKey<ContactInformation>(ci => ci.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(ci => ci.CreatedBy);
        builder.Ignore(ci => ci.UpdatedBy);
    }
}
