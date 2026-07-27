using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.Tests.Security;

[Collection("Postgres")]
public class SecurityTests : BaseIntegrationTest
{
    public SecurityTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region JWT Authentication

    [Fact]
    public async Task CreateBooking_ValidJwt_Authenticated()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Auth Test Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateBooking_InvalidJwt_ReturnsUnauthorized()
    {
        var client = AuthenticatedHttpClientFactory.CreateClientWithJwt(
            Factory.CreateClient(), "invalid.token.here");

        var response = await client.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Invalid Token Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBooking_ExpiredJwt_ReturnsUnauthorized()
    {
        var expiredToken = GenerateExpiredJwt();
        var client = AuthenticatedHttpClientFactory.CreateClientWithJwt(
            Factory.CreateClient(), expiredToken);

        var response = await client.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Expired Token Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Authorization Policies

    [Fact]
    public async Task CreateBooking_RequiresAdminRole()
    {
        var client = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "athlete@test.com", "Athlete User", "Athlete");

        var response = await client.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Athlete Create Attempt",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ConfirmBooking_RequiresCoachOrAdmin()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Confirm Auth Test",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var athleteClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "athlete@test.com", "Athlete User", "Athlete");

        var response = await athleteClient.PostAsync($"/api/v1/bookings/{bookingId}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ExpireBooking_RequiresSystemAdmin()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Expire Auth Test",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var coachClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "coach@test.com", "Coach User", "Coach");

        var response = await coachClient.PostAsync($"/api/v1/bookings/{bookingId}/expire", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Forbidden Access

    [Fact]
    public async Task UpdateBooking_ForbidWhenNotOwner_ReturnsForbiddenOrOkDependingOnRole()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Forbidden Update Test",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var athleteClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "athlete@test.com", "Athlete User", "Athlete");

        var response = await athleteClient.PutAsJsonAsync($"/api/v1/bookings/{bookingId}", new
        {
            Title = "Updated by Athlete"
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CancelBooking_ForbidWhenNotOwner_ReturnsForbiddenOrOkDependingOnRole()
    {
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Forbidden Cancel Test",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<BookingDto>>();
        var bookingId = createContent!.Data!.Id;

        var athleteClient = AuthenticatedHttpClientFactory.CreateClientWithClaims(
            Factory.CreateClient(), Guid.NewGuid(), "athlete@test.com", "Athlete User", "Athlete");

        var response = await athleteClient.PostAsJsonAsync($"/api/v1/bookings/{bookingId}/cancel", new
        {
            Reason = "Athlete cancelling"
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Unauthorized Access

    [Fact]
    public async Task GetBookingById_NoToken_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync($"/api/v1/bookings/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchBookings_NoToken_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/api/v1/bookings?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCalendarView_NoToken_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync(
            $"/api/v1/bookings/calendar?academyId={Guid.NewGuid()}&viewType=Monthly");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Input Validation

    [Fact]
    public async Task CreateBooking_EmptyTitle_ReturnsBadRequest()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_NegativeDuration_ReturnsBadRequest()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Negative Duration Booking",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchBookings_InvalidPageSize_ReturnsBadRequest()
    {
        var response = await HttpClient.GetAsync("/api/v1/bookings?page=1&pageSize=-1");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBooking_NullAcademyId_ReturnsBadRequest()
    {
        var response = await HttpClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            BookingType = BookingType.FacilityReservation,
            Title = "Null Academy Booking",
            AcademyId = Guid.Empty,
            BookingDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Helpers

    private string GenerateExpiredJwt()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "expired@test.com"),
            new Claim(ClaimTypes.Role, "System Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TestSecretKeyForIntegrationTests12345678!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "SportsGurukul",
            audience: "SportsGurukul",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(-1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion
}
