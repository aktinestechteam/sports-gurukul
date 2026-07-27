using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Controllers.V1;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class BookingApiIntegrationTests : BookingIntegrationTestBase
{
    public BookingApiIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    #region Create Booking

    [Fact]
    public async Task CreateBooking_Admin_CreatesSuccessfully()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Morning Training Session",
            Description = "Reserved court for morning practice",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            CoachId = TestCoachEntityId,
            AthleteId = TestAthleteEntityId,
            BookingDate = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.BookingNumber.Should().StartWith("BK-");
        content.Data.Title.Should().Be("Morning Training Session");
        content.Data.Status.Should().Be("Pending");
        content.Data.AcademyId.Should().Be(TestAcademyId);
        content.Data.FacilityId.Should().Be(TestFacilityId);
        content.Data.Duration.Should().Be(90);
    }

    [Fact]
    public async Task CreateBooking_WithInvalidBookingType_ReturnsBadRequest()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = 999,
            Title = "Invalid Type Booking",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            BookingDate = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_StartAfterEnd_ReturnsBadRequest()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Bad Time Booking",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            BookingDate = date,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_PastDate_ReturnsBadRequest()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Past Date Booking",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            BookingDate = DateTime.UtcNow.Date.AddDays(-5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_AthleteRole_ReturnsForbidden()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await AthleteClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Athlete Booking Attempt",
            AcademyId = TestAcademyId,
            BookingDate = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBooking_Unauthenticated_ReturnsUnauthorized()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var response = await UnauthenticatedClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Unauth Booking",
            AcademyId = TestAcademyId,
            BookingDate = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get Booking

    [Fact]
    public async Task GetBookingById_Exists_ReturnsBooking()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.GetAsync($"/api/v1/bookings/{booking!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Id.Should().Be(booking.Id);
        content.Data.BookingNumber.Should().Be(booking.BookingNumber);
    }

    [Fact]
    public async Task GetBookingById_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update Booking

    [Fact]
    public async Task UpdateBooking_PendingBooking_UpdatesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/bookings/{booking!.Id}", new
        {
            Title = "Updated Booking Title",
            Description = "Updated description"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Title.Should().Be("Updated Booking Title");
        content.Data.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateBooking_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.PutAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}", new
        {
            Title = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBooking_ConfirmedBooking_ReturnsBadRequest()
    {
        var bookingId = await CreateBookingDirectlyInDbAsync(status: "Confirmed");
        var response = await AdminClient.PutAsJsonAsync($"/api/v1/bookings/{bookingId}", new
        {
            Title = "Should Fail"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Cancel Booking

    [Fact]
    public async Task CancelBooking_PendingBooking_CancelsSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/cancel", new
        {
            Reason = "Schedule conflict",
            Notes = "Will reschedule"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Status.Should().Be("Cancelled");

        var dbBooking = await GetBookingFromDbAsync(booking.Id);
        dbBooking.Should().NotBeNull();
        dbBooking!.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public async Task CancelBooking_AlreadyCancelled_ReturnsBadRequest()
    {
        var bookingId = await CreateBookingDirectlyInDbAsync(status: "Cancelled");

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/cancel", new
        {
            Reason = "Double cancel"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelBooking_CompletedBooking_ReturnsBadRequest()
    {
        var bookingId = await CreateBookingDirectlyInDbAsync(status: "Completed");

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/cancel", new
        {
            Reason = "Cancel completed"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelBooking_NotExists_ReturnsNotFound()
    {
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/cancel", new
        {
            Reason = "Ghost cancel"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Confirm Booking

    [Fact]
    public async Task ConfirmBooking_PendingBooking_ConfirmsSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsync($"/api/v1/bookings/{booking!.Id}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Status.Should().Be("Confirmed");
        content.Data.ApprovalStatus.Should().Be("Approved");
    }

    [Fact]
    public async Task ConfirmBooking_NotPending_ReturnsBadRequest()
    {
        var bookingId = await CreateBookingDirectlyInDbAsync(status: "Confirmed");

        var response = await AdminClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmBooking_AthleteRole_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AthleteClient.PostAsync($"/api/v1/bookings/{booking!.Id}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Complete Booking

    [Fact]
    public async Task CompleteBooking_ConfirmedBooking_CompletesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        await AdminClient.PostAsync($"/api/v1/bookings/{booking!.Id}/confirm", null);
        var response = await AdminClient.PostAsync($"/api/v1/bookings/{booking.Id}/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task CompleteBooking_NotConfirmed_ReturnsBadRequest()
    {
        var bookingId = await CreateBookingDirectlyInDbAsync(status: "Pending");

        var response = await AdminClient.PostAsync($"/api/v1/bookings/{bookingId}/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Expire Booking

    [Fact]
    public async Task ExpireBooking_PendingBooking_ExpiresSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsync($"/api/v1/bookings/{booking!.Id}/expire", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Status.Should().Be("Expired");
    }

    [Fact]
    public async Task ExpireBooking_NonSystemAdmin_ReturnsForbidden()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await CoachClient.PostAsync($"/api/v1/bookings/{booking!.Id}/expire", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Reject Booking

    [Fact]
    public async Task RejectBooking_PendingBooking_RejectsSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/reject", new
        {
            Reason = "Facility under maintenance"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Status.Should().Be("Rejected");
    }

    [Fact]
    public async Task RejectBooking_NotPending_ReturnsBadRequest()
    {
        var bookingId = await CreateBookingDirectlyInDbAsync(status: "Confirmed");

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reject", new
        {
            Reason = "Cannot reject confirmed"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Reschedule Booking

    [Fact]
    public async Task RescheduleBooking_PendingBooking_ReschedulesSuccessfully()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var newDate = DateTime.UtcNow.Date.AddDays(5);
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/reschedule", new
        {
            NewDate = newDate,
            NewStartTime = new TimeSpan(14, 0, 0),
            NewEndTime = new TimeSpan(15, 30, 0),
            Reason = "Coach requested change"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.BookingDate.Date.Should().Be(newDate.Date);
        content.Data.StartTime.Should().Be(new TimeSpan(14, 0, 0));
        content.Data.EndTime.Should().Be(new TimeSpan(15, 30, 0));
    }

    [Fact]
    public async Task RescheduleBooking_NewStartAfterEnd_ReturnsBadRequest()
    {
        var booking = await CreateBookingViaApiAsync(AdminClient);
        booking.Should().NotBeNull();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/bookings/{booking!.Id}/reschedule", new
        {
            NewDate = DateTime.UtcNow.Date.AddDays(5),
            NewStartTime = new TimeSpan(15, 0, 0),
            NewEndTime = new TimeSpan(14, 0, 0),
            Reason = "Bad times"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Search & List

    [Fact]
    public async Task SearchBookings_Empty_ReturnsEmptyList()
    {
        var response = await AdminClient.GetAsync("/api/v1/bookings?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchBookings_WithFilter_ReturnsFilteredResults()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Filter Test Alpha");
        await CreateBookingViaApiAsync(AdminClient, title: "Filter Test Beta");

        var response = await AdminClient.GetAsync("/api/v1/bookings?searchTerm=Filter+Test&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task SearchBookings_WithStatusFilter_ReturnsMatchingStatus()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Pending One");
        await CreateBookingDirectlyInDbAsync(status: "Confirmed", title: "Confirmed One");

        var response = await AdminClient.GetAsync("/api/v1/bookings?status=Pending&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().OnlyContain(b => b.Status == "Pending");
    }

    [Fact]
    public async Task GetUpcomingBookings_ReturnsUpcomingBookings()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Upcoming Test");

        var response = await AdminClient.GetAsync($"/api/v1/bookings/upcoming?academyId={TestAcademyId}&daysAhead=7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingSummaryDto>>>();
        content!.Success.Should().BeTrue();
    }

    #endregion

    #region Performance

    [Fact]
    public async Task CreateBooking_CompletesWithinTimeLimit()
    {
        var sw = Stopwatch.StartNew();
        var booking = await CreateBookingViaApiAsync(AdminClient, title: "Performance Test Booking");
        sw.Stop();

        booking.Should().NotBeNull();
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "creating a booking should complete within 5 seconds");
    }

    #endregion
}
