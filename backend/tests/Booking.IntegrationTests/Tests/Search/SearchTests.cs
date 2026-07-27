using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Search;

[Collection("Postgres")]
public class SearchTests : BaseIntegrationTest
{
    public SearchTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Search

    [Fact]
    public async Task SearchBookings_Empty_ReturnsEmptyList()
    {
        var response = await HttpClient.GetAsync("/api/v1/bookings?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchBookings_WithFilter_ReturnsFilteredResults()
    {
        await CreateBookingAsync("Filter Test Alpha");
        await CreateBookingAsync("Filter Test Beta");

        var response = await HttpClient.GetAsync("/api/v1/bookings?searchTerm=Filter+Test&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task SearchBookings_WithStatusFilter_ReturnsMatchingStatus()
    {
        await CreateBookingAsync("Pending Search Test", status: BookingStatus.Pending);
        await CreateBookingAsync("Confirmed Search Test", status: BookingStatus.Confirmed);

        var response = await HttpClient.GetAsync("/api/v1/bookings?status=Pending&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().OnlyContain(b => b.Status == BookingStatus.Pending.ToString());
    }

    [Fact]
    public async Task SearchBookings_WithBookinTypeFilter_ReturnsMatchingType()
    {
        await CreateBookingAsync("Facility Booking", type: BookingType.FacilityReservation);
        await CreateBookingAsync("Training Booking", type: BookingType.TrainingSession);

        var response = await HttpClient.GetAsync("/api/v1/bookings?bookingType=FacilityReservation&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().OnlyContain(b => b.BookingType == BookingType.FacilityReservation.ToString());
    }

    #endregion

    #region Filtering

    [Fact]
    public async Task SearchBookings_ByAcademyId_ReturnsFilteredResults()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync("Academy 1 Booking", academyId: academyId);
        await CreateBookingAsync("Academy 2 Booking", academyId: Guid.NewGuid());

        var response = await HttpClient.GetAsync($"/api/v1/bookings?academyId={academyId}&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().OnlyContain(b => b.AcademyId == academyId);
    }

    [Fact]
    public async Task SearchBookings_ByFacilityId_ReturnsFilteredResults()
    {
        var facilityId = Guid.NewGuid();
        await CreateBookingAsync("Facility 1 Booking", facilityId: facilityId);
        await CreateBookingAsync("Facility 2 Booking", facilityId: Guid.NewGuid());

        var response = await HttpClient.GetAsync($"/api/v1/bookings?facilityId={facilityId}&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().NotBeEmpty();
        content.Data!.Items.Should().OnlyContain(b => b.Title == "Facility 1 Booking");
    }

    #endregion

    #region Sorting

    [Fact]
    public async Task SearchBookings_Pagination_ReturnsPagedResults()
    {
        for (int i = 0; i < 25; i++)
        {
            await CreateBookingAsync($"Paging Test Booking {i}");
        }

        var page1Response = await HttpClient.GetAsync("/api/v1/bookings?page=1&pageSize=10");
        var page2Response = await HttpClient.GetAsync("/api/v1/bookings?page=2&pageSize=10");

        page1Response.StatusCode.Should().Be(HttpStatusCode.OK);
        page2Response.StatusCode.Should().Be(HttpStatusCode.OK);

        var page1Content = await page1Response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();
        var page2Content = await page2Response.Content.ReadFromJsonAsync<ApiResponse<SearchBookingsResponse>>();

        page1Content!.Data!.Items.Count.Should().BeLessThanOrEqualTo(10);
        page2Content!.Data!.Items.Count.Should().BeLessThanOrEqualTo(10);
        page1Content.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(20);
    }

    #endregion

    #region Upcoming Bookings

    [Fact]
    public async Task GetUpcomingBookings_ReturnsUpcomingBookings()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync("Upcoming Test", academyId: academyId);

        var response = await HttpClient.GetAsync($"/api/v1/bookings/upcoming?academyId={academyId}&daysAhead=7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<BookingSummaryDto>>>();
        content!.Success.Should().BeTrue();
    }

    #endregion

    #region Calendar View

    [Fact]
    public async Task GetCalendarView_ReturnsCalendarEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync("Calendar Event", academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar?academyId={academyId}&viewType=Monthly&viewDate={DateTime.UtcNow.Date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetResourceCalendar_ReturnsResourceEvents()
    {
        var academyId = Guid.NewGuid();
        await CreateBookingAsync("Resource Calendar Event", academyId: academyId);

        var response = await HttpClient.GetAsync(
            $"/api/v1/bookings/calendar/resource?academyId={academyId}&resourceType=Facility&resourceId={Guid.NewGuid()}&startDate={DateTime.UtcNow.Date:yyyy-MM-dd}&endDate={DateTime.UtcNow.Date.AddDays(7):yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CalendarViewResultDto>>();
        content!.Success.Should().BeTrue();
    }

    #endregion

    #region Helpers

    private async Task<BookingDto?> CreateBookingAsync(
        string title,
        BookingStatus status = BookingStatus.Pending,
        BookingType type = BookingType.FacilityReservation,
        Guid? academyId = null,
        Guid? facilityId = null)
    {
        var request = new
        {
            BookingType = type,
            Title = title,
            Description = "Search test booking",
            AcademyId = academyId ?? Guid.NewGuid(),
            FacilityId = facilityId ?? Guid.NewGuid(),
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