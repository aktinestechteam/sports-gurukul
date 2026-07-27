using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Approval;

[Collection("Postgres")]
public class ApprovalWorkflowTests : BaseIntegrationTest
{
    public ApprovalWorkflowTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Approve Booking

    [Fact]
    public async Task ApproveBooking_PendingBooking_ApprovesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Approvable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/approve", new
        {
            Comments = "Approved for upcoming session"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveBooking_AlreadyApproved_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Double Approve Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/approve", new
        {
            Comments = "First approval"
        });

        var secondApprovalResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/approve", new
        {
            Comments = "Second approval"
        });

        secondApprovalResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Reject Booking

    [Fact]
    public async Task RejectBooking_PendingBooking_RejectsSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Rejectable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/reject", new
        {
            Reason = "Facility unavailable"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RejectBooking_NotPending_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Non-Pending Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        var rejectResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/reject", new
        {
            Reason = "Cannot reject confirmed"
        });

        rejectResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Unauthorized Approval

    [Fact]
    public async Task ApproveBooking_AthleteRole_ReturnsForbidden()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Unauthorized Approve",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var athleteClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "athlete@test.com", "Athlete User", "Athlete");

        var response = await athleteClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/approve", new
        {
            Comments = "Athlete approving"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectBooking_CoachRole_ReturnsForbidden()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Unauthorized Reject",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var coachClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "coach@test.com", "Coach User", "Coach");

        var response = await coachClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/reject", new
        {
            Reason = "Coach rejecting"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Role-based Authorization

    [Fact]
    public async Task ApproveBooking_SystemAdmin_ApprovesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "System Admin Approve",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var adminClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "admin@test.com", "System Admin", "System Admin");

        var response = await adminClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/approve", new
        {
            Comments = "System admin approved"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApproveBooking_AcademyAdmin_ApprovesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Academy Admin Approve",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var academyAdminClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "academy@test.com", "Academy Admin", "Academy Admin");

        var response = await academyAdminClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/approval/approve", new
        {
            Comments = "Academy admin approved"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}