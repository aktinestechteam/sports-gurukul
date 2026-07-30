using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class ApiValidationTests : FinanceTestBase
{
    private const string OrdersUrl = "api/v1/payments/orders";
    private const string ProvidersUrl = "api/v1/payments/providers";

    public ApiValidationTests(FinanceWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrder_WithMissingAmount_Returns400()
    {
        var response = await PostAsJsonAsync(AdminClient, OrdersUrl, new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithNegativeAmount_Returns400()
    {
        var body = new { amount = -100, currency = "INR" };

        var response = await PostAsJsonAsync(AdminClient, OrdersUrl, body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidCurrency_Returns400()
    {
        var body = new { amount = 1000, currency = "XYZ" };

        var response = await PostAsJsonAsync(AdminClient, OrdersUrl, body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPaymentStatus_WithInvalidId_Returns404()
    {
        var response = await GetAsync(AdminClient, $"{OrdersUrl}/nonexistent/status");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithZeroAmount_Returns400()
    {
        var body = new { amount = 0, currency = "INR" };

        var response = await PostAsJsonAsync(AdminClient, OrdersUrl, body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProviders_Returns200()
    {
        var response = await GetAsync(AnonymousClient, ProvidersUrl);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_ReturnsOkResponse()
    {
        var response = await GetAsync(AnonymousClient, ProvidersUrl);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);

        body.ValueKind.Should().NotBe(JsonValueKind.Null);
        body.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task GetProviders_ResponseTime_UnderThreshold()
    {
        var sw = Stopwatch.StartNew();

        var response = await GetAsync(AnonymousClient, ProvidersUrl);

        sw.Stop();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000);
    }

    [Fact]
    public async Task ApiEndpoints_ReturnJsonContentType()
    {
        var response = await GetAsync(AnonymousClient, ProvidersUrl);

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task ApiEndpoints_SupportProblemDetails()
    {
        var body = new { amount = -1, currency = "INR" };

        var response = await PostAsJsonAsync(AdminClient, OrdersUrl, body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.TryGetProperty("type", out _).Should().BeTrue();
        content.TryGetProperty("title", out _).Should().BeTrue();
        content.TryGetProperty("status", out var statusProp).Should().BeTrue();
        statusProp.GetInt32().Should().Be(400);
    }

    [Fact]
    public async Task PaymentEndpoints_RequireAuth()
    {
        var postBody = new { amount = 1000, currency = "INR" };

        var response = await PostAsJsonAsync(AnonymousClient, OrdersUrl, postBody);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProviders_IsPublic_Returns200WithoutAuth()
    {
        var response = await GetAsync(AnonymousClient, ProvidersUrl);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
