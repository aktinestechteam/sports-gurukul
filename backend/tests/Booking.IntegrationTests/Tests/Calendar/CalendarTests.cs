using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Calendar;

[Collection("Postgres")]
public class CalendarTests : BaseIntegrationTest
{
    public CalendarTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Coach Calendar

    [Fact]
    public async Task GetCoachBookings_ReturnsCoachCalendar()
    {
        var coachId = Guid.NewGuid();
        await CreateBookingAsync(coachId: coachId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/coach/{coachId}?date={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingSummaryDto>>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetCoachBookings_NonExistentCoach_ReturnsEmptyList()
    {
        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/coach/{Guid.NewGuid()}?date={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingSummaryDto>>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCoachBookings_Unauthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync(
            $"/api/v1/bookings/coach/{Guid.NewGuid()}?date={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Athlete Calendar

    [Fact]
    public async Task GetAthleteBookings_ReturnsAthleteCalendar()
    {
        var athleteId = Guid.NewGuid();
        await CreateBookingAsync(athleteId: athleteId);

        var response = await HttpClient.GetAsync($"/api/v1/bookings/athlete/{athleteId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingSummaryDto>>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetAthleteBookings_Unauthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync($"/api/v1/bookings/athlete/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Facility Calendar

    [Fact]
    public async Task GetFacilityBookings_ReturnsFacilityCalendar()
    {
        var facilityId = Guid.NewGuid();
        await CreateBookingAsync(facilityId: facilityId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/facility/{facilityId}?date={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingSummaryDto>>>();
        content!.Success.Should().BeTrue();
    }

    #endregion

    #region Agenda View

    [Fact]
    public async Task GetCalendarView_Agenda_ReturnsAgendaEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync(academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Agenda&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetCalendarView_Daily_ReturnsDailyEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync(academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Daily&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetCalendarView_Weekly_ReturnsWeeklyEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync(academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Weekly&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetCalendarView_Monthly_ReturnsMonthlyEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync(academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Monthly&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetCalendarView_InvalidViewType_ReturnsBadRequest()
    {
        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={Guid.NewGuid()}&viewType=InvalidView");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Recurring Events

    [Fact]
    public async Task GetCalendarView_WithRecurringBooking_ReturnsRecurringEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateRecurringBookingAsync(academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Monthly&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    #endregion

    #region ICS Export

    [Fact]
    public async Task ExportToIcs_ReturnsIcsFile()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync(academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar/export/ics?academyId={academyId}&startDate={DateTime.UtcNow.Date:yyyy-MM-dd}&endDate={DateTime.UtcNow.Date.AddDays(7):yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/calendar; charset=utf-8");
    }

    #endregion

    #region Helpers

    private async Task<BookingDto?> CreateBookingAsync(
        Guid? academyId = null,
        Guid? facilityId = null,
        Guid? coachId = null,
        Guid? athleteId = null)
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Calendar Test Booking",
            Description = "Test booking for calendar",
            AcademyId = academyId ?? Guid.NewGuid(),
            FacilityId = facilityId ?? Guid.NewGuid(),
            CoachId = coachId ?? Guid.NewGuid(),
            AthleteId = athleteId ?? Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
            return content?.Data;
        }
        return null;
    }

    private async Task<BookingDto?> CreateRecurringBookingAsync(Guid? academyId = null)
    {
        var request = new
        {
            BookingType = BookingType.TrainingSession,
            Title = "Recurring Calendar Event",
            Description = "Recurring test booking",
            AcademyId = academyId ?? Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            RecurrenceType = RecurrenceType.Weekly,
            OccurrenceCount = 4
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings/recurring", request);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
            return content?.Data;
        }
        return null;
    }

    #endregion
}