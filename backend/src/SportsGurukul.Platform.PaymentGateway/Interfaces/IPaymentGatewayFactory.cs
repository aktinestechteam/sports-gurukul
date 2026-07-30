namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentGatewayFactory
{
    IPaymentGateway GetGateway(string providerName);
    IPaymentGateway GetDefaultGateway();
    IReadOnlyCollection<string> GetRegisteredProviders();
    bool IsProviderSupported(string providerName);
    void RegisterProvider(string providerName, IPaymentGateway gateway);
}
