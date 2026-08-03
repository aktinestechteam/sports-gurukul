using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.RoleType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        builder.HasIndex(r => r.RoleType)
            .IsUnique()
            .HasDatabaseName("IX_Roles_RoleType");

        builder.Ignore(r => r.UserRoles);
        builder.Ignore(r => r.RolePermissions);

        builder.HasData(
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567801"),
                Name = "SuperAdmin",
                Description = "Full system access with all permissions",
                RoleType = RoleType.SuperAdmin,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567802"),
                Name = "Admin",
                Description = "Platform administrator with user and content management",
                RoleType = RoleType.Admin,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567803"),
                Name = "Academy",
                Description = "Sports academy managing coaches, athletes, and batches",
                RoleType = RoleType.Academy,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567804"),
                Name = "Coach",
                Description = "Sports coach managing training and athletes",
                RoleType = RoleType.Coach,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567805"),
                Name = "Athlete",
                Description = "Sports athlete managing profile and training",
                RoleType = RoleType.Athlete,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567806"),
                Name = "Parent",
                Description = "Parent or guardian of an athlete",
                RoleType = RoleType.Parent,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567807"),
                Name = "Scout",
                Description = "Talent scout discovering and evaluating athletes",
                RoleType = RoleType.Scout,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567808"),
                Name = "Sponsor",
                Description = "Sponsor providing funding and support",
                RoleType = RoleType.Sponsor,
                IsDeleted = false
            },
            new Role
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567809"),
                Name = "AI Administrator",
                Description = "Administrator managing AI assistants, prompts, knowledge bases, agents, workflows, and model usage",
                RoleType = RoleType.AIAdministrator,
                IsDeleted = false
            });
    }
}
