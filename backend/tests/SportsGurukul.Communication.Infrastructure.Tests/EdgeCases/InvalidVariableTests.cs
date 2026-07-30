using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Rendering;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class InvalidVariableTests
{
    [Fact]
    public void Resolve_WithVariables_ReturnsResolvedDictionary()
    {
        var resolver = new VariableResolver();

        var result = resolver.Resolve(new Dictionary<string, string>
        {
            ["name"] = "John",
            ["email"] = "john@test.com"
        });

        result["name"].Should().Be("John");
        result["email"].Should().Be("john@test.com");
    }

    [Fact]
    public void Resolve_AlwaysIncludesNowTodayYear()
    {
        var resolver = new VariableResolver();

        var result = resolver.Resolve(new Dictionary<string, string>());

        result.Should().ContainKey("now");
        result.Should().ContainKey("today");
        result.Should().ContainKey("year");
    }

    [Fact]
    public void Resolve_WithGlobalProvider_AddsGlobalVariable()
    {
        var resolver = new VariableResolver();
        resolver.RegisterGlobal("siteName", () => "SportsGurukul");

        var result = resolver.Resolve(new Dictionary<string, string>());

        result["siteName"].Should().Be("SportsGurukul");
    }

    [Fact]
    public void Resolve_GlobalProviderDoesNotOverrideExplicitVariable()
    {
        var resolver = new VariableResolver();
        resolver.RegisterGlobal("name", () => "GlobalName");

        var result = resolver.Resolve(new Dictionary<string, string>
        {
            ["name"] = "ExplicitName"
        });

        result["name"].Should().Be("ExplicitName");
    }

    [Fact]
    public void Resolve_GlobalProviderThatThrows_ReturnsEmptyString()
    {
        var resolver = new VariableResolver();
        resolver.RegisterGlobal("faulty", () => throw new InvalidOperationException());

        var result = resolver.Resolve(new Dictionary<string, string>());

        result["faulty"].Should().Be(string.Empty);
    }

    [Fact]
    public void ResolveWithContext_MergesContextOverVariables()
    {
        var resolver = new VariableResolver();

        var result = resolver.ResolveWithContext(
            new Dictionary<string, string> { ["name"] = "John" },
            new Dictionary<string, object> { ["name"] = "ContextName", ["role"] = "admin" });

        result["name"].Should().Be("ContextName");
        result["role"].Should().Be("admin");
    }

    [Fact]
    public void ResolveFromDictionary_ExistingKey_ReturnsValue()
    {
        var result = VariableResolver.ResolveFromDictionary("key",
            new Dictionary<string, string> { ["key"] = "value" });

        result.Should().Be("value");
    }

    [Fact]
    public void ResolveFromDictionary_MissingKey_ReturnsEmptyString()
    {
        var result = VariableResolver.ResolveFromDictionary("missing",
            new Dictionary<string, string> { ["key"] = "value" });

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Resolve_CaseInsensitiveKeys()
    {
        var resolver = new VariableResolver();

        var result = resolver.Resolve(new Dictionary<string, string>
        {
            ["Name"] = "John"
        });

        result["name"].Should().Be("John");
        result["NAME"].Should().Be("John");
        result["Name"].Should().Be("John");
    }

    [Fact]
    public void Resolve_WithEmptyVariables_ReturnsBuiltins()
    {
        var resolver = new VariableResolver();

        var result = resolver.Resolve(new Dictionary<string, string>());

        result.Should().HaveCount(3);
        result.Keys.Should().Contain(new[] { "now", "today", "year" });
    }
}
