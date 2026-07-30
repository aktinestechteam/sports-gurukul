using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.Rendering;

public class VariableResolverTests
{
    private readonly VariableResolver _resolver;

    public VariableResolverTests()
    {
        _resolver = new VariableResolver();
    }

    [Fact]
    public void Resolve_ShouldResolveSimpleVariables()
    {
        var variables = new Dictionary<string, string>
        {
            ["name"] = "John",
            ["age"] = "30"
        };

        var result = _resolver.Resolve(variables);

        result["name"].Should().Be("John");
        result["age"].Should().Be("30");
    }

    [Fact]
    public void Resolve_ShouldResolveNestedProperties()
    {
        var variables = new Dictionary<string, string>
        {
            ["user.name"] = "Alice",
            ["user.email"] = "alice@example.com"
        };

        var result = _resolver.Resolve(variables);

        result["user.name"].Should().Be("Alice");
        result["user.email"].Should().Be("alice@example.com");
    }

    [Fact]
    public void Resolve_ShouldResolveFromUserData()
    {
        var variables = new Dictionary<string, string>
        {
            ["firstName"] = "John",
            ["lastName"] = "Doe"
        };

        var result = _resolver.Resolve(variables);

        result["firstName"].Should().Be("John");
        result["lastName"].Should().Be("Doe");
    }

    [Fact]
    public void Resolve_ShouldResolveFromTemplateDefaults()
    {
        _resolver.RegisterGlobal("siteName", () => "SportsGurukul");
        _resolver.RegisterGlobal("year", () => DateTime.UtcNow.Year);

        var variables = new Dictionary<string, string>
        {
            ["title"] = "Welcome"
        };

        var result = _resolver.Resolve(variables);

        result["title"].Should().Be("Welcome");
        result["siteName"].Should().Be("SportsGurukul");
        result["year"].Should().Be(DateTime.UtcNow.Year);
    }

    [Fact]
    public void Resolve_ShouldReturnNowAndToday()
    {
        var result = _resolver.Resolve(new Dictionary<string, string>());

        result.Should().ContainKey("now");
        result["now"].Should().BeOfType<DateTime>();
        result.Should().ContainKey("today");
        result["today"].Should().BeOfType<DateTime>();
        result.Should().ContainKey("year");
        result["year"].Should().Be(DateTime.UtcNow.Year);
    }

    [Fact]
    public void Resolve_UserVariablesOverrideGlobals()
    {
        _resolver.RegisterGlobal("name", () => "Global");

        var result = _resolver.Resolve(new Dictionary<string, string> { ["name"] = "User" });

        result["name"].Should().Be("User");
    }

    [Fact]
    public void Resolve_ShouldHandleGlobalProviderException()
    {
        _resolver.RegisterGlobal("failing", () => throw new InvalidOperationException("Fail"));

        var result = _resolver.Resolve(new Dictionary<string, string>());

        result["failing"].Should().Be(string.Empty);
    }

    [Fact]
    public void Resolve_ShouldHandleEmptyVariables()
    {
        var result = _resolver.Resolve(new Dictionary<string, string>());

        result.Should().ContainKey("now");
        result.Should().ContainKey("today");
        result.Should().ContainKey("year");
    }

    [Fact]
    public void ResolveWithContext_ShouldMergeContext()
    {
        var variables = new Dictionary<string, string> { ["name"] = "John" };
        var context = new Dictionary<string, object>
        {
            ["role"] = "admin",
            ["permissions"] = new[] { "read", "write" }
        };

        var result = _resolver.ResolveWithContext(variables, context);

        result["name"].Should().Be("John");
        result["role"].Should().Be("admin");
        result["permissions"].Should().BeOfType<string[]>();
    }

    [Fact]
    public void ResolveWithContext_ContextOverridesVariables()
    {
        var variables = new Dictionary<string, string> { ["name"] = "Original" };
        var context = new Dictionary<string, object> { ["name"] = "Override" };

        var result = _resolver.ResolveWithContext(variables, context);

        result["name"].Should().Be("Override");
    }

    [Fact]
    public void ResolveFromDictionary_ShouldReturnValueIfExists()
    {
        var dict = new Dictionary<string, string> { ["key"] = "value" };

        var result = VariableResolver.ResolveFromDictionary("key", dict);

        result.Should().Be("value");
    }

    [Fact]
    public void ResolveFromDictionary_ShouldReturnEmptyIfNotExists()
    {
        var dict = new Dictionary<string, string> { ["key"] = "value" };

        var result = VariableResolver.ResolveFromDictionary("unknown", dict);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ResolveFromDictionary_ShouldBeCaseSensitive()
    {
        var dict = new Dictionary<string, string> { ["Key"] = "value" };

        var result = VariableResolver.ResolveFromDictionary("key", dict);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void RegisterGlobal_ShouldBeInvokedLazily()
    {
        var callCount = 0;
        _resolver.RegisterGlobal("lazy", () =>
        {
            callCount++;
            return "computed";
        });

        _resolver.Resolve(new Dictionary<string, string>());
        _resolver.Resolve(new Dictionary<string, string>());

        callCount.Should().Be(2);
    }
}
