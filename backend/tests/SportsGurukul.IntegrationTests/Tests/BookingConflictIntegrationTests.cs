using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class BookingConflictIntegrationTests : BookingIntegrationTestBase
{
    public BookingConflictIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task GetBookingConflicts_NoConflicts_ReturnsEmptyList()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.GetAsync($"/api/v1/bookings/conflicts?bookingId={booking!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingConflictDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBookingConflicts_WithConflicts_ReturnsConflicts()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var conflict = new BookingConflict
        {
            Id = Guid.NewGuid(),
            BookingId = booking!.Id,
            ConflictingBookingId = Guid.NewGuid(),
            ConflictType = BookingConflictType.FacilityOverlap,
            Description = "Facility already booked",
            IsResolved = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.BookingConflicts.Add(conflict);
        await dbContext.SaveChangesAsync();

        var response = await AdminClient.GetAsync($"/api/v1/bookings/conflicts?bookingId={booking.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingConflictDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeEmpty();
        content.Data!.Should().Contain(c => c.Description == "Facility already booked");
    }

    [Fact]
    public async Task ResolveConflict_UnresolvedConflict_ResolvesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var conflict = new BookingConflict
        {
            Id = Guid.NewGuid(),
            BookingId = booking!.Id,
            ConflictingBookingId = Guid.NewGuid(),
            ConflictType = BookingConflictType.CoachOverlap,
            Description = "Coach double-booked",
            IsResolved = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.BookingConflicts.Add(conflict);
        await dbContext.SaveChangesAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/conflicts/{conflict.Id}/resolve", new
        {
            ResolutionNotes = "Coach reassigned to different slot"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().BeTrue();

        var dbConflict = await GetConflictFromDbAsync(conflict.Id);
        dbConflict.Should().NotBeNull();
        dbConflict!.IsResolved.Should().BeTrue();
        dbConflict.ResolutionNotes.Should().Be("Coach reassigned to different slot");
    }

    [Fact]
    public async Task ResolveConflict_AlreadyResolved_ReturnsBadRequest()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var conflict = new BookingConflict
        {
            Id = Guid.NewGuid(),
            BookingId = booking!.Id,
            ConflictingBookingId = Guid.NewGuid(),
            ConflictType = BookingConflictType.AthleteOverlap,
            Description = "Athlete double-booked",
            IsResolved = true,
            ResolvedOn = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.BookingConflicts.Add(conflict);
        await dbContext.SaveChangesAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/conflicts/{conflict.Id}/resolve", new
        {
            ResolutionNotes = "Trying to resolve again"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResolveConflict_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/conflicts/{Guid.NewGuid()}/resolve", new
        {
            ResolutionNotes = "Ghost conflict"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResolveConflict_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var conflict = new BookingConflict
        {
            Id = Guid.NewGuid(),
            BookingId = booking!.Id,
            ConflictingBookingId = Guid.NewGuid(),
            ConflictType = BookingConflictType.FacilityOverlap,
            Description = "Test conflict",
            IsResolved = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.BookingConflicts.Add(conflict);
        await dbContext.SaveChangesAsync();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/bookings/conflicts/{conflict.Id}/resolve", new
        {
            ResolutionNotes = "Athlete resolving"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetBookingConflicts_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.GetAsync($"/api/v1/bookings/conflicts?bookingId={booking!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
