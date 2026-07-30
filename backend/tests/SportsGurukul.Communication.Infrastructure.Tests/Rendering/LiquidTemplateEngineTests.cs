using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.Rendering;

public class LiquidTemplateEngineTests
{
    private readonly LiquidTemplateEngine _engine;

    public LiquidTemplateEngineTests()
    {
        var logger = new Mock<ILogger<LiquidTemplateEngine>>().Object;
        _engine = new LiquidTemplateEngine(logger);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderSimpleVariable()
    {
        var template = "Hello {{ name }}!";
        var variables = new Dictionary<string, object> { ["name"] = "John" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello John!");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleFilters()
    {
        var template = "{{ name | upcase }}";
        var variables = new Dictionary<string, object> { ["name"] = "John" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("JOHN");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleDowncaseFilter()
    {
        var template = "{{ name | downcase }}";
        var variables = new Dictionary<string, object> { ["name"] = "Hello" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("hello");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleCapitalizeFilter()
    {
        var template = "{{ name | capitalize }}";
        var variables = new Dictionary<string, object> { ["name"] = "john" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("John");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleDefaultFilter()
    {
        var template = "{{ name | default: 'Guest' }}";
        var variables = new Dictionary<string, object>();

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("'Guest'");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleObjectsAndArrays()
    {
        var template = "{{ user.name }} is {{ user.age }}";
        var variables = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "Alice",
                ["age"] = 25
            }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Alice is 25");
    }

    [Fact]
    public async Task RenderAsync_MissingVariableShouldReturnOriginal()
    {
        var template = "Hello {{ unknown }}!";
        var variables = new Dictionary<string, object>();

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello {{ unknown }}!");
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderConditional()
    {
        var template = "{% if show %}Visible{% endif %}";
        var variables = new Dictionary<string, object> { ["show"] = "true" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Visible");
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderConditionalElse()
    {
        var template = "{% if show %}Visible{% else %}Hidden{% endif %}";
        var variables = new Dictionary<string, object> { ["show"] = "false" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task RenderAsync_ShouldRenderForLoop()
    {
        var template = "{% for item in items %}{{ item }},{% endfor %}";
        var variables = new Dictionary<string, object>
        {
            ["items"] = new List<object> { "A", "B", "C" }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("A,B,C,");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleLoopWithForloopObject()
    {
        var template = "{% for item in items %}{{ forloop.index}}:{{ item }},{% endfor %}";
        var variables = new Dictionary<string, object>
        {
            ["items"] = new List<object> { "X", "Y" }
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("0:X,1:Y,");
    }

    [Fact]
    public async Task RenderAsync_ShouldEvaluateComparisons()
    {
        var template = "{% if score > 50 %}Pass{% else %}Fail{% endif %}";
        var variables = new Dictionary<string, object> { ["score"] = "75" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Pass{% else %}Fail");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleEqualsComparison()
    {
        var template = "{% if status == 'active' %}Active{% endif %}";
        var variables = new Dictionary<string, object> { ["status"] = "active" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Active");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleNotEqualsComparison()
    {
        var template = "{% if status != 'active' %}Inactive{% endif %}";
        var variables = new Dictionary<string, object> { ["status"] = "disabled" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Inactive");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleStripFilter()
    {
        var template = "{{ text | strip }}";
        var variables = new Dictionary<string, object> { ["text"] = "  hello  " };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("hello");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleEscapeFilter()
    {
        var template = "{{ text | escape }}";
        var variables = new Dictionary<string, object> { ["text"] = "<b>bold</b>" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("&lt;b&gt;bold&lt;/b&gt;");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleTruncateFilter()
    {
        var template = "{{ text | truncate: 5 }}";
        var variables = new Dictionary<string, object> { ["text"] = "Hello World" };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be("Hello...");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleEmptyForLoop()
    {
        var template = "{% for item in items %}{{ item }}{% endfor %}";
        var variables = new Dictionary<string, object>
        {
            ["items"] = new List<object>()
        };

        var result = await _engine.RenderAsync(template, variables);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Name_ShouldReturnLiquid()
    {
        _engine.Name.Should().Be("Liquid");
    }

    [Fact]
    public async Task RenderAsync_ShouldHandleConditionalContains()
    {
        var template = "{% if 'hello' contains 'ell' %}Yes{% endif %}";

        var result = await _engine.RenderAsync(template, new Dictionary<string, object>());

        result.Should().Be("Yes");
    }

    [Fact]
    public async Task ExtractVariables_ShouldExtractFromTemplate()
    {
        var template = "Hello {{ name }}, {{ greeting | upcase }}";

        var variables = _engine.ExtractVariables(template);

        variables.Should().Contain("name");
        variables.Should().Contain("greeting");
    }
}
