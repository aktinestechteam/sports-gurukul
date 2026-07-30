using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pm => pm.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pm => pm.Description)
            .HasMaxLength(500);

        builder.HasIndex(pm => pm.Code)
            .IsUnique()
            .HasDatabaseName("IX_PaymentMethods_Code");

        builder.HasQueryFilter(pm => !pm.IsDeleted);

        builder.Ignore(pm => pm.CreatedBy);
        builder.Ignore(pm => pm.UpdatedBy);

        builder.HasData(
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000001"), Code = "CASH", Name = "Cash", Description = "Cash payment", IsActive = true, SortOrder = 1 },
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000002"), Code = "CARD", Name = "Credit/Debit Card", Description = "Payment via credit or debit card", IsActive = true, SortOrder = 2 },
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000003"), Code = "UPI", Name = "UPI", Description = "Payment via UPI", IsActive = true, SortOrder = 3 },
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000004"), Code = "NET_BANKING", Name = "Net Banking", Description = "Payment via net banking", IsActive = true, SortOrder = 4 },
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000005"), Code = "WALLET", Name = "Wallet", Description = "Payment via digital wallet", IsActive = true, SortOrder = 5 },
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000006"), Code = "CHEQUE", Name = "Cheque", Description = "Payment via cheque", IsActive = true, SortOrder = 6 },
            new PaymentMethod { Id = Guid.Parse("B2000000-0000-0000-0000-000000000007"), Code = "BANK_TRANSFER", Name = "Bank Transfer", Description = "Payment via bank transfer", IsActive = true, SortOrder = 7 }
        );
    }
}
