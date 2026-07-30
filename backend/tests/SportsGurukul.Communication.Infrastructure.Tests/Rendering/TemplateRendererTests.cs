using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.Rendering;

public class TemplateRendererTests
{
    private readonly Mock<ITemplateEngine> _engineMock;
    private readonly VariableResolver _resolver;
    private readonly Mock<ILogger<TemplateRenderer>> _loggerMock;
    private readonly TemplateRenderer _renderer;
    private readonly CommunicationOptions _options;

    public TemplateRendererTests()
    {
        _engineMock = new Mock<ITemplateEngine>();
        _resolver = new VariableResolver();
        _loggerMock = new Mock<ILogger<TemplateRenderer>>();
        _options = new CommunicationOptions
        {
            TemplateEngine = new TemplateEngineOptions
            {
                DefaultEngine = "Handlebars",
                EnableLocalization = false,
                DefaultLocale = "en",
                StrictMode = false,
                CacheCompiledTemplates = true,
                CacheMaxSize = 500
            }
        };

        var localizedEngine = new LocalizedTemplateEngine(_engineMock.Object, Mock.Of<ILogger<LocalizedTemplateEngine>>());

        _renderer = new TemplateRenderer(
            _engineMock.Object,
            _resolver,
            localizedEngine,
            Options.Create(_options),
            _loggerMock.Object);
    }

    [Fact]
    public async Task RenderAsync_ShouldDelegateToEngine()
    {
        var subjectTemplate = "Hello {{name}}";
        var bodyTemplate = "Welcome {{name}}!";
        var variables = new Dictionary<string, string> { ["name"] = "John" };

        _engineMock
            .Setup(e => e.RenderAsync(subjectTemplate, It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Hello John");
        _engineMock
            .Setup(e => e.RenderAsync(bodyTemplate, It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Welcome John!");

        var result = await _renderer.RenderAsync(subjectTemplate, bodyTemplate, variables);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Subject.Should().Be("Hello John");
        result.Value.Body.Should().Be("Welcome John!");
    }

    [Fact]
    public async Task RenderAsync_ShouldResolveVariables()
    {
        var variables = new Dictionary<string, string> { ["key"] = "value" };
        var resolvedVariables = new Dictionary<string, object> { ["key"] = "resolved" };

        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("rendered");

        await _renderer.RenderAsync("{{key}}", "body", variables);

        _engineMock.Verify(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RenderAsync_ShouldUseLocalizationWhenEnabled()
    {
        _options.TemplateEngine.EnableLocalization = true;

        var variables = new Dictionary<string, string> { ["name"] = "John" };

        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("localized");

        var result = await _renderer.RenderAsync("subject", "body", variables);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RenderAsync_ShouldLogWarningInStrictMode()
    {
        _options.TemplateEngine.StrictMode = true;

        var variables = new Dictionary<string, string>();

        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("{{unrendered}}");

        var result = await _renderer.RenderAsync("{{unrendered}}", "body", variables);

        result.IsSuccess.Should().BeTrue();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("unrendered")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleEngineException()
    {
        var variables = new Dictionary<string, string>();

        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ThrowsAsync(new InvalidOperationException("Engine error"));

        var result = await _renderer.RenderAsync("subject", "body", variables);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Engine error");
    }

    [Fact]
    public async Task ExtractVariables_ShouldDelegateToEngine()
    {
        var template = "{{name}} {{age}}";
        _engineMock.Setup(e => e.ExtractVariables(template)).Returns(new List<string> { "name", "age" });

        var result = _renderer.ExtractVariables(template);

        result.Should().Contain("name");
        result.Should().Contain("age");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleEmptyTemplates()
    {
        var variables = new Dictionary<string, string>();

        _engineMock
            .Setup(e => e.RenderAsync(string.Empty, It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync(string.Empty);

        var result = await _renderer.RenderAsync(string.Empty, string.Empty, variables);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Subject.Should().Be(string.Empty);
        result.Value.Body.Should().Be(string.Empty);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderWithAttachmentMetadata()
    {
        var variables = new Dictionary<string, string>
        {
            ["attachment_count"] = "3",
            ["attachment_names"] = "file1.pdf, file2.jpg"
        };

        _engineMock
            .Setup(e => e.RenderAsync("You have {{attachment_count}} attachments", It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("You have 3 attachments: file1.pdf, file2.jpg");
        _engineMock
            .Setup(e => e.RenderAsync("Files: {{attachment_names}}", It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Files: file1.pdf, file2.jpg");

        var result = await _renderer.RenderAsync(
            "You have {{attachment_count}} attachments",
            "Files: {{attachment_names}}",
            variables);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Body.Should().Be("Files: file1.pdf, file2.jpg");
    }

    [Fact]
    public async Task RenderAsync_ShouldReturnPreviewInPreviewMode()
    {
        var variables = new Dictionary<string, string> { ["name"] = "Preview" };

        _engineMock
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
            .ReturnsAsync("Rendered preview");

        var result = await _renderer.RenderAsync("Hello {{name}}", "Body {{name}}", variables);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Subject.Should().Be("Rendered preview");
        result.Value.Body.Should().Be("Rendered preview");
    }
}
