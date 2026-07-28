using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.EventName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(5000);

        builder.Property(e => e.ShortDescription)
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.RegistrationType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.RegistrationFee)
            .HasPrecision(10, 2);

        builder.Property(e => e.BannerUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ContactEmail)
            .HasMaxLength(200);

        builder.Property(e => e.ContactPhone)
            .HasMaxLength(20);

        builder.Property(e => e.Website)
            .HasMaxLength(500);

        builder.Property(e => e.Tags)
            .HasMaxLength(1000);

        builder.Property(e => e.Requirements)
            .HasMaxLength(2000);

        builder.Property(e => e.WhatToBring)
            .HasMaxLength(2000);

        builder.Property(e => e.CancellationPolicy)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EventCode)
            .IsUnique()
            .HasDatabaseName("IX_Events_EventCode");

        builder.HasIndex(e => e.AcademyId)
            .HasDatabaseName("IX_Events_AcademyId");

        builder.HasIndex(e => e.SportId)
            .HasDatabaseName("IX_Events_SportId");

        builder.HasIndex(e => e.EventTypeId)
            .HasDatabaseName("IX_Events_EventTypeId");

        builder.HasIndex(e => e.EventCategoryId)
            .HasDatabaseName("IX_Events_EventCategoryId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Events_Status");

        builder.HasIndex(e => new { e.StartDate, e.EndDate })
            .HasDatabaseName("IX_Events_StartDate_EndDate");

        builder.HasIndex(e => new { e.AcademyId, e.Status })
            .HasDatabaseName("IX_Events_AcademyId_Status");

        builder.HasIndex(e => new { e.SportId, e.Status })
            .HasDatabaseName("IX_Events_SportId_Status");

        builder.HasOne(e => e.Academy)
            .WithMany()
            .HasForeignKey(e => e.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sport)
            .WithMany()
            .HasForeignKey(e => e.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EventType)
            .WithMany(et => et.Events)
            .HasForeignKey(e => e.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EventCategory)
            .WithMany(ec => ec.Events)
            .HasForeignKey(e => e.EventCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
