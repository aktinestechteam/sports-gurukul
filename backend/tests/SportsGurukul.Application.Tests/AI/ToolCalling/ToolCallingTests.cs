using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Features.AIManagement.ToolCalling;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.ToolCalling;

public class DefaultToolRegistryTests
{
    private readonly DefaultToolRegistry _registry = new();

    private static ToolDescriptor CreateTool(string name = "weather", bool isSystem = false, bool requiresApproval = false)
        => new(Guid.NewGuid(), name, AIToolType.Http, "Get weather", "{}", "{}", isSystem, requiresApproval, null);

    [Fact]
    public void RegisterAndGet_ReturnsRegisteredTool()
    {
        var tool = CreateTool();

        _registry.Register(tool);

        _registry.Get("weather").Should().Be(tool);
    }

    [Fact]
    public void Get_UnregisteredTool_ReturnsNull()
    {
        _registry.Get("missing").Should().BeNull();
    }

    [Fact]
    public void Unregister_ExistingTool_ReturnsTrue()
    {
        _registry.Register(CreateTool());

        _registry.Unregister("weather").Should().BeTrue();
        _registry.Get("weather").Should().BeNull();
    }

    [Fact]
    public void Contains_RegisteredTool_ReturnsTrue()
    {
        _registry.Register(CreateTool());

        _registry.Contains("weather").Should().BeTrue();
    }
}

public class ToolAuthorizationServiceTests
{
    private readonly ToolAuthorizationService _service = new();

    private static ToolDescriptor CreateTool(bool isSystem, bool requiresApproval)
        => new(Guid.NewGuid(), "weather", AIToolType.Http, null, null, null, isSystem, requiresApproval, null);

    private static ToolCallContext CreateContext(Guid? userId = null) => new(null, null, userId, null, null);

    [Fact]
    public async Task AuthorizeAsync_ToolRequiresApproval_Fails()
    {
        var result = await _service.AuthorizeAsync(CreateTool(false, true), CreateContext(Guid.NewGuid()));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("approval");
    }

    [Fact]
    public async Task AuthorizeAsync_SystemTool_IsAuthorized()
    {
        var result = await _service.AuthorizeAsync(CreateTool(true, false), CreateContext());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_NonSystemToolWithoutUser_Fails()
    {
        var result = await _service.AuthorizeAsync(CreateTool(false, false), CreateContext());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("authenticated user");
    }

    [Fact]
    public async Task AuthorizeAsync_NonSystemToolWithUser_IsAuthorized()
    {
        var result = await _service.AuthorizeAsync(CreateTool(false, false), CreateContext(Guid.NewGuid()));

        result.IsSuccess.Should().BeTrue();
    }
}

public class ToolExecutorTests
{
    private readonly Mock<IToolResolver> _resolverMock = new();
    private readonly Mock<IToolAuthorizationService> _authMock = new();
    private readonly Mock<ILogger<ToolExecutor>> _loggerMock = new();
    private readonly ToolExecutor _executor;

    public ToolExecutorTests()
    {
        _executor = new ToolExecutor(_resolverMock.Object, _authMock.Object, _loggerMock.Object);
    }

    private static ToolCallRequest CreateRequest() => new("weather", "{}", new ToolCallContext(null, null, Guid.NewGuid(), null, null));

    [Fact]
    public async Task ExecuteAsync_UnregisteredTool_Fails()
    {
        _resolverMock.Setup(r => r.ResolveAsync("weather", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ToolDescriptor?)null);

        var result = await _executor.ExecuteAsync("weather", CreateRequest());

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not registered");
    }

