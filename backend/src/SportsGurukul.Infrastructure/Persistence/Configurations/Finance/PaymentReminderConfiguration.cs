using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class PaymentReminderConfiguration : IEntityTypeConfiguration<PaymentReminder>
{
    public void Configure(EntityTypeBuilder<PaymentReminder> builder)
    {
        builder.ToTable("PaymentReminders");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.ReminderType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(pr => pr.SentTo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pr => pr.Status)
            .HasMaxLength(50);

        builder.Property(pr => pr.Notes)
            .HasMaxLength(500);

        builder.HasIndex(pr => pr.InvoiceId)
            .HasDatabaseName("IX_PaymentReminders_InvoiceId");

        builder.HasOne(pr => pr.Invoice)
            .WithMany(i => i.Reminders)
            .HasForeignKey(pr => pr.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(pr => !pr.IsDeleted);

        builder.Ignore(pr => pr.CreatedBy);
        builder.Ignore(pr => pr.UpdatedBy);
    }
}
