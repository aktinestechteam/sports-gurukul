using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Finance;

public class PaymentGatewayConfiguration : IEntityTypeConfiguration<PaymentGateway>
{
    public void Configure(EntityTypeBuilder<PaymentGateway> builder)
    {
        builder.ToTable("PaymentGateways");

        builder.HasKey(pg => pg.Id);

        builder.Property(pg => pg.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pg => pg.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pg => pg.Configuration)
            .HasColumnType("jsonb");

        builder.HasIndex(pg => pg.Code)
            .IsUnique()
            .HasDatabaseName("IX_PaymentGateways_Code");

        builder.HasQueryFilter(pg => !pg.IsDeleted);

        builder.Ignore(pg => pg.CreatedBy);
        builder.Ignore(pg => pg.UpdatedBy);
    }
}
