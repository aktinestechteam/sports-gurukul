using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.Currency)
            .HasMaxLength(10);

        builder.Property(i => i.SubTotal)
            .HasPrecision(18, 2);

        builder.Property(i => i.TaxTotal)
            .HasPrecision(18, 2);

        builder.Property(i => i.DiscountTotal)
            .HasPrecision(18, 2);

        builder.Property(i => i.Total)
            .HasPrecision(18, 2);

        builder.Property(i => i.AmountPaid)
            .HasPrecision(18, 2);

        builder.Property(i => i.AmountDue)
            .HasPrecision(18, 2);

        builder.Property(i => i.Notes)
            .HasMaxLength(2000);

        builder.Property(i => i.TermsAndConditions)
            .HasMaxLength(4000);

        builder.Property(i => i.BillingAddress)
            .HasMaxLength(1000);

        builder.Property(i => i.ShippingAddress)
            .HasMaxLength(1000);

        builder.Property(i => i.PurchaseOrderNumber)
            .HasMaxLength(100);

        builder.Property(i => i.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_Invoices_InvoiceNumber");

        builder.HasIndex(i => i.Status)
            .HasDatabaseName("IX_Invoices_Status");

        builder.HasIndex(i => i.IssueDate)
            .HasDatabaseName("IX_Invoices_IssueDate");

        builder.HasIndex(i => i.DueDate)
            .HasDatabaseName("IX_Invoices_DueDate");

        builder.HasIndex(i => i.AthleteId)
            .HasDatabaseName("IX_Invoices_AthleteId");

        builder.HasIndex(i => i.AcademyId)
            .HasDatabaseName("IX_Invoices_AcademyId");

        builder.HasIndex(i => i.EventId)
            .HasDatabaseName("IX_Invoices_EventId");

        builder.HasIndex(i => i.TournamentId)
            .HasDatabaseName("IX_Invoices_TournamentId");

        builder.HasIndex(i => i.TrainingProgramId)
            .HasDatabaseName("IX_Invoices_TrainingProgramId");

        builder.HasIndex(i => i.MembershipId)
            .HasDatabaseName("IX_Invoices_MembershipId");

        builder.HasIndex(i => new { i.Status, i.DueDate })
            .HasDatabaseName("IX_Invoices_Status_DueDate");

        builder.HasOne(i => i.Athlete)
            .WithMany()
            .HasForeignKey(i => i.AthleteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Academy)
            .WithMany()
            .HasForeignKey(i => i.AcademyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Event)
            .WithMany()
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Tournament)
            .WithMany()
            .HasForeignKey(i => i.TournamentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.TrainingProgram)
            .WithMany()
            .HasForeignKey(i => i.TrainingProgramId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Membership)
            .WithMany()
            .HasForeignKey(i => i.MembershipId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.Ignore(i => i.CreatedBy);
        builder.Ignore(i => i.UpdatedBy);
    }
}
