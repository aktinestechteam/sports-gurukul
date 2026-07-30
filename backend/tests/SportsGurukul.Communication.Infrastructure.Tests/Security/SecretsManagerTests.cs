using Microsoft.Extensions.Configuration;
using SportsGurukul.Platform.Communication.Security;

namespace SportsGurukul.Communication.Infrastructure.Tests.Security;

public class SecretsManagerTests
{
    [Fact]
    public void GetSecret_RetrievesSecret()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Secrets:SendGridApiKey"] = "sg-secret-key"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetSecret("SendGridApiKey");
        result.Should().Be("sg-secret-key");
    }

    [Fact]
    public void GetSecret_FallsBackToProviderApiKey()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Providers:SendGrid:ApiKey"] = "provider-api-key"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetSecret("SendGrid");
        result.Should().Be("provider-api-key");
    }

    [Fact]
    public void GetSecret_ReturnsNull_ForUnknownKey()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetSecret("NonExistentKey");
        result.Should().BeNull();
    }

    [Fact]
    public void GetProviderApiKey_RetrievesByProviderName()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Secrets:Twilio:ApiKey"] = "twilio-api-key"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderApiKey("Twilio");
        result.Should().Be("twilio-api-key");
    }

    [Fact]
    public void GetProviderApiKey_FallsBackToApiSecret()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Secrets:Twilio:ApiSecret"] = "twilio-api-secret"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderApiKey("Twilio");
        result.Should().Be("twilio-api-secret");
    }

    [Fact]
    public void GetProviderApiKey_FallsBackToSettingsApiKey()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Providers:Twilio:Settings:ApiKey"] = "settings-api-key"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderApiKey("Twilio");
        result.Should().Be("settings-api-key");
    }

    [Fact]
    public void GetProviderApiKey_ReturnsNull_WhenNotFound()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderApiKey("Unknown");
        result.Should().BeNull();
    }

    [Fact]
    public void GetProviderSecret_RetrievesSetting()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Secrets:Twilio:AuthToken"] = "auth-token-value"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderSecret("Twilio", "AuthToken");
        result.Should().Be("auth-token-value");
    }

    [Fact]
    public void GetProviderSecret_FallsBackToSettings()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Providers:Twilio:Settings:AuthToken"] = "settings-auth-token"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderSecret("Twilio", "AuthToken");
        result.Should().Be("settings-auth-token");
    }

    [Fact]
    public void HasSecret_ReturnsTrue_WhenKeyExists()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Secrets:ApiKey"] = "exists"
            })
            .Build();

        var manager = new SecretsManager(config);
        manager.HasSecret("ApiKey").Should().BeTrue();
    }

    [Fact]
    public void HasSecret_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var manager = new SecretsManager(config);
        manager.HasSecret("Missing").Should().BeFalse();
    }

    [Fact]
    public void GetProviderSecrets_ReturnsAllSettings()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Communication:Providers:Twilio:Settings:ApiKey"] = "key1",
                ["Communication:Providers:Twilio:Settings:AuthToken"] = "token1",
                ["Communication:Providers:Twilio:Settings:AccountSid"] = "sid1"
            })
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderSecrets("Twilio");

        result.Should().ContainKey("ApiKey");
        result.Should().ContainKey("AuthToken");
        result.Should().ContainKey("AccountSid");
        result["ApiKey"].Should().Be("key1");
    }

    [Fact]
    public void GetProviderSecrets_ReturnsEmpty_ForUnknownProvider()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var manager = new SecretsManager(config);
        var result = manager.GetProviderSecrets("Unknown");

        result.Should().BeEmpty();
    }
}
