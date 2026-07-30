using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Providers;
using SportsGurukul.Platform.Communication.Providers.Email;
using SportsGurukul.Platform.Communication.Providers.Sms;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers;

public class NotificationProviderFactoryTests
{
    private sealed class MockEmailProvider : INotificationProvider
    {
        public string Name => "MockEmail";
        public NotificationChannelType ChannelType => NotificationChannelType.Email;
        public bool IsAvailable => true;
        public Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken ct = default)
            => Task.FromResult(new ProviderSendResult { IsSuccess = true });
        public Task<bool> HealthCheckAsync(CancellationToken ct = default)
            => Task.FromResult(true);
    }

    private sealed class MockSmsProvider : INotificationProvider
    {
        public string Name => "MockSms";
        public NotificationChannelType ChannelType => NotificationChannelType.SMS;
        public bool IsAvailable => true;
        public Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken ct = default)
            => Task.FromResult(new ProviderSendResult { IsSuccess = true });
        public Task<bool> HealthCheckAsync(CancellationToken ct = default)
            => Task.FromResult(true);
    }

    private sealed class UnavailableProvider : INotificationProvider
    {
        public string Name => "Unavailable";
        public NotificationChannelType ChannelType => NotificationChannelType.Email;
        public bool IsAvailable => false;
        public Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken ct = default)
            => Task.FromResult(new ProviderSendResult { IsSuccess = false });
        public Task<bool> HealthCheckAsync(CancellationToken ct = default)
            => Task.FromResult(false);
    }

    [Fact]
    public void GetProvider_ShouldReturnCorrectProviderForChannelType()
    {
        var emailProvider = new MockEmailProvider();
        var smsProvider = new MockSmsProvider();
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { emailProvider, smsProvider },
            options,
            logger);

        var emailResult = factory.GetProvider(NotificationChannelType.Email);
        emailResult.Should().BeSameAs(emailProvider);

        var smsResult = factory.GetProvider(NotificationChannelType.SMS);
        smsResult.Should().BeSameAs(smsProvider);
    }

    [Fact]
    public void GetProvider_ShouldThrowForUnknownChannel()
    {
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            Array.Empty<INotificationProvider>(),
            options,
            logger);

        var act = () => factory.GetProvider(NotificationChannelType.Email);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*No active provider found*");
    }

    [Fact]
    public void GetProvider_ShouldReturnCachedInstanceOnRepeatedCall()
    {
        var emailProvider = new MockEmailProvider();
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { emailProvider },
            options,
            logger);

        var first = factory.GetProvider(NotificationChannelType.Email);
        var second = factory.GetProvider(NotificationChannelType.Email);

        first.Should().BeSameAs(emailProvider);
        second.Should().BeSameAs(emailProvider);
    }

    [Fact]
    public void GetProvidersForChannel_ShouldReturnAvailableProviders()
    {
        var available = new MockEmailProvider();
        var unavailable = new UnavailableProvider();
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { available, unavailable },
            options,
            logger);

        var emailProviders = factory.GetProvidersForChannel(NotificationChannelType.Email);

        emailProviders.Should().Contain(available);
        emailProviders.Should().NotContain(unavailable);
    }

    [Fact]
    public void GetProviderByName_ShouldReturnMatchingProvider()
    {
        var emailProvider = new MockEmailProvider();
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { emailProvider },
            options,
            logger);

        var found = factory.GetProviderByName("MockEmail");
        found.Should().BeSameAs(emailProvider);
    }

    [Fact]
    public void GetProviderByName_ShouldReturnNullForUnknown()
    {
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            Array.Empty<INotificationProvider>(),
            options,
            logger);

        factory.GetProviderByName("NonExistent").Should().BeNull();
    }

    [Fact]
    public void GetAllProviders_ShouldReturnAllRegistered()
    {
        var emailProvider = new MockEmailProvider();
        var smsProvider = new MockSmsProvider();
        var options = Options.Create(new CommunicationOptions());
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { emailProvider, smsProvider },
            options,
            logger);

        var all = factory.GetAllProviders();
        all.Should().HaveCount(2);
        all.Should().Contain(emailProvider);
        all.Should().Contain(smsProvider);
    }

    [Fact]
    public void Constructor_ShouldSkipDisabledProviders()
    {
        var emailProvider = new MockEmailProvider();
        var options = Options.Create(new CommunicationOptions
        {
            Providers = new ProviderOptions
            {
                Providers = new Dictionary<string, ProviderConfig>
                {
                    ["MockEmail"] = new() { IsActive = false }
                }
            }
        });
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { emailProvider },
            options,
            logger);

        var act = () => factory.GetProvider(NotificationChannelType.Email);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetProvider_ShouldRespectPriority()
    {
        var lowPriority = new ProviderMock("LowPriority", NotificationChannelType.Email, priority: 100);
        var highPriority = new ProviderMock("HighPriority", NotificationChannelType.Email, priority: 1);

        var options = Options.Create(new CommunicationOptions
        {
            Providers = new ProviderOptions
            {
                Providers = new Dictionary<string, ProviderConfig>
                {
                    ["LowPriority"] = new() { IsActive = true, Priority = 100 },
                    ["HighPriority"] = new() { IsActive = true, Priority = 1 }
                }
            }
        });
        var logger = new Mock<ILogger<NotificationProviderFactory>>().Object;

        var factory = new NotificationProviderFactory(
            new INotificationProvider[] { lowPriority, highPriority },
            options,
            logger);

        var selected = factory.GetProvider(NotificationChannelType.Email);
        selected.Name.Should().Be("LowPriority");
    }

    private class ProviderMock : INotificationProvider
    {
        public string Name { get; }
        public NotificationChannelType ChannelType { get; }
        public bool IsAvailable => true;
        private readonly int _priority;

        public ProviderMock(string name, NotificationChannelType channel, int priority)
        {
            Name = name;
            ChannelType = channel;
            _priority = priority;
        }

        public Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken ct = default)
            => Task.FromResult(new ProviderSendResult { IsSuccess = true });

        public Task<bool> HealthCheckAsync(CancellationToken ct = default)
            => Task.FromResult(true);
    }
}
