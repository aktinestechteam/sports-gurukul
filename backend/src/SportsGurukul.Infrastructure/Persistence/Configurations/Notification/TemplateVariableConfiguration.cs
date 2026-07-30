using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class TemplateVariableConfiguration : IEntityTypeConfiguration<TemplateVariable>
{
    public void Configure(EntityTypeBuilder<TemplateVariable> builder)
    {
        builder.ToTable("TemplateVariables");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Description)
            .HasMaxLength(500);

        builder.Property(v => v.DefaultValue)
            .HasMaxLength(500);

        builder.Property(v => v.DataType)
            .HasMaxLength(50)
            .HasDefaultValue("string");

        builder.HasIndex(v => new { v.TemplateId, v.Name })
            .IsUnique()
            .HasDatabaseName("IX_TemplateVariables_TemplateId_Name");

        builder.HasOne(v => v.Template)
            .WithMany(t => t.Variables)
            .HasForeignKey(v => v.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
