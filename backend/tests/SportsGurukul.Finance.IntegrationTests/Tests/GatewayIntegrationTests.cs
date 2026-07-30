using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Platform.PaymentGateway.Models;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class GatewayIntegrationTests : FinanceTestBase
{
    public GatewayIntegrationTests(FinanceWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrder_AsAdmin_ReturnsSuccess()
    {
        var request = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 5000m,
            Currency = "INR"
        };

        var response = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);
        order.Should().NotBeNull();
        order!.Amount.Should().Be(5000m);
        order.Currency.Should().Be("INR");
    }

    [Fact]
    public async Task CreateOrder_AsAnonymous_Returns401()
    {
        var request = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 1000m,
            Currency = "INR"
        };

        var response = await PostAsJsonAsync(AnonymousClient, "api/v1/payments/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPaymentStatus_WithValidOrder_ReturnsStatus()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 3000m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var response = await GetAsync(AdminClient, $"api/v1/payments/orders/{createdOrder!.GatewayOrderId}/status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PaymentStatusResponse>(JsonOptions);
        status.Should().NotBeNull();
        status!.GatewayOrderId.Should().Be(createdOrder.GatewayOrderId);
    }

    [Fact]
    public async Task AuthorizePayment_WithValidOrder_ReturnsOk()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 4000m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var response = await PostAsJsonAsync(AdminClient, $"api/v1/payments/orders/{createdOrder!.GatewayOrderId}/authorize", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CapturePayment_WithValidData_ReturnsOk()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 6000m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var captureRequest = new PaymentCaptureRequest
        {
            GatewayOrderId = createdOrder!.GatewayOrderId,
            Amount = 6000m,
            Currency = "INR"
        };
        var response = await PostAsJsonAsync(AdminClient, "api/v1/payments/capture", captureRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefundPayment_WithValidData_ReturnsOk()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 2000m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var refundRequest = new PaymentRefundRequest
        {
            GatewayPaymentId = createdOrder!.GatewayOrderId,
            Amount = 2000m,
            Reason = "Customer requested refund"
        };
        var response = await PostAsJsonAsync(AdminClient, "api/v1/payments/refund", refundRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelPayment_WithValidOrder_ReturnsOk()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 3500m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var cancelRequest = new PaymentCancelRequest
        {
            GatewayOrderId = createdOrder!.GatewayOrderId,
            Reason = "Order cancelled by admin"
        };
        var response = await PostAsJsonAsync(AdminClient, $"api/v1/payments/orders/{createdOrder.GatewayOrderId}/cancel", cancelRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VoidPayment_WithValidOrder_ReturnsOk()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 4500m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var voidRequest = new PaymentVoidRequest
        {
            GatewayOrderId = createdOrder!.GatewayOrderId,
            Reason = "Voided by admin"
        };
        var response = await PostAsJsonAsync(AdminClient, $"api/v1/payments/orders/{createdOrder.GatewayOrderId}/void", voidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RetryPayment_WithValidOrder_ReturnsOk()
    {
        var createRequest = new PaymentOrderRequest
        {
            OrderId = Guid.NewGuid().ToString(),
            Amount = 5500m,
            Currency = "INR"
        };
        var createResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var retryRequest = new PaymentRetryRequest
        {
            GatewayOrderId = createdOrder!.GatewayOrderId
        };
        var response = await PostAsJsonAsync(AdminClient, $"api/v1/payments/orders/{createdOrder.GatewayOrderId}/retry", retryRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_ReturnsProviderList()
    {
        var response = await GetAsync(AdminClient, "api/v1/payments/providers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var providers = await response.Content.ReadFromJsonAsync<List<string>>(JsonOptions);
        providers.Should().NotBeNull();
    }

    [Fact]
    public async Task Webhook_ValidPayload_ReturnsOk()
    {
        var payload = new
        {
            event_type = "payment.success",
            payment_id = "pay_test_001",
            order_id = "order_test_001",
            amount = 50000,
            currency = "INR"
        };

        var response = await PostAsJsonAsync(AnonymousClient, "api/v1/payments/webhook/razorpay", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WebhookResult>(JsonOptions);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Webhook_InvalidProvider_ReturnsBadRequest()
    {
        var payload = new { event_type = "payment.success" };
        var response = await PostAsJsonAsync(AnonymousClient, "api/v1/payments/webhook/invalidprovider", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithDuplicateIdempotencyKey_ReturnsSameResult()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var orderId = Guid.NewGuid().ToString();

        var request = new PaymentOrderRequest
        {
            OrderId = orderId,
            Amount = 8000m,
            Currency = "INR",
            IdempotencyKey = idempotencyKey
        };

        var firstResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var firstOrder = await firstResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        var secondResponse = await PostAsJsonAsync(AdminClient, "api/v1/payments/orders", request);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var secondOrder = await secondResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);

        secondOrder!.GatewayOrderId.Should().Be(firstOrder!.GatewayOrderId);
    }

    [Fact]
    public async Task CancelPayment_AsAnonymous_Returns401()
    {
        var cancelRequest = new PaymentCancelRequest
        {
            GatewayOrderId = Guid.NewGuid().ToString(),
            Reason = "Unauthorized attempt"
        };
        var response = await PostAsJsonAsync(AnonymousClient, $"api/v1/payments/orders/{Guid.NewGuid()}/cancel", cancelRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProviders_AsAnonymous_Returns200()
    {
        var response = await GetAsync(AnonymousClient, "api/v1/payments/providers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var providers = await response.Content.ReadFromJsonAsync<List<string>>(JsonOptions);
        providers.Should().NotBeNull();
    }
}
