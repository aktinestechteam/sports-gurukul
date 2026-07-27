using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Scheduling;

[Collection("Postgres")]
public class SchedulingTests : BaseIntegrationTest
{
    public SchedulingTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Recurring Booking Creation

    [Fact]
    public async Task CreateRecurringBooking_Admin_CreatesSuccessfully()
    {
        var request = new
        {
            BookingType = BookingType.TrainingSession,
            Title = "Weekly Group Coaching",
            Description = "Recurring weekly training",
            AcademyId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            RecurrenceType = RecurrenceType.Weekly,
            OccurrenceCount = 4
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings/recurring", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRecurringBooking_MissingRecurrenceType_ReturnsBadRequest()
    {
        var request = new
        {
            BookingType = BookingType.TrainingSession,
            Title = "Invalid Recurring",
            AcademyId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings/recurring", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRecurringBooking_Unauthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var request = new
        {
            BookingType = BookingType.TrainingSession,
            Title = "Unauth Recurring",
            AcademyId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            RecurrenceType = RecurrenceType.Weekly
        };

        var response = await client.PostAsJsonAsync("/api/v1/bookings/recurring", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Reschedule Booking

    [Fact]
    public async Task RescheduleBooking_ValidRequest_ReschedulesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Reschedulable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var newDate = DateTime.UtcNow.Date.AddDays(7);
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reschedule", new
        {
            NewDate = newDate,
            NewStartTime = new TimeSpan(14, 0, 0),
            NewEndTime = new TimeSpan(15, 30, 0),
            Reason = "Facility maintenance"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.BookingDate.Date.Should().Be(newDate.Date);
    }

    [Fact]
    public async Task RescheduleBooking_ConfirmedBooking_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Confirmed Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reschedule", new
        {
            NewDate = DateTime.UtcNow.Date.AddDays(7),
            NewStartTime = new TimeSpan(14, 0, 0),
            NewEndTime = new TimeSpan(15, 30, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Conflict Detection

    [Fact]
    public async Task ValidateBookingConflict_NoConflict_ReturnsNoConflicts()
    {
        var request = new
        {
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings/validate-conflict", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        content!.Success.Should().BeTrue();
    }

    #endregion

    #region Alternative Slot Generation

    [Fact]
    public async Task SearchBookings_WithSlotAvailability_ReturnsAvailableSlots()
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Available Slot Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var searchResponse = await HttpClient.GetAsync(
            $"/api/v1/bookings?academyId={request.AcademyId}&status=Pending&page=1&pageSize=10");

        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchContent = await searchResponse.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        searchContent!.Success.Should().BeTrue();
        searchContent.Data!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region Availability Validation

    [Fact]
    public async Task GetBookingById_ExistingBooking_ReturnsBooking()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Availability Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var getResponse = await HttpClient.GetAsync($"/api/v1/bookings/{bookingId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getContent = await getResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        getContent!.Success.Should().BeTrue();
        getContent.Data!.Id.Should().Be(bookingId);
    }

    [Fact]
    public async Task GetBookingById_NonExisting_ReturnsNotFound()
    {
        var response = await HttpClient.GetAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}