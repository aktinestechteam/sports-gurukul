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

public class BookingApprovalIntegrationTests : BookingIntegrationTestBase
{
    public BookingApprovalIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task ApproveBooking_PendingBooking_ApprovesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/approval/approve", new
        {
            Comments = "Approved for tournament"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.ApprovalStatus.Should().Be("Approved");
    }

    [Fact]
    public async Task ApproveBooking_BookingNotFound_ReturnsNotFound()
    {
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/approval/approve", new
        {
            Comments = "Approving ghost booking"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RejectBookingApproval_PendingBooking_RejectsSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/approval/reject", new
        {
            Comments = "Insufficient documentation"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveBooking_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/approval/approve", new
        {
            Comments = "Athlete trying to approve"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectBookingApproval_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/approval/reject", new
        {
            Comments = "Athlete trying to reject"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
