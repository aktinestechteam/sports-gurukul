using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class BookingWaitlistIntegrationTests : BookingIntegrationTestBase
{
    public BookingWaitlistIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task JoinWaitlist_ValidBooking_AddsToWaitlist()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId,
            Notes = "Preferred morning slots"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.BookingId.Should().Be(booking.Id);
        content.Data.Status.Should().Be("Active");
        content.Data.Priority.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task JoinWaitlist_BookingNotFound_ReturnsNotFound()
    {
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task JoinWaitlist_DuplicateUser_ReturnsConflict()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId
        });

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task JoinWaitlist_MultipleUsers_PrioritiesIncrement()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response1 = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId,
            Notes = "First in line"
        });
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var response2 = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.CoachUserId,
            Notes = "Second in line"
        });
        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var content1 = await response1.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var content2 = await response2.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();

        content2!.Data!.Priority.Should().BeGreaterThan(content1!.Data!.Priority);
    }

    [Fact]
    public async Task RemoveFromWaitlist_ExistingEntry_RemovesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var joinResponse = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId
        });
        var joinContent = await joinResponse.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var waitlistEntryId = joinContent!.Data!.Id;

        var response = await AdminClient.DeleteAsync($"/api/v1/bookings/{booking.Id}/waitlist/{waitlistEntryId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var entry = await dbContext.BookingWaitlists.FirstOrDefaultAsync(w => w.Id == waitlistEntryId);
        entry.Should().NotBeNull();
        entry!.Status.Should().Be(WaitlistStatus.Cancelled);
    }

    [Fact]
    public async Task RemoveFromWaitlist_NotExists_ReturnsNotFound()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.DeleteAsync($"/api/v1/bookings/{booking!.Id}/waitlist/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PromoteWaitlistedBooking_ActiveEntry_PromotesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var joinResponse = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId
        });
        var joinContent = await joinResponse.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var waitlistEntryId = joinContent!.Data!.Id;

        var response = await AdminClient.PostAsync($"/api/v1/bookings/{booking.Id}/waitlist/{waitlistEntryId}/promote", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task PromoteWaitlistedBooking_CoachRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var joinResponse = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/waitlist", new
        {
            WaitlistUserId = SeedData.AthleteUserId
        });
        var joinContent = await joinResponse.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var waitlistEntryId = joinContent!.Data!.Id;

        var response = await CoachClient.PostAsync($"/api/v1/bookings/{booking.Id}/waitlist/{waitlistEntryId}/promote", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
