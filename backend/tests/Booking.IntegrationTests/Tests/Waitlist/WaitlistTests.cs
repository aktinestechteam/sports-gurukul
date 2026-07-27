using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Waitlist;

[Collection("Postgres")]
public class WaitlistTests : BaseIntegrationTest
{
    public WaitlistTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Join Waitlist

    [Fact]
    public async Task JoinWaitlist_ValidBooking_AddsToWaitlist()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Waitlist Booking",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid(),
            Notes = "Preferred morning slots"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task JoinWaitlist_BookingNotFound_ReturnsNotFound()
    {
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task JoinWaitlist_DuplicateUser_ReturnsConflict()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Duplicate Waitlist Booking",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid()
        });

        var duplicateResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid()
        });

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task JoinWaitlist_MultipleUsers_PrioritiesIncrement()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Priority Waitlist Booking",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response1 = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid(),
            Notes = "First in line"
        });
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var response2 = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid(),
            Notes = "Second in line"
        });
        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var content1 = await response1.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var content2 = await response2.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();

        content2!.Data!.Priority.Should().BeGreaterThan(content1!.Data!.Priority);
    }

    #endregion

    #region Remove Waitlist

    [Fact]
    public async Task RemoveFromWaitlist_ExistingEntry_RemovesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Waitlist Remove Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var joinResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid()
        });
        var joinContent = await joinResponse.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var waitlistEntryId = joinContent!.Data!.Id;

        var response = await HttpClient.DeleteAsync($"/api/v1/bookings/{bookingId}/waitlist/{waitlistEntryId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveFromWaitlist_NotExists_ReturnsNotFound()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Remove Nonexistent Waitlist",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response = await HttpClient.DeleteAsync($"/api/v1/bookings/{bookingId}/waitlist/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Automatic Promotion

    [Fact]
    public async Task PromoteWaitlistedBooking_ActiveEntry_PromotesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Promotion Test Booking",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var joinResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
        {
            WaitlistUserId = Guid.NewGuid()
        });
        var joinContent = await joinResponse.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
        var waitlistEntryId = joinContent!.Data!.Id;

        var response = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/waitlist/{waitlistEntryId}/promote", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task PromoteWaitlistedBooking_NotExists_ReturnsNotFound()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Promote Nonexistent",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/waitlist/{Guid.NewGuid()}/promote", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Priority Ordering

    [Fact]
    public async Task JoinWaitlist_PriorityOrdering_IsSequential()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Priority Order Test",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var priorities = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            var resp = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/waitlist", new
            {
                WaitlistUserId = Guid.NewGuid(),
                Notes = $"Entry {i}"
            });
            resp.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await resp.Content.ReadFromJsonAsync<ApiResponse<WaitlistDto>>();
            priorities.Add(content!.Data!.Priority);
        }

        priorities[0].Should().BeLessThan(priorities[1]);
        priorities[1].Should().BeLessThan(priorities[2]);
    }

    #endregion
}