using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Webhook;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Webhook;

public class WebhookDeliveryServiceTests
{
    private readonly WebhookSignatureValidator _signatureValidator;
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<WebhookDeliveryService>> _logger;
    private readonly MockHttpMessageHandlerBuilder _handlerBuilder;
    private readonly HttpClient _httpClient;
    private readonly WebhookDeliveryService _service;

    public WebhookDeliveryServiceTests()
    {
        _signatureValidator = new WebhookSignatureValidator();
        _options = TestDataFactory.CreateOptions(o =>
        {
            o.Security.WebhookSignatureValidationEnabled = true;
            o.Security.DefaultWebhookSecret = "test-secret";
        });
        _logger = new Mock<ILogger<WebhookDeliveryService>>();
        _handlerBuilder = new MockHttpMessageHandlerBuilder();
        _httpClient = _handlerBuilder.CreateClient();
        _service = new WebhookDeliveryService(_httpClient, _signatureValidator, _options, _logger.Object);
    }

    [Fact]
    public async Task DeliverAsync_SendsWebhookPayload()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test", Data = "value" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeliverAsync_IncludesHMACSignatureHeader()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload, "custom-secret");

        result.IsSuccess.Should().BeTrue();
        _handlerBuilder.HandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Content!.Headers.Contains("X-Webhook-Signature") &&
                r.Content!.Headers.Contains("X-Webhook-Timestamp")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeliverAsync_HandlesHTTPSuccess()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Status = "ok" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK, """{"received":true}""");

        var result = await _service.DeliverAsync(url, payload);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.ResponseBody.Should().Contain("received");
    }

    [Fact]
    public async Task DeliverAsync_HandlesHTTPFailure()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.InternalServerError, "Server Error");

        var result = await _service.DeliverAsync(url, payload);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(500);
        result.ErrorMessage.Should().Contain("500");
    }

    [Fact]
    public async Task DeliverAsync_HandlesHttpRequestException()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.ThrowsOnSend(new HttpRequestException("Connection refused"));

        var result = await _service.DeliverAsync(url, payload);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Connection refused");
    }

    [Fact]
    public async Task DeliverAsync_LogsDeliveryAttempt()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload);

        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Webhook delivered")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task DeliverAsync_LogsFailedDelivery()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.BadRequest, "Bad Request");

        var result = await _service.DeliverAsync(url, payload);

        _logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Webhook delivery") && v.ToString()!.Contains("failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task DeliverAsync_IncludesTimestampHeader()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload, "secret123");

        _handlerBuilder.HandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Content!.Headers.Contains("X-Webhook-Timestamp")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeliverAsync_UsesDefaultSecret_WhenNoneProvided()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload);

        _handlerBuilder.HandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Content!.Headers.Contains("X-Webhook-Signature")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeliverAsync_SkipsSignature_WhenValidationDisabled()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.Security.WebhookSignatureValidationEnabled = false;
            o.Security.DefaultWebhookSecret = "secret";
        });

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var service = new WebhookDeliveryService(_httpClient, _signatureValidator, options, _logger.Object);
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        var result = await service.DeliverAsync(url, payload, "secret");

        _handlerBuilder.HandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                !r.Content!.Headers.Contains("X-Webhook-Signature")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeliveryCallback_SendsDeliveryUpdate()
    {
        var url = new Uri("https://example.com/callback");
        var notificationId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverDeliveryCallbackAsync(url, notificationId, deliveryId, "sent", "ext-msg-id");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task BounceNotification_SendsBouncePayload()
    {
        var url = new Uri("https://example.com/bounce");

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverBounceNotificationAsync(url, "ext-msg-id", "invalid email");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeliverAsync_ReturnsDurationMs()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload);

        result.DurationMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task DeliverAsync_SendsAsJson()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { Event = "test" };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload);

        _handlerBuilder.HandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Content!.Headers!.ContentType!.MediaType == "application/json"),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeliverAsync_HandlesEmptyPayload()
    {
        var url = new Uri("https://example.com/webhook");
        var payload = new { };

        _handlerBuilder.RespondWith(HttpStatusCode.OK);

        var result = await _service.DeliverAsync(url, payload);

        result.IsSuccess.Should().BeTrue();
    }
}
