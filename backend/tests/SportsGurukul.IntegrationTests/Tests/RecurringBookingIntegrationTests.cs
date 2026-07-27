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

public class RecurringBookingIntegrationTests : BookingIntegrationTestBase
{
    public RecurringBookingIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task CreateRecurringBooking_Daily_CreatesSuccessfully()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Daily Training Series",
            Description = "Daily morning training",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            CoachId = TestCoachEntityId,
            StartDate = startDate,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            RecurrenceType = RecurrenceType.Daily,
            OccurrenceCount = 5
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.BookingNumber.Should().StartWith("BK-");
        content.Data.Title.Should().Be("Daily Training Series");
        content.Data.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task CreateRecurringBooking_Weekly_CreatesSuccessfully()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.GroupCoaching,
            Title = "Weekly Group Coaching",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            StartDate = startDate,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            RecurrenceType = RecurrenceType.Weekly,
            OccurrenceCount = 4
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRecurringBooking_Monthly_CreatesSuccessfully()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.TournamentSlot,
            Title = "Monthly Tournament",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            StartDate = startDate,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            RecurrenceType = RecurrenceType.Monthly,
            OccurrenceCount = 3
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRecurringBooking_PastStartDate_ReturnsBadRequest()
    {
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Past Date Recurring",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            StartDate = DateTime.UtcNow.Date.AddDays(-5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            RecurrenceType = RecurrenceType.Daily,
            OccurrenceCount = 5
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRecurringBooking_StartAfterEnd_ReturnsBadRequest()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Bad Time Recurring",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            StartDate = startDate,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(9, 0, 0),
            RecurrenceType = RecurrenceType.Daily,
            OccurrenceCount = 5
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRecurringBooking_AthleteRole_ReturnsForbidden()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var response = await AthleteClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Athlete Recurring Attempt",
            AcademyId = TestAcademyId,
            StartDate = startDate,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            RecurrenceType = RecurrenceType.Daily,
            OccurrenceCount = 3
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateRecurringBooking_WithEndDate_CreatesSuccessfully()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = startDate.AddDays(28);
        var response = await AdminClient.PostAsJsonAsync("/api/v1/bookings/recurring", new
        {
            BookingType = BookingType.TrainingSession,
            Title = "End Date Series",
            AcademyId = TestAcademyId,
            FacilityId = TestFacilityId,
            StartDate = startDate,
            StartTime = new TimeSpan(7, 0, 0),
            EndTime = new TimeSpan(8, 30, 0),
            RecurrenceType = RecurrenceType.Weekly,
            EndDate = endDate
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }
}
