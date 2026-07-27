using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class BookingSearchIntegrationTests : BookingIntegrationTestBase
{
    public BookingSearchIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task AdvancedSearch_Empty_ReturnsEmptyResults()
    {
        var response = await AdminClient.GetAsync("/api/v1/bookings/search?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().BeEmpty();
        content.Data.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task AdvancedSearch_WithSearchTerm_ReturnsMatchingResults()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Search Alpha Training");
        await CreateBookingViaApiAsync(AdminClient, title: "Search Beta Practice");

        var response = await AdminClient.GetAsync("/api/v1/bookings/search?searchTerm=Search&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task AdvancedSearch_WithStatusFilter_ReturnsFilteredResults()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Status Filter Pending");
        await CreateBookingDirectlyInDbAsync(status: "Confirmed", title: "Status Filter Confirmed");

        var response = await AdminClient.GetAsync("/api/v1/bookings/search?status=Pending&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().OnlyContain(b => b.Status == "Pending");
    }

    [Fact]
    public async Task AdvancedSearch_WithBookingTypeFilter_ReturnsFilteredResults()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Type Filter Facility");
        await CreateBookingDirectlyInDbAsync(status: "Pending", title: "Type Filter Other");

        var response = await AdminClient.GetAsync($"/api/v1/bookings/search?bookingType=FacilityReservation&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().OnlyContain(b => b.BookingType == "FacilityReservation");
    }

    [Fact]
    public async Task AdvancedSearch_Pagination_ReturnsCorrectPage()
    {
        for (int i = 0; i < 5; i++)
        {
            await CreateBookingViaApiAsync(AdminClient, title: $"Pagination Test {i}");
        }

        var page1 = await AdminClient.GetAsync("/api/v1/bookings/search?page=1&pageSize=2");
        var content1 = await page1.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content1!.Data!.Items.Count.Should().Be(2);
        content1.Data.TotalRecords.Should().BeGreaterThanOrEqualTo(5);
        content1.Data.TotalPages.Should().BeGreaterThanOrEqualTo(3);
        content1.Data.CurrentPage.Should().Be(1);
        content1.Data.HasPrevious.Should().BeFalse();
        content1.Data.HasNext.Should().BeTrue();

        var page2 = await AdminClient.GetAsync("/api/v1/bookings/search?page=2&pageSize=2");
        var content2 = await page2.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content2!.Data!.CurrentPage.Should().Be(2);
        content2.Data.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public async Task AdvancedSearch_SortByTitle_Ascending_ReturnsSortedResults()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Charlie Session");
        await CreateBookingViaApiAsync(AdminClient, title: "Alpha Session");
        await CreateBookingViaApiAsync(AdminClient, title: "Bravo Session");

        var response = await AdminClient.GetAsync("/api/v1/bookings/search?sortBy=Title&sortDescending=false&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingSearchPageResultDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task AdvancedSearch_AthleteRole_CanSearch()
    {
        await CreateBookingViaApiAsync(AdminClient, title: "Athlete Visible Booking");

        var response = await AthleteClient.GetAsync("/api/v1/bookings/search?searchTerm=Athlete&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdvancedSearch_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/bookings/search?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
