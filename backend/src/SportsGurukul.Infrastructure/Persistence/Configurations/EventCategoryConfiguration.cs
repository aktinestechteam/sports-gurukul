using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventCategoryConfiguration : IEntityTypeConfiguration<EventCategory>
{
    public void Configure(EntityTypeBuilder<EventCategory> builder)
    {
        builder.ToTable("EventCategories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.CategoryType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_EventCategories_Code");

        builder.HasIndex(e => e.CategoryType)
            .HasDatabaseName("IX_EventCategories_CategoryType");

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000001"),
                Name = "Sports Training",
                Code = "SPORTS_TRAINING",
                Description = "Events focused on sports skill development",
                CategoryType = EventCategoryType.SportsTraining,
                IsActive = true,
                DisplayOrder = 1,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000002"),
                Name = "Education",
                Code = "EDUCATION",
                Description = "Educational events including seminars and workshops",
                CategoryType = EventCategoryType.Education,
                IsActive = true,
                DisplayOrder = 2,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000003"),
                Name = "Networking",
                Code = "NETWORKING",
                Description = "Events for professional networking",
                CategoryType = EventCategoryType.Networking,
                IsActive = true,
                DisplayOrder = 3,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000004"),
                Name = "Health",
                Code = "HEALTH",
                Description = "Health and wellness focused events",
                CategoryType = EventCategoryType.Health,
                IsActive = true,
                DisplayOrder = 4,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000005"),
                Name = "Talent Development",
                Code = "TALENT_DEV",
                Description = "Talent identification and development programs",
                CategoryType = EventCategoryType.TalentDevelopment,
                IsActive = true,
                DisplayOrder = 5,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000006"),
                Name = "Community Outreach",
                Code = "COMMUNITY_OUTREACH",
                Description = "Community engagement and outreach events",
                CategoryType = EventCategoryType.CommunityOutreach,
                IsActive = true,
                DisplayOrder = 6,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000007"),
                Name = "Professional",
                Code = "PROFESSIONAL",
                Description = "Professional development events",
                CategoryType = EventCategoryType.Professional,
                IsActive = true,
                DisplayOrder = 7,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000008"),
                Name = "Recreational",
                Code = "RECREATIONAL",
                Description = "Fun and recreational events",
                CategoryType = EventCategoryType.Recreational,
                IsActive = true,
                DisplayOrder = 8,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000009"),
                Name = "Competitive",
                Code = "COMPETITIVE",
                Description = "Competitive events and tournaments",
                CategoryType = EventCategoryType.Competitive,
                IsActive = true,
                DisplayOrder = 9,
                IsDeleted = false
            },
            new EventCategory
            {
                Id = Guid.Parse("ec000000-0000-0000-0000-000000000010"),
                Name = "Mixed",
                Code = "MIXED",
                Description = "Multi-purpose events combining multiple categories",
                CategoryType = EventCategoryType.Mixed,
                IsActive = true,
                DisplayOrder = 10,
                IsDeleted = false
            }
        );
    }
}
