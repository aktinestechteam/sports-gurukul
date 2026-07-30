using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.Rendering;

public class LocalizedTemplateEngineTests
{
    private readonly Mock<ITemplateEngine> _innerEngineMock;
    private readonly Mock<ILogger<LocalizedTemplateEngine>> _loggerMock;
    private readonly LocalizedTemplateEngine _engine;

    public LocalizedTemplateEngineTests()
    {
        _innerEngineMock = new Mock<ITemplateEngine>();
        _loggerMock = new Mock<ILogger<LocalizedTemplateEngine>>();
        _engine = new LocalizedTemplateEngine(_innerEngineMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RenderLocalizedAsync_ShouldSelectCorrectLocale()
    {
        _engine.RegisterTranslations("fr", new Dictionary<string, string>
        {
            ["greeting"] = "Bonjour",
            ["farewell"] = "Au revoir"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderLocalizedAsync("{t greeting}, John!", new Dictionary<string, object>(), "fr");

        result.Should().Be("Bonjour, John!");
    }

    [Fact]
    public async Task RenderLocalizedAsync_ShouldFallbackToDefaultLocale()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string>
        {
            ["greeting"] = "Hello"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderLocalizedAsync("{t greeting}, John!", new Dictionary<string, object>(), "fr");

        result.Should().Be("{t greeting}, John!");
    }

    [Fact]
    public async Task RenderWithLocaleDetectionAsync_ShouldUsePreferredLocale()
    {
        _engine.RegisterTranslations("es", new Dictionary<string, string>
        {
            ["hello"] = "Hola"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderWithLocaleDetectionAsync("{t hello}!", new Dictionary<string, object>(), "es");

        result.Should().Be("Hola!");
    }

    [Fact]
    public async Task RenderWithLocaleDetectionAsync_ShouldFallbackToEn()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string>
        {
            ["hello"] = "Hello"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderWithLocaleDetectionAsync("{t hello}!", new Dictionary<string, object>(), "de");

        result.Should().Be("Hello!");
    }

    [Fact]
    public async Task RenderWithLocaleDetectionAsync_ShouldUseDefaultLocaleWhenNull()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string>
        {
            ["welcome"] = "Welcome"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderWithLocaleDetectionAsync("{t welcome}!", new Dictionary<string, object>(), null);

        result.Should().Be("Welcome!");
    }

    [Fact]
    public async Task RenderLocalizedAsync_ShouldCacheLocalizedTemplates()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string>
        {
            ["subject"] = "Notification"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        await _engine.RenderLocalizedAsync("{t subject}", new Dictionary<string, object>(), "en");
        await _engine.RenderLocalizedAsync("{t subject}", new Dictionary<string, object>(), "en");

        _innerEngineMock.Verify(
            e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task RenderLocalizedAsync_ShouldHandleMissingLocaleGracefully()
    {
        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderLocalizedAsync("Hello {t name}", new Dictionary<string, object>(), "zz");

        result.Should().Be("Hello {t name}");
    }

    [Fact]
    public async Task RenderLocalizedAsync_ShouldReplaceMultipleTranslations()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string>
        {
            ["greeting"] = "Hello",
            ["name"] = "John"
        });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderLocalizedAsync("{t greeting}, {t name}!", new Dictionary<string, object>(), "en");

        result.Should().Be("Hello, John!");
    }

    [Fact]
    public async Task RegisterTranslations_ShouldOverrideExisting()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string> { ["key"] = "Old" });
        _engine.RegisterTranslations("en", new Dictionary<string, string> { ["key"] = "New" });

        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync((string t, IReadOnlyDictionary<string, object> _) => t);

        var result = await _engine.RenderLocalizedAsync("{t key}", new Dictionary<string, object>(), "en");

        result.Should().Be("New");
    }

    [Fact]
    public async Task RenderLocalizedAsync_ShouldPassVariablesToInnerEngine()
    {
        _engine.RegisterTranslations("en", new Dictionary<string, string>
        {
            ["greeting"] = "Hello"
        });

        IReadOnlyDictionary<string, object>? capturedVariables = null;
        _innerEngineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .Callback<string, IReadOnlyDictionary<string, object>>((_, v) => capturedVariables = v)
            .ReturnsAsync((string _, IReadOnlyDictionary<string, object> v) => v["name"]?.ToString() ?? string.Empty);

        var result = await _engine.RenderLocalizedAsync("{t greeting} {name}", new Dictionary<string, object>
        {
            ["name"] = "Alice"
        }, "en");

        capturedVariables.Should().ContainKey("name");
        capturedVariables!["name"].Should().Be("Alice");
    }
}
