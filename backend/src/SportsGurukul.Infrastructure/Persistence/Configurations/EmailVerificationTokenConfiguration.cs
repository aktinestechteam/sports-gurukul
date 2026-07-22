using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.ToTable("EmailVerificationTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(t => t.Token)
            .IsUnique()
            .HasDatabaseName("IX_EmailVerificationTokens_Token");

        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_EmailVerificationTokens_UserId");

        builder.HasOne(t => t.User)
            .WithMany(u => u.EmailVerificationTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.UpdatedBy);
    }
}
