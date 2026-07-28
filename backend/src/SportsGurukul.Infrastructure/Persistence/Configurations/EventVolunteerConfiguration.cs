using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventVolunteerConfiguration : IEntityTypeConfiguration<EventVolunteer>
{
    public void Configure(EntityTypeBuilder<EventVolunteer> builder)
    {
        builder.ToTable("EventVolunteers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.VolunteerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Assignment)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventId)
            .HasDatabaseName("IX_EventVolunteers_EventId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EventVolunteers_UserId");

        builder.HasIndex(e => e.Role)
            .HasDatabaseName("IX_EventVolunteers_Role");

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Volunteers)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
