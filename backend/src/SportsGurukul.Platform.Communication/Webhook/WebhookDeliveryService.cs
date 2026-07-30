using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Webhook;

public class WebhookDeliveryService
{
    private readonly HttpClient _httpClient;
    private readonly WebhookSignatureValidator _signatureValidator;
    private readonly SecurityOptions _options;
    private readonly ILogger<WebhookDeliveryService> _logger;

    public WebhookDeliveryService(
        HttpClient httpClient,
        WebhookSignatureValidator signatureValidator,
        IOptions<CommunicationOptions> options,
        ILogger<WebhookDeliveryService> logger)
    {
        _httpClient = httpClient;
        _signatureValidator = signatureValidator;
        _options = options.Value.Security;
        _logger = logger;
    }

    public async Task<WebhookDeliveryResult> DeliverAsync(
        Uri webhookUrl,
        object payload,
        string? secret = null,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var effectiveSecret = secret ?? _options.DefaultWebhookSecret;

            if (!string.IsNullOrEmpty(effectiveSecret) && _options.WebhookSignatureValidationEnabled)
            {
                var signature = _signatureValidator.GenerateSignature(json, effectiveSecret);
                content.Headers.Add("X-Webhook-Signature", signature);
                content.Headers.Add("X-Webhook-Timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            }

            var response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);
            sw.Stop();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Webhook delivered to {Url} with status {Status} in {Duration}ms",
                    webhookUrl, (int)response.StatusCode, sw.ElapsedMilliseconds);

                return new WebhookDeliveryResult
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                    ResponseBody = responseBody,
                    DurationMs = sw.ElapsedMilliseconds
                };
            }

            _logger.LogWarning("Webhook delivery to {Url} failed with status {Status}: {Body}",
                webhookUrl, (int)response.StatusCode, responseBody);

            return new WebhookDeliveryResult
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ResponseBody = responseBody,
                ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}",
                DurationMs = sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Webhook delivery to {Url} threw exception", webhookUrl);

            return new WebhookDeliveryResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                DurationMs = sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<WebhookDeliveryResult> DeliverDeliveryCallbackAsync(
        Uri callbackUrl,
        Guid notificationId,
        Guid deliveryId,
        string status,
        string? providerMessageId,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Event = "delivery_update",
            NotificationId = notificationId.ToString(),
            DeliveryId = deliveryId.ToString(),
            Status = status,
            ProviderMessageId = providerMessageId,
            Timestamp = DateTime.UtcNow
        };

        return await DeliverAsync(callbackUrl, payload, cancellationToken: cancellationToken);
    }

    public async Task<WebhookDeliveryResult> DeliverBounceNotificationAsync(
        Uri bounceUrl,
        string providerMessageId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Event = "bounce",
            ProviderMessageId = providerMessageId,
            Reason = reason,
            Timestamp = DateTime.UtcNow
        };

        return await DeliverAsync(bounceUrl, payload, cancellationToken: cancellationToken);
    }
}

public class WebhookDeliveryResult
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? ErrorMessage { get; set; }
    public long DurationMs { get; set; }
}
