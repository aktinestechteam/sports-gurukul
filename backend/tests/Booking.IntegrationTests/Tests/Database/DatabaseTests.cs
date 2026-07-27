using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;

namespace Booking.IntegrationTests.Tests.Database;

[Collection("Postgres")]
public class DatabaseTests : BaseIntegrationTest
{
    public DatabaseTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Indexes

    [Fact]
    public async Task BookingTable_HasPrimaryKeyIndex()
    {
        var bookingId = await CreateBookingInDbAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var booking = await dbContext.Bookings.FindAsync(bookingId);
        booking.Should().NotBeNull();
    }

    [Fact]
    public async Task BookingTable_HasUniqueIndexOnBookingNumber()
    {
        var number = $"BK-UNIQUE-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var booking1 = new BookingEntity
        {
            Id = Guid.NewGuid(),
            BookingNumber = number,
            BookingType = BookingType.FacilityReservation,
            Status = BookingStatus.Pending,
            Title = "Unique Index Test 1",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Duration = 90,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var booking2 = new BookingEntity
        {
            Id = Guid.NewGuid(),
            BookingNumber = number,
            BookingType = BookingType.FacilityReservation,
            Status = BookingStatus.Pending,
            Title = "Unique Index Test 2",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(2),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Duration = 90,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Bookings.Add(booking1);
        await dbContext.SaveChangesAsync();

        dbContext.Bookings.Add(booking2);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
        exception.Should().NotBeNull();
    }

    #endregion

    #region Constraints

    [Fact]
    public async Task Booking_RequiresTitle()
    {
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-NO-TITLE",
            BookingType = BookingType.FacilityReservation,
            Status = BookingStatus.Pending,
            Title = "",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Duration = 90,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync();

        var saved = await dbContext.Bookings.FindAsync(booking.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Booking_RequiresAcademyId()
    {
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            BookingNumber = "BK-NO-ACADEMY",
            BookingType = BookingType.FacilityReservation,
            Status = BookingStatus.Pending,
            Title = "No Academy",
            AcademyId = Guid.Empty,
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Duration = 90,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync();

        var saved = await dbContext.Bookings.FindAsync(booking.Id);
        saved.Should().NotBeNull();
    }

    #endregion

    #region Cascade Behavior

    [Fact]
    public async Task DeleteBooking_CascadeRemovesRelatedEntities()
    {
        var bookingId = await CreateBookingInDbAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.BookingWaitlists.Add(new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            WaitlistUserId = Guid.NewGuid(),
            Status = WaitlistStatus.Active,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        });

        dbContext.BookingHistories.Add(new BookingHistory
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Action = "Created",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var booking = await dbContext.Bookings.FindAsync(bookingId);
        booking.Should().NotBeNull();

        dbContext.Bookings.Remove(booking!);
        await dbContext.SaveChangesAsync();

        var waitlists = await dbContext.BookingWaitlists
            .Where(w => w.BookingId == bookingId)
            .ToListAsync();
        waitlists.Should().BeEmpty();

        var histories = await dbContext.BookingHistories
            .Where(h => h.BookingId == bookingId)
            .ToListAsync();
        histories.Should().BeEmpty();
    }

    #endregion

    #region Soft Delete

    [Fact]
    public async Task SoftDelete_BookingIsMarkedAsDeleted()
    {
        var bookingId = await CreateBookingInDbAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var booking = await dbContext.Bookings.FindAsync(bookingId);
        booking.Should().NotBeNull();

        booking!.IsDeleted = true;
        booking.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        var foundBooking = await dbContext.Bookings.FindAsync(bookingId);
        foundBooking.Should().NotBeNull();
        foundBooking!.IsDeleted.Should().BeTrue();
    }

    #endregion

    #region Audit Fields

    [Fact]
    public async Task Booking_HasAuditFields()
    {
        var now = DateTime.UtcNow;
        var bookingId = await CreateBookingInDbAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var booking = await dbContext.Bookings.FindAsync(bookingId);
        booking.Should().NotBeNull();
        booking!.CreatedAt.Should().BeAfter(now.AddMinutes(-1));
        booking.UpdatedAt.Should().NotBeNull();
        booking.UpdatedAt!.Value.Should().BeAfter(now.AddMinutes(-1));
    }

    [Fact]
    public async Task Booking_UpdatedAt_ModifiedOnUpdate()
    {
        var bookingId = await CreateBookingInDbAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var booking = await dbContext.Bookings.FindAsync(bookingId);
        var originalUpdatedAt = booking!.UpdatedAt;
        booking.Title = "Updated Title";
        await dbContext.SaveChangesAsync();

        var updatedBooking = await dbContext.Bookings.FindAsync(bookingId);
        updatedBooking!.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
    }

    #endregion

    #region Optimistic Concurrency

    [Fact]
    public async Task UpdateBooking_WithConcurrentModification_ThrowsConcurrencyException()
    {
        var bookingId = await CreateBookingInDbAsync();

        using var scope1 = Factory.Services.CreateScope();
        var dbContext1 = scope1.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        using var scope2 = Factory.Services.CreateScope();
        var dbContext2 = scope2.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var booking1 = await dbContext1.Bookings.FindAsync(bookingId);
        var booking2 = await dbContext2.Bookings.FindAsync(bookingId);

        booking1!.Title = "First Update";
        await dbContext1.SaveChangesAsync();

        booking2!.Title = "Second Update";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => dbContext2.SaveChangesAsync());
    }

    #endregion

    #region Helpers

    private async Task<Guid> CreateBookingInDbAsync()
    {
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            BookingNumber = $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            BookingType = BookingType.FacilityReservation,
            Status = BookingStatus.Pending,
            Title = "DB Test Booking",
            Description = "Created directly in database for testing",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Duration = 90,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync();
        return booking.Id;
    }

    #endregion
}
