using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.Rendering;

public class HandlebarsTemplateEngineTests
{
    private readonly HandlebarsTemplateEngine _engine;

    public HandlebarsTemplateEngineTests()
    {
        var logger = new Mock<ILogger<HandlebarsTemplateEngine>>().Object;
        _engine = new HandlebarsTemplateEngine(logger);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderSimpleVariableSubstitution()
    {
        var template = "Hello, {{name}}!";
        var variables = new Dictionary<string, object> { ["name"] = "John" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello, John!");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleNestedObjects()
    {
        var template = "{{user.name}} is {{user.age}} years old";
        var variables = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["age"] = 30
            }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("John is 30 years old");
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderConditionalSections()
    {
        var template = "{{#if showGreeting}}Hello!{{/if}}";
        var variables = new Dictionary<string, object> { ["showGreeting"] = "true" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello!");
    }

    [Fact]
    public async Task RenderAsync_ShouldOmitConditionalWhenFalse()
    {
        var template = "{{#if showGreeting}}Hello!{{/if}}";
        var variables = new Dictionary<string, object> { ["showGreeting"] = "false" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderElseBlock()
    {
        var template = "{{#if isAdmin}}Admin{{else}}User{{/if}}";
        var variables = new Dictionary<string, object> { ["isAdmin"] = "false" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderLoops()
    {
        var template = "{{#each items}}{{this}},{{/each}}";
        var variables = new Dictionary<string, object>
        {
            ["items"] = new List<object> { "A", "B", "C" }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("A,B,C,");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleMissingVariablesGracefully()
    {
        var template = "Hello, {{name}}!";
        var variables = new Dictionary<string, object>();

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello, {{name}}!");
    }

    [Fact]
    public async Task RenderAsync_ShouldUseDefaultValues()
    {
        var template = "Hello, {{name|Guest}}!";
        var variables = new Dictionary<string, object>();

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello, Guest!");
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderLoopWithItemContext()
    {
        var template = "{{#each items}}{{name}},{{/each}}";
        var variables = new Dictionary<string, object>
        {
            ["items"] = new List<object>
            {
                new Dictionary<string, object> { ["name"] = "Alice" },
                new Dictionary<string, object> { ["name"] = "Bob" }
            }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Alice,Bob,");
    }

    [Fact]
    public async Task RenderAsync_ShouldResolveNestedPropertyOnObject()
    {
        var template = "{{person.name}}";
        var variables = new Dictionary<string, object>
        {
            ["person"] = new Dictionary<string, object> { ["name"] = "Alice" }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Alice");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleNegatedConditional()
    {
        var template = "{{#if !isHidden}}Visible{{/if}}";
        var variables = new Dictionary<string, object> { ["isHidden"] = "false" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Visible");
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderConditionalWithEmptyStringAsFalse()
    {
        var template = "{{#if value}}HasValue{{else}}NoValue{{/if}}";
        var variables = new Dictionary<string, object> { ["value"] = "" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task ExtractVariables_ShouldExtractSimpleVariables()
    {
        var template = "Hello {{name}}, your {{item}} is ready";

        var variables = _engine.ExtractVariables(template);

        variables.Should().Contain("name");
        variables.Should().Contain("item");
    }

    [Fact]
    public async Task ExtractVariables_ShouldExtractEachBlockVariables()
    {
        var template = "{{#each items}}{{name}}{{/each}}";

        var variables = _engine.ExtractVariables(template);

        variables.Should().Contain("items");
    }

    [Fact]
    public void Name_ShouldReturnHandlebars()
    {
        _engine.Name.Should().Be("Handlebars");
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderEmptyTemplate()
    {
        var result = await _engine.RenderAsync(string.Empty, new Dictionary<string, object>());
        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleMultipleVariables()
    {
        var template = "{{greeting}}, {{name}}! You have {{count}} messages.";
        var variables = new Dictionary<string, object>
        {
            ["greeting"] = "Welcome",
            ["name"] = "Alice",
            ["count"] = 5
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Welcome, Alice! You have 5 messages.");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleLoopWithIndex()
    {
        var template = "{{#each items}}[{{@index}}:{{this}}]{{/each}}";
        var variables = new Dictionary<string, object>
        {
            ["items"] = new List<object> { "X", "Y" }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("[0:X][1:Y]");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandlePartialsAsEmpty()
    {
        var template = "Start.{{> header}}End";
        var variables = new Dictionary<string, object>();

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Start.End");
    }
}