    [Fact]
    public async Task ExecuteAsync_ApprovalRequired_ReturnsApprovalRequired()
    {
        var tool = new ToolDescriptor(Guid.NewGuid(), "weather", AIToolType.Http, null, null, null, false, true, null);
        _resolverMock.Setup(r => r.ResolveAsync("weather", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        _authMock.Setup(a => a.AuthorizeAsync(tool, It.IsAny<ToolCallContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SportsGurukul.Application.Common.Models.Result<bool>.Failure("Tool requires approval before execution"));

        var result = await _executor.ExecuteAsync("weather", CreateRequest());

        result.IsSuccess.Should().BeFalse();
        result.RequiresApproval.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesRegisteredExecutor()
    {
        var tool = new ToolDescriptor(
            Guid.NewGuid(), "weather", AIToolType.Http, null, null, null, false, false,
            (request, ct) => Task.FromResult(ToolCallResult.Success("{\"temp\":22}", 10, 0.0001m)));
        _resolverMock.Setup(r => r.ResolveAsync("weather", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        _authMock.Setup(a => a.AuthorizeAsync(tool, It.IsAny<ToolCallContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SportsGurukul.Application.Common.Models.Result<bool>.Success(true));

        var result = await _executor.ExecuteAsync("weather", CreateRequest());

        result.IsSuccess.Should().BeTrue();
        result.OutputJson.Should().Be("{\"temp\":22}");
        result.Cost.Should().Be(0.0001m);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutorThrows_ReturnsFailure()
    {
        var tool = new ToolDescriptor(
            Guid.NewGuid(), "weather", AIToolType.Http, null, null, null, false, false,
            (request, ct) => throw new InvalidOperationException("boom"));
        _resolverMock.Setup(r => r.ResolveAsync("weather", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        _authMock.Setup(a => a.AuthorizeAsync(tool, It.IsAny<ToolCallContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SportsGurukul.Application.Common.Models.Result<bool>.Success(true));

        var result = await _executor.ExecuteAsync("weather", CreateRequest());

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("boom");
    }
}

public class ToolResolverTests
{
    private readonly DefaultToolRegistry _registry = new();
    private readonly Mock<IAgentRepository> _agentRepoMock = new();
    private readonly ToolResolver _resolver;

    public ToolResolverTests()
    {
        _resolver = new ToolResolver(_registry, _agentRepoMock.Object);
    }

    [Fact]
    public async Task ResolveAsync_ReturnsRegisteredTool()
    {
        var tool = new ToolDescriptor(Guid.NewGuid(), "weather", AIToolType.Http, null, null, null, true, false, null);
        _registry.Register(tool);

        var resolved = await _resolver.ResolveAsync("weather");

        resolved.Should().Be(tool);
    }

    [Fact]
    public async Task ResolveAsync_UnknownTool_ReturnsNull()
    {
        var resolved = await _resolver.ResolveAsync("missing");

        resolved.Should().BeNull();
    }

    [Fact]
    public async Task ResolveForAgentAsync_ReturnsActiveToolsAndSystemTools()
    {
        var agent = new AgentDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Scout",
        };
        var tool = new ToolDefinition
        {
            Id = Guid.NewGuid(),
            Name = "drill-lookup",
            ToolType = AIToolType.Retriever,
            IsActive = true,
            RequiresApproval = false,
        };
        agent.Tools.Add(tool);
        _agentRepoMock.Setup(r => r.GetByIdWithToolsAsync(agent.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);
        var systemTool = new ToolDescriptor(Guid.NewGuid(), "system-note", AIToolType.System, null, null, null, true, false, null);
        _registry.Register(systemTool);

        var tools = await _resolver.ResolveForAgentAsync(agent.Id);

        tools.Should().Contain(t => t.Name == "drill-lookup");
        tools.Should().Contain(t => t.Name == "system-note");
    }

    [Fact]
    public async Task ResolveForAgentAsync_MissingAgent_ReturnsEmpty()
    {
        _agentRepoMock.Setup(r => r.GetByIdWithToolsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AgentDefinition?)null);

        var tools = await _resolver.ResolveForAgentAsync(Guid.NewGuid());

        tools.Should().BeEmpty();
    }
}
