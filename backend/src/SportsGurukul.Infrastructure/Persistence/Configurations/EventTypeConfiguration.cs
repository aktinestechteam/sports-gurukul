using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventTypeConfiguration : IEntityTypeConfiguration<EventTypeEntity>
{
    public void Configure(EntityTypeBuilder<EventTypeEntity> builder)
    {
        builder.ToTable("EventTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_EventTypes_Code");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_EventTypes_Name");

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000001"),
                Name = "Camp",
                Code = "CAMP",
                Description = "Sports camps for training and skill development",
                IsActive = true,
                DisplayOrder = 1,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000002"),
                Name = "Workshop",
                Code = "WORKSHOP",
                Description = "Interactive workshops for hands-on learning",
                IsActive = true,
                DisplayOrder = 2,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000003"),
                Name = "Seminar",
                Code = "SEMINAR",
                Description = "Educational seminars and lectures",
                IsActive = true,
                DisplayOrder = 3,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000004"),
                Name = "Coaching Clinic",
                Code = "COACHING_CLINIC",
                Description = "Professional coaching clinics for athletes and coaches",
                IsActive = true,
                DisplayOrder = 4,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000005"),
                Name = "Trial",
                Code = "TRIAL",
                Description = "Athlete trials and assessments",
                IsActive = true,
                DisplayOrder = 5,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000006"),
                Name = "Talent Hunt",
                Code = "TALENT_HUNT",
                Description = "Talent identification and scouting programs",
                IsActive = true,
                DisplayOrder = 6,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000007"),
                Name = "Competition",
                Code = "COMPETITION",
                Description = "Sports competitions and meets",
                IsActive = true,
                DisplayOrder = 7,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000008"),
                Name = "Community Event",
                Code = "COMMUNITY_EVENT",
                Description = "Community engagement and outreach events",
                IsActive = true,
                DisplayOrder = 8,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000009"),
                Name = "Sports Festival",
                Code = "SPORTS_FESTIVAL",
                Description = "Multi-sport festivals and celebrations",
                IsActive = true,
                DisplayOrder = 9,
                IsDeleted = false
            },
            new EventTypeEntity
            {
                Id = Guid.Parse("e1000000-0000-0000-0000-000000000010"),
                Name = "Webinar",
                Code = "WEBINAR",
                Description = "Online webinars and virtual sessions",
                IsActive = true,
                DisplayOrder = 10,
                IsDeleted = false
            }
        );
    }
}
