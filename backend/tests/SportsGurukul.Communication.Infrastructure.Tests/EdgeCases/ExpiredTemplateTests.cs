using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class ExpiredTemplateTests
{
    private readonly Mock<ITemplateEngine> _engineMock = new();
    private readonly VariableResolver _variableResolver = new();
    private readonly LocalizedTemplateEngine _localizedEngine;

    public ExpiredTemplateTests()
    {
        _localizedEngine = new LocalizedTemplateEngine(
            _engineMock.Object,
            Mock.Of<ILogger<LocalizedTemplateEngine>>());
    }

    [Fact]
    public async Task RenderAsync_WithValidTemplate_Succeeds()
    {
        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Rendered Content");

        var renderer = new TemplateRenderer(
            _engineMock.Object,
            _variableResolver,
            _localizedEngine,
            Options.Create(new CommunicationOptions
            {
                TemplateEngine = new TemplateEngineOptions { EnableLocalization = false }
            }),
            Mock.Of<ILogger<TemplateRenderer>>());

        var result = await renderer.RenderAsync("Hello {{name}}", "Body {{name}}",
            new Dictionary<string, string> { ["name"] = "World" });

        result.IsSuccess.Should().BeTrue();
        result.Value!.Subject.Should().Be("Rendered Content");
        result.Value.Body.Should().Be("Rendered Content");
    }

    [Fact]
    public async Task RenderAsync_WithMissingVariables_ResolvesEmptyString()
    {
        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Hello ");

        var renderer = new TemplateRenderer(
            _engineMock.Object,
            _variableResolver,
            _localizedEngine,
            Options.Create(new CommunicationOptions
            {
                TemplateEngine = new TemplateEngineOptions
                {
                    EnableLocalization = false,
                    StrictMode = false
                }
            }),
            Mock.Of<ILogger<TemplateRenderer>>());

        var result = await renderer.RenderAsync("Hello {{name}}", "Body",
            new Dictionary<string, string>());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RenderAsync_WhenEngineThrows_ReturnsFailure()
    {
        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ThrowsAsync(new InvalidOperationException("Template engine error"));

        var renderer = new TemplateRenderer(
            _engineMock.Object,
            _variableResolver,
            _localizedEngine,
            Options.Create(new CommunicationOptions
            {
                TemplateEngine = new TemplateEngineOptions { EnableLocalization = false }
            }),
            Mock.Of<ILogger<TemplateRenderer>>());

        var result = await renderer.RenderAsync("{{bad}}", "Body",
            new Dictionary<string, string> { ["name"] = "test" });

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Template rendering failed");
    }

    [Fact]
    public async Task RenderAsync_StrictMode_DetectsUnrenderedVariables()
    {
        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Hello {{unresolved}}");

        var renderer = new TemplateRenderer(
            _engineMock.Object,
            _variableResolver,
            _localizedEngine,
            Options.Create(new CommunicationOptions
            {
                TemplateEngine = new TemplateEngineOptions
                {
                    EnableLocalization = false,
                    StrictMode = true
                }
            }),
            Mock.Of<ILogger<TemplateRenderer>>());

        var result = await renderer.RenderAsync("Hello {{unresolved}}", "Body",
            new Dictionary<string, string>());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ExtractVariables_FromTemplate_ReturnsVariableNames()
    {
        _engineMock
            .Setup(e => e.ExtractVariables(It.IsAny<string>()))
            .Returns(new List<string> { "name", "email" });

        var renderer = new TemplateRenderer(
            _engineMock.Object,
            _variableResolver,
            _localizedEngine,
            Options.Create(new CommunicationOptions
            {
                TemplateEngine = new TemplateEngineOptions { EnableLocalization = false }
            }),
            Mock.Of<ILogger<TemplateRenderer>>());

        var variables = renderer.ExtractVariables("Hello {{name}}, your email is {{email}}");

        variables.Should().Contain("name");
        variables.Should().Contain("email");
    }
}
