using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventAgendaConfiguration : IEntityTypeConfiguration<EventAgenda>
{
    public void Configure(EntityTypeBuilder<EventAgenda> builder)
    {
        builder.ToTable("EventAgendas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.SpeakerName)
            .HasMaxLength(200);

        builder.Property(e => e.Location)
            .HasMaxLength(200);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventAgendas_EventId");

        builder.HasIndex(e => e.SessionId)
            .HasDatabaseName("IX_EventAgendas_SessionId");

        builder.HasIndex(e => new { e.EventId, e.DisplayOrder })
            .HasDatabaseName("IX_EventAgendas_EventId_DisplayOrder");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Agendas)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Session)
            .WithMany()
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
