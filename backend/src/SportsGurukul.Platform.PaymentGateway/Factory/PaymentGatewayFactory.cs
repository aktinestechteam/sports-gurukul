using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;

namespace SportsGurukul.Platform.PaymentGateway.Factory;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly ConcurrentDictionary<string, IPaymentGateway> _gateways = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<PaymentGatewayFactory> _logger;
    private string _defaultProvider = string.Empty;

    public PaymentGatewayFactory(ILogger<PaymentGatewayFactory> logger)
    {
        _logger = logger;
    }

    public IPaymentGateway GetGateway(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            _logger.LogWarning("Null or empty provider requested, returning default");
            return GetDefaultGateway();
        }

        if (_gateways.TryGetValue(providerName, out var gateway))
            return gateway;

        _logger.LogError("Payment gateway provider '{Provider}' is not registered", providerName);
        throw new InvalidOperationException($"Payment gateway provider '{providerName}' is not registered");
    }

    public IPaymentGateway GetDefaultGateway()
    {
        if (string.IsNullOrEmpty(_defaultProvider) || !_gateways.TryGetValue(_defaultProvider, out var gateway))
        {
            gateway = _gateways.Values.FirstOrDefault();
            if (gateway is null)
                throw new InvalidOperationException("No payment gateways are registered");
        }
        return gateway;
    }

    public IReadOnlyCollection<string> GetRegisteredProviders()
    {
        return _gateways.Keys.ToList().AsReadOnly();
    }

    public bool IsProviderSupported(string providerName)
    {
        return _gateways.ContainsKey(providerName);
    }

    public void RegisterProvider(string providerName, IPaymentGateway gateway)
    {
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(gateway);

        if (_gateways.TryAdd(providerName, gateway))
        {
            _logger.LogInformation("Registered payment gateway provider '{Provider}'", providerName);
        }
        else
        {
            _logger.LogWarning("Payment gateway provider '{Provider}' is already registered, overwriting", providerName);
            _gateways[providerName] = gateway;
        }

        if (string.IsNullOrEmpty(_defaultProvider))
            _defaultProvider = providerName;
    }

    public void SetDefaultProvider(string providerName)
    {
        if (_gateways.ContainsKey(providerName))
        {
            _defaultProvider = providerName;
            _logger.LogInformation("Default payment gateway set to '{Provider}'", providerName);
        }
        else
        {
            throw new InvalidOperationException($"Cannot set default: provider '{providerName}' is not registered");
        }
    }
}
