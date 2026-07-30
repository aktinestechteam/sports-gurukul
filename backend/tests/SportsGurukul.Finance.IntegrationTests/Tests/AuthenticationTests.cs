using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using SportsGurukul.Platform.PaymentGateway.Models;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class AuthenticationTests : FinanceTestBase
{
    public AuthenticationTests(FinanceWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Admin_CanAccess_PaymentEndpoints()
    {
        var request = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 1000m,
            Currency = "INR",
            Description = "Test order"
        };

        var response = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", request);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Anonymous_CannotAccess_PaymentEndpoints()
    {
        var request = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 1000m,
            Currency = "INR",
            Description = "Test order"
        };

        var response = await PostAsJsonAsync(AnonymousClient, "api/v1/payments/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Athlete_WithExpiredToken_Returns401()
    {
        var client = Factory.CreateClient();
        var expiredToken = JwtTokenHelper.GenerateExpiredToken(
            FinanceTestIds.AthleteUserId,
            FinanceTestIds.AthleteEmail,
            "Athlete");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

        var request = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 1000m,
            Currency = "INR",
            Description = "Test order"
        };

        var response = await PostAsJsonAsync(client, "api/v1/payments/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
