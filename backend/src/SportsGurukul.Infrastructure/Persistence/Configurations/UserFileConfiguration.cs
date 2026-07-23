using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class UserFileConfiguration : IEntityTypeConfiguration<UserFile>
{
    public void Configure(EntityTypeBuilder<UserFile> builder)
    {
        builder.ToTable("UserFiles");

        builder.HasKey(uf => uf.Id);

        builder.Property(uf => uf.OriginalFileName)
            .HasMaxLength(255);

        builder.Property(uf => uf.StoredFileName)
            .HasMaxLength(255);

        builder.Property(uf => uf.ContentType)
            .HasMaxLength(100);

        builder.Property(uf => uf.StoragePath)
            .HasMaxLength(1000);

        builder.Property(uf => uf.PublicUrl)
            .HasMaxLength(2000);

        builder.Property(uf => uf.FileType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(uf => uf.FileCategory)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(uf => uf.UserId)
            .HasDatabaseName("IX_UserFiles_UserId");

        builder.HasIndex(uf => new { uf.UserId, uf.FileType })
            .HasDatabaseName("IX_UserFiles_UserId_FileType");

        builder.HasIndex(uf => uf.IsDeleted)
            .HasDatabaseName("IX_UserFiles_IsDeleted");

        builder.HasOne(uf => uf.User)
            .WithMany()
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(uf => uf.CreatedBy);
        builder.Ignore(uf => uf.UpdatedBy);
    }
}
