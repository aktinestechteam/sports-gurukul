using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class BookingSecurityIntegrationTests : BookingIntegrationTestBase
{
    public BookingSecurityIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateBooking_NoToken_ReturnsUnauthorized()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await UnauthenticatedClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "No Auth Booking",
            AcademyId = TestAcademyId,
            BookingDate = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBooking_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateBooking_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.PutAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}", new
        {
            Title = "No Auth Update"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CancelBooking_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/cancel", new
        {
            Reason = "No auth cancel"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBooking_AthleteRole_ReturnsForbidden()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await AthleteClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Athlete Create Attempt",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            BookingDate = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ConfirmBooking_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.PostAsync($"/api/v1/bookings/{booking!.Id}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ExpireBooking_CoachRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await CoachClient.PostAsync($"/api/v1/bookings/{booking!.Id}/expire", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RescheduleBooking_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/reschedule", new
        {
            NewDate = DateTime.UtcNow.Date.AddDays(5),
            NewStartTime = new TimeSpan(14, 0, 0),
            NewEndTime = new TimeSpan(15, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectBooking_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/reject", new
        {
            Reason = "Athlete reject attempt"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task JoinWaitlist_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ApproveBooking_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/approval/approve", new
        {
            Comments = "No auth approve"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBookingConflicts_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/bookings/conflicts?bookingId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchBookings_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/bookings/search?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CalendarView_NoToken_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.GetAsync($"/api/v1/bookings/calendar?academyId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBooking_InvalidToken_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", "invalid.jwt.token");

        var response = await client.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Invalid Token Booking",
            AcademyId = TestAcademyId,
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
