using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Performance;

[Collection("Postgres")]
public class PerformanceTests : BaseIntegrationTest
{
    public PerformanceTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Create Booking Performance

    [Fact]
    public async Task CreateBooking_CompletesWithinFiveSeconds()
    {
        var sw = Stopwatch.StartNew();

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Performance Create Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "creating a booking should complete within 5 seconds");
    }

    #endregion

    #region Search Performance

    [Fact]
    public async Task SearchBookings_CompletesWithinThreeSeconds()
    {
        for (int i = 0; i < 10; i++)
        {
            await CreateBookingAsync($"Search Perf Booking {i}");
        }

        var sw = Stopwatch.StartNew();

        var response = await HttpClient.GetAsync("/api/v1/bookings?page=1&pageSize=10");

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "searching bookings should complete within 3 seconds");
    }

    #endregion

    #region Availability Lookup Performance

    [Fact]
    public async Task GetBookingById_CompletesWithinOneSecond()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Availability Lookup Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var sw = Stopwatch.StartNew();

        var response = await HttpClient.GetAsync($"/api/v1/bookings/{bookingId}");

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(1000,
            because: "getting a booking by ID should complete within 1 second");
    }

    #endregion

    #region Conflict Detection Performance

    [Fact]
    public async Task ValidateBookingConflict_CompletesWithinTwoSeconds()
    {
        await CreateBookingAsync("Conflict Test Booking");

        var sw = Stopwatch.StartNew();

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings/validate-conflict", new
        {
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(2000,
            because: "conflict validation should complete within 2 seconds");
    }

    #endregion

    #region Calendar Query Performance

    [Fact]
    public async Task GetCalendarView_CompletesWithinThreeSeconds()
    {
        var academyId = Guid.NewGuid();
        for (int i = 0; i < 5; i++)
        {
            await CreateBookingAsync($"Calendar Perf Booking {i}", academyId: academyId);
        }

        var sw = Stopwatch.StartNew();

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Monthly&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "calendar query should complete within 3 seconds");
    }

    #endregion

    #region N+1 Query Detection

    [Fact]
    public async Task SearchBookings_NoNPlusOneQueries()
    {
        for (int i = 0; i < 5; i++)
        {
            await CreateBookingAsync($"N+1 Test Booking {i}");
        }

        var stopwatch = Stopwatch.StartNew();

        var response = await HttpClient.GetAsync("/api/v1/bookings?page=1&pageSize=10");

        stopwatch.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Count.Should().BeGreaterThanOrEqualTo(5);

        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000,
            because: "search should be efficient and not suffer from N+1 query problems");
    }

    #endregion

    #region Helpers

    private async Task<BookingDto?> CreateBookingAsync(
        string title,
        BookingStatus status = BookingStatus.Pending,
        Guid? academyId = null)
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = title,
            Description = "Performance test booking",
            AcademyId = academyId ?? Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid(),
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

    #endregion
}