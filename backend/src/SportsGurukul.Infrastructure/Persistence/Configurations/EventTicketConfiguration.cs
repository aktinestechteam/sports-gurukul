using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventTicketConfiguration : IEntityTypeConfiguration<EventTicket>
{
    public void Configure(EntityTypeBuilder<EventTicket> builder)
    {
        builder.ToTable("EventTickets");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TicketCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.TicketType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Price)
            .HasPrecision(10, 2);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventTickets_EventId");

        builder.HasIndex(e => e.TicketCode)
            .IsUnique()
            .HasDatabaseName("IX_EventTickets_TicketCode");

        builder.HasIndex(e => new { e.EventId, e.TicketType })
            .HasDatabaseName("IX_EventTickets_EventId_TicketType");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Tickets)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
