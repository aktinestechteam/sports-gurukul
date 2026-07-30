using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Adapters;
using SportsGurukul.Platform.PaymentGateway.Factory;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class GatewayFactoryTests
{
    private readonly IPaymentSignatureValidator _validator;
    private readonly PaymentGatewayFactory _factory;

    public GatewayFactoryTests()
    {
        _validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);
        _factory = new PaymentGatewayFactory(NullLogger<PaymentGatewayFactory>.Instance);
    }

    [Fact]
    public void RegisterProvider_ShouldAddGateway()
    {
        var gateway = CreateMockGateway("TestProvider");
        _factory.RegisterProvider("TestProvider", gateway);

        Assert.True(_factory.IsProviderSupported("TestProvider"));
        Assert.Single(_factory.GetRegisteredProviders());
    }

    [Fact]
    public void GetGateway_ShouldReturnRegisteredGateway()
    {
        var gateway = CreateMockGateway("TestProvider");
        _factory.RegisterProvider("TestProvider", gateway);

        var result = _factory.GetGateway("TestProvider");
        Assert.Same(gateway, result);
    }

    [Fact]
    public void GetGateway_ShouldThrowForUnregisteredProvider()
    {
        Assert.Throws<InvalidOperationException>(() => _factory.GetGateway("NonExistent"));
    }

    [Fact]
    public void GetDefaultGateway_ShouldReturnFirstRegistered()
    {
        var gateway1 = CreateMockGateway("Provider1");
        var gateway2 = CreateMockGateway("Provider2");
        _factory.RegisterProvider("Provider1", gateway1);
        _factory.RegisterProvider("Provider2", gateway2);

        var defaultGw = _factory.GetDefaultGateway();
        Assert.Same(gateway1, defaultGw);
    }

    [Fact]
    public void SetDefaultProvider_ShouldOverrideDefault()
    {
        var gateway1 = CreateMockGateway("Provider1");
        var gateway2 = CreateMockGateway("Provider2");
        _factory.RegisterProvider("Provider1", gateway1);
        _factory.RegisterProvider("Provider2", gateway2);

        _factory.SetDefaultProvider("Provider2");
        Assert.Same(gateway2, _factory.GetDefaultGateway());
    }

    [Fact]
    public void OverwriteProvider_ShouldUpdateGateway()
    {
        var gateway1 = CreateMockGateway("Provider");
        var gateway2 = CreateMockGateway("Provider");
        _factory.RegisterProvider("Provider", gateway1);
        _factory.RegisterProvider("Provider", gateway2);

        Assert.Same(gateway2, _factory.GetGateway("Provider"));
    }

    private static IPaymentGateway CreateMockGateway(string name)
    {
        var config = new GatewayConfig { Provider = name, UseSandbox = true };
        var validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);

        return name switch
        {
            "Razorpay" => new RazorpayGateway(new HttpClient(), config, NullLogger<RazorpayGateway>.Instance, validator),
            "Stripe" => new StripeGateway(new HttpClient(), config, NullLogger<StripeGateway>.Instance, validator),
            "Cashfree" => new CashfreeGateway(new HttpClient(), config, NullLogger<CashfreeGateway>.Instance, validator),
            "PayU" => new PayUGateway(config, NullLogger<PayUGateway>.Instance, validator),
            "PayPal" => new PayPalGateway(new HttpClient(), config, NullLogger<PayPalGateway>.Instance, validator),
            _ => new RazorpayGateway(new HttpClient(), config, NullLogger<RazorpayGateway>.Instance, validator)
        };
    }
}
