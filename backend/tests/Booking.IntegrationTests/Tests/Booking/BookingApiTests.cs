using System.Net;
using System.Net.Http.Json;
using System.Diagnostics;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Booking.IntegrationTests.SeedBuilders;

namespace Booking.IntegrationTests.Tests.Booking;

[Collection("Postgres")]
public class BookingApiTests : BaseIntegrationTest
{
    public BookingApiTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Create Booking

    [Fact]
    public async Task CreateBooking_Admin_CreatesSuccessfully()
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Morning Training Session",
            Description = "Reserved court for morning practice",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Title.Should().Be("Morning Training Session");
        content.Data.Status.Should().Be(BookingStatus.Pending.ToString());
    }

    [Fact]
    public async Task CreateBooking_Unauthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Unauth Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };

        var response = await client.PostAsJsonAsync("/api/v1/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBooking_WrongRole_ReturnsForbidden()
    {
        var client = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "athlete@test.com", "Athlete User", "Athlete");

        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Athlete Booking Attempt",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };

        var response = await client.PostAsJsonAsync("/api/v1/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBooking_MissingTitle_ReturnsBadRequest()
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_StartAfterEnd_ReturnsBadRequest()
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Bad Time Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_PastDate_ReturnsBadRequest()
    {
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Past Date Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(-5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Get Booking

    [Fact]
    public async Task GetBookingById_ExistingBooking_ReturnsBooking()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Get Booking Test",
            AcademyId = Guid.NewGuid(),
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
        getContent.Data.Title.Should().Be("Get Booking Test");
    }

    [Fact]
    public async Task GetBookingById_NonExisting_ReturnsNotFound()
    {
        var response = await HttpClient.GetAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBookingById_Unauthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Update Booking

    [Fact]
    public async Task UpdateBooking_ExistingBooking_UpdatesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Original Title",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var updateResponse = await HttpClient.PutAsJsonAsync($"/api/v1/bookings/{bookingId}", new
        {
            Title = "Updated Title",
            Description = "Updated description"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updateContent = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        updateContent!.Success.Should().BeTrue();
        updateContent.Data!.Title.Should().Be("Updated Title");
        updateContent.Data.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateBooking_NonExisting_ReturnsNotFound()
    {
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}", new
        {
            Title = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBooking_ConfirmedBooking_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Confirmable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        var updateResponse = await HttpClient.PutAsJsonAsync($"/api/v1/bookings/{bookingId}", new
        {
            Title = "Should Fail"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Booking

    [Fact]
    public async Task DeleteBooking_ExistingBooking_DeletesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Deletable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var deleteResponse = await HttpClient.DeleteAsync($"/api/v1/bookings/{bookingId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await HttpClient.GetAsync($"/api/v1/bookings/{bookingId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBooking_NonExisting_ReturnsNotFound()
    {
        var response = await HttpClient.DeleteAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Cancel Booking

    [Fact]
    public async Task CancelBooking_PendingBooking_CancelsSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Cancellable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var cancelResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/cancel", new
        {
            Reason = "Schedule conflict",
            Notes = "Will reschedule"
        });

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var cancelContent = await cancelResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        cancelContent!.Success.Should().BeTrue();
        cancelContent.Data!.Status.Should().Be(BookingStatus.Cancelled.ToString());
    }

    [Fact]
    public async Task CancelBooking_AlreadyCancelled_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Already Cancelled",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/cancel", new
        {
            Reason = "First cancel"
        });

        var secondCancelResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/cancel", new
        {
            Reason = "Double cancel"
        });

        secondCancelResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelBooking_NonExisting_ReturnsNotFound()
    {
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{Guid.NewGuid()}/cancel", new
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
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Confirmable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var confirmResponse = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var confirmContent = await confirmResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        confirmContent!.Success.Should().BeTrue();
        confirmContent.Data!.Status.Should().Be(BookingStatus.Confirmed.ToString());
    }

    [Fact]
    public async Task ConfirmBooking_NotPending_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Pending Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        var secondConfirmResponse = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        secondConfirmResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Complete Booking

    [Fact]
    public async Task CompleteBooking_ConfirmedBooking_CompletesSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Completable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        var completeResponse = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/complete", null);

        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var completeContent = await completeResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        completeContent!.Success.Should().BeTrue();
        completeContent.Data!.Status.Should().Be(BookingStatus.Completed.ToString());
    }

    [Fact]
    public async Task CompleteBooking_NotConfirmed_ReturnsBadRequest()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Pending Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var completeResponse = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/complete", null);

        completeResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Expire Booking

    [Fact]
    public async Task ExpireBooking_PendingBooking_ExpiresSuccessfully()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Expirable Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var expireResponse = await HttpClient.PostAsync($"/api/v1/bookings/{bookingId}/expire", null);

        expireResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var expireContent = await expireResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        expireContent!.Success.Should().BeTrue();
        expireContent.Data!.Status.Should().Be(BookingStatus.Expired.ToString());
    }

    [Fact]
    public async Task ExpireBooking_NonSystemAdmin_ReturnsForbidden()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Expirable Booking",
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

        var expireResponse = await coachClient.PostAsync($"/api/v1/bookings/{bookingId}/expire", null);

        expireResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Reschedule Booking

    [Fact]
    public async Task RescheduleBooking_PendingBooking_ReschedulesSuccessfully()
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

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var newDate = DateTime.UtcNow.Date.AddDays(5);
        var rescheduleResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reschedule", new
        {
            NewDate = newDate,
            NewStartTime = new TimeSpan(14, 0, 0),
            NewEndTime = new TimeSpan(15, 30, 0),
            Reason = "Coach requested change"
        });

        rescheduleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var rescheduleContent = await rescheduleResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        rescheduleContent!.Success.Should().BeTrue();
        rescheduleContent.Data!.BookingDate.Date.Should().Be(newDate.Date);
        rescheduleContent.Data.StartTime.Should().Be(new TimeSpan(14, 0, 0));
        rescheduleContent.Data.EndTime.Should().Be(new TimeSpan(15, 30, 0));
    }

    [Fact]
    public async Task RescheduleBooking_NewStartAfterEnd_ReturnsBadRequest()
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

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var response = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reschedule", new
        {
            NewDate = DateTime.UtcNow.Date.AddDays(5),
            NewStartTime = new TimeSpan(15, 0, 0),
            NewEndTime = new TimeSpan(14, 0, 0),
            Reason = "Bad times"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        var rejectResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reject", new
        {
            Reason = "Facility under maintenance"
        });

        rejectResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var rejectContent = await rejectResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        rejectContent!.Success.Should().BeTrue();
        rejectContent.Data!.Status.Should().Be(BookingStatus.Rejected.ToString());
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

        var rejectResponse = await HttpClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/reject", new
        {
            Reason = "Cannot reject confirmed"
        });

        rejectResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Performance

    [Fact]
    public async Task CreateBooking_CompletesWithinTimeLimit()
    {
        var sw = Stopwatch.StartNew();
        var request = new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Performance Test Booking",
            AcademyId = Guid.NewGuid(),
            FacilityId = Guid.NewGuid(),
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0)
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", request);
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            because: "creating a booking should complete within 5 seconds");
    }

    #endregion
}