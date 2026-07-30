using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/payments")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Payment Gateway")]
public class PaymentGatewayController : ControllerBase
{
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly IPaymentWebhookHandler _webhookHandler;
    private readonly ILogger<PaymentGatewayController> _logger;

    public PaymentGatewayController(
        IPaymentGatewayFactory gatewayFactory,
        IPaymentWebhookHandler webhookHandler,
        ILogger<PaymentGatewayController> logger)
    {
        _gatewayFactory = gatewayFactory;
        _webhookHandler = webhookHandler;
        _logger = logger;
    }

    [HttpPost("orders")]
    [ProducesResponseType(typeof(PaymentOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] PaymentOrderRequest request,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.CreateOrderAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("orders/{gatewayOrderId}/authorize")]
    [ProducesResponseType(typeof(PaymentOrderResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthorizePayment(
        string gatewayOrderId,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.AuthorizePaymentAsync(gatewayOrderId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("capture")]
    [ProducesResponseType(typeof(PaymentOrderResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CapturePayment(
        [FromBody] PaymentCaptureRequest request,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.CapturePaymentAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("orders/{gatewayOrderId}/status")]
    [ProducesResponseType(typeof(PaymentStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentStatus(
        string gatewayOrderId,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.GetPaymentStatusAsync(gatewayOrderId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refund")]
    [ProducesResponseType(typeof(PaymentRefundResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefundPayment(
        [FromBody] PaymentRefundRequest request,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.RefundPaymentAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("orders/{gatewayOrderId}/cancel")]
    [ProducesResponseType(typeof(GatewayOperationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelPayment(
        string gatewayOrderId,
        [FromBody] PaymentCancelRequest? request,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.CancelPaymentAsync(
            request ?? new PaymentCancelRequest { GatewayOrderId = gatewayOrderId },
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("orders/{gatewayOrderId}/void")]
    [ProducesResponseType(typeof(GatewayOperationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> VoidPayment(
        string gatewayOrderId,
        [FromBody] PaymentVoidRequest? request,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.VoidPaymentAsync(
            request ?? new PaymentVoidRequest { GatewayOrderId = gatewayOrderId },
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("orders/{gatewayOrderId}/retry")]
    [ProducesResponseType(typeof(PaymentOrderResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetryPayment(
        string gatewayOrderId,
        [FromBody] PaymentRetryRequest request,
        [FromQuery] string provider = "Razorpay",
        CancellationToken cancellationToken = default)
    {
        var gateway = _gatewayFactory.GetGateway(provider);
        var result = await gateway.RetryPaymentAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("webhook/{provider}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(WebhookResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleWebhook(
        string provider,
        CancellationToken cancellationToken = default)
    {
        string rawBody;
        using (var reader = new StreamReader(Request.Body))
        {
            rawBody = await reader.ReadToEndAsync(cancellationToken);
        }

        var signature = Request.Headers["X-Signature"].FirstOrDefault()
                        ?? Request.Headers["x-razorpay-signature"].FirstOrDefault()
                        ?? Request.Headers["Stripe-Signature"].FirstOrDefault()
                        ?? string.Empty;

        var webhookId = Request.Headers["X-Webhook-Id"].FirstOrDefault()
                        ?? Request.Headers["X-Event-Id"].FirstOrDefault()
                        ?? Guid.NewGuid().ToString();

        var eventType = Request.Headers["X-Event-Type"].FirstOrDefault()
                        ?? Request.Query["event"].FirstOrDefault()
                        ?? "unknown";

        var payload = new WebhookPayload
        {
            RawBody = rawBody,
            Signature = signature,
            EventType = eventType,
            WebhookId = webhookId,
            Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _webhookHandler.HandleWebhookAsync(payload, provider, cancellationToken);
        return Ok(result);
    }

    [HttpGet("providers")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public IActionResult GetProviders()
    {
        var providers = _gatewayFactory.GetRegisteredProviders();
        return Ok(providers);
    }
}
